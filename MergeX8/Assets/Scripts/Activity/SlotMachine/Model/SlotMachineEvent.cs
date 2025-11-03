using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string SlotMachineEnd = "SlotMachineEnd";
    public const string SlotMachineScoreChange = "SlotMachineScoreChange";
    public const string SlotMachinePerformSpinReel = "SlotMachinePerformSpinReel";
    public const string SlotMachinePerformSpinSingleReel = "SlotMachinePerformSpinSingleReel";
    public const string SlotMachineCollectReward = "SlotMachineCollectReward";
}
public class EventSlotMachineEnd : BaseEvent
{
    public EventSlotMachineEnd() : base(EventEnum.SlotMachineEnd) { }
}

public class EventSlotMachineScoreChange : BaseEvent
{
    public int ChangeValue;

    public EventSlotMachineScoreChange() : base(EventEnum.SlotMachineScoreChange) { }

    public EventSlotMachineScoreChange(int changeValue) : base(EventEnum.SlotMachineScoreChange)
    {
        ChangeValue = changeValue;
    }
}
public class EventSlotMachinePerformSpinReel: BaseEvent
{
    public StorageSlotMachine Storage;
    public EventSlotMachinePerformSpinReel() : base(EventEnum.SlotMachinePerformSpinReel) { }

    public EventSlotMachinePerformSpinReel(StorageSlotMachine storage) : base(EventEnum.SlotMachinePerformSpinReel)
    {
        Storage = storage;
    }
}

public class EventSlotMachinePerformSingleSpinReel: BaseEvent
{
    public StorageSlotMachine Storage;
    public int ReelIndex;
    public EventSlotMachinePerformSingleSpinReel() : base(EventEnum.SlotMachinePerformSpinSingleReel) { }

    public EventSlotMachinePerformSingleSpinReel(StorageSlotMachine storage,int reelIndex) : base(EventEnum.SlotMachinePerformSpinSingleReel)
    {
        Storage = storage;
        ReelIndex = reelIndex;
    }
}
public class EventSlotMachineCollectReward : BaseEvent
{
    public StorageSlotMachine Storage;
    public EventSlotMachineCollectReward() : base(EventEnum.SlotMachineCollectReward) { }
    public EventSlotMachineCollectReward(StorageSlotMachine storage) : base(EventEnum.SlotMachineCollectReward)
    {
        Storage = storage;
    }
}