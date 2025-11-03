using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string KapiTile = "卡皮Tile";
    [Category(KapiTile)]
    [DisplayName("重制")]
    public void ResetKapiTile()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().KapiTile.Clear();
        if (KapiTileModel.Instance.IsInitFromServer())
            KapiTileModel.Instance.InitStorage();
        var guideIdList = new List<int>() {4447};
        CleanGuideList(guideIdList);
    }

    [Category(KapiTile)]
    [DisplayName("复活道具")]
    public int KapiTileRebornCount
    {
        get
        {
            return KapiTileModel.Instance.GetRebornCount();
        }
        set
        {
            if (KapiTileModel.Instance.IsInitFromServer())
            {
                KapiTileModel.Instance.AddRebornItem(value - KapiTileModel.Instance.GetRebornCount(),"Debug");
            }
        }
    }
    
    [Category(KapiTile)]
    [DisplayName("生命值")]
    public int KapiTileLifeCount
    {
        get
        {
            return KapiTileModel.Instance.GetLife();
        }
        set
        {
            if (KapiTileModel.Instance.IsInitFromServer())
            {
                KapiTileModel.Instance.AddLife(value - KapiTileModel.Instance.GetLife(),"Debug");
            }
        }
    }
    
    [Category(KapiTile)]
    [DisplayName("大关卡(0开始)")]
    public int KapiTileBigLevel
    {
        get
        {
            return KapiTileModel.Instance.Storage.BigLevel;
        }
        set
        {
            KapiTileModel.Instance.Storage.BigLevel = value;
        }
    }
 
    [Category(KapiTile)]
    [DisplayName("小关卡(0开始)")]
    public int KapiTileSmallLevel
    {
        get
        {
            return KapiTileModel.Instance.Storage.SmallLevel;
        }
        set
        {
            KapiTileModel.Instance.Storage.SmallLevel = value;
        }
    }
    [Category(KapiTile)]
    [DisplayName("过关")]
    public void KapiTileWin()
    {
        KapiTileModel.Instance.DealStartGame();
        KapiTileModel.Instance.DealWin();
        UIKapiTileMainController.Show(true);
    }
    [Category(KapiTile)]
    [DisplayName("失败")]
    public void KapiTileFail()
    {
        KapiTileModel.Instance.DealStartGame();
        KapiTileModel.Instance.DealFail();
        UIKapiTileMainController.Show(false);
    }
}