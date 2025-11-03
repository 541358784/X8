using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string KapiScrew = "卡皮钉子";
    [Category(KapiScrew)]
    [DisplayName("重制")]
    public void ResetKapiScrew()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().KapiScrew.Clear();
        if (KapiScrewModel.Instance.IsInitFromServer())
            KapiScrewModel.Instance.InitStorage();
        var guideIdList = new List<int>() {4415,4416};
        CleanGuideList(guideIdList);
    }

    [Category(KapiScrew)]
    [DisplayName("复活道具")]
    public int KapiScrewRebornCount
    {
        get
        {
            return KapiScrewModel.Instance.GetRebornCount();
        }
        set
        {
            if (KapiScrewModel.Instance.IsInitFromServer())
            {
                KapiScrewModel.Instance.AddRebornItem(value - KapiScrewModel.Instance.GetRebornCount(),"Debug");
            }
        }
    }
    
    [Category(KapiScrew)]
    [DisplayName("生命值")]
    public int KapiScrewLifeCount
    {
        get
        {
            return KapiScrewModel.Instance.GetLife();
        }
        set
        {
            if (KapiScrewModel.Instance.IsInitFromServer())
            {
                KapiScrewModel.Instance.AddLife(value - KapiScrewModel.Instance.GetLife(),"Debug");
            }
        }
    }
    
    [Category(KapiScrew)]
    [DisplayName("大关卡(0开始)")]
    public int KapiScrewBigLevel
    {
        get
        {
            return KapiScrewModel.Instance.Storage.BigLevel;
        }
        set
        {
            KapiScrewModel.Instance.Storage.BigLevel = value;
        }
    }
 
    [Category(KapiScrew)]
    [DisplayName("小关卡(0开始)")]
    public int KapiScrewSmallLevel
    {
        get
        {
            return KapiScrewModel.Instance.Storage.SmallLevel;
        }
        set
        {
            KapiScrewModel.Instance.Storage.SmallLevel = value;
        }
    }
    [Category(KapiScrew)]
    [DisplayName("过关")]
    public void KapiScrewWin()
    {
        KapiScrewModel.Instance.DealStartGame();
        KapiScrewModel.Instance.DealWin();
        UIKapiScrewMainController.Instance?.PerformOnGameWin();
    }
    [Category(KapiScrew)]
    [DisplayName("失败")]
    public void KapiScrewFail()
    {
        KapiScrewModel.Instance.DealStartGame();
        KapiScrewModel.Instance.DealFail();
        UIKapiScrewMainController.Instance?.PerformOnGameFail();
    }
}