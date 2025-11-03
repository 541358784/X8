using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Storage;

public partial class SROptions
{
    private const string ExChange = "钻石兑换体力";

    [Category(ExChange)]
    [DisplayName("重置")]
    public void RestExChange()
    {
        StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.Remove(UIPopupExchangeEnergyController.ExChangeEnergyKey);
    }
}