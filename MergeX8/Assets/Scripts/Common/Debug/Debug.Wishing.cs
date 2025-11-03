using System.ComponentModel;
using Gameplay.UI.Store.Vip.Model;

public partial class SROptions
{
    private const string Wishing = "1 许愿池";
    
    [Category(Wishing)]
    [DisplayName("重置许愿池CD")]
    public void RestWishingCD()
    {
       CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, UIPopupTaskController.CoolTimeKey);
    }
}
