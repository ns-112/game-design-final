using UnityEngine;

public enum ItemWeight { 
	Normal, 
    Heavy 
}

public class ItemPickup : MonoBehaviour
{
  // decides if the item can only be picked up by heavy or both
  public ItemWeight itemWeight = ItemWeight.Normal;

  // marks if the item is the final goal to steal
  public bool isTargetItem = false;

  // tracks if a player is currently within pickup range
  private bool playerInRange = false;
  private PlayerType playerInRangeType;

  // saved on Start so we can return the item here if the floor countdown expires
  private Vector3 originalPosition;

  void Start()
  {
    originalPosition = transform.position;
  }

  // checks for interact input each frame when a player is in range
  void Update()
  {
    PlayerType active = PlayerManager.Instance.ActivePlayer;
    Player activePlayer = PlayerManager.Instance.Players[active];

    if (activePlayer.heldItem == this)
    {
      if (PlayerManager.Instance.interact.WasPressedThisFrame())
        Drop(activePlayer);
    }
    else if (playerInRange && activePlayer.heldItem == null && CanPickUp(active))
    {
      if (PlayerManager.Instance.interact.WasPressedThisFrame())
        PickUp(activePlayer);
    }
  }

  // returns true if the player type is allowed to pick up this item
  // heavy items restricted to heavy player, normal items for both
  bool CanPickUp(PlayerType type)
  {
    if (itemWeight == ItemWeight.Heavy)
      return type == PlayerType.Player1;
    return true;
  }

  // attaches the item to the player, disables its physics, then starts the escape timer if it's the target item
  void PickUp(Player player)
  {
    player.heldItem = this;
    transform.SetParent(player.gameObject.transform);
    transform.localPosition = new Vector3(0f, 1f, 0f);
    GetComponent<Rigidbody2D>().simulated = false;

    // cancel floor return timer since the item is being carried again
    EscapeTimer.Instance.CancelFloorCountdown();

    if (isTargetItem)
      EscapeTimer.Instance.StartTimer();
  }

  // detaches the item from the player and enables physics again
  // if the target item is dropped during escape, starts the 10 second floor countdown
  void Drop(Player player)
  {
    player.heldItem = null;
    transform.SetParent(null);
    GetComponent<Rigidbody2D>().simulated = true;

    if (isTargetItem && EscapeTimer.Instance.TimerRunning)
      EscapeTimer.Instance.StartFloorCountdown(this, originalPosition);
  }

  // detects when a player enters the trigger collider surrounding this item
  void OnTriggerEnter2D(Collider2D other)
  {
    BasicPlayer bp = other.GetComponent<BasicPlayer>();
    if (bp != null)
    {
      playerInRange = true;
      playerInRangeType = bp.playerType;
    }
  }

  // clears the in range flag when the player leaves the trigger area
  void OnTriggerExit2D(Collider2D other)
  {
    BasicPlayer bp = other.GetComponent<BasicPlayer>();
    if (bp != null)
    {
      playerInRange = false;
    }
  }

  // forcibly returns the item to its original spawn position
  // clears heldItem from whichever player was carrying it
  public void ReturnToOrigin()
  {
    if (PlayerManager.Instance.Players.TryGetValue(PlayerType.Player1, out Player heavy) && heavy.heldItem == this)
      heavy.heldItem = null;
    if (PlayerManager.Instance.Players.TryGetValue(PlayerType.Player2, out Player light) && light.heldItem == this)
      light.heldItem = null;

    transform.SetParent(null);
    transform.position = originalPosition;
    GetComponent<Rigidbody2D>().simulated = true;
  }
}