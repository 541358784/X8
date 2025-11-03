using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string FishCultureScoreChange = "FishCultureScoreChange";
    public const string FishCultureGetNewFish = "FishCultureGetNewFish";
}
public class EventFishCultureScoreChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventFishCultureScoreChange() : base(EventEnum.FishCultureScoreChange) { }

    public EventFishCultureScoreChange(int oldValue,int newValue) : base(EventEnum.FishCultureScoreChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventFishCultureGetNewFish : BaseEvent
{
    public FishCultureRewardConfig FishConfig;

    public EventFishCultureGetNewFish() : base(EventEnum.FishCultureGetNewFish) { }

    public EventFishCultureGetNewFish(FishCultureRewardConfig fishConfig) : base(EventEnum.FishCultureGetNewFish)
    {
        FishConfig = fishConfig;
    }
}