using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TilemapEntry
{
    public Tilemap tilemap;
    public ObjectType type;
}

public class TilemapParent : MonoBehaviour
{
    public List<TilemapEntry> tilemaps;

    public void RefreshTilemaps()
    {
        if (tilemaps != null || tilemaps.Count > 0)
        {
            tilemaps.Clear();
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out TilemapIdentifier tid))
            {
                tilemaps.Add(new TilemapEntry
                {
                    tilemap = transform.GetChild(i).GetComponent<Tilemap>(),
                    type = tid.identifier
                });
            }
        }
    }
}