// When a player holding an item tagged SafeKey walks into the safe, the key is destroyed
// and the safe opens switching to its open sprite and spawning the level target item
// at the safe center which then drops to the floor.
//
// For setup in Unity:
//   - Create Safe game object and assign Safe.cs script
//   - Add a BoxCollider2D with Is Trigger on to the safe object
//   - Assign the SpriteRenderer on the safe to the Safe1 sprite
//   - Assign the Safe1 sprite in the SpriteRenderer to the Safe Sprite field  
//   - Assign the Safe4 sprite to Safe Open Sprite
//   - Assign the target item prefab to Target Item Prefab
//   - Create a game object with item pickup and tag it with SafeKey for the key to open the safe

using UnityEngine;

public class Safe : MonoBehaviour
{
  [SerializeField] private SpriteRenderer safeSprite;
  [SerializeField] private Sprite safeOpenSprite;
  [SerializeField] private GameObject targetItemPrefab;

  private bool isOpen = false;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (isOpen || !other.CompareTag("Player")) return;

    PlayerType playerType = other.GetComponent<BasicPlayer>().playerType;
    Player player = PlayerManager.Instance.Players[playerType];

    if (player.heldItem != null && player.heldItem.CompareTag("SafeKey"))
    {
      Destroy(player.heldItem.gameObject);
      player.heldItem = null;
      OpenSafe();
    }
  }

  void OpenSafe()
  {
    isOpen = true;

    if (safeSprite != null && safeOpenSprite != null)
      safeSprite.sprite = safeOpenSprite;

    if (targetItemPrefab != null)
      Instantiate(targetItemPrefab, transform.position, Quaternion.identity);
  }
}