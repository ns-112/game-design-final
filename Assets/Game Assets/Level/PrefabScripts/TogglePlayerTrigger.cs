using UnityEngine;
public enum ToggleState
{
    Toggle,
    Activate,
    Deactivate,
    Swap
}

public class TogglePlayerTrigger : PlayerTrigger
{
    public PlayerType TogglePlayer;
    public ToggleState State;
    public override void ActivateTrigger()
    {
        
        switch (State)
        {
            case ToggleState.Activate:
            PlayerManager.Instance.Players[TogglePlayer].characterActive = true;
            break;

            case ToggleState.Deactivate:
            PlayerManager.Instance.Players[TogglePlayer].characterActive = false;
            break;

            case ToggleState.Toggle:
            PlayerManager.Instance.Players[TogglePlayer].characterActive = !PlayerManager.Instance.Players[TogglePlayer].characterActive;
            break;

            case ToggleState.Swap:
            if (PlayerManager.Instance.ActivePlayer == PlayerType.Player1)
            {
                PlayerManager.Instance.Players[PlayerType.Player2].characterActive = true;
                PlayerManager.Instance.Players[PlayerType.Player1].characterActive = false;
            } else
            {
                PlayerManager.Instance.Players[PlayerType.Player2].characterActive = false;
                PlayerManager.Instance.Players[PlayerType.Player1].characterActive = true;
            }
            break;
        }
        
    }
}
