using System.Collections.Generic;

public class UIPopupKeepPetTaskCompletedController : UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    public static void PushCompletedTask(KeepPetDailyTaskConfig taskConfig, int oldValue, int newValue)
    {
        var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(() =>
        {
            Open(taskConfig, oldValue, newValue);
            return true;
        }, new[] {UINameConst.UIPopupKeepPetTaskCompleted});
        AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup,AutoPopupManager.AutoPopupManager.AutoPopupLogicGroup.TaskFinish);
    }

    public static UIPopupKeepPetTaskCompletedController Open(KeepPetDailyTaskConfig taskConfig,int oldValue,int newValue)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetTaskCompleted,taskConfig,oldValue,newValue) as UIPopupKeepPetTaskCompletedController;
    }
    protected override async void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var config = (KeepPetDailyTaskConfig) objs[0];
        var oldValue = (int) objs[1];
        var newValue = (int) objs[2];
        var KeepPetTaskItem = transform.Find("Root").gameObject.AddComponent<KeepPetTaskItem>();
        KeepPetTaskItem.Init(config);
        await KeepPetTaskItem.PerformCompletedTask(oldValue,newValue);
        AnimCloseWindow();
    }
}