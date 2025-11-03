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

public class ClimbTreeLeaderBoardModel
{
    public static bool InGuideChain = false;
    public static string LeadBoardAPITypeName = "ClimbTreeLeaderBoard";
    private static ClimbTreeLeaderBoardModel _instance;
    public static ClimbTreeLeaderBoardModel Instance => _instance ?? (_instance = new ClimbTreeLeaderBoardModel());
    
    

    public int MaxPlayerCount => ClimbTreeModel.Instance.GlobalConfig.MaxPlayerCount;
    public int LeastEnterBoardScore
    {
        get
        {
            return ClimbTreeModel.Instance.GlobalConfig.LeastEnterBoardScore;
        }
    }

    public StorageClimbTreeLeaderBoard CurStorageClimbTreeLeaderBoardWeek
    {
        get
        {
            return ClimbTreeModel.Instance.CurStorageClimbTreeWeek?.LeaderBoardStorage;
        }
    }

    public void CreateStorage(StorageClimbTreeLeaderBoard newWeek)
    {
        newWeek.JsonRecoverCoinRewardConfig = JsonConvert.SerializeObject(ClimbTreeModel.Instance.LeaderBoardRewardConfig);
        newWeek.EndTime = ClimbTreeModel.Instance.CurStorageClimbTreeWeek.EndTime;
        newWeek.StartTime = ClimbTreeModel.Instance.CurStorageClimbTreeWeek.StartTime;
        newWeek.StarCount = ClimbTreeModel.Instance.CurStorageClimbTreeWeek.TotalScore;
        newWeek.IsFinish = false;
        newWeek.IsStart = true;
        newWeek.MaxPlayerCount = MaxPlayerCount;
        newWeek.ActivityId = ClimbTreeModel.Instance.CurStorageClimbTreeWeek.ActivityId;
        newWeek.StarUpdateTime = APIManager.Instance.GetServerTime();
        newWeek.IsUpdateFinalData = false;
        newWeek.ActivityResList.Clear();
        for (var i = 0; i < ClimbTreeModel.Instance.CurStorageClimbTreeWeek.ActivityResList.Count; i++)
        {
            newWeek.ActivityResList.Add(ClimbTreeModel.Instance.CurStorageClimbTreeWeek.ActivityResList[i]);
        }
        ClimbTreeLeaderBoardUtils.StorageWeekInitStateDictionary.Add(newWeek.ActivityId,false);
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventClimbTreeLeaderBoardEnter);
        // EnterLeaderBoard(newWeek);
    }
    private bool InEnterLeaderBoardLoop;
    public async void EnterLeaderBoard(StorageClimbTreeLeaderBoard newWeek,Action callback = null)
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
        ClimbTreeLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        if (CurStorageClimbTreeLeaderBoardWeek != null)
            ClimbTreeLeaderBoardUtils.StorageWeekInitStateDictionary.Add(CurStorageClimbTreeLeaderBoardWeek.ActivityId,false);
        XUtility.WaitFrames(1, () =>
        {
            DebugUtil.LogError("2");
            if (IsStart())
            {
                CurStorageClimbTreeLeaderBoardWeek.ForceUpdateLeaderBoardFromServer().WrapErrors();
            }
        });
    }

    public bool IsStart()
    {
        return CurStorageClimbTreeLeaderBoardWeek != null && !CurStorageClimbTreeLeaderBoardWeek.IsTimeOut();
    }

    public bool IsStartAndStorageInitFromServer()
    {
        return IsStart() && CurStorageClimbTreeLeaderBoardWeek.IsStorageWeekInitFromServer();
    }

    public void SetStar(int setCount)
    {
        if (!IsStart())
            return;
        CurStorageClimbTreeLeaderBoardWeek.CollectStar(setCount);
    }

    public int GetStar()
    {
        if (!IsStart())
            return 0;
        return CurStorageClimbTreeLeaderBoardWeek.StarCount;
    }

    public static StorageClimbTreeLeaderBoard GetFirstWeekCanGetReward()
    {
        var ClimbTreeStorage = ClimbTreeModel.GetFirstWeekCanGetReward();
        if (ClimbTreeStorage == null)
            return null;
        return ClimbTreeStorage.LeaderBoardStorage;
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

    public static UIClimbTreeLeaderBoardMainController OpenMainPopup(StorageClimbTreeLeaderBoard storageWeek)
    {
        if (!storageWeek.IsUpdateFinalData)
        {
            storageWeek.TryUpdateLeaderBoardFromServer().WrapErrors();
        }
        return UIClimbTreeLeaderBoardMainController.Open(storageWeek);;
    }
    private const string coolTimeKey = "ClimbTreeLeaderBoard";
    public static bool CanShowMainUIPerDay()
    {
        if (!Instance.IsStart())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            OpenMainPopup(Instance.CurStorageClimbTreeLeaderBoardWeek);
            return true;
        }
        return false;
    }
    //目前不需要排行榜开始弹窗
    // public static bool CanShowActivityStartUI()
    // {
    //     if (Instance.IsPrivateOpened() && !Instance.CurStorageClimbTreeLeaderBoardWeek.IsStart)
    //     {
    //         if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIClimbTreeLeaderBoardStart) == null)
    //         {
    //             var startWindow =
    //                 UIManager.Instance.OpenUI(UINameConst.UIClimbTreeLeaderBoardStart) as
    //                     UIClimbTreeLeaderBoardStartController;
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