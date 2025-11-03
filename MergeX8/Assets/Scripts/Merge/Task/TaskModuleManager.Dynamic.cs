// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Decoration;
// using DragonPlus.ConfigHub.Ad;
// using DragonU3DSDK.Storage;
// using UnityEngine;
// using Decoration;
// using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
// using DragonPlus;
// using DragonU3DSDK;
// using DragonU3DSDK.Network.API;
// using Gameplay;
// using SomeWhere;
// using Random = UnityEngine.Random;
// using Utils = DragonU3DSDK.Utils;
//
// public partial class TaskModuleManager
// {
//     private List<int> _allItemsMergeLines = new List<int>();
//     private List<int> _itemOnceMergeLines = new List<int>();
//     private Dictionary<int, int> _dicMergeLineNums = new Dictionary<int, int>();
//     private List<int> _itemLessMergeLines = new List<int>();
//     
//     private const int _backNormalIndex = 50;
//     private const int _backSpecialIndex = 10;
//     
//     private bool _isActive = true;
//     private int _repeatRate = 5;
//     
//     public void InitDynamicTask()
//     {
//         InitDynamicIndex();
//         
//         CancelInvoke("InvokeUpdate");
//         InvokeRepeating("InvokeUpdate", 0, _repeatRate);
//     }
//
//     private void InitDynamicIndex()
//     {
//         if (StorageTaskGroup.DynamicNormalId > 0 && StorageTaskGroup.DynamicSpecialId > 0)
//         {
//             FixDynamicIndex();
//             return;
//         }
//
//         var rules = DynamicTaskConfigManager.Instance.GetAdaptRules(ExperenceModel.Instance.GetLevel());
//         StorageTaskGroup.DynamicNormalId = rules.normalTaskId;
//         StorageTaskGroup.DynamicSpecialId = rules.specialTaskId;
//
//         StorageTaskGroup.DynamicNormalIndex = DynamicTaskConfigManager.Instance.GetNormalIndexById(StorageTaskGroup.DynamicNormalId);
//         StorageTaskGroup.DynamicNormalIndex = Math.Max(0, StorageTaskGroup.DynamicNormalIndex);
//         
//         StorageTaskGroup.DynamicSpecialIndex = DynamicTaskConfigManager.Instance.GetSpecialIndexById(StorageTaskGroup.DynamicSpecialId);
//         StorageTaskGroup.DynamicSpecialIndex = Math.Max(0, StorageTaskGroup.DynamicSpecialIndex);
//     }
//
//     private void FixDynamicIndex()
//     {
//         if (!IsCompleteAllNormalTask())
//         {
//             int index  = DynamicTaskConfigManager.Instance.GetNormalIndexById(StorageTaskGroup.DynamicNormalId);
//             if(index > StorageTaskGroup.DynamicNormalIndex)
//                 StorageTaskGroup.DynamicNormalIndex = index+1;
//         }
//
//         if (!IsCompleteAllSpecialTask())
//         {
//             int index  = DynamicTaskConfigManager.Instance.GetSpecialIndexById(StorageTaskGroup.DynamicSpecialId);
//             if(index > StorageTaskGroup.DynamicSpecialIndex)
//                 StorageTaskGroup.DynamicSpecialIndex = index+1;
//         }
//     }
//     private bool IsCompleteAllNormalTask()
//     {
//         return DynamicTaskConfigManager.Instance.IsNormalTaskEnd(StorageTaskGroup.DynamicNormalId);
//     }
//     
//     private bool IsCompleteAllSpecialTask()
//     {
//         return DynamicTaskConfigManager.Instance.IsSpecialTaskEnd(StorageTaskGroup.DynamicSpecialId);
//     }
//
//     private List<StorageTaskItem> CompleteDynamicTask(StorageTaskItem taskItem)
//     {
//         if (!IsOpenDynamicTask())
//             return null;
//         
//         if (taskItem == null)
//             return null;
//
//         if (taskItem.Type == (int)TaskType.Easy || taskItem.Type == (int)TaskType.Seal)
//             return null;
//         
//         TableTask tableTask = TaskConfigManager.Instance.GetTableTaskConfig(taskItem.OrgId);
//         if (tableTask != null && tableTask.jumpDynamic == 0)
//             return null;
//
//         List<StorageTaskItem> taskItems = new List<StorageTaskItem>();
//         if (taskItem.PreProductId > 0)
//         {
//             var storageTask = PreProduct(taskItem.PreProductId, taskItem.Type != (int)TaskType.Special, false);
//             if (storageTask != null)
//             {
//                 taskItems.Add(storageTask);
//                 return taskItems;
//             }
//         }
//         
//         if (taskItem.Type == (int)TaskType.Special)
//         {
//             if (GetSpecialTaskNum() == 0)
//             {
//                 return DynamicAlign(false);
//             }
//             
//             if (!CanCreateTask(false))
//                 return null;
//             
//             taskItems.Add(CreateSpecialTask());
//         }
//         else
//         {
//             if (GetNormalTaskNum() == 0)
//             {
//                 return DynamicAlign(true);
//             }
//             
//             if(StorageTaskGroup.IsAttenuation)
//                 AddAttenuation();
//             
//             if (!CanCreateTask(true))
//                 return null;
//             
//             taskItems.Add(CreateNormalTask());
//         }
//         
//         //首次解锁动态任务 补全任务
//         var tasks = InitDynamicAlign();
//         if(tasks != null && tasks.Count > 0)
//             taskItems.AddRange(tasks);
//         
//         return taskItems;
//     }
//
//     public List<StorageTaskItem> InitDynamicAlign()
//     {
//         if (!IsOpenDynamicTask())
//             return null;
//         
//         if (StorageTaskGroup.IsAlignTask)
//             return null;
//         
//         StorageTaskGroup.IsAlignTask = true;
//             
//         List<StorageTaskItem> taskItems = new List<StorageTaskItem>();
//         var normalItems = DynamicAlign(true);
//         var specialItems = DynamicAlign(false);
//
//         if(normalItems != null && normalItems.Count > 0)
//             taskItems.AddRange(normalItems);
//         
//         if(specialItems != null && specialItems.Count > 0)
//             taskItems.AddRange(specialItems);
//
//         return taskItems;
//     }
//
//     private bool CanProProduct(int mergeLineId, bool isNormal, bool filterPreProduct)
//     {
//         var unlockLines = GetMergeLine(isNormal, filterPreProduct);
//         var lineUnlock = unlockLines.Find(a => a.mergeline_id == mergeLineId);
//         if (lineUnlock == null)
//             return false;
//         
//         if (lineUnlock.unlock_task == null || lineUnlock.unlock_task.Length == 0)
//             return false;
//
//         int unLockIndex = 0;
//         if(StorageTaskGroup.MergeLineUnlockIndex.ContainsKey(mergeLineId))
//             unLockIndex = StorageTaskGroup.MergeLineUnlockIndex[mergeLineId];
//         
//         if (unLockIndex >= lineUnlock.unlock_task.Length)
//             return false;
//
//         return true;
//     }
//     
//     //预产出队列
//     private StorageTaskItem PreProduct(int mergeLineId, bool isNormal, bool filterPreProduct)
//     {
//         var unlockLines = GetMergeLine(isNormal, filterPreProduct);
//         var lineUnlock = unlockLines.Find(a => a.mergeline_id == mergeLineId);
//         if (lineUnlock == null)
//             return null;
//
//         if (lineUnlock.unlock_task == null || lineUnlock.unlock_task.Length == 0)
//             return null;
//         
//         if(!StorageTaskGroup.MergeLineUnlockIndex.ContainsKey(mergeLineId))
//             StorageTaskGroup.MergeLineUnlockIndex.Add(mergeLineId, 0);
//         
//         int unLockIndex = StorageTaskGroup.MergeLineUnlockIndex[mergeLineId];
//
//         if (unLockIndex >= lineUnlock.unlock_task.Length)
//         {
//             if(StorageTaskGroup.MergeLineUnlockId.ContainsKey(mergeLineId))
//                 StorageTaskGroup.MergeLineUnlockId.Remove(mergeLineId);
//             return null;
//         }
//         
//         if(!StorageTaskGroup.MergeLineUnlockId.ContainsKey(mergeLineId))
//             StorageTaskGroup.MergeLineUnlockId.Add(mergeLineId, mergeLineId);
//         
//         int taskId = lineUnlock.unlock_task[unLockIndex];
//         StorageTaskGroup.MergeLineUnlockIndex[mergeLineId]++;
//         if(StorageTaskGroup.MergeLineUnlockIndex[mergeLineId] >= lineUnlock.unlock_task.Length)
//             StorageTaskGroup.MergeLineUnlockId.Remove(mergeLineId);
//             
//         var unLockTaskTable = TaskConfigManager.Instance.GetTableTaskConfig(taskId);
//         if (unLockTaskTable == null)
//             return null;
//         
//         StorageTaskItem unLockTask = AddTask(unLockTaskTable);
//         unLockTask.PreProductId = mergeLineId;
//         
//         string biParam = "";
//         for(int j = 0; j < unLockTask.ItemIds.Count; j++)
//             biParam += unLockTask.ItemIds[j] + ":" + unLockTask.ItemNums[j] + " ";
//             
//         GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,
//             unLockTask.OrgId + "_" + unLockTask.Type, biParam);
//         
//         DebugUtil.Log("[Dynamic] 生成预产出任务 " + unLockTask.Id);
//         return unLockTask;
//     }
//     
//     //预产出队列
//     private StorageTaskItem PreProduct(List<TableMergeLineUnlock> lineUnlocks, bool isNormal)
//     {
//         if (lineUnlocks == null)
//             return null;
//         
//         foreach (var tableMergeLineUnlock in lineUnlocks)
//         {
//             if(tableMergeLineUnlock.unlock_task == null || tableMergeLineUnlock.unlock_task.Length == 0)
//                 continue;
//             
//             if(StorageTaskGroup.MergeLineUnlockId.ContainsKey(tableMergeLineUnlock.mergeline_id))
//                 continue;
//
//             StorageTaskItem preTaskItem = PreProduct(tableMergeLineUnlock.mergeline_id, isNormal, true);
//             if (preTaskItem != null)
//                 return preTaskItem;
//         }
//
//         return null;
//     }
//     
//     private StorageTaskItem CreateNormalTask()
//     {
//         UserTaskType taskType = AdConfigHandle.Instance.GetUserTaskType();
//         List<TableFreeNormalTask> normalTasks;
//         if (taskType == UserTaskType.Free)
//         {
//             normalTasks = DynamicTaskConfigManager.Instance._freeNomalTasks;
//         }
//         else
//         {
//             normalTasks = DynamicTaskConfigManager.Instance._payNomalTasks;
//         }
//
//         
//         if (StorageTaskGroup.DynamicNormalIndex >= normalTasks.Count)
//         {
//             StorageTaskGroup.DynamicNormalIndex = normalTasks.Count;
//             StorageTaskGroup.DynamicNormalIndex = Math.Max(0, StorageTaskGroup.DynamicNormalIndex-_backNormalIndex);
//         }
//        
//         
//         var taskConfig = normalTasks[StorageTaskGroup.DynamicNormalIndex];
//         //taskConfig = DynamicTaskConfigManager.Instance._freeNomalTasks.Find(a => a.id == 1000037);
//         //taskConfig = normalTasks[13];
//         
//         var hardCacheTask = CreateHardTaskByCache();
//         if (hardCacheTask != null)
//             return hardCacheTask;
//         
//         if (taskConfig.ishardTask == 1)
//         {
//             if (!CanCreateHardTask())
//             {
//                 StorageTaskGroup.HardTaskCacheQueue.Add(taskConfig.id);
//                 StorageTaskGroup.DynamicNormalIndex++;
//                 DebugUtil.Log("[Dynamic] 困难任务不可生成  [缓存] " + taskConfig.id);
//                 return CreateNormalTask();
//             }
//         }
//         
//         return CreateNormalTask(taskConfig);
//     }
//
//     public StorageTaskItem CreateNormalTask(TableFreeNormalTask taskConfig)
//     {
//         var unlockLines = GetMergeLine(true);
//         // for (int i = 0; i < unlockLines.Count; i++)
//         // {
//         //     if (unlockLines[i].mergeline_id != 20412)
//         //     {
//         //         unlockLines.RemoveAt(i);
//         //         i--;
//         //     }
//         // }
//         var preTask = PreProduct(unlockLines, true);
//         if (preTask != null)
//             return preTask;
//         
//         StorageTaskGroup.DynamicNormalId = Math.Max(taskConfig.id, StorageTaskGroup.DynamicNormalId);
//         StorageTaskItem heardTask = DynamicHardTask(taskConfig);
//         if (heardTask != null)
//         {
//             StorageTaskGroup.DynamicNormalIndex++;
//             return heardTask;
//         }
//         
//         var mergeWeight = GetMergeLineWeight(unlockLines);
//         if (mergeWeight == null || mergeWeight.Count == 0)
//         {
//             StorageTaskGroup.DynamicNormalIndex++;
//             return DynamicEasyTask(); 
//         }
//         
//         StorageTaskGroup.DynamicNormalIndex++;
//         for (int i = 0; i < mergeWeight.Count; i++)
//         {
//             if (mergeWeight[i].min_price <= taskConfig.price * 100)
//                 continue;
//             
//             mergeWeight.RemoveAt(i);
//             i--;
//         }
//         if (mergeWeight == null || mergeWeight.Count == 0)
//         {
//             return DynamicEasyTask();
//         }
//
//         var copyMergeLine = RepeatedFiltration(mergeWeight, true);
//         if (copyMergeLine == null || copyMergeLine.Count == 0)
//         {
//             return DynamicEasyTask();
//         }
//         
//         foreach (var tableMergeLineWeight in copyMergeLine)
//         {
//             if(!CanProProduct(tableMergeLineWeight.mergeline_id, true, true))
//                 continue;
//             
//             var preTaskItem = PreProduct(tableMergeLineWeight.mergeline_id, true, true);
//             if (preTaskItem != null)
//                 return preTaskItem;
//         }
//         return FillNormalItem(taskConfig, mergeWeight, null);
//     }
//     private StorageTaskItem DynamicEasyTask()
//     {
//         var randomList = TaskConfigManager.Instance.GetEasyTaskList(ExperenceModel.Instance.GetLevel());
//         StorageTaskItem taskItem = AddEasyTask(randomList.RandomPickOne());
//         if (taskItem != null)
//         {
//             taskItem.Id = StorageTaskGroup.OnlyId++;
//             taskItem.Type = (int)TaskType.Dynamic;
//         }
//         
//         DebugUtil.Log("[Dynamic] 生成简单 " + taskItem.Id);
//         GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,taskItem.OrgId + "_" + taskItem.Type);
//
//         return taskItem;
//     }
//
//     private StorageTaskItem DynamicHardTask(TableFreeNormalTask taskConfig)
//     {
//         if (taskConfig.ishardTask != 1 || taskConfig.hard_line == 0)
//             return null;
//
//         if (!IsUnlockMergeLine(DynamicTaskConfigManager.Instance._mergeLineUnlocks.Find(a => a.mergeline_id == taskConfig.hard_line),true))
//             return null;
//         
//         if (StorageTaskGroup.MergeLineUnlockId.ContainsKey(taskConfig.hard_line))
//             return null;
//         
//         foreach (var storageTaskItem in CurTaskList)
//         {
//             if(!storageTaskItem.IsHard)
//                 continue;
//             
//             if(storageTaskItem.ItemIds == null || storageTaskItem.ItemIds.Count == 0)
//                 continue;
//             
//             int itemOneId = storageTaskItem.ItemIds[0];
//             TableMergeItem config = GameConfigManager.Instance.GetItemConfig(itemOneId);
//             if (config == null)
//                 continue;
//
//             if (config.in_line == taskConfig.hard_line)
//                 return null;
//         }
//
//         var weights = DynamicTaskConfigManager.Instance.GetMergeLineWeight(ExperenceModel.Instance.GetLevel());
//         var hardLine = weights.Find(a => a.mergeline_id == taskConfig.hard_line);
//         if (hardLine == null)
//             return null;
//         
//         var unlockLines = GetMergeLine(true);
//         var mergeWeight = GetMergeLineWeight(unlockLines);
//         if (mergeWeight == null || mergeWeight.Count == 0)
//             return null;
//         return FillNormalItem(taskConfig, mergeWeight, hardLine);
//     }
//     private StorageTaskItem CreateSpecialTask()
//     {
//         UserTaskType taskType = AdConfigHandle.Instance.GetUserTaskType();
//         List<TableFreeSpecialTask> specialTasks;
//         if (taskType == UserTaskType.Free)
//         {
//             specialTasks = DynamicTaskConfigManager.Instance._freeSpecialTasks;
//         }
//         else
//         {
//             specialTasks = DynamicTaskConfigManager.Instance._paySpecialTasks;
//         }
//
//         var unlockLines = GetMergeLine(false);
//         var mergeWeight = GetMergeLineWeight(unlockLines);
//         if (mergeWeight == null || mergeWeight.Count == 0)
//             return null;
//         
//         var preTask = PreProduct(unlockLines, false);
//         if (preTask != null)
//             return preTask;
//         
//         if (StorageTaskGroup.DynamicSpecialIndex >= specialTasks.Count)
//         {
//             StorageTaskGroup.DynamicSpecialIndex = specialTasks.Count;
//             StorageTaskGroup.DynamicSpecialIndex = Math.Max(0, StorageTaskGroup.DynamicSpecialIndex-_backSpecialIndex);
//         }
//         
//         var taskConfig = specialTasks[StorageTaskGroup.DynamicSpecialIndex++];
//         StorageTaskGroup.DynamicSpecialId = Math.Max(taskConfig.id, StorageTaskGroup.DynamicSpecialId);
//        
//         var copyMergeLine = RepeatedFiltration(mergeWeight, false);
//         if (copyMergeLine == null || copyMergeLine.Count == 0)
//             return null;
//         
//         List<int> weightNum = new List<int>();
//         copyMergeLine.ForEach(a=>weightNum.Add(a.weight));
//         int randomIndex = CommonUtils.RandomIndexByWeight(weightNum);
//         randomIndex = Math.Max(randomIndex, 0);
//         TableMergeLineWeight mergeLine = copyMergeLine[randomIndex];
//         var preTaskItem = PreProduct(mergeLine.mergeline_id, true, true);
//         if (preTaskItem != null)
//             return preTaskItem;
//
//         return FillSpecialItem(taskConfig, copyMergeLine, mergeLine);
//     }
//
//     private List<TableMergeLineWeight> RepeatedFiltration(List<TableMergeLineWeight> mergeWeight, bool isNormal, StorageTaskItem taskItem = null)
//     {
//         CalculationCurTaskMergeLine(isNormal, taskItem);
//
//         var copyMergeLine = new List<TableMergeLineWeight>(mergeWeight);
//         for (int i = 0; i < copyMergeLine.Count; i++)
//         {
//             if(!_allItemsMergeLines.Contains(copyMergeLine[i].mergeline_id))
//                 continue;
//             
//             if(GetHaveMergeLineNum(copyMergeLine[i].mergeline_id) < copyMergeLine[i].appear_times)
//                 continue;
//             
//             copyMergeLine.RemoveAt(i);
//             i--;
//         }
//
//         if (copyMergeLine.Count > 0)
//             return copyMergeLine;
//         
//  
//         copyMergeLine.Clear();
//         copyMergeLine.AddRange(mergeWeight);
//         for (int i = 0; i < copyMergeLine.Count; i++)
//         {
//             if(!_itemOnceMergeLines.Contains(copyMergeLine[i].mergeline_id))
//                 continue;
//         
//             copyMergeLine.RemoveAt(i);
//             i--;
//         }
//         
//         if (copyMergeLine.Count > 0)
//             return copyMergeLine;
//         
//   
//         copyMergeLine.Clear();
//         var mergeLineWeightsWeight = DynamicTaskConfigManager.Instance.GetMergeLineWeight(ExperenceModel.Instance.GetLevel());
//         foreach (var mergeLineId in _itemLessMergeLines)
//         {
//             var weight = mergeLineWeightsWeight.Find(a => a.mergeline_id == mergeLineId);
//             if(weight != null)
//                 copyMergeLine.Add(weight);
//         }
//
//         return copyMergeLine;
//     }
//     
//     private StorageTaskItem FillSpecialItem(TableFreeSpecialTask taskConfig,  List<TableMergeLineWeight> mergeLineWeights, TableMergeLineWeight mergeLine)
//     {
//         if (mergeLineWeights == null || mergeLineWeights.Count == 0)
//             return null;
//         
//         int leftPrice = taskConfig.price;
//         StorageTaskItem taskItem = new StorageTaskItem();
//         taskItem.Id = IsCompleteAllSpecialTask() ? StorageTaskGroup.OnlyId++ : taskConfig.id;
//         taskItem.OrgId = taskConfig.id;
//         taskItem.HeadIndex = RandomHeadIndex();
//         taskItem.DogCookiesNum = taskConfig.dogCookies;
//         taskItem.Assist = false;
//         taskItem.IsHard = taskConfig.ishardTask == 1;
//         taskItem.Type = (int)TaskType.Special;
//
//         int minLevel = (int)Mathf.Log(mergeLine.min_price / mergeLine.lv1_price, 2) + 1;
//         float itemPrice = mergeLine.lv1_price / 100f;
//         float price = 1.0f * leftPrice / itemPrice;
//         int level = (int)Mathf.Log(price, 2)+1;
//         level = Math.Max(level, minLevel);
//         level = Math.Min(level, mergeLine.max_level);
//         
//         TableMergeLine mergeItem = GameConfigManager.Instance.GetMergeLine(mergeLine.mergeline_id);
//         level = Math.Min(level, mergeItem.output.Length);
//
//         taskItem.ItemNums.Add(1);
//         taskItem.ItemIds.Add(mergeItem.output[level-1]);
//
//         FillReward(taskItem, false);
//         
//         string biParam = "";
//         for(int j = 0; j < taskItem.ItemIds.Count; j++)
//             biParam += taskItem.ItemIds[j] + ":" + taskItem.ItemNums[j] + " ";
//             
//         GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,taskItem.OrgId + "_" + taskItem.Type, biParam);
//
//         DebugUtil.Log("[Dynamic] 生成特殊任务 " + taskItem.Id + "\t items: " + biParam + "\t type="+taskItem.RewardTypes[0]+"\tnum="+taskItem.RewardNums[0]);
//         
//         StorageTaskGroup.CurTasks.Add(taskItem);
//         return taskItem;
//     }
//
//     private StorageTaskItem FillNormalItem(TableFreeNormalTask taskConfig,  List<TableMergeLineWeight> unlockMergeLineWeights, TableMergeLineWeight appointLine)
//     {
//         if (unlockMergeLineWeights == null || unlockMergeLineWeights.Count == 0)
//             return DynamicEasyTask();
//         
//         StorageTaskItem taskItem = new StorageTaskItem();
//         taskItem.Id = IsCompleteAllNormalTask() ? StorageTaskGroup.OnlyId++ : taskConfig.id;
//         taskItem.OrgId = taskConfig.id;
//         taskItem.HeadIndex = RandomHeadIndex();
//         taskItem.DogCookiesNum = taskConfig.dogCookies;
//         taskItem.Assist = taskConfig.assist;
//         taskItem.IsHard = taskConfig.ishardTask == 1;
//         taskItem.Type = (int)TaskType.Dynamic;
//
//         if (CanCreateTriItems())
//         {
//             if (Random.Range(0, 100) < DynamicTaskConfigManager.Instance._attenuations[0].tritask_weight)
//             {
//                 FillThreeItem(ref taskItem, taskConfig, unlockMergeLineWeights, appointLine);
//             }
//             else
//             {
//                 FillTwoItem(ref taskItem, taskConfig, unlockMergeLineWeights, appointLine);
//             }
//         }
//         else
//         {
//             FillTwoItem(ref taskItem, taskConfig, unlockMergeLineWeights, appointLine);
//         }
//
//         if (taskItem.ItemIds.Count == 0)
//         {
//             return DynamicEasyTask();
//         }
//         else
//         {
//             FillReward(taskItem, true);
//         }
//         
//         string biParam = "";
//         for(int j = 0; j < taskItem.ItemIds.Count; j++)
//             biParam += taskItem.ItemIds[j] + ":" + taskItem.ItemNums[j] + " ";
//             
//         GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,taskItem.OrgId + "_" + taskItem.Type, biParam);
//
//         if (taskConfig.ishardTask == 1)
//         {
//             DebugUtil.Log("[Dynamic] 生成困难任务 " + taskItem.Id + "\t items: " + biParam + "\t type="+taskItem.RewardTypes[0]+"\tnum="+taskItem.RewardNums[0]);
//         }
//         else
//         {
//             DebugUtil.Log("[Dynamic] 生成普通任务 " + taskItem.Id + "\t items: " + biParam + "\t type="+taskItem.RewardTypes[0]+"\tnum="+taskItem.RewardNums[0]);
//         }
//
//         StorageTaskGroup.CurTasks.Add(taskItem);
//         return taskItem;
//     }
//
//     private TableMergeLineWeight RandomMergeLineWeight(List<TableMergeLineWeight> unlockMergeLineWeights, bool isNormal, bool filterPreProduct, StorageTaskItem taskItem = null)
//     {
//         if (unlockMergeLineWeights == null || unlockMergeLineWeights.Count == 0)
//             return null;
//         
//         var copyMergeLine = RepeatedFiltration(unlockMergeLineWeights, true, taskItem);
//         if (copyMergeLine == null || copyMergeLine.Count == 0)
//             return null;
//
//         for (int i = 0; i < copyMergeLine.Count; i++)
//         {
//             if(!CanProProduct(copyMergeLine[i].mergeline_id, true, true))
//                 continue;
//                 
//             copyMergeLine.RemoveAt(i);
//             i--;
//         }
//         if (copyMergeLine == null || copyMergeLine.Count == 0)
//             return null;
//             
//         List<int> weightNum = new List<int>();
//         copyMergeLine.ForEach(a=>weightNum.Add(a.weight));
//         int randomIndex = CommonUtils.RandomIndexByWeight(weightNum);
//         randomIndex = Math.Max(randomIndex, 0);
//         return  copyMergeLine[randomIndex];
//     }
//     
//     private void FillTwoItem(ref StorageTaskItem taskItem, TableFreeNormalTask taskConfig,  List<TableMergeLineWeight> unlockMergeLineWeights,  TableMergeLineWeight appointLine)
//     {
//         TableMergeLineWeight mergeLine = appointLine;
//         if(appointLine == null)
//             mergeLine = RandomMergeLineWeight(unlockMergeLineWeights, true, true, null);
//         if(mergeLine == null)
//             return;
//
//         int leftPrice = taskConfig.price;
//         int fullNum = 0;
//         while (leftPrice > 2 && fullNum < 2)
//         {
//             if(mergeLine == null)
//                 break;
//             
//             int minLevel = (int)Mathf.Log(mergeLine.min_price / mergeLine.lv1_price, 2) + 1;
//             float itemPrice = mergeLine.lv1_price / 100f;
//             float price = 1.0f * leftPrice / itemPrice;
//             int level = (int)Mathf.Log(price, 2)+1;
//             level = Math.Max(level, minLevel);
//             level = Math.Min(level, mergeLine.max_level);
//             
//             TableMergeLine mergeItem = GameConfigManager.Instance.GetMergeLine(mergeLine.mergeline_id);
//             level = Math.Min(level, mergeItem.output.Length);
//
//             leftPrice -= (int)(Math.Pow(2, level-1) * itemPrice); 
//             taskItem.ItemNums.Add(1);
//             taskItem.ItemIds.Add(mergeItem.output[level-1]);
//
//             fullNum++;
//             
//             if(leftPrice <= 2)
//                 break;
//
//             if (Random.Range(1, 100 + 1) < mergeLine.repetitioin_rate)
//             {
//                 if(mergeLine.min_price / 100 > leftPrice)
//                     break;
//                 
//                 //DebugUtil.LogError("------第二个物品 和 第一个物品使用相同的链");
//                 continue;
//             }
//
//             mergeLine = RandomMergeLineWeight(unlockMergeLineWeights, true, true, taskItem);
//         }
//     }
//
//     private void FillThreeItem(ref StorageTaskItem taskItem, TableFreeNormalTask taskConfig, List<TableMergeLineWeight> unlockMergeLineWeights, TableMergeLineWeight appointLine, int debugIndex = -1)
//     {
//         List<int> weightNum = new List<int>();
//         DynamicTaskConfigManager.Instance._tritaskRate.ForEach(a=>weightNum.Add(a.weight));
//         int index = CommonUtils.RandomIndexByWeight(weightNum);
//         index = debugIndex >= 0 ? debugIndex : index;
//         var rate = DynamicTaskConfigManager.Instance._tritaskRate[index];
//
//         var mergeLine = appointLine;
//         if(mergeLine == null)
//             mergeLine = RandomMergeLineWeight(unlockMergeLineWeights, true, true, null);
//         
//         var firstLine = mergeLine;
//         
//         for (int i = 0; i < rate.rate.Length; i++)
//         {
//             if(mergeLine == null)
//                 break;
//             
//             int leftPrice = (int)(rate.rate[i]/100f * taskConfig.price);
//             
//             if(leftPrice <= 2)
//                 continue;
//             
//             int minLevel = (int)Mathf.Log(mergeLine.min_price / mergeLine.lv1_price, 2) + 1;
//             float itemPrice = mergeLine.lv1_price / 100f;
//             float price = 1.0f * leftPrice / itemPrice;
//             int level = (int)Mathf.Log(price, 2)+1;
//             level = Math.Max(level, minLevel);
//             level = Math.Min(level, mergeLine.max_level);
//             
//             TableMergeLine mergeItem = GameConfigManager.Instance.GetMergeLine(mergeLine.mergeline_id);
//             level = Math.Min(level, mergeItem.output.Length);
//
//             taskItem.ItemNums.Add(1);
//             taskItem.ItemIds.Add(mergeItem.output[level-1]);
//
//             if (Random.Range(1, 100 + 1) < mergeLine.repetitioin_rate)
//             {
//                 mergeLine = firstLine;
//                 continue;
//             }
//             
//             mergeLine = RandomMergeLineWeight(unlockMergeLineWeights, true, true, taskItem);
//         }
//     }
//
//     private void FillReward(StorageTaskItem item, bool isNormal)
//     {
//         if(item == null)
//             return;
//
//         int rewardType = 0;
//         int rewardNum = 0;
//
//         for (int i = 0; i < item.ItemIds.Count; i++)
//         {
//             var adaptReward = DynamicTaskConfigManager.Instance.GetAdaptReward(item.ItemIds[i]);
//
//             if (rewardType == 0)
//                 rewardType = adaptReward.reward_type;
//
//             rewardNum += adaptReward.reward_num;
//         }
//
//         rewardType = rewardType == 0 ? (int)UserData.ResourceId.Coin : rewardType;
//         if (isNormal)
//         {
//             if(IsCompleteAllNormalTask() && !RecoverCoinModel.Instance.IsStart())
//                 rewardNum /= 2;
//         }
//         else
//         {
//             if(IsCompleteAllSpecialTask() && !RecoverCoinModel.Instance.IsStart())
//                 rewardNum /= 2;
//         }
//         
//         rewardNum = rewardNum <= 0 ? 1 : rewardNum;
//         
//         item.RewardTypes.Add(rewardType);
//         item.RewardNums.Add(rewardNum);
//     }
//     private List<TableMergeLineUnlock> GetMergeLine(bool isNormal, bool filterPreProduct = true)
//     {
//         List<TableMergeLineUnlock> unlocks = new List<TableMergeLineUnlock>();
//         
//         foreach (var unlock in DynamicTaskConfigManager.Instance._mergeLineUnlocks)
//         {
//              if(IsUnlockMergeLine(unlock, isNormal))
//                  unlocks.Add(unlock);
//         }
//
//         if (filterPreProduct)
//         {
//             for (int i = 0; i < unlocks.Count; i++)
//             {
//                 if(!StorageTaskGroup.MergeLineUnlockId.ContainsKey(unlocks[i].mergeline_id))
//                     continue;
//             
//                 unlocks.RemoveAt(i);
//                 i--;
//             }
//         }
//         return unlocks;
//     }
//
//     private bool IsUnLockMergeLine(int id)
//     {
//         return IsUnlockMergeLine(DynamicTaskConfigManager.Instance._mergeLineUnlocks.Find(a => a.mergeline_id == id),
//             true, false);
//     }
//     
//     private bool IsUnlockMergeLine(TableMergeLineUnlock unlock, bool isNormal, bool checkTaskType = true)
//     {
//         if (unlock == null)
//             return false;
//
//         if (checkTaskType)
//         {
//             if (isNormal)
//             {
//                 if(unlock.task_type != 1)
//                     return false;
//             }
//             else
//             {
//                 if(unlock.task_type != 2)
//                     return false;
//             }
//         }
//
//         TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(unlock.unlock_mergeid);
//         if (mergeItem == null)
//             return false;
//
//         TableMergeLine mergeLine = GameConfigManager.Instance.GetMergeLine(mergeItem.in_line);
//         if (mergeLine == null)
//             return false;
//
//         if (mergeLine.output == null)
//             return false;
//
//         bool isFind = false;
//         for (int i = 0; i < mergeLine.output.Length; i++)
//         {
//             if(!isFind)
//                 isFind = mergeLine.output[i] == unlock.unlock_mergeid;
//                 
//             if(!isFind)
//                 continue;
//
//             if (MergeManager.Instance.IsGetItem(mergeLine.output[i]) &&
//                 DecoManager.Instance.IsOwnedNode(unlock.unlock_decoid))
//                 return true;
//         }
//
//         return false;
//     }
//
//     private List<TableMergeLineWeight> GetMergeLineWeight(List<TableMergeLineUnlock> unlocks)
//     {
//         var weights = DynamicTaskConfigManager.Instance.GetMergeLineWeight(ExperenceModel.Instance.GetLevel());
//         var mergeWeights = new List<TableMergeLineWeight>();
//         if (unlocks != null)
//         {
//             foreach (var tableMergeLineUnlock in unlocks)
//             {
//                 var config = weights.Find(a => a.mergeline_id == tableMergeLineUnlock.mergeline_id);
//                 if(config == null)
//                     continue;
//             
//                 mergeWeights.Add(config);
//             }
//         }
//
//         return mergeWeights;
//     }
//
//     private void CalculationCurTaskMergeLine(bool isNormal, StorageTaskItem taskItem = null)
//     {
//         _allItemsMergeLines.Clear();
//         _itemOnceMergeLines.Clear();
//         _dicMergeLineNums.Clear();
//         _itemLessMergeLines.Clear();
//         
//         foreach (var storageTaskItem in CurTaskList)
//         {
//             int index = 0;
//             foreach (var itemId in storageTaskItem.ItemIds)
//             {
//                 if(itemId <= 0)
//                     continue;
//
//                 TableMergeItem config = GameConfigManager.Instance.GetItemConfig(itemId);
//                 if(config == null)
//                     continue;
//
//                 int inLine = config.in_line;
//                 if(!_allItemsMergeLines.Contains(inLine))
//                     _allItemsMergeLines.Add(inLine);
//
//                 if (index == 0)
//                 {
//                     if(!_itemOnceMergeLines.Contains(inLine))
//                         _itemOnceMergeLines.Add(inLine);
//                 }
//
//                 if (_dicMergeLineNums.ContainsKey(inLine))
//                     _dicMergeLineNums[inLine]++;
//                 else
//                     _dicMergeLineNums.Add(inLine, 1);
//                 
//                 index++;
//             }
//         }
//
//         if (taskItem != null && taskItem.ItemIds != null && taskItem.ItemIds.Count > 0)
//         {
//             int index = 0;
//             foreach (var itemId in taskItem.ItemIds)
//             {
//                 if(itemId <= 0)
//                     continue;
//
//                 TableMergeItem config = GameConfigManager.Instance.GetItemConfig(itemId);
//                 if(config == null)
//                     continue;
//
//                 int inLine = config.in_line;
//                 if(!_allItemsMergeLines.Contains(inLine))
//                     _allItemsMergeLines.Add(inLine);
//
//                 if (index == 0)
//                 {
//                     if(!_itemOnceMergeLines.Contains(inLine))
//                         _itemOnceMergeLines.Add(inLine);
//                 }
//
//                 if (_dicMergeLineNums.ContainsKey(inLine))
//                     _dicMergeLineNums[inLine]++;
//                 else
//                     _dicMergeLineNums.Add(inLine, 1);
//             
//                 index++;
//             }
//         }
//         
//         Dictionary<int, int> sortedByKey = _dicMergeLineNums.OrderBy(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
//
//         _dicMergeLineNums = sortedByKey;
//
//         int num = int.MaxValue;
//         foreach (var kv in _dicMergeLineNums)
//         {
//             if(StorageTaskGroup.MergeLineUnlockId.ContainsKey(kv.Key))
//                 continue;
//
//             var unlockMerge = DynamicTaskConfigManager.Instance._mergeLineUnlocks.Find(a => a.mergeline_id == kv.Key);
//             if(unlockMerge == null)
//                 continue;
//
//             if (isNormal)
//             {
//                 if(unlockMerge.task_type != 1)
//                     continue;
//             }
//             else
//             {
//                 if(unlockMerge.task_type != 2)
//                     continue;
//             }
//             
//             if (kv.Value <= num)
//             {
//                 num = kv.Value;
//                 _itemLessMergeLines.Add(kv.Key);
//             }
//             else
//             {
//                 break;
//             }
//         }
//     }
//
//     private int GetHaveMergeLineNum(int mergeId)
//     {
//         if (!_dicMergeLineNums.ContainsKey(mergeId))
//             return 0;
//
//         return _dicMergeLineNums[mergeId];
//     }
//     
//     public int GetNormalTaskNum(bool isFilterHard = false)
//     {
//         int taskNum = 0;
//         foreach (var taskItem in CurTaskList)
//         {
//             if (taskItem.Type == (int)TaskType.Normal || taskItem.Type == (int)TaskType.Random ||
//                 taskItem.Type == (int)TaskType.Dynamic)
//             {
//                 if(isFilterHard && taskItem.IsHard)
//                     continue;
//                 
//                 taskNum++;
//             }
//         }
//
//         return taskNum;
//     }
//     
//     public int GetHardTaskNum()
//     {
//         int taskNum = 0;
//         foreach (var taskItem in CurTaskList)
//         {
//             if (taskItem.Type == (int)TaskType.Normal || taskItem.Type == (int)TaskType.Random ||taskItem.Type == (int)TaskType.Dynamic)
//             {
//                 if(taskItem.IsHard)
//                     taskNum++;
//             }
//         }
//
//         return taskNum;
//     }
//
//     public bool CanCreateHardTask()
//     {
//         var taskNum = DynamicTaskConfigManager.Instance.GetTaskNum(ExperenceModel.Instance.GetLevel());
//
//         return GetHardTaskNum() < taskNum.hard_task_max_num;
//     }
//     
//     public int GetSpecialTaskNum()
//     {
//         int taskNum = 0;
//         foreach (var taskItem in CurTaskList)
//         {
//             if (taskItem.Type == (int)TaskType.Special)
//                 taskNum++;
//         }
//
//         return taskNum;
//     }
//     
//     private void InvokeUpdate()
//     {
//         if(!_isActive)
//             return;
//         
//         CreateEasyTask();
//         CreateReadilyTask();
//         
//         if (!IsOpenDynamicTask())
//             return;
//         
//         if(ExperenceModel.Instance.GetLevel() < DynamicTaskConfigManager.Instance._attenuations[0].unlock_level)
//             return;
//         
//         StorageTaskGroup.PlayGameTime += _repeatRate;
//         if (StorageTaskGroup.IsAttenuation)
//         {
//             long diffTime = (long)APIManager.Instance.GetServerTime() / 1000 - StorageTaskGroup.AlignTime;
//             StorageTaskGroup.AlignTime = (long)APIManager.Instance.GetServerTime() / 1000;
//             StorageTaskGroup.ReplenishTime += diffTime;
//         }
//
//         if (StorageTaskGroup.ReplenishTime >= DynamicTaskConfigManager.Instance._attenuations[0].replenish_time * 60)
//         {
//             DynamicAlign();
//         }
//
//         if (StorageTaskGroup.PlayGameTime >= DynamicTaskConfigManager.Instance._attenuations[0].attenuation_time * 60 || StorageTaskGroup.DailyCompleteNum >= DynamicTaskConfigManager.Instance._attenuations[0].attenuation_task)
//         {
//             if (!StorageTaskGroup.IsAttenuation)
//             {
//                 StorageTaskGroup.IsAttenuation = true;
//                 StorageTaskGroup.AttenuationNum = 0;
//                 StorageTaskGroup.AlignTime = (long)APIManager.Instance.GetServerTime() / 1000;
//             }
//         }
//     }
//
//     public bool IsAttenuation()
//     {
//         return StorageTaskGroup.IsAttenuation;
//     }
//
//     public void AddAttenuation()
//     {
//         StorageTaskGroup.AttenuationNum++;
//     }
//
//     public int TriItemTaskNum()
//     {
//         int taskNum = 0;
//         foreach (var storageTaskItem in CurTaskList)
//         {
//             if(storageTaskItem.ItemIds == null || storageTaskItem.ItemIds.Count == 0)
//                 continue;
//             
//             if (storageTaskItem.ItemIds.Count >= 3)
//                 taskNum++;
//         }
//
//         return taskNum;
//     }
//
//     public bool CanCreateTriItems()
//     {
//         var taskNum = DynamicTaskConfigManager.Instance.GetTaskNum(ExperenceModel.Instance.GetLevel());
//
//         return taskNum.tritask_max_num > TriItemTaskNum();
//     }
//     
//     public void DynamicAlign()
//     {       
//         if (!IsOpenDynamicTask())
//             return;
//         
//         List<StorageTaskItem> taskItems = new List<StorageTaskItem>();
//         var normalItems = DynamicAlign(true);
//         var specialItems = DynamicAlign(false);
//
//         if(normalItems != null && normalItems.Count > 0)
//             taskItems.AddRange(normalItems);
//         
//         if(specialItems != null && specialItems.Count > 0)
//             taskItems.AddRange(specialItems);
//         
//         if (MergeTaskTipsController.Instance.gameObject.activeInHierarchy && taskItems.Count > 0)
//         {
//             MergeTaskTipsController.Instance.RefreshTask(taskItems, () =>
//             {
//                 EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
//                 MergeMainController.Instance?.UpdateTaskRedPoint();
//             });
//         }
//
//         InitAttenuationData();
//     }
//
//     private void InitAttenuationData()
//     {
//         StorageTaskGroup.PlayGameTime = 0;
//         StorageTaskGroup.ReplenishTime = 0;
//         StorageTaskGroup.AttenuationNum = 0;
//         StorageTaskGroup.IsAttenuation = false;
//         StorageTaskGroup.AlignTime = 0;
//         StorageTaskGroup.DailyCompleteNum = 0;
//     }
//
//     private bool IsOpenDynamicTask()
//     {
//         return IsCompleteTask(DynamicTaskConfigManager.Instance._attenuations[0].dynamic_open);
//     }
//     
//     private List<StorageTaskItem> DynamicAlign(bool isNormal)
//     {
//         int taskNum = GetCanCreateTaskNum(isNormal);
//         if (taskNum <= 0)
//             return null;
//
//         List<StorageTaskItem> taskItems = new List<StorageTaskItem>();
//         for (int i = 0; i < taskNum; i++)
//         {
//             var taskItem = isNormal ? CreateNormalTask() : CreateSpecialTask();
//             if(taskItem != null)
//                 taskItems.Add(taskItem);
//         }
//
//         return taskItems;
//     }
//
//     public int GetCanCreateTaskNum(bool isNormal)
//     {
//         var taskNum = DynamicTaskConfigManager.Instance.GetTaskNum(ExperenceModel.Instance.GetLevel());
//         int haveNum = isNormal ? GetNormalTaskNum() : GetSpecialTaskNum();
//         
//         int diffNum = (isNormal ? taskNum.normal_task_num : taskNum.special_task_num) - haveNum;
//         if(diffNum <= 0)
//             return 0;
//
//         return diffNum;
//     }
//
//     public bool CanCreateTask(bool isNormal)
//     {
//         var taskNum = DynamicTaskConfigManager.Instance.GetTaskNum(ExperenceModel.Instance.GetLevel());
//         int haveNum = isNormal ? GetNormalTaskNum() : GetSpecialTaskNum();
//
//         int maxNum = isNormal ? taskNum.normal_task_num : taskNum.special_task_num;
//
//         if (isNormal)
//         {
//             maxNum -= StorageTaskGroup.AttenuationNum;
//             maxNum = Mathf.Max(1, maxNum);
//             
//             return maxNum > haveNum;
//         }
//
//         return haveNum < maxNum;
//     }
//     
//     private void OnApplicationPause(bool pause)
//     {
//         _isActive = !pause;
//     }
//
//     public void DebugCreateDynamicTask(int id, int index)
//     {
//         UserTaskType taskType = AdConfigHandle.Instance.GetUserTaskType();
//         List<TableFreeNormalTask> normalTasks;
//         if (taskType == UserTaskType.Free)
//         {
//             normalTasks = DynamicTaskConfigManager.Instance._freeNomalTasks;
//         }
//         else
//         {
//             normalTasks = DynamicTaskConfigManager.Instance._payNomalTasks;
//         }
//
//         var taskConfig = normalTasks.Find(a => a.id == id);
//         if(taskConfig == null)
//             return;
//         
//         StorageTaskItem taskItem = new StorageTaskItem();
//         taskItem.Id = IsCompleteAllNormalTask() ? StorageTaskGroup.OnlyId++ : taskConfig.id;
//         taskItem.OrgId = taskConfig.id;
//         taskItem.HeadIndex = RandomHeadIndex();
//         taskItem.DogCookiesNum = taskConfig.dogCookies;
//         taskItem.Assist = taskConfig.assist;
//         taskItem.IsHard = taskConfig.ishardTask == 1;
//         taskItem.Type = (int)TaskType.Dynamic;
//
//         
//         var unlockLines = GetMergeLine(true);
//         var mergeWeight = GetMergeLineWeight(unlockLines);
//         if (mergeWeight == null || mergeWeight.Count == 0)
//             return;
//         
//         FillThreeItem(ref taskItem, taskConfig, mergeWeight, null, index);
//
//         if (taskItem.ItemIds.Count == 0)
//         {
//             DynamicEasyTask();
//         }
//         else
//         {
//             FillReward(taskItem, true);
//         }
//         
//         string biParam = "";
//         for(int j = 0; j < taskItem.ItemIds.Count; j++)
//             biParam += taskItem.ItemIds[j] + ":" + taskItem.ItemNums[j] + " ";
//             
//         GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,taskItem.OrgId + "_" + taskItem.Type, biParam);
//
//         if (taskConfig.ishardTask == 1)
//         {
//             DebugUtil.Log("[Dynamic] 生成困难任务 " + taskItem.Id + "\t items: " + biParam + "\t type="+taskItem.RewardTypes[0]+"\tnum="+taskItem.RewardNums[0]);
//         }
//         else
//         {
//             DebugUtil.Log("[Dynamic] 生成普通任务 " + taskItem.Id + "\t items: " + biParam + "\t type="+taskItem.RewardTypes[0]+"\tnum="+taskItem.RewardNums[0]);
//         }
//
//         StorageTaskGroup.CurTasks.Add(taskItem);
//     }
//     
//     public StorageTaskItem CreateHardTaskByCache()
//     {
//         if (StorageTaskGroup.HardTaskCacheQueue.Count == 0)
//             return null;
//
//         if (!CanCreateHardTask())
//             return null;
//         
//         var taskNum = DynamicTaskConfigManager.Instance.GetTaskNum(ExperenceModel.Instance.GetLevel());
//         int randomNum = Random.Range(0, 101);
//         if (randomNum > taskNum.hardTask_Refresh_Weight)
//         {
//             DebugUtil.Log("[Dynamic] 困难任务缓存生成 失败 随机数: " + randomNum);
//             return null;
//         }
//         
//         int id = StorageTaskGroup.HardTaskCacheQueue[0];
//         UserTaskType taskType = AdConfigHandle.Instance.GetUserTaskType();
//         List<TableFreeNormalTask> normalTasks;
//         if (taskType == UserTaskType.Free)
//         {
//             normalTasks = DynamicTaskConfigManager.Instance._freeNomalTasks;
//         }
//         else
//         {
//             normalTasks = DynamicTaskConfigManager.Instance._payNomalTasks;
//         }
//
//         var taskConfig = normalTasks.Find(a => a.id == id);
//
//         DebugUtil.Log("[Dynamic] 困难任务缓存生成 " + taskConfig.id);
//         
//         int index = StorageTaskGroup.DynamicNormalIndex++;
//         var taskItem = CreateNormalTask(taskConfig);
//         StorageTaskGroup.DynamicNormalIndex = index;
//         if (taskItem != null)
//         {
//             StorageTaskGroup.HardTaskCacheQueue.RemoveAt(0);
//         }
//         return taskItem;
//     }
// }