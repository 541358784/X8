using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;

public class TaskAssistPackModel : Manager<TaskAssistPackModel>
{
    public const int TaskAssistPackCount = 3;
    public StorageTaskAssistPack packData
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().TaskAssistPackData; }
    }

    public bool IsOpen()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TaskAssist))
            return false;
        Common commonData = AdConfigHandle.Instance.GetCommon();
        if (commonData == null)
            return false;
        if (commonData.TaskAssist < 0)
            return false;

        return true;
    }

    public StorageTaskAssistPack GetTaskAssistPack()
    {
        GenTaskAssistPack();
        return packData;
    }

    /// <summary>
    /// 生成礼包 
    /// </summary>
    public void GenTaskAssistPack()
    {
        if (!IsOpen())
            return;
        //检查是否有过期礼包，清除
        RefreshPackStorage();
        //如果当时礼包大于2 不生成 
        if (packData.TaskAssistPacks.Count >= TaskAssistPackCount)
            return;
        // 获取当前任务需要的物品
        var taskList = GetTaskNeedAssist();
        //根据合成链找对应配置
        foreach (var taskItem in taskList)
        {
            //如果任务已经生成过礼包 跳过
            if (packData.FinishTaskList.Contains(taskItem.Id))
                continue;
            var maxPriceItem = MainOrderManager.Instance.GetTaskMaxPrice(taskItem);
            if (maxPriceItem != null)
            {
                var configs = AdConfigHandle.Instance.GetTaskAssistPacksByLine(maxPriceItem.in_line);
                if (configs != null && configs.Count > 0)
                {
                    TaskAssist config = configs[0];
                    foreach (var taskAssistItem in configs)
                    {
                        if (maxPriceItem.level >= taskAssistItem.TastItemLevel)
                            config = taskAssistItem;
                    }

                    if (packData.TaskAssistPacks.Count >= TaskAssistPackCount)
                        break;

                    //写入礼包存档 
                    StorageTaskAssistPackItem storageTaskAssistPackItem = new StorageTaskAssistPackItem();
                    storageTaskAssistPackItem.PackId = config.Id;
                    storageTaskAssistPackItem.EndTime = CommonUtils.GetTimeStamp() + config.Duration * 1000;
                    storageTaskAssistPackItem.LeftBuyCount = config.Times;
                    if (packData.TaskAssistPacks.Find(a => { return a.PackId == config.Id; }) == null)
                        packData.TaskAssistPacks.Add(storageTaskAssistPackItem);
                    packData.FinishTaskList.Add(taskItem.Id);
                    Debug.Log("TaskAssist  生成礼包-------------");
                }
            }
        }
    }

    public void RefreshPackStorage()
    {
        if (packData.TaskAssistPacks != null && packData.TaskAssistPacks.Count > 0)
        {
            for (int i = packData.TaskAssistPacks.Count - 1; i >= 0; i--)
            {
                var config = AdConfigHandle.Instance.GetTaskAssistPackById(packData.TaskAssistPacks[i].PackId);
                if (CommonUtils.GetTimeStamp() > packData.TaskAssistPacks[i].EndTime || config == null ||
                    packData.TaskAssistPacks[i].LeftBuyCount == 0)
                {
                    packData.TaskAssistPacks.Remove(packData.TaskAssistPacks[i]);
                }
            }
        }
    }

    public bool IsCanBuy(int packId)
    {
        StorageTaskAssistPackItem packItem = packData.TaskAssistPacks.Find(a => { return a.PackId == packId; });
        if (packItem == null || CommonUtils.GetTimeStamp() > packItem.EndTime || packItem.LeftBuyCount == 0)
        {
            return false;
        }

        return true;
    }

    public int GetCanBuyCount()
    {
        int count = 0;
        foreach (var packItem in packData.TaskAssistPacks)
        {
            if (IsCanBuy(packItem.PackId))
            {
                count++;
            }
        }

        return count;
    }

    public StorageTaskAssistPackItem GetTaskAssistPackItem(int packId)
    {
        StorageTaskAssistPackItem packItem = packData.TaskAssistPacks.Find(a => { return a.PackId == packId; });
        return packItem;
    }

    public int GetLeftCount(int packId)
    {
        StorageTaskAssistPackItem packItem = packData.TaskAssistPacks.Find(a => { return a.PackId == packId; });
        if (packItem != null)
            return packItem.LeftBuyCount;

        return 0;
    }

    /// <summary>
    /// 获取当前需要礼包的任务
    /// </summary>
    public List<StorageTaskItem> GetTaskNeedAssist()
    {
        List<StorageTaskItem> itemList = new List<StorageTaskItem>();
        foreach (var taskItem in MainOrderManager.Instance.CurTaskList)
        {
            if (taskItem.Assist)
            {
                itemList.Add(taskItem);
            }
        }

        return itemList;
    }

    public void RecordBuyPack(int packId)
    {
        StorageTaskAssistPackItem packItem = packData.TaskAssistPacks.Find(a => { return a.PackId == packId; });
        if (packItem != null)
            packItem.LeftBuyCount--;
    }

    public void PurchaseSuccess(TableShop shopConfig, int packId)
    {
        var bundleCfg = AdConfigHandle.Instance.GetTaskAssistPackById(packId);
        if (bundleCfg == null)
        {
            foreach (var data in packData.TaskAssistPacks)
            {
                var tempConfig = AdConfigHandle.Instance.GetTaskAssistPackById(data.PackId);
                if (shopConfig.id == tempConfig.ShopItem)
                    bundleCfg = tempConfig;
            }
        }

        if (bundleCfg == null)
            return;
        List<ResData> listResData = new List<ResData>();
        for (int i = 0; i < bundleCfg.Content.Count; ++i)
        {
            int id = bundleCfg.Content[i];
            int count = bundleCfg.Count[i];
            ResData resData = new ResData(id, count);
            listResData.Add(resData);
            if (!UserData.Instance.IsResource(id))
            {
                TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
                if (mergeItemConfig != null)
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonTaskAssist,
                        itemAId = mergeItemConfig.id,
                        ItemALevel = mergeItemConfig.level,
                        isChange = true,
                    });
                }
            }
        }

        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TaskAssist);
        reasonArgs.data1 = shopConfig.id.ToString();
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);
        CommonRewardManager.Instance.PopCommonReward(listResData,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, reasonArgs, () =>
            {
                RecordBuyPack(packId);
                EventDispatcher.Instance.DispatchEvent(EventEnum.TASKASSIST_PURCHASE_REFRESH);
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
            });
    }
    
    public void DebugGenTaskAssistPack()
    {
        packData.TaskAssistPacks.Clear();
        var configs=  AdConfigHandle.Instance.GetTaskAssistPacks();
        for (int i = 0; i < configs.Count; i++)
        {
            TaskAssist config = configs[i];
            //写入礼包存档 
            StorageTaskAssistPackItem storageTaskAssistPackItem = new StorageTaskAssistPackItem();
            storageTaskAssistPackItem.PackId = config.Id;
            storageTaskAssistPackItem.EndTime = CommonUtils.GetTimeStamp() + config.Duration * 1000;
            storageTaskAssistPackItem.LeftBuyCount = config.Times;
            if (packData.TaskAssistPacks.Find(a => { return a.PackId == config.Id; }) == null)
                packData.TaskAssistPacks.Add(storageTaskAssistPackItem);
            packData.FinishTaskList.Add(i);
            Debug.Log("TaskAssist  生成礼包-------------");
        }
      
    }
}