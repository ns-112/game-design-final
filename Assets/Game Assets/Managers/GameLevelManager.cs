using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


//Contains data for the currently loaded level, like timers
//Add in menu and use to load levels when needed
public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }
    public Dictionary<string, GameLevel> LevelDict = new Dictionary<string, GameLevel>();
    public GameLevel currentLevel; //NOT for creating new levels

    public TileBase[] tileLookup;

    Dictionary<TileBase, int> tileToIndex;
    Dictionary<int, TileBase> indexToTile;


    private Tilemap staticMap;
    private Tilemap interactableMap;
    private Tilemap switchMap;


    private Tilemap bg1Map;


    public GameObject staticObjectsGameobject;
    public GameObject interactableObjectsGameobject;
    public GameObject switchesObjectsGameobject;


    public GameObject bg1Gameobject;




    public float Timer1;
    public float Timer2;

    void Awake()
    {
        

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            RefilTileDicts();
            LoadAllLevelsFromFolder();

            
        }
    }

    void LoadAllLevelsFromFolder()
    {
        LevelDict = new Dictionary<string, GameLevel>();

        string path = Path.Combine(Application.dataPath, "Game Assets/Level/Levels");

        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Level folder does not exist: " + path);
            return;
        }

        string[] files = Directory.GetFiles(path, "*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            GameLevel level = GameLevel.DeserializeLevel(json);

            if (level != null)
            {
                LevelDict[level.name] = level;
                Debug.Log("Loaded level: " + level.name);
            }
        }
    }

    void SaveTilemap(Tilemap map, List<TileData> list)
    {

        foreach (var pos in map.cellBounds.allPositionsWithin)
        {
            TileBase tile = map.GetTile(pos);
            if (tile == null) continue;

            if (!tileToIndex.TryGetValue(tile, out int index))
            {
                Debug.LogWarning("Tile not in lookup!");
                continue;
            }

            list.Add(new TileData
            {
                position = new Vector2Int(pos.x, pos.y),
                textureIndex = index
            });
        }
    }

    void RefilTileDicts()
    {
        tileToIndex = new Dictionary<TileBase, int>();
        indexToTile = new Dictionary<int, TileBase>();

        for (int i = 0; i < tileLookup.Length; i++)
        {
            tileToIndex[tileLookup[i]] = i;
            indexToTile[i] = tileLookup[i];
        }
    }

    void CacheTilemaps()
    {
        if (staticObjectsGameobject != null)
            staticMap = staticObjectsGameobject.GetComponent<Tilemap>();

        if (bg1Gameobject != null)
            bg1Map = bg1Gameobject.GetComponent<Tilemap>();

        if (interactableObjectsGameobject != null)
            interactableMap = interactableObjectsGameobject.GetComponent<Tilemap>();

        if (switchesObjectsGameobject != null)
            switchMap = switchesObjectsGameobject.GetComponent<Tilemap>();
    }

    public void SaveOpenLevelAsNewGameLevel()
    {
        string name = currentLevel != null ? currentLevel.name : "NewLevel";
        RefilTileDicts();
        



        GameLevel level = new GameLevel(name);
        if (staticObjectsGameobject != null)
        {
            staticMap = staticObjectsGameobject.GetComponent<Tilemap>();
            SaveTilemap(staticMap, level.StaticObjects);
        }
        if (bg1Gameobject != null)
        {    
            bg1Map = bg1Gameobject.GetComponent<Tilemap>();
            SaveTilemap(bg1Map, level.BG1);
        }
        if (interactableObjectsGameobject != null)
        {
            interactableMap = interactableObjectsGameobject.GetComponent<Tilemap>();
            SaveTilemap(interactableMap, level.InteractableProps);
        }
        if (switchesObjectsGameobject != null)
        {
            switchMap = switchesObjectsGameobject.GetComponent<Tilemap>();
            SaveTilemap(switchMap, level.Switches);
        }
        
        string json = level.SerializeLevel();

        string folder = Path.Combine(Application.dataPath, "Game Assets/Level/Levels");
        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, name + ".json");

        File.WriteAllText(path, json);
        Debug.Log("Saved level: " + path);
    }

    void LoadTilemap(Tilemap map, List<TileData> list)
    {
        if (map == null)
        {
            Debug.LogWarning("Tilemap is null!");
            return;
        }
        map.ClearAllTiles();

        foreach (var tile in list)
        {
            Vector3Int pos = new Vector3Int(tile.position.x, tile.position.y, 0);

            if (indexToTile.TryGetValue(tile.textureIndex, out TileBase tileAsset))
            {
                map.SetTile(pos, tileAsset);
            }
            else
            {
                Debug.LogWarning($"Missing tile index: {tile.textureIndex}");
            }
        }
    }

    public void LoadGameLevel(string levelName)
    {
        CacheTilemaps();
        RefilTileDicts();
        LoadAllLevelsFromFolder();
        if (!LevelDict.TryGetValue(levelName, out GameLevel found))
        {
            Debug.LogWarning("Level not found: " + levelName);
            return;
        }

        currentLevel = found;
        

    
        LoadTilemap(staticMap, found.StaticObjects);  
        LoadTilemap(interactableMap, found.InteractableProps);
        LoadTilemap(switchMap, found.Switches);


        LoadTilemap(bg1Map, found.BG1);

        Debug.Log("Loaded level: " + levelName);
    }


}





[CustomEditor(typeof(GameLevelManager))]
public class GameLevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameLevelManager manager = (GameLevelManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("save"))
        {
            manager.SaveOpenLevelAsNewGameLevel();
        }

        if (GUILayout.Button("load"))
        {
            manager.LoadGameLevel(manager.currentLevel != null ? manager.currentLevel.name : "");
        }
    }
}