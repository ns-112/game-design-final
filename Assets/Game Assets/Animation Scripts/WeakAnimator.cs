using UnityEngine;

public class WeakAnimator : MonoBehaviour
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
    Player player = PlayerManager.Instance.Players[PlayerType.Player2];

    bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    bool isCarrying = player.heldItem != null;

    animator.SetBool("isRunning", isRunning);
    animator.SetBool("isCarrying", isCarrying);
  }

  public void TriggerHurt()
  {
    animator.SetTrigger("isHurt");
  }

  public void TriggerInteract()
  {
    animator.SetTrigger("isInteracting");
  }
}