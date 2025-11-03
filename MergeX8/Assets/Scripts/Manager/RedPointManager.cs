using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;

public enum RedPointType
{
    Theme,
    Room,
}

public enum RedPointSubType
{
    Theme_Bg,
    Theme_Face,
    Theme_Back,
    Theme_Anim,
}

public class RedPointManager : Singleton<RedPointManager>
{
    private StorageHome storageHome = null;

    public void Init()
    {
        storageHome = StorageManager.Instance.GetStorage<StorageHome>();
    }

    public void UpdateRedPoint(RedPointType type, RedPointSubType subType, bool isRed)
    {
        StorageDictionary<int, bool> redPointData = GetRedPointData(type);
        if (redPointData == null)
        {
            StorageRedPoint storageRedPoint = new StorageRedPoint();
            storageHome.RedPoint.Add((int) type, storageRedPoint);

            redPointData = storageRedPoint.RedPointData;
        }

        if (redPointData.ContainsKey((int) subType))
            redPointData[(int) subType] = isRed;
        else
            redPointData.Add((int) subType, isRed);
    }

    public bool IsRedPoint(RedPointType type)
    {
        StorageDictionary<int, bool> redPointData = GetRedPointData(type);
        if (redPointData == null)
            return false;

        foreach (var kv in redPointData)
        {
            if (kv.Value)
                return true;
        }

        return false;
    }

    public bool IsRedPoint(RedPointType type, RedPointSubType subType)
    {
        StorageDictionary<int, bool> redPointData = GetRedPointData(type);
        if (redPointData == null)
            return false;

        if (!redPointData.ContainsKey((int) subType))
            return false;

        return redPointData[(int) subType];
    }

    private StorageDictionary<int, bool> GetRedPointData(RedPointType type)
    {
        if (storageHome.RedPoint.ContainsKey((int) type))
        {
            return storageHome.RedPoint[(int) type].RedPointData;
        }

        return null;
    }

    public void UpdateChildRedPoint(RedPointType type, RedPointSubType subType, bool isRed, int id = -1)
    {
        StorageDictionary<string, bool> redPointData = GetRedPointChildData(type);
        if (redPointData == null)
        {
            StorageRedPoint storageRedPoint = new StorageRedPoint();
            storageHome.RedPoint.Add((int) type, storageRedPoint);

            redPointData = storageRedPoint.RedPointChildData;
        }

        if (id > 0)
        {
            string key = subType.ToString() + id;
            if (redPointData.ContainsKey(key))
                redPointData[key] = isRed;
            else
                redPointData.Add(key, isRed);
        }
        else
        {
            List<string> keys = new List<string>(redPointData.Keys);

            foreach (var key in keys)
            {
                redPointData[key] = isRed;
            }
        }
    }

    public bool IsRedPoint(RedPointType type, RedPointSubType subType, int id)
    {
        StorageDictionary<string, bool> redPointData = GetRedPointChildData(type);
        if (redPointData == null)
            return false;

        string key = subType.ToString() + id;

        if (!redPointData.ContainsKey(key))
            return false;

        return redPointData[key];
    }

    private StorageDictionary<string, bool> GetRedPointChildData(RedPointType type)
    {
        if (storageHome.RedPoint.ContainsKey((int) type))
        {
            return storageHome.RedPoint[(int) type].RedPointChildData;
        }

        return null;
    }
}