using System.Collections.Generic;
using UnityEngine;



public enum PrefabParentType
{
    Test
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