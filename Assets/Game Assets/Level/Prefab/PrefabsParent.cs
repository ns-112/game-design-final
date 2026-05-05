using System.Collections.Generic;
using UnityEngine;



public enum PrefabParentType
{
    Test,
    TestEnemy
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