using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonPlus.Config.Easter2024;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using UnityEngine;


public partial class SROptions
{
    private const string Easter2024 = "复活节2024";
    [Category(Easter2024)]
    [DisplayName("重置活动")]
    public void ResetEaster2024()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().Easter2024.Clear();
        var guideIdList = new List<int>() {720,721,722,723,724,725,726,727,728,729};
        CleanGuideList(guideIdList);
        Easter2024LeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        Easter2024Model.Instance.CreateStorage();
        
        
        
        
        int resetAreaId = 900;
        var decoItemList = new List<int>();
        if (Easter2024ConfigManager.Instance.Easter2024StoreItemConfigList != null)
        {
            foreach (var config in Easter2024ConfigManager.Instance.Easter2024StoreItemConfigList)
            {
                if (config.Type == 2)
                {
                    decoItemList.Add(config.RewardId[0]);
                    resetAreaId = config.RewardId[0] / 10000;
                }
            }
        }
        foreach (var item in decoItemList)
        {
            DecoManager.Instance.LockDecoBuilding(item);
        }
        var storage = StorageManager.Instance.GetStorage<StorageDecoration>();
        
        foreach (var worldKv in storage.WorldMap)
        {
            var world = worldKv.Value;
            var worldId = worldKv.Key;
            foreach (var kv in world.AreasData)
            {
                var areaId = kv.Key;
                var areaData = kv.Value;

                if (areaId != resetAreaId)
                    continue;
                areaData.State = 1;
                foreach (var stage in areaData.StagesData)
                {
                    stage.Value.State = 1;
                    foreach (var node in stage.Value.NodesData)
                    {
                        node.Value.Status = 1;
                        node.Value.CurrentItemId =
                            DecorationConfigManager.Instance.GetNodeConfig(node.Value.Id).defaultItem;
                    }
                }
            }
        }
    }

    [Category(Easter2024)]
    [DisplayName("设置蛋数量")]
    public int SetEaster2024StarCount
    {
        get
        {
            return Easter2024Model.Instance.GetEgg();
        }
        set
        {
            Easter2024Model.Instance.AddEgg(value-Easter2024Model.Instance.GetEgg(),"Debug");
        }
    }
    
    [Category(Easter2024)]
    [DisplayName("设置胡萝卜")]
    public int SetEaster2024ScoreCount
    {
        get
        {
            return Easter2024Model.Instance.GetScore();
        }
        set
        {
            var changeValue = value - Easter2024Model.Instance.GetScore();
            if (changeValue < 0)
            {
                Easter2024Model.Instance.ReduceScore(-changeValue,"Debug");   
            }
            else
            {
                Easter2024Model.Instance.AddScore(changeValue,"Debug");
            }
        }
    }
    
    [Category(Easter2024)]
    [DisplayName("设置结束时间")]
    public int SetEaster2024CurWorldEndTime
    {
        get
        {
            if (Easter2024Model.Instance.CurStorageEaster2024Week == null)
                return 0;
            return (int)Easter2024Model.Instance.CurStorageEaster2024Week.GetLeftTime()/1000;
        }
        set
        {
            Easter2024Model.Instance.CurStorageEaster2024Week?.SetLeftTime((long)value*1000);
        }
    }

    private int _easter2024CardId = 1;
    [Category(Easter2024)]
    [DisplayName("卡Id")]
    public int Easter2024CardId
    {
        get
        {
            return _easter2024CardId;
        }
        set
        {
            _easter2024CardId = value;
        }
    }

    [Category(Easter2024)]
    [DisplayName("获得卡")]
    public void AddEaster2024Card()
    {
        Easter2024Model.Instance.DebugAddCard(_easter2024CardId);
    }
    
    
    [Category(Easter2024)]
    [DisplayName("设置结束时间")]
    public int SetEaster2024LeaderBoardCurWorldEndTime
    {
        get
        {
            if (Easter2024Model.Instance.CurStorageEaster2024Week == null)
                return 0;
            return (int)Easter2024Model.Instance.CurStorageEaster2024Week.GetLeftTime()/1000;
        }
        set
        {
            Easter2024Model.Instance.CurStorageEaster2024Week?.SetLeftTime((long)value*1000);
        }
    }
    [Category(Easter2024)]
    [DisplayName("设置开始时间")]
    public int SetEaster2024PreheatTime
    {
        get
        {
            if (Easter2024Model.Instance.CurStorageEaster2024Week == null)
                return 0;
            return (int)Easter2024Model.Instance.CurStorageEaster2024Week.GetPreheatLeftTime()/1000;
        }
        set
        {
            Easter2024Model.Instance.CurStorageEaster2024Week?.SetPreheatLeftTime((long)value*1000);
        }
    }

    [Category(Easter2024)]
    [DisplayName("填满小游戏进度")]
    public void FullMiniGame()
    {
        Easter2024Model.Instance.AddLuckyPoint(3);
    }

    [Category(Easter2024)]
    [DisplayName("卡池状态")]
    public string CardPoolState
    {
        get
        {
            if (Easter2024Model.Instance.CurStorageEaster2024Week == null)
                return "";
            var result = "";
            for (var i = 0; i < Easter2024Model.Instance.CurStorageEaster2024Week.CardRandomPool.Count; i++)
            {
                result += Easter2024Model.Instance.CurStorageEaster2024Week.CardRandomPool[i];
                result += ",";
            }
            return result;
        }
    }
    
    [Category(Easter2024)]
    [DisplayName("大蛋")]
    public int Easter2024BigBall
    {
        get
        {
            return Easter2024Model.Instance.DebugBigBall;
        }
        set
        {
            Easter2024Model.Instance.DebugBigBall = value;
        }
    }
}