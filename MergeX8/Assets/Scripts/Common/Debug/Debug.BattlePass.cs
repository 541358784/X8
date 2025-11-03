using System.ComponentModel;
using Activity.BattlePass;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;


public partial class SROptions
{
    private const string BattlePass = "战令";
    [Category(BattlePass)]
    [DisplayName("重置战令")]
    public void RestBattlePass()
    {
        if(BattlePassTaskModel.Instance.battlePassTask == null)
            return;
        
        BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.Clear();
        BattlePassTaskModel.Instance.battlePassTask.DailyTask.Clear();
        BattlePassTaskModel.Instance.battlePassTask.FixationTask.Clear();
        BattlePassTaskModel.Instance.battlePassTask.RefreshTime = 0;
        BattlePassTaskModel.Instance.battlePassTask.CompleteDatas.Clear();
        BattlePassModel.Instance.ClearActivityData();
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, "battlepassbuypurchase",0);
    }

    private TaskType _taskType = TaskType.Serve;
    [Category(BattlePass)]
    [DisplayName("任务类型")]
    public TaskType TaskType
    {
        get
        {
            return _taskType;
        }
        set
        {
            _taskType = value;
        }
    }

    private int _mergeId = 0;
    [Category(BattlePass)]
    [DisplayName("MergeID")]
    public int MergeId
    {
        get { return _mergeId; }
        set { _mergeId = value ; }
    }
    
    
    private int _taskNum = 0;
    [Category(BattlePass)]
    [DisplayName("任务数量")]
    public int TaskNum
    {
        get { return _taskNum; }
        set { _taskNum = value ; }
    }
    
    [Category(BattlePass)]
    [DisplayName("完成任务")]
    public void CompleteTask()
    {
        EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, _taskType, _mergeId, _taskNum);
    }
    [Category(BattlePass)]
    [DisplayName("完成所有任务")]
    public void CompleteAllBPTask()
    {       
        if(BattlePassTaskModel.Instance.battlePassTask == null)
            return;

        BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.TaskInfos.ForEach(a =>
        {
            BattlePassTaskModel.Instance.AddCompleteData(a.Id);
            a.IsComplete = true;
            a.TotalNum = 1000;
        });
        
        BattlePassTaskModel.Instance.battlePassTask.FixationTask.TaskInfos.ForEach(a =>
        {
            BattlePassTaskModel.Instance.AddCompleteData(a.Id);
            a.IsComplete = true;
            a.TotalNum = 1000;
        });
        BattlePassTaskModel.Instance.battlePassTask.DailyTask.TaskInfos.ForEach(a =>
        {
            BattlePassTaskModel.Instance.AddCompleteData(a.Id);
            a.IsComplete = true;
            a.TotalNum = 1000;
        });
    }
    private int addStore = 0;
    [Category(BattlePass)]
    [DisplayName("增加分数")]
    public int AddStore
    {
        get { return addStore; }
        set { addStore = value ; }
    }
    
        
    [Category(BattlePass)]
    [DisplayName("增加分数")]
    public void AddBattlePassStore()
    {
        BattlePassModel.Instance.AddScore(AddStore);
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BATTLE_PASS_STORE_REFRESH, AddStore);

    }

    private int tRefreshTime1 = 0;
    [Category(BattlePass)]
    [DisplayName("任务更新时间")]
    public int TrefreshTime1
    {
        get { return tRefreshTime1; }
        set
        {
            tRefreshTime1 = value;
            Activity.BattlePass.BattlePassTaskModel.Instance.battlePassTask.RefreshTime = value;
        }
    }

    [Category(BattlePass)]
    [DisplayName("新老用户")]
    public bool IsOldUser
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey("BattlePass");
        }
        set
        {
            if (value == true)
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig["BattlePass"] = "BattlePass";
            else
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Remove("BattlePass");
        }
    }
    
}