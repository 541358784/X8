using UnityEngine;
using DragonU3DSDK.Storage;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using Framework;
using DragonU3DSDK.Network.API.Protocol;
using BiUtil = DragonPlus.GameBIManager;
namespace TMatch
{


    // public enum EnergyState
    // {
    //     NotFull = 0, // 不满体力（非0）
    //     Full, // 满体力
    //     Zero, // 体力为0
    //     Unlimited // 无限体力
    // }

    public class EnergyModel : GlobalSystem<EnergyModel>, IUpdatable
    {
        private bool _isReady = false;

        private long UnlimitedDelayShowTime;
        private float _updateTime;

        // 无限体力状态
        bool IsEnergyUnlimitedState { get; set; }

        // 进入游戏时，记录当前是否为无限体力状态
        bool EnterGameIsUnlimitedState = false;

        // /// <summary>
        // /// 获取体力状态
        // /// </summary>
        // /// <returns></returns>
        // public EnergyState GetEnergyState()
        // {
        //     if (IsEnergyUnlimited()) return EnergyState.Unlimited;
        //     if (IsEnergyFull()) return EnergyState.Full;
        //     var energyNumber = EnergyNumber();
        //     if (energyNumber == 0) return EnergyState.Zero;
        //     return EnergyState.NotFull;
        // }

        // // 获取体力值
        // public int EnergyNumber()
        // {
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     return storage.Energy;
        // }

        // public long MilisecondsBeforeEnergyFull()
        // {
        //     if (IsEnergyFull())
        //     {
        //         return 0;
        //     }
        //
        //     int gap = MaxEnergy() - EnergyNumber();
        //
        //     return LeftAutoAddEnergyTime() + (gap - 1) *
        //         TMatchShopConfigManager.Instance.GlobalList[0].EnergyRefillTime * 1000;
        // }

        // // 体力是否满了
        // public bool IsEnergyFull()
        // {
        //     return EnergyNumber() >= MaxEnergy();
        // }

        // // 获取最大体力值
        // public int MaxEnergy()
        // {
        //     return 5;
        //     // return TMatch.GameConfigManager.Instance.GlobalConfig.MaxUserEnergy;
        // }

        // // 体力是否空了, 注意这个方法算了无限体力，也就是说在无限体力状态下， 体力肯定不是空的
        // public bool IsEnergyEmpty()
        // {
        //     return EnergyNumber() <= 0 && !IsEnergyUnlimited();
        // }
        
        // // 花费金币， 补满体力
        // public bool BuyEnergyWithCoin()
        // {
        //     if (IsEnergyFull())
        //     {
        //         return false;
        //     }
        //
        //     int needCoin = TMatchShopConfigManager.Instance.GlobalList[0].EnergyGemPrice;
        //     if (CurrencyModel.Instance.GetRes(ResourceId.Coin) < needCoin)
        //     {
        //         // 金币不够
        //         return false;
        //     }
        //
        //     // var changReasonArgs = new BiUtil.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.BuyItem);
        //     // changReasonArgs.data1 = ItemType.Energy.ToString();
        //     // changReasonArgs.data2 = (MaxEnergy() - EnergyNumber()).ToString();
        //     // // 局内要记录当前关卡信息
        //     // if (MyMain.myGame.IsInMatch())
        //     // {
        //     //     changReasonArgs.data3 = TMatchModel.Instance.GetMainLevel().ToString();
        //     // }
        //     // CurrencyModel.Instance.CostRes(ResourceId.Coin, needCoin, changReasonArgs);
        //     EventDispatcher.Instance.DispatchEvent(new ResChangeEvent(ResourceId.Coin));
        //     AddEnergy(MaxEnergy() - EnergyNumber(),
        //         new BiUtil.ItemChangeReasonArgs
        //         {
        //             reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug,
        //             data1 = needCoin.ToString()
        //         });
        //     return true;
        // }

        // // 免费加满体力
        // void SetEnergyFull()
        // {
        //     if (IsEnergyFull())
        //     {
        //         return;
        //     }
        //
        //     SetEnergy(MaxEnergy());
        // }

        // // 时间到了自动添加体力
        // void AutoAddEnergy()
        // {
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     long currentTime = CommonUtils.GetTimeStamp();
        //     long intervalTime = currentTime - storage.LastAddEnergyTime;
        //     long addEnergyInterval =
        //         TMatchShopConfigManager.Instance.GlobalList[0].EnergyRefillTime * 1000;
        //     long n = intervalTime / addEnergyInterval;
        //     long leftTime = intervalTime - addEnergyInterval * n;
        //     if (n > 0)
        //     {
        //         long addNum = TMatchShopConfigManager.Instance.GlobalList[0].EnergyRefillAmount * n;
        //         if (addNum + EnergyNumber() >= MaxEnergy())
        //         {
        //             addNum = MaxEnergy() - EnergyNumber();
        //             StorageAutoAddEnergyTime(currentTime);
        //         }
        //         else
        //         {
        //             StorageAutoAddEnergyTime(currentTime - leftTime);
        //         }
        //
        //         AddEnergy((int) addNum, new BiUtil.ItemChangeReasonArgs
        //         {
        //             reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug,
        //             data1 = addNum.ToString(),
        //         });
        //     }
        // }

        // 无限体力状态
        public bool IsEnergyUnlimited()
        {
            return EnergyUnlimitedLeftTime() > 0;
        }

        // 添加无限体力时间
        public void AddEnergyUnlimitedTime(long timeMillSecond, BiUtil.ItemChangeReasonArgs args)
        {
            var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
            storage.UnlimitEnergyEndUTCTimeInSeconds =
                (long) ((EnergyUnlimitedLeftTime() + timeMillSecond + CommonUtils.GetTimeStamp()) * 0.001);
            // BiUtil.SendItemChangeEvent(BiEventMatchFrenzy.Types.Item.EnergyInfinity, (int)(timeMillSecond * 0.001), (ulong)(EnergyUnlimitedLeftTime() * 0.001), args);
        }

