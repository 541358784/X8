using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Util;
using UnityEngine;

public partial class ActivityManager : ActivityResHotUpdate
{
    Dictionary<string, StorageActivityCache> _activityCache
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().ActivityCache;
        }
    }

    private void AddActivityCache(bool isCache, string activityType, string activityId, string configJson, long startTime, long endTime)
    {
        if(!isCache)
            return;
        
        if (!_activityCache.ContainsKey(activityType))
            _activityCache.Add(activityType, new StorageActivityCache());
        
        StorageActivityCache cache = _activityCache[activityType];

        cache.ActivityId = activityId;
        cache.StartTime = startTime;
        cache.EndTime = endTime;
        cache.ConfigJson = EncryptJson(configJson);
        
        cache.ResMd5.Clear();
        cache.ResPath.Clear();
        
        var resMd5List = GetActivityMd5List(activityId);
        foreach (var resMd5 in resMd5List)
        {
            cache.ResMd5.Add(resMd5);
            var resPath = GetFilePath(resMd5);
            cache.ResPath.Add(resPath);
        }
    }

    public string DecryptJson(StorageActivityCache cache)
    {
        var encryptData = Convert.FromBase64String(cache.ConfigJson);
        return RijndaelManager.Instance.DecryptStringFromBytes(encryptData);
    }
    
    public string EncryptJson(string json)
    {
        var encryptData = RijndaelManager.Instance.EncryptStringToBytes(json);
        return Convert.ToBase64String(encryptData);
    }

    public bool IsActivityCacheTimeEnd(string activityType, string activityId)
    {
        var cache = GetActivityCache(activityType, activityId);
        if (cache == null)
            return false;

        return (long)APIManager.Instance.GetServerTime() > cache.EndTime;
    }

    public bool IsActivityCacheResReady(string activityType, string activityId)
    {
        var cache = GetActivityCache(activityType, activityId);
        if (cache == null)
            return false;
        
        return CheckResExist(cache.ResPath);
    }

    public StorageActivityCache GetActivityCache(string activityType, string activityId)
    {
        if (!_activityCache.ContainsKey(activityType))
            return null;

        if (_activityCache[activityType].ActivityId != activityType)
            return null;

        return _activityCache[activityType];
    }
}