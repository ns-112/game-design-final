using UnityEngine;

public class HeavyAnimator : MonoBehaviour
{
  private Animator animator;
  private Rigidbody2D rb;

  void Start()
  {
    animator = GetComponentInChildren<Animator>();
    rb = GetComponent<Rigidbody2D>();
  }

  void Update()
  {
    Player player = PlayerManager.Instance.Players[PlayerType.Player1];

    float xVelocity = rb.linearVelocity.x;
    bool isRunning = Mathf.Abs(xVelocity) > 0.1f;
    bool isCarrying = player.heldItem != null;

    animator.SetBool("isRunning", isRunning);
    animator.SetBool("isCarrying", isCarrying);

    // Debug
    Debug.Log($"isRunning={isRunning} isCarrying={isCarrying} vel={xVelocity}");
  }

  public void TriggerHurt()
  {
    animator.SetTrigger("isHurt");
  }

  public void TriggerKick()
  {
    animator.SetTrigger("isKicking");
  }
}