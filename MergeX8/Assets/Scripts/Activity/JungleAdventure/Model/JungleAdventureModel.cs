
using Activity.Base;
using Activity.JungleAdventure.Controller;
using Activity.PhotoAlbum.View;
using DragonPlus;
using DragonPlus.Config.JungleAdventure;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using UnityEngine;

public class JungleAdventureModel: ActivityEntityBase, I_ActivityStatus
{
    public override string Guid => "OPS_EVENT_TYPE_JUNGLE_ADVENTURE";
    
    
    private static JungleAdventureModel _instance;
    public static JungleAdventureModel Instance => _instance ?? (_instance = new JungleAdventureModel());
    
    public StorageJungleAdventure JungleAdventure
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().JungleAdventure;
        }
    }
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.JungleAdventure);
    }
    
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
        
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        JungleAdventure.ActivityJson = configJson;
        JungleAdventureConfigManager.Instance.InitConfig(configJson);
        JungleAdventureLeaderBoardModel.Instance.InitFromServerData();
        InitServerDataFinish();
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        if(ActivityId.IsEmptyString())
            return;
            
        if (JungleAdventure.ActivityId == ActivityId)
        {
            JungleAdventure.JoinStartTime = (long)StartTime;
            JungleAdventure.JoinEndTime = (long)EndTime;
        }
        else
        {
            JungleAdventure.Clear();
            JungleAdventure.ActivityId = ActivityId;
            JungleAdventure.JoinStartTime = (long)StartTime;
            JungleAdventure.JoinEndTime = (long)EndTime;
            JungleAdventure.PreActivityEndTime = JungleAdventure.JoinStartTime + JungleAdventureConfigManager.Instance.TableJungleAdventureSettingList[0].PreOpenTime * 60 * 1000;

            JungleAdventure.GetRewardState.Clear();
            JungleAdventure.Stage = 0;

            JungleAdventure.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().JungleAdventureGroupId;
        }
        
        XUtility.WaitFrames(1).AddCallBack(() =>
        {
            JungleAdventureLeaderBoardModel.Instance.CreateStorage(JungleAdventure); 
        });
    }

    public int PayLevelGroup()
    {
        return JungleAdventure.PayLevelGroup;
    }
    
    public virtual string GetEndTimeString()
    {
        if (!IsOpened())
            return "";

        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.JungleAdventure))
            return "";
            
        var left = (long) EndTime - (long) APIManager.Instance.GetServerTime();
        if (left < 0)
            left = 0;
            
        return CommonUtils.FormatLongToTimeStr(left);
    }
    
    public bool IsPreheatEnd()
    {
        if (!IsOpened())
            return false;

        if (IsSkipActivityPreheating())
            return true;
            
        return (long)APIManager.Instance.GetServerTime() - JungleAdventure.PreActivityEndTime > 0;
    }
    
    public string GetPreheatEndTimeString()
    {
        if (!IsOpened())
            return "";
            
        var left = JungleAdventure.PreActivityEndTime - (long) APIManager.Instance.GetServerTime();
        if (left < 0)
            left = 0;
            
        return CommonUtils.FormatLongToTimeStr(left);
    }
    
    public override bool IsOpened(bool hasLog = false)
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.JungleAdventure))
            return false;
            
        return base.IsOpened(hasLog);
    }

    public bool IsFinish()
    {
        int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;

        bool isFinish = stage >= JungleAdventureConfigManager.Instance.GetConfigs().Count;
        
        if(isFinish)
            Instance.SetActivityStatus(I_ActivityStatus.ActivityStatus.Completed);

        return isFinish;
    }

    public void AddScore(int value)
    {
        int oldValue = JungleAdventure.CurrentScore;
        JungleAdventure.CurrentScore += value;
        JungleAdventure.TotalScore += value;
        if (value > 0)
        {
            JungleAdventureLeaderBoardModel.Instance.GetLeaderBoardStorage(JungleAdventure.ActivityId)?.CollectStar(value);   
        }
        
        EventDispatcher.Instance.DispatchEvent(EventEnum.Event_Refre_JungleAdventure_Score, oldValue, JungleAdventure.CurrentScore);
        GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.JungleAdventure, value, (ulong)JungleAdventure.TotalScore, new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.JunbleAdventureGet
        });
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventJungleAdventureChange, value.ToString(), JungleAdventure.TotalScore.ToString());
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
        var configs = JungleAdventureConfigManager.Instance.TableJungleAdventureRewardConfigList;
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

    public bool IsActivityTimeEnd()
    {
        return (long)APIManager.Instance.GetServerTime() < Instance.JungleAdventure.JoinEndTime;
    }
    
    public static bool CanShow()
    {
        if (Instance.JungleAdventure.ActivityId.IsEmptyString())
            return false;
        
        if (Instance.IsOpened())
            return false;

        if ((long)APIManager.Instance.GetServerTime() < Instance.JungleAdventure.JoinEndTime)
            return false;

        if (Instance.JungleAdventure.AnimScore == Instance.JungleAdventure.CurrentScore)
            return false;
        
        if (Instance.JungleAdventure.ActivityJson == null)
            return false;

        if (JungleAdventureLeaderBoardModel.Instance.GetLeaderBoardStorage(Instance.JungleAdventure.ActivityId) == null)
            return false;
        
        if (!JungleAdventureLeaderBoardModel.Instance.GetLeaderBoardStorage(Instance.JungleAdventure.ActivityId).IsResExist())
            return false;
        
        JungleAdventureConfigManager.Instance.InitConfig(Instance.JungleAdventure.ActivityJson);
        if (Instance.JungleAdventure.Stage >= JungleAdventureConfigManager.Instance.GetConfigs().Count)
        {
            //Instance.JungleAdventure.ActivityId = "";
            return false;
        }
        if (Instance.JungleAdventure.ActivityStatus < (int)I_ActivityStatus.ActivityStatus.Incomplete)
        {
            Instance.SetActivityStatus(I_ActivityStatus.ActivityStatus.Incomplete);
        }
        UIManager.Instance.OpenWindow(UINameConst.UIJungleAdventureMain);
        return true;
    }

    public I_ActivityStatus.ActivityStatus GetActivityStatus()
    {
        return (I_ActivityStatus.ActivityStatus)JungleAdventure.ActivityStatus;
    }
        
    public void SetActivityStatus(I_ActivityStatus.ActivityStatus status)
    {
        JungleAdventure.ActivityStatus = (int)status;
    }
    
    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_JungleAdventure>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_JungleAdventure>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
}