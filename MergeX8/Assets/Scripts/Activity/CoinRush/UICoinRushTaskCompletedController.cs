using System.Collections.Generic;

public class UICoinRushTaskCompletedController : UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    
    
    public static void PushCompletedTask(CoinRushTaskConfig taskConfig, int oldValue, int newValue)
    {
        var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(() =>Open(taskConfig, oldValue, newValue)
        , CoinRushModel.TaskCompleteUIList);
        AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup,AutoPopupManager.AutoPopupManager.AutoPopupLogicGroup.TaskFinish);
    }

    public static UICoinRushTaskCompletedController Open(CoinRushTaskConfig taskConfig,int oldValue,int newValue)
    {
        return UIManager.Instance.OpenUI(CoinRushModel.Instance.StorageCoinRush.GetAssetPathWithSkinName(UINameConst.UICoinRushTaskCompleted),taskConfig,oldValue,newValue) as UICoinRushTaskCompletedController;
    }

    protected override async void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var config = (CoinRushTaskConfig) objs[0];
        var oldValue = (int) objs[1];
        var newValue = (int) objs[2];
        var coinRushTaskItem = transform.Find("Root").gameObject.AddComponent<CoinRushTaskItem>();
        coinRushTaskItem.Init(config);
        await coinRushTaskItem.PerformCompletedTask(oldValue,newValue);
        AnimCloseWindow();
    }
}