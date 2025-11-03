using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using UnityEngine;


public partial class SROptions
{
    private const string SnakeLadder = "蛇梯子";
    [Category(SnakeLadder)]
    [DisplayName("重置活动")]
    public void ResetSnakeLadder()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().SnakeLadder.Clear();
        var guideIdList = new List<int>() {741,742,743,744,745,746,747,748,749};
        CleanGuideList(guideIdList);
        SnakeLadderLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        SnakeLadderModel.Instance.CreateStorage();
        
        
        var storage = StorageManager.Instance.GetStorage<StorageDecoration>();
        storage.ExtendArea[1] = 888;
        
        foreach (var worldKv in storage.WorldMap)
        {
            var world = worldKv.Value;
            var worldId = worldKv.Key;
            foreach (var kv in world.AreasData)
            {
                var areaId = kv.Key;
                var areaData = kv.Value;

                if (areaId != 889)
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

    [Category(SnakeLadder)]
    [DisplayName("设置蛋数量")]
    public int SetSnakeLadderStarCount
    {
        get
        {
            return SnakeLadderModel.Instance.GetTurntableCount();
        }
        set
        {
            SnakeLadderModel.Instance.AddTurntable(value-SnakeLadderModel.Instance.GetTurntableCount(),"Debug");
        }
    }
    
    [Category(SnakeLadder)]
    [DisplayName("设置胡萝卜")]
    public int SetSnakeLadderScoreCount
    {
        get
        {
            return SnakeLadderModel.Instance.GetScore();
        }
        set
        {
            var changeValue = value - SnakeLadderModel.Instance.GetScore();
            if (changeValue < 0)
            {
                SnakeLadderModel.Instance.ReduceScore(-changeValue,"Debug");   
            }
            else
            {
                SnakeLadderModel.Instance.AddScore(changeValue,"Debug");
            }
        }
    }
    
    [Category(SnakeLadder)]
    [DisplayName("设置结束时间")]
    public int SetSnakeLadderCurWorldEndTime
    {
        get
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return 0;
            return (int)SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.GetLeftTime()/1000;
        }
        set
        {
            SnakeLadderModel.Instance.CurStorageSnakeLadderWeek?.SetLeftTime((long)value*1000);
        }
    }

    private int _SnakeLadderCardId = 1;
    [Category(SnakeLadder)]
    [DisplayName("卡Id")]
    public int SnakeLadderCardId
    {
        get
        {
            return _SnakeLadderCardId;
        }
        set
        {
            _SnakeLadderCardId = value;
        }
    }

    [Category(SnakeLadder)]
    [DisplayName("获得卡")]
    public void AddSnakeLadderCard()
    {
        SnakeLadderModel.Instance.DebugAddCard(_SnakeLadderCardId);
    }
    
    
    [Category(SnakeLadder)]
    [DisplayName("设置结束时间")]
    public int SetSnakeLadderLeaderBoardCurWorldEndTime
    {
        get
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return 0;
            return (int)SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.GetLeftTime()/1000;
        }
        set
        {
            SnakeLadderModel.Instance.CurStorageSnakeLadderWeek?.SetLeftTime((long)value*1000);
        }
    }
    [Category(SnakeLadder)]
    [DisplayName("设置开始时间")]
    public int SetSnakeLadderPreheatTime
    {
        get
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return 0;
            return (int)SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.GetPreheatLeftTime()/1000;
        }
        set
        {
            SnakeLadderModel.Instance.CurStorageSnakeLadderWeek?.SetPreheatLeftTime((long)value*1000);
        }
    }

    [Category(SnakeLadder)]
    [DisplayName("卡池状态")]
    public string SnakeLadderCardPoolState
    {
        get
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return "";
            var result = "";
            for (var i = 0; i < SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.TurntableRandomPool.Count; i++)
            {
                result += SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.TurntableRandomPool[i];
                result += ",";
            }
            return result;
        }
    }
    
    [Category(SnakeLadder)]
    [DisplayName("自动转")]
    public bool SnakeLadderAutoSpin
    {
        get
        {
            return UISnakeLadderMainController.IsAuto;
        }
        set
        {
            UISnakeLadderMainController.IsAuto = value;
        }
    }
    
    [Category(SnakeLadder)]
    [DisplayName("设置通关次数")]
    public int SetSnakeLadderCompleteTimes
    {
        get
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return 0;
            return (int) SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.CompleteTimes;
        }
        set
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return;
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.CompleteTimes == value)
                return;
            var CurStorageSnakeLadderWeek = SnakeLadderModel.Instance.CurStorageSnakeLadderWeek;
            var lastCurLevel = CurStorageSnakeLadderWeek.GetCurLevel();
            CurStorageSnakeLadderWeek.CompleteTimes = value;
            CurStorageSnakeLadderWeek.CurBlockIndex = 0;
            var newCurLevel = CurStorageSnakeLadderWeek.GetCurLevel();
            if (lastCurLevel != newCurLevel)
            {
                for (var i = 0; i < CurStorageSnakeLadderWeek.TurntableRandomPool.Count; i++)
                {
                    var cardId = CurStorageSnakeLadderWeek.TurntableRandomPool[i];
                    if (!newCurLevel.CardPool().Contains(cardId))
                    {
                        if (cardId > 100)
                        {
                            for (var j = 0; j < newCurLevel.CardPool().Count; j++)
                            {
                                var newCardId = newCurLevel.CardPool()[j];
                                if (newCardId > 100 && newCardId % 100 == cardId % 100)
                                {
                                    CurStorageSnakeLadderWeek.TurntableRandomPool[i] = newCardId;
                                    break;
                                }
                            }   
                        }
                        else
                        {
                            newCurLevel.CardPool().RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderLevelUp(CurStorageSnakeLadderWeek.GetCurLevel()));
        }
    }
    [Category(SnakeLadder)]
    [DisplayName("设置格子")]
    public int SetSnakeLadderBlockIndex
    {
        get
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return 0;
            return (int) SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.CurBlockIndex;
        }
        set
        {
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek == null)
                return;
            if (SnakeLadderModel.Instance.CurStorageSnakeLadderWeek.CurBlockIndex == value)
                return;
            var CurStorageSnakeLadderWeek = SnakeLadderModel.Instance.CurStorageSnakeLadderWeek;
            CurStorageSnakeLadderWeek.CurBlockIndex = value;
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderLevelUp(CurStorageSnakeLadderWeek.GetCurLevel()));
        }
    }
}