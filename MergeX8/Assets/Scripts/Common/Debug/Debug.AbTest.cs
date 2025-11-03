using System.ComponentModel;
using Difference;
using DragonU3DSDK.Storage;
using Merge.Order;

public partial class SROptions
{
    private const string AbTest = "ABTEST";

    [Category(AbTest)]
    [DisplayName("设置引导-A组")]
    public void SetATest()
    {
        StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[DifferenceManager.OpenDiffKey] = "0";
        DifferenceManager.Instance.IsOpenDifference = false;
        MergeManager.Instance.AdaptBoard();
    }

    [Category(AbTest)]
    [DisplayName("是否锁定地图")]
    public bool IsLockMap
    {
        get { return ABTest.ABTestManager.Instance.IsLockMap(); }
    }

    [Category(AbTest)]
    [DisplayName("生成任务类型")]
    public string OrderType
    {
        get { return ABTest.ABTestManager.Instance.GetCreateOrderType() == CreateOrderType.Level ? "等级" : "难度"; }
    }
}