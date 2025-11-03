using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Decoration;
using DragonPlus;
using DragonPlus.Config;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Game;
using Gameplay;
using Merge.Order;
using UnityEngine;

public partial class SROptions
{
      [Category(Task)]
    [DisplayName("完成当前主线任务")]
    public void CompleteAllTask()
    {
        if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
            return;

        foreach (var task in MainOrderManager.Instance.CurTaskList)
        {
            if (MainOrderManager.Instance.debugCompleteTaskIds.ContainsKey(task.Id))
                continue;

            MainOrderManager.Instance.debugCompleteTaskIds.Add(task.Id, task.Id);
        }

        EventDispatcher.Instance.DispatchEvent(MergeEvent.DEBUG_TASK_REFRESH);
    }

    [Category(Task)]
    [DisplayName("重置完成任务标记")]
    public void RestAllTask()
    {
        if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
            return;

        MainOrderManager.Instance.debugCompleteTaskIds.Clear();

        EventDispatcher.Instance.DispatchEvent(MergeEvent.DEBUG_TASK_REFRESH);
    }

    

    [Category(Task)]
    [DisplayName("开关debug模式")]
    public bool DebugModule
    {
        get
        {
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
                return false;

            return MainOrderManager.Instance.OpenDebugModule;
        }
        set
        {
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
                return;

            MainOrderManager.Instance.OpenDebugModule = value;
            MergeTaskTipsController.Instance.RefreshDebugModule();
        }
    }
    
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-Free")]
    public void Create_Free()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[MainOrderCreatorReturnFreeOrder._orderRefreshTimeKey] = -1;
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[SlotDefinition.Slot1.ToString()] = (long)APIManager.Instance.GetServerTime();
        MainOrderCreatorReturnFreeOrder.CreateOrder(SlotDefinition.Slot1);
    }
    
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-Random1")]
    public void Create_Random1()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime["MainOrderCreatorRandom1_Point"] = 0;
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[SlotDefinition.Slot2.ToString()] = (long)APIManager.Instance.GetServerTime();
        MainOrderCreatorRandom1.CreateOrder(SlotDefinition.Slot2);
    }
    
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-Random3")]
    public void Create_Random3()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[SlotDefinition.Slot3.ToString()] = (long)APIManager.Instance.GetServerTime();
        MainOrderCreatorRandom2.CreateOrder(SlotDefinition.Slot3);
    }
    
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-Random4")]
    public void Create_Random4()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[SlotDefinition.Slot4.ToString()] = (long)APIManager.Instance.GetServerTime();
        MainOrderCreatorRandom3.CreateOrder(SlotDefinition.Slot4);
    }
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-Random5")]
    public void Create_Random5()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[SlotDefinition.Slot5.ToString()] = (long)APIManager.Instance.GetServerTime();
        MainOrderCreatorRandom4.CreateOrder(SlotDefinition.Slot5);
    }
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-Random8")]
    public void Create_Random8()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[SlotDefinition.Slot8.ToString()] = (long)APIManager.Instance.GetServerTime();
        MainOrderCreatorRandom5.CreateOrder(SlotDefinition.Slot8);
    }
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-RandomBranch")]
    public void Create_Branch()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[SlotDefinition.BranchSlot.ToString()] = (long)APIManager.Instance.GetServerTime();
        MainOrderCreatorBranch.CreateOrder(SlotDefinition.BranchSlot);
    }
    
    [Sort(101)]
    [Category(Task)]
    [DisplayName("生成任务-所有")]
    public void Create_All()
    {
        Create_Free();
        Create_Random1();
        Create_Random3();
        Create_Random4();
        Create_Random5();
        Create_Random8();
        Create_Branch();
    }
    
    [Sort(101)]
    [Category(Task)]
    [DisplayName("清除CD")]
    public void ClearCD()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Clear();
    }
    [Category(Task)]
    [DisplayName("AB-任务规则组")]
    public int OrderRules
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().OrderRules;
        }
        set
        {
            StorageManager.Instance.GetStorage<StorageHome>().OrderRules = value;
        }
    }
    [Category(Task)]
    [DisplayName("理解刷新回收任务")]
    public void RefreshCommend()
    {
        MainOrderCreatorRecommend.RefreshRecommend();
    }
}