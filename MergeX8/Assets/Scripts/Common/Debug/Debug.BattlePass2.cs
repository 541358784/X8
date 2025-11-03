using System.ComponentModel;
using DragonU3DSDK.Storage;


public partial class SROptions
{
    private const string BattlePass2 = "战令2期";
    [Category(BattlePass2)]
    [DisplayName("重置战令")]
    public void RestBattlePass2()
    {
        if(Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask == null)
            return;
        
        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.Clear();
        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.DailyTask.Clear();
        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.FixationTask.Clear();
        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.RefreshTime = 0;
        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.CompleteDatas.Clear();
        Activity.BattlePass_2.BattlePassModel.Instance.ClearActivityData();
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, "battlepass2buypurchase",0);
        
    }

    private TaskType _taskType2 = TaskType.Serve;
    [Category(BattlePass2)]
    [DisplayName("任务类型")]
    public TaskType TaskType2
    {
        get
        {
            return _taskType2;
        }
        set
        {
            _taskType2 = value;
        }
    }

    private int _mergeId2 = 0;
    [Category(BattlePass2)]
    [DisplayName("MergeID")]
    public int MergeId2
    {
        get { return _mergeId2; }
        set { _mergeId2 = value ; }
    }
    
    
    private int _taskNum2 = 0;
    [Category(BattlePass2)]
    [DisplayName("任务数量")]
    public int TaskNum2
    {
        get { return _taskNum2; }
        set { _taskNum2 = value ; }
    }
    
    [Category(BattlePass2)]
    [DisplayName("完成任务")]
    public void CompleteTask2()
    {
        EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, _taskType, _mergeId, _taskNum);
    }
    [Category(BattlePass2)]
    [DisplayName("完成所有任务")]
    public void CompleteAllBPTask2()
    {       
        if(Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask == null)
            return;

        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.ChallengeTask.TaskInfos.ForEach(a =>
        {
            Activity.BattlePass_2.BattlePassTaskModel.Instance.AddCompleteData(a.Id);
            a.IsComplete = true;
            a.TotalNum = 1000;
        });
        
        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.FixationTask.TaskInfos.ForEach(a =>
        {
            Activity.BattlePass_2.BattlePassTaskModel.Instance.AddCompleteData(a.Id);
            a.IsComplete = true;
            a.TotalNum = 1000;
        });
        Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.DailyTask.TaskInfos.ForEach(a =>
        {
            Activity.BattlePass_2.BattlePassTaskModel.Instance.AddCompleteData(a.Id);
            a.IsComplete = true;
            a.TotalNum = 1000;
        });
    }
    private int addStore2 = 0;
    [Category(BattlePass2)]
    [DisplayName("增加分数")]
    public int AddStore2
    {
        get { return addStore2; }
        set { addStore2 = value ; }
    }
    
        
    [Category(BattlePass2)]
    [DisplayName("增加分数")]
    public void AddBattlePassStore2()
    {
        Activity.BattlePass_2.BattlePassModel.Instance.AddScore(AddStore2);
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BATTLE_PASS_2_STORE_REFRESH, AddStore2);

    }

    private int tRefreshTime = 0;
    [Category(BattlePass2)]
    [DisplayName("任务更新时间")]
    public int TrefreshTime
    {
        get { return tRefreshTime; }
        set
        {
            tRefreshTime = value;
            Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.RefreshTime = value;
        }
    }

    [Category(BattlePass2)]
    [DisplayName("新老用户")]
    public bool IsBattleOldUser
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