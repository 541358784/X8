using DragonU3DSDK.Storage;

public class UIParrotLeaderBoardMainController:UICommonLeaderBoardMainController
{
    public static UIParrotLeaderBoardMainController Open(StorageCommonLeaderBoard storageLeaderBoard)
    {

        storageLeaderBoard.ForceUpdateLeaderBoardFromServer().WrapErrors();
        var mainWindow = UIManager.Instance.OpenUI(storageLeaderBoard.MainPopupAssetPath,
        storageLeaderBoard,storageLeaderBoard.GetModelInstance()) as UIParrotLeaderBoardMainController;
        return mainWindow;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ParrotLeaderBoardDescribe, null);
    }
}