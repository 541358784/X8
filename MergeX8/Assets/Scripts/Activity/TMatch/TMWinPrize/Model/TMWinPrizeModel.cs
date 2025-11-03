using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMWinPrize;
using DragonPlus.ConfigHub.TMatchLevel;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using TMatch;
using UnityEngine;

public class TMWinPrizeModel : ActivityEntityBase
{

    public bool ShowEntrance()
    {
        return IsOpened();
    }
    private static TMWinPrizeModel _instance;
    public static TMWinPrizeModel Instance => _instance ?? (_instance = new TMWinPrizeModel());
    public override string Guid => "OPS_EVENT_TYPE_TM_WIN_PRIZE";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = TMWinPrizeConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public Dictionary<int, TMWinPrizeRewardConfig> RewardConfigDic = new Dictionary<int, TMWinPrizeRewardConfig>();

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        TMWinPrizeConfigManager.Instance.InitConfig(configJson);
        InitTable(RewardConfigDic);
        _lastActivityOpenState = IsOpened();
        InitServerDataFinish();
        InitAux();
    }
    public TMWinPrizeModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
        TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.TMATCH_GAME_WIN_BEFORE_ADD_MAIN_LEVEL,OnWin);
    }
    
    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;

        var currentActivityOpenState = IsOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (!currentActivityOpenState)
        {
        }
        else
        {
        }
        _lastActivityOpenState = currentActivityOpenState;
    }

    public List<ResData> GetCurReward()
    {
        return GetWinReward(TMatchModel.Instance.GetMainLevel());
    }
    public List<ResData> GetWinReward(int level)
    {
        var difficulty = (int)TMatchConfigManager.Instance.GetDifficulty(level);
        List<ResData> reward = null;
        if (IsOpened() && RewardConfigDic.TryGetValue(difficulty, out var config))
        {
            reward = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
        }
        return reward;
    }

    public List<ResData> GetLastWinReward()
    {
        if (LastGetPrizeWinLevel > 0)
        {
            var rewards = GetWinReward(LastGetPrizeWinLevel);
            LastGetPrizeWinLevel = -1;
            return rewards;
        }
        return null;
    }

    private int LastGetPrizeWinLevel = -1;
    public void CollectWinPrize(int winLevel)
    {
        var reward = GetWinReward(winLevel);
        if (reward == null)
            return;
        LastGetPrizeWinLevel = winLevel;
        UserData.Instance.AddRes(reward,new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SlotMachineGet
        });
    }
    public void OnWin(TMatch.BaseEvent evt)
    {
        CollectWinPrize(TMatchModel.Instance.GetMainLevel());
    }
    public override bool CanDownLoadRes()
    {
        return true;
    }
    public async void InitAux()
    {
    }
}