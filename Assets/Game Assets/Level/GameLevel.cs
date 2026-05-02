using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum ObjectType
{
    Static,
    Interactable,
    Switches,
    Goal,


    BG1
}


[System.Serializable]
public class TileData
{
    public int textureIndex;
    public Vector2Int position;
}


[System.Serializable]
public class GameLevel
{
    public List<TileData> StaticObjects = new();
    public List<TileData> InteractableProps = new();
    public List<TileData> Switches = new();



    public List<TileData> BG1 = new();

    

    public TileData GoalItem;

    public string name;

    public GameLevel(string name)
    {
        this.name = name;
    }

    public string SerializeLevel()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static GameLevel DeserializeLevel(string json)
    {
        return JsonUtility.FromJson<GameLevel>(json);
    }
}



