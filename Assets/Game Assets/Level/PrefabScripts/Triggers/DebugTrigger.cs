using UnityEngine;

public class DebugTrigger : PlayerTrigger
{
    public override void ActivateTrigger()
    {
        Debug.Log("Trigger activated");
    }
}
