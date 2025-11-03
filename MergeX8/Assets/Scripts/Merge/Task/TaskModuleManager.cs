// using System;
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using DG.Tweening;
// using DragonPlus;
// using DragonPlus.ConfigHub.Ad;
// using DragonU3DSDK;
// using DragonU3DSDK.Storage;
// using DragonU3DSDK.Config;
// using DragonU3DSDK.Network.API;
// using DragonU3DSDK.Network.API.Protocol;
// using Framework;
// using Gameplay;
// using Gameplay.UI.Task;
// using MagneticScrollView;
// using SomeWhere;
// using UnityEngine.PlayerLoop;
// using Random = UnityEngine.Random;
// using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
//
// public partial class TaskModuleManager : Manager<TaskModuleManager>
// {
//     public enum TaskType : int
//     {
//         Normal = 1, //普通
//         Special,    //特殊
//         Easy,       //简单
//         Random,     //随机
//         Seal,       //海豹
//         Dynamic,    //动态
//         Readily,    //爽单
//     }
//     
//     private const int _sealTaskId = 1101110;
//     private const int _dolphinTaskId = 1201110;
//     
//     public Dictionary<int, int> debugCompleteTaskIds = new Dictionary<int, int>();
//     
//     private StorageTaskGroup _storageTaskGroup = null;
//
//     public StorageTaskGroup StorageTaskGroup
//     {
//         get
//         {
//             if(_storageTaskGroup == null)
//                 _storageTaskGroup = StorageManager.Instance.GetStorage<StorageGame>().TaskGroups;
//
//             return _storageTaskGroup;
//         }
//     }
//     
//     List<int> headIndexList = new List<int>();
//
//     private MergeTaskTipsItem curMergeTask = null;
//     private List<int> mergeRemoveIndexs = null;
//     
//     public MergeTaskTipsItem CurMergeTask
//     {
//         get { return curMergeTask; }
//         set { curMergeTask = value; }
//     }
//
//     public bool OpenDebugModule { get; set; }
//
//     public bool IsDebugCompleteTask(int taskId)
//     {
//         if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
//             return false;
//
//         if (debugCompleteTaskIds.ContainsKey(taskId))
//             return true;
//
//         return false;
//     }
//
//     public static int ChangeTaskRewardType(int originalRewardType)
//     {
//         if (RecoverCoinModel.Instance.IsStart() && originalRewardType == (int) UserData.ResourceId.Coin)
//             originalRewardType = (int) UserData.ResourceId.RecoverCoinStar;
//         return originalRewardType;
//     }
//     public StorageList<StorageTaskItem> CurTaskList
//     {
//         get { return StorageTaskGroup.CurTasks; }
//     }
//
//     public int CompleteTaskNum
//     {
//         get { return StorageTaskGroup.CompletedTaskIds.Count; }
//     }
//
//     public bool IsCompleteTask(int id)
//     {
//         return StorageTaskGroup.CompletedTaskIds.ContainsKey(id);
//     }
//
//     public bool HaveTask(int id)
//     {
//         return CurTaskList.Find(a => a.OrgId == id) != null;
//     }
//
//     public void Init()
//     {
//         InitTask();
//         InitDynamicTask();
//         CreateEasyTask();
//         FixUnLockTask();
//         FixNullMergeTask();
//     }
//
//     public bool IsFinishSealTask()
//     {
//         return IsCompleteTask(_sealTaskId);
//     }
//     
//     public bool IsSealActiveTask()
//     {
//         return HaveTask(_sealTaskId);
//     }
//
//     public bool IsSealTask(int taskId)
//     {
//         return _sealTaskId == taskId;
//     }
//     
//     public bool IsFinishDolphinTask()
//     {
//         return IsCompleteTask(_dolphinTaskId);
//     }
//     
//     public bool IsDolphinActiveTask()
//     {
//         return HaveTask(_dolphinTaskId);
//     }
//
//     public bool IsDolphinTask(int taskId)
//     {
//         return _dolphinTaskId == taskId;
//     }
//     
//     private void InitTask()
//     {
//         if (!StorageTaskGroup.IsInitTask)
//         {
//             var refTaskMap = TaskConfigManager.Instance.GetReferenceList(-1);
//             AddTask(refTaskMap);
//             StorageTaskGroup.IsInitTask = true;
//             
//             return;
//         }
//
//         RefreshLevelUpUnLockTask();
//     }
//
//     public void FixUnLockTask()
//     {
//         if(StorageTaskGroup.CompletedTaskIds == null || StorageTaskGroup.CompletedTaskIds.Count == 0)
//             return;
//         
//         var comTaskIds = StorageTaskGroup.CompletedTaskIds.Keys.ToList();
//
//         for (int i = comTaskIds.Count - 1; i >= 0; i--)
//         {
//             TableTask config = TaskConfigManager.Instance.GetTableTaskConfig(comTaskIds[i]);
//             if (config == null)
//                 continue;
//             
//             if(config.jumpDynamic == 1)
//                 continue;
//             
//             List<TableTask> unLockTask = GetUnLockTasks(comTaskIds[i]);
//             if (unLockTask != null && unLockTask.Count > 0)
//             {
//                 foreach (var kv in unLockTask)
//                 {
//                     AddTask(kv);
//                     DebugUtil.LogError($"UnLockTask comID={comTaskIds[i]}, newTask{kv.id}");
//                 }
//             }
//         }
//     }
//
//     public void FixNullMergeTask()
//     {
//         if(CurTaskList == null || CurTaskList.Count == 0)
//             return;
//         
//         foreach (var storageTaskItem in CurTaskList)
//         {
//             for (int i = 0; i <  storageTaskItem.ItemIds.Count; i ++) 
//             {
//                 var config = GameConfigManager.Instance.GetItemConfig(storageTaskItem.ItemIds[i]);
//                 if (config != null)
//                     continue;
//                 
//                 DebugUtil.LogError($"FixUnllMerge taskId={storageTaskItem.ItemIds[i]}, index{i}");
//                 storageTaskItem.ItemIds[i] = 101107;
//                 if(i < storageTaskItem.ItemNums.Count)
//                     storageTaskItem.ItemNums[i] = 1;
//             }
//         }
//     }
//     public void RefreshLevelUpUnLockTask()
//     {
//         if(ExperenceModel.Instance.GetLevel() > 3)
//             return;
//         
//         //找前边5个
//         int findCount = 0;
//         var comTaskIds = StorageTaskGroup.CompletedTaskIds.Keys.ToList();
//
//         bool isRefresh = false;
//         for (int i = comTaskIds.Count - 1; i >= 0; i--)
//         {
//             findCount++;
//             List<TableTask> unLockTask = GetUnLockTasks(comTaskIds[i]);
//             if (unLockTask != null && unLockTask.Count > 0)
//             {
//                 foreach (var kv in unLockTask)
//                 {
//                     if(kv.refreshLevel == 0)
//                         continue;
//                     
//                     isRefresh = true;
//                     AddTask(kv);
//                 }
//             }
//         
//             if(findCount >= 5)
//                 break;
//         }
//         
//         if(isRefresh)
//             EventDispatcher.Instance.DispatchEvent(MergeEvent.LEVELUP_TASK_REFRESH);
//     }
//
//     public List<TableTask> GetUnLockTasks(int id)
//     {
//         var tasks = TaskConfigManager.Instance.GetReferenceList(id);
//         if (tasks == null)
//             return null;
//
//         List<TableTask> unLockTask = new List<TableTask>();
//         foreach (var config in tasks)
//         {
//             if(IsCompleteTask(config.id))
//                 continue;
//             
//             if(config.refreshLevel > ExperenceModel.Instance.GetLevel())
//                 continue;
//             
//             bool unLock = true;
//             foreach (var frontId in config.frontTaskIds)
//             {
//                 if (IsCompleteTask(frontId))
//                     continue;
//                 
//                 unLock = false;
//                 break;
//             }
//             
//             if(!unLock)
//                 continue;
//             
//             if(CurTaskList.Find(a=>a.Id == config.id) != null)
//                 continue;
//             
//             unLockTask.Add(config);
//         }
//
//         return unLockTask;
//     }
//     
//     public bool IsCompleteAllTask()
//     {
//         if(StorageTaskGroup.CompleteNormalNum >= TaskConfigManager.Instance._normalCount)
//             return true;
//
//         return false;
//     }
//
//     public void AddTask(List<TableTask> configs)
//     {
//         if(configs == null || configs.Count == 0)
//             return;
//
//         foreach (var kv in configs)
//         {
//             AddTask(kv);
//         }
//     }
//     public StorageTaskItem AddTask(TableTask config)
//     {
//         if (config == null)
//             return null;
//
//         StorageTaskItem taskItem = new StorageTaskItem();
//         taskItem.Id = config.id;
//         taskItem.HeadIndex = config.headId > 0 ? config.headId : RandomHeadIndex();
//         taskItem.Type = config.type;
//         
//         var itemId = config.free_itemId;
//         var itemNum = config.free_itemNum;
//         var rewardType = config.free_rewardType;
//         var rewardNum = config.free_rewardNum;
//         taskItem.DogCookiesNum = config.free_dogCookiesNum;
//         taskItem.Assist = config.free_assist;
//         
//         UserTaskType taskType = AdConfigHandle.Instance.GetUserTaskType();
//         if (taskType == UserTaskType.Pay)
//         {
//             itemId = config.pay_itemId;
//             itemNum = config.pay_itemNum;
//             rewardType = config.pay_rewardType;
//             rewardNum = config.pay_rewardNum;
//             taskItem.DogCookiesNum = config.pay_dogCookiesNum;
//             taskItem.Assist = config.pay_assist;
//         }
//         for (int i = 0; i < itemId.Length; i++)
//         {
//             taskItem.ItemIds.Add(itemId[i]);
//         }
//
//         for (int i = 0; i < itemNum.Length; i++)
//         {
//             taskItem.ItemNums.Add(itemNum[i]);
//         }
//         
//         for (int i = 0; i < rewardType.Length; i++)
//         {
//             taskItem.RewardTypes.Add(rewardType[i]);
//         }
//         
//         for (int i = 0; i < rewardNum.Length; i++)
//         {
//             taskItem.RewardNums.Add(rewardNum[i]);
//         }
//
//         bool isHard = false;
//         if (taskType == UserTaskType.Pay)
//         {
//             isHard = config.pay_hardTask == 1;
//         }
//         else
//         {
//             isHard = config.free_hardTask == 1;
//         }
//         taskItem.OrgId = taskItem.Id;
//         taskItem.IsHard = isHard;
//         StorageTaskGroup.CurTasks.Add(taskItem);
//
//         return taskItem;
//     }
//     public StorageTaskItem AddEasyTask(TableEasyTaskList config)
//     {
//         if (config == null)
//             return null;
//
//         StorageTaskItem taskItem = new StorageTaskItem();
//         taskItem.Id = config.id;
//         taskItem.HeadIndex = config.headId > 0 ? config.headId : RandomHeadIndex();
//         taskItem.Type = (int)TaskType.Easy;
//         
//         var itemId = config.itemId;
//         var itemNum = config.itemNum;
//         var rewardType = config.rewardType;
//         var rewardNum = config.rewardNum;
//         
//         for (int i = 0; i < itemId.Length; i++)
//         {
//             taskItem.ItemIds.Add(itemId[i]);
//         }
//
//         for (int i = 0; i < itemNum.Length; i++)
//         {
//             taskItem.ItemNums.Add(itemNum[i]);
//         }
//         
//         for (int i = 0; i < rewardType.Length; i++)
//         {
//             taskItem.RewardTypes.Add(rewardType[i]);
//         }
//         
//         for (int i = 0; i < rewardNum.Length; i++)
//         {
//             taskItem.RewardNums.Add(rewardNum[i]);
//         }
//
//         bool isHard = false;
//         taskItem.DogCookiesNum = config.dogCookiesNum;
//         taskItem.Assist = config.assist;
//         taskItem.OrgId = taskItem.Id;
//         taskItem.IsHard = isHard;
//         StorageTaskGroup.CurTasks.Add(taskItem);
//
//         return taskItem;
//     }
//
//
//     public int RandomHeadIndex()
//     {
//         headIndexList.Clear();
//         foreach (var spine in TaskConfigManager.Instance._taskHeadSpines)
//         {
//             headIndexList.Add(spine.id);
//         }
//
//         for (int i = 0; i < CurTaskList.Count; i++)
//         {
//             if (CurTaskList[i].HeadIndex <= 0)
//                 continue;
//
//             if (!headIndexList.Contains(CurTaskList[i].HeadIndex))
//                 continue;
//
//             headIndexList.Remove(CurTaskList[i].HeadIndex);
//         }
//
//         int hdIndex = 0;
//         if (headIndexList.Count > 0)
//             hdIndex = headIndexList[UnityEngine.Random.Range(0, headIndexList.Count)];
//         else
//             hdIndex = TaskConfigManager.Instance._taskHeadSpines.RandomPickOne().id;
//
//         return hdIndex;
//     }
//
//     public bool IsTaskNeedItem(int id, bool checkUI = true)
//     {
//         StorageList<StorageTaskItem> checkTaskList =  CurTaskList;
//
//         if (checkTaskList == null || checkTaskList.Count == 0)
//             return false;
//
//         for (int i = 0; i < checkTaskList.Count; i++)
//         {
//             if (checkUI && MergeTaskTipsController.Instance != null &&
//                 !MergeTaskTipsController.Instance.IsHaveTask(checkTaskList[i].Id))
//                 continue;
//
//             StorageTaskItem curStorageTaskItem = checkTaskList[i];
//
//             if (curStorageTaskItem == null)
//                 continue;
//
//             if (curStorageTaskItem.ItemIds == null || curStorageTaskItem.ItemIds.Count == 0)
//                 continue;
//
//             foreach (var value in curStorageTaskItem.ItemIds)
//             {
//                 if (value != id)
//                     continue;
//
//                 return true;
//             }
//         }
//
//         return false;
//     }
//     
//     public bool IsCompleteTaskByItem(int id)
//     {
//         StorageList<StorageTaskItem> checkTaskList =  CurTaskList;
//         if (checkTaskList == null || checkTaskList.Count == 0)
//             return false;
//
//         for (int i = 0; i < checkTaskList.Count; i++)
//         {
//             if(MergeTaskTipsController.Instance == null)
//                 continue;
//             
//             if (!MergeTaskTipsController.Instance.IsHaveTask(checkTaskList[i].Id))
//                 continue;
//
//             StorageTaskItem curStorageTaskItem = checkTaskList[i];
//             if (curStorageTaskItem == null)
//                 continue;
//
//             if (curStorageTaskItem.ItemIds == null || curStorageTaskItem.ItemIds.Count == 0)
//                 continue;
//
//             if(!curStorageTaskItem.ItemIds.Contains(id))
//                 continue;
//
//             var taskItem = MergeTaskTipsController.Instance.GetTaskItem(curStorageTaskItem.Id);
//             if(taskItem == null)
//                 continue;
//             
//             if (taskItem.IsComplteTask)
//                 return true;
//         }
//
//         return false;
//     }
//     
//     public bool IsTaskNeedInLineItem(int id)
//     {
//         var needItemConfig = GameConfigManager.Instance.GetItemConfig(id);
//
//         if (CurTaskList == null || CurTaskList.Count == 0 || needItemConfig == null)
//             return false;
//
//         for (int i = 0; i < CurTaskList.Count; i++)
//         {
//             if (MergeTaskTipsController.Instance != null &&
//                 !MergeTaskTipsController.Instance.IsHaveTask(CurTaskList[i].Id))
//                 continue;
//
//             StorageTaskItem curStorageTaskItem = GetCurTaskItem(CurTaskList[i].Id);
//
//             if (curStorageTaskItem == null)
//                 continue;
//
//             if (curStorageTaskItem.ItemIds == null || curStorageTaskItem.ItemIds.Count == 0)
//                 continue;
//
//             foreach (var value in curStorageTaskItem.ItemIds)
//             {
//                 var currConfig = GameConfigManager.Instance.GetItemConfig(value);
//                 if (currConfig == null || needItemConfig.in_line != currConfig.in_line ||
//                     needItemConfig.level > currConfig.level)
//                     continue;
//
//                 return true;
//             }
//         }
//
//         return false;
//     }
//
//     public List<StorageTaskItem> CompleteTask(MergeTaskTipsItem tipsItem)
//     {
//         if (curMergeTask != null)
//             return null;
//
//         if (tipsItem == null)
//             return null;
//
//         // if (IsCompleteTask(tipsItem.TableTaskId()))
//         // {
//         //     StorageTaskGroup.CurTasks.Remove(tipsItem.StorageTaskItem);
//         //     EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH);
//         //     return;
//         // }
//         
//         AudioManager.Instance.PlaySound(20);
//
//         if (tipsItem.StorageTaskItem == null)
//             return null;
//
//         //if(tipsItem.StorageTaskItem.Type != (int)TaskType.Easy)
//             AddCompleteTaskNum();
//
//         if (tipsItem.StorageTaskItem.Type == (int)TaskType.Normal || tipsItem.StorageTaskItem.Type == (int)TaskType.Easy || tipsItem.StorageTaskItem.Type == (int)TaskType.Dynamic)
//             StorageTaskGroup.DailyCompleteNum++;
//             
//         if (tipsItem.StorageTaskItem.Type == (int)TaskType.Normal || tipsItem.StorageTaskItem.Type == (int)TaskType.Dynamic)
//             StorageTaskGroup.ReadilyCompleteNum++;
//         
//         EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, Activity.BattlePass.Model.TaskType.Serve, 1, 1);
//             
//         Dictionary<int, int> demands = GetTaskDemands(tipsItem.StorageTaskItem);
//         mergeRemoveIndexs = MergeManager.Instance.GetTaskCompleteItemIndex(demands,MergeBoardEnum.Main);
//
//         bool isDebugComplete = IsDebugCompleteTask(tipsItem.TableTaskId());
//
//         if (!isDebugComplete && (mergeRemoveIndexs == null || mergeRemoveIndexs.Count == 0))
//         {
//             EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
//             return null;
//         }
//
//         curMergeTask = tipsItem;
//
//         List<StorageTaskItem> listTasks = null;
//         if (tipsItem.StorageTaskItem.Type == (int)TaskType.Readily)
//         {
//             Dictionary<int, int> copyData = null;
//             if(demands != null)
//                 copyData = new Dictionary<int, int>(demands);
//             listTasks = CompleteReadilyTask(tipsItem.StorageTaskItem, copyData);
//         }
//         else
//         {
//             listTasks = CompleteTask(tipsItem.StorageTaskItem);
//         }
//         
//         string openNewTaskId = "";
//         if (listTasks != null)
//         {
//             for (int i = 0; i < listTasks.Count; i++)
//             {
//                 if(listTasks[i] != null)
//                     continue;
//
//                 listTasks.RemoveAt(i);
//                 i--;
//             }
//             
//             for (int i = 0; i < listTasks.Count; i++)
//             {
//                 openNewTaskId += listTasks[i].OrgId;
//                 if (i < listTasks.Count)
//                     openNewTaskId += "_";
//             }
//         }
//         
//         GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFinishTask,tipsItem.StorageTaskItem.OrgId + "_" +tipsItem.StorageTaskItem.Type, openNewTaskId, CompleteTaskNum.ToString());
//         if (RecoverCoinModel.Instance.IsStart())
//         {
//             var starCount = 0;
//             for (int i = 0; i < tipsItem.StorageTaskItem.RewardTypes.Count; i++)
//             {
//                 int rewardType = tipsItem.StorageTaskItem.RewardTypes[i];
//                 if (rewardType == (int) UserData.ResourceId.Coin)
//                 {
//                     starCount += tipsItem.StorageTaskItem.RewardNums[i];
//                 }
//             }
//             if (starCount > 0)
//             {
//                 DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverGetstar,
//                     data1:starCount.ToString(),
//                     data2:tipsItem.TableTaskId().ToString(),
//                     data3:(++RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.CompletedTaskCount).ToString());
//             }
//         }
//
//         UpdateTotalDecoCoin(curMergeTask);
//         
//         if (isDebugComplete && (mergeRemoveIndexs == null || mergeRemoveIndexs.Count == 0))
//         {
//             bool isSeal = false;
//             bool isDolphin = false;
//             
//             for (int i = 0; i < tipsItem.StorageTaskItem.RewardTypes.Count; i++)
//             {
//                 int rewardType = tipsItem.StorageTaskItem.RewardTypes[i];
//                 rewardType = ChangeTaskRewardType(rewardType);
//                 int rewardNum = tipsItem.StorageTaskItem.RewardNums[i];
//             
//                 UserData.Instance.AddRes(rewardType, rewardNum,
//                     new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward}, false);
//
//                 if (rewardType == (int)UserData.ResourceId.Seal)
//                     isSeal = true;
//                          
//                 if (rewardType == (int)UserData.ResourceId.Dolphin)
//                     isDolphin = true;
//                 
//                 if(!isSeal && !isDolphin)
//                     FlyCurrency(rewardType, rewardNum, tipsItem.StarTransform(i).transform.position, listTasks, i == tipsItem.StorageTaskItem.RewardTypes.Count-1);
//                 else
//                 {
//                     FlyCurrency(rewardType, rewardNum, tipsItem.RewardTransform().position, listTasks, i == tipsItem.StorageTaskItem.RewardTypes.Count-1);
//                 }
//             }
//
//             var count = MermaidModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, true);
//             FlyMermaid(count,tipsItem._mermaidText.transform.position);
//             FlyDogCookies(tipsItem.StorageTaskItem.DogCookiesNum, tipsItem._dogHopeObj.transform.position);
//
//             ShakeManager.Instance.ShakeLight();
//             tipsItem.PlayShake();
//         }
//         else
//         {
//             AddResource(tipsItem.StorageTaskItem);
//             
//             for (int i = 0; i < mergeRemoveIndexs.Count; i++)
//             {
//                 MergeBoard.Grid grid =MergeMainController.Instance.MergeBoard.GetGridByIndex(mergeRemoveIndexs[i]);
//                 int index = i;
//                 int itemIndex = GetGoodItemIndex(i, grid.board.id, tipsItem.ItemList);
//
//                 FlyGameObjectManager.Instance.FlyObject(mergeRemoveIndexs[i], grid.board.id,
//                     grid.board.transform.position, tipsItem.ItemList[itemIndex].transform, 0.8f,
//                     () =>
//                     {
//                         FlyGameObjectManager.Instance.PlayHintStarsEffect(tipsItem.ItemList[itemIndex].transform
//                             .position);
//                         if (index == mergeRemoveIndexs.Count - 1)
//                         {
//                             EventDispatcher.Instance.DispatchEvent(EventEnum.TASKBOX_PLAYANIM);
//
//                             bool isSeal = false;
//                             bool isDolphin = false;
//                             
//                             for (int i = 0; i < tipsItem.StorageTaskItem.RewardTypes.Count; i++)
//                             {
//                                 int rewardType = tipsItem.StorageTaskItem.RewardTypes[i];
//                                 rewardType = ChangeTaskRewardType(rewardType);
//                                 int rewardNum = tipsItem.StorageTaskItem.RewardNums[i];
//             
//                                 // UserData.Instance.AddRes(rewardType, rewardNum,
//                                 //     new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward}, false);
//
//                                 if (rewardType == (int)UserData.ResourceId.Seal)
//                                     isSeal = true;
//                                 
//                                 if (rewardType == (int)UserData.ResourceId.Dolphin)
//                                     isDolphin = true;
//                                 
//                                 if(!isSeal && !isDolphin)
//                                     FlyCurrency(rewardType, rewardNum, tipsItem.StarTransform(i).transform.position, listTasks, i == tipsItem.StorageTaskItem.RewardTypes.Count-1);
//                                 else
//                                 {
//                                     FlyCurrency(rewardType, rewardNum, tipsItem.RewardTransform().position, listTasks, i == tipsItem.StorageTaskItem.RewardTypes.Count-1);
//                                 }
//                             }
//                             
//                             FlyDogCookies(tipsItem.StorageTaskItem.DogCookiesNum, tipsItem._dogHopeObj.transform.position);
//                             var count = MermaidModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, true);
//                             FlyMermaid(count,tipsItem._mermaidText.transform.position);
//                             
//                             ShakeManager.Instance.ShakeLight();
//                             tipsItem.PlayShake();
//                         }
//                     });
//             }
//         }
//
//         if (MergeMainController.Instance.MergeBoard.SelectIndex > 0)
//         {
//             if (mergeRemoveIndexs.Contains(MergeMainController.Instance.MergeBoard.SelectIndex))
//                 EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID,
//                     Vector2Int.zero,MergeBoardEnum.Main);
//         }
//
//         if (demands != null)
//         {
//             foreach (var kv in demands)
//             {
//                 var product = GameConfigManager.Instance.GetItemConfig(kv.Key);
//                 GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
//                 {
//                     MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeMainTaskUse,
//                     itemAId = product.id,
//                     ItemALevel = product.level,
//                     isChange = true,
//                     extras = new Dictionary<string, string>
//                     {
//                         {"task_id", tipsItem.TableTaskId().ToString()},
//                     }
//                 });
//             }
//         }
//
//         SendTaskCompleteBI(tipsItem.TableTaskId());
//         MergeManager.Instance.Consume(demands,MergeBoardEnum.Main);
//         DailyRVModel.Instance.LevelPassed4RVShop();
//         EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
//         return listTasks;
//     }
//
//     private void AddResource(StorageTaskItem taskItem)
//     {
//         for (int i = 0; i < taskItem.RewardTypes.Count; i++)
//         {
//             int rewardType = taskItem.RewardTypes[i];
//             rewardType = ChangeTaskRewardType(rewardType);
//             int rewardNum = taskItem.RewardNums[i];
//
//             UserData.Instance.AddRes(rewardType, rewardNum,
//                 new GameBIManager.ItemChangeReasonArgs()
//                     { reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward }, false);
//         }
//
//         
//         if (DogHopeModel.Instance.IsOpenActivity())
//         {
//             if(taskItem.DogCookiesNum > 0)
//                 UserData.Instance.AddRes(DogHopeModel._dogCookiesId, taskItem.DogCookiesNum, new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DogGet}, false);
//
//         }
//         
//     }
//     
//     private void UpdateTotalDecoCoin(MergeTaskTipsItem mergeTask)
//     {
//         if(mergeTask == null)
//             return;
//         
//         for(int i = 0; i < mergeTask.StorageTaskItem.RewardTypes.Count; i++)
//         {
//             int rewardType = mergeTask.StorageTaskItem.RewardTypes[i];
//             int rewardNum = mergeTask.StorageTaskItem.RewardNums[i];
//             
//             if(rewardType != (int)UserData.ResourceId.Coin)
//                 continue;
//             
//             StorageManager.Instance.GetStorage<StorageHome>().TotalDecoCoin += rewardNum;
//         }
//     }
//     
//     private int GetGoodItemIndex(int loopIndex, int id, List<MergeTaskTipsGoods> goodsList)
//     {
//         if (loopIndex <= goodsList.Count - 1)
//         {
//             if (goodsList[loopIndex].gameObject.activeSelf)
//             {
//                 if (goodsList[loopIndex].id == id)
//                     return loopIndex;
//             }
//         }
//
//         for (int i = 0; i < goodsList.Count; i++)
//         {
//             if (goodsList[i].gameObject.activeSelf)
//             {
//                 if (goodsList[i].id == id)
//                     return i;
//             }
//         }
//
//         return 0;
//     }
//
//     private void SendTaskCompleteBI(int taskID)
//     {
//         switch (taskID)
//         {
//             // case 100106:
//             //     GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue84);
//             //     break;
//             // case 100107:
//             //     GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue87);
//             //     break;
//             // case 100108:
//             //     GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue85);
//             //     break;
//             // case 100109:
//             //     GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue86);
//             //     break;
//         }
//     }
//
//     private void CheckFinishTaskPop(int headIndex)
//     {
//         StorageGame storageGame = StorageManager.Instance.GetStorage<StorageGame>();
//         storageGame.MysteryGiftCompTaskCount++;
//         
//         CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1, () =>
//         {
//             if (GuideSubSystem.Instance.IsShowingGuide())
//                 return;
//
//             if (UIPopupIcebreakingPackController.CanShowUI())
//                 return;
//             
//             if (UIPopupIcebreakingPackLowController.CanShowUI())
//                 return;  
//             
//             if (UIPopupPayRebateController.CanShowUI())
//                 return;
//             
//             if (UIPopupPayRebateLocalController.CanShowUI())
//                 return;
//             
//             if (UIPopupMysteryGiftController.CanShowUI())
//             {
//                 UIPopupMysteryGiftController.TryShowMysteryGift(headIndex);
//             }
//
//             if (PigBankModel.Instance.CanShow())
//             {
//                 return;
//             }
//         }));
//     }
//
//     private void FlyMermaid(int count, Vector3 scrPos)
//     {
//         if(IsSealTask(curMergeTask.StorageTaskItem.OrgId) || IsDolphinTask(curMergeTask.StorageTaskItem.OrgId))
//             return;
//         
//         if (!MermaidModel.Instance.IsStart())
//             return;
//         var num = UserData.Instance.GetRes(UserData.ResourceId.Mermaid);
//         if(count > 0)
//             UserData.Instance.AddRes((int)UserData.ResourceId.Mermaid, count, new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DogGet}, false);
//
//         count = Math.Min(count, 10);
//         for (int i = 0; i < count; i++)
//         {
//             int index = i;
//             FlyGameObjectManager.Instance.FlyObject(curMergeTask._mermaidIcon.gameObject, scrPos, MergeTaskTipsController.Instance._MergeMermaid.transform.position, true, 1f, 0.1f * i, () =>
//             {
//                 FlyGameObjectManager.Instance.PlayHintStarsEffect(MergeTaskTipsController.Instance._MergeMermaid.transform.position);
//                 if(index==0)
//                     MergeTaskTipsController.Instance._MergeMermaid.SetText(num);
//                 ShakeManager.Instance.ShakeLight();
//
//             });
//         }
//
//    
//     }
//
//     private void FlyDogCookies(int count, Vector3 scrPos)
//     {
//         
//         if(!DogHopeModel.Instance.IsOpenActivity())
//             return;
//         
//         float delayTime = 0.3f;
//         if (count >= 5)
//             delayTime = 0.1f;
//         FlyGameObjectManager.Instance.FlyCurrency(DogHopeModel._dogCookiesId, count, scrPos, 1, false, delayTime, action:
//             () =>
//             {
//             });
//         
//         for(int i = 0; i < count; i++)
//         {
//            if (!UserData.Instance.IsResource(DogHopeModel._dogCookiesId))
//            {
//                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
//                {
//                    MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDog,
//                    itemAId =DogHopeModel._dogCookiesId,
//                    isChange = true,
//                    
//                });
//            }
//         // UserData.Instance.AddRes(DogHopeModel._dogCookiesId, 1,
//         //         new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DogGet}, false);
//         }
//     }
//     private void FlyCurrency(int type, int count, Vector3 scrPos, List<StorageTaskItem> tasks, bool isEnd)
//     {
//         Vector3 startPos = curMergeTask.completButton.transform.position;
//
//         if (isEnd && PigBankModel.Instance.IsOpened())
//         {
//             EventDispatcher.Instance.DispatchEvent(EventEnum.PIGBANK_INITIMAGE);
//             PigBankModel.Instance.AddCollectValue();
//         }
//
//         Action<float> pigBankLogic = (delayTime) =>
//         {
//             if (!PigBankModel.Instance.IsOpened())
//                 return;
//             
//             if(!isEnd)
//                 return;
//             
//             Transform target =
//                 CurrencyGroupManager.Instance.currencyController.GetIconTransform(UserData.ResourceId.Diamond);
//
//             Vector3 clontroPos = startPos;
//             clontroPos.x -= 0.5f;
//             clontroPos.y -= 0.5f;
//             
//             EventDispatcher.Instance.DispatchEventImmediately(EventEnum.PIGBANK_SHOW_BUTTON);
//             CommonUtils.DelayedCall(0.6f, () => 
//             {
//                 FlyGameObjectManager.Instance.FlyObject(target.gameObject, startPos, clontroPos,
//                     MergeMainController.Instance.PigBoxController.transform.position, true, 0.5f, delayTime, () =>
//                     {
//                         EventDispatcher.Instance.DispatchEventImmediately(EventEnum.PIGBANK_VALUE_REFRESH);
//                         FlyGameObjectManager.Instance.PlayHintStarsEffect(MergeMainController.Instance.PigBoxController
//                             .transform.position);
//                     }, 1f);
//
//             });  
//         };
//
//         Action action = () =>
//         {
//             if(!isEnd)
//                 return;
//             
//             MergeTaskTipsController.Instance.CompleteTask(curMergeTask, tasks);
//             CheckFinishTaskPop(curMergeTask.HeadIndex);
//
//             curMergeTask = null;
//             mergeRemoveIndexs.Clear();
//         };
//
//         if (count <= 0)
//         {
//             action();
//             return;
//         }
//
//         if (type != (int)UserData.ResourceId.Seal && UserData.Instance.IsResource(type) && type != (int)UserData.ResourceId.Dolphin)
//         {
//             float delayTime = 0.3f;
//             if (count >= 5)
//                 delayTime = 0.1f;
//             FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.currencyController,
//                 (UserData.ResourceId) type, count, scrPos, 1, false, true, delayTime, action:
//                 () => { action(); });
//
//             count = Math.Min(count, 10);
//             pigBankLogic(count * delayTime + 0.8f);
//             return;
//         }
//         
//         Transform endTrans = null;
//         if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
//         {
//             endTrans = MergeMainController.Instance.rewardBtnTrans;
//         }
//         else
//         {
//             endTrans = UIHomeMainController.mainController.MainPlayTransform;
//         }
//
//         if (type == (int)UserData.ResourceId.Seal || type == (int)UserData.ResourceId.Dolphin)
//         {
//             endTrans = MergeMainController.Instance.backTrans;
//         }
//
//         FlyGameObjectManager.Instance.FlyObject(type, scrPos, endTrans, 0.8f, 0.8f, () =>
//         {
//             FlyGameObjectManager.Instance.PlayHintStarsEffect(endTrans.position);
//             Animator shake = endTrans.transform.GetComponent<Animator>();
//             if (shake != null)
//                 shake.Play("shake", 0, 0);
//
//             if(isEnd)
//                 EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
//
//             action();
//         }, true, 1.0f, 0.7f, true);
//
//         pigBankLogic(0.8f + 0.3f);
//     }
//
//     public List<StorageTaskItem> CompleteTask(StorageTaskItem taskItem)
//     {
//         if (taskItem == null)
//             return null;
//
//         DailyCompleteTaskNumManager.Instance.SaveCompleteTaskNum();
//         EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_STATISTICS, TaskStatisticsModel.CustomType.completeTask,1);
//         StorageTaskGroup.CurTasks.Remove(taskItem);
//
//         
//         if (!StorageTaskGroup.CompletedTaskIds.ContainsKey(taskItem.Id))
//         {
//             StorageTaskGroup.CompletedTaskIds.Add(taskItem.Id, taskItem.Id);
//         }
//
//         if (taskItem.Type == (int)TaskType.Normal)
//             StorageTaskGroup.CompleteNormalNum++;
//
//         List<StorageTaskItem> newTask = CompleteDynamicTask(taskItem);
//
//         if (newTask == null)
//         {
//             TableTask tableTask = TaskConfigManager.Instance.GetTableTaskConfig(taskItem.OrgId);
//             if (tableTask != null && tableTask.jumpDynamic != 1)
//                 newTask = CompleteNormalTask(taskItem);
//         }
//
//         return newTask;
//     }
//
//     private List<StorageTaskItem> CompleteNormalTask(StorageTaskItem taskItem)
//     {
//         List<TableTask> unLockTask = GetUnLockTasks(taskItem.Id);
//         if (unLockTask == null || unLockTask.Count == 0)
//         {
//             return null;
//         }
//
//         List<StorageTaskItem> listTask = new List<StorageTaskItem>();
//         string openNewTaskId = "";
//         for (int i = 0; i < unLockTask.Count; i++)
//         {
//             openNewTaskId += unLockTask[i].id;
//             if (i < unLockTask.Count)
//                 openNewTaskId += "_";
//             
//             StorageTaskItem storageTaskItem = AddTask(unLockTask[i]);
//             listTask.Add(storageTaskItem);
//             
//             string biParam = "";
//             for(int j = 0; j < storageTaskItem.ItemIds.Count; j++)
//                 biParam += storageTaskItem.ItemIds[j] + ":" + storageTaskItem.ItemNums[j] + " ";
//             
//             GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,
//                 storageTaskItem.OrgId + "_" + storageTaskItem.Type, biParam);
//         }
//         
//         return listTask;
//     }
//     
//     public StorageTaskItem GetCurTaskItem(int id)
//     {
//         if (CurTaskList == null || CurTaskList.Count == 0)
//             return null;
//
//         return CurTaskList.Find(a => a.Id == id);
//     }
//
//     //当前任务最大ID
//     public int GetCurMaxTaskID()
//     {
//         if (CurTaskList == null || CurTaskList.Count == 0)
//             return -1;
//         int maxId = -1;
//         for (int i = 0; i < CurTaskList.Count; i++)
//         {
//             if (CurTaskList[i].Id > maxId)
//                 maxId = CurTaskList[i].Id;
//         }
//
//         return maxId;
//     }
//
//     public Dictionary<int, int> GetTaskDemands(StorageTaskItem curStorageTaskItem)
//     {
//         if (curStorageTaskItem == null)
//             return null;
//
//         Dictionary<int, int> demands = new Dictionary<int, int>();
//         for (int i = 0; i < curStorageTaskItem.ItemIds.Count; i++)
//         {
//             int id = curStorageTaskItem.ItemIds[i];
//             int num = curStorageTaskItem.ItemNums[i];
//             if (!demands.ContainsKey(id))
//                 demands.Add(id, num);
//             else
//             {
//                 demands[id] += num;
//             }
//         }
//
//         return demands;
//     }
//
//     /// <summary>
//     /// 获取任务中物品价值最高的物品
//     /// </summary>
//     public TableMergeItem GetTaskMaxPrice(StorageTaskItem taskItem)
//     {
//         TableMergeItem maxPriceItem = null;
//         int tempPrice = 0;
//         
//         foreach (var itemId in taskItem.ItemIds)
//         {
//             var config = GameConfigManager.Instance.GetItemConfig(itemId);
//             if (config == null)
//                 continue;
//             if (config.price > tempPrice)
//             {
//                 tempPrice = config.price;
//                 maxPriceItem = config;
//             }
//         }
//
//         return maxPriceItem;
//     }
//
//     public void CreateEasyTask()
//     {
//         UpdateTaskCurrentDayTime();
//         
//         if(CurTaskList == null || CurTaskList.Count == 0)
//             return;
//         
//         for (int i = 0; i < CurTaskList.Count; i++)
//         {
//             if (!CurTaskList[i].IsHard)
//                 return;
//         }
//         
//         long serverTime = (long)APIManager.Instance.GetServerTime() / 1000;
//         if (!Utils.IsSameDay(StorageTaskGroup.RefreshEasyTime, serverTime))
//         {
//             StorageTaskGroup.RefreshEasyTime = 0;
//         }
//
//         if ((serverTime  - StorageTaskGroup.RefreshEasyTime) < TaskConfigManager.Instance._easyTask.refresh_interval*60)
//             return;
//         
//         if(StorageTaskGroup.CompleteTaskNum >= TaskConfigManager.Instance._easyTask.max_task_num)
//             return;
//         
//         StorageTaskGroup.RefreshEasyTime = serverTime;
//
//         int loopCount = TaskConfigManager.Instance._easyTask.refresh_num;
//         var randomList = TaskConfigManager.Instance.GetEasyTaskList(ExperenceModel.Instance.GetLevel());
//         List<StorageTaskItem> taskItems = new List<StorageTaskItem>();
//         for (int i = 0; i < loopCount; i++)
//         {
//             StorageTaskItem taskItem = AddEasyTask(randomList.RandomPickOne());
//             if (taskItem != null)
//             {
//                 taskItem.Id = StorageTaskGroup.OnlyId++;
//             }
//             
//             taskItems.Add(taskItem);
//             GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,taskItem.OrgId + "_" + taskItem.Type);
//         }
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
//     private void AddCompleteTaskNum()
//     {
//         UpdateTaskCurrentDayTime();
//         StorageTaskGroup.CompleteTaskNum++;
//     }
//
//     private void UpdateTaskCurrentDayTime()
//     {
//         long serverTime = (long)APIManager.Instance.GetServerTime() / 1000;
//         if (!Utils.IsSameDay(StorageTaskGroup.CurrentDayTime, serverTime))
//         {
//             StorageTaskGroup.CurrentDayTime = serverTime;
//             StorageTaskGroup.CompleteTaskNum = 0;
//             StorageTaskGroup.DailyCompleteNum = 0;
//         }
//     }
// }