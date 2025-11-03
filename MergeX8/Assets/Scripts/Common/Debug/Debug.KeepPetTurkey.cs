using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string KeepPetTurkey = "狗火鸡";
    [Category(KeepPetTurkey)]
    [DisplayName("重制")]
    public void ResetKeepPetTurkey()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().KeepPetTurkey.Clear();
        if (KeepPetTurkeyModel.Instance.IsInitFromServer())
            KeepPetTurkeyModel.Instance.InitStorage();
        var guideIdList = new List<int>() {};
        CleanGuideList(guideIdList);
    }

    [Category(KeepPetTurkey)]
    [DisplayName("积分")]
    public int KeepPetTurkeyBallCount
    {
        get
        {
            return KeepPetTurkeyModel.Instance.GetScore();
        }
        set
        {
            if (KeepPetTurkeyModel.Instance.IsInitFromServer())
            {
                KeepPetTurkeyModel.Instance.Storage.AddScore(value - KeepPetTurkeyModel.Instance.GetScore(),"Debug");   
            }
        }
    }
}