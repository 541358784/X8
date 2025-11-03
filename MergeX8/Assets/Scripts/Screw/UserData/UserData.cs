using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace Screw.UserData
{
    public class UserData : Manager<UserData>, IRunOnce
    {
        private StorageScrew storageScrew
        {
            get
            {
                return  StorageManager.Instance.GetStorage<StorageScrew>();
            }
        }
        
        public void OnRunOnce()
        {
            RunOnce.OnRunOnce("UserData_RunOnceKey", ExecutionRunOnce);
        }
        
        private void ExecutionRunOnce()
        {
        }
        
        public int GetRes(ResType resType)
        {
            string key = GetStorageResKey(resType);
            if (storageScrew.UserRes.TryGetValue(key, out var number))
            {
                return number.GetValue();
            }
            return 0;
        }
        
        public bool CanAfford(ResType resType, int resCount)
        {
            return GetRes(resType) >= resCount;
        }

        public string GetInfinityTimeString(long second)
        {
            var day = second / (long)XUtility.DayTime;
            second %= (long)XUtility.DayTime;
            var hour = second / (long)XUtility.Hour;
            second %= (long)XUtility.Hour;
            var min = second / (long)XUtility.Min;
            var str = "";
            if (day > 0)
                str += day + "d";
            if (hour > 0)
                str += hour + "h";
            if (min > 0)
                str += min + "m";
            return str;
        }
        public bool ConsumeRes(ResType resType, int resCount, GameBIManager.ItemChangeReasonArgs reason, bool isEvent = true)
        {
            if (!CanAfford(resType, resCount))
                return false;
            
            string key = GetStorageResKey(resType);
            if (!storageScrew.UserRes.ContainsKey(key))
                return false;
            
            var currentValue = storageScrew.UserRes[key].GetValue();
            storageScrew.UserRes[key].SetValue(currentValue - resCount);
            SendResChangeBI(resType, -resCount, reason);
        
            if(isEvent)
                EventDispatcher.Instance.DispatchEventImmediately(ConstEvent.SCREW_REFRESH_RES, resType, GetRes(resType), -resCount, true);
            
            return true;
        }

        public void AddRes(List<ResData> rewards, GameBIManager.ItemChangeReasonArgs reason,bool isEvent = true)
        {
            foreach (var reward in rewards)
            {
                AddRes(reward.id, reward.count, reason,isEvent);
            }
        }

        public void AddRes(int id, int count, GameBIManager.ItemChangeReasonArgs reason, bool isEvent = true)
        {
            if (!ItemDic.TryGetValue(id, out var itemConfig))
            {
                Debug.LogError("未找到ScrewItem配置"+id);
                return;   
            }
            var resType = (ResType)itemConfig.ItemType;
            var resCount = count;
            if (itemConfig.Infinity)
            {
                resCount *= itemConfig.InfinityTime;
                
            }
            AddRes(resType, resCount, reason, isEvent);
        }

        public static ResType GetResType(int id)
        {
            if (!ItemDic.TryGetValue(id, out var itemConfig))
            {
                Debug.LogError("未找到ScrewItem配置"+id);
                return ResType.None;
            }
            return (ResType)itemConfig.ItemType;
        }

        public static int GetFirstItemId(ResType resType)
        {
            var typeValue = (int)resType;
            foreach (var pair in ItemDic)
            {
                if (pair.Value.ItemType == typeValue)
                {
                    return pair.Key;
                }
            }
            Debug.LogError("未找到对应的ResType"+resType+" 请检查ScrewItem表");
            return -1;
        }

        private static Dictionary<int, DragonPlus.Config.Screw.TableItem> _itemDic;
        public static Dictionary<int, DragonPlus.Config.Screw.TableItem> ItemDic
        {
            get
            {
                if (_itemDic == null)
                {
                    _itemDic = new Dictionary<int, DragonPlus.Config.Screw.TableItem>();
                    var itemConfigs = DragonPlus.Config.Screw.GameConfigManager.Instance.TableItemList;
                    foreach (var config in itemConfigs)
                    {
                        _itemDic.Add(config.ItemId,config);
                    }
                }
                return _itemDic;
            }
        }
        public static Sprite GetResourceIcon(int id)
        {
            if (!ItemDic.TryGetValue(id, out var itemConfig))
            {
                Debug.LogError("未找到ScrewItemConfig"+id);
                return null;
            }
            return ResourcesManager.Instance.GetSpriteVariant(itemConfig.Atlas, itemConfig.Icon);
        }
        public void AddRes(ResType resType, int resCount, GameBIManager.ItemChangeReasonArgs reason, bool isEvent = true)
        {
            string key = GetStorageResKey(resType);
            if (resType == ResType.EnergyInfinity)
            {
                EnergyData.Instance.AddInfinityLeftTime(resCount);
            }
            else if (storageScrew.UserRes.ContainsKey(key))
            {
                var newCount = storageScrew.UserRes[key].GetValue() + resCount;
                newCount = Math.Max(0, newCount);
                storageScrew.UserRes[key].SetValue(newCount);
            }
            else
            {
                StorageUserRes storageRes = new StorageUserRes();
                storageRes.SetValue(resCount);
                storageScrew.UserRes.Add(key, storageRes);
            }

            SendResChangeBI(resType, resCount, reason);
            
            if(isEvent)
                EventDispatcher.Instance.DispatchEvent(ConstEvent.SCREW_REFRESH_RES, resType, GetRes(resType), resCount, true);
        }
        
        private string GetStorageResKey(ResType resType)
        {
            return "userreskey_" + (int) resType;
        }

        public static ResType SwitchBoostTypeToResType(BoosterType boosterType)
        {
            switch (boosterType)
            {
                case BoosterType.ExtraSlot:
                    return ResType.ExtraSlot;
                case BoosterType.BreakBody:
                    return ResType.BreakBody;
                case BoosterType.TwoTask:
                    return ResType.TwoTask;
            }
            return ResType.None;
        }
        
        private void SendResChangeBI(ResType resType, int resCount, GameBIManager.ItemChangeReasonArgs reason)
        {
            SendItemChangeEvent(resType, (long)resCount, (ulong)GetRes(resType), reason);
        }
        
        public long GetBuffLeftTime(ResType itemType)
        {
            long leftTime = 0;
            if (storageScrew.Buff != null && storageScrew.Buff.ContainsKey((int)itemType))
            {
                leftTime = storageScrew.Buff[(int)itemType] * 1000 - ScrewUtility.GetTimeStamp();
            }
            return leftTime > 0 ? leftTime : 0;
        }
        public bool InBuffTime(ResType itemType)
        {
            return GetBuffLeftTime(itemType) > 0;
        }

        public void SetBuffTime(ResType itemType, long timeMillSecond)
        {
            storageScrew.Buff[(int) itemType] = timeMillSecond;
        }
        public void AddBuffTime(ResType itemType, long timeMillSecond, GameBIManager.ItemChangeReasonArgs reason)
        {
            var leftTime = GetBuffLeftTime(itemType);
            if (storageScrew.Buff.ContainsKey((int)itemType))
            {
                if (leftTime > 0)
                {
                    storageScrew.Buff[(int)itemType] += (long)(timeMillSecond * 0.001);
                }
                else
                {
                    storageScrew.Buff[(int)itemType] = (long)((leftTime + timeMillSecond + ScrewUtility.GetTimeStamp()) * 0.001);
                }
            }
            else
            {
                storageScrew.Buff.Add((int)itemType, (long)((leftTime + timeMillSecond + ScrewUtility.GetTimeStamp()) * 0.001));
            }
            SendResChangeBI(itemType, (int) (timeMillSecond * 0.001), reason);
        }
        
        
        private static Dictionary<ResType, BiEventAdventureIslandMerge.Types.Item> _resourcesBiDict =
            new Dictionary<ResType, BiEventAdventureIslandMerge.Types.Item>()
            {
                {ResType.Coin, BiEventAdventureIslandMerge.Types.Item.CoinScrew},
                {ResType.Energy, BiEventAdventureIslandMerge.Types.Item.EnergyScrew},
                {ResType.EnergyInfinity, BiEventAdventureIslandMerge.Types.Item.EnergyInfiniteScrew},
                {ResType.ExtraSlot, BiEventAdventureIslandMerge.Types.Item.BonusslotScrew},
                {ResType.BreakBody, BiEventAdventureIslandMerge.Types.Item.HammerScrew},
                {ResType.TwoTask, BiEventAdventureIslandMerge.Types.Item.BonusmoduleScrew},
            };
        
        
        public void SendItemChangeEvent(ResType resType, long amount, ulong current,
            GameBIManager.ItemChangeReasonArgs args)
        {
            if (!_resourcesBiDict.ContainsKey(resType))
                return;
            try
            {
                var itemChangeEvent = new DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge.Types.ItemChange
                {
                    Item = _resourcesBiDict[resType],
                    Reason = args.reason,
                    Amount = amount,
                    Current = current,
                    BoostId = args.boostId,
                };
                if (!string.IsNullOrEmpty(args.data1))
                {
                    itemChangeEvent.Data1 = args.data1;
                }

                if (!string.IsNullOrEmpty(args.data2))
                {
                    itemChangeEvent.Data2 = args.data2;
                }

                if (!string.IsNullOrEmpty(args.data3))
                {
                    itemChangeEvent.Data3 = args.data3;
                }

                GameBIManager.Instance.SendBIEvent(itemChangeEvent);
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError("BI SendItemChangeEvent:" + e.Message);
                DebugUtil.LogError("BI SendItemChangeEvent:" + e.StackTrace);
            }
        }
    }
}