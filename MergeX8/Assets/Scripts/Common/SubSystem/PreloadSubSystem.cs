using System.Collections.Generic;
using System.IO;
using DragonU3DSDK.Asset;
using Framework;
using UnityEngine;

namespace Gameplay
{
    public enum PrefabName
    {
        //shop
        UIShopBaseBundleItem,
        UIShopBaseCommodityItem,
        UIShopBaseCommodityAbbreviationItem,
        UIShopBaseBundleCommodityItem1,
        UIShopTimeLimitedActivityBundleItem1,
        UIShopTimeLimitedActivityBundleItem2,
    }

    public class PreloadSubSystem : GlobalSystem<PreloadSubSystem>, IInitable
    {
        public Dictionary<PrefabName, GameObject> PreloadPrefabs;

        public void Init()
        {
            PreloadPrefabs = new Dictionary<PrefabName, GameObject>();
        }

        public void Release()
        {
            DragonU3DSDK.DebugUtil.Log("PreloadSubSystem Release");
            PreloadPrefabs.Clear();
            PreloadPrefabs = null;
            // manager destroyed only on scene unloading, dont call other manager
            // ResourcesManager.Instance.Clear ();
            // ResourcesManager.Instance.UnLoadAllCache ();
        }

        public GameObject GetPrefab(PrefabName resName)
        {
            GameObject obj = null;
            if (PreloadPrefabs.Count == 0) return null;
            if (PreloadPrefabs.TryGetValue(resName, out obj))
            {
                return obj;
            }

            return null;
        }

        public const string PlayerPrefs_TimeLimit_ExceptTag = "PlayerPrefs_TimeLimit_ExceptTag";

        // 0: 没有进入游戏， 1：进入游戏没有结束， 2：进入了成功通关 3：进入后失败退出
        public int ExceptTag
        {
            get { return PlayerPrefs.GetInt(PlayerPrefs_TimeLimit_ExceptTag, 0); }
            set { PlayerPrefs.SetInt(PlayerPrefs_TimeLimit_ExceptTag, value); }
        }
    }
}