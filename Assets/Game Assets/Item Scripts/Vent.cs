// Lets the heavy character open a vent by pressing E while standing on the enter side.
// Once opened, the vent stays open permanently so the weak character can pass through.
//
// Two Vent modes:
// VentMode.OneWay: players can enter but not exit back out. The exit blocker re-enables
//                  once a player has passed through to the other side.
//
// VentMode.TwoWay: both sides open and any player can pass in either direction.
//
// For setup in Unity:
//   Parent: Vent game object
//   -  BoxCollider2D with Is Trigger on sized to cover the vent opening
//      Child: EnterSide
//      -   BoxCollider2D with Is Trigger off sized at the entrance, assign to Vent EnterSide
//      Child: ExitSide
//         -  BoxCollider2D with Is Trigger off sized at the exit side of the vent, assign to Vent ExitSide
//         -  BoxCollider2D with Is Trigger on sized at the exit side of the vent, assign to Vent ExitSideTrigger

using UnityEngine;

public class Vent : MonoBehaviour
{
  public enum VentMode { OneWay, TwoWay }

  [SerializeField] private VentMode ventMode = VentMode.OneWay;

  [SerializeField] private Collider2D ventEnterSide;
  [SerializeField] private Collider2D ventExitSide;
  [SerializeField] private Collider2D ventExitSideTrigger;

  private bool isOpen = false;
  private bool playerInRange = false;

  void Update()
  {
    if (playerInRange && !isOpen && IsHeavyPlayerActive() && IsOnEnterSide())
    {
      if (PlayerManager.Instance.interact.triggered)
        OpenVent();
    }
  }

  void OpenVent()
  {
    isOpen = true;

    if (ventEnterSide != null)
      ventEnterSide.enabled = false;

    if (ventExitSide != null)
      ventExitSide.enabled = false;
  }

  void CloseVent()
  {
    isOpen = false;

    if (ventEnterSide != null)
      ventEnterSide.enabled = true;

    if (ventExitSide != null)
      ventExitSide.enabled = true;
  }

  bool IsHeavyPlayerActive()
  {
    if (PlayerManager.Instance == null) return false;
    return PlayerManager.Instance.ActivePlayer == PlayerType.Player1;
  }

  bool IsOnEnterSide()
  {
    if (ventMode == VentMode.TwoWay) return true;

    GameObject playerObj = PlayerManager.Instance.Players[PlayerType.Player1].gameObject;
    if (playerObj == null) return false;

    return playerObj.transform.position.x < transform.position.x;
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (!other.CompareTag("Player")) return;
    playerInRange = true;
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if (!other.CompareTag("Player")) return;

    if (isOpen && ventMode == VentMode.OneWay && ventExitSideTrigger != null)
    {
      if (other.transform.position.x > transform.position.x)
      {
        if (ventExitSide != null)
          ventExitSide.enabled = true;
      }
    }

    playerInRange = false;
  }
}