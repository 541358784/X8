using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UICommonResourceLeaderBoardMainController:UICommonLeaderBoardMainController
{
    public static UICommonResourceLeaderBoardMainController Open(StorageCommonLeaderBoard storageLeaderBoard)
    {
        UIManager.Instance._WindowMetaPublic(storageLeaderBoard.MainPopupAssetPath, UIWindowLayer.Normal,false);
        var mainWindow = UIManager.Instance.OpenUI(storageLeaderBoard.MainPopupAssetPath,
        storageLeaderBoard,storageLeaderBoard.GetModelInstance()) as UICommonResourceLeaderBoardMainController;
        return mainWindow;
    }
}