using System;
using System.Collections.Generic;
using System.Linq;
using Decoration;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using TMatch;
using UnityEngine;

public enum CardPackageWeightType
{
    Free = 1,
    Pay = 2,
    Bang = 3,
}

public enum GetCardSource
{
    Debug,
    CardPackage,
    WildCard,
    MergeOnView,
    TeamCardPackage,
}
public static class CardCollectionUtils
{
    public static CardCollectionCardThemeState GetUpGradeTheme(this CardCollectionCardThemeState themeState)
    {
        if (themeState == null)
            return null;
        var theme = themeState;
        while (theme.CardThemeConfig.UpGradeTheme > 0 && 
               theme.IsCompleted && 
               CardCollectionModel.Instance.StorageCardCollection.UnlockThemeList.Contains(theme.CardThemeConfig.UpGradeTheme))
        {
            theme = CardCollectionModel.Instance.GetCardThemeState(theme.CardThemeConfig.UpGradeTheme);
        }
        return theme;
    }
    public static TableCardCollectionCardPackage GetCardPackageConfig(this CardCollectionCardThemeState themeState,UserData.ResourceId packageResourceId)
    {
        if (themeState == null)
            return null;
        var level = 0;
        var weightType = CardPackageWeightType.Free;
        switch (packageResourceId)
        {
            case UserData.ResourceId.CardPackageFreeLevel1:
                level = 1;
                weightType = CardPackageWeightType.Free;
                break;
            case UserData.ResourceId.CardPackageFreeLevel2:
                level = 2;
                weightType = CardPackageWeightType.Free;
                break;
            case UserData.ResourceId.CardPackageFreeLevel3:
                level = 3;
                weightType = CardPackageWeightType.Free;
                break;
            case UserData.ResourceId.CardPackageFreeLevel4:
                level = 4;
                weightType = CardPackageWeightType.Free;
                break;
            case UserData.ResourceId.CardPackageFreeLevel5:
                level = 5;
                weightType = CardPackageWeightType.Free;
                break;
            case UserData.ResourceId.CardPackagePayLevel1:
                level = 1;
                weightType = CardPackageWeightType.Pay;
                break;
            case UserData.ResourceId.CardPackagePayLevel2:
                level = 2;
                weightType = CardPackageWeightType.Pay;
                break;
            case UserData.ResourceId.CardPackagePayLevel3:
                level = 3;
                weightType = CardPackageWeightType.Pay;
                break;
            case UserData.ResourceId.CardPackagePayLevel4:
                level = 4;
                weightType = CardPackageWeightType.Pay;
                break;
            case UserData.ResourceId.CardPackagePayLevel5:
                level = 5;
                weightType = CardPackageWeightType.Pay;
                break;
            case UserData.ResourceId.CardPackagePangLevel1:
                level = 1;
                weightType = CardPackageWeightType.Bang;
                break;
            case UserData.ResourceId.CardPackagePangLevel2:
                level = 2;
                weightType = CardPackageWeightType.Bang;
                break;
            case UserData.ResourceId.CardPackagePangLevel3:
                level = 3;
                weightType = CardPackageWeightType.Bang;
                break;
            case UserData.ResourceId.CardPackagePangLevel4:
                level = 4;
                weightType = CardPackageWeightType.Bang;
                break;
            case UserData.ResourceId.CardPackagePangLevel5:
                level = 5;
                weightType = CardPackageWeightType.Bang;
                break;
            default:
                return null;
        }

        foreach (var packageId in themeState.CardThemeConfig.CardPackages)
        {
            var package = CardCollectionModel.Instance.TableCardPackage[packageId];
            if (package.Level == level && (CardPackageWeightType)package.WeightType == weightType)
                return package;
        }
        return null;
    }

    public static Sprite GetCardBackSprite(this CardCollectionCardThemeState themeState)
    {
        var themeConfig = themeState.CardThemeConfig;
        return ResourcesManager.Instance.GetSpriteVariant(themeConfig.SpriteAtlas, "CardBG63");
    }
    public static Sprite GetWildCardSprite(this TableCardCollectionWildCard wildCardConfig)
    {
        return ResourcesManager.Instance.GetSpriteVariant($"CardMainAtlas",wildCardConfig.IconSprite);
    }
    
    public static Sprite GetCardPackageSprite(this TableCardCollectionCardPackage packageConfig)
    {
        int themeId = packageConfig.ThemeId;
        var themeConfig = CardCollectionModel.Instance.GetCardThemeState(themeId).CardThemeConfig;
        return ResourcesManager.Instance.GetSpriteVariant(themeConfig.SpriteAtlas,packageConfig.IconSprite);
    }
    public static Sprite GetCommonCardPackageSprite(this TableCardCollectionCardPackage packageConfig)
    {
        var weightType = (CardPackageWeightType)packageConfig.WeightType;
        UserData.ResourceId resourceId = UserData.ResourceId.None;
        if (weightType == CardPackageWeightType.Free)
        {
            resourceId = (UserData.ResourceId)(910 + packageConfig.Level);
        }
        else if (weightType == CardPackageWeightType.Pay)
        {
            resourceId = (UserData.ResourceId)(915 + packageConfig.Level);
        }
        else if (weightType == CardPackageWeightType.Bang)
        {
            resourceId = (UserData.ResourceId)(932 + packageConfig.Level);
        }
        return  UserData.GetResourceIcon(resourceId, UserData.ResourceSubType.Big);
    }