        // // 直接设置无限体力时间而不是添加时长
        // public void SetEnergyUnlimitedTime(long timeMillSecond, BiUtil.ItemChangeReasonArgs args)
        // {
        //     if (timeMillSecond < 0)
        //     {
        //         return;
        //     }
        //
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     storage.UnlimitEnergyEndUTCTimeInSeconds = (long) ((timeMillSecond + CommonUtils.GetTimeStamp()) * 0.001);
        // }
        //
        // // 设置无限体力延迟显示时间
        // public void SetEnergyUnlimitedTimeDelay(long timeMillSecond)
        // {
        //     UnlimitedDelayShowTime = timeMillSecond;
        // }
        //
        // // 获取无限体力剩余显示时间
        // public long EnergyUnlimitedLeftShowTime()
        // {
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     long left = storage.UnlimitEnergyEndUTCTimeInSeconds * 1000 - CommonUtils.GetTimeStamp() -
        //                 UnlimitedDelayShowTime;
        //     return left > 0L ? left : 0;
        // }

        // 获取无限体力剩余时间
        public long EnergyUnlimitedLeftTime()
        {
            var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
            long left = storage.UnlimitEnergyEndUTCTimeInSeconds * 1000 - CommonUtils.GetTimeStamp();
            return left > 0L ? left : 0;
        }

        // // 添加体力值，无视体力上限
        // // 现在修改为不能超过体力上限
        // public bool AddEnergy(int addNum, BiUtil.ItemChangeReasonArgs args)
        // {
        //     if (addNum <= 0)
        //     {
        //         return false;
        //     }
        //
        //     if (EnergyNumber() == MaxEnergy()) return false;
        //     if (EnergyNumber() + addNum > MaxEnergy())
        //     {
        //         addNum = MaxEnergy() - EnergyNumber();
        //     }
        //
        //     SetEnergy(EnergyNumber() + addNum);
        //     // BiUtil.SendItemChangeEvent(BiEventMatchFrenzy.Types.Item.Energy, addNum, (ulong)EnergyNumber(), args);
        //     return true;
        // }

        // // 花费体力值
        // public bool CostEnergy(int costNum, BiUtil.ItemChangeReasonArgs args)
        // {
        //     if (costNum <= 0)
        //     {
        //         return false;
        //     }
        //
        //     if (IsEnergyUnlimited())
        //     {
        //         EnterGameIsUnlimitedState = true;
        //         return true;
        //     }
        //
        //     if (costNum > EnergyNumber())
        //     {
        //         return false;
        //     }
        //
        //     SetEnergy(EnergyNumber() - costNum);
        //     EnterGameIsUnlimitedState = false;
        //     // BiUtil.SendItemChangeEvent(BiEventMatchFrenzy.Types.Item.Energy, -costNum, (ulong)EnergyNumber(), args);
        //     return true;
        // }

        // // 设置体力值
        // public bool SetEnergy(int num)
        // {
        //     if (num < 0)
        //     {
        //         return false;
        //     }
        //
        //     bool oldEnergyFull = IsEnergyFull();
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     int changedNum = num - storage.Energy;
        //     storage.Energy = num;
        //     if (oldEnergyFull && !IsEnergyFull())
        //     {
        //         // 体力从满变为不满自动记录恢复体力时间点
        //         StorageAutoAddEnergyTime(CommonUtils.GetTimeStamp());
        //     }
        //
        //     // 添加触发事件
        //     EventDispatcher.Instance.DispatchEvent(new EnergyChangedEvent(IsEnergyUnlimited(), num, changedNum));
        //     return true;
        // }

        // // 是否自动到了添加体力的时间
        // bool IsTimeToAutoAddEnergy()
        // {
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     return storage.LastAddEnergyTime > 0L && LeftAutoAddEnergyTime() <= 0L;
        // }

        // // 获取自动添加体力的剩余时间 单位:毫秒
        // public long LeftAutoAddEnergyTime()
        // {
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     long leftTime = storage.LastAddEnergyTime +
        //                     TMatchShopConfigManager.Instance.GlobalList[0].EnergyRefillTime * 1000 -
        //                     CommonUtils.GetTimeStamp();
        //     return leftTime < 0L ? 0L : leftTime;
        // }

        // // 记录能量恢复的时间
        // void StorageAutoAddEnergyTime(long timeStamp)
        // {
        //     var storage = StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        //     storage.LastAddEnergyTime = timeStamp;
        // }
        //
        // public void OnGameResReady()
        // {
        //     _isReady = true;
        // }

        public void Update(float deltaTime)
        {
            return;
            // if (!_isReady)
            // {
            //     return;
            // }
            //
            // _updateTime += Time.deltaTime;
            // if (_updateTime < 1f)
            // {
            //     return;
            // }
            //
            // _updateTime = 0f;
            //
            // if (!IsEnergyFull() && IsTimeToAutoAddEnergy())
            // {
            //     // 此处自动恢复体力
            //     AutoAddEnergy();
            // }
            //
            // bool isUnlimited = IsEnergyUnlimited();
            // if (isUnlimited != IsEnergyUnlimitedState)
            // {
            //     IsEnergyUnlimitedState = isUnlimited;
            //     // 无限体力状态改变，触发事件
            //     EventDispatcher.Instance.DispatchEvent(new EnergyChangedEvent(isUnlimited, EnergyNumber(), 0));
            // }
        }

        public bool GetEnterGameIsUnlimitedState()
        {
            return EnterGameIsUnlimitedState;
        }
    }
}
