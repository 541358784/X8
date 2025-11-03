namespace Activity.Base
{
    public interface I_ActivityStatus
    {
        public enum ActivityStatus
        {
            None, //null
            NotParticipated, //未参加
            Incomplete,//未完成
            Completed,//完成
            CompletedEarly, //提前完成
            Count,
        }

        public ActivityStatus GetActivityStatus();
    }
}