using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIZumaLeaderBoardMainController:UICommonLeaderBoardMainController
{
    public static UIZumaLeaderBoardMainController Open(StorageCommonLeaderBoard storageLeaderBoard)
    {

        storageLeaderBoard.ForceUpdateLeaderBoardFromServer().WrapErrors();
        var mainWindow = UIManager.Instance.OpenUI(storageLeaderBoard.MainPopupAssetPath,
        storageLeaderBoard,storageLeaderBoard.GetModelInstance()) as UIZumaLeaderBoardMainController;
        return mainWindow;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        // GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaLeaderBoardInfo, null);
    }
}