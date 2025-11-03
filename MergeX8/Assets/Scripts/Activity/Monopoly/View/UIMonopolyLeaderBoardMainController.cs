using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIMonopolyLeaderBoardMainController:UICommonLeaderBoardMainController
{
    public static UIMonopolyLeaderBoardMainController Open(StorageCommonLeaderBoard storageLeaderBoard)
    {
        var mainWindow = UIManager.Instance.OpenUI(storageLeaderBoard.MainPopupAssetPath,
        storageLeaderBoard,storageLeaderBoard.GetModelInstance()) as UIMonopolyLeaderBoardMainController;
        return mainWindow;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyLeaderBoardInfo, null);
    }
}