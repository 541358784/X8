using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string MixMasterUnlockFormula = "MixMasterUnlockFormula";
    public const string MixMasterUpdateRedPoint = "MixMasterUpdateRedPoint";
    public const string CreateMergeOrder = "CreateMergeOrder";
}

public class EventMixMasterUnlockFormula : BaseEvent
{
    public MixMasterFormulaConfig Formula;
    public EventMixMasterUnlockFormula() : base(EventEnum.MixMasterUnlockFormula) { }

    public EventMixMasterUnlockFormula(MixMasterFormulaConfig formula) : base(EventEnum.MixMasterUnlockFormula)
    {
        Formula = formula;
    }
}
public class EventMixMasterUpdateRedPoint : BaseEvent
{
    public EventMixMasterUpdateRedPoint() : base(EventEnum.MixMasterUpdateRedPoint) { }
}

public class EventCreateMergeOrder : BaseEvent
{
    public StorageTaskItem OrderItem;
    public EventCreateMergeOrder() : base(EventEnum.CreateMergeOrder) { }
    public EventCreateMergeOrder(StorageTaskItem orderItem) : base(EventEnum.CreateMergeOrder)
    {
        OrderItem = orderItem;
    }
}