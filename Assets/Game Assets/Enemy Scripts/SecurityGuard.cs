using System.Collections;
using UnityEngine;

public class SecurityGuard : MonoBehaviour
{
  // the two patrol points the guard walks between, set in Inspector
  public Transform patrolPointA;
  public Transform patrolPointB;

  // vision cone settings
  public float visionRange = 6f;
  public float visionAngle = 60f;

  // movement speeds
  public float patrolSpeed = 2f;
  public float chaseSpeed = 4f;

  // how long guard is suspicious before fully alerted
  public float suspicionTime = 2f;

  // how long guard stays knocked out
  public float knockoutDuration = 10f;

  // how long guard pauses at each patrol point
  public float patrolPauseDuration = 1.5f;

  private enum GuardState { Patrolling, Suspicious, Chasing, KnockedOut }
  private GuardState state = GuardState.Patrolling;

  private Rigidbody2D rb;
  private Transform currentTarget;
  private float suspicionTimer = 0f;
  private bool isPausing = false;

  void Start()
  {
    rb = GetComponent<Rigidbody2D>();

    if (patrolPointA != null)
      currentTarget = patrolPointA;
  }

  void Update()
  {
    switch (state)
    {
      case GuardState.Patrolling:
        Patrol();
        CheckVisionCone();
        break;

      case GuardState.Suspicious:
        BeSuspicious();
        break;

      case GuardState.Chasing:
        Chase();
        break;

      case GuardState.KnockedOut:
        // handled by coroutine
        break;
    }
  }

  // walks between patrol point A and B, pausing at each end
  void Patrol()
  {
    if (isPausing || currentTarget == null) return;

    Vector2 direction = (currentTarget.position - transform.position).normalized;
    rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);

    // flip sprite direction based on movement
    if (direction.x > 0)
      transform.localScale = new Vector3(1, 1, 1);
    else if (direction.x < 0)
      transform.localScale = new Vector3(-1, 1, 1);

    // close enough to patrol point, switch to the other one
    if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
    {
      rb.linearVelocity = Vector2.zero;
      currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;
      StartCoroutine(PatrolPause());
    }
  }

  // pauses at each patrol point before moving again
  IEnumerator PatrolPause()
  {
    isPausing = true;
    yield return new WaitForSeconds(patrolPauseDuration);
    isPausing = false;
  }

  // checks if any player is within the vision cone
  void CheckVisionCone()
  {
    foreach (var entry in PlayerManager.Instance.Players)
    {
      GameObject playerObj = entry.Value.gameObject;
      Vector2 directionToPlayer = (playerObj.transform.position - transform.position).normalized;
      float distToPlayer = Vector2.Distance(transform.position, playerObj.transform.position);

      if (distToPlayer > visionRange) continue;

      // check if player is within the vision angle
      float angle = Vector2.Angle(transform.right * transform.localScale.x, directionToPlayer);
      if (angle <= visionAngle / 2f)
      {
        state = GuardState.Suspicious;
        suspicionTimer = 0f;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Guard: Player spotted, suspicious");
        return;
      }
    }
  }

  // counts up while player is in cone, goes to chasing when timer hits suspicionTime
  void BeSuspicious()
  {
    suspicionTimer += Time.deltaTime;

    bool playerInCone = false;

    foreach (var entry in PlayerManager.Instance.Players)
    {
      GameObject playerObj = entry.Value.gameObject;
      Vector2 directionToPlayer = (playerObj.transform.position - transform.position).normalized;
      float distToPlayer = Vector2.Distance(transform.position, playerObj.transform.position);

      if (distToPlayer > visionRange) continue;

      float angle = Vector2.Angle(transform.right * transform.localScale.x, directionToPlayer);
      if (angle <= visionAngle / 2f)
      {
        playerInCone = true;
        break;
      }
    }

    // player left the cone before timer finished
    if (!playerInCone)
    {
      state = GuardState.Patrolling;
      suspicionTimer = 0f;
      Debug.Log("Guard: Lost sight, patrolling.");
      return;
    }

    if (suspicionTimer >= suspicionTime)
    {
      state = GuardState.Chasing;
      Debug.Log("Guard: chasing");
    }
  }

  // chases the active player
  void Chase()
  {
    GameObject target = PlayerManager.Instance.Players[PlayerManager.Instance.ActivePlayer].gameObject;
    Vector2 direction = (target.transform.position - transform.position).normalized;
    float dist = Vector2.Distance(transform.position, target.transform.position);

    if (dist > visionRange * 1.5f)
    {
      state = GuardState.Patrolling;
      rb.linearVelocity = Vector2.zero;
      Debug.Log("Guard: Lost the player, back to patrolling");
      return;
    }

    // get the left and right boundary from patrol points
    float leftBound = Mathf.Min(patrolPointA.position.x, patrolPointB.position.x);
    float rightBound = Mathf.Max(patrolPointA.position.x, patrolPointB.position.x);

    // stop if guard would move past either boundary
    if (transform.position.x <= leftBound && direction.x < 0)
    {
      rb.linearVelocity = Vector2.zero;
      return;
    }

    if (transform.position.x >= rightBound && direction.x > 0)
    {
      rb.linearVelocity = Vector2.zero;
      return;
    }

    rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

    if (direction.x > 0)
      transform.localScale = new Vector3(1, 1, 1);
    else if (direction.x < 0)
      transform.localScale = new Vector3(-1, 1, 1);
  }

  // called by MeleeAttack.cs when heavy player punches the guard
  public void KnockOut()
  {
    if (state == GuardState.KnockedOut) return;
    StartCoroutine(KnockedOutTimer());
  }

  // disables the guard for knockoutDuration seconds
  IEnumerator KnockedOutTimer()
  {
    state = GuardState.KnockedOut;
    rb.linearVelocity = Vector2.zero;
    Debug.Log("Guard: Knocked out");
    yield return new WaitForSeconds(knockoutDuration);
    state = GuardState.Patrolling;
    Debug.Log("Guard: Back on patrol");
  }

  // draws vision cone and patrol points in editor for tuning
  void OnDrawGizmosSelected()
  {
    // vision range circle
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, visionRange);

    // vision cone lines
    Vector3 facing = transform.right * transform.localScale.x;
    Vector3 coneLeft = Quaternion.Euler(0, 0, visionAngle / 2f) * facing * visionRange;
    Vector3 coneRight = Quaternion.Euler(0, 0, -visionAngle / 2f) * facing * visionRange;
    Gizmos.color = Color.cyan;
    Gizmos.DrawLine(transform.position, transform.position + coneLeft);
    Gizmos.DrawLine(transform.position, transform.position + coneRight);
  }
}