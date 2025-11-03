using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;

public class StoreSaleConfig
{
    // 对应商店的ID
    public int ShopItemId;

    // 活动数据1
    public int StoreSaleData1;

    // 活动数据2
    public int StoreSaleData2;

    // 活动数据
    public int StoreSaleData3;

    public string Config;

    // 过滤条件
    public string OpenType;

    // 过滤条件参数
    public string OpenTypeParam;
    // 数据信息
    // StorageStoreSaleItem StorageData;


    public string StoreSaleInsId { get; set; }
    public ulong StartTime { get; set; }
    public ulong EndTime { get; set; }
    public bool ManualEnd { get; set; }

    // 活动类型
    public StoreSaleGemType _storeSaleType;

    public StoreSaleGemType StoreSaleType
    {
        get => GetStoreSaleType(false);
        set => _storeSaleType = value;
    }

    public StoreSaleGemType GetStoreSaleType(bool ignoreEndTime = true)
    {
        if (_storeSaleType == StoreSaleGemType.Normal)
            return StoreSaleGemType.Normal;

        ulong curTime = (ulong) Utils.GetTimeStamp();
        bool startTimeOk = curTime > StartTime;
        bool endTimeOk;
        if (ignoreEndTime)
            endTimeOk = true;
        else
            endTimeOk = curTime < EndTime;

        // bool openResult = ActivityModel.Instance.CheckOpenType(OpenType, OpenTypeParam, "商城打折礼包");

        if (!ManualEnd && startTimeOk && endTimeOk) //&& openResult
            return _storeSaleType;

        return StoreSaleGemType.Normal;
    }
}