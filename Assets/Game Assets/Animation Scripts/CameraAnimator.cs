using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
  [SerializeField] private AudioClip detectSound;
  [SerializeField] private AudioClip alertSound;
  [SerializeField] private AudioClip hackedSound;

  private Animator animator;
  private AudioSource audioSource;
  private SecurityCamera cam;

  private SecurityCamera.CameraState lastState;

  void Start()
  {
    animator = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();
    cam = GetComponent<SecurityCamera>();
  }

  void Update()
  {
    SecurityCamera.CameraState currentState = cam.GetState();

    bool isDetecting = currentState == SecurityCamera.CameraState.Detecting;
    bool isAlerted = currentState == SecurityCamera.CameraState.Alerted;
    bool isHacked = currentState == SecurityCamera.CameraState.Hacked;

    animator.SetBool("isDetecting", isDetecting);
    animator.SetBool("isAlerted", isAlerted);
    animator.SetBool("isHacked", isHacked);

    HandleStateChangeSounds(currentState);

    lastState = currentState;
  }

  void HandleStateChangeSounds(SecurityCamera.CameraState currentState)
  {
    if (currentState == lastState) return;

    switch (currentState)
    {
      case SecurityCamera.CameraState.Detecting:
        PlaySound(detectSound);
        break;

      case SecurityCamera.CameraState.Alerted:
        PlaySound(alertSound);
        break;

      case SecurityCamera.CameraState.Hacked:
        PlaySound(hackedSound);
        break;
    }
  }

  void PlaySound(AudioClip clip)
  {
    if (clip != null && audioSource != null)
      audioSource.PlayOneShot(clip);
  }
}