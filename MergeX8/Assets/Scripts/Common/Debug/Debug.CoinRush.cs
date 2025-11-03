using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    [Category(CoinRush)]
    [DisplayName("重置金币rush")]
    public void ResetCoinRush()
    {
        HideDebugPanel();
        
        StorageManager.Instance.GetStorage<StorageHome>().CoinRush.Clear();
        
        // var guideIdList = new List<int>() {520, 521, 522, 523, 524,525,526,527};
        // var guideFinish = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
        // var cacheGuideFinished = GuideSubSystem.Instance.CacheGuideFinished;
        // for (var i = 0; i < guideIdList.Count; i++)
        // {
        //     var guideId = guideIdList[i];
        //     if (guideFinish.ContainsKey(guideId))
        //     {
        //         guideFinish.Remove(guideId);
        //     }
        //     if (cacheGuideFinished.ContainsKey(guideId))
        //     {
        //         cacheGuideFinished.Remove(guideId);
        //     }
        // }
    }

    public int _coinRushCollectType;
    [Category(CoinRush)]
    [DisplayName("类型")]
    public int CoinRushCollectType
    {
        get
        {
            return _coinRushCollectType;
        }
        set
        {
            _coinRushCollectType = value;
        }
    }

    public int _coinRushCollectCount;
    [Category(CoinRush)]
    [DisplayName("数量")]
    public int CoinRushCollectCount
    {
        get
        {
            return _coinRushCollectCount;
        }
        set
        {
            _coinRushCollectCount = value;
        }
    }

    [Category(CoinRush)]
    [DisplayName("消耗")]
    public void CoinRushConsume()
    {
        CoinRushModel.Instance.OnConsumeRes((UserData.ResourceId)_coinRushCollectType,_coinRushCollectCount);
    }
    [Category(CoinRush)]
    [DisplayName("获得")]
    public void CoinRushAddRes()
    {
        if (_coinRushCollectType == (int)UserData.ResourceId.Coin)
        {
            CoinRushModel.Instance.OnGetCoin(_coinRushCollectCount);
        }
        else
        {
            CoinRushModel.Instance.OnAddRes((UserData.ResourceId)_coinRushCollectType,_coinRushCollectCount);
        }
    }
}