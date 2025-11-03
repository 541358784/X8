using System;
using System.Collections.Generic;
using System.Linq;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Easter;
using DragonPlus.Config.Mermaid;
using DragonPlus.Config.PayRebate;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class WeeklyChallengeModel : ActivityEntityBase
{
    private static WeeklyChallengeModel _instance;
    public static WeeklyChallengeModel Instance => _instance ?? (_instance = new WeeklyChallengeModel());

    public override string Guid => "OPS_EVENT_TYPE_WEEKLYCHALLENGE";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime, ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson, activitySubType);
        DragonPlus.Config.WeeklyChallenge.WeeklyChallengeConfigManager.Instance.InitConfig(configJson);
        EventDispatcher.Instance.DispatchEvent(new BaseEvent(TMatch.EventEnum.TMATCH_EVENT_UPDATE));
        
        TMatch.WeeklyChallengeController.Instance.InitModel();
    }
    public override void UpdateActivityState()
    {
        EventDispatcher.Instance.DispatchEvent(new BaseEvent(TMatch.EventEnum.TMATCH_EVENT_UPDATE));
    }
    
    protected override void InitServerDataFinish()
    {
        
    }
    public override bool CanDownLoadRes()
    {
        return true;
    }
}