
public partial class EventEnum
{
    public const string KapibalaRebornCountChange = "KapibalaRebornCountChange";
    public const string KapibalaLifeChange = "KapibalaLifeChange";
}
public class EventKapibalaRebornCountChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventKapibalaRebornCountChange() : base(EventEnum.KapibalaRebornCountChange) { }

    public EventKapibalaRebornCountChange(int oldValue,int newValue) : base(EventEnum.KapibalaRebornCountChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKapibalaLifeChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventKapibalaLifeChange() : base(EventEnum.KapibalaLifeChange) { }

    public EventKapibalaLifeChange(int oldValue,int newValue) : base(EventEnum.KapibalaLifeChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}