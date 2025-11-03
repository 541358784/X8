using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace GamePool
{
    public partial class ObjectPoolName
    {
        public static string CommonHintStars = "Prefabs/Effects/vfx_hint_stars_001";
        public static string MergeItem = "Prefabs/Merge/MergeItem";
        public static string ResourceItem = "Prefabs/Common/ResourceItem";
        public static string CommonTrail = "Prefabs/Effects/vfx_Trail_0";
        public static string CommonOpen = "Prefabs/Effects/vfx_Open";
        public static string CommonNum = "Prefabs/Home/UIEffectGroup";
        public static string PortraitSpine = "Prefabs/PortraitSpine/{0}";
        public static string GiftTitlePath = "Prefabs/Shop/GiftTitle";
        public static string GameGiftTitlePath = "Prefabs/Shop/GameGiftTitle";
        public static string ShopItemPig = "Prefabs/Shop/ShopItemPig";
        public static string ShopItemSeal = "Prefabs/Shop/ShopItemSeal";
        public static string ShopItemDolphin = "Prefabs/Shop/ShopItemDolphin";
        public static string ShopItemBattlePass = "Prefabs/Shop/ShopItemBattlePass";
        public static string ShopItemBattlePass_2 = "Prefabs/Shop/ShopItemBattlePass_2";
        public static string ShopItemEaster = "Prefabs/Shop/ShopItemEaster";
        public static string ShopItemNomalPath = "Prefabs/Shop/ShopItemNomal";
        public static string ShopItemBundleNomalPath = "Prefabs/Shop/ShopItemBundleNormal";
        public static string ShopItemBundleNormal1Path = "Prefabs/Shop/ShopItemBundleNormal1";
        public static string TaskAssistBundlePath = "Prefabs/Shop/TaskAssistBundle";
        public static string FishBundlePath = "Prefabs/Shop/FishBundle";
        public static string ShopItemDailyPath = "Prefabs/Shop/ShopItemDaily";
        public static string ShopItemFlashPath = "Prefabs/Shop/ShopItemFlash";
        public static string ShopItemExchangePath = "Prefabs/Shop/ShopItemExchange";
        public static string MergePackageUnit = "Prefabs/Merge/MergePackageUnit";
        public static string CommonBgEffect = "Prefabs/Effects/vfx_Background_004";
        //public static string LevelRankTips = "Prefabs/Home/LevelRankTips";
        //public static string PopRewardTips = "Prefabs/Home/PopRewardTips";
        
        public static string vfx_ComboMerge_01 = "Prefabs/Effects/vfx_ComboMerge_01";
        public static string vfx_ComboMerge_02 = "Prefabs/Effects/vfx_ComboMerge_02";
        public static string vfx_ComboMerge_03 = "Prefabs/Effects/vfx_ComboMerge_03";
        public static string vfx_ComboMerge_04 = "Prefabs/Effects/vfx_ComboMerge_04";
        
        public static string vexLeaf01 = "Prefabs/Effects/vexLeaf01";
        public static string vexLeaf02 = "Prefabs/Effects/vexLeaf02";
        public static string vexSmoke = "Prefabs/Effects/vexSmoke";
        public static string vexFlower = "Prefabs/Effects/vexFlower";
        public static string vexClean = "Prefabs/Effects/vexClean";

        public static string RecoverCoinStar = "Prefabs/Activity/RecoverCoin/FlyItem";
        
        public static string HappyGoGiftTitlePath = "Prefabs/Activity/HappyGo/HappyGoGiftTitle";
        public static string HappyGoShopBundlePath = "Prefabs/Activity/HappyGo/HappyGoShopBundle1";
        public static string HappyGoShopBundlePath2 = "Prefabs/Activity/HappyGo/HappyGoShopBundle2";
        public static string HappyGoShopFlashPath = "Prefabs/Activity/HappyGo/HappyGoShopNomal";
        
    }

    public static class ObjectPoolDelegate
    {
        private static string lastObjectPath;

        private static GameObject gameItem = null;

        public static GameObject CreateGameItem(string path)
        {
            if (gameItem == null || string.IsNullOrEmpty(lastObjectPath) || path != lastObjectPath)
            {
                gameItem = DragonU3DSDK.Asset.ResourcesManager.Instance.LoadResource<GameObject>(path);
                lastObjectPath = path;
            }


            if (gameItem == null)
                return null;

            return GameObject.Instantiate(gameItem);
        }
    }
}