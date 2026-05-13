using UnityEngine;

public class OverlayScript : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void OnEnterStart()
    {
        GameLevelManager.Instance.CurrentTransitionState = TransitionState.Entering;
    }

    public void OnTransitionEnterComplete()
    {
        GameLevelManager.Instance.CurrentTransitionState = TransitionState.ReadyForLabels;
    }

    public void OnExitStart()
    {
        GameLevelManager.Instance.CurrentTransitionState = TransitionState.Exiting;
    }

    public void OnTransitionExitComplete()
    {
        GameLevelManager.Instance.CurrentTransitionState = TransitionState.ReadyToEnter;
    }
}
