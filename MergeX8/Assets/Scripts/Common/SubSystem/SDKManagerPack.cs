using System;
using System.Collections.Generic;
using System.Linq;
using Dlugin;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.U2D;
using Object = System.Object;

namespace Framework
{
    public class SDKManagerPack : GlobalSystem<SDKManagerPack>, IInitable, IStart
    {
        private MonoBehaviour _monoBehaviour;
        private GameObject _go = GameObjectFactory.Create(true);
        private Object x;
        public string AdvertisingId;

        public void Init()
        {
            DragonU3DSDK.DebugUtil.LogEnable = true;
            _go.name = GetType().ToString();
            _monoBehaviour = _go.AddComponent<ResourcesManager>();
            //_monoBehaviour = _go.AddComponent<VersionManager>();
            _monoBehaviour = _go.AddComponent<UnitySubject>();
            TableManager.Instance.InitLocation("configs/room"); //资源下载完成后还会加载一次，用来覆盖新数据
            // iOSATTManager.Instance.Forbid();
            SDK.GetInstance().Initialize();
            DragonU3DSDK.Utils.GetADIDByPlatformAsync(GetID);
            DownloadManager.Instance.SetMaxDownloadsCount(1);
            //x = AdventureIslandMergeBiReflection.Descriptor; // Ck7BiReflection静态方法在手机和不下断点时，不会调用。。。
        }

        public void Release()
        {
            _monoBehaviour = null;
            GameObjectFactory.Destroy(_go);
            SpriteAtlasManager.atlasRequested -= OnLoadAtlas;
        }

        void GetID(string advertisingId, bool trackingEnabled, string error)
        {
            AdvertisingId = advertisingId;
            Debug.Log("advertisingId " + advertisingId + " " + trackingEnabled + " " + error);
        }

        public void Start()
        {
            if (!NetworkSubSystem.Instance.UseNetwork)
            {
                VersionManager.Instance.StartOfflineMode();
            }


            // ResourcesManager.Instance.AddAtlasPrefix("SpriteAtlas/Home/");
            // ResourcesManager.Instance.AddAtlasPrefix("SpriteAtlas/Cooking/");
            // ResourcesManager.Instance.AddAtlasPrefix("SpriteAtlas/Room/");
            SpriteAtlasManager.atlasRequested += OnLoadAtlas;
        }


        private void OnLoadAtlas(string atlasName, Action<SpriteAtlas> act)
        {
            if (atlasName.EndsWith(".SD")) atlasName = atlasName.TrimEnd(".SD".ToArray());
            if (atlasName.EndsWith(".HD")) atlasName = atlasName.TrimEnd(".HD".ToArray());

            try
            {
                AtlasPathNode atlasPathNode = AtlasConfigController.Instance.GetAtlasPath(atlasName);
                if (atlasPathNode != null)
                {
                    SpriteAtlas sa = ResourcesManager.Instance.LoadSpriteAtlasVariant(atlasName);
                    act(sa);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                act(null);
            }
        }
    }
}