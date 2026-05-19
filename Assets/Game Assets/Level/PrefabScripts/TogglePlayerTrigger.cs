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
            if (TogglePlayer == PlayerType.Player1) GameLevelManager.Instance.currentLevel.StartData.P1Active = true;
            else GameLevelManager.Instance.currentLevel.StartData.P2Active = true;
            Debug.Log($"Player activated: {TogglePlayer}");
            break;

            case ToggleState.Deactivate:
            PlayerManager.Instance.Players[TogglePlayer].characterActive = false;
            if (TogglePlayer == PlayerType.Player1) GameLevelManager.Instance.currentLevel.StartData.P1Active = false;
            else GameLevelManager.Instance.currentLevel.StartData.P2Active = false;
            Debug.Log($"Player deactivated: {TogglePlayer}");
            break;

            case ToggleState.Toggle:
            PlayerManager.Instance.Players[TogglePlayer].characterActive = !PlayerManager.Instance.Players[TogglePlayer].characterActive;
            if (TogglePlayer == PlayerType.Player1) GameLevelManager.Instance.currentLevel.StartData.P1Active = !GameLevelManager.Instance.currentLevel.StartData.P1Active;
            else GameLevelManager.Instance.currentLevel.StartData.P2Active = !GameLevelManager.Instance.currentLevel.StartData.P2Active;
            Debug.Log($"Player toggled: {TogglePlayer}");
            break;

            case ToggleState.Swap:
            if (PlayerManager.Instance.ActivePlayer == PlayerType.Player1)
            {
                PlayerManager.Instance.Players[PlayerType.Player2].characterActive = true;
                PlayerManager.Instance.Players[PlayerType.Player1].characterActive = false;
                GameLevelManager.Instance.currentLevel.StartData.P1Active = false;
                GameLevelManager.Instance.currentLevel.StartData.P2Active = true;
            } else
            {
                PlayerManager.Instance.Players[PlayerType.Player2].characterActive = false;
                PlayerManager.Instance.Players[PlayerType.Player1].characterActive = true;
                GameLevelManager.Instance.currentLevel.StartData.P1Active = true;
                GameLevelManager.Instance.currentLevel.StartData.P2Active = false;
            }
            break;
        }
        
    }
}
