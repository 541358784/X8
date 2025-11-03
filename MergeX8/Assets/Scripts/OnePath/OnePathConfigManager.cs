using System.Collections.Generic;
using System.IO;
using Decoration.DynamicMap;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using UnityEditor;
using UnityEngine;

namespace OnePath
{
    public class OnePathConfigManager : Manager<OnePathConfigManager>
    {
        public Dictionary<int, OnePathConfig> _onePathConfigs = new Dictionary<int, OnePathConfig>();
        public List<TableOnePathLevel> _configs;
        
        public void InitTableConfigs()
        {
            TableManager.Instance.InitLocation("configs/OnePath");
        
            _configs = TableManager.Instance.GetTable<TableOnePathLevel>();
        }

        public List<TableOnePathLevel> GetOnePathConfigs()
        {
            return _configs;
        }
        
        public OnePathConfig GetConfig(int id)
        {
            if (_onePathConfigs.ContainsKey(id))
            {
                return _onePathConfigs[id];
            }
            
            TextAsset json = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                json = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/Export/configs/OnePath/{id}.json");
#endif
            if(json == null)
                json = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/OnePath", id.ToString()));
            
            OnePathConfig config = JsonUtility.FromJson<OnePathConfig>(json?.text);

            _onePathConfigs.Add(id, config);

            return _onePathConfigs[id];
        }
    }
}