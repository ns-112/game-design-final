using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{

    void FixedUpdate()
    {
        Vector3 target = PlayerManager.Instance.Players[PlayerManager.Instance.ActivePlayer].gameObject.transform.position;
        target.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, target, 5f * Time.deltaTime);
    }
}
