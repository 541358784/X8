using System;
using System.Collections.Generic;
using Activity.CrazeOrder.Model;
using Activity.Monopoly.View;
using Activity.SaveTheWhales;
using Activity.SlotMachine.View;
using Activity.Turntable.Model;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Framework;
using Gameplay;
using Merge.UnlockMergeLine;
using Scripts.UI;
using SomeWhere;
using ThemeDecorationLeaderBoard;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Merge.Order
{
    public partial class MainOrderManager
    {
        List<int> _headIndexList = new List<int>();


        private MergeTaskTipsItem curMergeTask = null;
        private List<int> mergeRemoveIndexs = null;

        public MergeTaskTipsItem CurMergeTask
        {
            get { return curMergeTask; }
            set { curMergeTask = value; }
        }

        public int CompleteTaskNum
        {
            get { return StorageTaskGroup.CompleteOrderNum; }
        }

        public StorageList<StorageTaskItem> CurTaskList
        {
            get { return StorageTaskGroup.CurTasks; }
        }

        public Dictionary<int, int> debugCompleteTaskIds = new Dictionary<int, int>();

        public int GetCurMaxTaskID()
        {
            if (CurTaskList == null || CurTaskList.Count == 0)
                return -1;
            int maxId = -1;
            for (int i = 0; i < CurTaskList.Count; i++)
            {
                if (CurTaskList[i].Id > maxId)
                    maxId = CurTaskList[i].Id;
            }

            return maxId;
        }

        public bool OpenDebugModule { get; set; }

        public int RandomHeadIndex()
        {
            _headIndexList.Clear();
            foreach (var spine in OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Normal))
            {
                _headIndexList.Add(spine.id);
            }

            for (int i = 0; i < StorageTaskGroup.CurTasks.Count; i++)
            {
                if (StorageTaskGroup.CurTasks[i].HeadIndex <= 0)
                    continue;

                if (!_headIndexList.Contains(StorageTaskGroup.CurTasks[i].HeadIndex))
                    continue;

                _headIndexList.Remove(StorageTaskGroup.CurTasks[i].HeadIndex);
            }

            int hdIndex = 0;
            if (_headIndexList.Count > 0)
                hdIndex = _headIndexList[UnityEngine.Random.Range(0, _headIndexList.Count)];
            else
                hdIndex = OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Normal).RandomPickOne().id;

            return hdIndex;
        }

        public bool HaveTask(int id)
        {
            return CurTaskList.Find(a => a.OrgId == id) != null;
        }

        public bool HaveTaskByType(int type)
        {
            return CurTaskList.Find(a => a.Type == type) != null;
        }
        
        public StorageTaskItem AddTask(TableOrderCreate config, SlotDefinition slot)
        {
            if (config == null)
                return null;

            StorageTaskItem taskItem = new StorageTaskItem();
            taskItem.Id = config.id;
            taskItem.HeadIndex = config.headId > 0 ? config.headId : RandomHeadIndex();
            taskItem.Type = config.type;
            taskItem.Slot = (int) slot;
            var itemId = config.itemId;
            var itemNum = config.itemNum;
            var rewardType = config.rewardType;
            var rewardNum = config.rewardNum;
            taskItem.DogCookiesNum = 0;
            taskItem.Assist = config.assist;

            for (int i = 0; i < itemId.Length; i++)
            {
                taskItem.ItemIds.Add(itemId[i]);
            }

            for (int i = 0; i < itemNum.Length; i++)
            {
                taskItem.ItemNums.Add(itemNum[i]);
            }

            for (int i = 0; i < rewardType.Length; i++)
            {
                taskItem.RewardTypes.Add(rewardType[i]);
            }

            for (int i = 0; i < rewardNum.Length; i++)
            {
                taskItem.RewardNums.Add(rewardNum[i]);
            }

            taskItem.OrgId = taskItem.Id;
            StorageTaskGroup.CurTasks.Add(taskItem);

            return taskItem;
        }

        public bool IsTaskNeedItem(int id, bool checkUI = true)
        {
            StorageList<StorageTaskItem> checkTaskList = CurTaskList;

            if (checkTaskList == null || checkTaskList.Count == 0)
                return false;

            for (int i = 0; i < checkTaskList.Count; i++)
            {
                if (checkUI && MergeTaskTipsController.Instance != null &&
                    !MergeTaskTipsController.Instance.IsHaveTask(checkTaskList[i].Id))
                    continue;

                StorageTaskItem curStorageTaskItem = checkTaskList[i];

                if (curStorageTaskItem == null)
                    continue;

                if (curStorageTaskItem.ItemIds == null || curStorageTaskItem.ItemIds.Count == 0)
                    continue;

                foreach (var value in curStorageTaskItem.ItemIds)
                {
                    if (value != id)
                        continue;

                    return true;
                }
            }

            return false;
        }

        public bool IsCompleteTaskByItem(int id)
        {
            StorageList<StorageTaskItem> checkTaskList = CurTaskList;
            if (checkTaskList == null || checkTaskList.Count == 0)
                return false;

            for (int i = 0; i < checkTaskList.Count; i++)
            {
                if (MergeTaskTipsController.Instance == null)
                    continue;

                if (!MergeTaskTipsController.Instance.IsHaveTask(checkTaskList[i].Id))
                    continue;

                StorageTaskItem curStorageTaskItem = checkTaskList[i];
                if (curStorageTaskItem == null)
                    continue;

                if (curStorageTaskItem.ItemIds == null || curStorageTaskItem.ItemIds.Count == 0)
                    continue;

                if (!curStorageTaskItem.ItemIds.Contains(id))
                    continue;

                var taskItem = MergeTaskTipsController.Instance.GetTaskItem(curStorageTaskItem.Id);
                if (taskItem == null)
                    continue;

                if (taskItem.IsComplteTask)
                    return true;
            }

            return false;
        }

        public void CompleteTask(MergeTaskTipsItem tipsItem)
        {
            if (curMergeTask != null)
                return;

            if (tipsItem == null)
                return;

            AudioManager.Instance.PlaySound(20);

            if (tipsItem.StorageTaskItem == null)
                return;

            if (tipsItem.StorageTaskItem.Type == (int)MainOrderType.Team)
            {
                TeamManager.Instance.Storage.RefreshOrderTime = (long)APIManager.Instance.GetServerTime();
            }
            StorageTaskGroup.CompleteNormalNum++;
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.Serve, 1, 1);
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.Serve, 1, 1);
            AdLocalConfigHandle.Instance.RefreshJudgingData(AdLocalDataType.CompletedOrder);
            AdSubSystem.Instance.TryRefreshInAdOrderNum();

            Dictionary<int, int> demands = GetTaskDemands(tipsItem.StorageTaskItem);
            mergeRemoveIndexs = MergeManager.Instance.GetTaskCompleteItemIndex(demands, MergeBoardEnum.Main);
            List<int> bagUseItemIndexList = null;
            if (MergeManager.Instance.completeNeedData.Count > 0)
            {
                bagUseItemIndexList =
                    MergeManager.Instance.GetTaskCompleteBagItemIndex(MergeManager.Instance.completeNeedData,
                        MergeBoardEnum.Main);
            }

            List<int> vipBagUseItemIndexList = null;
            if (MergeManager.Instance.completeNeedData.Count > 0)
            {
                vipBagUseItemIndexList =
                    MergeManager.Instance.GetTaskCompleteVipBagItemIndex(MergeManager.Instance.completeNeedData,
                        MergeBoardEnum.Main);
            }

            bool isDebugComplete = IsDebugCompleteTask(tipsItem.TableTaskId());

            if (!isDebugComplete && (
                    (mergeRemoveIndexs == null || mergeRemoveIndexs.Count == 0) &&
                    (bagUseItemIndexList == null || bagUseItemIndexList.Count == 0) &&
                    (vipBagUseItemIndexList == null || vipBagUseItemIndexList.Count == 0)
                ))
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH, MergeBoardEnum.Main);
                return;
            }

            curMergeTask = tipsItem;

            List<ResData> resDatas = new List<ResData>();
            AddResource(tipsItem.StorageTaskItem, ref resDatas);
            
            CompleteTask(tipsItem.StorageTaskItem);
            
            List<StorageTaskItem> listTasks = TryFillOrder(tipsItem.StorageTaskItem);

            string openNewTaskId = "";
            if (listTasks != null)
            {
                for (int i = 0; i < listTasks.Count; i++)
                {
                    if (listTasks[i] != null)
                        continue;

                    listTasks.RemoveAt(i);
                    i--;
                }

                for (int i = 0; i < listTasks.Count; i++)
                {
                    openNewTaskId += listTasks[i].OrgId;
                    if (i < listTasks.Count)
                        openNewTaskId += "_";
                }
            }

            DogPlayModel.Instance.CheckOrderState();

            Dictionary<string, string> extras = new Dictionary<string, string>();
            string extravalue = "";
            if(tipsItem.StorageTaskItem.ItemIds != null)
            {
                for (int i = 0; i < tipsItem.StorageTaskItem.ItemIds.Count; i++)
                {
                    extravalue += tipsItem.StorageTaskItem.ItemIds[i] + ":1";
                    if (i < tipsItem.StorageTaskItem.ItemIds.Count)
                        extravalue += ",";
                }
                extras.Add("OrderItemNum", extravalue);
            }
            
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFinishTask,
                tipsItem.StorageTaskItem.OrgId + "_" + tipsItem.StorageTaskItem.Type, openNewTaskId,
                CompleteTaskNum.ToString(), extras);

            if (RecoverCoinModel.Instance.IsStart())
            {
                var starCount = 0;
                for (int i = 0; i < tipsItem.StorageTaskItem.RewardTypes.Count; i++)
                {
                    int rewardType = tipsItem.StorageTaskItem.RewardTypes[i];
                    if (rewardType == (int) UserData.ResourceId.Coin)
                    {
                        starCount += tipsItem.StorageTaskItem.RewardNums[i];
                    }
                }

                if (starCount > 0)
                {
                    DragonPlus.GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverGetstar,
                        data1: starCount.ToString(),
                        data2: tipsItem.TableTaskId().ToString(),
                        data3: (++RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.CompletedTaskCount).ToString());
                }
            }

            UpdateTotalDecoCoin(curMergeTask);

            Action taskCompleteAction = () =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.TASKBOX_PLAYANIM);

                bool isSeal = false;
                bool isDolphin = false;

                for (int i = 0; i < tipsItem.StorageTaskItem.ExtraRewardTypes.Count; i++)
                {
                    int rewardType = tipsItem.StorageTaskItem.ExtraRewardTypes[i];
                    rewardType = ChangeTaskRewardType(rewardType);
                    int rewardNum = tipsItem.StorageTaskItem.ExtraRewardNums[i];

                    Vector3 flyPosition = Vector3.zero;
                    if (tipsItem.StorageTaskItem.Type == (int)MainOrderType.Time)
                        flyPosition = tipsItem._timeOrderIcon.transform.position;
                    else if (tipsItem.StorageTaskItem.Type == (int)MainOrderType.Craze)
                        flyPosition = tipsItem._currentCrazeOrderIcon.transform.position;
                    
                    if(rewardType == (int)UserData.ResourceId.Mermaid)
                        FlyMermaid(tipsItem, rewardNum, flyPosition, ref resDatas, false);
                    // if (rewardType == (int)UserData.ResourceId.ZumaBall)
                    // {
                    //     FlyZumaScore(tipsItem, rewardNum, flyPosition, 1);
                    // }
                    // else 
                    if (rewardType == DogHopeModel._dogCookiesId)
                    {
                        var multiValue =
                            ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.DogHope);
                        rewardNum = (int)(rewardNum*multiValue);
                        FlyDogCookies(rewardNum,flyPosition,multiValue);
                    }
                    else if (rewardType == ParrotModel.ParrotMergeItemId)
                    {
                        tipsItem.ParrotGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == FlowerFieldModel.FlowerFieldMergeItemId)
                    {
                        tipsItem.FlowerFieldGroup.CollectReward(ref resDatas, false);
                    }
                    else if(rewardType == (int)UserData.ResourceId.Easter2024Egg)
                        FlyEaster2024Egg(tipsItem, rewardNum,flyPosition, ref resDatas, false, true);          
                    else if(rewardType == (int)UserData.ResourceId.Water)
                        FlyWater(tipsItem, rewardNum,flyPosition, ref resDatas, false, true);
                    else if (rewardType == (int) UserData.ResourceId.SnakeLadderTurntable)
                    {
                        var multiValue =
                            ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.SnakeLadder);
                        rewardNum = (int)(rewardNum*multiValue);
                        FlySnakeLadderTurntable(tipsItem, rewardNum,flyPosition, multiValue,ref resDatas, false, true);
                    }
                    else if (rewardType == (int) UserData.ResourceId.ThemeDecorationScore)
                    {
                        var multiValue =
                            ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ThemeDecoration);
                        rewardNum = (int)(rewardNum*multiValue);
                        FlyThemeDecorationScore(tipsItem, rewardNum,flyPosition, multiValue,ref resDatas, false, true);
                    }
                    else if (rewardType == (int) UserData.ResourceId.SlotMachineScore)
                    {
                        var multiValue = 1f;
                        // var multiValue =
                        //     ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ThemeDecoration);
                        rewardNum = (int)(rewardNum*multiValue);
                        FlySlotMachineScore(tipsItem, rewardNum,flyPosition, multiValue,ref resDatas, false, true);
                    }
                    else if (rewardType == (int) UserData.ResourceId.Turntable)
                    {
                        var multiValue = 1f;
                        rewardNum = (int)(rewardNum*multiValue);
                        FlyTurntableScore(tipsItem, rewardNum,flyPosition, multiValue,ref resDatas, false, true);
                    }
                    else if (rewardType == (int) UserData.ResourceId.TeamCoin)
                    {
                        var multiValue = 1f;
                        FlyTeamCoin(tipsItem, rewardNum, flyPosition, multiValue,ref resDatas, false, true);
                    }
                    else if (rewardType == (int) UserData.ResourceId.MonopolyDice)
                    {
                        var multiValue = 1f;
                        // var multiValue =
                        //     ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.SnakeLadder);
                        rewardNum = (int)(rewardNum*multiValue);
                        FlyMonopolyDice(tipsItem, rewardNum,flyPosition, multiValue,ref resDatas, false, true);
                    }
                    else if (rewardType == (int) UserData.ResourceId.MixMasterCoffee ||
                             rewardType == (int) UserData.ResourceId.MixMasterTea ||
                             rewardType == (int) UserData.ResourceId.MixMasterMilk ||
                             rewardType == (int) UserData.ResourceId.MixMasterLemonJuice ||
                             rewardType == (int) UserData.ResourceId.MixMasterIceCream ||
                             rewardType == (int) UserData.ResourceId.MixMasterCream ||
                             rewardType == (int) UserData.ResourceId.MixMasterPearl ||
                             rewardType == (int) UserData.ResourceId.MixMasterSugar ||
                             rewardType == (int) UserData.ResourceId.MixMasterIce ||
                             rewardType == (int) UserData.ResourceId.MixMasterExtra1)
                    {
                        tipsItem.MixMasterGroup.CollectReward(ref resDatas);
                    }
                    else if (rewardType == (int) UserData.ResourceId.TurtlePangPackage)
                    {
                        tipsItem.TurtlePangGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == (int) UserData.ResourceId.StarrySkyCompassRocket)
                    {
                        tipsItem.StarrySkyCompassGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == (int) UserData.ResourceId.ZumaBall)
                    {
                        tipsItem.ZumaGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == (int) UserData.ResourceId.FishCultureScore)
                    {
                        tipsItem.FishCultureGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == (int) UserData.ResourceId.PhotoAlbumScore)
                    {
                        tipsItem.PhotoAlbumGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == ClimbTreeModel._climbTreeBananaId)
                    {
                        tipsItem.ClimbTreeGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == (int) UserData.ResourceId.JungleAdventure)
                    {
                        tipsItem._jungleAdventure.CollectReward(ref resDatas);
                    }
                    else if (rewardType == (int) UserData.ResourceId.PillowWheel)
                    {
                        tipsItem.PillowWheelGroup.CollectReward(ref resDatas, false);
                    }
                    else if (rewardType == (int) UserData.ResourceId.CatchFish)
                    {
                        tipsItem.CatchFishGroup.CollectReward(ref resDatas, false);
                    }
                    else
                    {
                        if (rewardType == (int) UserData.ResourceId.RecoverCoinStar ||
                            rewardType == (int) UserData.ResourceId.Coin)
                        {
                            rewardNum = (int)(rewardNum*ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.Coin));
                        }
                        FlyCurrency(rewardType, rewardNum,flyPosition, listTasks, resDatas, false);
                    }
                }

                var count = MermaidModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, true);
                FlyMermaid(tipsItem, count, tipsItem._mermaidText.transform.position, ref resDatas);
                
                var waterCount = SaveTheWhalesModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, true);
                FlyWater(tipsItem, waterCount, tipsItem._saveTheWhalesText.transform.position, ref resDatas);
                
                var eggCount = Easter2024Model.Instance.GetTaskValue(tipsItem.StorageTaskItem, true);
                FlyEaster2024Egg(tipsItem, eggCount, tipsItem._easter2024EggText.transform.position, ref resDatas);
                {
                    var multiValue =
                        ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.SnakeLadder);
                    var turntableCount = SnakeLadderModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, true);
                    turntableCount = (int)(turntableCount*multiValue); 
                    FlySnakeLadderTurntable(tipsItem, turntableCount, tipsItem._snakeLadderTurntableText.transform.position,multiValue, ref resDatas);   
                }
                {
                    var value = tipsItem.StorageTaskItem.DogCookiesNum;
                    var multiValue =
                        ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.DogHope);
                    value = (int)(value*multiValue);
                    FlyDogCookies(value, tipsItem._dogHopeObj.transform.position,multiValue);   
                }
                {
                    var multiValue =
                        ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ThemeDecoration);
                    var turntableCount = ThemeDecorationModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, multiValue == 1);
                    turntableCount = (int)(turntableCount*multiValue); 
                    FlyThemeDecorationScore(tipsItem, turntableCount, tipsItem.ThemeDecorationGroup.transform.position,multiValue, ref resDatas);
                }
                {
                    // var multiValue =
                    //     ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.SlotMachine);
                    var multiValue = 1f;
                    var turntableCount = SlotMachineModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, multiValue == 1);
                    turntableCount = (int)(turntableCount*multiValue); 
                    FlySlotMachineScore(tipsItem, turntableCount, tipsItem.SlotMachineGroup.transform.position,multiValue, ref resDatas);
                }
                {
                    // var multiValue =
                    //     ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.Monopoly);
                    var multiValue = 1f;
                    var turntableCount = MonopolyModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, multiValue == 1);
                    turntableCount = (int)(turntableCount*multiValue); 
                    FlyMonopolyDice(tipsItem, turntableCount, tipsItem.MonopolyGroup.transform.position,multiValue, ref resDatas);
                }
                {
                    var multiValue = 1f;
                    var turntableCount = TurntableModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, multiValue == 1);
                    turntableCount = (int)(turntableCount*multiValue); 
                    FlyTurntableScore(tipsItem, turntableCount, tipsItem._turntableIcon.transform.position,multiValue, ref resDatas);
                }
                {
                    AddBalloonRacing(tipsItem.StorageTaskItem, tipsItem);
                    AddRabbitRacing(tipsItem.StorageTaskItem, tipsItem);
                }
                // {
                //     var multiValue = 1f;
                //     var zumaCount = ZumaModel.Instance.GetTaskValue(tipsItem.StorageTaskItem, multiValue == 1);
                //     zumaCount = (int)(zumaCount*multiValue); 
                //     FlyZumaScore(tipsItem, zumaCount, tipsItem.ZumaGroup.transform.position,multiValue);
                // }
                tipsItem.MixMasterGroup.CollectReward(ref resDatas);
                tipsItem.TurtlePangGroup.CollectReward(ref resDatas);
                tipsItem.StarrySkyCompassGroup.CollectReward(ref resDatas);
                tipsItem.ZumaGroup.CollectReward(ref resDatas);
                tipsItem.DogPlayGroup.CollectReward();
                tipsItem.FishCultureGroup.CollectReward(ref resDatas);
                tipsItem.PhotoAlbumGroup.CollectReward(ref resDatas);
                tipsItem.ClimbTreeGroup.CollectReward(ref resDatas);
                tipsItem._jungleAdventure.CollectReward(ref resDatas);
                tipsItem.ParrotGroup.CollectReward(ref resDatas);
                tipsItem.FlowerFieldGroup.CollectReward(ref resDatas);
                tipsItem.PillowWheelGroup.CollectReward(ref resDatas);
                tipsItem.CatchFishGroup.CollectReward(ref resDatas);
                ShakeManager.Instance.ShakeLight();
                tipsItem.PlayShake();
                
                

                for (int i = 0; i < tipsItem.StorageTaskItem.RewardTypes.Count; i++)
                {
                    int rewardType = tipsItem.StorageTaskItem.RewardTypes[i];
                    rewardType = ChangeTaskRewardType(rewardType);
                    int rewardNum = tipsItem.StorageTaskItem.RewardNums[i];
                    if (rewardType == (int) UserData.ResourceId.RecoverCoinStar ||
                        rewardType == (int) UserData.ResourceId.Coin)
                    {
                        rewardNum = (int)(rewardNum*ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.Coin));
                    }
                    if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                    {
                        UIRoot.Instance.EnableEventSystem = false;
                        FlyCurrency(rewardType, rewardNum, tipsItem.RewardTransform().position,
                            listTasks, resDatas, i == tipsItem.StorageTaskItem.RewardTypes.Count - 1);
                    }
                    else
                    {
                        FlyCurrency(rewardType, rewardNum,
                            tipsItem.StarTransform(i).transform.position, listTasks,resDatas,
                            i == tipsItem.StorageTaskItem.RewardTypes.Count - 1);
                    }
                }
            };
            if (mergeRemoveIndexs != null && mergeRemoveIndexs.Count > 0)
            {
                for (int i = 0; i < mergeRemoveIndexs.Count; i++)
                {
                    MergeBoard.Grid grid =
                        MergeMainController.Instance.MergeBoard.GetGridByIndex(mergeRemoveIndexs[i]);
                    int index = i;
                    int itemIndex = GetGoodItemIndex(i, grid.board.id, tipsItem.ItemList);

                    FlyGameObjectManager.Instance.FlyObject(mergeRemoveIndexs[i], grid.board.id,
                        grid.board.transform.position, tipsItem.ItemList[itemIndex].transform, 0.8f,
                        () =>
                        {
                            FlyGameObjectManager.Instance.PlayHintStarsEffect(tipsItem.ItemList[itemIndex].transform
                                .position);
                            if (index == mergeRemoveIndexs.Count - 1)
                            {
                                taskCompleteAction();
                            }
                        });
                }
            }
            else
            {
                taskCompleteAction();
            }

            if (MergeMainController.Instance.MergeBoard.SelectIndex > 0)
            {
                if (mergeRemoveIndexs.Contains(MergeMainController.Instance.MergeBoard.SelectIndex))
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID,
                        Vector2Int.zero, MergeBoardEnum.Main);
            }

            if (demands != null)
            {
                foreach (var kv in demands)
                {
                    var product = GameConfigManager.Instance.GetItemConfig(kv.Key);
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeMainTaskUse,
                        itemAId = product.id,
                        ItemALevel = product.level,
                        isChange = true,
                        extras = new Dictionary<string, string>
                        {
                            {"task_id", tipsItem.TableTaskId().ToString()},
                        }
                    });

                    // if (IsSealTask(tipsItem.StorageTaskItem.OrgId)|| IsDolphinTask(tipsItem.StorageTaskItem.OrgId))
                    // {
                    //     GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    //     {
                    //         MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemSpecialTask,
                    //         itemAId = product.id,
                    //         ItemALevel = product.level,
                    //         isChange = true,
                    //         extras = new Dictionary<string, string>
                    //         {
                    //             {"task_id", tipsItem.TableTaskId().ToString()},
                    //         }
                    //     });
                    // }
                }
            }

            MergeManager.Instance.Consume(demands, MergeBoardEnum.Main);
            DailyRVModel.Instance.LevelPassed4RVShop();
            EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH, MergeBoardEnum.Main);
        }

        public void AddResource(StorageTaskItem taskItem, ref List<ResData> resDatas)
        {
            if (taskItem.Slot == (int)SlotDefinition.Craze)
            {
                if (taskItem.ExtraRewardTypes != null && taskItem.ExtraRewardTypes.Count > 0)
                {
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCrazeOrderReward, CrazeOrderModel.Instance.Stage.ToString());
                }
            }
            AddResource(taskItem.Id, taskItem.OrgId, taskItem.RewardTypes, taskItem.RewardNums, ref resDatas);
            AddResource(taskItem.Id, taskItem.OrgId, taskItem.ExtraRewardTypes, taskItem.ExtraRewardNums, ref resDatas);
            
            if (DogHopeModel.Instance.IsOpenActivity())
            {
                if (taskItem.DogCookiesNum > 0)
                {
                    var addValue = taskItem.DogCookiesNum;
                    addValue = (int) (addValue *
                                      ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType
                                          .DogHope));
                    UserData.Instance.AddRes(DogHopeModel._dogCookiesId, addValue,
                        new GameBIManager.ItemChangeReasonArgs()
                            {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DogGet}, false);   
                    
                    resDatas.Add(new ResData(DogHopeModel._dogCookiesId, addValue));
                }
            }
        }

        public void AddResource(int id, int orgId, List<int> type, List<int> num, ref List<ResData> resDatas)
        {
            if(type == null || type.Count == 0)
                return;
            
            if(num == null || num.Count == 0)
                return;
            
            for (int i = 0; i < type.Count; i++)
            {
                int rewardType = type[i];
                int rewardNum = num[i];
                rewardType = ChangeTaskRewardType(rewardType);
                var multiValue = 1f;
                if (rewardType == (int) UserData.ResourceId.RecoverCoinStar ||
                    rewardType == (int) UserData.ResourceId.Coin)
                {
                    multiValue = ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.Coin);
                    rewardNum = (int)(rewardNum * multiValue);
                    
                    resDatas.Add(new ResData(rewardType, rewardNum));
                }

                if (rewardType == (int)UserData.ResourceId.RareDecoCoin)
                {
                    resDatas.Add(new ResData(rewardType, rewardNum));
                }

                GameBIManager.ItemChangeReasonArgs reasonArgs;
                if (IsSpecialTask(orgId))
                {
                    reasonArgs = new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward,
                        data2 = multiValue.ToString(),
                    };
                }
                else
                {
                    reasonArgs = new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward,
                        data1 = id.ToString(),
                        data2 = multiValue.ToString(),
                    };
                }
                
                UserData.Instance.AddRes(rewardType, rewardNum, reasonArgs, false);
            }
        }
        
        private void UpdateTotalDecoCoin(MergeTaskTipsItem mergeTask)
        {
            if (mergeTask == null)
                return;

            for (int i = 0; i < mergeTask.StorageTaskItem.RewardTypes.Count; i++)
            {
                int rewardType = mergeTask.StorageTaskItem.RewardTypes[i];
                int rewardNum = mergeTask.StorageTaskItem.RewardNums[i];

                if (rewardType != (int) UserData.ResourceId.Coin)
                    continue;

                StorageManager.Instance.GetStorage<StorageHome>().TotalDecoCoin += rewardNum;
            }
        }

        private int GetGoodItemIndex(int loopIndex, int id, List<MergeTaskTipsGoods> goodsList)
        {
            if (loopIndex <= goodsList.Count - 1)
            {
                if (goodsList[loopIndex].gameObject.activeSelf)
                {
                    if (goodsList[loopIndex].id == id)
                        return loopIndex;
                }
            }

            for (int i = 0; i < goodsList.Count; i++)
            {
                if (goodsList[i].gameObject.activeSelf)
                {
                    if (goodsList[i].id == id)
                        return i;
                }
            }

            return 0;
        }

        public void CheckFinishTaskPop(int headIndex)
        {
            StorageGame storageGame = StorageManager.Instance.GetStorage<StorageGame>();
            storageGame.MysteryGiftCompTaskCount++;
            
            int orderId = curMergeTask.StorageTaskItem.OrgId;
            bool isUnlock = UnlockMergeLineManager.Instance.FinishOrder(orderId);
            bool isShowingGuide = GuideSubSystem.Instance.IsShowingGuide();
            
            if(isUnlock || isShowingGuide)
                return;
            
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1, ()=>
            {
                CoroutineManager.Instance.StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.TaskFinishPopUIViewLogic());
            }));
        }

        private void FlyMermaid(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos, ref List<ResData> resDatas, bool autoAdd = true)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!MermaidModel.Instance.IsStart())
                return;
            
            if (MermaidModel.Instance.IsFinished())
                return;
            
            var num = UserData.Instance.GetRes(UserData.ResourceId.Mermaid);
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.Mermaid, count,
                    new GameBIManager.ItemChangeReasonArgs()
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EarnStar}, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.Mermaid, count));
            }

            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                FlyGameObjectManager.Instance.FlyObject(tipsItem._mermaidIcon.gameObject, scrPos,
                    MergeTaskTipsController.Instance._MergeMermaid.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(MergeTaskTipsController.Instance._MergeMermaid
                            .transform.position);
                        if (index == 0)
                            MergeTaskTipsController.Instance._MergeMermaid.SetText(num);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }
        private void FlyWater(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,  ref List<ResData> resDatas, bool autoAdd = true, bool isExtraReward = false)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!SaveTheWhalesModel.Instance.IsJoin())
                return;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSaveTheWhalesWaterGet,
                count.ToString(),
                SaveTheWhalesModel.Instance.StorageSaveTheWhales.GroupId.ToString());
            var num = UserData.Instance.GetRes(UserData.ResourceId.Water);
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.Water, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EarnEgg,
                        data1 = tipsItem.StorageTaskItem.Id.ToString()
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.Water, count));
            }

            MergeTaskTipsController.Instance._MergeSaveTheWhales.SetText(num);
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = tipsItem._saveTheWhalesIcon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem);
                    icon = extraIcon == null ? icon : extraIcon;
                }
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    MergeTaskTipsController.Instance._MergeSaveTheWhales.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(MergeTaskTipsController.Instance._MergeSaveTheWhales.transform.position);
                        ShakeManager.Instance.ShakeLight();
                        if(index==count-1)
                            SaveTheWhalesModel.Instance.CheckFinish();
                    });
            }
        }
        private void FlyEaster2024Egg(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,  ref List<ResData> resDatas, bool autoAdd = true, bool isExtraReward = false)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!Easter2024Model.Instance.IsStart())
                return;

            var num = UserData.Instance.GetRes(UserData.ResourceId.Easter2024Egg);
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.Easter2024Egg, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EarnEgg,
                        data1 = tipsItem.StorageTaskItem.Id.ToString()
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.Easter2024Egg, count));
            }
            if (!MergeTaskTipsController.Instance.MergeEaster2024)
                return;
            MergeTaskTipsController.Instance.MergeEaster2024.SetText(num);
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = tipsItem._easter2024EggIcon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem);
                    icon = extraIcon == null ? icon : extraIcon;
                }
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    MergeTaskTipsController.Instance.MergeEaster2024.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(MergeTaskTipsController.Instance.MergeEaster2024.transform.position);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }
        private void FlyThemeDecorationScore(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,float multiValue, ref List<ResData> resDatas, bool autoAdd = true, bool isExtraReward = false)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!ThemeDecorationModel.Instance.IsStart())
                return;

            var num = UserData.Instance.GetRes(UserData.ResourceId.ThemeDecorationScore);
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.ThemeDecorationScore, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EarnThemeDecorationScore,
                        data1 = tipsItem.StorageTaskItem.Id.ToString(),
                        data2 = multiValue.ToString(),
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.ThemeDecorationScore, count));
            }
            var storage = ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek;
            var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeThemeDecoration, DynamicEntry_Game_ThemeDecoration>();
            if (!entrance)
                return;
            entrance.SetText(num);
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = tipsItem.ThemeDecorationGroup.Icon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem);
                    icon = extraIcon == null ? icon : extraIcon;
                }
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    entrance.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(entrance.transform.position);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }

        private GameObject GetExtraIcon(MergeTaskTipsItem tipsItem)
        {
            if (tipsItem.StorageTaskItem.ExtraRewardNums == null || tipsItem.StorageTaskItem.ExtraRewardNums.Count == 0)
                return null;
            
            if (tipsItem.StorageTaskItem.Type  == (int)MainOrderType.Time)
                return tipsItem._timeOrderIcon.gameObject;
            else if(tipsItem.StorageTaskItem.Type  == (int)MainOrderType.Craze)
                return tipsItem._currentCrazeOrderIcon.gameObject;

            return null;
        }
        
        private void FlySlotMachineScore(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,float multiValue,  ref List<ResData> resDatas,bool autoAdd = true, bool isExtraReward = false)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!SlotMachineModel.Instance.IsOpened())
                return;

            var num = UserData.Instance.GetRes(UserData.ResourceId.SlotMachineScore);
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.SlotMachineScore, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EarnSlotMachineScore,
                        data1 = tipsItem.StorageTaskItem.Id.ToString(),
                        data2 = multiValue.ToString(),
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.SlotMachineScore, count));
            }
            var storage = SlotMachineModel.Instance.CurStorage;
            var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeSlotMachine, DynamicEntry_Game_SlotMachine>();
            if (!entrance)
                return;
            entrance.SetText(num);
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = tipsItem.SlotMachineGroup.Icon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem); 
                    icon = extraIcon == null ? icon : extraIcon;
                }
            
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    entrance.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(entrance.transform.position);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }

        
        private void FlyTurntableScore(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,float multiValue,  ref List<ResData> resDatas,bool autoAdd = true, bool isExtraReward = false)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!TurntableModel.Instance.IsOpened())
                return;
            
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.Turntable, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TurntableGet,
                        data1 = tipsItem.StorageTaskItem.Id.ToString(),
                        data2 = multiValue.ToString(),
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.Turntable, count));
            }
            
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = tipsItem._turntableIcon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem); 
                    icon = extraIcon == null ? icon : extraIcon;
                }
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    MergeTaskTipsController.Instance.MergeTurntableEntry.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect( MergeTaskTipsController.Instance.MergeTurntableEntry.transform.position);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }
        
        
        private void FlyTeamCoin(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,float multiValue,  ref List<ResData> resDatas,bool autoAdd = true, bool isExtraReward = false)
        {
            if (!TeamManager.Instance.HasOrder())
                return;
            
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.TeamCoin, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward,
                        data1 = tipsItem.StorageTaskItem.Id.ToString(),
                        data2 = multiValue.ToString(),
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.TeamCoin, count));
            }

            scrPos = tipsItem._teamIcon.transform.position;
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                GameObject icon = tipsItem._teamIcon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem); 
                    icon = extraIcon == null ? icon : extraIcon;
                }
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    MergeMainController.Instance.backTrans.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect( MergeMainController.Instance.backTrans.transform.position);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }
        // private void FlyZumaScore(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,float multiValue, bool autoAdd = true, bool isExtraReward = false)
        // {
        //     if (IsSealTask(tipsItem.StorageTaskItem.OrgId) || IsDolphinTask(tipsItem.StorageTaskItem.OrgId))
        //         return;
        //
        //     if (!ZumaModel.Instance.IsOpened())
        //         return;
        //     
        //     if (autoAdd && count > 0)
        //         UserData.Instance.AddRes((int) UserData.ResourceId.ZumaBall, count,
        //             new GameBIManager.ItemChangeReasonArgs()
        //             {
        //                 reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ZumaGet,
        //                 data1 = tipsItem.StorageTaskItem.Id.ToString(),
        //                 data2 = multiValue.ToString(),
        //             }, false);
        //     
        //     count = Math.Min(count, 10);
        //     for (int i = 0; i < count; i++)
        //     {
        //         int index = i;
        //         GameObject icon = tipsItem._ZumaIcon.gameObject;
        //         if (isExtraReward)
        //         {
        //             var extraIcon = GetExtraIcon(tipsItem); 
        //             icon = extraIcon == null ? icon : extraIcon;
        //         }
        //         
        //         FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
        //             MergeTaskTipsController.Instance.MergeZumaEntry.transform.position, true, 1f, 0.1f * i, () =>
        //             {
        //                 FlyGameObjectManager.Instance.PlayHintStarsEffect( MergeTaskTipsController.Instance.MergeZumaEntry.transform.position);
        //                 ShakeManager.Instance.ShakeLight();
        //             });
        //     }
        // }
        //
        
        
        public void FlyMonopolyDice(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos, float multiValue,  ref List<ResData> resDatas,bool autoAdd = true, bool isExtraReward = false)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!MonopolyModel.Instance.IsOpened())
                return;

            var num = UserData.Instance.GetRes(UserData.ResourceId.MonopolyDice);
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.MonopolyDice, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EarnMonopolyDice,
                        data1 = tipsItem.StorageTaskItem.Id.ToString(),
                        data2 = multiValue.ToString(),
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.MonopolyDice, count));
            }
            var storage = MonopolyModel.Instance.CurStorageMonopolyWeek;
            var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeMonopoly, DynamicEntry_Game_Monopoly>();
            if (!entrance)
                return;
            entrance.SetText(num);
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = tipsItem.MonopolyGroup.Icon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem);
                    icon = extraIcon == null ? icon : extraIcon;
                }
                
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    entrance.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(entrance.transform.position);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }
        private void FlySnakeLadderTurntable(MergeTaskTipsItem tipsItem, int count, Vector3 scrPos,float multiValue,  ref List<ResData> resDatas,bool autoAdd = true, bool isExtraReward = false)
        {
            if (IsSpecialTask(tipsItem.StorageTaskItem.OrgId))
                return;

            if (!SnakeLadderModel.Instance.IsStart())
                return;

            var num = UserData.Instance.GetRes(UserData.ResourceId.SnakeLadderTurntable);
            if (autoAdd && count > 0)
            {
                UserData.Instance.AddRes((int) UserData.ResourceId.SnakeLadderTurntable, count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EarnSnakeLadderTurntable,
                        data1 = tipsItem.StorageTaskItem.Id.ToString(),
                        data2 = multiValue.ToString(),
                    }, false);
                
                resDatas.Add(new ResData(UserData.ResourceId.SnakeLadderTurntable, count));
            }
            if (!MergeTaskTipsController.Instance.MergeSnakeLadder)
                return;
            MergeTaskTipsController.Instance.MergeSnakeLadder.SetText(num);
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = tipsItem._snakeLadderTurntableIcon.gameObject;
                if (isExtraReward)
                {
                    var extraIcon = GetExtraIcon(tipsItem);
                    icon = extraIcon == null ? icon : extraIcon;
                }
                
                FlyGameObjectManager.Instance.FlyObject(icon, scrPos,
                    MergeTaskTipsController.Instance.MergeSnakeLadder.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(MergeTaskTipsController.Instance.MergeSnakeLadder.transform.position);
                        ShakeManager.Instance.ShakeLight();
                    });
            }
        }

        private void FlyCurrency(int type, int count, Vector3 scrPos, List<StorageTaskItem> tasks, List<ResData> resDatas, bool isEnd)
        {
            Vector3 startPos = curMergeTask.completButton.transform.position;

            if (isEnd && PigBankModel.Instance.IsOpened())
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.PIGBANK_INITIMAGE);
                PigBankModel.Instance.AddCollectValue();
            }

            Action<float> pigBankLogic = (delayTime) =>
            {
                if (!PigBankModel.Instance.IsOpened())
                    return;

                if (!isEnd)
                    return;

                Transform target = CurrencyGroupManager.Instance.currencyController.GetIconTransform(UserData.ResourceId.Diamond);

                Vector3 clontroPos = startPos;
                clontroPos.x -= 0.5f;
                clontroPos.y -= 0.5f;

                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.PIGBANK_SHOW_BUTTON);
                CommonUtils.DelayedCall(0.6f, () =>
                {
                    FlyGameObjectManager.Instance.FlyObject(target.gameObject, startPos, clontroPos,
                        MergeMainController.Instance.PigBoxController.transform.position, true, 0.5f, delayTime, () =>
                        {
                            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.PIGBANK_VALUE_REFRESH);
                            FlyGameObjectManager.Instance.PlayHintStarsEffect(MergeMainController.Instance.PigBoxController.transform.position);
                        }, 1f);
                });
            };

            Action action = () =>
            {
                if (!isEnd)
                    return;

                MergeTaskTipsController.Instance.CompleteTask(curMergeTask, tasks, resDatas);
                
                MainOrderManager.Instance.CheckFinishTaskPop(curMergeTask.HeadIndex);
                curMergeTask = null;
                mergeRemoveIndexs.Clear();
                
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            };

            if (count <= 0)
            {
                action();
                return;
            }

            if (type != (int) UserData.ResourceId.Seal && UserData.Instance.IsResource(type) &&
                type != (int) UserData.ResourceId.Dolphin && type != (int) UserData.ResourceId.Capybara && type != (int) UserData.ResourceId.TeamCoin)
            {
                float delayTime = 0.3f;
                if (count >= 5)
                    delayTime = 0.1f;
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.currencyController,
                    (UserData.ResourceId) type, count, scrPos, 1, false, true, delayTime, action:
                    () => { action(); });

                count = Math.Min(count, 10);
                pigBankLogic(count * delayTime + 0.8f);
                return;
            }

            Transform endTrans = null;
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                endTrans = MergeMainController.Instance.rewardBtnTrans;
            }
            else
            {
                endTrans = UIHomeMainController.mainController.MainPlayTransform;
            }

            if (type == (int) UserData.ResourceId.Seal || type == (int) UserData.ResourceId.Dolphin || type == (int) UserData.ResourceId.Capybara || type == (int) UserData.ResourceId.TeamCoin)
            {
                endTrans = MergeMainController.Instance.backTrans;
            }

            FlyGameObjectManager.Instance.FlyObject(type, scrPos, endTrans, 0.8f, 0.8f, () =>
            {
                FlyGameObjectManager.Instance.PlayHintStarsEffect(endTrans.position);
                Animator shake = endTrans.transform.GetComponent<Animator>();
                if (shake != null)
                    shake.Play("shake", 0, 0);

                if (isEnd)
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);

                action();
            }, true, 1.0f, 0.7f, true);

            pigBankLogic(0.8f + 0.3f);
        }

        public void CompleteTask(StorageTaskItem taskItem)
        {
            StorageTaskGroup.CurTasks.Remove(taskItem);

            StorageTaskGroup.LastFinishOrder[taskItem.Slot] = taskItem;

            StorageTaskGroup.CompleteOrderNum++;
            
            if(taskItem.Type < (int)MainOrderType.Random1 || taskItem.Type > (int)MainOrderType.Recomment 
                                                          || OrderConfigManager.Instance._orderCreates.Find(a=>a.id == taskItem.Id) != null)
            {
                if (!StorageTaskGroup.CompletedTaskIds.ContainsKey(taskItem.Id))
                    StorageTaskGroup.CompletedTaskIds.Add(taskItem.Id, taskItem.Id);
            }
            
            EventDispatcher.Instance.SendEventImmediately<EventCompleteTask>(new EventCompleteTask(taskItem));
        }

        public Dictionary<int, int> GetTaskDemands(StorageTaskItem curStorageTaskItem)
        {
            if (curStorageTaskItem == null)
                return null;

            Dictionary<int, int> demands = new Dictionary<int, int>();
            for (int i = 0; i < curStorageTaskItem.ItemIds.Count; i++)
            {
                int id = curStorageTaskItem.ItemIds[i];
                int num = curStorageTaskItem.ItemNums[i];
                if (!demands.ContainsKey(id))
                    demands.Add(id, num);
                else
                {
                    demands[id] += num;
                }
            }

            return demands;
        }

        public bool IsDebugCompleteTask(int taskId)
        {
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
                return false;

            if (debugCompleteTaskIds.ContainsKey(taskId))
                return true;

            return false;
        }

        private void FlyDogCookies(int count, Vector3 scrPos,float multiValue)
        {
            if (!DogHopeModel.Instance.IsOpenActivity())
                return;

            float delayTime = 0.3f;
            if (count >= 5)
                delayTime = 0.1f;
            FlyGameObjectManager.Instance.FlyCurrency(DogHopeModel._dogCookiesId, count, scrPos, 1, false, delayTime,
                action:
                () => { });

            for (int i = 0; i < count; i++)
            {
                if (!UserData.Instance.IsResource(DogHopeModel._dogCookiesId))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDog,
                        itemAId = DogHopeModel._dogCookiesId,
                        isChange = true,
                        data1 = multiValue.ToString(),
                    });
                }
                // UserData.Instance.AddRes(DogHopeModel._dogCookiesId, 1,
                //         new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DogGet}, false);
            }
        }

        public static int ChangeTaskRewardType(int originalRewardType)
        {
            if (RecoverCoinModel.Instance.IsStart() && originalRewardType == (int) UserData.ResourceId.Coin)
                originalRewardType = (int) UserData.ResourceId.RecoverCoinStar;
            return originalRewardType;
        }

        public bool IsTaskNeedInLineItem(int id)
        {
            
            if (CurTaskList == null || CurTaskList.Count == 0)
                return false;

            for (int i = 0; i < CurTaskList.Count; i++)
            {
                if (MergeTaskTipsController.Instance != null &&
                    !MergeTaskTipsController.Instance.IsHaveTask(CurTaskList[i].Id))
                    continue;

                StorageTaskItem curStorageTaskItem = GetCurTaskItem(CurTaskList[i].Id);

                if (curStorageTaskItem == null)
                    continue;

                if (curStorageTaskItem.ItemIds == null || curStorageTaskItem.ItemIds.Count == 0)
                    continue;

                foreach (var value in curStorageTaskItem.ItemIds)
                {
                    if(id==value)
                        return true;
                }
            }

            return false;
        }

        public StorageTaskItem GetCurTaskItem(int id)
        {
            if (CurTaskList == null || CurTaskList.Count == 0)
                return null;

            return CurTaskList.Find(a => a.Id == id);
        }

        /// <summary>
        /// 获取任务中物品价值最高的物品
        /// </summary>
        public TableMergeItem GetTaskMaxPrice(StorageTaskItem taskItem)
        {
            TableMergeItem maxPriceItem = null;
            int tempPrice = 0;

            foreach (var itemId in taskItem.ItemIds)
            {
                var config = GameConfigManager.Instance.GetItemConfig(itemId);
                if (config == null)
                    continue;
                if (config.price > tempPrice)
                {
                    tempPrice = config.price;
                    maxPriceItem = config;
                }
            }

            return maxPriceItem;
        }

        public bool IsWaitRemoving(int index)
        {
            if (mergeRemoveIndexs == null || mergeRemoveIndexs.Count == 0)
                return false;

            return mergeRemoveIndexs.Contains(index);
        }
    }
}