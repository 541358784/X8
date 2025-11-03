public partial class EventEnum
{
    public const string ExitFsmState = "ExitFsmState";
    public const string EnterFsmState = "EnterFsmState";
}
public class EventExitFsmState : BaseEvent
{
    public IFsmState State;
    public EventExitFsmState() : base(EventEnum.ExitFsmState) { }

    public EventExitFsmState(IFsmState state) : base(EventEnum.ExitFsmState)
    {
        State = state;
    }
}
public class EventEnterFsmState : BaseEvent
{
    public IFsmState State;
    public EventEnterFsmState() : base(EventEnum.EnterFsmState) { }

    public EventEnterFsmState(IFsmState state) : base(EventEnum.EnterFsmState)
    {
        State = state;
    }
}