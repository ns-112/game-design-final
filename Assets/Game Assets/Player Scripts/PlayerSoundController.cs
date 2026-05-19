using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
  [SerializeField] private PlayerType playerType;

  [SerializeField] private AudioClip jumpSound;
  [SerializeField] private AudioClip footstepSound;
  [SerializeField] private AudioClip hurtSound;
  [SerializeField] private AudioClip pickUpSound;
  [SerializeField] private AudioClip actionSound;

  [SerializeField] private float footstepInterval = 0.3f;

  private AudioSource audioSource;
  private Rigidbody2D rb;
  private Player player;

  private float footstepTimer = 0f;
  private bool wasGrounded = false;
  private bool wasHoldingItem = false;

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    rb = GetComponent<Rigidbody2D>();
    player = PlayerManager.Instance.Players[playerType];
  }

  void Update()
  {
    player = PlayerManager.Instance.Players[playerType];

    HandleFootsteps();
    HandlePickUp();
  }

  void HandleFootsteps()
  {
    bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    bool isGrounded = player.canJump;

    if (isRunning && isGrounded)
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

  void HandlePickUp()
  {
    bool isHoldingItem = player.heldItem != null;

    if (isHoldingItem && !wasHoldingItem)
      PlaySound(pickUpSound);

    wasHoldingItem = isHoldingItem;
  }

  public void PlayActionSound()
  {
    PlaySound(actionSound);
  }

  public void PlayHurtSound()
  {
    PlaySound(hurtSound);
  }

  public void PlayJumpSound()
  {
    PlaySound(jumpSound);
  }

  void PlaySound(AudioClip clip)
  {
    if (clip != null && audioSource != null)
      audioSource.PlayOneShot(clip);
  }
}