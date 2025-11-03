using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonPlus.Config.TMatchShop;
// using DragonPlus.Config.Game;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{


    public sealed class ResUtil
    {
        static private Dictionary<ResourceId, Sprite> _cacheBigIcon = new Dictionary<ResourceId, Sprite>();
        static private Dictionary<ResourceId, Sprite> _cacheSmallIcon = new Dictionary<ResourceId, Sprite>();
        static private Dictionary<ResourceId, Sprite> _cacheSpecialIcon = new Dictionary<ResourceId, Sprite>();


        // public static ResourceId ConvertKeyType(int keyType)
        // {
        //     switch (keyType)
        //     {
        //         case 502:
        //             return ResourceId.NewKey;
        //     }
        //
        //     return ResourceId.Star;
        // }

        public static Sprite GetIcon(ResourceId resouceId, ResourceIconType iconType = ResourceIconType.Normal)
        {
            Dictionary<ResourceId, Sprite> cache = null;
            switch (iconType)
            {
                case ResourceIconType.Normal:
                    cache = _cacheSmallIcon;
                    break;
                case ResourceIconType.Big:
                    cache = _cacheBigIcon;
                    break;
                case ResourceIconType.Special:
                    cache = _cacheSpecialIcon;
                    break;
            }

            Sprite icon = null;
            if (cache.TryGetValue(resouceId, out icon))
            {
                return icon;
            }

            var tableItems = TMatchShopConfigManager.Instance.ItemConfigList;
            var tableGoods = tableItems.Find(config => config.id == (int) resouceId);
            if (tableGoods == null)
            {
                DebugUtil.LogError("GetIcon Error resouceId:" + (int) resouceId);
                return null;
            }

            var iconName = string.Empty;
            switch (iconType)
            {
                case ResourceIconType.Normal:
                    iconName = tableGoods.pic_res;
                    break;
                case ResourceIconType.Big:
                    iconName = tableGoods.pic_res_big;
                    break;
                case ResourceIconType.Special:
                    iconName = tableGoods.pic_res_special;
                    break;
            }

            try
            {
                var goodsIcon = ResourcesManager.Instance.GetSpriteVariant(AtlasName.HPCommon, iconName);
                cache.Add(resouceId, goodsIcon);
                return goodsIcon;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError($"获取icon异常 resouceId:{resouceId} Message:{e.Message}");
            }

            return null;
        }

        // /// <summary>
        // /// for UI
        // /// </summary>
        // /// <param name="rewardId">GameConfigManager.Instance.RewardsConfigs[rewardsConfigIndex]</param>
        // /// <param name="num">reward num</param>
        // /// <returns></returns>
        // public static string ParseRewardNumText(ResourceId rewardId, uint num)
        // {
        //     var config = ItemModel.Instance.GetConfigById((int) rewardId);
        //     return ParseRewardNumText(config, (int) num);
        // }

        // /// <summary>
        // /// https://jira.dragonplus.com/browse/CK4-318
        // /// </summary>
        // /// <param name="itemsConfig"></param>
        // /// <param name="num"></param>
        // /// <returns></returns>
        // public static string ParseRewardNumText(ItemConfig itemsConfig, int num)
        // {
        //     if (itemsConfig == null) return "";
        //
        //     if (itemsConfig.id == (int) ResourceId.Energy_Infinity)
        //     {
        //         if (num % 3600 == 0)
        //         {
        //             var hour = num / 3600;
        //             var text = hour + LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_h");
        //             return text;
        //         }
        //         else
        //         {
        //             var minute = num / 60;
        //             var text = minute + LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_m");
        //             return text;
        //         }
        //     }
        //     else
        //     {
        //         return $"{num}";
        //     }
        // }

        // public static bool IsCKBoost(int resourceId)
        // {
        //     // return resourceId > (int)ResourceId.None && resourceId < (int)ResourceId.CK_ITEM_MAX;
        //     return false;
        // }
        //
        // private static GameObject getResFlyPrefab(ResourceId resourceId, Sprite iconSprite)
        // {
        //     var prefab = TMatchModel.ObjectPoolMgr.SpawnGameObject(GlobalPrefabPath.FlyItemPrefab);
        //     var imageTransform = prefab.transform.Find("KeyImage");
        //     if (resourceId != ResourceId.None)
        //     {
        //         imageTransform.GetComponent<Image>().sprite = ResUtil.GetIcon(resourceId, ResourceIconType.Big);
        //     }
        //
        //     if (iconSprite != null)
        //     {
        //         imageTransform.GetComponent<Image>().sprite = iconSprite;
        //     }
        //
        //     return prefab;
        // }

        // public static IResBarHost GetResBarHost(ResHostUI flyUI)
        // {
        //     IResBarHost currencyBar = null;
        //     switch (flyUI)
        //     {
        //         case ResHostUI.Shop:
        //             // currencyBar = UIManager.Instance.GetOpenedWindow<StoreUIController>();
        //             break;
        //         case ResHostUI.MainUI:
        //             // currencyBar = UIManager.Instance.GetOpenedWindow<UIWorldMainController>();
        //             break;
        //         case ResHostUI.LevelWinUI:
        //             // currencyBar = UIManager.Instance.GetOpenedWindow<LevelWinUIController>();
        //             break;
        //         case ResHostUI.BeginnerHelp:
        //             // currencyBar = UIManager.Instance.GetOpenedWindow<CookingFundMainController>();
        //             break;
        //         case ResHostUI.DailyLoginMission:
        //             // currencyBar = UIManager.Instance.GetOpenedWindow<UIWorldMainController>();
        //             break;
        //         // case ResHostUI.FoodModelUI:
        //         //     currencyBar = UIManager.Instance.GetOpenedWindow<UIFoodModelController>();
        //         //     break;
        //         // case ResHostUI.Gacha:
        //         //     currencyBar = UIManager.Instance.GetOpenedWindow<GachaponController>();
        //         //     break;
        //         // case ResHostUI.FoodModelDetailPopup:
        //         //     currencyBar = UIManager.Instance.GetOpenedWindow<UIAwardDetailsPopupController>();
        //         //     break;
        //         default:
        //             DebugUtil.LogError("GetResBarHost 类型未处理:" + flyUI.ToString());
        //             break;
        //     }
        //
        //     return currencyBar;
        // }

        // public static void FlyToResBar(ResHostUI hostUI, ResourceId resId, int count, Transform sourceTransform,
        //     Action onFinish, int ignoreNumber = 0, Sprite iconSprite = null)
        // {
        //     if (hostUI == ResHostUI.None) Debug.LogError("ResHostUI.None");
        //
        //     var prefab = getResFlyPrefab(resId, iconSprite);
        //     var imageTransform = prefab.transform.Find("KeyImage");
        //
        //     var resBar = GetResBarHost(hostUI);
        //     if (resBar == null) return;
        //     if (sourceTransform == null) return;
        //     var targetTransfrom = resBar.GetResTransform(resId);
        //     if (targetTransfrom != null)
        //     {
        //         resBar.FlyFromBegin();
        //         var prefabSizeX = (imageTransform.transform as RectTransform).rect.width;
        //         var scaleTo = (targetTransfrom.transform as RectTransform).rect.width / prefabSizeX;
        //         var scaleFrom = (sourceTransform as RectTransform).rect.width / prefabSizeX;
        //         count = count > 3 ? 3 : count;
        //         FlyEffectManager.Instance.Fly(sourceTransform, targetTransfrom, prefab, null, resBar.flyParentTransform,
        //             () =>
        //             {
        //                 onFinish?.Invoke();
        //                 resBar.FlyEnd();
        //                 EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(resId, hostUI, ignoreNumber));
        //             }, count, 0.7f, scaleFrom: scaleFrom, scaleTo: scaleTo);
        //     }
        //
        //     MyMain.myGame.ObjectPoolMgr.RecycleGameObject(prefab);
        // }
        //
        // public static void FlyResToTarget(ResHostUI hostUI, ResourceId resId, int count, Transform targetTransform,
        //     Action onFinish, int ignoreNumber = 0)
        // {
        //     if (hostUI == ResHostUI.None) Debug.LogError("ResHostUI.None");
        //
        //     var prefab = getResFlyPrefab(resId, null);
        //     var imageTransform = prefab.transform.Find("KeyImage");
        //
        //     var resBar = GetResBarHost(hostUI);
        //     if (resBar == null) return;
        //     if (targetTransform == null) return;
        //     var srcTransfrom = resBar.GetResTransform(resId);
        //     if (srcTransfrom != null)
        //     {
        //         count = count > 3 ? 3 : count;
        //         EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(resId, hostUI, ignoreNumber));
        //         resBar.FlyToBegin();
        //         var prefabSizeX = (imageTransform.transform as RectTransform).rect.width;
        //         var scaleFrom = (srcTransfrom.transform as RectTransform).rect.width / prefabSizeX;
        //         var scaleTo = (targetTransform as RectTransform).rect.width / prefabSizeX;
        //         FlyEffectManager.Instance.Fly(srcTransfrom, targetTransform, prefab, null, resBar.flyParentTransform,
        //             () =>
        //             {
        //                 resBar.FlyEnd();
        //                 onFinish?.Invoke();
        //             }, count, 0.7f, scaleFrom: scaleFrom, scaleTo: scaleTo);
        //     }
        //
        //     MyMain.myGame.ObjectPoolMgr.RecycleGameObject(prefab);
        // }

        // public static void FlyResToTarget(ResHostUI hostUI, ResourceId resId, int count, Transform sourceTransform,
        //     Transform targetTransform,
        //     Action onFinish, int ignoreNumber = 0)
        // {
        //     if (hostUI == ResHostUI.None) Debug.LogError("ResHostUI.None");
        //
        //     var prefab = getResFlyPrefab(resId, null);
        //     var imageTransform = prefab.transform.Find("KeyImage");
        //
        //     var resBar = GetResBarHost(hostUI);
        //     if (resBar == null) return;
        //     if (sourceTransform == null || targetTransform == null) return;
        //     count = count > 3 ? 3 : count;
        //     EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(resId, hostUI, ignoreNumber));
        //     resBar.FlyToBegin();
        //     var prefabSizeX = (imageTransform.transform as RectTransform).rect.width;
        //     var scaleFrom = (sourceTransform.transform as RectTransform).rect.width / prefabSizeX;
        //     var scaleTo = (targetTransform as RectTransform).rect.width / prefabSizeX;
        //     FlyEffectManager.Instance.Fly(sourceTransform, targetTransform, prefab, null, resBar.flyParentTransform,
        //         () =>
        //         {
        //             resBar.FlyEnd();
        //             onFinish?.Invoke();
        //         }, count, 0.7f, scaleFrom: scaleFrom, scaleTo: scaleTo);
        //
        //     MyMain.myGame.ObjectPoolMgr.RecycleGameObject(prefab);
        // }
    }

    public enum ResHostUI
    {
        None,

        /// <summary>
        /// 普通商城
        /// </summary>
        Shop,

        /// <summary>
        /// 主界面
        /// </summary>
        MainUI,

        /// <summary>
        /// 游戏获胜
        /// </summary>
        LevelWinUI,

        /// <summary>
        /// 新手助手界面
        /// </summary>
        BeginnerHelp,

        /// <summary>
        /// 门票商城
        /// </summary>
        SpringDayShopUI,

        /// <summary>
        /// 春日活动胜利界面
        /// </summary>
        SpringDayLevelWin,

        /// <summary>
        /// 春日活动开始界面
        /// </summary>
        SpringDayStartUI,

        /// <summary>
        /// 夏日活动开始界面
        /// </summary>
        SummerDayStartUI,

        /// <summary>
        /// 夏日活动胜利界面
        /// </summary>
        SummerDayLevelWin,

        /// <summary>
        /// 夏日活动失败UI
        /// </summary>
        SummerDayLevelFailedUI,

        /// <summary>
        /// 夏日活动主UI
        /// </summary>
        SummerDayMainUI,

        /// <summary>
        /// 夏日活动商店UI
        /// </summary>
        SummerDayShopUI,

        /// <summary>
        /// 轮盘抽奖界面
        /// </summary>
        RouletteDraw,

        /// <summary>
        /// 每日登录闯关
        /// </summary>
        DailyLoginMission,

        /// <summary>
        /// 模型界面
        /// </summary>
        FoodModelUI,

        /// <summary>
        /// 模型详情界面
        /// </summary>
        FoodModelDetailPopup,

        /// <summary>
        /// 扭蛋机界面
        /// </summary>
        Gacha,
    }

    public interface IResBarHost
    {
        Transform GetResTransform(ResourceId resourceId);
        void FlyFromBegin();
        void FlyToBegin();
        void FlyEnd();
        void OnNumberAnimationEnd();
        Transform flyParentTransform { get; }
    }
}