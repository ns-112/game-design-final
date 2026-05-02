using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum Players
{
    Player1,
    Player2,
}

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
public class LevelStartData
{
    public Vector2 P1Start;
    public Vector2 P2Start;

    public bool P1Active;
    public bool P2Active;

    //Which player to start the camera on
    public Players CameraStart;
}

[System.Serializable]
public class GameLevel
{
    public LevelStartData StartData = new();

    
    public List<TileData> StaticObjects = new();
    public List<TileData> InteractableProps = new();
    public List<TileData> Switches = new();



    public List<TileData> BG1 = new();


    

    public TileData GoalItem = new();

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



