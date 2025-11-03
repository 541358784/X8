// using System;
// using System.Collections.Generic;
// using DragonPlus;
// using DragonPlus.ConfigHub.Ad;
// using DragonU3DSDK;
// using DragonU3DSDK.Network.API;
// using DragonU3DSDK.Storage;
// using Gameplay;
// using UnityEngine;
// using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
// using Random = UnityEngine.Random;
//
// public partial class TaskModuleManager
// {
//     private void CreateReadilyTask()
//     {
//         if(DynamicTaskConfigManager.Instance._attenuations[0].readily_open >  ExperenceModel.Instance.GetLevel())
//             return;
//         
//         UpdateReadilyTime();
//
//         if(_storageTaskGroup.ReadilyCompleteNum >= DynamicTaskConfigManager.Instance._attenuations[0].readily_taskmax)
//             return;
//         
//         if(_storageTaskGroup.ReadilyStorageNum + _storageTaskGroup.ReadilyRefreshNum >= DynamicTaskConfigManager.Instance._attenuations[0].readily_daymax)
//             return;
//         
//         StorageTaskGroup.ReadilyRecordsTime += _repeatRate;
//         if(StorageTaskGroup.ReadilyRecordsTime < DynamicTaskConfigManager.Instance._attenuations[0].readily_time*60)
//             return;
//
//         StorageTaskGroup.ReadilyRecordsTime = 0;
//         if(HaveReadilyTask())
//             return;
//         
//         if(GetNormalTaskNum(true) > DynamicTaskConfigManager.Instance._attenuations[0].readily_normaltasknum)
//             return;
//
//         int num = CalculationReadilyTaskNum();
//         if(num <= 0)
//             return;
//
//         List<StorageTaskItem> taskItems = CreateReadilyTask(1);
//         if(taskItems == null || taskItems.Count == 0)
//             return;
//
//         _storageTaskGroup.ReadilyRefreshNum += taskItems.Count;
//         _storageTaskGroup.ReadilyStorageNum += Math.Max(0, num-taskItems.Count);
//         
//         
//         if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.gameObject.activeInHierarchy && taskItems.Count > 0)
//         {
//             MergeTaskTipsController.Instance.RefreshTask(taskItems, () =>
//             {
//                 EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
//                 MergeMainController.Instance?.UpdateTaskRedPoint();
//             });
//         }
//     }
//
//     public List<StorageTaskItem> CompleteReadilyTask(StorageTaskItem taskItem, Dictionary<int, int> filter)
//     {
//         DailyCompleteTaskNumManager.Instance.SaveCompleteTaskNum();
//         EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_STATISTICS, TaskStatisticsModel.CustomType.completeTask,1);
//         StorageTaskGroup.CurTasks.Remove(taskItem);
//
//         if (!StorageTaskGroup.CompletedTaskIds.ContainsKey(taskItem.Id))
//         {
//             StorageTaskGroup.CompletedTaskIds.Add(taskItem.Id, taskItem.Id);
//         }
//         
//         if (_storageTaskGroup.ReadilyStorageNum <= 0)
//             return null;
//         
//         _storageTaskGroup.ReadilyStorageNum--;
//         _storageTaskGroup.ReadilyRefreshNum++;
//         
//         return CreateReadilyTask(1, filter);
//     }
//     private void UpdateReadilyTime()
//     {
//         long serverTime = (long)APIManager.Instance.GetServerTime() / 1000;
//         if (!Utils.IsSameDay(StorageTaskGroup.ReadilyTime, serverTime))
//         {
//             StorageTaskGroup.RefreshEasyTime = 0;
//             _storageTaskGroup.ReadilyRefreshNum = 0;
//             StorageTaskGroup.ReadilyRecordsTime = 0;
//             StorageTaskGroup.ReadilyCompleteNum = 0;
//             StorageTaskGroup.ReadilyTime = (long)APIManager.Instance.GetServerTime()/1000;
//         }
//     }
//
//     public bool HaveReadilyTask()
//     {
//         foreach (var taskItem in CurTaskList)
//         {
//             if (taskItem.Type == (int)TaskType.Readily )
//                return true;
//         }
//
//         return false;
//     }
//     
//     private int CalculationReadilyTaskNum()
//     {
//         int diffNum = DynamicTaskConfigManager.Instance._attenuations[0].readily_daymax - (_storageTaskGroup.ReadilyStorageNum + _storageTaskGroup.ReadilyRefreshNum);
//         if (diffNum == 0)
//             return 0;
//         
//         int randomNum = Math.Min(diffNum, DynamicTaskConfigManager.Instance._attenuations[0].readily_randomnum);
//         if (randomNum == 0)
//             return 0;
//
//         randomNum = Random.Range(1, randomNum+1);
//         return randomNum;
//     }
//
//     public List<StorageTaskItem>  CreateReadilyTask(int num, Dictionary<int, int> filter = null)
//     {
//         HashSet<int> curTaskMergeLine = new HashSet<int>();
//         foreach (var storageTaskItem in CurTaskList)
//         {
//             if(storageTaskItem.ItemIds == null || storageTaskItem.ItemIds.Count == 0)
//                 continue;
//
//             for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
//             {
//                 int itemOneId = storageTaskItem.ItemIds[i];
//                 TableMergeItem config = GameConfigManager.Instance.GetItemConfig(itemOneId);
//                 if (config == null)
//                     continue;
//
//                 curTaskMergeLine.Add(config.in_line);
//             }
//         }
//         if (curTaskMergeLine.Count == 0)
//             return null;
//
//         List<TableMergeItem> freeItems = new List<TableMergeItem>();
//         for (int i = 0; i < MergeMainController.Instance.MergeBoard.Grids.Length; i++)
//         {
//             if (MergeMainController.Instance.MergeBoard.Grids[i].id < 0 || MergeMainController.Instance.MergeBoard.Grids[i].board == null)
//                 continue;
//
//             if (MergeMainController.Instance.MergeBoard.Grids[i].state != 1 || MergeMainController.Instance.MergeBoard.Grids[i].isProduct)
//                 continue;
//             
//             if(MergeMainController.Instance.MergeBoard.Grids[i].board.tableMergeItem == null)
//                 continue;
//             
//             if(!IsUnLockMergeLine(MergeMainController.Instance.MergeBoard.Grids[i].board.tableMergeItem.in_line))
//                 continue;
//             
//             if(curTaskMergeLine.Contains(MergeMainController.Instance.MergeBoard.Grids[i].board.tableMergeItem.in_line))
//                 continue;
//             
//             if(!DynamicTaskConfigManager.Instance.CanReadilyTask(MergeMainController.Instance.MergeBoard.Grids[i].board.tableMergeItem))
//                 continue;
//
//             if (filter != null && filter.ContainsKey(MergeMainController.Instance.MergeBoard.Grids[i].board.tableMergeItem.id))
//             {
//                 filter.Remove(MergeMainController.Instance.MergeBoard.Grids[i].board.tableMergeItem.id);
//                 continue;
//             }
//             freeItems.Add(MergeMainController.Instance.MergeBoard.Grids[i].board.tableMergeItem);
//         }
//
//         if (freeItems.Count == 0)
//             return null;
//
//         freeItems.Sort((a, b) => b.price - a.price);
//
//         List<StorageTaskItem> taskItems = new List<StorageTaskItem>();
//         for (int i = 0; i < num; i++)
//         {
//             if(freeItems.Count == 0)
//                 break;
//             
//             List<TableMergeItem> items = new List<TableMergeItem>();
//
//             if (freeItems.Count >= 2)
//             {
//                 items.Add(freeItems[0]);
//                 items.Add(freeItems[1]);
//                 
//                 freeItems.RemoveAt(0);
//                 freeItems.RemoveAt(0);
//             }
//             else
//             {
//                 items.Add(freeItems[0]);
//                 freeItems.RemoveAt(0);
//             }
//             
//            var item = AddReadilyTask(items);
//            if(item == null)
//                continue;
//            
//            taskItems.Add(item);
//         }
//         
//         return taskItems;
//     }
//     
//     public StorageTaskItem AddReadilyTask(List<TableMergeItem> items)
//     {
//         if (items == null)
//             return null;
//
//         StorageTaskItem taskItem = new StorageTaskItem();
//         taskItem.Id = _storageTaskGroup.OnlyId++;
//         taskItem.HeadIndex = RandomHeadIndex();
//         taskItem.Type = (int)TaskType.Readily;
//         taskItem.Assist = false;
//
//         int price = 0;
//         for (int i = 0; i < items.Count; i++)
//         {
//             taskItem.ItemIds.Add(items[i].id);
//             taskItem.ItemNums.Add(1);
//
//             price += items[i].price;
//             var adaptReward = DynamicTaskConfigManager.Instance.GetAdaptReward(items[i].id);
//
//             int rewardType = adaptReward.reward_type;
//             rewardType = rewardType == 0 ? (int)UserData.ResourceId.Coin : rewardType;
//             
//             if (taskItem.RewardTypes.Count > 0)
//             {
//                 int index = taskItem.RewardTypes.FindIndex((type) => type == rewardType);
//                 if (index >= 0)
//                 {
//                     taskItem.RewardNums[index] += adaptReward.reward_num;
//                 }
//                 else
//                 {
//                     taskItem.RewardTypes.Add(rewardType);
//                     taskItem.RewardNums.Add(adaptReward.reward_num);
//                 }
//             }
//             else
//             {
//                 taskItem.RewardTypes.Add(rewardType);
//                 taskItem.RewardNums.Add(adaptReward.reward_num);
//             }
//         }
//         
//         taskItem.DogCookiesNum = DynamicTaskConfigManager.Instance.ConvertDogReward(price);
//         
//         taskItem.OrgId = taskItem.Id;
//         taskItem.IsHard = false;
//         StorageTaskGroup.CurTasks.Add(taskItem);
//
//         string biParam = "";
//         for(int j = 0; j < taskItem.ItemIds.Count; j++)
//             biParam += taskItem.ItemIds[j] + ":" + taskItem.ItemNums[j] + " ";
//             
//         GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,
//             taskItem.OrgId + "_" + taskItem.Type, biParam);
//         
//         DebugUtil.Log("[Dynamic] 生成爽单任务 " + taskItem.Id);
//         
//         return taskItem;
//     }
// }