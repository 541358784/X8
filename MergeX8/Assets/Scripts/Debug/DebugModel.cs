/*
 * @file DebugModel
 * Debug
 * @author lu
 */

using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Storage;

public class DebugCfg
{
    public string TitleStr { get; set; }
    public Action<string, string> ClickCallBack { get; set; }
}

public enum DebugTabEnum
{
    Common, // 公共
    News, // 新的
    PiggyBank, // 小猪
    Storage, // 存档
    NotGroup // 未分组
}

public class DebugTabCfg
{
    public string TitleStr { get; set; }
    public List<DebugCfg> Cfg { get; set; }
    public int Idx { get; set; }
}

public sealed partial class DebugModel : Manager<DebugModel>
{
    //private StorageCook cookStorage
    //{
    //    get { return StorageManager.Instance.GetStorage<StorageCook>(); }
    //}

    //public StorageMain _storageMain
    //{
    //    get { return StorageManager.Instance.GetStorage<StorageMain>(); }
    //}

    public bool CloseDebug { get; set; }
    public List<DebugTabCfg> Cfg { get; set; }
    public bool CloseFPS { get; set; } = true;

    public void Init()
    {
        if (Cfg == null)
            Cfg = GetCfg();
    }

    public List<DebugTabCfg> GetCfg()
    {
        var finalCfg = new List<DebugTabCfg>();

        //finalCfg.Add(new DebugTabCfg
        //{
        //    TitleStr = "InGame",
        //    Idx = finalCfg.Count,
        //    Cfg = GetInGameCfg()
        //});

        //finalCfg.Add(new DebugTabCfg
        //{
        //    TitleStr = "公共类",
        //    Idx = finalCfg.Count,
        //    Cfg = GetCommonCfg()
        //});

        //finalCfg.Add(new DebugTabCfg
        //{
        //    TitleStr = "Home",
        //    Idx = finalCfg.Count,
        //    Cfg = GetHomeCfg()
        //});

        //finalCfg.Add(new DebugTabCfg
        //{
        //    TitleStr = "Haptics",
        //    Idx = finalCfg.Count,
        //    Cfg = GetHapticsCfg()
        //});

        return finalCfg;
    }

    public void CloseUI()
    {
        //var debugui = UIManager.Instance.GetOpenedWindow<DebugUiController>();
        //debugui.CloseWindowWithinUIMgr(true);
    }
}