using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabType
{
    public GameObject prefab;
    public int index;
}

public enum PrefabParentType
{
    Enemy,
    Goal,
    Item,
    Label
}



public class PrefabsParent : MonoBehaviour
{
    public List<GameObject> prefabs = new List<GameObject>();

    public void RefreshPrefabs()
    {
        if (prefabs != null || prefabs.Count > 0)
        {
            prefabs.Clear();
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            prefabs.Add(transform.GetChild(i).gameObject);
        }
    }

    public void ClearPrefabs()
    {
        var children = new List<GameObject>();

        foreach (Transform child in transform)
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
    }
}


[CreateAssetMenu(menuName = "Prefabs/Prefab Type Database")]
public class PrefabTypeDatabase : ScriptableObject
{
    public List<PrefabTypeEntry> types;
}

[System.Serializable]
public class PrefabTypeEntry
{
    public string id;
    public GameObject prefab;
}