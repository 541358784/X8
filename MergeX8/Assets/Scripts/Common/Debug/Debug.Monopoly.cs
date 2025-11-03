using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonPlus.Config.Monopoly;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using UnityEngine;


public partial class SROptions
{
    private const string Monopoly = "大富翁";
    [Category(Monopoly)]
    [DisplayName("重置活动")]
    public void ResetMonopoly()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().Monopoly.Clear();
        foreach (var pair in StorageManager.Instance.GetStorage<StorageHome>().MonopolyLeaderBoard)
        {
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(pair.Value.ActivityId);
        }
        StorageManager.Instance.GetStorage<StorageHome>().MonopolyLeaderBoard.Clear();
        var guideIdList = new List<int>() {781,782,783,784,785,786,787,788,789,790};
        CleanGuideList(guideIdList);
        MonopolyModel.Instance.CreateStorage();

        int resetAreaId = 900;
        var decoItemList = new List<int>();
        if (MonopolyConfigManager.Instance.MonopolyStoreItemConfigList != null)
        {
            foreach (var config in MonopolyConfigManager.Instance.MonopolyStoreItemConfigList)
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

    [Category(Monopoly)]
    [DisplayName("设置骰子数量")]
    public int SetMonopolyStarCount
    {
        get
        {
            return MonopolyModel.Instance.GetDiceCount();
        }
        set
        {
            MonopolyModel.Instance.AddDice(value-MonopolyModel.Instance.GetDiceCount(),"Debug");
        }
    }
    
    [Category(Monopoly)]
    [DisplayName("设置积分")]
    public int SetMonopolyScoreCount
    {
        get
        {
            return MonopolyModel.Instance.GetScore();
        }
        set
        {
            var changeValue = value - MonopolyModel.Instance.GetScore();
            if (changeValue < 0)
            {
                MonopolyModel.Instance.ReduceScore(-changeValue,"Debug");   
            }
            else
            {
                MonopolyModel.Instance.CurStorageMonopolyWeek?.AddScore(changeValue,"Debug");
            }
        }
    }
    
    [Category(Monopoly)]
    [DisplayName("设置结束时间")]
    public int SetMonopolyCurWorldEndTime
    {
        get
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return 0;
            return (int)(MonopolyModel.Instance.CurStorageMonopolyWeek.GetLeftTime()/1000);
        }
        set
        {
            var storage = MonopolyModel.Instance.CurStorageMonopolyWeek;
            if (storage != null)
            {
                storage.SetLeftTime((long)value*1000);
                var leaderBoardStorage = MonopolyLeaderBoardModel.Instance.GetLeaderBoardStorage(storage.ActivityId);
                leaderBoardStorage?.SetLeftTime((long)value*1000);
            }
        }
    }

    private int _MonopolyCardId = 1;
    [Category(Monopoly)]
    [DisplayName("卡Id")]
    public int MonopolyCardId
    {
        get
        {
            return _MonopolyCardId;
        }
        set
        {
            _MonopolyCardId = value;
        }
    }

    [Category(Monopoly)]
    [DisplayName("获得卡")]
    public void AddMonopolyCard()
    {
        MonopolyModel.Instance.DebugAddCard(_MonopolyCardId);
    }
    
    [Category(Monopoly)]
    [DisplayName("设置开始时间")]
    public int SetMonopolyPreheatTime
    {
        get
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return 0;
            return (int)(MonopolyModel.Instance.CurStorageMonopolyWeek.GetPreheatLeftTime()/1000);
        }
        set
        {
            var storage = MonopolyModel.Instance.CurStorageMonopolyWeek;
            if (storage != null)
            {
                storage.SetPreheatLeftTime((long)value*1000);
                var leaderBoardStorage = MonopolyLeaderBoardModel.Instance.GetLeaderBoardStorage(storage.ActivityId);
                leaderBoardStorage?.SetStartTime((long)value*1000);
            }
        }
    }

    [Category(Monopoly)]
    [DisplayName("卡池状态")]
    public string MonopolyCardPoolState
    {
        get
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return "";
            var result = "";
            for (var i = 0; i < MonopolyModel.Instance.CurStorageMonopolyWeek.CardRandomPool.Count; i++)
            {
                result += MonopolyModel.Instance.CurStorageMonopolyWeek.CardRandomPool[i];
                result += ",";
            }
            return result;
        }
    }
    
    [Category(Monopoly)]
    [DisplayName("骰子池状态")]
    public string MonopolyDicePoolState
    {
        get
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return "";
            var result = "";
            for (var i = 0; i < MonopolyModel.Instance.CurStorageMonopolyWeek.DiceRandomPool.Count; i++)
            {
                result += MonopolyModel.Instance.CurStorageMonopolyWeek.DiceRandomPool[i];
                result += ",";
            }
            return result;
        }
    }
    
    [Category(Monopoly)]
    [DisplayName("自动转")]
    public bool MonopolyAutoSpin
    {
        get
        {
            return UIMonopolyMainController.IsAuto;
        }
        set
        {
            UIMonopolyMainController.IsAuto = value;
        }
    }
    
    [Category(Monopoly)]
    [DisplayName("设置宝箱分数")]
    public int SetMonopolyRewardBoxCollectNum
    {
        get
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return 0;
            return (int) MonopolyModel.Instance.CurStorageMonopolyWeek.RewardBoxCollectNum;
        }
        set
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return;
            if (MonopolyModel.Instance.CurStorageMonopolyWeek.RewardBoxCollectNum == value)
                return;
            var oldValue = MonopolyModel.Instance.CurStorageMonopolyWeek.RewardBoxCollectNum; 
            MonopolyModel.Instance.CurStorageMonopolyWeek.RewardBoxCollectNum = value;
            EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIAddRewardBoxScore(
                MonopolyModel.Instance.CurStorageMonopolyWeek.GetCurRewardBoxConfig(),oldValue,value));
        }
    }
    [Category(Monopoly)]
    [DisplayName("设置格子")]
    public int SetMonopolyBlockIndex
    {
        get
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return 0;
            return (int) MonopolyModel.Instance.CurStorageMonopolyWeek.CurBlockIndex;
        }
        set
        {
            if (MonopolyModel.Instance.CurStorageMonopolyWeek == null)
                return;
            if (MonopolyModel.Instance.CurStorageMonopolyWeek.CurBlockIndex == value)
                return;
            MonopolyModel.Instance.CurStorageMonopolyWeek.CurBlockIndex = value%MonopolyModel.Instance.BlockConfigList.Count;
            MonopolyModel.Instance.CurStorageMonopolyWeek.CurBlockBuyState =
                (MonopolyBlockType) MonopolyModel.Instance
                    .BlockConfigList[MonopolyModel.Instance.CurStorageMonopolyWeek.CurBlockIndex].BlockType
                == MonopolyBlockType.Score;
        }
    }
}