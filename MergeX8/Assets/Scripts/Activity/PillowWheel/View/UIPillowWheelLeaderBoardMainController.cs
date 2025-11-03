using DragonU3DSDK.Storage;

public class UIPillowWheelLeaderBoardMainController:UICommonLeaderBoardMainController
{
    public static UIPillowWheelLeaderBoardMainController Open(StorageCommonLeaderBoard storageLeaderBoard)
    {

        storageLeaderBoard.ForceUpdateLeaderBoardFromServer().WrapErrors();
        var mainWindow = UIManager.Instance.OpenUI(storageLeaderBoard.MainPopupAssetPath,
        storageLeaderBoard,storageLeaderBoard.GetModelInstance()) as UIPillowWheelLeaderBoardMainController;
        return mainWindow;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.PillowWheelLeaderBoardDescribe, null);
    }
}