using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonU3DSDK.Storage;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using DragonPlus.ConfigHub.Ad;

public class HappyGoEnergyModel : Manager<HappyGoEnergyModel>
{
    // bool IsConfigResReady = false;

    int UnlimitedDelayShowTime;

    // 无限体力状态
    bool IsEnergyUnlimitedState { get; set; }

    private int maxEnergyNum = -1;

    public int MaxEnergyNum
    {
        get
        {
            maxEnergyNum = GlobalConfigManager.Instance.GetNumValue("MaxUserEnergy");
            maxEnergyNum = maxEnergyNum == 0 ? 100 : maxEnergyNum;
            return maxEnergyNum;
        }
    }

    private StorageGame storageGame= null;

    private void InitStorageHome()
    {
        if (storageGame != null)
            return;

        storageGame = StorageManager.Instance.GetStorage<StorageGame>();
    }

    // 获取体力值
    public int EnergyNumber()
    {
        InitStorageHome();
        return storageGame.HappyGo.HgEnergy;
    }

    public long MilisecondsBeforeEnergyFull()
    {
        if (IsEnergyFull())
        {
            return 0;
        }

        int gap = MaxEnergyNum - EnergyNumber();

        return LeftAutoAddEnergyTime() +
               (gap - 1) * GlobalConfigManager.Instance.GetNumValue("EnergyRefillTime") * 1000;
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
        long currentTime = CommonUtils.GetTimeStamp();
        long intervalTime = currentTime - storageGame.HappyGo.HgLastAddEnergyTime;
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

        storageGame.HappyGo.UnlimitEnergyEndUTCTimeInSeconds =
            (long) ((EnergyUnlimitedLeftTime() + timeMillSecond + CommonUtils.GetTimeStamp()) * 0.001);
        if (ignoreBi == false)
        {
            GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Energy, (uint) timeMillSecond,
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
        storageGame.HappyGo.UnlimitEnergyEndUTCTimeInSeconds = (long) ((timeMillSecond + CommonUtils.GetTimeStamp()) * 0.001);
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
        long left =  storageGame.HappyGo.UnlimitEnergyEndUTCTimeInSeconds * 1000 - CommonUtils.GetTimeStamp() -
                     UnlimitedDelayShowTime;
        return left > 0L ? (int) left : 0;
    }

    // 获取无限体力剩余时间
    public int EnergyUnlimitedLeftTime()
    {
        InitStorageHome();
        long left =  storageGame.HappyGo.UnlimitEnergyEndUTCTimeInSeconds * 1000 - CommonUtils.GetTimeStamp();
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
        SetEnergy(EnergyNumber() - costNum);
        GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.HappyGo_Energy, (long) -costNum, (ulong) EnergyNumber(), r);
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
        int changedNum = num - storageGame.HappyGo.HgEnergy;
        storageGame.HappyGo.HgEnergy = num;
        if (oldEnergyFull && !IsEnergyFull())
        {
            // 体力从满变为不满自动记录恢复体力时间点
            StorageAutoAddEnergyTime(CommonUtils.GetTimeStamp());
        }

        // 添加触发事件
        if (isEvent)
            EventDispatcher.Instance.DispatchEvent(EventEnum.HGEnergyChanged, IsEnergyUnlimited(), num, changedNum);
        return true;
    }

    // 是否自动到了添加体力的时间
    bool IsTimeToAutoAddEnergy()
    {
        InitStorageHome();
        if (storageGame.HappyGo.HgLastAddEnergyTime <= 0)
            StorageAutoAddEnergyTime(CommonUtils.GetTimeStamp());
        return storageGame.HappyGo.HgLastAddEnergyTime > 0L && LeftAutoAddEnergyTime() <= 0L;
    }

    // 获取自动添加体力的剩余时间 单位:毫秒
    public long LeftAutoAddEnergyTime()
    {
        InitStorageHome();
        long leftTime = storageGame.HappyGo.HgLastAddEnergyTime +
            GlobalConfigManager.Instance.GetNumValue("EnergyRefillTime") * 1000 - CommonUtils.GetTimeStamp();
        return leftTime < 0L ? 0L : leftTime;
    }

    // 记录能量恢复的时间
    void StorageAutoAddEnergyTime(long timeStamp)
    {
        InitStorageHome();
        storageGame.HappyGo.HgLastAddEnergyTime = timeStamp;
    }

    public void OnGameResReady()
    {
        // IsConfigResReady = true;
    }
    
    void Update()
    {
        if (maxEnergyNum <= 0)
            return;
        //if (!IsConfigResReady)
        //{
        //    return;
        //}

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
            EventDispatcher.Instance.DispatchEvent(EventEnum.HGEnergyChanged, isUnlimited, EnergyNumber(), 0);
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