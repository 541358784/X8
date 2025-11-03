using DragonU3DSDK.Asset;
using OnePath.View;
using OnePathSpace;
using UnityEngine;

namespace OnePath.Model
{
    public partial class OnePathModel : Manager<OnePathModel>
    {
        public int debugLevelId = -1;
        
        // public TableOnePathLevel _config;
        // private GameObject _prefab;
        // public OnePathControl _pathControl;
        
        // public void LoadLevel(TableOnePathLevel config)
        // {
        //     _config = config;
        //     InitConfig();
        //     
        //     var levelPath = $"OnePath/Level{config.levelId}/Prefab/OnePath";
        //     var prefab = ResourcesManager.Instance.LoadResource<GameObject>(levelPath);
        //     if (prefab == null)
        //         return;
        //
        //     _prefab = GameObject.Instantiate(prefab);
        //     _prefab.transform.position = new Vector3(0,0,-100);
        //     _pathControl = _prefab.AddComponent<OnePathControl>();
        // }
        
        public void Release()
        {
            // if(_pathControl != null)
            //     GameObject.DestroyImmediate(_pathControl);
            //     
            // if(_prefab != null)
            //     GameObject.DestroyImmediate(_prefab);
            //
            // _onePathConfigs.Clear();
            
            // _prefab = null;
            // _config = null;
            // _pathControl = null;
            // _onePathConfigs = null;
            // _onePathConfig = null;
        }

        public void Win(int levelId)
        {
            OnePathEntryControllerModel.Instance.FinishLevel(levelId);
        }

        public void Failed(int levelId)
        {
        }
    }
}