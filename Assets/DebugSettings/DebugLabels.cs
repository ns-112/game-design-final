using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

[InitializeOnLoad]
public static class DebugLabels
{
    static DebugLabels()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        string text = $"ID:{obj.GetInstanceID()}";

        Rect r = new Rect(selectionRect);
        r.x = selectionRect.xMax - 80;
        r.width = 80;

        GUI.Label(r, text);
    }
}
#endif