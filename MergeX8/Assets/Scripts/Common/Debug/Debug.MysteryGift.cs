using System.ComponentModel;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(MysteryGift)]
    [DisplayName("显示神秘礼物")]
    public void ShowMysteryGift()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupMysteryGift, false);
    }
    [Category(MysteryGift)]
    [DisplayName("显示神秘礼物充值")]
    public void ShowMysteryGiftPay()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupMysteryGift, true);
    }
    
    [Category(MysteryGift)]
    [DisplayName("神秘礼物次数限制")]
    public int MysteryGiftShowLimitCount
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageGame>().MysteryGiftShowLimitCount;
        }
        set
        {
            StorageManager.Instance.GetStorage<StorageGame>().MysteryGiftShowLimitCount = value;
        }
    }

    
    [Category(MysteryGift)]
    [DisplayName("当前完成任务次数")]
    public int MysteryGiftCompTaskCount
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageGame>().MysteryGiftCompTaskCount;
        }
        set
        {
            StorageManager.Instance.GetStorage<StorageGame>().MysteryGiftCompTaskCount = value;
        }
    }
    
    [Category(MysteryGift)]
    [DisplayName("展示神秘礼物时间")]
    public int MysteryGiftShowTime
    {
        get
        {
            return UIPopupMysteryGiftController.MysteryGiftShowTime;
        }
        set
        {
            UIPopupMysteryGiftController.MysteryGiftShowTime = value;
        }
    }
}