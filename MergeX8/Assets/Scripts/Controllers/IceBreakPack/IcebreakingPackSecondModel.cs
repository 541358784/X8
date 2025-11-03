using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Game;
using Gameplay.UI;

public class IcebreakingPackSecondModel : Manager<IcebreakingPackSecondModel>
{
    public StorageIceBreakPack packData
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().SecondIceBreakPackData; }
    }

    public List<IceBreakPack> GetIceBreakingPacks(int type = 2)
    {
        return AdConfigHandle.Instance.GetIceBreakPacksSecond().FindAll(a => a.Type == type);
    }

    public bool IsOpen()
    {
        if (IcebreakingPackModel.Instance.IsLock())
            return false;
        if (packData.IsFinish)
            return false;
        if (packData.StartTime <= 0)
        {
            return false;
        }

        return true;
    }

    public int GetPackCoolTime(int type = 3)
    {
        var configs = GetIceBreakingPacks(type);
        if (configs == null || configs.Count <= 0)
            return 0;
        var PackCfg = configs[0];
        if (IsOpen() == false)
            return 0;
        long leftTime = packData.StartTime + PackCfg.Duration - CommonUtils.GetTimeStamp() / 1000;
        if (packData.IsFinish == false && leftTime <= 0)
        {
            packData.IsFinish = true;
            packData.FinishTime = CommonUtils.GetTimeStamp() / 1000;
            EventDispatcher.Instance.DispatchEvent(EventEnum.IceBreak_Pack_Finish);
        }

        return Math.Max(0, (int) leftTime);
    }

    public bool IsCanBuyItem(int shopID)
    {
        return !packData.GotShopItem.Contains(shopID);
    }

    public void RecordBuyItem(int shopID)
    {
        if (!packData.GotShopItem.Contains(shopID))
        {
            packData.GotShopItem.Add(shopID);
            EventDispatcher.Instance.DispatchEvent(EventEnum.IceBreak_Pack_REFRESH, shopID);
        }

        //
        // if (packData.GotShopItem.Count >= GetIceBreakingPacks(2).Count)
        // {
        packData.IsFinish = true;
        packData.FinishTime = CommonUtils.GetTimeStamp() / 1000;
        EventDispatcher.Instance.DispatchEvent(EventEnum.IceBreak_Pack_Finish);
        // }
        var existExtraView = UIPopupExtraView.CheckExtraViewOpenState<IcebreakingPackLowExtraView>();
        if (existExtraView)
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpbuyLifeUseup,data1:"3");
    }

    public void RecordOpenState()
    {
        if (packData.LastPopUpTime <= 0)
        {
            packData.StartTime = CommonUtils.GetTimeStamp() / 1000;
            EventDispatcher.Instance.DispatchEvent(EventEnum.IceBreak_Pack_Begin);
        }

        packData.LastPopUpTime = CommonUtils.GetTimeStamp();
        packData.PopTimes++;
    }

    public void ClearPack()
    {
        packData.StartTime = 0;
        packData.LastPopUpTime = 0;
        packData.IsFinish = false;
        packData.GotShopItem.Clear();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.LossTime, "IceBreakPackLow");
    }
}