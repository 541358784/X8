using Gameplay;

public partial class EventEnum
{
    public const string UserDataAddRes = "UserDataAddRes";
    public const string UserDataConsumeRes = "UserDataConsumeRes";
}

public class EventUserDataAddRes : BaseEvent
{
    public UserData.ResourceId ResId;
    public int Count;

    public EventUserDataAddRes() : base(EventEnum.UserDataAddRes) { }

    public EventUserDataAddRes(UserData.ResourceId resId,int count) : base(
        EventEnum.UserDataAddRes)
    {
        ResId = resId;
        Count = count;
    }
}
public class EventUserDataConsumeRes : BaseEvent
{
    public UserData.ResourceId ResId;
    public int Count;

    public EventUserDataConsumeRes() : base(EventEnum.UserDataConsumeRes) { }

    public EventUserDataConsumeRes(UserData.ResourceId resId,int count) : base(
        EventEnum.UserDataConsumeRes)
    {
        ResId = resId;
        Count = count;
    }
}