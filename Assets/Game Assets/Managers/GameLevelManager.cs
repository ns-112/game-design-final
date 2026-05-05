using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System;
using System.Linq;

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









    public GameObject TilemapContainer;
    public GameObject PrefabContainer;



    public GameObject Player1;
    public GameObject Player2;
    public GameObject Camera;





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

    public void LoadAllLevelsFromFolder()
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

    void AddTilesToMapList(List<TileData> tiles, Tilemap map)
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

            tiles.Add(new TileData
            {
                position = new Vector2Int(pos.x, pos.y),
                textureIndex = index
            });
        }
    }

    void SaveTilemap(Tilemap map, ObjectType type)
    {
        if (currentLevel.LevelSets == null)
        {
            currentLevel.LevelSets = new List<LevelData>();
        }

        if (currentLevel != null && currentLevel.LevelSets.Any(item => item.type == type))
        {
            var set = currentLevel.LevelSets.FirstOrDefault(item => item.type == type);

            AddTilesToMapList(set.tiles, map);
            
            set.prefabs = null;
        } else
        {
            LevelData newData = new();
            List<TileData> tiles = new();

            AddTilesToMapList(tiles, map);

            newData.type = type;
            newData.tiles = tiles;
            newData.prefabs = null;

            currentLevel.LevelSets.Add(newData);
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

    public void SaveOpenLevelAsNewGameLevel()
    {
        string name = currentLevel != null ? currentLevel.name : "NewLevel";
        RefilTileDicts();

        GameLevel level = new GameLevel(name);

        TilemapContainer.GetComponent<TilemapParent>().RefreshTilemaps();
        foreach (var tm in TilemapContainer.GetComponent<TilemapParent>().tilemaps)
        {  
            if (tm.tilemap != null)
            { 
                SaveTilemap(tm.tilemap, tm.type);    
            }
        }

        level.LevelSets = currentLevel.LevelSets;

        level.StartData.P1Start = new Vector2(Player1.transform.position.x, Player1.transform.position.y);
        level.StartData.P2Start = new Vector2(Player2.transform.position.x, Player2.transform.position.y);

        level.StartData.P1Active = currentLevel.StartData.P1Active;
        level.StartData.P2Active = currentLevel.StartData.P2Active;

        level.StartData.CameraStart = currentLevel.StartData.CameraStart;
        
        string json = level.SerializeLevel();

        string folder = Path.Combine(Application.dataPath, "Game Assets/Level/Levels");
        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, name + ".json");

        File.WriteAllText(path, json);
        Debug.Log("Saved level: " + path);
    }

    void LoadTilemap(Tilemap map, ObjectType type)
    {
        if (map == null)
        {
            Debug.LogWarning("Tilemap is null!");
            return;
        }

        if (currentLevel == null)
        {
            Debug.LogError("CurrentLevel is null!");
            return;
        }

        if (currentLevel.LevelSets == null)
        {
            Debug.LogError("LevelSets is NULL — bad load or bad save.");
            return;
        }

        map.ClearAllTiles();

        var set = currentLevel.LevelSets.FirstOrDefault(item => item.type == type);

        if (set?.tiles == null)
        {
            Debug.LogWarning($"No tiles found for type: {type}");
            return;
        }

        foreach (var tile in set.tiles)
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

    public void NewGameLevel(string name)
    {
        currentLevel = new GameLevel(name);
        TilemapContainer.GetComponent<TilemapParent>().RefreshTilemaps();
        foreach (var tm in TilemapContainer.GetComponent<TilemapParent>().tilemaps)
        { 
            if (tm.tilemap != null)
            {
                tm.tilemap.ClearAllTiles();
            }
        }


       
        Player1.transform.position = new Vector3(-5, 0, 0);
        Player2.transform.position = new Vector3(5, 0, 0);
        Camera.transform.position = Player1.transform.position;
    }

    public void LoadGameLevel(string levelName)
    {
        RefilTileDicts();
        LoadAllLevelsFromFolder();
        if (!LevelDict.TryGetValue(levelName, out GameLevel found))
        {
            Debug.LogWarning("Level not found: " + levelName);
            return;
        }

        currentLevel = found;
        TilemapContainer.GetComponent<TilemapParent>().RefreshTilemaps();
        foreach (var tm in TilemapContainer.GetComponent<TilemapParent>().tilemaps)
        {  
            LoadTilemap(tm.tilemap, tm.type);
        }

        Player1.transform.position = new Vector3(found.StartData.P1Start.x, found.StartData.P1Start.y, 0);
        Player2.transform.position = new Vector3(found.StartData.P2Start.x, found.StartData.P2Start.y, 0);
        
        
        switch (found.StartData.CameraStart)
        {
            case Players.Player1:
            Camera.transform.position = Player1.transform.position;
            break;

            case Players.Player2:
            Camera.transform.position = Player2.transform.position;
            break;
        }

        #if !UNITY_EDITOR
            PlayerManager.Instance.levelOptions = new LevelOptions
            {
                P1Locked = !found.StartData.P1Active,
                P2Locked = !found.StartData.P2Active
            };
        #endif

        Debug.Log("Loaded level: " + levelName);
    }


}





[CustomEditor(typeof(GameLevelManager))]
public class GameLevelManagerEditor : Editor
{

    private string saveName = "";
    private string loadLabel = "Load levels";
    private int selectedIndex = 0;
    private string[] levelNames = new string[0];
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();

        GameLevelManager manager = (GameLevelManager)target;

        saveName = manager.currentLevel.name;

        if (GUILayout.Button(loadLabel))
        {
            manager.LoadAllLevelsFromFolder(); // refresh list
            levelNames = new List<string>(manager.LevelDict.Keys).ToArray();
            loadLabel = "Reload levels";
        }

        

        

        if (levelNames.Length > 0)
        {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, levelNames.Length - 1);
        }

        GUILayout.Label("Load Existing Level:");

        if (levelNames.Length > 0)
        {
            selectedIndex = EditorGUILayout.Popup(selectedIndex, levelNames);

            if (GUILayout.Button("Load Selected"))
            {
                manager.LoadGameLevel(levelNames[selectedIndex]);
                manager.currentLevel.name = levelNames[selectedIndex];
                saveName = levelNames[selectedIndex];
            }
        }
        else
        {
            GUILayout.Label("No levels found.");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("New Level"))
        {
            manager.NewGameLevel("UNSAVED"); // temporary name
            manager.currentLevel.name = "UNSAVED";
        }

        GUILayout.Space(10);

    GUILayout.Label("Save Level As:");

    saveName = GUILayout.TextField(saveName);

    GUI.enabled = !string.IsNullOrWhiteSpace(saveName);

    if (GUILayout.Button("Save"))
    {
        manager.currentLevel.name = saveName;
        manager.SaveOpenLevelAsNewGameLevel();
    }

    GUI.enabled = true;
    }
}