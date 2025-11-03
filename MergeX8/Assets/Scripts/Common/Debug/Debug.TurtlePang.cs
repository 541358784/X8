using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string TurtlePang = "乌龟对对碰";
    [Category(TurtlePang)]
    [DisplayName("重制")]
    public void ResetTurtlePang()
    {
        HideDebugPanel();
        var storage = StorageManager.Instance.GetStorage<StorageHome>().TurtlePang;
        StorageManager.Instance.GetStorage<StorageHome>().TurtlePang.Clear();
        var guideIdList = new List<int>(){4327,4328,4329,4330};
        CleanGuideList(guideIdList);
        // if (TurtlePangModel.Instance.IsInitFromServer())
        //     return;
        TurtlePangModel.Instance.InitStorage();
    }

    [Category(TurtlePang)]
    [DisplayName("包数")]
    public int TurtlePangPackageCount
    {
        get
        {
            return TurtlePangModel.Instance.Storage.PackageCount;
        }
        set
        {
            TurtlePangModel.Instance.Storage.PackageCount = value;
        }
    }

    [Category(TurtlePang)]
    [DisplayName("分数")]
    public int TurtlePangScore
    {
        get
        {
            return TurtlePangModel.Instance.Storage.Score;
        }
        set
        {
            TurtlePangModel.Instance.Storage.Score = value;
        }
    }

    private int _fakePlayTimes = 1000;
    [Category(TurtlePang)]
    [DisplayName("模拟次数")]
    public int FakePlayTimes
    {
        get
        {
            return _fakePlayTimes;
        }
        set
        {
            _fakePlayTimes = value;
        }
    }
    [Category(TurtlePang)]
    [DisplayName("模拟选包配置1")]
    public void FakePlayConfig1()
    {
        TurtlePangModel.Instance.FakePlay(_fakePlayTimes,TurtlePangModel.Instance.GlobalConfig.PackageType1);
    }
    [Category(TurtlePang)]
    [DisplayName("模拟选包配置2")]
    public void FakePlayConfig2()
    {
        TurtlePangModel.Instance.FakePlay(_fakePlayTimes,TurtlePangModel.Instance.GlobalConfig.PackageType2);
    }

    [Category(TurtlePang)]
    [DisplayName("清场牌面")]
    public bool TurtlePangDebugClear
    {
        get
        {
            return TurtlePangModel.Instance.TurtlePangDebugClear;
        }
        set
        {
            TurtlePangModel.Instance.TurtlePangDebugClear = value;
        }
    }
    [Category(TurtlePang)]
    [DisplayName("全家福牌面")]
    public bool TurtlePangDebugFull
    {
        get
        {
            return TurtlePangModel.Instance.TurtlePangDebugFull;
        }
        set
        {
            TurtlePangModel.Instance.TurtlePangDebugFull = value;
        }
    }
}