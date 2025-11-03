using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonU3DSDK.Storage;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;

public class EnergyModel : Manager<EnergyModel>
{
    bool IsConfigResReady = false;

    int UnlimitedDelayShowTime;

    // 无限体力状态
    bool IsEnergyUnlimitedState { get; set; }

    private int maxEnergyNum = -1;
    private int energyRefillTime = -1;
    public int MaxEnergyNum
    {
        get
        {
            if(maxEnergyNum <= 0)
                maxEnergyNum = GlobalConfigManager.Instance.GetNumValue("MaxUserEnergy");
            
            maxEnergyNum = maxEnergyNum == 0 ? 100 : maxEnergyNum;
            if (Activity.BattlePass.BattlePassModel.Instance.IsLocalPurchasing())
                maxEnergyNum =150;
            
            if (Activity.BattlePass_2.BattlePassModel.Instance.IsLocalPurchasing())
                maxEnergyNum =150;
            
            return maxEnergyNum;
        }
    }

    private StorageHome storageHome = null;

    private void InitStorageHome()
    {
        if (storageHome != null)
            return;

        storageHome = StorageManager.Instance.GetStorage<StorageHome>();
    }

    // 获取体力值
    public int EnergyNumber()
    {
        InitStorageHome();
        return storageHome.Energy;
    }

    public long MilisecondsBeforeEnergyFull()
    {
        if (IsEnergyFull())
        {
            return 0;
        }

        int gap = MaxEnergyNum - EnergyNumber();
        if (energyRefillTime <= 0)
        {
            energyRefillTime = GlobalConfigManager.Instance.GetNumValue("EnergyRefillTime");
        }
        return LeftAutoAddEnergyTime() +
               (gap - 1) * energyRefillTime * 1000;
    }

    // 体力是否满了
    public bool IsEnergyFull()
    {
        return EnergyNumber() >= MaxEnergyNum;
    }

    // 获取最大体力值
    public int MaxEnergy()
    {
        return MaxEnergyNum;
    }

    // 体力是否空了, 注意这个方法算了无限体力，也就是说在无限体力状态下， 体力肯定不是空的
    public bool IsEnergyEmpty()
    {
        return EnergyNumber() <= 0 && !IsEnergyUnlimited();
    }

