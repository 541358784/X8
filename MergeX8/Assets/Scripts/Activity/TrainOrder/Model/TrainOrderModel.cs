using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TrainOrder
{
    public class TrainOrderModel : ActivityEntityBase
    {

        /// <summary>
        /// Debug完成状态
        /// </summary>
        public bool DebugComplete = false;
        
        
        public List<int> BuildPos = new List<int>() { 20, 24 };


        public const int MAX_PROGRESS = 6;
        
        public const int BOARD_WIDTH = 5;
        public const int BOARD_HEIGHT = 5;

        private static TrainOrderModel _instance;
        public static TrainOrderModel Instance => _instance ?? (_instance = new TrainOrderModel());

        public override string Guid => "OPS_EVENT_TYPE_TRAIN_ORDER";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitAuto()
        {
            Instance.Init();
        }

        public StorageTrainOrder Storage => StorageManager.Instance.GetStorage<StorageHome>().TrainOrder;

        public TrainOrderLevel CurLevel => _curLevel;
        public List<TrainOrderOrderGroup> CurGroups => _curGroups;
        public TrainOrderOrderGroup CurGroup => _curGroup;
        public List<TrainOrderOrder> CurOrders => _curOrders;


        private List<TrainOrderLevel> _trainOrderLevels = new List<TrainOrderLevel>();

        //当前关卡
        private TrainOrderLevel _curLevel;

        //当前 任务组
        private TrainOrderOrderGroup _curGroup;


        private List<TrainOrderOrderGroup> _curGroups = new List<TrainOrderOrderGroup>();

        //当前任务
        private List<TrainOrderOrder> _curOrders = new List<TrainOrderOrder>();

        public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
            ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
        {
            base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
                activitySubType);
            TrainOrderConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }


        protected override void InitServerDataFinish()
        {
            if (!IsInitFromServer())
                return;

            if (Storage.ActivityId != ActivityId)
                Storage.Clear();

            if (Storage.ActivityId == ActivityId)
            {
                RefreshLevel();
            }
            else
            {
                Storage.ActivityId = ActivityId;
                Storage.GroupId = PayLevelModel.Instance.GetCurPayLevelConfig().TrainOrderGroupId;
                //容错
                if (TrainOrderConfigManager.Instance.TrainOrderLevelList.Find(p=>p.GroupId==Storage.GroupId)==null)
                    Storage.GroupId = TrainOrderConfigManager.Instance.TrainOrderLevelList.First().GroupId;
                Storage.CurLevelIndex = 0;
                Storage.CurOrderGroupIndex = 0;
                Storage.NeedDelayRefreshOrder = true;
                RefreshLevel();
            }
        }

        public override bool IsOpened(bool hasLog = false)
        {
            return base.IsOpened(hasLog) &&UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TrainOrder);
        }


        /// <summary>
        /// 刷新关卡
        /// </summary>
        private void RefreshLevel()
        {
            _trainOrderLevels =
                TrainOrderConfigManager.Instance.TrainOrderLevelList.FindAll(p => p.GroupId == Storage.GroupId);
            _trainOrderLevels.Sort((a, b) => a.Id - b.Id);

            _curLevel = _trainOrderLevels[Storage.CurLevelIndex];

            _curGroups =
                TrainOrderConfigManager.Instance.TrainOrderOrderGroupList.FindAll(p =>
                    _curLevel.OrderGroupId.Contains(p.Id));

            if (Storage.NeedDelayRefreshOrder)
                return;
            
            RefreshOrder();
        }

        /// <summary>
        /// 刷新任务
        /// </summary>
        public void RefreshOrder()
        {
            _curGroup = TrainOrderConfigManager.Instance.TrainOrderOrderGroupList.Find(p =>
                p.Id == _curLevel.OrderGroupId[Storage.CurOrderGroupIndex]);
            _curOrders =
                TrainOrderConfigManager.Instance.TrainOrderOrderList.FindAll(p => _curGroup.OrderId.Contains(p.Id));

            //初始化任务相关状态数据
            _curOrders.ForEach((order =>
            {
                Storage.CurOrderState.TryAdd(order.Id, 0);

                for (var i = 0; i < order.MergeItemId.Count; i++)
                {
                    if (Storage.CurOrderItemState.Find(p =>
                            p.ItemId == order.MergeItemId[i] && p.OrderId == order.Id && p.Index == i) != null)
                        continue;

                    int itemId = order.MergeItemId[i];
                    bool hasExtraReward = order.ExtraLimitTime != null && order.ExtraLimitTime.Count > i &&
                                          order.ExtraLimitTime[i] > 0;
                    int limitTime = hasExtraReward ? order.ExtraLimitTime[i] : 0;
                    StorageTrainOrderItemState itemState = new StorageTrainOrderItemState();
                    itemState.ItemId = itemId;
                    itemState.Index = i;
                    itemState.State = 0;
                    itemState.OrderId = order.Id;
                    if (hasExtraReward)
                        itemState.ExtraEndTime = (long)APIManager.Instance.GetServerTime() + limitTime * 1000;
                    Storage.CurOrderItemState.Add(itemState);
                }
            }));
        }

        /// <summary>
        /// 刷新任务物品状态
        /// </summary>
        /// <param name="train"></param>
        /// <param name="mergeItem"></param>
        /// <param name="order"></param>
        /// <param name="orderItemId"></param>
        /// <param name="orderIndex"></param>
        public async void OrderItemComplete(TrainOrderItem train, TrainOrderMergeItem mergeItem, TrainOrderOrder order,
            int orderItemId, int orderIndex)
        {
            StorageTrainOrderItemState itemState =
                Storage.CurOrderItemState.Find(p =>
                    p.ItemId == orderItemId && p.Index == orderIndex && p.OrderId == order.Id && p.State == 0);

            if (itemState != null)
            {
                itemState.State = 1;
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventTrainorderCleanSubmit,
                    data1: _curLevel.Id.ToString(),
                    data2: order.Id.ToString(),
                    data3:orderItemId.ToString());
                EventDispatcher.Instance.DispatchEvent(EventEnum.TRAIN_ORDER_ORDER_REFRESH,MergeBoardEnum.TrainOrder);

                //尝试添加额外奖励
                TryAddExtraReward(itemState,mergeItem,order,orderIndex);
                //价值转换金币
                TryAddCoin(mergeItem,orderItemId,order);
                
                //订单是否完成
                bool orderComplete = true;
                //订单组是否完成
                bool groupComplete = false;
                //关卡是否完成
                bool levelComplete = false;

                //是否有下一关
                bool hasNextLevel = false;

                int groupCompleteIndex = Storage.CurOrderGroupIndex;
                TrainOrderOrderGroup groupConfig = TrainOrderConfigManager.Instance.TrainOrderOrderGroupList.Find(p =>
                    p.Id == _curLevel.OrderGroupId[groupCompleteIndex]);

                if (Storage.CurOrderState.TryGetValue(order.Id, out int orderState) && orderState == 0)
                {
                    for (var i = 0; i < order.MergeItemId.Count; i++)
                    {
                        int itemId = order.MergeItemId[i];
                        StorageTrainOrderItemState checkState =
                            Storage.CurOrderItemState.Find(p =>
                                p.ItemId == itemId && p.Index == i && p.State == 0 && p.OrderId == order.Id);
                        if (checkState != null)
                        {
                            orderComplete = false;
                            break;
                        }
                    }

                    if (orderComplete)
                    {
                        Storage.CurOrderState[order.Id] = 1;
                    }
                }
                else
                {
                    orderComplete = false;
                }

                if (orderComplete)
                {
                    Storage.Progress++;
                    groupComplete = IsCurGroupComplete();
                    levelComplete = IsCurLevelComplete();

                    //订单奖励添加
                    ResData resData = new ResData(order.OrderReward[0], order.OrderReward[1]);
                    UserData.Instance.AddRes(resData, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TrainorderTrainget), false);
                    GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventTrainorderCleanTrainfinish,
                        data1: order.Id.ToString());

                    //订单组奖励添加
                    if (groupComplete)
                    {
                        resData = new ResData(groupConfig.GroupReward[0], groupConfig.GroupReward[1]);
                        UserData.Instance.AddRes(resData, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TrainorderGroupget), false);
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventTrainorderCleanGroupfinish,
                            data1: groupConfig.Id.ToString());
                        
                        //关卡完成了刷新下一关卡或者活动完成
                        if (levelComplete)
                        {
                            hasNextLevel = TryRefreshNextLevel();
                        }
                        else
                        {
                            TryRefreshNextGroup();
                        }
                    }
                }

                await MergeItemFlyToOrder(mergeItem.MergeItem.transform, orderItemId);
                if (orderComplete)
                {
                    //刷新进度显示
                    if (UITrainOrderMainController.Instance != null)
                        UITrainOrderMainController.Instance.RefreshProgress();


                    //任务组是否完成
                    if (groupComplete)
                    {
                        TrainOrderGroupItem groupItem = null;
                        if (UITrainOrderMainController.Instance != null)
                        {
                            groupItem = UITrainOrderMainController.Instance.GetGroupItem(groupCompleteIndex);
                        }

                        FlyTrainReward(train, order);
                        await FlyTrainGroupReward(groupItem, groupConfig);
                        UIRoot.Instance.EnableEventSystem = true;
                        await train.TrainMoveOut();

                        if (levelComplete)
                        {
                            if (hasNextLevel)
                            {
                                Storage.IsInitLevelBuild = false;
                                Storage.Progress = 0;
                                //清空棋盘返还剩余物品金币
                                RecycleMergeItem((() =>
                                {
                                    if (UITrainOrderMainController.Instance != null)
                                    {
                                        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent
                                            .MERGE_REWARD_REFRESH);
                                        UITrainOrderMainController.Instance.RefreshView();
                                    }
                                }));

                                MergeManager.Instance.ClearMerBoard(MergeBoardEnum.TrainOrder);
                                //初始化棋盘
                                MergeManager.Instance.Refresh(MergeBoardEnum.TrainOrder);
                            }
                            else
                            {
                                Storage.IsDone = true;
                                RecycleMergeItem((() =>
                                {
                                    if (UITrainOrderMainController.Instance != null)
                                    {
                                        UITrainOrderMainController.Instance.AnimCloseWindow();
                                    }
                                }));
                            }
                        }
                        else
                        {
                            if (UITrainOrderMainController.Instance != null)
                            {
                                UITrainOrderMainController.Instance.InitOrderItem(_curOrders, true);
                            }
                        }
                    }
                    else
                    {
                        await FlyTrainReward(train, order);
                        train.TrainMoveOut();
                    }
                }
            }
        }


        private void TryAddExtraReward(StorageTrainOrderItemState itemState,TrainOrderMergeItem mergeItem,TrainOrderOrder order, int orderIndex)
        {
            //限时额外奖励
            if (itemState.ExtraEndTime > 0 &&
                itemState.ExtraEndTime - (long)APIManager.Instance.GetServerTime() > 0)
            {
                ResData resData = new ResData(order.ExtraRewardId[orderIndex], order.ExtraRewardNum[orderIndex]);
                UserData.Instance.AddRes(resData, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TrainorderSubmitget), false);
                List<RewardData> list = new List<RewardData>();
                RewardData rewardData = new RewardData();
                rewardData.gameObject = mergeItem.ExtraItem.gameObject;
                rewardData.numText = mergeItem.ExtraItem._numText;
                rewardData.image = mergeItem.ExtraItem._image;

                rewardData.UpdateReward(resData);
                list.Add(rewardData);


                FlyGameObjectManager.Instance.FlyObject(list, CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    () =>
                    {
                    }, needHide: false,ignoreEventSystem:true);
            } 
        }
        
        /// <summary>
        /// 提交订单后转换奖励
        /// </summary>
        /// <param name="mergeItem"></param>
        /// <param name="itemId"></param>
        /// <param name="order"></param>
        private void TryAddCoin(TrainOrderMergeItem mergeItem,int itemId,TrainOrderOrder order)
        {
            TableOrderItem item = OrderConfigManager.Instance.GetOrderItem(itemId);
            if (item != null)
            {
                ResData resData = new ResData(UserData.ResourceId.Coin,Mathf.CeilToInt(order.Coefficient*1f/100*item.price) );
                UserData.Instance.AddRes(resData, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TrainorderSubmitget), false);
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    UserData.ResourceId.Coin, resData.count,mergeItem.transform.position,0.5f,true,true);
            }
        }
        
        
        private void RecycleMergeItem(Action callback = null)
        {
            int coinCount = 0;

            StorageMergeBoard storageMergeBoard = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.TrainOrder);

            if (storageMergeBoard != null)
            {
                for (var i = 0; i < storageMergeBoard.Items.Count - 1; i++)
                {
                    if (storageMergeBoard.Items[i].Id > 0)
                    {
                        TableMergeItem item = GameConfigManager.Instance.GetItemConfig(storageMergeBoard.Items[i].Id);
                        if (item != null && item.sold_gold > 0)
                            coinCount += item.sold_gold;

                        MergeManager.Instance.RemoveBoardItem(i, MergeBoardEnum.TrainOrder,"TrainOrder_Recycle");
                    }
                }
            }

            if (coinCount > 0)
            {
                List<ResData> temp = new List<ResData>();
                temp.Add(new ResData((int)UserData.ResourceId.Coin, coinCount));
                CommonRewardManager.Instance.PopCommonReward(temp,
                    CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    true, new GameBIManager.ItemChangeReasonArgs(), clickGetCall: (
                        () => { callback?.Invoke(); }));
            }
            else
            {
                XUtility.WaitFrames(1, (() =>
                {
                    callback?.Invoke();

                }));
            }
        }
        
        private async Task FlyTrainReward(TrainOrderItem train, TrainOrderOrder order)
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();

            ResData resData = new ResData(order.OrderReward[0], order.OrderReward[1]);
            if (train != null)
            {
                List<RewardData> list = new List<RewardData>();
                RewardData rewardData = new RewardData();
                rewardData.gameObject = train.RewardItem.gameObject;
                rewardData.numText = train.RewardItem._numText;
                rewardData.image = train.RewardItem._image;

                rewardData.UpdateReward(resData);
                list.Add(rewardData);


                FlyGameObjectManager.Instance.FlyObject(list, CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    () =>
                    {
                        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                        waitTask.SetResult(true);
                    }, needHide: false,ignoreEventSystem:true);
            }
            else
            {
                waitTask.SetResult(true);
            }


            await waitTask.Task;
        }


        private async Task FlyTrainGroupReward(TrainOrderGroupItem train, TrainOrderOrderGroup group)
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();

            ResData resData = new ResData(group.GroupReward[0], group.GroupReward[1]);

            if (train != null)
            {
                train.RefreshView();
                List<RewardData> list = new List<RewardData>();
                
                GameObject rewardItemObj = GameObject.Instantiate( train.RewardItem.gameObject,  train.RewardItem.transform.parent);
                RewardData rewardData = new RewardData();
                rewardData.gameObject = rewardItemObj;
                rewardData.image =rewardItemObj.transform.Find("Icon").GetComponent<Image>();
                rewardData.numText =rewardItemObj.transform.Find("Text")?.GetComponent<LocalizeTextMeshProUGUI>();
                if (rewardData.numText==null)
                    rewardData.numText = rewardItemObj.transform.Find("Num")?.GetComponent<LocalizeTextMeshProUGUI>();
                rewardData.UpdateReward(resData);
                
                list.Add(rewardData);
                
                FlyGameObjectManager.Instance.FlyObject(list, CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    () =>
                    {
                        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                        list.ForEach((data =>
                        {
                            GameObject.Destroy(data.gameObject);

                        }));
                        waitTask.SetResult(true);
                    }, needHide: false,ignoreEventSystem:true);
            }
            else
            {
                waitTask.SetResult(true);
            }


            await waitTask.Task;
        }


        private async Task MergeItemFlyToOrder(Transform tranOrder, int orderItemId)
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            if (DebugComplete)
            {
                waitTask.SetResult(true);
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, (int) MergeBoardEnum.TrainOrder);
            }
            else
            {
                Dictionary<int, int> tempDic = new Dictionary<int, int>();
                tempDic.Add(orderItemId, 1);

                var indexList =
                    MergeManager.Instance.GetTaskCompleteItemIndex(tempDic, MergeBoardEnum.TrainOrder);
                for (int i = 0; i < indexList.Count; i++)
                {
                    MergeBoard.Grid grid =
                        UITrainOrderMainController.Instance.MergeBoard.GetGridByIndex(indexList[i]);
                    FlyGameObjectManager.Instance.FlyObject(indexList[i], grid.board.id,
                        grid.board.transform.position, tranOrder, 0.8f,
                        () =>
                        {
                            FlyGameObjectManager.Instance.PlayHintStarsEffect(tranOrder
                                .position);

                            waitTask.SetResult(true);
                        });
                    MergeManager.Instance.RemoveBoardItem(indexList[i], MergeBoardEnum.TrainOrder,"TrainOrder_Complete");
                }
            }
            
            await waitTask.Task;
        }


        private void TryRefreshNextGroup()
        {
            Storage.CurOrderGroupIndex++;
            RefreshOrder();
        }

        /// <summary>
        /// 是否是任务需要的物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool IsOrderNeedItem(int itemId)
        {
            if (!IsOpened())
                return false;

            return Storage.CurOrderItemState.Find(p => p.ItemId == itemId && p.State == 0) != null;
        }


        /// <summary>
        /// 任务是否完成
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderItemId"></param>
        /// <param name="orderIndex"></param>
        /// <returns></returns>
        public bool IsOrderItemComplete(TrainOrderOrder order, int orderItemId, int orderIndex)
        {
            return Storage.CurOrderItemState.Find(p =>
                p.ItemId == orderItemId && p.Index == orderIndex && p.OrderId == order.Id && p.State == 1) != null;
        }

        /// <summary>
        /// 任务是否完成
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderItemId"></param>
        /// <param name="orderIndex"></param>
        /// <returns></returns>
        public StorageTrainOrderItemState GetOrderItemState(TrainOrderOrder order, int orderItemId, int orderIndex)
        {
            return Storage.CurOrderItemState.Find(p =>
                p.ItemId == orderItemId && p.Index == orderIndex && p.OrderId == order.Id);
        }


        /// <summary>
        /// 订单是否完成
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool IsOrderComplete(TrainOrderOrder order)
        {
            return Storage.CurOrderState.TryGetValue(order.Id, out int orderState) && orderState == 1;
        }

        /// <summary>
        /// 任务组是否完成
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool IsOrderGroupComplete(TrainOrderOrderGroup group)
        {
            return Storage.CurOrderGroupState.ContainsKey(group.Id);
        }


        /// <summary>
        /// 当前任务组是否完成
        /// </summary>
        /// <returns></returns>
        private bool IsCurGroupComplete()
        {
            if (Storage.CurOrderGroupState.ContainsKey(_curGroup.Id))
            {
                return true;
            }
            else
            {
                bool isGroupComplete = true;

                foreach (TrainOrderOrder order in CurOrders)
                {
                    if (!IsOrderComplete(order))
                    {
                        isGroupComplete = false;
                        break;
                    }
                }

                if (isGroupComplete)
                    Storage.CurOrderGroupState.TryAdd(_curGroup.Id, 1);

                return isGroupComplete;
            }
        }


        /// <summary>
        /// 当前关卡是否完成
        /// </summary>
        /// <returns></returns>
        private bool IsCurLevelComplete()
        {
            bool isComplete = true;

            foreach (int groupId in _curLevel.OrderGroupId)
            {
                if (!Storage.CurOrderGroupState.ContainsKey(groupId))
                {
                    isComplete = false;
                    break;
                }
            }

            return isComplete;
        }


        /// <summary>
        /// 尝试刷新下一关
        /// </summary>
        /// <returns></returns>
        private bool TryRefreshNextLevel()
        {
            int maxLevel = _trainOrderLevels.Count;
            if (Storage.CurLevelIndex + 1 >= maxLevel)
            {
                return false;
            }
            else
            {
                Storage.CurLevelIndex++;
                Storage.CurOrderGroupIndex = 0;
                RefreshLevel();
                return true;
            }
        }


        public bool CanShowEntrance()
        {
            return IsOpened() && !Storage.IsDone;
        }


        public void DebugReset()
        {
            Storage.Clear();
            InitServerDataFinish();
        }

        public void TryOpenMain()
        {
            if (Storage.IsPopupStart)
            {
                UITrainOrderMainController.Open();
            }
            else
            {
                Storage.IsPopupStart = true;
                UIPopupTrainOrderStartController.Open();
            }
        }
        
        
        public bool CanShowStart()
        {
            if (!IsOpened())
                return false;
            
            if (Storage.IsPopupStart)
                return false;

            //combo里配置先写死
            Storage.IsPopupStart = true;
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupTrainOrderStart);
            return true;
        }


        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.TrainOrder);
        }
    }
}