using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string PillowWheelItemChange = "PillowWheelItemChange";
    public const string PillowWheelCollectStateChange = "PillowWheelCollectStateChange";
}
public class EventPillowWheelItemChange : BaseEvent
{
    public int ChangeValue;

    public EventPillowWheelItemChange() : base(EventEnum.PillowWheelItemChange) { }

    public EventPillowWheelItemChange(int changeValue) : base(EventEnum.PillowWheelItemChange)
    {
        ChangeValue = changeValue;
    }
}

public class EventPillowWheelCollectStateChange : BaseEvent
{
    public int ChangeValue;

    public EventPillowWheelCollectStateChange() : base(EventEnum.PillowWheelCollectStateChange) { }

    public EventPillowWheelCollectStateChange(int changeValue) : base(EventEnum.PillowWheelCollectStateChange)
    {
        ChangeValue = changeValue;
    }
}