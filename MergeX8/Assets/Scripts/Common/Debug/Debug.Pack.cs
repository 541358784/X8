using System.ComponentModel;
using DragonU3DSDK.Storage;
using OptionalGift;
using UnityEngine;


public partial class SROptions
{
    [Category(Pack)]
    [DisplayName("显示破冰礼包 ")]
    public void ShowIceBreakPack()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPack, "debug");
    }

    [Category(Pack)]
    [DisplayName("显示破冰礼包 Low")]
    public void ShowIceBreakPack_Low()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPackLow, "debug");
    }


    [Category(Pack)]
    [DisplayName("清破冰礼包 ")]
    public void ClearIceBreakPack()
    {
        IcebreakingPackModel.Instance.ClearPack();
    }

    [Category(Pack)]
    [DisplayName("显示每日礼包 ")]
    public void ShowMaterialPack()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIDailyPack2, 9);
    }
    private int dailyPackId = 0;
    [Sort(100)]
    [Category(Pack)]
    [DisplayName("dailyPackId")]
    public int DailyPackID
    {
        get { return dailyPackId ;}
        set { dailyPackId = value; }
    }
    [Category(Pack)]
    [DisplayName("指定每日礼包 ")]
    public void RefreshDailyPack()
    {
        HideDebugPanel();
        DailyPackModel.Instance.DebugRefresh(DailyPackID);
    }
    [Category(Pack)]
    [DisplayName("每日刷新每日礼包")]
    public void DailyRefreshDailyPack()
    {
        HideDebugPanel();
        DailyPackModel.Instance.packData.StartTime = -1;
        DailyPackModel.Instance.RefreshPack();
    }    [Category(Pack)]
    [DisplayName("清自选礼包 ")]
    public void CleanOptional()
    {
        HideDebugPanel();
        OptionGiftModel.Instance.StorageOptionalGift.Clear();
    }
    

    [Category(Pack)]
    [DisplayName("清每日礼包 ")]
    public void ClearMaterialPack()
    {
        DailyPackModel.Instance.ClearPack();
    }

    #region NewDailyPack

    [Category(Pack)]
    [DisplayName("显示新每日礼包 ")]
    public void ShowNewDailyPack()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupNewDailyGift, "pack");
    }
    private int newDailyPackId = 0;
    [Sort(100)]
    [Category(Pack)]
    [DisplayName("newDailyPackId")]
    public int NewDailyPackID
    {
        get { return newDailyPackId ;}
        set { newDailyPackId = value; }
    }
    [Category(Pack)]
    [DisplayName("指定新每日礼包 ")]
    public void RefreshNewDailyPack()
    {
        HideDebugPanel();
        NewDailyPackModel.Instance.DebugRefresh(NewDailyPackID);
    }
    
    [Category(Pack)]
    [DisplayName("每日刷新新每日礼包")]
    public void DailyRefreshNewDailyPack()
    {
        HideDebugPanel();
        NewDailyPackModel.Instance.packData.StartTime = -1;
        NewDailyPackModel.Instance.RefreshPack();
    }
    

    [Category(Pack)]
    [DisplayName("清新每日礼包 ")]
    public void ClearNewDailyPack()
    {
        NewDailyPackModel.Instance.ClearPack();
    }

    #endregion


    [Category(Pack)]
    [DisplayName("生成所有闪购任务礼包  ")]
    public void GenAllTaskAssist()
    {
        HideDebugPanel();
        TaskAssistPackModel.Instance.DebugGenTaskAssistPack();
        
    }
    
    [Category(Pack)]
    [DisplayName("海豹礼包重置 ")]
    public void ClearSealPack()
    {
        SealPackModel.Instance.storageSealPack.IsFinish = false;
        SealPackModel.Instance.storageSealPack.PopTimes = 0;
        SealPackModel.Instance.storageSealPack.LastPopUpTime = 0;
        SealPackModel.Instance.storageSealPack.FinishTime = 0;
        
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, UISealPackController.constPlaceId, 0);
    }
    
    [Category(Pack)]
    [DisplayName("显示海豹礼包")]
    public void ShowSealPack()
    {
        UIManager.Instance.OpenUI(UINameConst.UISealPack);
    }
    
    [Category(Pack)]
    [DisplayName("三连礼包重置")]
    public void ClearThreeGift()
    {
        StorageManager.Instance.GetStorage<StorageHome>().ThreeGift.Clear();
    }
}