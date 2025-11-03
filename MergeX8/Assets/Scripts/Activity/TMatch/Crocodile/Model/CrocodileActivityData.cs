// #define FOR_DEV_TEST

using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.WinStreak;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using TMatch;
using UnityEngine;
using Random = UnityEngine.Random;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class CrocodileActivityData
{
    private CrocodileActivityModel _model;
    public WinStreakConfigManager configManager;

    public StorageActivityWinStreak Storage
    {
        get
        {
            var activity = StorageManager.Instance.GetStorage<StorageTMatch>();
            if (!activity.WinStreak.ContainsKey(_model.StorageKey))
                activity.WinStreak.Add(_model.StorageKey, new StorageActivityWinStreak());
            return activity.WinStreak[_model.StorageKey];
        }
    }

    public CrocodileActivityData(CrocodileActivityModel model)
    {
        _model = model;
    }

    public void Init(WinStreakConfigManager configManager)
    {
        this.configManager = configManager;
    }

    /// <summary>
    /// 挑战成功时，获取到的金币数额
    /// </summary>
    /// <returns></returns>
    public int GetMyPrize()
    {
        return (int)Math.Ceiling((double)GetBaseConfig().RewardCnt[0] / (Storage.RobotCount + 1));
    }

    /// <summary>
    /// 计算一次机器人的淘汰，刷新到 Storage.RobotCount 中去
    /// </summary>
    public void UpdateStorageRobot()
    {
        var challengeLevel = TMatchModel.Instance.GetMainLevel();
        // 如果关卡成功，则上面的值实际上是指向了下一关的 ID，因此需要再减 1 得到刚刚挑战的关卡 ID
        if (Storage.EnterLevelResult) challengeLevel--;

        var outRateList = configManager.OutRateList;
        var outRateConfig = outRateList[0];
        foreach (var rateConfig in outRateList)
        {
            if (challengeLevel > rateConfig.levelRange)
            {
                continue;
            }

            outRateConfig = rateConfig;
            break;
        }

        // 根据关卡难度，初步计算机器人淘汰率
        var difficult = Storage.LevelDifficult;
        difficult = difficult == 0 ? 1 : difficult;
        var difficultIndex = difficult - 1;
        var outRateMin = outRateConfig.outRateMin[difficultIndex];
        var outRateMax = outRateConfig.outRateMax[difficultIndex];
        var outRate = Random.Range(outRateMin, outRateMax);

        // 根据使用道具的数量或者关卡复活的情况，校正机器人淘汰率
        var useBoostCfg = outRateConfig.useBoost;
        var threshold = 0;
        var useBoostCfgCount = useBoostCfg.Length;
        if (useBoostCfg != null && useBoostCfgCount > 0)
        {
            threshold = useBoostCfgCount == 1 ? useBoostCfg[0] : useBoostCfg[difficultIndex];
        }

        if (Storage.UseBoost >= threshold || Storage.ReviveCount > 0)
        {
            var upRate = outRateConfig.upRate;
            outRate += upRate;
        }

        var outRobotCount = (int)Math.Floor((float)Storage.RobotCount * outRate / 100f);
        var robotCountBeforeOut = Storage.RobotCount;
        Storage.RobotCount -= outRobotCount;

        // 关卡难度、道具数量、是否复活、淘汰前机器人总数量、淘汰概率、淘汰数量、淘汰后机器人总数量
        DebugUtil.Log("[WinStreakLog] <======== Robot outRate info ========>");
        DebugUtil.Log($"[WinStreakLog] 关卡难度：{difficult}");
        DebugUtil.Log($"[WinStreakLog] 关卡使用道具数量：{Storage.UseBoost}");
        DebugUtil.Log($"[WinStreakLog] 关卡是否复活过：{Storage.ReviveCount > 0}");
        DebugUtil.Log($"[WinStreakLog] 淘汰前-机器人总数量：{robotCountBeforeOut}");
        DebugUtil.Log($"[WinStreakLog] 淘汰概率：{outRate}%");
        DebugUtil.Log($"[WinStreakLog] 淘汰数量： {robotCountBeforeOut} * {outRate}% = {outRobotCount}");
        DebugUtil.Log($"[WinStreakLog] 淘汰后-机器人总数量：{robotCountBeforeOut} - {outRobotCount} = {Storage.RobotCount}");

        // 倒数 3 个关卡进行强控
        var currLevel = Storage.Level;
        if (currLevel < (CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT - 2))
        {
            return;
        }

        var currStorageRobotCnt = Storage.RobotCount;
        var baseConfig = GetBaseConfig();
        var cfgIndex = currLevel - (CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT - 2); // 倒数的 3 个关，对应的配置索引分别为 0, 1, 2
        var isLessThanCfg = currStorageRobotCnt <= baseConfig.hardControllCnt[cfgIndex];
        if (isLessThanCfg)
        {
            return;
        }

        var floor = baseConfig.hardControllMin[cfgIndex];
        var ceil = baseConfig.hardControllMax[cfgIndex] + 1; // 由于 Random.Range 方法是左闭右开的，因此这里要 +1
        Storage.RobotCount = Random.Range(floor, ceil);

        DebugUtil.Log($"[WinStreakLog] 强控后-机器人总数量变为：{Storage.RobotCount}");
    }

    /// <summary>
    /// 主界面跳跃动画播放之前调用
    /// </summary>
    public void PreJump()
    {
        Storage.Level++;
        UpdateStorageRobot();

        if (!Storage.EnterLevelResult || Storage.Level == CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT)
        {
            Storage.InChallenge = false;
            Storage.EndChallengeTime = GetNowTime().ToString();

            if (!Storage.EnterLevelResult)
            {
                Storage.Level--;
            }
            else
            {
                Storage.ChallengeResult = true;
                GetRewards();
            }
        }

        TMatch.EventDispatcher.Instance.DispatchEvent(EventEnum.RefreshWinStreakProgress);
    }

    /// <summary>
    /// 获取最终奖励
    /// </summary>
    private void GetRewards()
    {
        var cfg = CrocodileActivityModel.Instance.GetBaseConfig();
        var myPrize = GetMyPrize();
        
        foreach (var id in cfg.RewardId)
        {
            ItemModel.Instance.Add(id, myPrize, new DragonPlus.GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventCooking.Types.ItemChangeReason.LavaquestGetTm,
            });
        }
    }

    /// <summary>
    /// 局内使用复活
    /// </summary>
    public void GameRevive()
    {
        if (!IsInChallenge()) return;
        Storage.ReviveCount++;
    }

    /// <summary>
    /// 局内使用道具
    /// </summary>
    public void UseBoost()
    {
        if (!IsInChallenge()) return;
        Storage.UseBoost++;
    }

    public bool TryToShowPopup()
    {
        if (IsActivityDisable())
        {
            return false;
        }

        // 挑战中，则判断是否需要展示升级的动画，或者失败的动画
        if (IsInChallenge())
        {
            if (Storage.EnterLevel == Storage.Level)
            {
                return false;
            }

            UICrocodileMainController.Open();
            return true;
        }

        if (!CanStartChallenge()) return false;
        var nowTime = GetNowTime();
        var lastPopTime = Storage.LastPopupTime.ToLong();
        bool isPop = false;
        if (lastPopTime != 0)
        {
            var dataTime = CommonUtils.ConvertFromUnixTimestamp((ulong)lastPopTime);
            var zeroTime = CommonUtils.ConvertDateTimeToTimeStamp(dataTime.Date);
            var nextDayZeroTime = zeroTime + 24 * 60 * 60 * 1000;
            if (nowTime < nextDayZeroTime)
                return false;
            isPop = true;
        }
        else
        {
            isPop = true;
        }

        if (isPop)
        {
            UIPopupCrocodileStartController.Open();
            Storage.LastPopupTime = nowTime.ToString();
        }

        return isPop;
    }

    public bool TryToPopupWinInGame()
    {
        if (!IsInChallenge()) return false;
        if (Storage.EnterLevel > Storage.Level && Storage.Level < CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT)
        {
            UICrocodileMainController.Open();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取当前时间 此活动获取时间都用这个接口，好保持统一，方便调整
    /// </summary>
    /// <returns></returns>
    public long GetNowTime()
    {
        return (long)APIManager.Instance.GetServerTime();
    }

    public void StartChallenge()
    {
        if (Storage.Turn == 0 || Storage.ChallengeResult)
        {
            Storage.Turn++;
        }

        Storage.ChallengeTimes++;
        Storage.InChallenge = true;
        var nowTime = GetNowTime();
        Storage.StartChallengeTime = nowTime.ToString();
        // 结束时间取当前轮次结束时间和挑战限时中靠前的一个
        Storage.EndChallengeTime = Math.Min(
            (long)(_model.EndTime),
            nowTime + (long)GetBaseConfig().timeLimit * 1000).ToString();
        Storage.Level = 0;
        Storage.EnterLevel = 0;
        Storage.EnterLevelResult = false;
        Storage.ChallengeResult = false;
        Storage.RobotCount = 99;
    }

    /// <summary>
    /// 获取玩家点击挑战的时候，正处于第几个轮次。活动启动当天视为第一轮
    /// </summary>
    /// <returns></returns>
    public int GetStartChallengeTurn()
    {
        if (Storage.StartChallengeTime.ToLong() == 0) return 0;
        var startChallengeTime = Storage.StartChallengeTime.ToLong();
        var activityPassedTime = startChallengeTime - (long)_model.StartTime;
        var startChallengeTurn = activityPassedTime / (GetTurnTime() * 1000) + 1;
        return (int)startChallengeTurn;
    }

    /// <summary>
    /// 是否可以开始挑战
    /// </summary>
    /// <returns></returns>
    public bool CanStartChallenge()
    {
        if (Storage.ChallengeTimes == 0) return true;

        // 玩家挑战胜利后，如果还处于当前轮次的活动时间，则提前进入等待，直到下一轮挑战开始；如果处于下一轮挑战时间，则可立即发起下一轮挑战
        // 上一次挑战失败，如果当前轮次剩余时间大于失败冷却时间，则进入失败冷却时间，冷却结束后可再次挑战，否则进入等待时间
        // 如果失败时已处于下一轮挑战时间内，则可立即发起下一轮挑战
        var lastTurn = GetStartChallengeTurn();
        // 进入到下一轮了，无论是胜利还是失败都可以立即发起挑战
        if (Storage.Turn > lastTurn) return true;

        // 上次挑战结果为成功
        if (Storage.ChallengeResult)
        {
            return true;
        }

        // 上次挑战结果为失败
        var turnEndTime = GetTurnEndTime();
        var nextStartTimeIfFailed = GetNextStartTimeIfFailed();

        // 未进入下一轮但是开始时间大于这一轮结束时间，需要等待，当前不可以挑战
        if (nextStartTimeIfFailed > turnEndTime)
        {
            return false;
        }

        // 当前时间已经大于开始时间（已排除开始时间在下一轮内），即可发起挑战
        return GetNowTime() > nextStartTimeIfFailed;
    }

    /// <summary>
    /// 是否是完成领奖的状态
    /// </summary>
    /// <returns></returns>
    public bool IsFinishCurrentTurnReward()
    {
        if (IsActivityDisable())
        {
            return false;
        }

        if (!Storage.ChallengeResult)
        {
            return false;
        }

        // 挑战结果是成功且在下一轮挑战开启之前都是奖励领取完成状态
        return GetNowTime() < GetTurnEndTime();
    }

    /// <summary>
    /// 是否正在挑战中
    /// </summary>
    /// <returns></returns>
    public bool IsInChallenge()
    {
        return !IsActivityDisable() && Storage.InChallenge;
    }

    /// <summary>
    /// 检测是否超时
    /// </summary>
    public void CheckChallengeIsOutTime()
    {
        if (GetNowTime() > Storage.EndChallengeTime.ToLong() && Storage.Level < CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT)
        {
            Storage.InChallenge = false;
            Storage.ChallengeResult = false;
        }
    }

    /// <summary>
    /// 一旦关卡失败，检测是否还有机会在当前这一轮重新开启挑战
    /// </summary>
    /// <returns></returns>
    public bool CanStartInCurrentTurn()
    {
        return GetNextStartTimeIfFailed() < GetTurnEndTime();
    }

    /// <summary>
    /// 获取能够开始挑战的剩余时间（一定会有下一次挑战的情况下）
    /// </summary>
    /// <returns></returns>
    public long GetCanChallengeLeftTime()
    {
        var currentTurnEndTime = GetTurnEndTime();
        var nowTime = GetNowTime();
        if (nowTime < currentTurnEndTime)
        {
            return GetNextStartTimeIfFailed() - nowTime;
        }

        return currentTurnEndTime - nowTime; // 返回负数，说明已经可以开始新一次挑战了
    }

    // 更新当前的轮次信息
    public void UpdateCurrentTurn()
    {
        var activityTime = GetNowTime() - (long)_model.StartTime;
        var curTurn = activityTime / (GetTurnTime() * 1000) + 1;

        Storage.Turn = (int)curTurn;
        Storage.EndChallengeTime = "0";
        Storage.InChallenge = false;
        Storage.ChallengeResult = false;
        Storage.Level = 0;

        TMatch.EventDispatcher.Instance.DispatchEvent(EventEnum.RefreshWinStreak);
    }

    /// <summary>
    /// 获取本次挑战的剩余时间
    /// </summary>
    /// <returns></returns>
    public long GetChallengeLeftTime()
    {
        var left = Storage.EndChallengeTime.ToLong() - GetNowTime();
        if (left < 0)
            left = 0;
        return left;
    }

    /// <summary>
    /// 获取当前轮次的剩余时间，如果剩余时间小于0，则轮次+1
    /// </summary>
    /// <returns></returns>
    public long GetCurrentTurnLeftTime()
    {
        var left = GetTurnEndTime() - GetNowTime();
        if (left >= 0)
        {
            return left;
        }

        UpdateCurrentTurn();
        return GetCurrentTurnLeftTime();
    }

    public long GetCurrentActivityLeftTime()
    {
        if (Storage.InChallenge)
        {
            return GetChallengeLeftTime();
        }
    
        return  (long)_model.GetActivityLeftTime();
    }

    /// <summary>
    /// 关卡胜利事件处理
    /// </summary>
    public void OnGameWin()
    {
        if (IsActivityDisable())
        {
            return;
        }

        if (Storage.InChallenge)
        {
            Storage.EnterLevelResult = true;
        }
                
       
    }

    /// <summary>
    /// 关卡失败事件处理
    /// </summary>
    public void OnGameFailed()
    {
        if (!Storage.InChallenge)
        {
            return;
        }

    }

    /// <summary>
    /// 游戏开始事件处理, 暂存连赢次数
    /// </summary>
    public void OnGameStart()
    {
        if (IsActivityDisable())
        {
            return;
        }

        CheckChallengeIsOutTime();

        if (!IsInChallenge())
        {
            return;
        }

        if (Storage.EnterLevel < CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT && Storage.EnterLevel == Storage.Level)
        {
            Storage.EnterLevel++;
        }

        Storage.EnterLevelResult = false;
        var difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchModel.Instance.GetMainLevel());
        Storage.LevelDifficult = (int)difficulty;
        Storage.UseBoost = 0;
        Storage.ReviveCount = 0;
    }

    public Base GetBaseConfig()
    {
        return configManager.BaseList[0];
    }

    public List<Robot> GetRobotConfig()
    {
        return configManager.RobotList;
    }

    /// <summary>
    /// 活动是否失效
    /// </summary>
    /// <returns></returns>
    private bool IsActivityDisable()
    {
        return !_model.IsActivityOpened() && !_model.IsActivityInReward();
    }

    /// <summary>
    /// 当天轮次的结束时间
    /// </summary>
    /// <returns></returns>
    private long GetTurnEndTime()
    {
        return (long)_model.StartTime + GetTurnTime() * 1000 * Storage.Turn;
    }

    /// <summary>
    /// 关卡失败后，下次能够开始挑战的时间
    /// </summary>
    /// <returns></returns>
    private long GetNextStartTimeIfFailed()
    {
        return Storage.EndChallengeTime.ToLong() + GetBaseConfig().failWaitTime * 1000;
    }

    private long GetTurnTime()
    {
#if FOR_DEV_TEST 
        return 60 ;
#else
        return GetBaseConfig().turnTime;
#endif
    }
}