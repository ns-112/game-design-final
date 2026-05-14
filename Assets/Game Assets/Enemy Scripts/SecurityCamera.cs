using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Detection Cone")]
	public float detectionRange = 5f;
	public float startAngle = -45f;
	public float endAngle = 45f;
    public float detectionTime = 4f;

	[Header("Hack Zone")]
	public float hackRadius = 2f;

    [Header("Enemies")]
    public GameObject enemyPrefab;
    public int enemySpawnCount = 2;
    public float enemySpawnRadius = 3f;
    public float spawnInterval = 5f;

    [Header("Damage")]
    public float damageInterval = 1f;

	[Header("Line of Sight")]
	public LayerMask obstacleMask;

    private enum CameraState { Surveying, Detecting, Alerted, Hacked }
    private CameraState state = CameraState.Surveying;

	private int PlayerInRange = 0;

    private float detectionTimer;
    private float damageTimer;

    void Update()
    {
        switch (state)
        {
            case CameraState.Surveying:
                HandleSurveying();
                break;

            case CameraState.Detecting:
                HandleDetecting();
                break;

            case CameraState.Alerted:
                HandleAlerted();
                break;

            case CameraState.Hacked:
                break;
        }

        CheckForHack();
    }

  

    void HandleSurveying()
    {
        damageTimer = 0f;

        if (IsAnyPlayerInRange())
        {
            state = CameraState.Detecting;
            detectionTimer = 0f;
        }
    }

    void HandleDetecting()
    {
        if (!IsAnyPlayerInRange())
        {
            state = CameraState.Surveying;
            detectionTimer = 0f;
            return;
        }

        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionTime)
        {
            state = CameraState.Alerted;
            StartCoroutine(SpawnEnemies());
        }
    }

    void HandleAlerted()
    {
        damageTimer -= Time.deltaTime;

        if (damageTimer <= 0f)
        {
            damageTimer = damageInterval;
			MoneySystem.Instance.TakeDamage();
            //todo: screen shake or somehting
        }
    }

	bool CanSeePlayer(Transform target)
	{
		Vector2 origin = transform.position;
		Vector2 direction = (target.position - transform.position);

		float distance = direction.magnitude;
		direction.Normalize();

		// ray hits walls first if blocked
		RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleMask);

		// if nothing blocks it, we can see player
		return hit.collider == null;
	}

    

    bool IsAnyPlayerInRange()
	{
		foreach (var entry in PlayerManager.Instance.Players)
		{
			Transform target = entry.Value.gameObject.transform;

			Vector2 direction = (target.position - transform.position);
			float distance = direction.magnitude;

			if (distance > detectionRange)
				continue;

			float angle = Vector2.SignedAngle(transform.right, direction.normalized);

			if (angle < startAngle || angle > endAngle)
				continue;

			if (!CanSeePlayer(target))
				continue;

			return true;
		}

		return false;
	}

  

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemySpawnCount; i++)
        {
            if (enemyPrefab)
            {
                Vector2 offset = Random.insideUnitCircle * enemySpawnRadius;
                Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

                RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, 20f);
                if (hit.collider)
                {
                    Instantiate(enemyPrefab, hit.point, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

   

    void CheckForHack()
	{
		if (state == CameraState.Hacked) return;
		if (PlayerManager.Instance.ActivePlayer != PlayerType.Player2) return;
		if (!PlayerManager.Instance.interact.WasPressedThisFrame()) return;

		var p2 = PlayerManager.Instance.Players[PlayerType.Player2];

		float dist = Vector2.Distance(transform.position, p2.gameObject.transform.position);

		if (dist <= hackRadius)
		{
			StartCoroutine(HackCamera());
		}
	}

    IEnumerator HackCamera()
    {
        state = CameraState.Hacked;
        yield return new WaitForSeconds(15f);
        state = CameraState.Surveying;
        detectionTimer = 0f;
    }

    void OnDrawGizmosSelected()
	{
		

		Vector3 origin = transform.position;

		Vector3 leftDir = Quaternion.Euler(0, 0, startAngle) * transform.right;
		Vector3 rightDir = Quaternion.Euler(0, 0, endAngle) * transform.right;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(origin, origin + leftDir * detectionRange);
		Gizmos.DrawLine(origin, origin + rightDir * detectionRange);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(origin, hackRadius);
		
		#if UNITY_EDITOR
		UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
		UnityEditor.Handles.DrawSolidArc(
			origin,
			Vector3.forward,
			leftDir,
			endAngle - startAngle,
			detectionRange
		);
		#endif
	}
}