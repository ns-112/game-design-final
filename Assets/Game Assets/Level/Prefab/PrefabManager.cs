using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;

public enum ReservedPrefabs
{
    GoalZone,
    Res1,
    Res2,
    Res3,
    Res4,
    Res5,
    Res6,
    Res7,
    Res8,
    ResMax
}

[System.Serializable]
public class RegisteredPrefab
{
    public GameObject prefab;
    public int index;
}

public class PrefabManager : MonoBehaviour
{
    [ReadOnlyAttribute]
    public List<RegisteredPrefab> RegisteredPrefabs = new List<RegisteredPrefab>();

}

public class ReadOnlyAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}

[CustomEditor(typeof(PrefabManager))]
public class PrefabManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabManager manager = (PrefabManager)target;

        GUILayout.Space(15);
        GUILayout.Label("Tools", EditorStyles.boldLabel);

        DrawPrefabDropArea(manager);

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh Registered Prefabs"))
        {
            int index = manager.RegisteredPrefabs.Count;
            while (manager.RegisteredPrefabs.Count < (int)ReservedPrefabs.ResMax + 1)
            {
                manager.RegisteredPrefabs.Add(new RegisteredPrefab{ index = index });
                index++;
            }
            
            EditorUtility.SetDirty(manager);
        }
    }

    private void DrawPrefabDropArea(PrefabManager manager)
    {
        Rect dropArea = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));

        GUI.Box(dropArea, "Drop Level Prefab");

        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:

                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        GameObject go = draggedObject as GameObject;

                        if (go != null)
                        {
                            
                            if (PrefabUtility.IsPartOfPrefabAsset(go))
                            {
                                int index = (manager.RegisteredPrefabs.Count == 0 ? (int)ReservedPrefabs.ResMax + 1 : 0) + manager.RegisteredPrefabs.Count;
                                manager.RegisteredPrefabs.Add(new RegisteredPrefab
                                {
                                    prefab = go,
                                    index = index
                                });
                            }
                        }
                    }

                    EditorUtility.SetDirty(manager);
                }

                evt.Use();
                break;
        }
    }
}