    // 花费钻石， 补满体力
    public bool BuyEnergyWithDiamond()
    {
        if (IsEnergyFull())
        {
            return false;
        }

        int needDiamond = GlobalConfigManager.Instance.GetNumValue("EnergyGemPrice");
        if (UserData.Instance.GetRes(UserData.ResourceId.Coin) < needDiamond)
        {
            // 钻石不够
            return false;
        }

        UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, needDiamond, new GameBIManager.ItemChangeReasonArgs
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy
        });
        // AddEnergy(MaxEnergyNum - EnergyNumber(), new GameBIManager.ItemChangeReasonArgs {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BuyEnergy});
        var rewards = new List<ResData>();
        rewards.Add(new ResData((int) UserData.ResourceId.Energy, MaxEnergyNum - EnergyNumber()));
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            new GameBIManager.ItemChangeReasonArgs {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy});
        return true;
    }

    // 免费加满体力
    public void SetEnergyFull()
    {
        if (IsEnergyFull())
        {
            return;
        }

        SetEnergy(MaxEnergyNum);
    }

    // 时间到了自动添加体力
    void AutoAddEnergy()
    {
        InitStorageHome();
        long currentTime = (long)APIManager.Instance.GetServerTime();
        long intervalTime = currentTime - storageHome.LastAddEnergyTime;
        long addEnergyInterval = GlobalConfigManager.Instance.GetNumValue("EnergyRefillTime") * 1000;
        long n = intervalTime / addEnergyInterval;
        long leftTime = intervalTime - addEnergyInterval * n;
        if (n > 0)
        {
            long addNum = GlobalConfigManager.Instance.GetNumValue("EnergyRefillAmount") * n;
            if (addNum + EnergyNumber() >= MaxEnergy())
            {
                addNum = MaxEnergy() - EnergyNumber();
                StorageAutoAddEnergyTime(currentTime);
            }
            else
            {
                StorageAutoAddEnergyTime(currentTime - leftTime);
            }

            AddEnergy((int) addNum,
                new GameBIManager.ItemChangeReasonArgs {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TimeEnergy});
        }
    }

    // 无限体力状态
    public bool IsEnergyUnlimited()
    {
        return EnergyUnlimitedLeftTime() > 0;
    }

    // 添加无限体力时间
    public void AddEnergyUnlimitedTime(int timeMillSecond, GameBIManager.ItemChangeReasonArgs r, string data1 = "",
        string data2 = "", string data3 = "", string data4 = "", bool ignoreBi = false)
    {
        InitStorageHome();

        storageHome.UnlimitEnergyEndUTCTimeInSeconds =
            (long) ((EnergyUnlimitedLeftTime() + timeMillSecond +(long)APIManager.Instance.GetServerTime()) * 0.001);
        if (ignoreBi == false)
        {
            GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Infinity_Energy, (uint) timeMillSecond,
                (uint) EnergyUnlimitedLeftTime(), r);
        }
    }

    // 直接设置无限体力时间而不是添加时长
    public void SetEnergyUnlimitedTimeDirectly(int timeMillSecond)
    {
        if (timeMillSecond < 0)
        {
            return;
        }

        InitStorageHome();
        storageHome.UnlimitEnergyEndUTCTimeInSeconds = (long) ((timeMillSecond +(long)APIManager.Instance.GetServerTime()) * 0.001);
    }

    // 设置无限体力延迟显示时间
    public void SetEnergyUnlimitedTimeDelay(int timeMillSecond)
    {
        UnlimitedDelayShowTime = timeMillSecond;
    }

    // 获取无限体力剩余显示时间
    public int EnergyUnlimitedLeftShowTime()
    {
        InitStorageHome();
        long left = storageHome.UnlimitEnergyEndUTCTimeInSeconds * 1000 -(long)APIManager.Instance.GetServerTime() -
                    UnlimitedDelayShowTime;
        return left > 0L ? (int) left : 0;
    }

    // 获取无限体力剩余时间
    public int EnergyUnlimitedLeftTime()
    {
        InitStorageHome();
        long left = storageHome.UnlimitEnergyEndUTCTimeInSeconds * 1000 -(long)APIManager.Instance.GetServerTime();
        return left > 0L ? (int) left : 0;
    }

    // 添加体力值
    public bool AddEnergy(int addNum, GameBIManager.ItemChangeReasonArgs r, bool ignoreBi = false, bool isEvent = true)
    {
        if (addNum <= 0)
        {
            return false;
        }

        // if (EnergyNumber() + addNum > MaxEnergy())
        // {
        //     addNum = MaxEnergy() - EnergyNumber();
        // }

        SetEnergy(EnergyNumber() + addNum, isEvent);
        if (ignoreBi == false)
        {
            GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Energy, (long) addNum,
                (ulong) EnergyNumber(), r);
        }

        EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.GetRes, (int)UserData.ResourceId.Energy, addNum);
        EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.GetRes, (int)UserData.ResourceId.Energy, addNum);
        return true;
    }

    // 花费体力值
    public bool CostEnergy(int costNum, GameBIManager.ItemChangeReasonArgs r)
    {
        if (costNum <= 0)
        {
            return false;
        }

        if (IsEnergyUnlimited())
        {
      
            return true;
        }

        if (costNum > EnergyNumber())
        {
            return false;
        }
        EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.Consume, (int)UserData.ResourceId.Energy, costNum);
        EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.Consume, (int)UserData.ResourceId.Energy, costNum);
        SetEnergy(EnergyNumber() - costNum);
        GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Energy, (long) -costNum, (ulong) EnergyNumber(),
            r);
        return true;
    }

    // 设置体力值
    public bool SetEnergy(int num, bool isEvent = true)
    {
        // num = Mathf.Min(num, MaxEnergy());

        if (num < 0)
        {
            return false;
        }

        bool oldEnergyFull = IsEnergyFull();
        InitStorageHome();
        int changedNum = num - storageHome.Energy;
        storageHome.Energy = num;
        if (oldEnergyFull && !IsEnergyFull())
        {
            // 体力从满变为不满自动记录恢复体力时间点
            StorageAutoAddEnergyTime((long)APIManager.Instance.GetServerTime());
        }

        // 添加触发事件
        if (isEvent)
            EventDispatcher.Instance.DispatchEvent(EventEnum.EnergyChanged, IsEnergyUnlimited(), num, changedNum);
        return true;
    }

    // 是否自动到了添加体力的时间
    bool IsTimeToAutoAddEnergy()
    {
        InitStorageHome();
        if (storageHome.LastAddEnergyTime <= 0)
            StorageAutoAddEnergyTime((long)APIManager.Instance.GetServerTime());
        return storageHome.LastAddEnergyTime > 0L && LeftAutoAddEnergyTime() <= 0L;
    }

    // 获取自动添加体力的剩余时间 单位:毫秒
    public long LeftAutoAddEnergyTime()
    {
        InitStorageHome();
        long leftTime = storageHome.LastAddEnergyTime +
            GlobalConfigManager.Instance.GetNumValue("EnergyRefillTime") * 1000 -(long)APIManager.Instance.GetServerTime();
        return leftTime < 0L ? 0L : leftTime;
    }

    // 记录能量恢复的时间
    void StorageAutoAddEnergyTime(long timeStamp)
    {
        InitStorageHome();
        storageHome.LastAddEnergyTime = timeStamp;
    }

    public void OnGameResReady()
    {
        IsConfigResReady = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GerRvEnergy()
    {
        List<ResData> resDatas = AdConfigHandle.Instance.GetBonus(ADConstDefine.RV_GET_ENERGY);
        if (resDatas == null || resDatas.Count <= 0)
        {
            return 0;
        }

        ResData resData = resDatas[0];
        if (resData.id != (int) UserData.ResourceId.Energy)
        {
            return 0;
        }

        return resData.count;
    }

    void Update()
    {
        if (maxEnergyNum <= 0)
            return;
        if (!IsConfigResReady)
        {
            return;
        }

        if (!IsEnergyFull() && IsTimeToAutoAddEnergy())
        {
            // 此处自动恢复体力
            AutoAddEnergy();
        }

        bool isUnlimited = IsEnergyUnlimited();
        if (isUnlimited != IsEnergyUnlimitedState)
        {
            IsEnergyUnlimitedState = isUnlimited;
            // 无限体力状态改变，触发事件
            EventDispatcher.Instance.DispatchEvent(EventEnum.EnergyChanged, isUnlimited, EnergyNumber(), 0);
        }
    }
    
    private static int _getEnergyNumber;
    public static int GetEnergyNumber()
    {
        if (IsDestroy)
            return _getEnergyNumber;
        else
            return Instance.EnergyNumber();
    }
    private static int _getCoinNumber;
    public static int GetCoinNumber()
    {
        if (IsDestroy)
            return _getCoinNumber;
        else
            return UserData.Instance.GetRes(UserData.ResourceId.Coin);
    }
    public static bool IsDestroy;
    private void OnDestroy()
    {
        IsDestroy = true;
        _getEnergyNumber = EnergyNumber();
        _getCoinNumber = UserData.Instance.GetRes(UserData.ResourceId.Coin);
    }
}