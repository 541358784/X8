
public partial class EventEnum
{
    public const string KapiTileRebornCountChange = "KapiTileRebornCountChange";
    public const string KapiTileLifeChange = "KapiTileLifeChange";
}
public class EventKapiTileRebornCountChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventKapiTileRebornCountChange() : base(EventEnum.KapiTileRebornCountChange) { }

    public EventKapiTileRebornCountChange(int oldValue,int newValue) : base(EventEnum.KapiTileRebornCountChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
public class EventKapiTileLifeChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventKapiTileLifeChange() : base(EventEnum.KapiTileLifeChange) { }

    public EventKapiTileLifeChange(int oldValue,int newValue) : base(EventEnum.KapiTileLifeChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}