    public static int GetWildCardMaxLevel(this TableCardCollectionWildCard wildCardConfig)
    {
        var maxLevel = 0;
        for (var i = 0; i < wildCardConfig.LevelRange.Count; i++)
        {
            if (maxLevel < wildCardConfig.LevelRange[i])
            {
                maxLevel = wildCardConfig.LevelRange[i];
            }
        }
        return maxLevel;
    }

    public static List<StorageCardCollectionCardPackage> GetUnOpenCardPackages(this CardCollectionCardThemeState themeState)
    {
        var unOpenCardPackages = new List<StorageCardCollectionCardPackage>();
        if (themeState == null)
            return unOpenCardPackages;
        for (var i = 0; i < CardCollectionModel.Instance.StorageCardCollection.UnOpenPackageList.Count; i++)
        {
            var cardPackageStorage = CardCollectionModel.Instance.StorageCardCollection.UnOpenPackageList[i];
            var cardPackageConfig = CardCollectionModel.Instance.TableCardPackage[cardPackageStorage.Id];
            var themeId = cardPackageConfig.ThemeId;
            if (cardPackageConfig.Id == 999999)
                themeId = CardCollectionModel.Instance.ThemeInUse != null ? CardCollectionModel.Instance.ThemeInUse.GetUpGradeTheme().CardThemeConfig.Id : 1;
            if (themeId == themeState.CardThemeConfig.Id)
            {
                unOpenCardPackages.Add(cardPackageStorage);
            }
        }
        return unOpenCardPackages;
    }
    
    public static int GetStarCount(this CardCollectionCardThemeState themeState)
    {
        if (themeState == null)
            return 0;
        var starCount = 0;
        foreach (var bookPair in themeState.CardBookStateList)
        {
            foreach (var itemPair in bookPair.Value.CardItemStateList)
            {
                var cardItem = itemPair.Value;
                if (cardItem.CollectCount > 1)
                {
                    starCount += (cardItem.CollectCount - 1) * cardItem.CardItemConfig.StarValue;
                }
            }
        }
        return starCount;
    }

    private static readonly Dictionary<CardCollectionCardThemeState, Dictionary<int, List<CardCollectionCardItemState>>>
        ThemeCardItemConfigGroupByStarValueDic =
            new Dictionary<CardCollectionCardThemeState, Dictionary<int, List<CardCollectionCardItemState>>>();

    public static Dictionary<int, List<CardCollectionCardItemState>> GetCardItemConfigGroupByStarValue(this CardCollectionCardThemeState themeState)
    {
        if (!ThemeCardItemConfigGroupByStarValueDic.TryGetValue(themeState, out var config))
        {
            config = new Dictionary<int, List<CardCollectionCardItemState>>();
            foreach (var bookPair in themeState.CardBookStateList)
            {
                foreach (var itemPair in bookPair.Value.CardItemStateList)
                {
                    var cardItem = itemPair.Value;
                    if (!config.TryGetValue(cardItem.CardItemConfig.StarValue,out var itemList))
                    {
                        itemList = new List<CardCollectionCardItemState>();
                        config.Add(cardItem.CardItemConfig.StarValue,itemList);
                    }
                    itemList.Add(cardItem);
                }
            }
            ThemeCardItemConfigGroupByStarValueDic.Add(themeState,config);
        }
        return config;
    }

    public static int GetFreeWeight(this TableCardCollectionCardItem config,StorageCardCollectionCardPackage package)
    {
        var storage = CardCollectionActivityModel.Instance.CurStorage;
        if (storage == null)
            return config.FreeWeight1;
        if (storage.FreeWeightTime1 <= 0)
            return config.FreeWeight1;
        if ((long)package.GetTime > storage.FreeWeightTime1)
            return config.FreeWeight2;
        else
            return config.FreeWeight1;
    }
    public static int GetPayWeight(this TableCardCollectionCardItem config,StorageCardCollectionCardPackage package)
    {
        var storage = CardCollectionActivityModel.Instance.CurStorage;
        if (storage == null)
            return config.PayWeight1;
        if (storage.FreeWeightTime1 <= 0)
            return config.PayWeight1;
        if ((long)package.GetTime > storage.FreeWeightTime1)
            return config.PayWeight2;
        else
            return config.PayWeight1;
    }
    public static List<AssetGroup> GetDownloadAssetGroups(this TableCardCollectionTheme themeConfig)
    {
        var assets = new List<AssetGroup>();
        var variantPostFix = ResourcesManager.Instance.IsSdVariant ? "sd" : "hd";
        assets.Add(new AssetGroup("CardCollection", "spriteatlas/card"+themeConfig.Id+"atlas/"+variantPostFix+".ab"));
        assets.Add(new AssetGroup("CardCollection", "prefabs/card"+themeConfig.Id+".ab"));
            
        return assets;
    }

    public static int GetPoolLeftCardCount(this StorageCardCollectionRandomGroup randomGroup)
    {
        var leftCount = 0;
        foreach (var pair in randomGroup.Pool)
        {
            leftCount += pair.Value;
        }
        return leftCount;
    }
}