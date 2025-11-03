using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string FlowerField_SCORE_CHANGE = "FlowerField_SCORE_CHANGE";
}
public class EventFlowerFieldScoreChange : BaseEvent
{
    public int ChangeValue;

    public EventFlowerFieldScoreChange() : base(EventEnum.FlowerField_SCORE_CHANGE) { }

    public EventFlowerFieldScoreChange(int changeValue) : base(EventEnum.FlowerField_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
    }
}