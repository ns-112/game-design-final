using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum Players
{
    Player1,
    Player2,
}

//Used for layering
[System.Serializable]
public enum TilemapType
{
    Static,

    //BG and parallax layers are intertwined, it follows the pattern: (highest) bg1 -> pa1 -> bg2 -> pa2... (lowest)

    //BG Layers
    BG1,
    BG2,
    BG3,
    BG4,
    //Parallax Layers
    PA1,
    PA2,
    PA3,
    PA4,

    //Foreground Layers
    FG1,
    FG2,
    FG3,
    FG4,
}

//Contains a texture id and position of a tile
[System.Serializable]
public class TileData
{
    public int textureIndex;
    public Vector2Int position;
}

//
[System.Serializable]
public class PrefabData
{
    public int prefabIndex;
    public string prefabName;
    public Vector2 position;
    public int zLayer;
    public float rotation;
    public List<string> Arguments;
}

//Contains data about the start, like player pos
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

//Class that contains tiles and their type for layering
[System.Serializable]
public class LevelData
{
    public TilemapType type;
    public List<TileData> tiles;
}


//Main level class that contains all data
[System.Serializable]
public class GameLevel
{
    public LevelStartData StartData = new();
    
    public List<LevelData> LevelSets = new();
    public List<PrefabData> Prefabs = new();


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



