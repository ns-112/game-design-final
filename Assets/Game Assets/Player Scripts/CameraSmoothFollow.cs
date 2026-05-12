using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public bool PauseCamera = false;
    public bool CameraReady = false;
    void Update()
    {
        
        var pManager = PlayerManager.Instance;
        var gManager = GameLevelManager.Instance;

        if (!CameraReady || PauseCamera || gManager.IsLoadingLevel || pManager == null || pManager.Players == null || gManager == null)
            return;

        if (!pManager.Players.TryGetValue(pManager.ActivePlayer, out var player) || player == null)
            return;

        if (player.gameObject == null || !player.gameObject.activeInHierarchy || !player.gameObject.scene.IsValid())
            return;

        Vector3 target = player.gameObject.transform.position;
        target.z = transform.position.z;

        float t = Mathf.Clamp01(5f * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, target, t);
    }
}
