using UnityEngine;

public class EscapeZone : MonoBehaviour
{
  // triggered when a player enters the exit zone
  void OnTriggerEnter2D(Collider2D other)
  {
    BasicPlayer bp = other.GetComponent<BasicPlayer>();
    if (bp == null) return;

    PlayerType type = bp.playerType;

    // check if this player is holding the target item
    if (PlayerManager.Instance.Players.TryGetValue(type, out Player player))
    {
      if (player.heldItem != null && player.heldItem.isTargetItem)
      {
        EscapeTimer.Instance.StopTimer();
        LevelComplete();
      }
    }
  }

  // called when the player escapes with the target item
  void LevelComplete()
  {
    MoneySystem.Instance.LevelComplete();
    Debug.Log("Level Complete!");
  }
}