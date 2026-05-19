using UnityEngine;

public class DogAnimator : MonoBehaviour
{
  [SerializeField] private AudioClip barkSound;
  [SerializeField] private float barkInterval = 2f;

  private Animator animator;
  private AudioSource audioSource;
  private Rigidbody2D rb;
  private GuardDog dog;

  private float barkTimer = 0f;
  private GuardDog.DogState lastState;

  void Start()
  {
    animator = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();
    rb = GetComponent<Rigidbody2D>();
    dog = GetComponent<GuardDog>();
  }

  void Update()
  {
    GuardDog.DogState currentState = dog.GetState();

    bool isWalking = currentState == GuardDog.DogState.Patrolling && Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    bool isRunning = currentState == GuardDog.DogState.Chasing;
    bool isAlert = currentState == GuardDog.DogState.Chasing || currentState == GuardDog.DogState.BackingOff;
    bool isFrozen = currentState == GuardDog.DogState.KnockedOut;

    animator.SetBool("isWalking", isWalking);
    animator.SetBool("isRunning", isRunning);
    animator.SetBool("isAlert", isAlert);
    animator.SetBool("isFrozen", isFrozen);

    HandleBark(currentState);

    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (rb.linearVelocity.x > 0.1f)
      sr.flipX = false;
    else if (rb.linearVelocity.x < -0.1f)
      sr.flipX = true;

    lastState = currentState;
  }

  void HandleBark(GuardDog.DogState currentState)
  {
    if (currentState != GuardDog.DogState.Chasing)
    {
      barkTimer = 0f;
      return;
    }

    barkTimer -= Time.deltaTime;
    if (barkTimer <= 0f)
    {
      PlaySound(barkSound);
      barkTimer = barkInterval;
    }
  }

  void PlaySound(AudioClip clip)
  {
    if (clip != null && audioSource != null)
      audioSource.PlayOneShot(clip);
  }
}