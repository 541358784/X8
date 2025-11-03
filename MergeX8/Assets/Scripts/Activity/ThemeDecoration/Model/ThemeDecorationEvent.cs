using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string ThemeDecorationScoreChange = "ThemeDecorationScoreChange";
    public const string ThemeDecorationBuyStoreItem = "ThemeDecorationBuyStoreItem";
    public const string ThemeDecorationBuySuccess = "ThemeDecorationBuySuccess";
    public const string ThemeDecorationLeaderBoardScoreChange = "ThemeDecorationLeaderBoardScoreChange";
    public const string ThemeDecorationCollectStoreCompleteReward = "ThemeDecorationCollectStoreCompleteReward";
}
public class EventThemeDecorationScoreChange : BaseEvent
{
    public int ChangeValue;

    public EventThemeDecorationScoreChange() : base(EventEnum.ThemeDecorationScoreChange) { }

    public EventThemeDecorationScoreChange(int changeValue) : base(EventEnum.ThemeDecorationScoreChange)
    {
        ChangeValue = changeValue;
    }
}
public class EventThemeDecorationBuyStoreItem : BaseEvent
{
    public ThemeDecorationStoreItemConfig StoreItemConfig;

    public EventThemeDecorationBuyStoreItem() : base(EventEnum.ThemeDecorationBuyStoreItem) { }

    public EventThemeDecorationBuyStoreItem(ThemeDecorationStoreItemConfig storeItemConfig) : base(EventEnum.ThemeDecorationBuyStoreItem)
    {
        StoreItemConfig = storeItemConfig;
    }
}

public class EventThemeDecorationBuySuccess : BaseEvent
{
    public StorageThemeDecoration Storage;
    public EventThemeDecorationBuySuccess() : base(EventEnum.ThemeDecorationBuySuccess) { }

    public EventThemeDecorationBuySuccess(StorageThemeDecoration storage) : base(EventEnum.ThemeDecorationBuySuccess)
    {
        Storage = storage;
    }
}

public class EventThemeDecorationLeaderBoardScoreChange : BaseEvent
{
    public StorageThemeDecorationLeaderBoard Storage;

    public EventThemeDecorationLeaderBoardScoreChange() : base(EventEnum.ThemeDecorationLeaderBoardScoreChange) { }

    public EventThemeDecorationLeaderBoardScoreChange(StorageThemeDecorationLeaderBoard storage) : base(EventEnum.ThemeDecorationLeaderBoardScoreChange)
    {
        Storage = storage;
    }
}

public class EventThemeDecorationCollectStoreCompleteReward : BaseEvent
{
    public ThemeDecorationStoreLevelConfig StoreLevelConfig;

    public EventThemeDecorationCollectStoreCompleteReward() : base(EventEnum.ThemeDecorationCollectStoreCompleteReward) { }

    public EventThemeDecorationCollectStoreCompleteReward(ThemeDecorationStoreLevelConfig storeLevelConfig) : base(EventEnum.ThemeDecorationCollectStoreCompleteReward)
    {
        StoreLevelConfig = storeLevelConfig;
    }
}