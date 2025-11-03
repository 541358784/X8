using System;
using System.Collections.Generic;
using System.Linq;
using ABTest;
using DragonPlus;
using DragonPlus.Config.AdLocal;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

public class PayLevelModel : Manager<PayLevelModel>
{
    const int OffsetHour = 0;

    private int CurDayId =>
        (int)((APIManager.Instance.GetServerTime() - OffsetHour * XUtility.Hour) / XUtility.DayTime);

    private Dictionary<int, PayLevelConfig> _payLevelConfig;
    public Dictionary<int, PayLevelConfig> PayLevelConfig
    {
        get
        {
            if (_payLevelConfig == null)
            {
                _payLevelConfig = new Dictionary<int, PayLevelConfig>();
                var configs = AdLocalConfigManager.Instance.PayLevelConfigList;
                foreach (var config in configs)
                {
                    _payLevelConfig.Add(config.Id,config);
                }
            }
            return _payLevelConfig;
        }
    }
    // public TablePayLevelGlobalConfig GlobalConfig => GlobalConfigManager.Instance.TablePayLevelGlobalConfig[0];

    public List<TablePayLevelStartLevelConfig> StartLevelConfig =>
        GlobalConfigManager.Instance.TablePayLevelStartLevelConfig;

    public List<TablePayLevelGlobalConfig> GlobalConfig => 
        GlobalConfigManager.Instance.TablePayLevelGlobalConfig;
    public Dictionary<int, TablePayLevelIgnoreConfig> IgnoreConfig => 
        GlobalConfigManager.Instance.TablePayLevelIgnoreConfig;
    public StoragePayLevel Storage => StorageManager.Instance.GetStorage<StorageHome>().PayLevel;

    public int PlayLevel => Storage.PayLevel;

    public void OnPaySuccess(TableShop shopCfg)
    {
        UpdateState();
        var value = shopCfg.price;
        if (value > 0 && !IgnoreConfig.ContainsKey(shopCfg.id))
        {
            Storage.HasPay = true;
            Storage.CurDayPayValue += value;
            Storage.DayPayDic.TryAdd(CurDayId, 0);
            Storage.DayPayDic[CurDayId] += value;
        }
        if (value > 0)
        {
            Storage.AllDayPayDic.TryAdd(CurDayId, 0);
            Storage.AllDayPayDic[CurDayId] += value;
        }
    }

    public int SaveCount = 30;
    public void ClearDayPayStorage()
    {
        var keys = Storage.DayPayDic.Keys.ToList();
        for (var i = 0; i < keys.Count - SaveCount; i++)
        {
            Storage.DayPayDic.Remove(keys[i]);
        }
    }
    public void ClearAllDayPayStorage()
    {
        var keys = Storage.AllDayPayDic.Keys.ToList();
        for (var i = 0; i < keys.Count - SaveCount; i++)
        {
            Storage.AllDayPayDic.Remove(keys[i]);
        }
    }

