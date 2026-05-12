using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
  // detection radius for noticing players
  public float detectionRadius = 5f;

  // how long before camera goes from detecting to alerted
  public float detectionTime = 4f;

  // enemy prefab to spawn when alerted, drag in from Inspector
  public GameObject enemyPrefab;

  // how many enemies to spawn on alert
  public int enemySpawnCount = 2;

  // radius around camera to spawn enemies from
  public float enemySpawnRadius = 3f;

  // delay between each enemy spawn
  public float spawnInterval = 5f;

  private CameraState state = CameraState.Surveying;
  private float detectionTimer = 0f;

  private enum CameraState { Surveying, Detecting, Alerted, Hacked }

  void Update()
  {
    switch (state)
    {
      case CameraState.Surveying:
        Survey();
        break;

      case CameraState.Detecting:
        Detect();
        break;

      case CameraState.Alerted:
      case CameraState.Hacked:
        // handled by coroutines or permanently disabled
        break;
    }

    // check for hack input from Player2 when in range
    CheckForHack();
  }

  // surveys the area for players each frame
  void Survey()
  {
    foreach (var entry in PlayerManager.Instance.Players)
    {
      float dist = Vector2.Distance(transform.position, entry.Value.gameObject.transform.position);
      if (dist <= detectionRadius)
      {
        state = CameraState.Detecting;
        detectionTimer = 0f;
        Debug.Log("Camera: Player noticed");
        return;
      }
    }
  }

  // counts up while player is in range, goes to alerted when timer hits detectionTime
  void Detect()
  {
    bool playerInRange = false;

    foreach (var entry in PlayerManager.Instance.Players)
    {
      float dist = Vector2.Distance(transform.position, entry.Value.gameObject.transform.position);
      if (dist <= detectionRadius)
      {
        playerInRange = true;
        break;
      }
    }

    // player left the radius before timer finished, go back to surveying
    if (!playerInRange)
    {
      state = CameraState.Surveying;
      detectionTimer = 0f;
      Debug.Log("Camera: Player left range, back to surveying.");
      return;
    }

    detectionTimer += Time.deltaTime;

    if (detectionTimer >= detectionTime)
    {
      state = CameraState.Alerted;
      StartCoroutine(SpawnEnemies());
      Debug.Log("Camera: Alerted, spawning enemies.");
    }
  }

  // spawns enemies one at a time on the ground in a radius around the camera
  IEnumerator SpawnEnemies()
  {
    for (int i = 0; i < enemySpawnCount; i++)
    {
      if (enemyPrefab != null)
      {
        Vector2 spawnOffset = Random.insideUnitCircle * enemySpawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(spawnOffset.x, 0, 0);

        RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, 20f);
        if (hit.collider != null)
        {
          spawnPos = hit.point;
          Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
      }
      yield return new WaitForSeconds(spawnInterval);
    }
  }

  // checks if Player2 is in range and presses E to hack the camera
  void CheckForHack()
  {
    if (state == CameraState.Hacked) return;

    if (PlayerManager.Instance.ActivePlayer != PlayerType.Player2) return;

    if (!PlayerManager.Instance.interact.WasPressedThisFrame()) return;

    Player p2 = PlayerManager.Instance.Players[PlayerType.Player2];
    float dist = Vector2.Distance(transform.position, p2.gameObject.transform.position);

    if (dist <= detectionRadius)
    {
      state = CameraState.Hacked;
      Debug.Log("Camera: Hacked and disabled");
    }
  }

  // draws detection and spawn radius in editor for tuning
  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, enemySpawnRadius);
  }
}