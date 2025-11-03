public partial class EventEnum
{
    public const string KeepPetStateChange = "KeepPetStateChange";
    public const string KeepPetFrisbeeCountChange = "KeepPetFrisbeeCountChange";
    public const string KeepPetMedicineCountChange = "KeepPetMedicineCountChange";
    public const string KeepPetSearchPropCountChange = "KeepPetSearchPropCountChange";
    public const string KeepPetDogHeadChange = "KeepPetDogHeadChange";
    public const string KeepPetExpChange = "KeepPetExpChange";
    public const string KeepPetAwakeStateChange = "KeepPetAwakeStateChange";
    public const string KeepPetGetNewBuildingItem = "KeepPetGetNewBuildingItem";
    public const string KeepPetBuildingActiveChange = "KeepPetBuildingActiveChange";
    public const string KeepPetCollectLevelReward = "KeepPetCollectLevelReward";
    public const string KeepPetCollectFinalDailyTaskReward = "KeepPetCollectFinalDailyTaskReward";
    public const string KeepPetCollectDailyTaskReward = "KeepPetCollectDailyTaskReward";
    public const string KeepPetChangeSkin = "KeepPetChangeSkin";
}

public class EventKeepPetStateChange : BaseEvent
{
    public KeepPetStateEnum OldState;
    public KeepPetStateEnum NewState;

    public EventKeepPetStateChange() : base(EventEnum.KeepPetStateChange) { }

    public EventKeepPetStateChange(KeepPetStateEnum oldState,KeepPetStateEnum newState) : base(EventEnum.KeepPetStateChange)
    {
        OldState = oldState;
        NewState = newState;
    }
}

public class EventKeepPetFrisbeeCountChange : BaseEvent
{
    public int OldValue;
    public int NewValue;
    public EventKeepPetFrisbeeCountChange() : base(EventEnum.KeepPetFrisbeeCountChange) { }
    public EventKeepPetFrisbeeCountChange(int oldValue,int newValue) : base(EventEnum.KeepPetFrisbeeCountChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKeepPetMedicineCountChange : BaseEvent
{
    public int OldValue;
    public int NewValue;
    public EventKeepPetMedicineCountChange() : base(EventEnum.KeepPetMedicineCountChange) { }
    public EventKeepPetMedicineCountChange(int oldValue,int newValue) : base(EventEnum.KeepPetMedicineCountChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKeepPetSearchPropCountChange : BaseEvent
{
    public int OldValue;
    public int NewValue;
    public EventKeepPetSearchPropCountChange() : base(EventEnum.KeepPetSearchPropCountChange) { }
    public EventKeepPetSearchPropCountChange(int oldValue,int newValue) : base(EventEnum.KeepPetSearchPropCountChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

public class EventKeepPetDogHeadChange : BaseEvent
{
    public int OldValue;
    public int NewValue;
    public EventKeepPetDogHeadChange() : base(EventEnum.KeepPetDogHeadChange) { }
    public EventKeepPetDogHeadChange(int oldValue,int newValue) : base(EventEnum.KeepPetDogHeadChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKeepPetExpChange : BaseEvent
{
    public int OldValue;
    public int NewValue;
    public EventKeepPetExpChange() : base(EventEnum.KeepPetExpChange) { }
    public EventKeepPetExpChange(int oldValue,int newValue) : base(EventEnum.KeepPetExpChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKeepPetAwakeStateChange : BaseEvent
{
    public bool OldValue;
    public bool NewValue;
    public EventKeepPetAwakeStateChange() : base(EventEnum.KeepPetAwakeStateChange) { }
    public EventKeepPetAwakeStateChange(bool oldValue,bool newValue) : base(EventEnum.KeepPetAwakeStateChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKeepPetGetNewBuildingItem : BaseEvent
{
    public KeepPetBuildingItemConfig BuildingItem;
    public EventKeepPetGetNewBuildingItem() : base(EventEnum.KeepPetGetNewBuildingItem) { }
    public EventKeepPetGetNewBuildingItem(KeepPetBuildingItemConfig buildingItem) : base(EventEnum.KeepPetGetNewBuildingItem)
    {
        BuildingItem = buildingItem;
    }
}

public class EventKeepPetBuildingActiveChange : BaseEvent
{
    public KeepPetBuildingHangPointConfig HangPoint;
    public KeepPetBuildingItemConfig ItemOld;
    public KeepPetBuildingItemConfig ItemNew;
    public EventKeepPetBuildingActiveChange() : base(EventEnum.KeepPetBuildingActiveChange) { }
    public EventKeepPetBuildingActiveChange(KeepPetBuildingHangPointConfig hangPoint,
        KeepPetBuildingItemConfig itemOld,KeepPetBuildingItemConfig itemNew)
        : base(EventEnum.KeepPetBuildingActiveChange)
    {
        HangPoint = hangPoint;
        ItemOld = itemOld;
        ItemNew = itemNew;
    }
}

public class EventKeepPetCollectLevelReward : BaseEvent
{
    public KeepPetLevelConfig Level;
    public EventKeepPetCollectLevelReward() : base(EventEnum.KeepPetCollectLevelReward) { }
    public EventKeepPetCollectLevelReward(KeepPetLevelConfig level) : base(EventEnum.KeepPetCollectLevelReward)
    {
        Level = level;
    }
}
public class EventKeepPetCollectFinalDailyTaskReward : BaseEvent
{
    public EventKeepPetCollectFinalDailyTaskReward() : base(EventEnum.KeepPetCollectFinalDailyTaskReward) { }
}
public class EventKeepPetCollectDailyTaskReward : BaseEvent
{
    public EventKeepPetCollectDailyTaskReward() : base(EventEnum.KeepPetCollectDailyTaskReward) { }
}
public class EventKeepPetChangeSkin : BaseEvent
{
    public EventKeepPetChangeSkin() : base(EventEnum.KeepPetChangeSkin) { }
}