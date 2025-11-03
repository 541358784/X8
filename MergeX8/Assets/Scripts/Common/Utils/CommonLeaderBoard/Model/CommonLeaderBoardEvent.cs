using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string CommonLeaderBoardScoreChange = "CommonLeaderBoardScoreChange";
}
public class EventCommonLeaderBoardScoreChange : BaseEvent
{
    public StorageCommonLeaderBoard Storage;

    public EventCommonLeaderBoardScoreChange() : base(EventEnum.CommonLeaderBoardScoreChange) { }

    public EventCommonLeaderBoardScoreChange(StorageCommonLeaderBoard storage) : base(EventEnum.CommonLeaderBoardScoreChange)
    {
        Storage = storage;
    }
}