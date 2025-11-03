using LevelEditor;
using UnityEditor;
using UnityEngine;

namespace Screw.Editor
{
    [CustomEditor(typeof(GenerateLevelData))]
    public class GenerateLevelDataEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
     
            if (GUILayout.Button("GenerateLevel"))
            {
                ((GenerateLevelData)target).GenerateLevel();
            }
        }
    }
}