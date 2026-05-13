using Unity.VisualScripting;
using UnityEngine;



public class EscapeZone : MonoBehaviour
{
	public ToLevel toLevel = ToLevel.Level1;
  // triggered when a player enters the exit zone
  	void Start()
	{
		MoneySystem.Instance.toLevel = toLevel;
	}
  void OnTriggerEnter2D(Collider2D other)
  {
	other.TryGetComponent(out BasicPlayer bp);
	if (bp == null) return;

	PlayerType type = bp.playerType;

	// check if this player is holding the target item
	if (PlayerManager.Instance.Players.TryGetValue(type, out Player player))
	{
		if (player.heldItem != null && player.heldItem.isTargetItem)
		{
			GameLevelManager.Instance.Camera.GetComponent<CameraSmoothFollow>().PauseCamera = true;
			GameLevelManager.Instance.Camera.GetComponent<CameraSmoothFollow>().CameraReady = false;

			//Deestroy object
			Destroy(player.heldItem.gameObject);
			EscapeTimer.Instance.StopTimer();
			LevelComplete();
		}
	}
  }

  // called when the player escapes with the target item
	void LevelComplete()
	{
		GameLevelManager.Instance.nextLevel = toLevel;
		MoneySystem.Instance.LevelComplete();
	}
}