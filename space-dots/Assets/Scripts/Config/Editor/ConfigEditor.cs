using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Config))]
public class ConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Write Unique IDs"))
        {
            Config cfg = (Config)target;
            cfg.WriteUniqueIds();
            EditorUtility.SetDirty(target);
        }
    }
}
