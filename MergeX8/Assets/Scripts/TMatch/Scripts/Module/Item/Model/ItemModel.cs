using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
// using PlayerInfo;
using UnityEngine;

/// <summary>
/// 物品模块
/// </summary>
namespace TMatch
{


    public partial class ItemModel : Manager<ItemModel>
    {
        public class ItemRewards
        {
            public readonly List<int> ItemIds = new List<int>();
            public readonly List<int> ItemNumbers = new List<int>();
        }

        private readonly Dictionary<int, ItemConfig> _itemConfigById = new Dictionary<int, ItemConfig>();
        private readonly Dictionary<string, ItemConfig> _itemConfigByName = new Dictionary<string, ItemConfig>();
        private StorageCurrencyTMatch _data => StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        private readonly List<int> _removeIDs = new List<int>();
        private float _lastUpdateTime = 0f;
        private const float UpdateInterval = 1f;
        private Dictionary<string, int> _energyDic = new Dictionary<string, int>();
        private Dictionary<string, Sprite> _itemSprites = new Dictionary<string, Sprite>();

        private StorageItemData Create(int id)
        {
            StorageItemData itemData = new StorageItemData() {Id = id, Count = 0, Timestamp = 0};
            _data.Items.Add(id, itemData);
            return itemData;
        }

        private StorageItemData GetData(int id, bool create = false)
        {
            if (_data.Items.TryGetValue(id, out StorageItemData data))
            {
                return data;
            }

            return create ? Create(id) : null;
        }

        private void Dirty(int id, int num = -1, ulong timestamp = 0)
        {
            StorageItemData data = GetData(id, true);
            if (num != -1)
            {
                data.Count = num;
            }

            if (timestamp != 0)
            {
                data.Timestamp = timestamp;
            }
        }

        private void Dirty(int id, int delta, int num, bool dispatchEvent)
        {
            Dirty(id, num);
            EventDispatcher.Instance.DispatchEvent(new ItemChangeEvent(id, delta, num));
            if (dispatchEvent) EventDispatcher.Instance.DispatchEvent(new ItemUpdateEvent(id, delta, num));
        }

        private void Dirty(int id, ulong timestamp, bool dispatchEvent)
        {
            Dirty(id, -1, timestamp);
            EventDispatcher.Instance.DispatchEvent(new ItemChangeEvent(id, -1, (int) timestamp));
            if (dispatchEvent) EventDispatcher.Instance.DispatchEvent(new ItemDurationUpdateEvent(id, timestamp));
        }

        private void Dirty(int sid, int id, int delta, ulong timestamp, bool dispatchEvent)
        {
            var data = GetData(sid, true);
            data.Id = id;
            data.Count = delta;
            data.Timestamp = timestamp;
            EventDispatcher.Instance.DispatchEvent(new ItemChangeEvent(sid, delta, (int) timestamp));
            if (dispatchEvent) EventDispatcher.Instance.DispatchEvent(new ItemUpdateEvent(sid, delta, delta));
        }

        private int GenerateSid()
        {
            if (_data.Sid < 1000000)
                _data.Sid = 1000000;
            return ++_data.Sid;
        }

        private int GetEnergyMax()
        {
            var num = 0;
            foreach (var key in _energyDic.Keys)
            {
                if (_energyDic[key] < 0)
                {
                    DebugUtil.LogWarning($"Add energy max {_energyDic[key]} by {key}, Please check it.");
                    continue;
                }

                num += _energyDic[key];
            }

            return num;
        }

        private void AddDuration(int id, int duration, bool dispatchEvent)
        {
            Dirty(id,
                (ulong) duration * 1000 +
                (!IsInDuration(id) ? APIManager.Instance.GetServerTime() : GetData(id).Timestamp),
                dispatchEvent);
        }

