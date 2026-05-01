using UnityEngine;
using UnityEngine.Tilemaps;

public class GameLevelUtils
{
    public void AddTileToLevel(
        GameLevel level,
        ObjectType type,
        Vector3Int position,
        int textureIndex)
    {
        TileData data = new TileData
        {
            position = new Vector2Int(position.x, position.y),
            textureIndex = textureIndex
        };

        switch (type)
        {
            case ObjectType.Static:
                level.StaticObjects.Add(data);
                break;

            case ObjectType.Dynamic:
                level.DynamicProps.Add(data);
                break;

            case ObjectType.Interactable:
                level.InteractableProps.Add(data);
                break;

            case ObjectType.Switches:
                level.Switches.Add(data);
                break;

            case ObjectType.Goal:
                level.GoalItem = data;
                break;
        }
    }

    
}
