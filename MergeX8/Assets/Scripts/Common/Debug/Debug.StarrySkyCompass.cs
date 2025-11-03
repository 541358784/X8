using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string StarrySkyCompass = "星空罗盘";
    [Category(StarrySkyCompass)]
    [DisplayName("重制")]
    public void ResetStarrySkyCompass()
    {
        HideDebugPanel();
        var storage = StorageManager.Instance.GetStorage<StorageHome>().StarrySkyCompass;
        StorageManager.Instance.GetStorage<StorageHome>().StarrySkyCompass.Clear();
        var guideIdList = new List<int>(){4321,4322,4323,4324,4325,4326};
        CleanGuideList(guideIdList);
        if (StarrySkyCompassModel.Instance.IsInitFromServer())
            return;
        StarrySkyCompassModel.Instance.InitStorage();
    }

    [Category(StarrySkyCompass)]
    [DisplayName("潜水艇数")]
    public int StarrySkyCompassPackageCount
    {
        get
        {
            return StarrySkyCompassModel.Instance.GetRocketCount();
        }
        set
        {
            StarrySkyCompassModel.Instance.AddRocket(value-StarrySkyCompassModel.Instance.GetRocketCount(),"Debug");
        }
    }

    [Category(StarrySkyCompass)]
    [DisplayName("分数")]
    public int StarrySkyCompassScore
    {
        get
        {
            return StarrySkyCompassModel.Instance.GetScore();
        }
        set
        {
            StarrySkyCompassModel.Instance.AddScore(value - StarrySkyCompassModel.Instance.GetScore(),"Debug");
        }
    }
    
    [Category(StarrySkyCompass)]
    [DisplayName("开心时刻")]
    public int StarrySkyCompassHappyTime
    {
        get
        {
            return (int)((StarrySkyCompassModel.Instance.Storage.HappyEndTime -
                          (long)APIManager.Instance.GetServerTime()) / (long)XUtility.Second);
        }
        set
        {
            StarrySkyCompassModel.Instance.Storage.SetHappyTime(value);
        }
    }
}