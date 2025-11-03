using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string Parrot_SCORE_CHANGE = "Parrot_SCORE_CHANGE";
}
public class EventParrotScoreChange : BaseEvent
{
    public int ChangeValue;

    public EventParrotScoreChange() : base(EventEnum.Parrot_SCORE_CHANGE) { }

    public EventParrotScoreChange(int changeValue) : base(EventEnum.Parrot_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
    }
}