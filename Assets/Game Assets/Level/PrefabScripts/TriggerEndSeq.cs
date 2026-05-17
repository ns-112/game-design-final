using UnityEngine;

public class TriggerEndSeq : PlayerTrigger
{
    public override void ActivateTrigger()
    {
        GameLevelManager.Instance.TransitionPanelAnimator.SetBool("GameOver", true);
    }
}
