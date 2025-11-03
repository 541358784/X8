using System;
using System.Collections.Generic;
using DragonPlus.Config.WinStreak;
using DragonU3DSDK.Storage;
using TMatch;
using UnityEngine;

public class CrocodileActivityModel : ActivityEntityBase
{
    private CrocodileActivityData data;
    private CrocodileActivityData _data
    {
        get
        {
            if (data == null)
                data = new CrocodileActivityData(this);
            return data;
        }
    }
    public StorageActivityWinStreak Storage => _data.Storage;
    
    private static CrocodileActivityModel _instance;
    public static CrocodileActivityModel Instance => _instance ?? (_instance = new CrocodileActivityModel());
    
    public readonly int TOTAL_PLAT_COUNT = 7;

    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public override string Guid => "OPS_EVENT_TYPE_CROCODILE";

    public override void InitFromServerData(string activityId, string activityType,
        ulong startTime, ulong endTime, ulong rewardEndTime, bool manualEnd,
        string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson, activitySubType);

        var config = new WinStreakConfigManager();
        config.InitConfig(configJson);
       
        _data.Init(config);
    }

    public Base GetBaseConfig()
    {
        return _data.GetBaseConfig();
    }

    public List<Robot> GetRobotConfig()
    {
        return _data.GetRobotConfig();
    }

    /// <summary>
    /// 游戏开始事件处理, 暂存连赢次数
    /// </summary>
    public void OnGameStart()
    {
        _data.OnGameStart();
    }

    /// <summary>
    /// 关卡胜利事件处理
    /// </summary>
    public void OnGameWin()
    {
        _data.OnGameWin();
    }

    /// <summary>
    /// 关卡失败事件处理
    /// </summary>
    public void OnGameFailed()
    {
        _data.OnGameFailed();
    }

    public  bool IsActivityOpened()
    {
        if (!base.IsOpened()) return false;
        if (GetBaseConfig().openLevel > TMatchModel.Instance.GetMainLevel()) return false;
        return true;
    }

    public bool CanShowGateView()
    {
        return (IsActivityOpened() || IsActivityInReward()) ;
    }

    public  bool IsActivityInReward()
    {
        if (!base.IsInReward()) return false;
        if (GetBaseConfig().openLevel > TMatchModel.Instance.GetMainLevel()) return false;
        return true;
    }

    /// <summary>
    /// 获取当前轮次的剩余时间
    /// </summary>
    /// <returns></returns>
    public long GetCurrentActivityLeftTime()
    {
        return _data.GetCurrentActivityLeftTime();
    }

    /// <summary>
    /// 获取当前轮次的剩余时间,如果剩余时间小于1，则轮次+1
    /// </summary>
    /// <returns></returns>
    public long GetCurrentTurnLeftTime()
    {
        return _data.GetCurrentTurnLeftTime();
    }

    // 更新当前的轮次信息
    public void UpdateCurrentTurn()
    {
        _data.UpdateCurrentTurn();
    }

    public long GetChallengeLeftTime()
    {
        return _data.GetChallengeLeftTime();
    }

    /// <summary>
    /// 获取能够开始挑战的剩余时间（一定会有下一次挑战的情况下）
    /// </summary>
    /// <returns></returns>
    public long GetCanChallengeLeftTime()
    {
        return _data.GetCanChallengeLeftTime();
    }

    /// <summary>
    /// 判断失败时，是否能在当前这一轮开启
    /// </summary>
    /// <returns></returns>
    public bool CanStartInCurrentTurn()
    {
        return _data.CanStartInCurrentTurn();
    }

    /// <summary>
    /// 检测是否超时
    /// </summary>
    public void CheckChallengeIsOutTime()
    {
        _data.CheckChallengeIsOutTime();
    }

    /// <summary>
    /// 是否正在挑战中
    /// </summary>
    /// <returns></returns>
    public bool IsInChallenge()
    {
        return _data.IsInChallenge();
    }

    /// <summary>
    /// 是否是完成领奖的状态
    /// </summary>
    /// <returns></returns>
    public bool IsFinishCurrentTurnReward()
    {
        return _data.IsFinishCurrentTurnReward();
    }

    /// <summary>
    /// 是否可以开始挑战
    /// </summary>
    /// <returns></returns>
    public bool CanStartChallenge()
    {
        return _data.CanStartChallenge();
    }

    public int GetStartChallengeTurn()
    {
        return _data.GetStartChallengeTurn();
    }

    public void StartChallenge()
    {
        _data.StartChallenge();
    }

    /// <summary>
    /// 获取当前时间 此活动获取时间都用这个接口，好保持统一，方便调整
    /// </summary>
    /// <returns></returns>
    public long GetNowTime()
    {
        return _data.GetNowTime();
    }

    public bool TryToPopupWinInGame()
    {
        if (_data == null)
            return false;
        
        return _data.TryToPopupWinInGame();
    }

    public bool TryToShowPopup()
    {
        if (_data == null)
            return false;
        return _data.TryToShowPopup();
    }

    /// <summary>
    /// 局内使用道具
    /// </summary>
    public void UseBoost()
    {
        _data.UseBoost();
    }

    /// <summary>
    /// 局内使用复活
    /// </summary>
    public void GameRevive()
    {
        _data.GameRevive();
    }

    /// <summary>
    /// 主界面动画播放之前调用
    /// </summary>
    public void PreJump()
    {
        _data.PreJump();
    }

    /// <summary>
    /// 计算一次机器人的淘汰
    /// </summary>
    public void UpdateRobotCount()
    {
        _data.UpdateStorageRobot();
    }

    /// <summary>
    /// 挑战成功时，获取到的金币数额
    /// </summary>
    /// <returns></returns>
    public int GetMyPrize()
    {
        return _data.GetMyPrize();
    }
    public override bool CanDownLoadRes()
    {
        return TMatchModel.Instance.GetMainLevel() > 30;
    }
}