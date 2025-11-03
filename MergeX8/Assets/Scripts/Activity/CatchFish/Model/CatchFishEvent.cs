using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string CatchFishItemChange = "CatchFishItemChange";
    public const string CatchFishCollectStateChange = "CatchFishCollectStateChange";
}
public class EventCatchFishItemChange : BaseEvent
{
    public int ChangeValue;

    public EventCatchFishItemChange() : base(EventEnum.CatchFishItemChange) { }

    public EventCatchFishItemChange(int changeValue) : base(EventEnum.CatchFishItemChange)
    {
        ChangeValue = changeValue;
    }
}

public class EventCatchFishCollectStateChange : BaseEvent
{
    public int ChangeValue;

    public EventCatchFishCollectStateChange() : base(EventEnum.CatchFishCollectStateChange) { }

    public EventCatchFishCollectStateChange(int changeValue) : base(EventEnum.CatchFishCollectStateChange)
    {
        ChangeValue = changeValue;
    }
}