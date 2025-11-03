using System.ComponentModel;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Gameplay;

/// <summary>
/// 临时debug相关， 单元测试相关
/// </summary>
public partial class SROptions
{
    private const string AdLocalConfig = "0用户本地分层";

    // [Category(AdLocalConfig)]
    // [DisplayName("强制使用本地分组")]
    // public bool AdLocalDebug
    // {
    //     get => AdLocalConfigHandle.Instance.IsDebug;
    //     set => AdLocalConfigHandle.Instance.IsDebug = value;
    // }
    
    [Category(AdLocalConfig)]
    [DisplayName("强制使用本地分组PayLevel测试")]
    public bool AdLocalPayLevelDebug
    {
        get => AdLocalConfigHandle.Instance.IsDebugPayLevel;
        set => AdLocalConfigHandle.Instance.IsDebugPayLevel = value;
    }
    
    // [Category(AdLocalConfig)]
    // [DisplayName("强制产出气泡")]
    // public bool AdLocalDebugBubble
    // {
    //     get => AdLocalConfigHandle.Instance.IsDebugBubble;
    //     set => AdLocalConfigHandle.Instance.IsDebugBubble = value;
    // }
    
  
     private float _payNum;

    [Category(AdLocalConfig)]
    [DisplayName("付费金额")]
    public float PayNum
    {
        get => _payNum;
        set => _payNum = value;
    }


    [Category(AdLocalConfig)]
    [DisplayName("设置单天付费金额")]
    public void EnergyUserGroup_1()
    {
        AdLocalConfigHandle.Instance.SetOneDayPayNumDebug(_payNum);
    }
    

    [Category(AdLocalConfig)]
    [DisplayName("重置当前付费金额")]
    public void EnergyUserGroup_2()
    {
        AdLocalConfigHandle.Instance.Storage.CurDayPay = 0;
    }

    [Category(AdLocalConfig)]
    [DisplayName("清空付费数据")]
    public void EnergyUserGroup_3()
    {
        AdLocalConfigHandle.Instance.Storage.LastPayData.Clear();
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("清空Rv数据")]
    public void EnergyUserGroup_4()
    {
        AdLocalConfigHandle.Instance.Storage.LastPlayRvNum.Clear();
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("清空In数据")]
    public void EnergyUserGroup_5()
    {
        AdLocalConfigHandle.Instance.Storage.LastPlayInNum.Clear();
    }
    

    [Category(AdLocalConfig)]
    [DisplayName("用户本地分组分组:  ")]
    public string EnergyUserGroup_6
    {
        get { return AdLocalConfigHandle.Instance.Storage.CurGroup.ToString(); }
    }
    
    
    private int addProgress;

    [Category(AdLocalConfig)]
    [DisplayName("需要增加的进度数量")]
    public int AddProgress
    {
        get => addProgress;
        set => addProgress = value;
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("增加活跃天")]
    public void EnergyUserGroup_7()
    {
        AdLocalConfigHandle.Instance.AddActiveDayDebug(addProgress);
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("增加完成任务数量")]
    public void EnergyUserGroup_8()
    {
        AdLocalConfigHandle.Instance.RefreshJudgingData(AdLocalDataType.CompletedOrder,add:AddProgress);
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("增加关闭商店数量")]
    public void EnergyUserGroup_9()
    {
        AdLocalConfigHandle.Instance.RefreshJudgingData(AdLocalDataType.CloseShop,add:AddProgress);
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("增加连续跳过Rv数量")]
    public void EnergyUserGroup_10()
    {
        AdLocalConfigHandle.Instance.RefreshJudgingData(AdLocalDataType.SkipRv,add:AddProgress);
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("中断连续Rv")]
    public void EnergyUserGroup_11()
    {
        AdLocalConfigHandle.Instance.RefreshJudgingData(AdLocalDataType.SkipRv,false);
    }
    [Category(AdLocalConfig)]
    [DisplayName("重置数据并初始化分组")]
    public void EnergyUserGroup_12()
    {
        AdLocalConfigHandle.Instance.Storage.Clear();
        AdLocalConfigHandle.Instance.TryInitLastLoginTime();
        
        AdLocalConfigHandle.Instance.InitGroup();
    }
    
    
    private int _adLocalGroup;

    [Category(AdLocalConfig)]
    [DisplayName("分组Id")]
    public int AdConfigLocalGroup
    {
        get => _adLocalGroup;
        set => _adLocalGroup = value;
    }
    [Category(AdLocalConfig)]
    [DisplayName("设置分组")]
    public void EnergyUserGroup_13()
    {
        AdLocalConfigHandle.Instance.SetGroupDebug(AdConfigLocalGroup);
    }
    
    [Category(AdLocalConfig)]
    [DisplayName("清空当天广告播放相关数据")]
    public void EnergyUserGroup_14()
    {
        StorageManager.Instance.GetStorage<StorageHome>().AdData.InterstitalPlayRecords.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().AdData.AdPlayRecords.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().AdData.PlayActiveInterTodayNum=0;
        StorageManager.Instance.GetStorage<StorageHome>().AdData.PlayPassiveInterTodayNum=0;
        StorageManager.Instance.GetStorage<StorageHome>().AdData.PlayPassiveInterTodayTime=0;
        StorageManager.Instance.GetStorage<StorageHome>().AdData.PlayActiveInterTodayTime=0;
        StorageManager.Instance.GetStorage<StorageHome>().AdData.PlayRvTodayTime=0;
        AdLocalConfigHandle.Instance.ClearCurDayDataDebug();
    }
    [Category(AdLocalConfig)]
    [DisplayName("清空活跃天相关数据(包含当天)")]
    public void EnergyUserGroup_15()
    {
        AdLocalConfigHandle.Instance.ClearDataDebug();
    }
    
    
    [Category(AdLocalConfig)]
    [DisplayName("增加体力消耗数量")]
    public void EnergyUserGroup_16()
    {
        AdLocalConfigHandle.Instance.RefreshJudgingData(AdLocalDataType.CostEnergy,add:AddProgress);
    }
    
}