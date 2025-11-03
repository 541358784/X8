using DragonU3DSDK.Storage;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using BiUtil = DragonPlus.GameBIManager;
namespace TMatch
{


    public class CurrencyModel : Manager<CurrencyModel>
    {
        private StorageDictionary<int, StorageSafeCount> _userCurrency =>
            StorageManager.Instance.GetStorage<StorageCurrencyTMatch>().UserResourceDic;

        static public BiEventAdventureIslandMerge.Types.Item ConvertResourceIdToBiItem(ResourceId resourceId)
        {
            switch (resourceId)
            {
                // case ResourceId.Gem:
                //     return BiEventAdventureIslandMerge.Types.Item.Diamond;
                // case ResourceId.Coin:
                //     return BiEventAdventureIslandMerge.Types.Item.Coin;
                // case ResourceId.Energy:
                //     return BiEventAdventureIslandMerge.Types.Item.Energy;
                // case ResourceId.Energy_Infinity:
                //     return BiEventAdventureIslandMerge.Types.Item.EnergyInfinite;
                // case ResourceId.Star:
                //     return BiEventAdventureIslandMerge.Types.Item.Key;
                // case ResourceId.BoxKey:
                //     return BiEventAdventureIslandMerge.Types.Item.Key;
                default:
                    DebugUtil.LogError($"ResourceId = {resourceId.ToString()}在BI表里未配置Item");
                    break;
            }

            return BiEventAdventureIslandMerge.Types.Item.Coin;
        }

        public void SetRes(ResourceId resourceId, int amount, bool dispathEvent = true)
        {
            var intResId = (int) resourceId;

            var preAmount = 0;
            if (_userCurrency.ContainsKey(intResId))
            {
                preAmount = _userCurrency[intResId].GetValue();
                _userCurrency[intResId].SetValue(amount);
            }
            else
            {
                var newSafeCount = new StorageSafeCount();
                newSafeCount.SetValue(amount);
                _userCurrency.Add(intResId, newSafeCount);
            }
        }

        public int GetRes(ResourceId resourceId)
        {
            return _userCurrency.ContainsKey((int) resourceId) ? _userCurrency[(int) resourceId].GetValue() : 0;
        }

        // public bool CanAford(ResourceId resourceId, int num)
        // {
        //     return GetRes(resourceId) >= num;
        // }
        //
        // public bool CostRes(ResourceId resourceId, int amount, BiUtil.ItemChangeReasonArgs args, bool disEvent = true)
        // {
        //     if (amount > 0)
        //     {
        //         var intResId = (int) resourceId;
        //
        //         if (_userCurrency.ContainsKey(intResId) && _userCurrency[intResId].GetValue() >= amount)
        //         {
        //             var preAmount = _userCurrency[intResId].GetValue();
        //             // 当前值不包括变化值,所以在修改前发
        //             var item = ConvertResourceIdToBiItem(resourceId);
        //             _userCurrency[intResId].SetValue(preAmount - amount);
        //             var newCurrent = _userCurrency[intResId].GetValue();
        //             BiUtil.SendItemChangeEvent(item, -amount, (ulong) newCurrent, args);
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }

        // public void AddRes(ResourceId resourceId, int amount, BiUtil.ItemChangeReasonArgs args, bool disEvent = true)
        // {
        //     if (amount > 0)
        //     {
        //         var intResId = (int) resourceId;
        //
        //         // 无限体力立即生效,不进背包
        //         if (resourceId == ResourceId.Energy_Infinity)
        //         {
        //             EnergyModel.Instance.SetEnergyUnlimitedTime(amount * 1000, args);
        //             return;
        //         }
        //
        //         var preAmount = 0;
        //         if (_userCurrency.ContainsKey(intResId))
        //         {
        //             preAmount = _userCurrency[intResId].GetValue();
        //             _userCurrency[intResId].SetValue(preAmount + amount);
        //         }
        //         else
        //         {
        //             var newSafeCount = new StorageSafeCount();
        //             newSafeCount.SetValue(amount);
        //             _userCurrency.Add(intResId, newSafeCount);
        //         }
        //
        //         var newCurrent = _userCurrency[intResId].GetValue();
        //         var item = ConvertResourceIdToBiItem(resourceId);
        //         //args = ItemChangeReasonAddInfo(args, resourceId);
        //         // 当前值不包括变化值,所以扣掉变化值
        //         BiUtil.SendItemChangeEvent(item, amount, (ulong) newCurrent, args);
        //     }
        // }
    }
}