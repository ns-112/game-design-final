using UnityEngine;

public class GuardAnimator : MonoBehaviour
{
  [SerializeField] private AudioClip alertSound;
  [SerializeField] private AudioClip footstepSound;
  [SerializeField] private float footstepInterval = 0.35f;

  private Animator animator;
  private AudioSource audioSource;
  private Rigidbody2D rb;
  private SecurityGuard guard;

  private float footstepTimer = 0f;
  private SecurityGuard.GuardState lastState;

  void Start()
  {
    animator = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();
    rb = GetComponent<Rigidbody2D>();
    guard = GetComponent<SecurityGuard>();
  }

  void Update()
  {
    SecurityGuard.GuardState currentState = guard.GetState();

    bool isRunning = currentState == SecurityGuard.GuardState.Chasing && Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    bool isAlert = currentState == SecurityGuard.GuardState.Suspicious || currentState == SecurityGuard.GuardState.Chasing;
    bool isFrozen = currentState == SecurityGuard.GuardState.KnockedOut;

    animator.SetBool("isRunning", isRunning);
    animator.SetBool("isAlert", isAlert);
    animator.SetBool("isFrozen", isFrozen);

    HandleFootsteps(isRunning);
    HandleStateChangeSounds(currentState);

    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (rb.linearVelocity.x > 0.1f)
      sr.flipX = false;
    else if (rb.linearVelocity.x < -0.1f)
      sr.flipX = true;

    lastState = currentState;
  }

  void HandleFootsteps(bool isRunning)
  {
    if (isRunning)
    {
      footstepTimer -= Time.deltaTime;
      if (footstepTimer <= 0f)
      {
        PlaySound(footstepSound);
        footstepTimer = footstepInterval;
      }
    }
    else
    {
      footstepTimer = 0f;
    }
  }

  void HandleStateChangeSounds(SecurityGuard.GuardState currentState)
  {
    if (currentState == lastState) return;

    if (currentState == SecurityGuard.GuardState.Suspicious || currentState == SecurityGuard.GuardState.Chasing)
      PlaySound(alertSound);
  }

  void PlaySound(AudioClip clip)
  {
    if (clip != null && audioSource != null)
      audioSource.PlayOneShot(clip);
  }
}