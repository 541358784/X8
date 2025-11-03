using DragonU3DSDK.Asset;
using SomeWhere;
using UnityEditor;
using UnityEngine;

namespace Zuma
{
    public class ZumaPathMapConfigManager : Singleton<ZumaPathMapConfigManager>
    {
        public const string _configPath = "Prefabs/Activity/Zuma/path_{0}";
        public const string _exportPath = "Export/" + _configPath;
        public const string _assetsPath = "Assets/"+_exportPath;
        
        public void LoadConfig(string pathId)
        {
            TextAsset json = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                json = AssetDatabase.LoadAssetAtPath<TextAsset>(string.Format(_assetsPath, pathId)+".json");
#endif
            if(json == null)
                json = ResourcesManager.Instance.LoadResource<TextAsset>(string.Format(_configPath, pathId));
        }
    }
}