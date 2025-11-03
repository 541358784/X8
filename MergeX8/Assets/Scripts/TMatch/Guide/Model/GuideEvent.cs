
namespace OutsideGuide
{
    public class GuideEvent : TMatch.BaseEvent
    {
        public string GuideEventType;
        public int ParamId;
        public System.Action EndAction;
        public GuideEvent(string eventType, int id = 0) : base(TMatch.EventEnum.GUIDE_EVENT)
        {
            GuideEventType = eventType;
            ParamId = id;
        }
    }
}