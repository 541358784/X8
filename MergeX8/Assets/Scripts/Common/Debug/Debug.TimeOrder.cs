using System;
using System.ComponentModel;
using Activity.CrazeOrder.Model;
using Activity.LimitTimeOrder;
using Activity.TimeOrder;
using DragonPlus.Config.CrazeOrder;
using DragonPlus.Config.LimitOrderLine;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;


public partial class SROptions
{
    private const string TimeOrder = "限时任务";
    [Category(TimeOrder)]
    [DisplayName("清理限时任务")]
    public void ClearTimeOrder()
    {
        HideDebugPanel();
        TimeOrderModel.Instance.TimeOrder.Clear();
        
        for (var i = 0; i < MainOrderManager.Instance.CurTaskList.Count; i++)
        {
            if(MainOrderManager.Instance.CurTaskList[i].Type != (int)MainOrderType.Time)
                continue;
            
            MainOrderManager.Instance.CurTaskList.RemoveAt(i);
            i--;
        }
        
        if(TimeOrderConfigManager.Instance.TableTimeOrderConfigList == null)
            return;
        
        foreach (var timeOrderConfig in TimeOrderConfigManager.Instance.TableTimeOrderConfigList)
        {
            MainOrderManager.Instance.StorageTaskGroup.CompletedTaskIds.Remove(timeOrderConfig.Id);
        }
    }
    
    private const string TimeOrderLine = "限时任务链";
    [Category(TimeOrderLine)]
    [DisplayName("清理限时任务链")]
    public void ClearTimeOrderLine()
    {
        HideDebugPanel();
        LimitTimeOrderModel.Instance.LimitOrderLine.Clear();
        
        for (var i = 0; i < MainOrderManager.Instance.CurTaskList.Count; i++)
        {
            if(MainOrderManager.Instance.CurTaskList[i].Type != (int)MainOrderType.Limit)
                continue;
            
            MainOrderManager.Instance.CurTaskList.RemoveAt(i);
            i--;
        }
        
        if(LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList == null)
            return;
        
        foreach (var timeOrderConfig in LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList)
        {
            foreach (var orderId in timeOrderConfig.OrderIds)
            {
                MainOrderManager.Instance.StorageTaskGroup.CompletedTaskIds.Remove(orderId);
            }
        }
    }
    
    
    private const string CrazeOrder = "订单狂热";
    [Category(CrazeOrder)]
    [DisplayName("清理订单狂热")]
    public void ClearCrazeOrder()
    {
        HideDebugPanel();
        CrazeOrderModel.Instance.CrazeOrder.Clear();
        
        for (var i = 0; i < MainOrderManager.Instance.CurTaskList.Count; i++)
        {
            if(MainOrderManager.Instance.CurTaskList[i].Type != (int)MainOrderType.Craze)
                continue;
            
            MainOrderManager.Instance.CurTaskList.RemoveAt(i);
            i--;
        }
    }
    [Category(CrazeOrder)]
    [DisplayName("设置完成任务数量")]
    public int crazeNum
    {
        get
        {
            return CrazeOrderModel.Instance.CompleteNum;
        }
        set
        {
            var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
            CrazeOrderModel.Instance.CompleteNum = value;
            CrazeOrderModel.Instance.CompleteNum = Math.Min(CrazeOrderModel.Instance.CompleteNum, stageConfigs[stageConfigs.Count - 1].OrderNum);
            CrazeOrderModel.Instance.CompleteNum = Math.Max(0, CrazeOrderModel.Instance.CompleteNum);
        }
    }
    
}