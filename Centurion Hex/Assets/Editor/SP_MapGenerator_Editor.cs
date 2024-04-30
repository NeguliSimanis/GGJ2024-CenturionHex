using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SP_MapGenerator))]
public class SP_MapGenerator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SP_MapGenerator mapGenerator = (SP_MapGenerator)target;
        if (GUILayout.Button("GENERATE MAP"))
        {
            mapGenerator.GenerateMap();
        }
    }
}
