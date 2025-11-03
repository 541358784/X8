using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynamic;
using Activity.PhotoAlbum.View;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.PhotoAlbum;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using SomeWhere;
using SRF;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class PhotoAlbumModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static PhotoAlbumModel _instance;
    public static PhotoAlbumModel Instance => _instance ?? (_instance = new PhotoAlbumModel());

    public override string Guid => "OPS_EVENT_TYPE_PHOTO_ALBUM";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    public PhotoAlbumGlobalConfig GlobalConfig => PhotoAlbumConfigManager.Instance.GetConfig<PhotoAlbumGlobalConfig>()[0];
    public List<PhotoAlbumTaskRewardConfig> TaskRewardConfig => PhotoAlbumConfigManager.Instance.GetConfig<PhotoAlbumTaskRewardConfig>();
    public long PreheatTime=> IsSkipActivityPreheating()?0:(long)((ulong)GlobalConfig.PreheatTime * XUtility.Hour);
    public long PreEndTime=> (long)((ulong)GlobalConfig.PreEndTime * XUtility.Hour);
    public StoragePhotoAlbum Storage => StorageManager.Instance.GetStorage<StorageHome>().PhotoAlbum;

    public bool IsFinish => IsInitFromServer() && Storage.FinishStoreItemList.Count >= StoreItemConfig.Count;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActivityId != ActivityId)
        {
            // Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.FinishStoreItemList.Clear();
            Storage.Score = 0;
            Storage.IsStart = false;
            Storage.IsEnd = false;
            Storage.UnLockStoreLevel.Clear();
            Storage.UnLockStoreLevel.Add(1);
            ResetStory("PhotoAlbum_Guide");
        }
        Storage.StartTime = (long)StartTime;
        Storage.PreheatCompleteTime = (long)StartTime + PreheatTime;
        Storage.EndTime = (long)EndTime;
        Storage.PreEndTime = (long)EndTime - PreEndTime;
    }
    public void ResetStory(string storyKey)
    {
        var story = GlobalConfigManager.Instance.GetTableStory(23, storyKey);
        if (story == null)
            return;
        StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog.Remove(story.id);
    }
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = PhotoAlbumConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public List<PhotoAlbumStoreLevelConfig> StoreLevelConfig => PhotoAlbumConfigManager.Instance.GetConfig<PhotoAlbumStoreLevelConfig>();
    public Dictionary<int, PhotoAlbumStoreItemConfig> StoreItemConfig = new Dictionary<int, PhotoAlbumStoreItemConfig>();
    public Dictionary<int, PhotoAlbumPhotoConfig> PhotoConfig = new Dictionary<int, PhotoAlbumPhotoConfig>();
    public Dictionary<int, PhotoAlbumPhotoPieceConfig> PhotoPieceConfig = new Dictionary<int, PhotoAlbumPhotoPieceConfig>();
    public Dictionary<int, StorageSinglePhotoAlbum> PhotoAlbumStorage => Storage.PhotoAlbumCollectState;
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        PhotoAlbumConfigManager.Instance.InitConfig(configJson);
        InitTable(StoreItemConfig);
        // InitTable(PhotoConfig);
        {
            var photoConfigs = PhotoAlbumConfigManager.Instance.GetConfig<PhotoAlbumPhotoConfig>();
            var photoList = GlobalConfig.PhotoList;
            foreach (var photo in photoList)
            {
                var photoConfig = photoConfigs.Find(a => a.Id == photo);
                PhotoConfig.Add(photoConfig.Id,photoConfig);
            }
        }
        InitTable(PhotoPieceConfig);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.PhotoAlbum);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut();
    }
    public int GetScore()
    {
        return Storage.Score;
    }
    public bool ReduceScore(int reduceCount,string reason)
    {
        if (Storage.Score < reduceCount)
            return false;
        Storage.Score -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventPhotoAlbumScoreChange(-reduceCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPhotoAlbumRadishChange,
            (-reduceCount).ToString(),Storage.Score.ToString(),reason);
        return true;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.PhotoAlbum);
    }

    public Transform GetCommonFlyTarget()
    {
        var storage = Storage;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_PhotoAlbum>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_PhotoAlbum>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    // public static bool CanShowStart()
    // {
    //     if (Instance.IsOpened() && 
    //         !Instance.Storage.IsStart && 
    //         (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
    //          SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome) &&
    //         !GuideSubSystem.Instance.IsShowingGuide())
    //     {
    //         Instance.Storage.IsStart = true;
    //         UIPopupPhotoAlbumStartController.Open(Instance.Storage);
    //         return true;
    //     }
    //     return false;
    // }
    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (ulong)Storage.PreheatCompleteTime && Storage.GetLeftPreEndTime() > 0;
    }
    

    public int GetTaskValue(StorageTaskItem taskItem, bool isMul)
    {
        if (!IsInitFromServer())
            return 0;
        int tempPrice = 0;
        for (var i = 0; i < taskItem.RewardTypes.Count; i++)
        {
            if (taskItem.RewardTypes[i] == (int)UserData.ResourceId.Coin || taskItem.RewardTypes[i] == (int)UserData.ResourceId.RareDecoCoin)
            {
                if(taskItem.RewardNums.Count > i)
                    tempPrice = taskItem.RewardNums[i];
                
                break;
            }
        }

        if (tempPrice == 0)
        {
            foreach (var itemId in taskItem.ItemIds)
            {
                tempPrice += OrderConfigManager.Instance.GetItemPrice(itemId);
            }
        }

        var value = 0;
        var configs = TaskRewardConfig;
        if (configs != null && configs.Count > 0)
        {
            foreach (var config in configs)
            {
                if (tempPrice <= config.Max_value)
                {
                    value = config.Output;
                    break;
                }
            }
        }
        return value;
    }
    public void AddScore(int addCount,string reason)
    {
        var oldValue = Storage.Score;
        Storage.Score += addCount;
        var newValue = Storage.Score;
        EventDispatcher.Instance.SendEventImmediately(new EventPhotoAlbumScoreChange(addCount));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventPhotoAlbumRadishChange,
            addCount.ToString(), newValue.ToString(), reason);
    }

    public PhotoAlbumStoreItemConfig GetNextFish()
    {
        if (Storage.FinishStoreItemList.Count >= StoreItemConfig.Count)
            return null;
        return StoreItemConfig[Storage.GetCurStoreLevel().StoreItemList[0]];
    }

    public int GetRandomPhotoPiece(PhotoAlbumPhotoConfig photo)
    {
        var randomList = photo.Parts.DeepCopy();
        if (PhotoAlbumStorage.ContainsKey(photo.Id))
        {
            foreach (var piece in PhotoAlbumStorage[photo.Id].CollectState)
            {
                randomList.Remove(piece);
            }
        }
        if (randomList.Count > 0)
            return randomList.RandomPickOne();
        else
            return photo.Parts[0];
    }

    public bool CollectPhotoPiece(int pieceId)
    {
        if (!PhotoPieceConfig.ContainsKey(pieceId))
            return false;
        var photoPieceConfig = PhotoPieceConfig[pieceId];
        var photoConfig = PhotoConfig[photoPieceConfig.PhotoId];
        if (!PhotoAlbumStorage.ContainsKey(photoPieceConfig.PhotoId))
            PhotoAlbumStorage.Add(photoPieceConfig.PhotoId,new StorageSinglePhotoAlbum());
        PhotoAlbumStorage[photoPieceConfig.PhotoId].CollectState.Add(pieceId);
        foreach (var part in photoConfig.Parts)
        {
            if (!PhotoAlbumStorage[photoPieceConfig.PhotoId].CollectState.Contains(part))
                return false;
        }
        return true;
    }
    
    public static bool CanShowFinishPopup()
    {
        if (Instance.IsPrivateOpened() && 
            Instance.Storage.GetLeftPreEndTime() <= 0 && 
            !Instance.Storage.IsEnd && 
            Instance.Storage.IsStart)
        {
            Instance.Storage.IsEnd = true;
            if (!Instance.IsFinish)
            {
                UIPhotoAlbumShopController.Open(Instance.Storage);   
                return true;
            }
        }
        return false;
    }
    public const string preheatCoolTimeKey = "PhotoAlbumPreheat";
    public static bool CanShowPreheatPopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, preheatCoolTimeKey))
            return false;
        if (CanShowPreheatPopup())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, preheatCoolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && Instance.Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupPhotoAlbumPreviewController.Open(Instance.Storage);
            return true;
        }
        return false;
    }
    public static bool CanShowStartPopup()
    {
        if (Instance.IsStart() && !Instance.Storage.IsStart)
        {
            UIPhotoAlbumShopController.Open(Instance.Storage);
            return true;
        }
        return false;
    }

    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;

        return Storage.ShowAuxItem();
    }
    public bool ShowTaskEntrance()
    {
        if (Storage == null)
            return false;

        return Storage.ShowTaskEntrance();
    }
    
}