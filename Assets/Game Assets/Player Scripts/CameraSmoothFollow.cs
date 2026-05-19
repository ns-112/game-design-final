using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public bool PauseCamera = false;
    public bool CameraReady = false;

    void Update()
    {
        var pManager = PlayerManager.Instance;
        var gManager = GameLevelManager.Instance;

        //Debug.Log("Camera Update Running");

        if (gManager == null)
        {
            Debug.Log("gManager is NULL");
            return;
        }

        if (pManager == null)
        {
            Debug.Log("pManager is NULL");
            return;
        }

        

        if (pManager.Players == null)
        {
            Debug.Log("Players dictionary is NULL");
            return;
        }

        //Debug.Log("Players count: " + pManager.Players.Count);

        if (!pManager.Players.TryGetValue(pManager.ActivePlayer, out var player))
        {
            Debug.Log("Active player NOT FOUND");
            Debug.Log("ActivePlayer key: " + pManager.ActivePlayer);
            return;
        }

        if (player == null)
        {
            Debug.Log("Player is NULL");
            return;
        }

        if (!player.gameObject.activeInHierarchy)
        {
            Debug.Log("Player inactive in hierarchy");
            return;
        }

        if (!player.gameObject.scene.IsValid())
        {
            Debug.Log("Player scene invalid");
            return;
        }

        //Debug.Log("Following player: " + player.gameObject.name);

        Vector3 target = player.gameObject.transform.position;
        target.z = transform.position.z;

        float t = 5f * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, target, t);

        //Debug.Log("Camera moved to: " + transform.position);
    }
}