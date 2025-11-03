using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using Difference;
using DragonPlus;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.Config.KeepPet;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Config;
using Gameplay;
using Merge.Order;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;


public partial class SROptions
{
    private int addMergeId = 0;
    [Sort(100)]
    [Category(Merge)]
    [DisplayName("mergeId")]
    public int AddMergeId
    {
        get { return addMergeId ;}
        set { addMergeId = value; }
    }
    
    [Sort(110)]
    [Category(Merge)]
    [DisplayName("增加merge元素")]
    public void AddMergeItem()
    {
        if (GameConfigManager.Instance.GetItemConfig(addMergeId) == null)
            return;

        var mergeItem = MergeManager.Instance.GetEmptyItem();
        mergeItem.Id = addMergeId;
        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main, 1);
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
        
    }
    [Sort(110)]
    [Category(Merge)]
    [DisplayName("增加背包Merge元素")]
    public void AddBagItem()
    {
        if (GameConfigManager.Instance.GetItemConfig(addMergeId) == null)
            return;
        
        var mergeItem = MergeManager.Instance.GetEmptyItem();
        mergeItem.Id = addMergeId;
        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem, MergeBoardEnum.Main,1);
        MergeManager.Instance.AddBagItem(mergeItem,MergeBoardEnum.Main);
    }   

    [Category(Merge)]
    [DisplayName("显示 Merge 棋盘")]
    public void ShowMergeBoard()
    {
        foreach (var kv in MergeMainController.Instance.MergeBoard.Grids)
        {
            if (kv.board == null)
                continue;

            kv.board.Debug_ShowIcon();
        }
    }
    [Category(Merge)]
    [DisplayName("清理 棋盘")]
    public void ClearMainMergeBoard()
    {
        MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Clear();
    }
    
    [Sort(120)]
    [Category(Merge)]
    [DisplayName("效验任务mergeId")]
    public void CheckGuideMergeId()
    {
        Debug.LogError("每日任务 检测开始");
        foreach (var table in OrderConfigManager.Instance._orderCreates)
        {
            var itemIds = table.itemId;
            var itemNums = table.itemNum;


            string taskType = "order create";
            if (itemIds == null || itemNums == null)
            {
                Debug.LogError("tableId: " + table.id + "\t type: " + taskType + "\t itemid itemnum null");
                continue;
            }
                
            if (itemIds.Length != itemNums.Length)
            {
                Debug.LogError("tableId: " + table.id + "\t type: " + taskType + "\t itemid itemnum num error");
                continue;
            }
                
            foreach (var id in itemIds)
            {
                if (GameConfigManager.Instance.GetItemConfig(id) == null)
                    Debug.LogError("tableId " + table.id + "\t type: " + taskType + "\t mergeItem null ID: " + id);
            }
        }
        Debug.LogError("每日任务 检测结束");
        Debug.LogError("闪购奖励 检测开始");
        foreach (var table in  AdConfigExtendConfigManager.Instance.GetConfig<FlashSale>())
        {
            
            foreach (var item in table.GemItem)
            {
                if(UserData.Instance.IsResource(item))
                    continue;
            
                if (GameConfigManager.Instance.GetItemConfig(item) == null)
                    Debug.LogError("tableId " + table.Id + " mergeItem null ID: " + item);
            }
       
            if(table.GemItem.Count!=table.GmeWighht.Count)
                Debug.LogError("tableId " + table.Id + " GemItem != GemWight: "+"table.GemItem.Count:"+table.GemItem.Count+" table.GmeWighht.Count :"+table.GmeWighht.Count);
                    
         
            if(table.GemItem.Count!=table.GemPrice.Count)
                Debug.LogError("tableId " + table.Id + " GemItem != GemPrice: "+" table.GemItem.Count"+table.GemItem.Count+" table.GemPrice.Count:"+table.GemPrice.Count);
         
        }
        Debug.LogError("闪购奖励 检测结束");
        
        Debug.LogError("闪购礼包 检测开始");
        foreach (var table in AdConfigExtendConfigManager.Instance.GetConfig<TaskAssist>())
        {
            foreach (var item in table.Content)
            {
                if (!UserData.Instance.IsResource(item) && GameConfigManager.Instance.GetItemConfig(item) == null)
                    Debug.LogError("tableId " + table.Id + "reward error " + item);
            }
      
        }
        Debug.LogError("闪购礼包 检测结束");      
        
        Debug.LogError("神秘礼物 检测开始");
        foreach (var table in AdConfigExtendConfigManager.Instance.GetConfig<MysteryGift>())
        {
            foreach (var item in table.Reward1)
            {
                if (!UserData.Instance.IsResource(item) && GameConfigManager.Instance.GetItemConfig(item) == null)
                    Debug.LogError("tableId " + table.Id + "Reward1 error " + item);
            }
            foreach (var item in table.Reward2)
            {
                if (!UserData.Instance.IsResource(item) && GameConfigManager.Instance.GetItemConfig(item) == null)
                    Debug.LogError("tableId " + table.Id + "Reward2 error " + item);
            }
            foreach (var item in table.Reward3)
            {
                if (!UserData.Instance.IsResource(item) && GameConfigManager.Instance.GetItemConfig(item) == null)
                    Debug.LogError("tableId " + table.Id + "Reward3 error " + item);
            }
            foreach (var item in table.Reward4)
            {
                if (!UserData.Instance.IsResource(item) && GameConfigManager.Instance.GetItemConfig(item) == null)
                    Debug.LogError("tableId " + table.Id + "Reward4 error " + item);
            }
            
      
        }
        Debug.LogError("神秘礼物 检测结束");
       
    }

    [Sort(120)]
    [Category(Merge)]
    [DisplayName("效验Merge表")]
    public void CheckMergeId()
    {
        //----------------------------------------------------------------------------------------------------------------------
        Debug.LogError("初始棋盘1 检测开始");
        var mergeBoard= GameConfigManager.Instance.BoardGrid_1;
        foreach (var table in mergeBoard)
        {
            if (table.itemId > 0 && !UserData.Instance.IsResource(table.itemId) &&
                GameConfigManager.Instance.GetItemConfig(table.itemId) == null)
                Debug.LogError("tableId " + table.id + " error " + table.itemId);
        }
        //----------------------------------------------------------------------------------------------------------------------
        Debug.LogError("初始棋盘1 检测结束");
        //----------------------------------------------------------------------------------------------------------------------
        Debug.LogError("初始棋盘2 检测开始");
        mergeBoard= GameConfigManager.Instance.BoardGrid_2;
        foreach (var table in mergeBoard)
        {
            if (table.itemId > 0 && !UserData.Instance.IsResource(table.itemId) &&
                GameConfigManager.Instance.GetItemConfig(table.itemId) == null)
                Debug.LogError("tableId " + table.id + " error " + table.itemId);
        }
        
        //----------------------------------------------------------------------------------------------------------------------
        Debug.LogError("初始棋盘2 检测结束");
        
        Debug.LogError("初始棋盘3 检测开始");
        mergeBoard= GameConfigManager.Instance.BoardGrid_3;
        foreach (var table in mergeBoard)
        {
            if (table.itemId > 0 && !UserData.Instance.IsResource(table.itemId) &&
                GameConfigManager.Instance.GetItemConfig(table.itemId) == null)
                Debug.LogError("tableId " + table.id + " error " + table.itemId);
        }
        
        //----------------------------------------------------------------------------------------------------------------------
        Debug.LogError("初始棋盘3 检测结束");
        
        var mergeItems= GameConfigManager.Instance.MergeItemList;
       //----------------------------------------------------------------------------------------------------------------------
       Debug.LogError("产出对象 检测开始");
       foreach (var table in mergeItems)
       {
     
           if (table.output==null)
                continue;
           foreach (var item in table.output)
           {
               if (item > 0 && !UserData.Instance.IsResource(item) &&
                   GameConfigManager.Instance.GetItemConfig(item) == null)
                   Debug.LogError("tableId " + table.id + " error " + item);
           }
       }
       Debug.LogError("产出对象 检测结束");
       Debug.LogError("产出权重 检测开始");
       foreach (var table in mergeItems)
       {
           if (table.output==null||table.power==null)
               continue;
           //----------------------------------------------------------------------------------------------------------------------
           if(table.output.Length!=table.power.Length)
               Debug.LogError("tableId " + table.id + " error table.output.Length!=table.power.Length" );
       }
       Debug.LogError("产出权重 检测结束");
   
       Debug.LogError("时间产出对象 检测开始");
       foreach (var table in mergeItems)
       {
           //----------------------------------------------------------------------------------------------------------------------
           if (table.timeOutput != null)
           {
               foreach (var item in table.timeOutput)
               {
                   if (item>0 && !UserData.Instance.IsResource(item) && GameConfigManager.Instance.GetItemConfig(item) == null)
                       Debug.LogError("tableId " + table.id + " error " + item);
               } 
           }

       }
       Debug.LogError("时间产出对象 检测结束");
       //----------------------------------------------------------------------------------------------------------------------
       Debug.LogError("时间产出权重 检测开始");
       foreach (var table in mergeItems)
       {
           if (table.timeOutput != null)
           {
               if(table.timeOutputPower!=null &&table.timeOutput.Length!=table.timeOutputPower.Length)
                   Debug.LogError("tableId " + table.id + " error table.timeOutput.Length!=table.timeOutputPower.Length" );

           }
       }  
       Debug.LogError("时间产出权重 检测结束");
          
       
       //----------------------------------------------------------------------------------------------------------------------
       Debug.LogError("合成链 检测开始");
       var mergeLineList= GameConfigManager.Instance.MergeLineList;
       foreach (var table in mergeLineList)
       {
     
           if (table.output==null)
               continue;
           foreach (var item in table.output)
           {
               if (item > 0 && !UserData.Instance.IsResource(item) &&
                   GameConfigManager.Instance.GetItemConfig(item) == null)
                   Debug.LogError("tableId " + table.id + "output error " + item);
           }
       }
       foreach (var table in mergeLineList)
       {
     
           if (table.presetDropQueue==null)
               continue;
           foreach (var item in table.presetDropQueue)
           {
               if (item > 0 && !UserData.Instance.IsResource(item) &&
                   GameConfigManager.Instance.GetItemConfig(item) == null)
                   Debug.LogError("tableId " + table.id + "presetDropQueue error " + item);
           }
       }
       Debug.LogError("合成链 检测结束");
       
       
       //----------------------------------------------------------------------------------------------------------------------
       Debug.LogError("小狗奖励池 检测开始");
       var poolConfig= KeepPetConfigManager.Instance.GetConfig<KeepPetSearchTaskRewardPoolConfig>();
       foreach (var table in poolConfig)
       {
           if (table.ItemId > 0 && !UserData.Instance.IsResource(table.ItemId) &&
               GameConfigManager.Instance.GetItemConfig(table.ItemId) == null)
               Debug.LogError("tableId " + table.Id + " error " + table.ItemId);
       }
       Debug.LogError("小狗奖励池 检测结束");
       //----------------------------------------------------------------------------------------------------------------------
       
       
    }

    [Sort(120)]
    [Category(Merge)]
    [DisplayName("效验装修表")]
    public void CheckDeco()
    {
       var nodeConfigs= DecorationConfigManager.Instance.nodeConfigs;
       Debug.LogError("开始效验 Node 表");
       foreach (var node in nodeConfigs)
       {
           if (node.rewardId == null || node.rewardNumber == null)
           {
               Debug.LogError("tableId " + node.id + " node.rewardId == null || node.rewardNumber == null  ");
               continue;
           }
            if(node.rewardId.Length!=node.rewardNumber.Length)   
                Debug.LogError("tableId " + node.id + " node.rewardId.Length!=node.rewardNumber.Length  ");
            
            foreach (var item in node.rewardId)
            {
                if (item > 0 && !UserData.Instance.IsResource(item) &&
                    GameConfigManager.Instance.GetItemConfig(item) == null)
                    Debug.LogError("tableId " + node.id + "node item err " + item);
            }
       }
       Debug.LogError(" Node 表 检测结束");

    }
    [Sort(120)]
    [Category(Merge)]
    [DisplayName("效验RVRES表")]
    public void CheckRVRES()
    {
        List<RVshopResource> rVshopResources = AdConfigExtendConfigManager.Instance.GetConfig<RVshopResource>();
       Debug.LogError("开始效验 rVshopResources 表");
       foreach (var node in rVshopResources)
       {
          if(node.RewardID.Count!= node.Amount.Count)
              Debug.LogError(" rVshopResources 表数量 不相等"+node.Id);

          foreach (var rewardID in node.RewardID)
          {
              if (rewardID > 0 && !UserData.Instance.IsResource(rewardID) &&
                  GameConfigManager.Instance.GetItemConfig(rewardID) == null)
                  Debug.LogError("tableId " + node.Id + "node item err " + rewardID);
          }
       }
       Debug.LogError(" rVshopResources 表 检测结束");

    }

    [Category(Merge)]
    [DisplayName("使用棋盘-1")]
    public void RefreshBoard1()
    {
        DifferenceManager.Instance.IsOpenDifference = false;

        MergeManager.Instance.AdaptBoard();
    }
    
    
    [Category(Merge)]
    [DisplayName("使用棋盘-2")]
    public void RefreshBoard2()
    {
        DifferenceManager.Instance.IsOpenDifference = true;
        MergeManager.Instance.AdaptBoard();
    }
    
    [Category(Merge)]
    [DisplayName("清空棋盘")]
    public void CleanAllBoard()
    {
        foreach (var storageMergeItem in MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Items)
        {
            storageMergeItem.Id = -1;
            storageMergeItem.State = -1;
            MergeManager.Instance.ResetStorageMergeBoardStatus(storageMergeItem);
        }
    }
}