    public int DebugPayLevel=1;
    public bool DebugPayLevelOpen = false;
    public PayLevelConfig GetCurPayLevelConfig()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (DebugPayLevelOpen)
        {
            if (PayLevelConfig.ContainsKey(DebugPayLevel))
            {
                return PayLevelConfig[DebugPayLevel];
            }
            else
            {
                Debug.LogError("没有指定PayLevel分层"+DebugPayLevel);
            }
        }
#endif
        if (ABTestManager.Instance.IsAdLocalConfigPayLevelTest())
        {
            Common common = AdConfigHandle.Instance.GetCommonByUserTypeId();
            return PayLevelConfig[common.PayLevelPack];
        }
        else
        {
            UpdateState();
            return PayLevelConfig[Storage.PayLevel];
        }
    }

    public int GetPayLevel()
    {
        var config = GetCurPayLevelConfig();
        if (config == null)
            return 0;

        return config.Id;
    }

    public void LoadPayDataToAdLocal()
    {
        AdLocalConfigHandle.Instance.Storage.LastPayData.Clear();
        var curDay = CurDayId;
        for (var i = curDay - SaveCount; i < curDay; i++)
        {
            if (Storage.AllDayPayDic.TryGetValue(i, out var value))
            {
                AdLocalConfigHandle.Instance.Storage.LastPayData.Add(value);  
                // Debug.LogError("老付费分层付费记录导入新付费分层 日期:"+i+" 付费金额"+value);
            }
            else
            {
                AdLocalConfigHandle.Instance.Storage.LastPayData.Add(-1);
                // Debug.LogError("老付费分层付费记录导入新付费分层 日期:"+i+" 付费金额0");
            }
        }
    }
    
    public void UpdateState()
    {
        // {
        //     var key = "2025_4_23_SetPayValueToNewUserGroup";
        //     if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(key))
        //     {
        //         StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(key, "true");
        //         LoadPayDataToAdLocal();
        //     }
        // }
        if (IsOpenAB() && Storage.NewPayLevelStartDay == 0) //新分层初始日期
        {
            Storage.NewPayLevelStartDay = CurDayId;
        }

        if (Storage.DayId == 0)
        {
            var startLevel = 0;
            // if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyPack) || 
            //     UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewDailyPack))
            // {
            //     if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewDailyPack))
            //     {
            //         var newDailyLevel = 0;
            //         var newDailyGroupId = NewDailyPackModel.Instance.GetCurPackageLevel();
            //         for (var i = 0; i < GlobalConfig.NewDailyPackGroup.Length; i++)
            //         {
            //             if (GlobalConfig.NewDailyPackGroup[i] == newDailyGroupId)
            //             {
            //                 newDailyLevel = GlobalConfig.NewDailyPackPayLevel[i];
            //                 break;
            //             }
            //         }
            //         if (newDailyLevel > startLevel)
            //             startLevel = newDailyLevel;
            //     }
            //     if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyPack))
            //     {
            //         var dailyLevel = 0;
            //         var dailyGroupId = DailyPackModel.Instance.GetCurPackageLevel();
            //         for (var i = 0; i < GlobalConfig.DailyPackGroup.Length; i++)
            //         {
            //             if (GlobalConfig.DailyPackGroup[i] == dailyGroupId)
            //             {
            //                 dailyLevel = GlobalConfig.DailyPackPayLevel[i];
            //                 break;
            //             }
            //         }
            //         if (dailyLevel > startLevel)
            //             startLevel = dailyLevel;
            //     }
            // }
            // else
            // {
            //     startLevel = GlobalConfig.StartLevel;
            // }
            if (AdConfigManager.Instance.IsRemote)
            {
                // var userGroup = UserGroupManager.Instance.UserGroup;
                // var initConfig = StartLevelConfig.Find(a => a.UserGroups.Contains(userGroup));
                // if (initConfig == null)
                // {
                //     initConfig = StartLevelConfig.First();
                // }
                var globalId = AdConfigHandle.Instance.GetCommon().FirstDayPack;
                var initConfig = GlobalConfig.Find(a => a.id == globalId);

                startLevel = initConfig.StartLevel;
                Storage.MinLevel = initConfig.MinLevelAfterPay;
                Debug.LogError("付费等级服务器分层初始化 Level:" + startLevel);
            }
            else
            {
                var initConfig = StartLevelConfig.First();
                startLevel = initConfig.StartLevel;
                Storage.MinLevel = initConfig.MinLevelAfterPay;
                EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent>();
                Action<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent> callback = null;
                EventFunctor<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent> handler = null;
                var storage = Storage;
                callback = (a) =>
                {
                    if (storage != Storage)
                        return;
                    // var userGroup = UserGroupManager.Instance.UserGroup;
                    // var initConfig2 = StartLevelConfig.Find(b => b.UserGroups.Contains(userGroup));
                    // if (initConfig2 == null)
                    // {
                    //     initConfig2 = StartLevelConfig.First();
                    // }
                    var globalId = AdConfigHandle.Instance.GetCommon().FirstDayPack;
                    var initConfig2 = GlobalConfig.Find(a => a.id == globalId);

                    var startLevel2 = initConfig2.StartLevel;
                    Storage.MinLevel = initConfig2.MinLevelAfterPay;
                    Debug.LogError("付费等级延迟初始化 Level:" + startLevel2);
                    Storage.DayId = CurDayId;
                    ChangePayLevel(startLevel2);
                    EventManager.Instance.Unsubscribe(handler);
                };
                handler = new EventFunctor<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent>(callback);
                EventManager.Instance.Subscribe(handler);
                Debug.LogError("付费等级本地临时默认初始化 Level:" + startLevel);
            }

            Storage.DayId = CurDayId;
            ChangePayLevel(startLevel);
            return;
        }
        else
        {
            //老用户掉档补救措施
            if (Storage.MinLevel == 0)
                Storage.MinLevel = 4;
            var key = "2025_2_13_HotFix";
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(key))
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(key, "true");
                if (Storage.PayLevel == 3 && Storage.HasPay)
                {
                    Debug.LogError("热更付费等级变更为4,最低等级变更为4");
                    Storage.MinLevel = 4;
                    Storage.DayId = CurDayId;
                    ChangePayLevel(4);
                }
                else
                {
                    Debug.LogError("未触发热更,条件不满足");
                }
            }
        }

        var curDayId = CurDayId;
        if (Storage.DayId > curDayId)
            Storage.DayId = curDayId;
        if (Storage.DayId == curDayId)
        {
            return;
        }
        Storage.DownLevelEnableDay++;
        Storage.DayPayList.Insert(0,Storage.CurDayPayValue);
        

        var oldLevelConfig = PayLevelConfig[Storage.PayLevel];
        var maxUpCount = 0;
        var upCount = 0;
        var threePayValue = 0f;
        var sevenPayValue = -1f;
        if (IsOpenAB())
        {
            Debug.LogError("新付费分层组");
            maxUpCount = oldLevelConfig.MaxUpCount;
            upCount = 0;
            threePayValue = GetLastDaysTotalPayValue(curDayId, 3);
            sevenPayValue = GetLastLoginDaysPayValue(oldLevelConfig.DownDayNum);
        }

        ClearDayPayStorage();
        ClearAllDayPayStorage();
        UpdateAvgPayValue();

        while (Storage.DayId != curDayId)
        {
            Debug.LogError("付费等级日期更新");
            var passDay = curDayId - Storage.DayId;
            var lastDayPayValue = Storage.CurDayPayValue;
            var payConfig = PayLevelConfig[Storage.PayLevel];
            if (passDay > 1 && lastDayPayValue == 0 &&
                (payConfig.DownGradeContinueUnPayDays < 0 ||
                 (Storage.HasPay && Storage.PayLevel <= Storage.MinLevel)))
            {
                Debug.LogError("付费等级触发可合并情况 passDay:" + passDay);
                Storage.ContinueUnPayDays ++;
                Storage.ContinuePayDays = 0;
                Storage.CurDayPayValue = 0;
                Storage.DayId = curDayId;
                continue;
            }

            // Storage.DayId++;
            Storage.DayId = curDayId;
            Storage.CurDayPayValue = 0;
            if (lastDayPayValue > 0)
            {
                Storage.ContinuePayDays++;
                Storage.ContinueUnPayDays = 0;
            }
            else
            {
                Storage.ContinueUnPayDays++;
                Storage.ContinuePayDays = 0;
            }

            //单日付费达标升档
            if (payConfig.UpGradeSingleDayPayValue > 0 &&
                lastDayPayValue >= payConfig.UpGradeSingleDayPayValue)
            {
                if (IsOpenAB())
                {
                    if (upCount < maxUpCount)
                    {
                        upCount++;
                        ChangePayLevel(Storage.PayLevel + 1);
                        continue;   
                    }
                }
                else
                {
                    ChangePayLevel(Storage.PayLevel + 1);
                    continue;   
                }
            }

            //连续付费天数达标
            if (payConfig.UpGradeContinuePayDays > 0 &&
                Storage.ContinuePayDays >= payConfig.UpGradeContinuePayDays)
            {
                if (IsOpenAB())
                {
                    if (upCount < maxUpCount)
                    {
                        upCount++;
                        ChangePayLevel(Storage.PayLevel + 1);
                        continue;   
                    }
                }
                else
                {
                    ChangePayLevel(Storage.PayLevel + 1);
                    continue;   
                }
            }

            //降级
            if ((!Storage.HasPay || Storage.PayLevel > Storage.MinLevel) &&
                payConfig.DownGradeContinueUnPayDays > 0 &&
                Storage.ContinueUnPayDays >= payConfig.DownGradeContinueUnPayDays)
            {
                if (IsOpenAB())
                {
                    if (Storage.DownLevelEnableDay > payConfig.DownKeepDay)
                    {
                        upCount--;
                        ChangePayLevel(Storage.PayLevel - 1);
                        continue;   
                    }
                }
                else
                {
                    ChangePayLevel(Storage.PayLevel - 1);
                    continue;   
                }
            }

            if (IsOpenAB())//新分层逻辑
            {
                if (curDayId - Storage.NewPayLevelStartDay >= 3) //新升级判断
                {
                    var isUp = false;
                    while (payConfig.ThreeDayUpValue > 0 && 
                        threePayValue >= payConfig.ThreeDayUpValue && 
                        upCount < maxUpCount)
                    {
                        isUp = true;
                        upCount++;
                        ChangePayLevel(Storage.PayLevel + 1);
                        payConfig = PayLevelConfig[Storage.PayLevel];
                        Debug.LogError("新付费分层升级,当前等级为"+Storage.PayLevel);
                    }
                    if (isUp)
                        continue;
                }

                if (curDayId - Storage.NewPayLevelStartDay >= payConfig.DownDayNum)
                {
                    if (payConfig.SevenDayDownValue > 0 &&
                        sevenPayValue >= 0 &&
                           sevenPayValue < payConfig.SevenDayDownValue &&
                           (!Storage.HasPay || Storage.PayLevel > Storage.MinLevel))
                    {
                        if (Storage.DownLevelEnableDay > payConfig.DownKeepDay)
                        {
                            upCount--;
                            ChangePayLevel(Storage.PayLevel - 1);
                            Debug.LogError("新付费分层降级,当前等级为"+Storage.PayLevel);
                            continue;
                        }
                    }
                }
            }
        }
    }

    public void ChangePayLevel(int newPayLevel)
    {
        if (Storage.PayLevel == newPayLevel)
            return;
        Debug.LogError("付费等级变动 Level:" + newPayLevel);
        if (Storage.PayLevel < newPayLevel)
        {
            Storage.DayPayList.Clear();
        }
        Storage.DownLevelEnableDay = 0;
        Storage.PayLevel = newPayLevel;
        Storage.CurDayPayValue = 0;
        Storage.ContinuePayDays = 0;
        Storage.ContinueUnPayDays = 0;

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventUserPayLevel,
            newPayLevel.ToString());
    }

    public bool IsOpenAB()
    {
        return false;
        // return ABTestManager.Instance.IsOpenNewPayLevel();
    }

    public float GetLastDaysTotalPayValue(int curDayId, int dayCount)
    {
        var totalPayValue = 0f;
        for (var i = 1; i <= dayCount; i++)
        {
            var dayId = (curDayId - i);
            if (Storage.DayPayDic.TryGetValue(dayId, out var value))
            {
                totalPayValue += value;
            }
        }

        return totalPayValue;
    }

    public float GetLastLoginDaysPayValue(int dayCount)
    {
        if (Storage.DayPayList.Count < dayCount)
        {
            return -1;
        }

        var value = 0f;
        for (var i = 0; i < dayCount; i++)
        {
            value += Storage.DayPayList[i];
        }
        return value;
    }
    
    
    public int GetLastDaysAllTotalPayDays(int curDayId, int dayCount)
    {
        var totalPayDays = 0;
        for (var i = 1; i <= dayCount; i++)
        {
            var dayId = (curDayId - i);
            if (Storage.AllDayPayDic.TryGetValue(dayId, out var value) && value > 0)
            {
                totalPayDays++;
            }
        }

        return totalPayDays;
    }
    public float GetLastDaysAllTotalPayValue(int curDayId, int dayCount)
    {
        var totalPayValue = 0f;
        for (var i = 1; i <= dayCount; i++)
        {
            var dayId = (curDayId - i);
            if (Storage.AllDayPayDic.TryGetValue(dayId, out var value))
            {
                totalPayValue += value;
            }
        }

        return totalPayValue;
    }

    public void UpdateAvgPayValue()
    {
        {
            var dayCount2 = GetLastDaysAllTotalPayDays(CurDayId, 2);
            var payValue2 = GetLastDaysAllTotalPayValue(CurDayId, 2);
            if (dayCount2 == 0)
            {
                Storage.AvgValue2 = 0;
            }
            else
            {
                Storage.AvgValue2 = payValue2 / dayCount2;
            }   
        }
        {
            var dayCount7 = GetLastDaysAllTotalPayDays(CurDayId, 7);
            var payValue7 = GetLastDaysAllTotalPayValue(CurDayId, 7);
            if (dayCount7 == 0)
            {
                Storage.AvgValue7 = 0;
            }
            else
            {
                Storage.AvgValue7 = payValue7 / dayCount7;
            }
        }
        {
            var dayCount30 = GetLastDaysAllTotalPayDays(CurDayId, 30);
            var payValue30 = GetLastDaysAllTotalPayValue(CurDayId, 30);
            if (dayCount30 == 0)
            {
                Storage.AvgValue30 = 0;
            }
            else
            {
                Storage.AvgValue30 = payValue30 / dayCount30;
            }   
        }
    }
}