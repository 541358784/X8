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

public class Easter2024LeaderBoardModel
{
    public static bool InGuideChain = false;
    public static string LeadBoardAPITypeName = "Easter2024LeaderBoard";
    private static Easter2024LeaderBoardModel _instance;
    public static Easter2024LeaderBoardModel Instance => _instance ?? (_instance = new Easter2024LeaderBoardModel());
    
    

    public int MaxPlayerCount => Easter2024Model.Instance.GlobalConfig.MaxPlayerCount;
    public int LeastEnterBoardScore
    {
        get
        {
            return Easter2024Model.Instance.GlobalConfig.LeastEnterBoardScore;
        }
    }

    public StorageEaster2024LeaderBoard CurStorageEaster2024LeaderBoardWeek
    {
        get
        {
            return Easter2024Model.Instance.CurStorageEaster2024Week?.LeaderBoardStorage;
        }
    }

    public void CreateStorage(StorageEaster2024LeaderBoard newWeek)
    {
        newWeek.JsonRecoverCoinRewardConfig = JsonConvert.SerializeObject(Easter2024Model.Instance.LeaderBoardRewardConfig);
        newWeek.EndTime = Easter2024Model.Instance.CurStorageEaster2024Week.EndTime;
        newWeek.StartTime = Easter2024Model.Instance.CurStorageEaster2024Week.StartTime;
        newWeek.StarCount = Easter2024Model.Instance.CurStorageEaster2024Week.Score;
        newWeek.IsFinish = false;
        newWeek.IsStart = true;
        newWeek.MaxPlayerCount = MaxPlayerCount;
        newWeek.ActivityId = Easter2024Model.Instance.CurStorageEaster2024Week.ActivityId;
        newWeek.StarUpdateTime = APIManager.Instance.GetServerTime();
        newWeek.IsUpdateFinalData = false;
        newWeek.ActivityResList.Clear();
        for (var i = 0; i < Easter2024Model.Instance.CurStorageEaster2024Week.ActivityResList.Count; i++)
        {
            newWeek.ActivityResList.Add(Easter2024Model.Instance.CurStorageEaster2024Week.ActivityResList[i]);
        }
        Easter2024LeaderBoardUtils.StorageWeekInitStateDictionary.Add(newWeek.ActivityId,false);
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEaster2024LeaderBoardEnter);
        // EnterLeaderBoard(newWeek);
    }
    private bool InEnterLeaderBoardLoop;
    public async void EnterLeaderBoard(StorageEaster2024LeaderBoard newWeek,Action callback = null)
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
        Easter2024LeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        if (CurStorageEaster2024LeaderBoardWeek != null)
            Easter2024LeaderBoardUtils.StorageWeekInitStateDictionary.Add(CurStorageEaster2024LeaderBoardWeek.ActivityId,false);
        XUtility.WaitFrames(1, () =>
        {
            DebugUtil.LogError("2");
            if (IsStart())
            {
                CurStorageEaster2024LeaderBoardWeek.ForceUpdateLeaderBoardFromServer().WrapErrors();
            }
        });
    }

    public bool IsStart()
    {
        return CurStorageEaster2024LeaderBoardWeek != null && !CurStorageEaster2024LeaderBoardWeek.IsTimeOut();
    }

    public bool IsStartAndStorageInitFromServer()
    {
        return IsStart() && CurStorageEaster2024LeaderBoardWeek.IsStorageWeekInitFromServer();
    }

    public void SetStar(int setCount)
    {
        if (!IsStart())
            return;
        CurStorageEaster2024LeaderBoardWeek.CollectStar(setCount);
        CurStorageEaster2024LeaderBoardWeek.SortController().UpdateMe();
    }

    public int GetStar()
    {
        if (!IsStart())
            return 0;
        return CurStorageEaster2024LeaderBoardWeek.StarCount;
    }

    public static StorageEaster2024LeaderBoard GetFirstWeekCanGetReward()
    {
        var easter2024Storage = Easter2024Model.GetFirstWeekCanGetReward();
        if (easter2024Storage == null)
            return null;
        return easter2024Storage.LeaderBoardStorage;
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

    public static UIEaster2024LeaderBoardMainController OpenMainPopup(StorageEaster2024LeaderBoard storageWeek)
    {
        if (!storageWeek.IsUpdateFinalData)
        {
            storageWeek.TryUpdateLeaderBoardFromServer().WrapErrors();
        }
        storageWeek.SortController().UpdateAll();
        return UIEaster2024LeaderBoardMainController.Open(storageWeek);;
    }
    private const string coolTimeKey = "Easter2024LeaderBoard";
    public static bool CanShowMainUIPerDay()
    {
        if (!Instance.IsStart())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            OpenMainPopup(Instance.CurStorageEaster2024LeaderBoardWeek);
            return true;
        }
        return false;
    }
    //目前不需要排行榜开始弹窗
    // public static bool CanShowActivityStartUI()
    // {
    //     if (Instance.IsPrivateOpened() && !Instance.CurStorageEaster2024LeaderBoardWeek.IsStart)
    //     {
    //         if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIEaster2024LeaderBoardStart) == null)
    //         {
    //             var startWindow =
    //                 UIManager.Instance.OpenUI(UINameConst.UIEaster2024LeaderBoardStart) as
    //                     UIEaster2024LeaderBoardStartController;
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