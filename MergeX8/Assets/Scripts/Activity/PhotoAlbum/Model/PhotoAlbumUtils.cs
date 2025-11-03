using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.PhotoAlbum;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public enum PhotoAlbumStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class PhotoAlbumUtils
{
    public static PhotoAlbumModel Model => PhotoAlbumModel.Instance;
    
    public static bool BuyStoreItem(this StoragePhotoAlbum storage, PhotoAlbumStoreItemConfig storeItemConfig)
    {
        if (storage.Score < storeItemConfig.Price)
            return false;
        if (!PhotoAlbumModel.Instance.ReduceScore(storeItemConfig.Price, "BuyItem"))
            return false;
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventPhotoAlbumRadishExchange,
            storeItemConfig.Id.ToString(), storage.GetCurStoreLevel().Id.ToString(), storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);
        if ((PhotoAlbumStoreItemType) storeItemConfig.Type == PhotoAlbumStoreItemType.BuildItem)
        {
            var photoConfig = PhotoAlbumModel.Instance.PhotoConfig[storeItemConfig.RewardId[0]];
            var pieceId= PhotoAlbumModel.Instance.GetRandomPhotoPiece(photoConfig);
            var isFull = PhotoAlbumModel.Instance.CollectPhotoPiece(pieceId);
            var mainUI = UIPhotoAlbumShopController.Instance;
            if (mainUI)
                mainUI.PerformCollectPhotoPiece(pieceId, isFull);
        }
        else
        {
            var rewardData = CommonUtils.FormatReward(storeItemConfig.RewardId, storeItemConfig.RewardNum);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.PhotoAlbumGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs);
            foreach (var reward in rewardData)
            {
                if (!UserData.Instance.IsResource(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonPhotoAlbumGet,
                        itemAId =reward.id,
                        isChange = true,
                    });
                }
            }
        }

        EventDispatcher.Instance.SendEventImmediately(new EventPhotoAlbumBuyStoreItem(storeItemConfig));
        return true;
    }

    public static PhotoAlbumStoreLevelConfig GetCurStoreLevel(this StoragePhotoAlbum storage)
    {
        if (!PhotoAlbumModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = PhotoAlbumModel.Instance.StoreLevelConfig;
        for (var i = 0; i < levelConfig.Count; i++)
        {
            var storeItemList = levelConfig[i].StoreItemList;
            for (var i1 = 0; i1 < storeItemList.Count; i1++)
            {
                if (!storage.FinishStoreItemList.Contains(storeItemList[i1]))
                {
                    return levelConfig[i];
                }
            }
        }

        return levelConfig.Last();
    }

    public static long GetPreheatLeftTime(this StoragePhotoAlbum storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StoragePhotoAlbum storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StoragePhotoAlbum storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }

    public static bool IsTimeOut(this StoragePhotoAlbum storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftPreEndTime(this StoragePhotoAlbum storageWeek)
    {
        return Math.Max(storageWeek.PreEndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static string GetLeftPreEndTimeText(this StoragePhotoAlbum storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftPreEndTime());
    }
    public static void SetLeftPreEndTime(this StoragePhotoAlbum storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.PreEndTime = endTime;
    }
    
    public static long GetLeftTime(this StoragePhotoAlbum storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StoragePhotoAlbum storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StoragePhotoAlbum storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    
    public static bool ShowAuxItem(this StoragePhotoAlbum storage)
    {
        if (!PhotoAlbumModel.Instance.IsOpened())
            return false;
        if (storage.IsTimeOut())
            return false;
        if (storage.GetLeftPreEndTime() <= 0)
            return false;
        return true;
    }
    public static bool ShowTaskEntrance(this StoragePhotoAlbum storage)
    {
        if (!PhotoAlbumModel.Instance.IsOpened())
            return false;
        if (PhotoAlbumModel.Instance.Storage.GetPreheatLeftTime() > 0)
            return false;
        if (storage.IsTimeOut())
            return false;
        if (storage.GetLeftPreEndTime() <= 0)
            return false;
        if (PhotoAlbumModel.Instance.IsFinish)
            return false;
        return true;
    }

    public static string GetAuxItemAssetPath(this StoragePhotoAlbum storage)
    {
        return "Prefabs/Activity/PhotoAlbum/Aux_PhotoAlbum";
    }

    public static string GetTaskItemAssetPath(this StoragePhotoAlbum storage)
    {
        return "Prefabs/Activity/PhotoAlbum/TaskList_PhotoAlbum";
    }
    
}