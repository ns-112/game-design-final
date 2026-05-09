using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System;
using System.Linq;
using TMPro;
using System.Collections;

//Contains data for the currently loaded level, like timers
//Add in menu and use to load levels when needed
public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }
    public GameObject PrefabRegistrar;
    public Dictionary<string, GameLevel> LevelDict = new Dictionary<string, GameLevel>();
    public GameLevel currentLevel; //NOT for creating new levels

    public TileBase[] tileLookup;
    public List<GameObject> prefabLookup;

    Dictionary<TileBase, int> tileToIndex;
    Dictionary<int, TileBase> indexToTile;


    Dictionary<GameObject, int> prefabToIndex;
    Dictionary<int, GameObject> indexToPrefab;







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
            RefilPrefabDicts();
            LoadAllLevelsFromFolder();
            
            
        }
    }

    IEnumerator Start()
    {
        yield return null;

        LoadGameLevel(currentLevel.name);
    }

    public void LoadAllLevelsFromFolder()
    {
        LevelDict = new Dictionary<string, GameLevel>();

        string path = Path.Combine(Application.dataPath, "Game Assets/Resources/Levels");

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

    void SaveTilemap(Tilemap map, TilemapType type)
    {
        if (currentLevel.LevelSets == null)
            currentLevel.LevelSets = new List<LevelData>();

        // remove old entry
        currentLevel.LevelSets.RemoveAll(x => x.type == type);

        LevelData newData = new LevelData
        {
            type = type,
            tiles = new List<TileData>()
        };

        AddTilesToMapList(newData.tiles, map);

        currentLevel.LevelSets.Add(newData);
    }

    void SavePrefab(GameObject prefab)
    {
        if (currentLevel?.Prefabs == null)
            currentLevel.Prefabs = new List<PrefabData>();
        
        

        if (!prefabToIndex.TryGetValue(prefab.GetComponent<PrefabIdentifier>().basePrefab.prefab, out int index))
        {
            Debug.LogWarning($"Prefab not in lookup: {prefab.GetComponent<PrefabIdentifier>().basePrefab}");
            return;
        }

        List<string> args = new List<string>();

        if (prefab.TryGetComponent(out TextMeshPro tmp))
        {
            args.Add(tmp.text); //args[0]
        } else if (prefab.TryGetComponent(out TogglePlayerTrigger tpt))
        {
            args.Add(tpt.ActivateAmount.ToString()); //args[0]
            args.Add(tpt.DeactivateAmount.ToString()); //args[1]
            args.Add(((int)tpt.TogglePlayer).ToString()); //args[2]
            args.Add(((int)tpt.State).ToString()); //args[3]
        } else if (prefab.TryGetComponent(out PlayerTrigger pt))
        {
            args.Add(pt.ActivateAmount.ToString()); //args[0]
            args.Add(pt.DeactivateAmount.ToString()); //args[1]
        }

        currentLevel.Prefabs.Add(new PrefabData
        {
            prefabIndex = index,
            prefabName = prefab.name,
            position = new Vector2(prefab.transform.position.x, prefab.transform.position.y),
            zLayer = (int)prefab.transform.position.z,
            rotation = prefab.transform.eulerAngles.z,
            Arguments = args
        });
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

    public void RefilPrefabDicts()
    {
        prefabToIndex = new Dictionary<GameObject, int>();
        indexToPrefab = new Dictionary<int, GameObject>();

        prefabLookup.Clear();

        foreach (var entry in PrefabRegistrar.GetComponent<PrefabManager>().RegisteredPrefabs)
        {
            if (entry.prefab == null)
            {
                #if DEBUG
                Debug.Log(entry.index);
                #endif
                continue;
            }
            

            prefabToIndex[entry.prefab] = entry.index;
            indexToPrefab[entry.index] = entry.prefab;

            prefabLookup.Add(entry.prefab);
            
            #if DEBUG
            Debug.Log($"Registered prefab: {indexToPrefab[entry.index].name}({indexToPrefab[entry.index].GetHashCode()}) -> {prefabToIndex[entry.prefab]}");
            #endif
        }
    }

    public void SaveOpenLevelAsNewGameLevel()
    {
        string name = currentLevel != null ? currentLevel.name : "NewLevel";

        RefilTileDicts();
        RefilPrefabDicts();

        GameLevel level = new GameLevel(name);

        //Save Tiles
        TilemapContainer.GetComponent<TilemapParent>().RefreshTilemaps();
        foreach (var tm in TilemapContainer.GetComponent<TilemapParent>().tilemaps)
        {  
            if (tm.tilemap != null)
            { 
                SaveTilemap(tm.tilemap, tm.type);    
            }
        }

        //Save Prefabs
        PrefabContainer.GetComponent<PrefabsParent>().RefreshPrefabs();
        #if DEBUG
        Debug.Log("Prefabs found: " + PrefabContainer.GetComponent<PrefabsParent>().prefabs.Count);
        #endif

        if (currentLevel.Prefabs.Count > 0)
        {
            currentLevel.Prefabs.Clear();
        }

        foreach (GameObject pf in PrefabContainer.GetComponent<PrefabsParent>().prefabs)
        {  
            if (pf != null)
            { 
                #if DEBUG
                Debug.Log($"Looking up prefab: {indexToPrefab[pf.GetComponent<PrefabIdentifier>().basePrefab.index].name}({indexToPrefab[pf.GetComponent<PrefabIdentifier>().basePrefab.index].GetHashCode()}) -> {prefabToIndex[pf.GetComponent<PrefabIdentifier>().basePrefab.prefab]}");
                #endif
                pf.GetComponent<PrefabIdentifier>().basePrefab.index = prefabToIndex[pf.GetComponent<PrefabIdentifier>().basePrefab.prefab];
                SavePrefab(pf);   
            }
        }
        level.Prefabs = currentLevel.Prefabs != null ? new List<PrefabData>(currentLevel.Prefabs) : new List<PrefabData>();

        level.LevelSets = currentLevel.LevelSets.Select(x => new LevelData
        {
            type = x.type,
            tiles = new List<TileData>(x.tiles)
        }).ToList();

        

        level.StartData.P1Start = new Vector2(Player1.transform.position.x, Player1.transform.position.y);
        level.StartData.P2Start = new Vector2(Player2.transform.position.x, Player2.transform.position.y);

        level.StartData.P1Active = currentLevel.StartData.P1Active;
        level.StartData.P2Active = currentLevel.StartData.P2Active;

        level.StartData.CameraStart = currentLevel.StartData.CameraStart;
        
        string json = level.SerializeLevel();

        string folder = Path.Combine(Application.dataPath, "Game Assets/Resources/Levels");
        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, name + ".json");

        File.WriteAllText(path, json);
        Debug.Log("Saved level: " + path);
    }

    void LoadTilemap(Tilemap map, TilemapType type)
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

    void LoadPrefabs()
    {
        if (currentLevel == null)
        {
            Debug.LogError("CurrentLevel is null!");
            return;
        }

        if (currentLevel.Prefabs == null)
        {
            Debug.LogWarning("No prefabs in level.");
            return;
        }

        foreach (var data in currentLevel.Prefabs)
        {
            if (!indexToPrefab.TryGetValue(data.prefabIndex, out GameObject prefab))
            {
                Debug.LogWarning($"Missing prefab index: {data.prefabIndex}");
                continue;
            }

            

            Vector3 pos = new Vector3(
                data.position.x,
                data.position.y,
                data.zLayer
            );

            Vector3 rot = new Vector3(0, 0, data.rotation);

            var instance = Instantiate(prefab, pos, Quaternion.Euler(rot), PrefabContainer.transform);
            instance.name = data.prefabName;
            instance.GetComponent<PrefabIdentifier>().basePrefab.index = data.prefabIndex;
            instance.GetComponent<PrefabIdentifier>().basePrefab.prefab = prefab;

            if (instance.TryGetComponent(out TextMeshPro tmp))
            {
                tmp.text = data.Arguments[0];
            } else if (instance.TryGetComponent(out TogglePlayerTrigger tpt))
            {
                if (int.TryParse(data.Arguments[0], out int res1))
                {
                    tpt.ActivateAmount = res1;
                }
                else
                {
                    Debug.LogError("Argument[0] for " + data.prefabName + " failed to parse");
                }

                if (int.TryParse(data.Arguments[0], out int res2))
                {
                    tpt.DeactivateAmount = res2;
                }
                else
                {
                    Debug.LogError("Argument[1] for " + data.prefabName + " failed to parse");
                }

                if (int.TryParse(data.Arguments[2], out int res3))
                {
                    tpt.TogglePlayer = (PlayerType)res3;
                }
                else
                {
                    Debug.LogError("Argument[2] for " + data.prefabName + " failed to parse");
                }

                if (int.TryParse(data.Arguments[3], out int res4))
                {
                    tpt.State = (ToggleState)res4;
                }
                else
                {
                    Debug.LogError("Argument[3] for " + data.prefabName + " failed to parse");
                }
            } else if (instance.TryGetComponent(out PlayerTrigger pt))
            {
                if (int.TryParse(data.Arguments[0], out int res1))
                {
                    pt.ActivateAmount = res1;
                }
                else
                {
                    Debug.LogError("Argument[0] for " + data.prefabName + " failed to parse");
                }

                if (int.TryParse(data.Arguments[0], out int res2))
                {
                    pt.DeactivateAmount = res2;
                }
                else
                {
                    Debug.LogError("Argument[1] for " + data.prefabName + " failed to parse");
                }
            } 
        }
    }

    public void NewGameLevel(string name)
    {
        RefilTileDicts();
        RefilPrefabDicts();
        

        currentLevel = new GameLevel(name);
        TilemapContainer.GetComponent<TilemapParent>().RefreshTilemaps();
        foreach (var tm in TilemapContainer.GetComponent<TilemapParent>().tilemaps)
        { 
            if (tm.tilemap != null)
            {
                tm.tilemap.ClearAllTiles();
            }
        }

        PrefabContainer.GetComponent<PrefabsParent>().RefreshPrefabs();
        PrefabContainer.GetComponent<PrefabsParent>().ClearPrefabs();

        #if DEBUG
        Debug.Log("Prefabs found: " + PrefabContainer.GetComponent<PrefabsParent>().prefabs.Count);
        #endif

        if (currentLevel.Prefabs.Count > 0)
        {
            currentLevel.Prefabs.Clear();
        }

        currentLevel.StartData.CameraStart = Players.Player1;
       
        Player1.transform.position = new Vector3(-5, 0, 0);
        Player2.transform.position = new Vector3(5, 0, 0);
        Camera.transform.position = new Vector3(Player1.transform.position.x, Player1.transform.position.y, Camera.transform.position.z);
    }

    public void LoadGameLevel(string levelName)
    {
        RefilTileDicts();
        RefilPrefabDicts();
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

        var children = new List<GameObject>();

        foreach (Transform child in PrefabContainer.transform)
        {
            children.Add(child.gameObject);
        }

        foreach (var go in children)
        {
            #if !UNITY_EDITOR
                Destroy(go);
            #else
                DestroyImmediate(go);
            #endif
        }
        LoadPrefabs();

        Player1.transform.position = new Vector3(found.StartData.P1Start.x, found.StartData.P1Start.y, 0);
        Player2.transform.position = new Vector3(found.StartData.P2Start.x, found.StartData.P2Start.y, 0);
        
        #if !UNITY_EDITOR
            PlayerManager.Instance.Players[PlayerType.Player1].characterActive = found.StartData.P1Active;
            PlayerManager.Instance.Players[PlayerType.Player2].characterActive = found.StartData.P2Active;
        #endif
        currentLevel.StartData = found.StartData;
        
        switch (found.StartData.CameraStart)
        {
            case Players.Player1:
            Camera.transform.position = new Vector3(Player1.transform.position.x, Player1.transform.position.y, Camera.transform.position.z);
            break;

            case Players.Player2:
            Camera.transform.position = new Vector3(Player2.transform.position.x, Player2.transform.position.y, Camera.transform.position.z);
            break;
        }

        #if !UNITY_EDITOR
            PlayerManager.Instance.levelOptions = new LevelOptions
            {
                P1Locked = !found.StartData.P1Active,
                P2Locked = !found.StartData.P2Active
            };
        #endif

        #if DEBUG
        Debug.Log("Loaded level: " + levelName);
        #endif


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

        

        if (GUILayout.Button(loadLabel))
        {
            manager.LoadAllLevelsFromFolder(); // refresh list
            levelNames = new List<string>(manager.LevelDict.Keys).ToArray();
            loadLabel = "Reload levels";
            saveName = manager.currentLevel.name;
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

        if (GUILayout.Button("Reload Prefabs"))
        {
            manager.RefilPrefabDicts();
        }

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