using System.Collections.Generic;

public class UIPopupMixMasterTaskCompletedController : UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    public static void PushCompletedTask(MixMasterMixTaskConfig taskConfig, int oldValue, int newValue)
    {
        var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(() =>
        {
            Open(taskConfig, oldValue, newValue);
            return true;
        }, new[] {UINameConst.UIPopupMixMasterTaskCompleted});
        AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup,AutoPopupManager.AutoPopupManager.AutoPopupLogicGroup.TaskFinish);
    }

    public static UIPopupMixMasterTaskCompletedController Open(MixMasterMixTaskConfig taskConfig,int oldValue,int newValue)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMixMasterTaskCompleted,taskConfig,oldValue,newValue) as UIPopupMixMasterTaskCompletedController;
    }
    protected override async void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var config = (MixMasterMixTaskConfig) objs[0];
        var oldValue = (int) objs[1];
        var newValue = (int) objs[2];
        var MixMasterTaskItem = transform.Find("Root").gameObject.AddComponent<MixMasterTaskItem>();
        MixMasterTaskItem.Init(config);
        await MixMasterTaskItem.PerformCompletedTask(oldValue,newValue);
        AnimCloseWindow();
    }
}