using UnityEditor;
using UnityEngine;

namespace Plugins.CommonTools.Editor
{
    public class CreateMaterialMenu
    {
        [MenuItem("Assets/Create/*Custom Material*", false, 300)]
        public static void CreateCustomMaterial()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPath = path + $"/New Material{Time.frameCount}.mat";

            var mat = new Material(ShaderReplacer.Instance.GetShader("Custom/Standard"));
            AssetDatabase.CreateAsset(mat, assetPath);
            AssetDatabase.SaveAssets();

            Selection.activeObject = mat;
        }
    }
}