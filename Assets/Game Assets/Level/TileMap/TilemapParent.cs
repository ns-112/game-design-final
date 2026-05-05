using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TilemapEntry
{
    public Tilemap tilemap;
    public TilemapType type;
}

public class TilemapParent : MonoBehaviour
{
    public List<TilemapEntry> tilemaps = new List<TilemapEntry>();

    public void RefreshTilemaps()
    {
        if (tilemaps != null || tilemaps.Count > 0)
        {
            tilemaps.Clear();
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out LevelTypeID ID))
            {
                tilemaps.Add(new TilemapEntry
                {
                    tilemap = transform.GetChild(i).GetComponent<Tilemap>(),
                    type = ID.t_id
                });
            }
        }
    }
}