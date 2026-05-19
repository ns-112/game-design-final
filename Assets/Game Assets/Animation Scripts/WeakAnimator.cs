using UnityEngine;

public class WeakAnimator : MonoBehaviour
{
  [SerializeField] private Transform spriteRoot;

  private Animator animator;
  private Rigidbody2D rb;
  private Vector3 originalScale;

  void Start()
  {
    spriteRoot = GetComponent<Transform>();
    animator = GetComponentInChildren<Animator>();
    rb = GetComponent<Rigidbody2D>();
    originalScale = spriteRoot.localScale;
  }

  void Update()
  {
    Player player = PlayerManager.Instance.Players[PlayerType.Player2];

    bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    bool isCarrying = player.heldItem != null;

    animator.SetBool("isRunning", isRunning);
    animator.SetBool("isCarrying", isCarrying);

    if (rb.linearVelocity.x > 0.1f)
      spriteRoot.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    else if (rb.linearVelocity.x < -0.1f)
      spriteRoot.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
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