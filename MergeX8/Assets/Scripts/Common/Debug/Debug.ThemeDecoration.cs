using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using UnityEngine;


public partial class SROptions
{
    private const string ThemeDecoration = "主题装修";
    [Category(ThemeDecoration)]
    [DisplayName("重置活动")]
    public void ResetThemeDecoration()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().ThemeDecoration.Clear();
        var guideIdList = new List<int>() {751,752,753,754,755,756};
        CleanGuideList(guideIdList);
        ThemeDecorationLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        ThemeDecorationModel.Instance.CreateStorage();
        
        
        var storage = StorageManager.Instance.GetStorage<StorageDecoration>();
        
        foreach (var worldKv in storage.WorldMap)
        {
            var world = worldKv.Value;
            var worldId = worldKv.Key;
            foreach (var kv in world.AreasData)
            {
                var areaId = kv.Key;
                var areaData = kv.Value;

                if (areaId != 880)
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

    [Category(ThemeDecoration)]
    [DisplayName("设置分数")]
    public int SetThemeDecorationScoreCount
    {
        get
        {
            return ThemeDecorationModel.Instance.GetScore();
        }
        set
        {
            var changeValue = value - ThemeDecorationModel.Instance.GetScore();
            if (changeValue < 0)
            {
                ThemeDecorationModel.Instance.ReduceScore(-changeValue,"Debug");   
            }
            else
            {
                ThemeDecorationModel.Instance.AddScore(changeValue,"Debug");
            }
        }
    }
    
    [Category(ThemeDecoration)]
    [DisplayName("设置结束时间")]
    public int SetThemeDecorationCurWorldEndTime
    {
        get
        {
            if (ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek == null)
                return 0;
            return (int)ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.GetLeftTime()/1000;
        }
        set
        {
            ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek?.SetLeftTime((long)value*1000);
        }
    }
    [Category(ThemeDecoration)]
    [DisplayName("设置购买结束时间")]
    public int SetThemeDecorationCurWorldBuyEndTime
    {
        get
        {
            if (ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek == null)
                return 0;
            return (int)ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.GetPreEndBuyLeftTime()/1000;
        }
        set
        {
            ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek?.SetPreEndBuyLeftTime((long)value*1000);
        }
    }
    
    [Category(ThemeDecoration)]
    [DisplayName("设置提前结束时间")]
    public int SetThemeDecorationCurWorldPreEndTime
    {
        get
        {
            if (ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek == null)
                return 0;
            return (int)ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.GetPreEndLeftTime()/1000;
        }
        set
        {
            ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek?.SetPreEndLeftTime((long)value*1000);
        }
    }
    [Category(ThemeDecoration)]
    [DisplayName("设置开始时间")]
    public int SetThemeDecorationPreheatTime
    {
        get
        {
            if (ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek == null)
                return 0;
            return (int)ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.GetPreheatLeftTime()/1000;
        }
        set
        {
            ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek?.SetPreheatLeftTime((long)value*1000);
        }
    }

    [Category(ThemeDecoration)]
    [DisplayName("设置排行榜结束时间")]
    public int SetThemeDecorationLeaderBoardCurWorldEndTime
    {
        get
        {
            if (ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek == null)
                return 0;
            return (int)ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek.GetLeftTime()/1000;
        }
        set
        {
            ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek?.SetLeftTime((long)value*1000);
        }
    }
    [Category(ThemeDecoration)]
    [DisplayName("设置下一个排行榜开始时间")]
    public int SetThemeDecorationLeaderBoardNextWorldStartTime
    {
        get
        {
            if (ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek != null)
                return 0;
            if (ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek == null)
                return 0;
            var curTime = (long)APIManager.Instance.GetServerTime();
            foreach (var leaderBoard in ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.LeaderBoardStorageList)
            {
                if (leaderBoard.StartTime > curTime)
                {
                    return (int)(leaderBoard.StartTime - curTime)/1000;
                }
            }
            return 0;
        }
        set
        {
            if (ThemeDecorationLeaderBoardModel.Instance.CurStorageThemeDecorationLeaderBoardWeek != null)
                return;
            if (ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek == null)
                return;
            var curTime = (long)APIManager.Instance.GetServerTime();
            foreach (var leaderBoard in ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek.LeaderBoardStorageList)
            {
                if (leaderBoard.StartTime > curTime)
                {
                    leaderBoard.SetStartTime(value * 1000);
                    return;
                }
            }
        }
    }
}