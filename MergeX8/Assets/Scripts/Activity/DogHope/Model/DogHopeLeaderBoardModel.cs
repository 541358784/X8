using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogHopeLeaderBoardModel
{
    public static bool InGuideChain = false;
    public static string LeadBoardAPITypeName = "DogHopeLeaderBoard";
    private static DogHopeLeaderBoardModel _instance;
    public static DogHopeLeaderBoardModel Instance => _instance ?? (_instance = new DogHopeLeaderBoardModel());
    
    

    public int MaxPlayerCount => DogHopeModel.Instance.GlobalConfig.MaxPlayerCount;
    public int LeastEnterBoardScore
    {
        get
        {
            return DogHopeModel.Instance.GlobalConfig.LeastEnterBoardScore;
        }
    }

    public StorageDogHopeLeaderBoard CurStorageDogHopeLeaderBoardWeek
    {
        get
        {
            return DogHopeModel.Instance.CurStorageDogHopeWeek?.LeaderBoardStorage;
        }
    }

    public void CreateStorage(StorageDogHopeLeaderBoard newWeek)
    {
        newWeek.JsonRecoverCoinRewardConfig = JsonConvert.SerializeObject(DogHopeModel.Instance.LeaderBoardRewardConfig);
        newWeek.EndTime = DogHopeModel.Instance.CurStorageDogHopeWeek.EndTime;
        newWeek.StartTime = DogHopeModel.Instance.CurStorageDogHopeWeek.StartTime;
        newWeek.StarCount = DogHopeModel.Instance.CurStorageDogHopeWeek.TotalScore;
        newWeek.IsFinish = false;
        newWeek.IsStart = true;
        newWeek.MaxPlayerCount = MaxPlayerCount;
        newWeek.ActivityId = DogHopeModel.Instance.CurStorageDogHopeWeek.ActivityId;
        newWeek.StarUpdateTime = APIManager.Instance.GetServerTime();
        newWeek.IsUpdateFinalData = false;
        newWeek.ActivityResList.Clear();
        for (var i = 0; i < DogHopeModel.Instance.CurStorageDogHopeWeek.ActivityResList.Count; i++)
        {
            newWeek.ActivityResList.Add(DogHopeModel.Instance.CurStorageDogHopeWeek.ActivityResList[i]);
        }
        DogHopeLeaderBoardUtils.StorageWeekInitStateDictionary.Add(newWeek.ActivityId,false);
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDogHopeLeaderBoardEnter);
        // EnterLeaderBoard(newWeek);
    }
    private bool InEnterLeaderBoardLoop;
    public async void EnterLeaderBoard(StorageDogHopeLeaderBoard newWeek,Action callback = null)
    {
        if (InEnterLeaderBoardLoop)
            return;
        InEnterLeaderBoardLoop = true;
        Debug.LogError("新周期创建并与后端通信");
        Action OnSuccess = () =>
        {
            WaitingManager.Instance.CloseWindow();
            if (callback != null)
                callback();
        };
        WaitingManager.Instance.OpenWindow(5f);
        var maxRepeatCount = 10;
        for (var i = 0; i < maxRepeatCount; i++)
        {
            var success = await newWeek.ForceUpdateLeaderBoardFromServer();
            if (success)
            {
                Debug.LogError("新周期创建成功");
                OnSuccess();
                break;
            }
            else
            {
                Debug.LogError("新周期创建失败");
                if (i == maxRepeatCount - 1)
                {
                    WaitingManager.Instance.CloseWindow();
                }
                else
                {
                    Debug.LogError("再次创建新周期");
                }
            }
        }
        Debug.LogError("新周期创建循环退出");
        InEnterLeaderBoardLoop = false;
    }
    

    public void InitFromServerData()
    {
        DogHopeLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        if (CurStorageDogHopeLeaderBoardWeek != null)
            DogHopeLeaderBoardUtils.StorageWeekInitStateDictionary.Add(CurStorageDogHopeLeaderBoardWeek.ActivityId,false);
        XUtility.WaitFrames(1, () =>
        {
            DebugUtil.LogError("2");
            if (IsStart())
            {
                CurStorageDogHopeLeaderBoardWeek.ForceUpdateLeaderBoardFromServer().WrapErrors();
            }
        });
    }

    public bool IsStart()
    {
        return CurStorageDogHopeLeaderBoardWeek != null && !CurStorageDogHopeLeaderBoardWeek.IsTimeOut();
    }

    public bool IsStartAndStorageInitFromServer()
    {
        return IsStart() && CurStorageDogHopeLeaderBoardWeek.IsStorageWeekInitFromServer();
    }

    public void SetStar(int setCount)
    {
        if (!IsStart())
            return;
        CurStorageDogHopeLeaderBoardWeek.CollectStar(setCount);
    }

    public int GetStar()
    {
        if (!IsStart())
            return 0;
        return CurStorageDogHopeLeaderBoardWeek.StarCount;
    }

    public static StorageDogHopeLeaderBoard GetFirstWeekCanGetReward()
    {
        var DogHopeStorage = DogHopeModel.GetFirstWeekCanGetReward();
        if (DogHopeStorage == null)
            return null;
        return DogHopeStorage.LeaderBoardStorage;
    }
    
    public static bool CanShowUnCollectRewardsUI()
    {
        var weekCanGetReward = GetFirstWeekCanGetReward();
        if (weekCanGetReward == null)
            return false;
        if (!weekCanGetReward.IsResExist())
            return false;

        WaitingManager.Instance.OpenWindow(5f);
        weekCanGetReward.ForceUpdateLeaderBoardFromServer().AddBoolCallBack((success) =>
        {
            WaitingManager.Instance.CloseWindow();
            if (!success)
                return;
            weekCanGetReward.IsUpdateFinalData = true;
            OpenMainPopup(weekCanGetReward);
        }).WrapErrors();

        return true;
    }

    public static UIDogHopeLeaderBoardMainController OpenMainPopup(StorageDogHopeLeaderBoard storageWeek)
    {
        if (!storageWeek.IsUpdateFinalData)
        {
            storageWeek.TryUpdateLeaderBoardFromServer().WrapErrors();
        }
        return UIDogHopeLeaderBoardMainController.Open(storageWeek);;
    }
    private const string coolTimeKey = "DogHopeLeaderBoard";
    public static bool CanShowMainUIPerDay()
    {
        if (!Instance.IsStart())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            OpenMainPopup(Instance.CurStorageDogHopeLeaderBoardWeek);
            return true;
        }
        return false;
    }
    //目前不需要排行榜开始弹窗
    // public static bool CanShowActivityStartUI()
    // {
    //     if (Instance.IsPrivateOpened() && !Instance.CurStorageDogHopeLeaderBoardWeek.IsStart)
    //     {
    //         if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIDogHopeLeaderBoardStart) == null)
    //         {
    //             var startWindow =
    //                 UIManager.Instance.OpenUI(UINameConst.UIDogHopeLeaderBoardStart) as
    //                     UIDogHopeLeaderBoardStartController;
    //         }
    //         CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
    //         return true;
    //     }
    //
    //     return false;
    // }

    public static void FlyStar(int rewardNum, Vector2 srcPos, Transform starTransform, float time, bool showEffect,
        Action action = null)
    {
        Transform target = starTransform;
        int count = Math.Min(rewardNum, 10);
        float delayTime = 0.3f;
        if (count >= 5)
            delayTime = 0.1f;
        for (int i = 0; i < count; i++)
        {
            int index = i;

            Vector3 position = target.position;

            FlyGameObjectManager.Instance.FlyObject(target.gameObject, srcPos, position, showEffect, time,
                delayTime * i, () =>
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                    ShakeManager.Instance.ShakeLight();
                    if (index == count - 1)
                    {
                        action?.Invoke();
                    }
                });
        }
    }
}