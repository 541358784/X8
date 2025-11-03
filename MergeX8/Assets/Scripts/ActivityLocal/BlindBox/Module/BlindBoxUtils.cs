using System;
using System.Collections.Generic;
using System.Linq;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public static class BlindBoxUtils
{
    public static UIBlindBoxThemeBase OpenThemeView(this StorageBlindBox storage)
    {
        var path = "BlindBox/Theme" + storage.ThemeId + "/UIBlindBox"+storage.ThemeId;
        var openView = UIManager.Instance.GetOpenedUIByPath<UIBlindBoxThemeBase>(path);
        if (openView)
            return openView;
        openView = UIManager.Instance.OpenUI(path,storage) as UIBlindBoxThemeBase;
        return openView;
    }
    public static UIBlindBoxRewardBase OpenThemeRewardView(this StorageBlindBox storage)
    {
        var path = "BlindBox/Theme"+storage.ThemeId+"/UIPopupBlindBoxReward";
        var openView = UIManager.Instance.GetOpenedUIByPath<UIBlindBoxRewardBase>(path);
        if (openView)
            return openView;
        openView = UIManager.Instance.OpenUI(path,storage) as UIBlindBoxRewardBase;
        return openView;
    }
    public static UIBlindBoxOpenBoxBase OpenThemeOpenBoxView(this StorageBlindBox storage,BlindBoxItemConfig config,Action callback = null)
    {
        var path = "BlindBox/Theme"+storage.ThemeId+"/UIBlindBoxOpen";
        var openView = UIManager.Instance.GetOpenedUIByPath<UIBlindBoxOpenBoxBase>(path);
        if (openView)
            return openView;
        openView = UIManager.Instance.OpenUI(path,storage,config,callback) as UIBlindBoxOpenBoxBase;
        return openView;
    }
    public static UIBlindBoxPreviewBase OpenThemePreviewView(this StorageBlindBox storage,BlindBoxItemConfig config)
    {
        var path = "BlindBox/Theme"+storage.ThemeId+"/UIBlindBoxPreview";
        var openView = UIManager.Instance.GetOpenedUIByPath<UIBlindBoxPreviewBase>(path);
        if (openView)
            return openView;
        openView = UIManager.Instance.OpenUI(path,storage,config) as UIBlindBoxPreviewBase;
        return openView;
    }

    public static BlindBoxItemConfig OpenBlindBox(this StorageBlindBox storage)
    {
        return BlindBoxModel.Instance.OpenBlindBox(storage);
    }

    public static bool IsResReady(this StorageBlindBox storage)
    {
        return BlindBoxModel.Instance.IsResReady(storage);
    }

    public static void TryDownloadRes(this StorageBlindBox storage,Action<float, string> onProgressUpdate = null,Action<bool> onFinish = null)
    {
        BlindBoxModel.Instance.TryDownloadRes(storage,onProgressUpdate,onFinish);
    }

    public static List<BlindBoxItemConfig> GetSpecialItemConfigs(this BlindBoxThemeConfig themeConfig)
    {
        var configs = new List<BlindBoxItemConfig>();
        foreach (var item in themeConfig.ItemList)
        {
            var itemConfig = BlindBoxModel.Instance.ItemConfigDic[item];
            if (itemConfig.IsSpecial)
                configs.Add(itemConfig);
        }
        return configs;
    }
    public static List<BlindBoxItemConfig> GetNormalItemConfigs(this BlindBoxThemeConfig themeConfig)
    {
        var configs = new List<BlindBoxItemConfig>();
        foreach (var item in themeConfig.ItemList)
        {
            var itemConfig = BlindBoxModel.Instance.ItemConfigDic[item];
            if (!itemConfig.IsSpecial)
                configs.Add(itemConfig);
        }
        return configs;
    }

    public static bool CollectItem(this StorageBlindBox storage,BlindBoxItemConfig itemConfig)
    {
        var isNew = storage.CollectItems.TryAdd(itemConfig.Id,0);
        storage.CollectItems[itemConfig.Id]++;
        if (isNew)
        {
            BlindBoxModel.Instance.CheckCollectAllItem(storage);
        }
        return isNew;
    }

    public static bool HasCollectGroup(this StorageBlindBox storage,BlindBoxGroupConfig groupConfig)
    {
        return storage.CollectGroups.Contains(groupConfig.Id);
    }

    public static bool CanCollectGroup(this StorageBlindBox storage, BlindBoxGroupConfig groupConfig)
    {
        if (storage.HasCollectGroup(groupConfig))
            return false;
        foreach (var item in groupConfig.ItemList)
        {
            if (!storage.CollectItems.ContainsKey(item))
                return false;
        }
        return true;
    }

    public static bool CollectGroupReward(this StorageBlindBox storage, BlindBoxGroupConfig groupConfig)
    {
        if (!storage.CanCollectGroup(groupConfig))
        {
            return false;
        }
        storage.CollectGroups.Add(groupConfig.Id);
        var rewards = CommonUtils.FormatReward(groupConfig.RewardId, groupConfig.RewardNum);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BlindBoxGet
        };
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            reason);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBlindBoxGetReward,groupConfig.Id.ToString());
        return true;
    }

    public static string GetAtlasName(this BlindBoxThemeConfig themeConfig)
    {
        return "BlindBoxTheme" + themeConfig.Id + "Atlas";
    }
    public static Sprite GetItemSprite(this BlindBoxItemConfig itemConfig, bool isGray = false)
    {
        var themeConfig = BlindBoxModel.Instance.ThemeConfigDic[itemConfig.ThemeId];
        var atlas = themeConfig.GetAtlasName();
        return ResourcesManager.Instance.GetSpriteVariant(atlas, isGray ? itemConfig.GrayImage : itemConfig.Image);
    }
    public static List<AssetGroup> GetDownloadAssetGroups(this BlindBoxThemeConfig themeConfig)
    {
        var assets = new List<AssetGroup>();
        var variantPostFix = ResourcesManager.Instance.IsSdVariant ? "sd" : "hd";
        assets.Add(new AssetGroup("BlindBox", "spriteatlas/blindbox/blindboxtheme"+themeConfig.Id+"atlas/"+variantPostFix+".ab"));
        assets.Add(new AssetGroup("BlindBox", "prefabs/blindbox/theme"+themeConfig.Id+".ab"));
            
        return assets;
    }
    public static Dictionary<string, string> GetNeedDownloadAssets(this BlindBoxThemeConfig themeConfig)
    {
        List<AssetGroup> resGroupList = themeConfig.GetDownloadAssetGroups();
        if (resGroupList == null) 
            return new Dictionary<string, string>();
        return AssetCheckManager.Instance.ResDownloadFitter(resGroupList,true);
    }

    public static int GetRecycleCount(this StorageBlindBox storage,out int boxCount)
    {
        boxCount = 0;
        var value = 0;
        var keys = storage.CollectItems.Keys.ToList();
        for (var i=0;i<keys.Count;i++)
        {
            var item = keys[i];
            var count = storage.CollectItems[item];
            if (count > 1)
            {
                var itemConfig = BlindBoxModel.Instance.ItemConfigDic[item];
                value += itemConfig.RecycleValue * (count - 1);
                boxCount += count - 1;
            }
        }
        return value;
    }
    public static void RecycleRepeatItem(this StorageBlindBox storage)
    {
        var value = 0;
        var keys = storage.CollectItems.Keys.ToList();
        var boxCount = 0;
        var extraData = new Dictionary<string, string>();
        for (var i=0;i<keys.Count;i++)
        {
            var item = keys[i];
            var count = storage.CollectItems[item];
            if (count > 1)
            {
                var itemConfig = BlindBoxModel.Instance.ItemConfigDic[item];
                value += itemConfig.RecycleValue * (count - 1);
                storage.CollectItems[item] = 1;
                boxCount += count - 1;
                extraData.Add(item.ToString(),(count - 1).ToString());
            }
        }
        BlindBoxModel.Instance.StorageGlobal.RecycleValue += value;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBlindBoxStarGet,
            value.ToString(),boxCount.ToString(),storage.ThemeId.ToString(),extraData);
    }
    public static bool BuyRecycleShopItem(this BlindBoxRecycleShopConfig shopConfig)
    {
        if (BlindBoxModel.Instance.StorageGlobal.RecycleValue < shopConfig.RecyclePrice)
            return false;
        BlindBoxModel.Instance.StorageGlobal.RecycleValue -= shopConfig.RecyclePrice;
        var rewards = new List<ResData>() { new ResData(shopConfig.BoxId, 1) };
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.BlindBoxGet);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, false,
            reason);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBlindBoxStarExchange,
            shopConfig.RecyclePrice.ToString(),shopConfig.BoxId.ToString(),BlindBoxModel.Instance.StorageGlobal.RecycleValue.ToString());
        return true;
    }
}