using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPreview))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapPreview mapPreview = (MapPreview)target;

        if (DrawDefaultInspector())
        {
            if (mapPreview.heightMapSettings.autoUpdate)
                mapPreview.DrawMapInEditor();
        }

        if (GUILayout.Button("Generate"))
            mapPreview.DrawMapInEditor();
    }
}
