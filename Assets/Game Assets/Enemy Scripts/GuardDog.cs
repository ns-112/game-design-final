using System.Collections;
using UnityEngine;

public class GuardDog : MonoBehaviour
{
  // distance the dog patrols left and right from its spawn point
  public float patrolRadius = 3f;

  // distance at which the dog detects and starts chasing a player
  public float detectionRadius = 5f;

  // movement speeds
  public float patrolSpeed = 2f;
  public float chaseSpeed = 4f;

  // how long the dog backs off after hitting a player
  public float backoffDuration = 2f;

  // how long the dog is knocked out after being hit by the heavy player
  public float knockoutDuration = 10f;

  // damage dealt to player on contact
  public float damage = 10f;

  private Vector3 spawnPoint;
  private Rigidbody2D rb;

  private enum DogState { Patrolling, Chasing, BackingOff, KnockedOut }
  private DogState state = DogState.Patrolling;

  // tracks which direction the dog is patrolling
  private float patrolDirection = 1f;

  void Start()
  {
    spawnPoint = transform.position;
    rb = GetComponent<Rigidbody2D>();
  }

  void Update()
  {
    switch (state)
    {
      case DogState.Patrolling:
        Patrol();
        CheckForPlayer();
        break;

      case DogState.Chasing:
        ChasePlayer();
        break;

      case DogState.BackingOff:
      case DogState.KnockedOut:
        break;
    }
  }

  // walks left and right within patrolRadius from spawn point
  void Patrol()
  {
    transform.Translate(Vector2.right * patrolDirection * patrolSpeed * Time.deltaTime);

    if (transform.position.x >= spawnPoint.x + patrolRadius)
      patrolDirection = -1f;
    else if (transform.position.x <= spawnPoint.x - patrolRadius)
      patrolDirection = 1f;
  }

  // checks if either player is within detection radius
  void CheckForPlayer()
  {
    foreach (var entry in PlayerManager.Instance.Players)
    {
      float dist = Vector2.Distance(transform.position, entry.Value.gameObject.transform.position);
      if (dist <= detectionRadius)
      {
        state = DogState.Chasing;
        return;
      }
    }
  }

  // chases the active player, goes back to patrolling if player leaves detection radius
  void ChasePlayer()
  {
    GameObject target = PlayerManager.Instance.Players[PlayerManager.Instance.ActivePlayer].gameObject;
    float dist = Vector2.Distance(transform.position, target.transform.position);

    if (dist > detectionRadius)
    {
      state = DogState.Patrolling;
      return;
    }

    Vector2 direction = (target.transform.position - transform.position).normalized;
    rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
  }

  // called when dog touches a player
  void OnCollisionEnter2D(Collision2D collision)
  {
    if (state == DogState.KnockedOut || state == DogState.BackingOff) return;

    BasicPlayer bp = collision.gameObject.GetComponent<BasicPlayer>();
    if (bp != null)
    {
      Player player = PlayerManager.Instance.Players[bp.playerType];
      MoneySystem.Instance.TakeDamage();
      StartCoroutine(BackOff());
    }
  }

  // backs the dog off for 2 seconds then returns to patrolling
  IEnumerator BackOff()
  {
    state = DogState.BackingOff;
    rb.linearVelocity = new Vector2(-patrolDirection * chaseSpeed, rb.linearVelocity.y);
    yield return new WaitForSeconds(backoffDuration);
    rb.linearVelocity = Vector2.zero;
    state = DogState.Patrolling;
  }

  // called by the heavy player melee attack to knock the dog out
  public void KnockOut()
  {
    if (state == DogState.KnockedOut) return;
    StartCoroutine(KnockedOutTimer());
  }

  // disables the dog for knockoutDuration seconds
  IEnumerator KnockedOutTimer()
  {
    state = DogState.KnockedOut;
    rb.linearVelocity = Vector2.zero;
    yield return new WaitForSeconds(knockoutDuration);
    state = DogState.Patrolling;
  }
}