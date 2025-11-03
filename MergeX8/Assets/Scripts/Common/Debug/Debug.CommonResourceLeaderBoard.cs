using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string CommonResourceLeaderBoard = "公共资源排行榜";
    [Category(CommonResourceLeaderBoard)]
    [DisplayName("重置")]
    public void ResetCommonResourceLeaderBoard()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().CommonResourceLeaderBoard.Clear();
    }

    public string CommonResourceLeaderBoardStorageState
    {
        get
        {
            return "";
        }
    }
}