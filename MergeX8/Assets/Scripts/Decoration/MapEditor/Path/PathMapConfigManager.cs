using System.Collections.Generic;
using System.IO;
using Decoration.DynamicMap;
using DragonU3DSDK.Asset;
using UnityEditor;
using UnityEngine;

namespace SomeWhere
{
    public class PathMapConfigManager : Singleton<PathMapConfigManager>
    {
        public RT_PathMap _pathMap;
        public void InitConfig()
        {
            TextAsset json = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                json = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Export/configs/Decoration/PathMap.json");
#endif
            if(json == null)
                json = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/Decoration", "PathMap"));
            
            _pathMap  = JsonUtility.FromJson<RT_PathMap>(json?.text);
        }

        public RT_PathInfo GetPathInfo(string pathId)
        {
            if (_pathMap == null || _pathMap._pathInfos == null)
                return null;

            return _pathMap._pathInfos.Find(a => a._pathId == pathId);
        }
    }
}