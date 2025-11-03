using System.Collections.Generic;
using DragonU3DSDK.Storage;

public class UIPopupGiftBagProgressTaskCompletedController : UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    public static void PushCompletedTask(StorageGiftBagProgress storage,GiftBagProgressTaskConfig taskConfig, int oldValue, int newValue)
    {
        var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(() =>
        {
            Open(storage,taskConfig, oldValue, newValue);
            return true;
        }, new[] {UINameConst.UIPopupGiftBagProgressTaskCompleted});
        AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup,AutoPopupManager.AutoPopupManager.AutoPopupLogicGroup.TaskFinish);
    }

    public static UIPopupGiftBagProgressTaskCompletedController Open(StorageGiftBagProgress storage,GiftBagProgressTaskConfig taskConfig,int oldValue,int newValue)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupGiftBagProgressTaskCompleted,storage,taskConfig,oldValue,newValue) as UIPopupGiftBagProgressTaskCompletedController;
    }
    protected override async void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var storage = (StorageGiftBagProgress) objs[0];
        var config = (GiftBagProgressTaskConfig) objs[1];
        var oldValue = (int) objs[2];
        var newValue = (int) objs[3];
        var GiftBagProgressTaskItem = transform.Find("Root").gameObject.AddComponent<GiftBagProgressTaskItem>();
        GiftBagProgressTaskItem.Init(config,storage);
        await GiftBagProgressTaskItem.PerformCompletedTask(oldValue,newValue);
        AnimCloseWindow();
    }
}