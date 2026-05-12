using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [Header("Vision")]
    public float radius = 10f;

    [Range(-180, 180)]
    public float startAngle = -45f;

    [Range(-180, 180)]
    public float endAngle = 45f;

    public float hitTimer = 1;

    public LayerMask playerMask;
    public LayerMask wallMask;

    [Header("Debug")]
    public bool drawDebug = true;

    public bool CanSeePlayer(GameObject player)
    {
        Vector2 origin = transform.position;
        Vector2 dirToPlayer =
            (player.transform.position - transform.position);

        float distance = dirToPlayer.magnitude;

        if (distance > radius)
            return false;

        dirToPlayer.Normalize();

        Vector2 forward =
            DirectionFromAngle(GetMidAngle());

        float angle =
            Vector2.SignedAngle(forward, dirToPlayer);

        float halfAngle = GetAngleSize() * 0.5f;

        if (Mathf.Abs(angle) > halfAngle)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dirToPlayer,
            distance,
            wallMask | playerMask
        );

        if (hit.collider == null)
            return false;

        return hit.collider.CompareTag("Player");
    }

    float GetMidAngle()
    {
        return (startAngle + endAngle) * 0.5f;
    }

    float GetAngleSize()
    {
        float angle = endAngle - startAngle;

        if (angle < 0)
            angle += 360;

        return angle;
    }

    Vector2 DirectionFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(rad),
            Mathf.Sin(rad)
        );
    }

    void OnDrawGizmos()
    {
        if (!drawDebug)
            return;

        Vector3 origin = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, radius);

        Vector2 startDir =
            DirectionFromAngle(startAngle);

        Vector2 endDir =
            DirectionFromAngle(endAngle);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            origin,
            origin + (Vector3)(startDir * radius)
        );

        Gizmos.DrawLine(
            origin,
            origin + (Vector3)(endDir * radius)
        );
    }



    void Update()
    {
        if (CanSeePlayer(PlayerManager.Instance.Players[PlayerManager.Instance.ActivePlayer].gameObject))
        {
            if (hitTimer <= 0) {
                MoneySystem.Instance.TakeDamage();
                hitTimer = 1;
            }
            else
            {
                hitTimer--;
            }
            
        } else
        {
            hitTimer = 1;
        }
    }
}