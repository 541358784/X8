using System.Collections.Generic;
using System.ComponentModel;
using Activity.FarmTimeOrder;
using DragonU3DSDK.Account;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Farm.Model;
using Farm.Order;
using Gameplay;
using Gameplay.UI.BindEmail;

public partial class SROptions
{
    private const string Email = "绑定邮箱";

    [Category(Email)]
    [DisplayName("清理绑定邮箱")]
    public void RestEmail()
    {
        StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Clear();
        StorageManager.Instance.GetStorage<StorageCommon>().Email = "";

        AccountManager.Instance.LogoutWithEmailAccount();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, UIPopupBindEmailController.constPlaceId);
    }
    
    [Category(Email)]
    [DisplayName("绑定邮箱 阶段")]
    public int Stage
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.Stage;
        }
    }
    
    [Category(Email)]
    [DisplayName("是否领取所有奖励")]
    public bool EmailGetReward
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.IsGetReward;
        }
    }
}