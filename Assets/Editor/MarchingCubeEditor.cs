using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MarchingCube))]
public class MarchingCubeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MarchingCube marchingCube = (MarchingCube)target;

        if (DrawDefaultInspector())
        {
            //marchingCube.CreateMeshData();
        }

        if (GUILayout.Button("Generate")){
            //marchingCube.GenerateTerrainMap();
            //marchingCube.CreateMeshData();
        }
    }
}
