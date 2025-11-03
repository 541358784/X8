using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;

public class CoolingTimeManager : Singleton<CoolingTimeManager>
{
    public enum CDType
    {
        OtherDay,
        LossTime,
    }

    private StorageCoolTime storageCoolTime = null;

    public void Init()
    {
        storageCoolTime = StorageManager.Instance.GetStorage<StorageHome>().CoolTimeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key"></param>
    /// <param name="time">单位毫秒</param>
    /// <param name="interval">单位毫秒</param>
    public void UpdateCoolTime(CDType type, string key, long time, long interval = 0)
    {
        StorageDictionary<string, long> dictionary = GetDictionary(type);

        if (dictionary == null)
            return;

        if (dictionary.ContainsKey(key))
            dictionary[key] = time;
        else
            dictionary.Add(key, time);

        if (type == CDType.LossTime)
        {
            if (storageCoolTime.IntervalTime.ContainsKey(key))
                storageCoolTime.IntervalTime[key] = interval;
            else
                storageCoolTime.IntervalTime.Add(key, interval);
        }
    }

    /// <summary>
    /// 是否CD中
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsCooling(CDType type, string key)
    {
        StorageDictionary<string, long> dictionary = GetDictionary(type);
        if (dictionary == null)
            return false;

        if (!dictionary.ContainsKey(key))
            return false;

        long oldTime = dictionary[key];
        switch (type)
        {
            case CDType.OtherDay:
                return CommonUtils.IsSameDayWithToday((ulong) oldTime);
            case CDType.LossTime:
                return CommonUtils.GetTimeStamp() - oldTime < GetIntervalTime(key);
        }

        return false;
    }

    /// <summary>
    /// 移除cd中的key
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key"></param>
    public void RemoveCooling(CDType type, string key)
    {
        StorageDictionary<string, long> dictionary = GetDictionary(type);
        if (dictionary == null)
            return;

        if (!dictionary.ContainsKey(key))
            return;

        dictionary.Remove(key);
    }

    public long GetIntervalTime(string key)
    {
        if (storageCoolTime.IntervalTime.ContainsKey(key))
            return storageCoolTime.IntervalTime[key];

        return 0;
    }

    public long GetCDTime(CDType type, string key)
    {
        StorageDictionary<string, long> dictionary = GetDictionary(type);
        if (dictionary == null)
            return 0;
        
        if (!dictionary.ContainsKey(key))
            return 0;

        return dictionary[key];
    }
    
    private StorageDictionary<string, long> GetDictionary(CDType type)
    {
        switch (type)
        {
            case CDType.OtherDay:
                return storageCoolTime.OtherDay;
            case CDType.LossTime:
                return storageCoolTime.LossTime;
        }

        return null;
    }
}