using System;
using System.Collections.Generic;
using Activity.TreasureMap;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Merge.Order;

public enum KeepPetStateEnum
{
    Happy=0,
    Hunger=1,
    Sleep=2,
    Searching=5,
    SearchFinish=6,
}
public abstract class KeepPetBaseState
{
    public StorageKeepPet Storage { get; set; }
    public abstract KeepPetStateEnum Enum { get; }
    public abstract KeepPetStateEnum CheckStateChange();

    public virtual void QuitState()
    {
        
    }
    public virtual void EnterState()
    {
        Storage.CurPetState = (int) Enum;
    }
}

public class KeepPetStateHappy : KeepPetBaseState
{
    public override KeepPetStateEnum Enum => KeepPetStateEnum.Happy;
    public override KeepPetStateEnum CheckStateChange()
    {
        Storage.CheckHungry();
        if (Storage.SearchTaskId > 0)
        {
            return KeepPetStateEnum.Searching;   
        }
        if (!Storage.Cure)
        {
            if (Storage.MedicineCount == 0)
            {
                if (KeepPetModel.Instance.IsOpen())
                    MainOrderCreateKeepPet.TryCreateOrder(SlotDefinition.KeepPet); 
            }
            return KeepPetStateEnum.Hunger;
        }
        if (Storage.IsSleepByTime())
        {
            return KeepPetStateEnum.Sleep;
        }
        return Enum;
    }
}

public class KeepPetStateHunger : KeepPetBaseState
{
    public override KeepPetStateEnum Enum => KeepPetStateEnum.Hunger;
    
    public override KeepPetStateEnum CheckStateChange()
    {
        if (Storage.Cure)
        {
            return KeepPetStateEnum.Happy;
        }
        // if (Storage.IsSleepByTime())
        // {
        //     return KeepPetStateEnum.Sleep;
        // }
        return Enum;
    }
}
public class KeepPetStateSleep : KeepPetBaseState
{
    public override KeepPetStateEnum Enum => KeepPetStateEnum.Sleep;
    
    public override KeepPetStateEnum CheckStateChange()
    {
        Storage.CheckHungry();
        if (!Storage.Cure)
            return KeepPetStateEnum.Hunger;
        if (Storage.IsSleepByTime())
        {
            return Enum;
        }
        return KeepPetStateEnum.Happy;
    }

    public override void EnterState()
    {
        base.EnterState();
        EventDispatcher.Instance.SendEventImmediately<EventKeepPetAwakeStateChange>(
            new EventKeepPetAwakeStateChange(false, true));
    }

    public override void QuitState()
    {
        base.QuitState();
        EventDispatcher.Instance.SendEventImmediately<EventKeepPetAwakeStateChange>(
            new EventKeepPetAwakeStateChange(true, false));
    }
}

public class KeepPetStateSearching : KeepPetBaseState
{
    public override KeepPetStateEnum Enum => KeepPetStateEnum.Searching;
    public override KeepPetStateEnum CheckStateChange()
    {
        if ((long) APIManager.Instance.GetServerTime() >= Storage.SearchEndTime)
        {
            return KeepPetStateEnum.SearchFinish;
        }
        return Enum;
    }

    public override void EnterState()
    {
        base.EnterState();
        Storage.SearchStartTime = (long) APIManager.Instance.GetServerTime();
        var taskConfig = KeepPetModel.Instance.SearchTaskConfig.Find(a => a.Id == Storage.SearchTaskId);
        Storage.SearchEndTime = Storage.SearchStartTime + taskConfig.Time * (long) XUtility.Min;
    }
}

public class KeepPetStateSearchFinish : KeepPetBaseState
{
    public override KeepPetStateEnum Enum => KeepPetStateEnum.SearchFinish;

    public override KeepPetStateEnum CheckStateChange()
    {
        if (Storage.CollectSearchTaskReward)
        {
            var taskConfig = KeepPetModel.Instance.SearchTaskConfig.Find(a => a.Id == Storage.SearchTaskId);
            Storage.SearchTaskId = 0;
            Storage.SearchTaskRewardList.Clear();
            Storage.GetTreasureMap = false;
            Storage.SearchTaskExtraSelectRewardCount = 0;
            return KeepPetStateEnum.Happy;
        }
        return Enum;
    }
    public override void EnterState()
    {
        base.EnterState();
        Storage.CollectSearchTaskReward = false;
        Storage.SearchTaskExtraSelectRewardCount = 0;
        Storage.SearchTaskRewardList.Clear();
        var taskConfig = KeepPetModel.Instance.SearchTaskConfig.Find(a => a.Id == Storage.SearchTaskId);
        Storage.GetTreasureMap = TreasureMapModel.Instance.FinishTask(taskConfig.MapExp);
        var rewards = KeepPetModel.Instance.GetSearchTaskRewards(Storage.SearchTaskId);
        foreach (var reward in rewards)
        {
            Storage.SearchTaskRewardList.Add(new StorageResData()
            {
                Id = reward.id,
                Count = reward.count
            });
        }
    }
}