        private void ClearData()
        {
            foreach (var element in _data.Items)
            {
                if (element.Value.Timestamp != 0 && element.Value.Timestamp < APIManager.Instance.GetServerTime())
                {
                    ItemConfig cfg = GetConfigById(element.Value.Id);
                    if (cfg == null) continue;
                    // if (cfg.type == (int) ItemType.AddMaxLimit)
                    // {
                    //     _removeIDs.Add(element.Key);
                    // }
                }
            }

            foreach (var id in _removeIDs)
            {
                if (_data.Items.ContainsKey(id))
                {
                    var config = GetConfigById(_data.Items[id].Id);
                    _data.Items.Remove(id);
                    EventDispatcher.Instance.DispatchEvent(new ItemMaxUpdateEvent(config.param[0],
                        GetItemMax(config.param[0])));
                }
            }

            _removeIDs.Clear();
        }

        private void Update()
        {
            if (Time.time - _lastUpdateTime > UpdateInterval)
            {
                _lastUpdateTime = Time.time;

                ClearData();
            }
        }

        private ItemType ToBIItemId(int id)
        {
            ItemConfig cfg = GetConfigById(id);
            return (ItemType) cfg.type;
        }
        // /// <summary>
        // /// 物品ID转BIItem
        // /// </summary>
        // /// <param name="id">物品Id</param>
        // /// <returns></returns>
        // private BiEventAdventureIslandMerge.Types.Item ToBIItemId(int id)
        // {
        //     var config = GetConfigById(id);
        //     if (config.type == (int) ItemType.TMEnergyInfinity)
        //         return BiEventAdventureIslandMerge.Types.Item.EnergyInfinityTm;
        //     if (config.type == (int) ItemType.TMLightingInfinity)
        //         return BiEventAdventureIslandMerge.Types.Item.LightingInfinityTm;
        //     if (config.type == (int) ItemType.TMClockInfinity)
        //         return BiEventAdventureIslandMerge.Types.Item.ClockInfinityTm;
        //     if (config.type == (int) ItemType.TMInfinityActivityCollect)
        //         return BiEventAdventureIslandMerge.Types.Item.InfinityActivityCollectTm;
        //     if (config.type == (int) ItemType.TMWeeklyChallengeCollect)
        //         return BiEventAdventureIslandMerge.Types.Item.WeeklyChallengeCollectTm;
        //     if (config.type == (int) ItemType.TMWeeklyChallengeBuff)
        //         return BiEventAdventureIslandMerge.Types.Item.WeeklyChallengeBuffTm;
        //
        //     switch (id)
        //     {
        //         case 100001:
        //             return BiEventAdventureIslandMerge.Types.Item.CoinTm;
        //         case 100002:
        //             return BiEventAdventureIslandMerge.Types.Item.EnergyTm;
        //         case 100010:
        //             return BiEventAdventureIslandMerge.Types.Item.MagnetTm;
        //         case 100011:
        //             return BiEventAdventureIslandMerge.Types.Item.BroomTm;
        //         case 100012:
        //             return BiEventAdventureIslandMerge.Types.Item.WindmillTm;
        //         case 100013:
        //             return BiEventAdventureIslandMerge.Types.Item.FrozenTm;
        //         case 100014:
        //             return BiEventAdventureIslandMerge.Types.Item.LightingTm;
        //         case 100015:
        //             return BiEventAdventureIslandMerge.Types.Item.ClockTm;
        //         default:
        //             DebugUtil.LogError("Can't convert item id to BI Item! item id: " + id);
        //             return BiEventAdventureIslandMerge.Types.Item.Coin;
        //     }
        // }

        // /// <summary>
        // /// 增加体力上限
        // /// </summary>
        // /// <param name="key">来源</param>
        // /// <param name="value">值</param>
        // public void SetEnergyMax(string key, int value)
        // {
        //     // if (!_energyDic.ContainsKey(key))
        //     // {
        //     //     _energyDic.Add(key, 0);
        //     // }
        //
        //     _energyDic[key] = value;
        //     if (_energyDic[key] < 0)
        //     {
        //         DebugUtil.LogError($"Add energy max {_energyDic[key]} by {key}");
        //         _energyDic[key] = 0;
        //     }
        //
        //     var id = (int) ResourceId.Energy;
        //     EventDispatcher.Instance.DispatchEvent(new ItemMaxUpdateEvent(id, GetItemMax(id)));
        // }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public int GetNum(int id)
        {
            StorageItemData itemData = GetData(id);
            return itemData?.Count ?? 0;
        }

