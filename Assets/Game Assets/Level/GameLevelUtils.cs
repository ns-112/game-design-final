using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
public class GameLevelUtils
{
    public void AddTileToLevel(
        GameLevel level,
        TilemapType type,
        Vector3Int position,
        int textureIndex)
    {
        TileData data = new TileData
        {
            position = new Vector2Int(position.x, position.y),
            textureIndex = textureIndex
        };

        var set = level.LevelSets.FirstOrDefault(item => item.type == type);
        set.tiles.Add(data);
    }

    
}
