public partial class EventEnum
{
    public const string NewDailyPackageExtraRewardEnd = "NewDailyPackageExtraRewardEnd";
}
public class EventNewDailyPackageExtraRewardEnd : BaseEvent
{
    public EventNewDailyPackageExtraRewardEnd() : base(EventEnum.NewDailyPackageExtraRewardEnd) { }
}