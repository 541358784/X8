using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.CoinLeaderBoard;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinLeaderBoardModel : ActivityEntityBase
{
    public static bool InGuideChain = false;
    public static string LeadBoardAPITypeName = "CoinLeaderBoard";
    private static CoinLeaderBoardModel _instance;
    public static CoinLeaderBoardModel Instance => _instance ?? (_instance = new CoinLeaderBoardModel());

    public override string Guid => "OPS_EVENT_TYPE_COIN_LEADER_BOARD";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Debug.LogError("金币排行榜InitAuto");
        Instance.Init();
        EventDispatcher.Instance.AddEventListener(EventEnum.AddCoin,Instance.OnCollectCoin);
    }
    public void OnCollectCoin(BaseEvent e)
    {
        var collectCount = (int) e.datas[0];
        if (collectCount <= 0)
            return;
        AddStar(collectCount);
    }

    public static StorageCoinLeaderBoard StorageCoinLeaderBoard => StorageManager.Instance.GetStorage<StorageHome>().CoinLeaderBoard;

    public static List<int> LastFinishNodeList => StorageCoinLeaderBoard.LastFinishNodeList;

    public int MaxPlayerCount => CoinLeaderBoardConfigManager.Instance.GetConfig<CoinLeaderBoardPlayerCountConfig>()[0].MaxPlayerCount;

    public int LeastEnterBoardScore
    {
        get
        {
            if (IsInitFromServer())
                return CoinLeaderBoardConfigManager.Instance.GetConfig<CoinLeaderBoardPlayerCountConfig>()[0].LeastEnterBoardScore;
            return 100;
        }
    }
    

    public bool IsCurWeekExistByStorage() //如果存储中存在未结束的week数据，则视为在排行赛活动中
    {
        foreach (var week in StorageCoinLeaderBoard.StorageByWeek)
        {
            if (!week.Value.IsTimeOut())
            {
                return true;
            }
        }
        return false;
    }

    public StorageCoinLeaderBoardWeek CurStorageCoinLeaderBoardWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageCoinLeaderBoard.StorageByWeek.TryGetValue(ActivityId, out StorageCoinLeaderBoardWeek curWeek);
            return curWeek;
        }
    }

    public bool CreateStorage()
    {
        DebugUtil.LogError("3");
        if (CurStorageCoinLeaderBoardWeek == null && !IsFinishedAllTarget() && IsInitFromServer()
            && CoinLeaderBoardModel.GetFirstWeekCanGetReward() == null
            && IsOpened())
        {
            DebugUtil.LogError("4");
            var newWeek = new StorageCoinLeaderBoardWeek()
            {
                JsonRecoverCoinRewardConfig =
                    JsonConvert.SerializeObject(CoinLeaderBoardConfigManager.Instance
                        .GetConfig<CoinLeaderBoardRewardConfig>()),
                EndTime = (long) EndTime,
                StartTime = (long) StartTime,
                StarCount = 0,
                IsFinish = false,
                IsStart = false,
                MaxPlayerCount = MaxPlayerCount,
                ActivityId = ActivityId,
                StarUpdateTime = APIManager.Instance.GetServerTime(),
                IsUpdateFinalData = false
            };
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            newWeek.ActivityResMd5List.Clear();
            newWeek.ActivityResList.Clear();
            foreach (var resMd5 in resMd5List)
            {
                newWeek.ActivityResMd5List.Add(resMd5);
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                newWeek.ActivityResList.Add(resPath);
            }
            StorageCoinLeaderBoard.StorageByWeek.Add(ActivityId, newWeek);
            CoinLeaderBoardUtils.StorageWeekInitStateDictionary.Add(newWeek.ActivityId,false);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinLeaderBoardEnter);
            // EnterLeaderBoard(newWeek);
            return true;
        }

        return false;
    }
    private bool InEnterLeaderBoardLoop;
    public async void EnterLeaderBoard(StorageCoinLeaderBoardWeek newWeek,Action callback = null)
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

    public bool IsFinishedAllTarget()
    {
        return DecoManager.Instance.IsFinishLastNode() || RecoverCoinModel.Instance.IsCurWeekExistByStorage();
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        EventDispatcher.Instance.AddEventListener(EventEnum.OwnDecoNode, (e) =>
        {
            var decoNodeData = (DecoNodeData) e.datas[0];
            if (decoNodeData._config.costId == (int) UserData.ResourceId.Coin)
                _lastFinishNodeId = decoNodeData._config.id;
        });
        CoinLeaderBoardConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");

        TryReleaseUselessStorage();
        CoinLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        foreach (var pair in StorageCoinLeaderBoard.StorageByWeek)
        {
            CoinLeaderBoardUtils.StorageWeekInitStateDictionary.Add(pair.Value.ActivityId,false);
        }

        XUtility.WaitFrames(1, () =>
        {
            DebugUtil.LogError("2");
            if (CoinLeaderBoardModel.GetFirstWeekCanGetReward() == null)
            {
                CreateStorage();
            }
            _lastActivityOpenState = IsPrivateOpened();
            if (IsPrivateOpened())
            {
                CurStorageCoinLeaderBoardWeek.ForceUpdateLeaderBoardFromServer().WrapErrors();
            }
        });
    }

    public void TryReleaseUselessStorage()
    {
        var releaseWeekList = new List<StorageCoinLeaderBoardWeek>();
        foreach (var pair in StorageCoinLeaderBoard.StorageByWeek)
        {
            if (pair.Value.IsFinish || 
                (pair.Value.IsTimeOut() && (!pair.Value.IsInitFromServer())))
            {
                releaseWeekList.Add(pair.Value);
            }
        }

        foreach (var releaseWeek in releaseWeekList)
        {
            releaseWeek.TryRelease();
        }
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CoinLeaderBoard);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && CurStorageCoinLeaderBoardWeek != null && !CurStorageCoinLeaderBoardWeek.IsTimeOut();
    }

    public bool IsStart()
    {
        return IsPrivateOpened() && CurStorageCoinLeaderBoardWeek.IsStart;
    }

    public bool IsStartAndStorageInitFromServer()
    {
        return IsStart() && CurStorageCoinLeaderBoardWeek.IsStorageWeekInitFromServer();
    }

    public void AddStar(int addCount)
    {
        if (!IsStart())
            return;
        CurStorageCoinLeaderBoardWeek.CollectStar(addCount);
        CurStorageCoinLeaderBoardWeek.SortController().UpdateMe();
    }

    public int GetStar()
    {
        if (!IsStart())
            return 0;
        return CurStorageCoinLeaderBoardWeek.StarCount;
    }

    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动

    public CoinLeaderBoardModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        // CurStorageRankLeaderBoard?.SortController().Update();
        var currentActivityOpenState = IsPrivateOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (!currentActivityOpenState)
            CanShowUnCollectRewardsUI();
        _lastActivityOpenState = currentActivityOpenState;
    }

    public static StorageCoinLeaderBoardWeek GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageCoinLeaderBoard.StorageByWeek)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.CanStorageCoinLeaderBoardWeekGetReward())
            {
                return storageWeek;
            }
        }
        return null;
    }

    public static bool CanCreateNewStorage()
    {
        return Instance.CreateStorage();
    }
    public static bool CanShowUnCollectRewardsUI()
    {
        var weekCanGetReward = GetFirstWeekCanGetReward();
        if (weekCanGetReward == null)
            return false;
        if (!weekCanGetReward.IsResExist())
            return false;

        var buyPopup = UIManager.Instance.GetOpenedUIByPath(UINameConst.UICoinLeaderBoardBuy);
        if (buyPopup != null)
        {
            buyPopup.CloseWindowWithinUIMgr();
        }

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

    public static UICoinLeaderBoardMainController OpenMainPopup(StorageCoinLeaderBoardWeek storageWeek)
    {
        if (!storageWeek.IsUpdateFinalData)
        {
            storageWeek.TryUpdateLeaderBoardFromServer().WrapErrors();
        }

        var mainWindow =
            UIManager.Instance.OpenUI(UINameConst.UICoinLeaderBoardMain) as UICoinLeaderBoardMainController;
        if (!mainWindow)
            return null;
        storageWeek.SortController().UpdateAll();
        mainWindow.BindStorageWeek(storageWeek);
        return mainWindow;
    }
    private const string coolTimeKey = "CoinLeaderBoard";
    public static bool CanShowMainUIPerDay()
    {
        if (!Instance.IsStart())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            OpenMainPopup(Instance.CurStorageCoinLeaderBoardWeek);
            return true;
        }
        return false;
    }
    public static bool CanShowActivityStartUI()
    {
        if (Instance.IsPrivateOpened() && !Instance.CurStorageCoinLeaderBoardWeek.IsStart)
        {
            if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UICoinLeaderBoardStart) == null)
            {
                var startWindow =
                    UIManager.Instance.OpenUI(UINameConst.UICoinLeaderBoardStart) as
                        UICoinLeaderBoardStartController;
            }
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            return true;
        }

        return false;
    }

    private static int _lastFinishNodeId = 0;

    // public static bool CanShowAllNodeFinishUI()
    // {
    //     if (Instance.IsCurWeekExistByStorage() && Instance.IsFinishedAllTarget() && !LastFinishNodeList.Contains(_lastFinishNodeId))
    //     {
    //         LastFinishNodeList.Clear();
    //         LastFinishNodeList.Add(_lastFinishNodeId);
    //         if (!LastFinishNodeList.Contains(0))
    //         {
    //             LastFinishNodeList.Add(0);
    //         }
    //
    //         var finishAllNodeWindow =
    //             UIManager.Instance.OpenUI(UINameConst.UIPopupCoinLeaderBoardFinish) as
    //                 UIPopupCoinLeaderBoardFinishController;
    //         return true;
    //     }
    //
    //     return false;
    // }

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.CoinLeaderBoard);
    }
    
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