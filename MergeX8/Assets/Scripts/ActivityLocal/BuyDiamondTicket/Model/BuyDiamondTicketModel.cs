using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class BuyDiamondTicketModel:Manager<BuyDiamondTicketModel>
{
    public Dictionary<int, TableBuyDiamondTicket> Config => GlobalConfigManager.Instance.TableBuyDiamondTickets;
    public List<StorageBuyDiamondTicket> Storage=>StorageManager.Instance.GetStorage<StorageHome>().BuyDiamondTicket;

    public bool IsTicket(int id)
    {
        return Config.ContainsKey(id);
    }
    public void OnCollectTicket(int ticketId)
    {
        if (!IsTicket(ticketId))
            return;
        var config = Config[ticketId];
        var endTime = (long)APIManager.Instance.GetServerTime() + (config.activeTime * (long)XUtility.Min);
        var ticketStorage = new StorageBuyDiamondTicket();
        ticketStorage.TicketId = config.id;
        ticketStorage.EndTime = endTime;
        Storage.Add(ticketStorage);
        EventDispatcher.Instance.SendEventImmediately(new EventBuyDiamondTicketBagChange());
    }

    public int GetTicketCount(int ticketId)
    {
        if (!IsTicket(ticketId))
            return 0;
        var leftTicketCount = 0;
        foreach (var ticket in Storage)
        {
            if (ticket.TicketId == ticketId)
                leftTicketCount++;
        }
        return leftTicketCount;
    }

    public StorageBuyDiamondTicket GetActiveTicket()
    {
        if (Storage.Count == 0)
            return null;
        TableBuyDiamondTicket activeTicketConfig = null;
        for (var i = 0; i < Storage.Count; i++)
        {
            var config = Config[Storage[i].TicketId];
            if (activeTicketConfig == null || activeTicketConfig.percent < config.percent)
            {
                activeTicketConfig = config;
            }
        }
        StorageBuyDiamondTicket activeTicket = null;
        for (var i = 0; i < Storage.Count; i++)
        {
            if (Storage[i].TicketId == activeTicketConfig.id)
            {
                if (activeTicket == null || Storage[i].EndTime < activeTicket.EndTime)
                {
                    activeTicket = Storage[i];
                }
            }
        }
        return activeTicket;
    }

    public void OnUseTicket(StorageBuyDiamondTicket ticket)
    {
        Storage.Remove(ticket);
        EventDispatcher.Instance.SendEventImmediately(new EventBuyDiamondTicketBagChange());
    }

    protected override void InitImmediately()
    {
        base.InitImmediately();
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }

    public void UpdateTime()
    {
        if (Storage.Count == 0)
            return;
        var curTime = (long)APIManager.Instance.GetServerTime();
        for (var i=0;i<Storage.Count;i++)
        {
            var ticket = Storage[i];
            if (ticket.EndTime < curTime)
            {
                Storage.RemoveAt(i);
                i--;
                EventDispatcher.Instance.SendEventImmediately(new EventBuyDiamondTicketBagChange());
            }
        }
    }

    public List<ResData> PurchaseDiamond(int shopId)
    {
        var ticket = GetActiveTicket();
        if (ticket == null)
            return null;
        var ticketConfig = Config[ticket.TicketId];
        var triggerIndex = -1;
        for (var i = 0; i < ticketConfig.shopId.Length; i++)
        {
            if (ticketConfig.shopId[i] == shopId)
            {
                triggerIndex = i;
                break;
            }
        }
        if (triggerIndex < 0)
        {
            return null;
        }
        var extraDiamond = ticketConfig.diamond[triggerIndex];
        OnUseTicket(ticket);
        var rewards = new List<ResData>
        {
            new ResData((int)UserData.ResourceId.Diamond,extraDiamond)
        };
        return rewards;
    }
    
    
    
    
    #region  Entrance
    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = GetTaskEntrance();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = GetAuxItem();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    public bool ShowAuxItem()
    {
        return GetActiveTicket() != null;
    }

    public Aux_BuyDiamondTicket GetAuxItem()
    {
        return Aux_BuyDiamondTicket.Instance;
    }

    public bool ShowTaskEntrance()
    {
        return GetActiveTicket() != null;
    }
    public MergeBuyDiamondTicket GetTaskEntrance()
    {
        return MergeTaskTipsController.Instance.MergeBuyDiamondTicket;
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Home/Aux_BuyDiamondTicket";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Home/TaskList_BuyDiamondTicket";
    }
    #endregion
    
    
    public const string coolTimeKey = "BuyDiamondTicket";
    public static bool CanShowStartPopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;
        if (CanShowStartPopup())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }

    public static bool CanShowStartPopup()
    {
        var ticket = Instance.GetActiveTicket();
        if (ticket != null)
        {
            UIBuyDiamondTicketController.Open(ticket);
            return true;
        }
        return false;
    }
}