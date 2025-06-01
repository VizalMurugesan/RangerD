using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TileManager tileManager = (TileManager)target;

        if (GUILayout.Button("Generate Terrain"))
        {
            tileManager.ClearAllTiles();
            tileManager.GenerateBaseGrass(tileManager.MapWidth, tileManager.MapHeight);
            tileManager.GenerateBaseGround(tileManager.MapWidth, tileManager.MapHeight);
        }

        EditorGUILayout.Space();

        if(GUILayout.Button("Generate Bushes"))
        {
            Debug.Log("editor clicked");
            tileManager.GenerateBushes(tileManager.MapWidth, tileManager.MapHeight);
        }
    }
}
