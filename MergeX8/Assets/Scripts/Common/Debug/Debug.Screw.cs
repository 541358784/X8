using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Screw;
using Screw.GameLogic;
using Screw.UserData;
using UnityEngine;
using UserData = Screw.UserData.UserData;

public partial class SROptions
{
    private const string Screw = "钉子";

    [Category(Screw)]
    [DisplayName("每日礼包日期")]
    public int ScrewDailyPackageDayId
    {
        get
        {
            return DailyPackageModel.Instance.Storage.DayId;
        }
        set
        {
            DailyPackageModel.Instance.Storage.DayId = value;
        }
    }
    [Category(Screw)]
    [DisplayName("无限体力时间")]
    public int ScrewInfinityLifeTime
    {
        get
        {
            return (int)(EnergyData.Instance.GetEnergyInfinityLeftTime()/(long)XUtility.Second);
        }
        set
        {
            EnergyData.Instance.AddInfinityLeftTime(value-(int)(EnergyData.Instance.GetEnergyInfinityLeftTime()/(long)XUtility.Second));
        }
    }
    
    [Category(Screw)]
    [DisplayName("金币")]
    public int ScrewCoinCount
    {
        get
        {
            return UserData.Instance.GetRes(ResType.Coin);
        }
        set
        {
            if (value > 0)
            {
                UserData.Instance.AddRes(ResType.Coin, value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }
            else
            {
                UserData.Instance.ConsumeRes(ResType.Coin, -value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }
        }
    }
    
    [Category(Screw)]
    [DisplayName("生命")]
    public int ScrewLifeCount
    {
        get
        {
            return UserData.Instance.GetRes(ResType.Energy);
        }
        set
        {
            if (value > 0)
            {
                UserData.Instance.AddRes(ResType.Energy, value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }
            else
            {
                UserData.Instance.ConsumeRes(ResType.Energy, -value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }        
        }
    }
    
    [Category(Screw)]
    [DisplayName("关卡ID")]
    public int MainLevel
    {
        get
        {
            return ScrewGameLogic.Instance.GetMainLevelIndex();
        }
        set
        {
            ScrewGameLogic.Instance._screwStorage.MainLevelIndex = value;
        }
    }


    [Category(Screw)]
    [DisplayName("完成当前关卡")]
    public void CompleteScrewLevel()
    {
        ScrewGameLogic.Instance.DebugPassWin();
    }
    
    [Category(Screw)]
    [DisplayName("道具_洞")]
    public int ScrewHoleCount
    {
        get
        {
            return UserData.Instance.GetRes(ResType.ExtraSlot);
        }
        set
        {
            if (value > 0)
            {
                UserData.Instance.AddRes(ResType.ExtraSlot, value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }
            else
            {
                UserData.Instance.ConsumeRes(ResType.ExtraSlot, -value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }    
        }
    }
    
    [Category(Screw)]
    [DisplayName("道具_锤子")]
    public int ScrewHammerCount
    {
        get
        {
            return UserData.Instance.GetRes(ResType.BreakBody);
        }
        set
        {  
            if (value > 0)
            {
                UserData.Instance.AddRes(ResType.BreakBody, value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }
            else
            {
                UserData.Instance.ConsumeRes(ResType.BreakBody, -value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }  
        }
    }
    
    [Category(Screw)]
    [DisplayName("道具_工具箱")]
    public int ScrewBoxCount
    {
        get
        {
            return UserData.Instance.GetRes(ResType.TwoTask);
        }
        set
        {
            if (value > 0)
            {
                UserData.Instance.AddRes(ResType.TwoTask, value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }
            else
            {
                UserData.Instance.ConsumeRes(ResType.TwoTask, -value,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
            }  
        }
    }
    
    [Category(Screw)]
    [DisplayName("重置道具解锁弹窗")]
    public void ScrewResetPropUnlockPopup()
    {
        StorageManager.Instance.GetStorage<StorageScrew>().IsTwoTaskPopGuide = false;
        StorageManager.Instance.GetStorage<StorageScrew>().IsBreakBodyPopGuide = false;
        StorageManager.Instance.GetStorage<StorageScrew>().IsExtraSlotPopGuide = false;
    }
    
    [Category(Screw)]
    [DisplayName("打开钉子界面")]
    public void ScrewOpenMainView()
    {
        SceneFsm.mInstance.EnterScrewHome();
    }
}