        /// <summary>
        /// 获取物品持续时间
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public ulong GetTimestamp(int id)
        {
            StorageItemData itemData = GetData(id);
            if (itemData == null)
            {
                return 0;
            }

            return itemData.Timestamp;
        }

        /// <summary>
        /// 获取物品倒计时
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public ulong GetCountdown(int id)
        {
            StorageItemData itemData = GetData(id);
            if (itemData == null)
            {
                return 0;
            }

            if (APIManager.Instance.GetServerTime() > itemData.Timestamp)
            {
                return 0;
            }

            return itemData.Timestamp - APIManager.Instance.GetServerTime();
        }

        /// <summary>
        /// 是否在持续时间内
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public bool IsInDuration(int id)
        {
            return GetTimestamp(id) >= APIManager.Instance.GetServerTime();
        }

        /// <summary>
        /// 是否足够
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="count">物品数量</param>
        /// <returns></returns>
        public bool IsEnough(int id, int count = 1)
        {
            return GetNum(id) >= count;
        }

        /// <summary>
        /// 增加物品
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="count">物品数量</param>
        /// <param name="args">BI数据</param>
        /// <param name="dispatchEvent">广播事件</param>
        /// <returns></returns>
        
        public bool Add(int id, int count, DragonPlus.GameBIManager.ItemChangeReasonArgs? args, bool dispatchEvent = false,int addType = 0)
        {
            if (count == 0)
            {
                return false;
            }

            ItemConfig cfg = GetConfigById(id);
            if (cfg == null)
            {
                DebugUtil.LogError($"Add item failed! Cause item {id} cfg not exist!");
                return false;
            }
            var arg = args ?? new DragonPlus.GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            // switch ((ResourceId) cfg.id) //关卡道具需要传人boosterId
            // {
            //     case ResourceId.Booster1:
            //     case ResourceId.Booster2:
            //     case ResourceId.Booster3:
            //         arg.boostId = (uint) id;
            //         break;
            // }

            if (cfg.type == (int) ItemType.TMEnergyInfinity)
            {
                var duration = EnergyModel.Instance.EnergyUnlimitedLeftTime();
                EnergyModel.Instance.AddEnergyUnlimitedTime(cfg.infiniityTime * (long) count * 1000, arg);
                DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), cfg.infiniityTime * 1000, (ulong) duration, arg);
            }
            else if (cfg.type == (int) ItemType.TMLightingInfinity)
            {
                var duration = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMLightingInfinity);
                UnlimitItemModel.Instance.AddUnlimitedItemTime(ItemType.TMLightingInfinity,
                    cfg.infiniityTime * (long) count * 1000);
                DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), cfg.infiniityTime * 1000, (ulong) duration, arg);
            }
            else if (cfg.type == (int) ItemType.TMClockInfinity)
            {
                var duration = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMClockInfinity);
                UnlimitItemModel.Instance.AddUnlimitedItemTime(ItemType.TMClockInfinity,
                    cfg.infiniityTime * (long) count * 1000);
                DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), cfg.infiniityTime * 1000, (ulong) duration, arg);
            }
            else if (cfg.type == (int) ItemType.TMWeeklyChallengeBuff)
            {
                var duration = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMWeeklyChallengeBuff);
                UnlimitItemModel.Instance.AddUnlimitedItemTime(ItemType.TMWeeklyChallengeBuff,
                    cfg.infiniityTime * (long) count * 1000);
                DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), cfg.infiniityTime * 1000, (ulong) duration, arg);
            }
            // else if (cfg.type == (int) ItemType.AddDuration)
            // {
            //     ulong duration = GetCountdown(cfg.param[0]);
            //     long addDuration = cfg.param[1] * count;
            //     AddDuration(cfg.param[0], (int) addDuration, dispatchEvent);
            //
            //     DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), addDuration * 1000, duration, arg);
            //     // if (arg.reason == BiEventAdventureIslandMerge.Types.ItemChangeReason.None)
            //     // {
            //     //     CommonUtils.CatchErrorLog("Add item no change reason:" + new StackTrace());
            //     // }
            // }
            else
            {
                int num = GetNum(id);

                if (!cfg.ignoreMax)
                {
                    int max = GetItemMax(id);
                    if (num >= max)
                    {
                        return false;
                    }

                    if (num + count > max)
                    {
                        count = max - num;
                    }
                }
                int now = num + count;
                Dirty(id, count, now, dispatchEvent);
                EventDispatcher.Instance.DispatchEvent(new GameItemChangeEvent(id));
                DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                    .GameEventTmGetItem,data1:id.ToString(),data2:now.ToString(),data3:count.ToString());
                // switch ((ResourceId) id)
                // {
                //     case ResourceId.Coin:
                //         Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(
                //             DataCenter.NameConst.GetGoldCoins, DataCenter.eOperateType.Add, count));
                //         break;
                //     case ResourceId.BoxKey:
                //         // 心照不宣：钥匙在关卡结算时特殊处理，以便在因子关也能触发统计
                //         // Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(DataCenter.NameConst.CollectKeysInTotal,DataCenter.eOperateType.Add, count));
                //         break;
                //     case ResourceId.Gem:
                //         Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(
                //             GeneralGameMission.MissionConst.AcquireDiamond, DataCenter.eOperateType.Add, count));
                //         break;
                //     default:
                //         break;
                // }

                // if (cfg.type == (int) ItemType.AddMaxLimit)
                // {
                //     EventDispatcher.Instance.DispatchEvent(new ItemMaxUpdateEvent(cfg.param[0],
                //         GetItemMax(cfg.param[0])));
                // }
                DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), count, (ulong) now, arg);   
                // if (arg.reason == BiEventAdventureIslandMerge.Types.ItemChangeReason.None)
                // {
                //     CommonUtils.CatchErrorLog("Add item no change reason:" + new StackTrace());
                // }
            }

            return true;
        }

        /// <summary>
        /// 批量增加物品
        /// </summary>
        /// <param name="ids">Id列表</param>
        /// <param name="nums">数量列表</param>
        /// <param name="args">BI数据</param>
        /// <param name="dispatchEvent">事件分发</param>
        /// <returns></returns>
        public bool Add(List<int> ids, List<int> nums, DragonPlus.GameBIManager.ItemChangeReasonArgs? args, bool dispatchEvent = false,int addType = 0)
        {
            bool suc = true;
            for (int i = 0; i < ids.Count; i++)
            {
                suc &= Add(ids[i], nums[i], args, dispatchEvent,addType:addType);
            }

            return suc;
        }

        /// <summary>
        /// 增加独立物品
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="count">数量</param>
        /// <param name="duration">到期时间</param>
        /// <param name="args">BI数据</param>
        /// <param name="dispatchEvent">事件分发</param>
        /// <returns></returns>
        public int AddUniqueItem(int id, int count, ulong duration, DragonPlus.GameBIManager.ItemChangeReasonArgs args,
            bool dispatchEvent = false)
        {
            if (count == 0)
            {
                return -1;
            }

            var cfg = GetConfigById(id);
            if (cfg == null)
            {
                DebugUtil.LogError($"Add item failed! Cause item {id} cfg not exist!");
                return -1;
            }

            var sid = GenerateSid();
            Dirty(sid, id, count, duration, dispatchEvent);
            DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), count, (ulong) count, args);

            // if (cfg.type == (int) ItemType.AddMaxLimit)
            // {
            //     EventDispatcher.Instance.DispatchEvent(new ItemMaxUpdateEvent(cfg.param[0], GetItemMax(cfg.param[0])));
            // }

            return sid;
        }

        /// <summary>
        /// 移除独立物品
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="dispatchEvent">事件分发</param>
        public void RemoveUniqueItem(int id, bool dispatchEvent = false)
        {
            var data = GetData(id);

            if (data != null)
            {
                _data.Items.Remove(id);

                EventDispatcher.Instance.DispatchEvent(new ItemChangeEvent(id, -data.Count, 0));
                if (dispatchEvent) EventDispatcher.Instance.DispatchEvent(new ItemUpdateEvent(id, -data.Count, 0));
            }
        }

        /// <summary>
        /// 消耗物品
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="count">物品数量</param>
        /// <param name="args"></param>
        /// <param name="dispatchEvent">广播事件</param>
        /// <param name="costSuccess">货币不足，购买完货币后的回调，正常回调不走</param>
        /// <returns></returns>
        public bool Cost(int id, int count, DragonPlus.GameBIManager.ItemChangeReasonArgs? args, bool dispatchEvent = false,
            System.Action costSuccess = null)
        {
            if (count < 0) return false;

            // 如果有无限体力，这里单独处理
            if (GetCountdown(id) > 0) return true;
            var arg = args ?? new DragonPlus.GameBIManager.ItemChangeReasonArgs
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            var num = GetNum(id);
            if (count > num)
            {
                DebugUtil.LogError($"Cost item {id} failed! Cause item num is not enough!");
                // UICurrencyExchange.Open(new UICurrencyExchangeData()
                // {
                //     itemId = id,
                //     itemCount = count - num,
                //     onFinish = () =>
                //     {
                //         if (Cost(id, count, args, true)) costSuccess?.Invoke();
                //     },
                //     source = arg.reason.ToString()
                // });
                return false;
            }

            // switch ((ResourceId) id) //关卡道具需要传人boosterId
            // {
            //     case ResourceId.Booster1:
            //     case ResourceId.Booster2:
            //     case ResourceId.Booster3:
            //         arg.boostId = (uint) id;
            //         break;
            // }

            int now = num - count;
            Dirty(id, -count, now, dispatchEvent);
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                .GameEventTmUseItem,data1:id.ToString(),data2:now.ToString(),data3:count.ToString());
            // switch ((ResourceId) id)
            // {
            //     case ResourceId.Coin:
            //         Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(
            //             GeneralGameMission.MissionConst.CostGold, DataCenter.eOperateType.Add, count));
            //         break;
            //     case ResourceId.Gem:
            //         Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(
            //             GeneralGameMission.MissionConst.CostDiamond, DataCenter.eOperateType.Add, count));
            //         break;
            //     default:
            //         break;
            // }

            DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), -count, (ulong) now, arg);
            if (args == null)
            {
                DebugUtil.LogError("No item change reason in cost!");
            }

            return true;
        }

        /// <summary>
        /// 清理物品
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="args">ItemChangeReason</param>
        /// <param name="dispatchEvent">广播事件</param>
        public void Clear(int id, DragonPlus.GameBIManager.ItemChangeReasonArgs args, bool dispatchEvent = false)
        {
            if (GetData(id) == null)
                return;

            var num = GetNum(id);
            if (num <= 0)
                return;

            Dirty(id, -num, 0, dispatchEvent);
            DragonPlus.GameBIManager.Instance.SendTMItemChangeEvent(ToBIItemId(id), -num, 0, args);
        }

        /// <summary>
        /// 获取物品上限
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public int GetItemMax(int id)
        {
            ItemConfig cfg = GetConfigById(id);
            if (cfg == null)
            {
                DebugUtil.LogError($"Get item max failed! Cause item cfg {id} not exist!");
                return 0;
            }

            if (cfg.max == 0)
            {
                return int.MaxValue;
            }

            int add = 0;
            foreach (StorageItemData data in _data.Items.Values)
            {
                if (data.Count <= 0) continue;

                ItemConfig itemCfg = GetConfigById(data.Id);
                if (itemCfg != null)
                {
                    // if (itemCfg.type == (int) ItemType.AddMaxLimit && itemCfg.param[0] == id)
                    // {
                    //     add += itemCfg.param[1] * data.Count;
                    // }
                }
            }

            // if (id == (int) ResourceId.Energy)
            // {
            //     add += GetEnergyMax();
            // }
            if (id == (int)ResourceId.TMEnergy)
            {
                return TMBPModel.Instance.GetTMEnergyMaxNum() + add;
            }
            return cfg.max + add;
        }

        /// <summary>
        /// 是否物品上限
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public bool IsNumMax(int id)
        {
            return GetNum(id) >= GetItemMax(id);
        }

        /// <summary>
        /// 获取物品价格（钻石）
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public int GetItemPrice(int id)
        {
            ItemConfig cfg = GetConfigById(id);
            if (cfg == null)
            {
                DebugUtil.LogError($"Get item price failed! Cause item cfg {id} not exist!");
                return 0;
            }

            return cfg.price;
        }

        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="count">物品数量</param>
        /// <param name="costEvent">消耗事件分发</param>
        /// <param name="addEvent">增加事件分发</param>
        /// <returns></returns>
        public bool Buy(int id, int count, DragonPlus.GameBIManager.ItemChangeReasonArgs? args = null,
            DragonPlus.GameBIManager.ItemChangeReasonArgs? costArgs = null, bool costEvent = false, bool addEvent = false)
        {
            ItemConfig cfg = GetConfigById(id);
            if (cfg == null)
            {
                DebugUtil.LogError($"Buy item failed! Cause item cfg {id} not exist!");
                return false;
            }

            if (cfg.exchange == 0 || cfg.price == 0)
            {
                DebugUtil.LogError($"Buy item failed! Cause item {id} not for sale!");
                return false;
            }

            int cost = count * cfg.price;
            if (!IsEnough(cfg.exchange, cost))
            {
                DebugUtil.LogError($"Buy item failed! Cause diamond {cost} not enough!");
                return false;
            }

            DragonPlus.GameBIManager.ItemChangeReasonArgs costArg;
            if (costArgs != null)
            {
                costArg = costArgs.Value;

            }
            else
            {
                costArg = new DragonPlus.GameBIManager.ItemChangeReasonArgs();
                costArg.reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug;
                costArg.data1 = $"{id}";
                costArg.data2 = $"{count}";
            }

            if (Cost(cfg.exchange, cost, costArg, costEvent))
            {
                Add(id, count, args, addEvent,addType:1);
                return true;
            }

            return false;
        }

        // /// <summary>
        // /// 获取随机奖励
        // /// 仅仅是获取数据并没有实际增加
        // /// </summary>
        // /// <param name="package">包ID</param>
        // /// <returns></returns>
        // public ItemRewards GetRandomRewards(int package)
        // {
        //     var sum = 0;
        //     var result = new ItemRewards();
        //     var rewards = new List<DragonPlus.Config.Game.ItemRewards>();
        //     foreach (var reward in GameConfigManager.Instance.ItemRewardsList)
        //     {
        //         if (reward.Package == package)
        //         {
        //             rewards.Add(reward);
        //             sum += reward.Value;
        //         }
        //     }
        //
        //     int value = 0;
        //     int random = Random.Range(0, sum);
        //     foreach (var reward in rewards)
        //     {
        //         value += reward.Value;
        //         if (value >= random)
        //         {
        //             result.ItemIds.AddRange(reward.ItemIds);
        //             result.ItemNumbers.AddRange(reward.ItemNums);
        //             break;
        //         }
        //     }
        //
        //     return result;
        // }

        /// <summary>
        /// 清理
        /// </summary>
        public void Release()
        {
            _energyDic.Clear();
        }

        /// <summary>
        /// 通过Id获取物品配置
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public ItemConfig GetConfigById(int id)
        {
            return _itemConfigById.ContainsKey(id) ? _itemConfigById[id] : null;
        }

        /// <summary>
        /// 通过名字获取物品配置
        /// </summary>
        /// <param name="n">物品名字</param>
        /// <returns></returns>
        public ItemConfig GetConfigByName(string n)
        {
            return _itemConfigByName.ContainsKey(n) ? _itemConfigByName[n] : null;
        }

        /// <summary>
        /// 初始化物品配置
        /// </summary>
        public void InitCfg()
        {
            _itemConfigById.Clear();
            _itemConfigByName.Clear();
            foreach (var itemConfig in TMatchShopConfigManager.Instance.ItemConfigList)
            {
                _itemConfigById[itemConfig.id] = itemConfig;
                _itemConfigByName[itemConfig.name] = itemConfig;
            }
        }

        /// <summary>
        /// 获取道具图标
        /// </summary>
        /// <param name="itemId">道具Id</param>
        /// <param name="isSmall">是否获取小图标</param>
        /// <returns></returns>
        public Sprite GetItemSprite(int itemId, bool isSmall = true)
        {
            _itemConfigById.TryGetValue(itemId, out ItemConfig config);
            if (config == null) return null;
            string spriteName = isSmall ? config.pic_res : config.pic_res_big;
            if (!_itemSprites.ContainsKey(spriteName))
                _itemSprites.Add(spriteName,
                    CommonUtils.GetItemSprite(itemId, isSmall ? ResourceIconType.Normal : ResourceIconType.Big));
            return _itemSprites[spriteName];
        }

        /// <summary>
        /// 生成当前物品预览数据（带换算，如果有重复物品不精确，需要精确再说）
        /// </summary>
        /// <param name="lstIds">物品Ids</param>
        /// <param name="lstCounts">物品Counts</param>
        /// <param name="feedbackConverter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> PreviewItemsWithExchangeable<T>(List<int> lstIds, List<int> lstCounts,
            IFeedbackItemConverter<T> feedbackConverter)
        {
            var previewList = new List<T>();
            for (var index = 0; index < lstIds.Count; index++)
            {
                var id = lstIds[index];
                var count = lstCounts[index];

                var itemCfg = GetConfigById(id);
                if (null == itemCfg)
                {
                    DebugUtil.LogError("itemCfg is Null " + id);
                    continue;
                }


                //判定是否存在持有上限，如果存在则根据汇率拆分为其他道具
                // if (itemCfg.HoldingLimit > 0) // <= 0 表示无上限
                // {
                //     //根据现在持有量判定溢出量
                //     var holdingCount = GetNum(id);
                //     var overFlowCount = count - Mathf.Max( itemCfg.HoldingLimit - holdingCount, 0); //溢出量
                //     var acceptCount = count - overFlowCount; //还能接受数量
                //     if (acceptCount > 0)
                //     {
                //         InnerPreviewItemsWithExchangeable(id, acceptCount, previewList, feedbackConverter);
                //     }
                //
                //     if (overFlowCount > 0) //根据溢出量和汇率计算拆分物品
                //     {
                //         for (var i = 0; i < itemCfg.ExchangeItemIds.Count(); ++i)
                //         {
                //             var exchangeItemId = itemCfg.ExchangeItemIds[i];
                //             var exchangeItemCount = itemCfg.ExchangeItemCounts[i];
                //             var exchangeItemCfg = GetConfigById(exchangeItemId);
                //             if (null == exchangeItemCfg)
                //             {
                //                 DebugUtil.LogError($"ExchangeItemCfg itemCfg is Null id : {id} exchangeItemId : {exchangeItemId}");
                //                 continue;
                //             }
                //             UnityEngine.Debug.Assert(exchangeItemCfg.HoldingLimit <= 0, $"exchange Item Is Not Support Hold Limit : id {id} exchangeItemId {exchangeItemId}");
                //             
                //             InnerPreviewItemsWithExchangeable(exchangeItemId, exchangeItemCount * overFlowCount, previewList, feedbackConverter);
                //         }
                //     }
                // }
                // else
                // {
                InnerPreviewItemsWithExchangeable(id, count, previewList, feedbackConverter);
                // }
            }

            return previewList;
        }

        /// <summary>
        /// 生成预览数据Inner
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="feedbackList"></param>
        /// <param name="feedbackConverter"></param>
        /// <typeparam name="T"></typeparam>
        private void InnerPreviewItemsWithExchangeable<T>(int id, int count, List<T> feedbackList,
            IFeedbackItemConverter<T> feedbackConverter)
        {
            UnityEngine.Debug.Assert(!((feedbackList != null) ^ (feedbackConverter != null)));
            if (feedbackList != null)
            {
                var feedbackItem = feedbackList.FirstOrDefault(m => feedbackConverter.IsMapping(id, m));
                if (feedbackItem != null)
                {
                    feedbackConverter.ConvertAddTo(feedbackItem, count);
                }
                else
                {
                    feedbackList.Add(feedbackConverter.ConvertCreate(id, count));
                }
            }
        }

        // /// <summary>
        // /// 是否是头像类型的道具
        // /// </summary>
        // /// <param name="id">道具id</param>
        // /// <returns></returns>
        // public bool IsPortrait(int id)
        // {
        //     return GetConfigById(id).type == (int) ItemType.Portrait;
        // }
        //
        // /// <summary>
        // /// 是否是头像框类型的道具
        // /// </summary>
        // /// <param name="id">道具id</param>
        // /// <returns></returns>
        // public bool IsPortraitFrame(int id)
        // {
        //     return GetConfigById(id).type == (int) ItemType.PortraitFrame;
        // }
        //
        // /// <summary>
        // /// 是否是时装类型道具
        // /// </summary>
        // /// <param name="id">道具id</param>
        // /// <returns></returns>
        // public bool IsAvatar(int id)
        // {
        //     return GetConfigById(id).type == (int) ItemType.Avatar;
        // }
        //
        // /// <summary>
        // /// 是否是宠物类型道具
        // /// </summary>
        // /// <param name="id"></param>
        // /// <returns></returns>
        // public bool IsPet(int id)
        // {
        //     return GetConfigById(id).type == (int) ItemType.Pet;
        // }

        // /// <summary>
        // /// 获取时装类型
        // /// </summary>
        // /// <param name="id">道具id</param>
        // /// <returns></returns>
        // public AvatarType GetAvatarType(int id)
        // {
        //     return (AvatarType) GetConfigById(id).Param[0];
        // }

        // /// <summary>
        // /// 获取时装列表
        // /// </summary>
        // /// <param name="type">时装类型</param>
        // /// <returns></returns>
        // public List<ItemConfig> GetAvatarList(AvatarType type)
        // {
        //     var list = new List<ItemConfig>();
        //     foreach (var itemConfig in GameConfigManager.Instance.ItemConfigList)
        //     {
        //         if (IsAvatar(itemConfig.Id) && itemConfig.Param[0] == (int) type)
        //         {
        //             list.Add(itemConfig);
        //         }
        //     }
        //
        //     return list;
        // }

        // /// <summary>
        // /// 是否是局内道具类型
        // /// </summary>
        // /// <param name="id">道具id</param>
        // /// <returns></returns>
        // public bool IsBooster(int id)
        // {
        //     return GetConfigById(id).type == (int) ItemType.Booster;
        // }

        /// <summary>
        /// 通过类型获取物品列表
        /// </summary>
        /// <param name="type">物品类型</param>
        /// <returns></returns>
        public List<ItemConfig> GetItemsByType(ItemType type)
        {
            return TMatchShopConfigManager.Instance.ItemConfigList.FindAll(item => item.type == (int) type);
        }
    }
}
