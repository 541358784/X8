using System.ComponentModel;

public partial class SROptions
{
    private const string TrainOrder = "1搬家订单";
    
    
    [Category(TrainOrder)]
    [DisplayName("重置")]
    public void TrainOrder1()
    {
        MergeManager.Instance.ClearMerBoard(MergeBoardEnum.TrainOrder);
        Activity.TrainOrder.TrainOrderModel.Instance.DebugReset();
    }
    
    [Category(TrainOrder)]
    [DisplayName("开关debug模式")]
    public bool TrainOrderDebugModule
    {
        get
        {
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
                return false;
            
            return Activity.TrainOrder.TrainOrderModel.Instance.DebugComplete;
        }
        set
        {
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
                return;

            Activity.TrainOrder.TrainOrderModel.Instance.DebugComplete = value;
        }
    }
    
}