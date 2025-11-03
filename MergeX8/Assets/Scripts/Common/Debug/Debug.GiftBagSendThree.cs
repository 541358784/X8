using System.ComponentModel;
using DragonU3DSDK.Storage;

public partial class SROptions
{
    private const string GiftBagSendN = "0买一赠n礼包";
    
    
    
    [Category(GiftBagSendN)]
    [DisplayName("重置1")]
    public void ResetGiftBagSendOne()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagSendOne.Clear();
        GiftBagSendOneModel.Instance.InitStorage();
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("当前分组1")]
    public int GiftBagSendOneCurGroup
    {
        get
        {
            return GiftBagSendOneModel.Instance.StorageGiftBagSendOne.PayLevelGroup;
        }
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("重置2")]
    public void ResetGiftBagSendTwo()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagSendTwo.Clear();
        GiftBagSendTwoModel.Instance.InitStorage();
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("当前分组2")]
    public int GiftBagSendTwoCurGroup
    {
        get
        {
            return GiftBagSendTwoModel.Instance.StorageGiftBagSendTwo.PayLevelGroup;
        }
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("重置3")]
    public void ResetGiftBagSendThree()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagSendThree.Clear();
        GiftBagSendThreeModel.Instance.InitStorage();
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("当前分组3")]
    public int GiftBagSendThreeCurGroup
    {
        get
        {
            return GiftBagSendThreeModel.Instance.StorageGiftBagSendThree.PayLevelGroup;
        }
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("重置4")]
    public void ResetGiftBagSend4()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagSend4.Clear();
        GiftBagSend4Model.Instance.InitStorage();
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("当前分组4")]
    public int GiftBagSend4CurGroup
    {
        get
        {
            return GiftBagSend4Model.Instance.StorageGiftBagSend4.PayLevelGroup;
        }
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("重置6")]
    public void ResetGiftBagSend6()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().GiftBagSend6.Clear();
        GiftBagSend6Model.Instance.InitStorage();
    }
    
    [Category(GiftBagSendN)]
    [DisplayName("当前分组6")]
    public int GiftBagSend6CurGroup
    {
        get
        {
            return GiftBagSend6Model.Instance.StorageGiftBagSend6.PayLevelGroup;
        }
    }
}