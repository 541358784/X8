using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string Kapibala = "0卡皮巴拉";
    [Category(Kapibala)]
    [DisplayName("重制")]
    public void ResetKapibala()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().Kapibala.Clear();
        if (KapibalaModel.Instance.IsInitFromServer())
            KapibalaModel.Instance.InitStorage();
        var guideIdList = new List<int>() {4414};
        CleanGuideList(guideIdList);
    }

    [Category(Kapibala)]
    [DisplayName("复活道具")]
    public int KapibalaRebornCount
    {
        get
        {
            return KapibalaModel.Instance.GetRebornCount();
        }
        set
        {
            if (KapibalaModel.Instance.IsInitFromServer())
            {
                KapibalaModel.Instance.AddRebornItem(value - KapibalaModel.Instance.GetRebornCount(),"Debug");
            }
        }
    }
    
    [Category(Kapibala)]
    [DisplayName("生命值")]
    public int KapibalaLifeCount
    {
        get
        {
            return KapibalaModel.Instance.GetLife();
        }
        set
        {
            if (KapibalaModel.Instance.IsInitFromServer())
            {
                KapibalaModel.Instance.AddLife(value - KapibalaModel.Instance.GetLife(),"Debug");
            }
        }
    }
    
    [Category(Kapibala)]
    [DisplayName("大关卡(0开始)")]
    public int KapibalaBigLevel
    {
        get
        {
            return KapibalaModel.Instance.Storage.BigLevel;
        }
        set
        {
            KapibalaModel.Instance.Storage.BigLevel = value;
        }
    }
 
    [Category(Kapibala)]
    [DisplayName("小关卡(0开始)")]
    public int KapibalaSmallLevel
    {
        get
        {
            return KapibalaModel.Instance.Storage.SmallLevel;
        }
        set
        {
            KapibalaModel.Instance.Storage.SmallLevel = value;
        }
    }
    [Category(Kapibala)]
    [DisplayName("过关")]
    public void KapibalaWin()
    {
        KapibalaModel.Instance.DealStartGame();
        KapibalaModel.Instance.DealWin();
        UIKapibalaMainController.Show(true);
    }
    [Category(Kapibala)]
    [DisplayName("失败")]
    public void KapibalaFail()
    {
        KapibalaModel.Instance.DealStartGame();
        KapibalaModel.Instance.DealFail();
        UIKapibalaMainController.Show(false);
    }
}