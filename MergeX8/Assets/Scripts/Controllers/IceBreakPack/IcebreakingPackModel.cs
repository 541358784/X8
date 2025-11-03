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

public class IcebreakingPackModel : Manager<IcebreakingPackModel>
{
    public StorageIceBreakPack packData
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().IceBreakPackData; }
    }

    public int packType = 1;

    public List<IceBreakPack> GetIceBreakingPacks()
    {
        return AdConfigHandle.Instance.GetIceBreakPacks().FindAll(a => a.Type == packType);
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
    
    public bool IsLock()//是否被新新破冰礼包锁定
    {
        return NewNewIceBreakPackModel.Instance.Storage.IsNewUser;
    }

    public int GetPackCoolTime()
    {
        var configs = GetIceBreakingPacks();
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
        // if (packData.GotShopItem.Count >= GetIceBreakingPacks().Count)
        // {
        packData.FinishTime = CommonUtils.GetTimeStamp() / 1000;
        packData.IsFinish = true;
        EventDispatcher.Instance.DispatchEvent(EventEnum.IceBreak_Pack_Finish);
        // }
        var existExtraView = UIPopupExtraView.CheckExtraViewOpenState<IcebreakingPackExtraView>();
        if (existExtraView)
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpbuyLifeUseup,data1:"2");
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
        packData.Clear();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.LossTime, "IceBreakPack");
    }
}