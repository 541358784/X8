using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string EventCompleteTask = "EventCompleteTask";
}
public class EventCompleteTask : BaseEvent
{
    public StorageTaskItem TaskItem;
    public EventCompleteTask() : base(EventEnum.EventCompleteTask) { }

    public EventCompleteTask(StorageTaskItem taskItem) : base(EventEnum.EventCompleteTask)
    {
        TaskItem = taskItem;
    }
}