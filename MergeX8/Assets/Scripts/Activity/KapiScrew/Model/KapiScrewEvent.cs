
public partial class EventEnum
{
    public const string KapiScrewRebornCountChange = "KapiScrewRebornCountChange";
    public const string KapiScrewLifeChange = "KapiScrewLifeChange";
}
public class EventKapiScrewRebornCountChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventKapiScrewRebornCountChange() : base(EventEnum.KapiScrewRebornCountChange) { }

    public EventKapiScrewRebornCountChange(int oldValue,int newValue) : base(EventEnum.KapiScrewRebornCountChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKapiScrewLifeChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventKapiScrewLifeChange() : base(EventEnum.KapiScrewLifeChange) { }

    public EventKapiScrewLifeChange(int oldValue,int newValue) : base(EventEnum.KapiScrewLifeChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}