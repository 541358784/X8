using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;


public partial class SROptions
{
    [Category(DailyRank)]
    [DisplayName("重置每日排行")]
    public void RestDailyRank()
    {
        DailyRankModel.Instance._dailyRankGroup.DailyRanks.Clear();;
        DailyRankModel.Instance._dailyRankGroup.WinCount = 0;
        DailyRankModel.Instance._dailyRankGroup.DefaultValue = 0;
        DailyRankModel.Instance._dailyRankGroup.DefaultValues.Clear();
        DailyRankModel.Instance._dailyRankGroup.LostCount = 0;
        DailyRankModel.Instance._dailyRankGroup.DifficultyId = 0;
    }
    
    
    [Category(DailyRank)]
    [DisplayName("刷新机器人时间")]
    public string UpdateTime
    {
        get
        {
            if(DailyRankModel.Instance._curDailyRank != null)
                return CommonUtils.FormatLongToTimeStr((long)APIManager.Instance.GetServerTime()-(long)DailyRankModel.Instance._curDailyRank.UpdateTime);
            return
                "0:0";
        }
    }
    
    [Category(DailyRank)]
    [DisplayName("强制刷新机器人时间")]
    public void UpdateRobotTime()
    {
        DailyRankModel.Instance.UpdateRobotInfo(true, null);
    }
    
    [Category(DailyRank)]
    [DisplayName("连赢次数")]
    public int WinCount
    {
        get
        {
            return DailyRankModel.Instance._dailyRankGroup.WinCount;
        }
        set
        {
            DailyRankModel.Instance._dailyRankGroup.WinCount = value;
        }
    }
    
    [Category(DailyRank)]
    [DisplayName("连输次数")]
    public int LostCount
    {
        get
        {
            return DailyRankModel.Instance._dailyRankGroup.LostCount;
        }
        set
        {
            DailyRankModel.Instance._dailyRankGroup.LostCount = value;
        }
    }
    
    [Category(DailyRank)]
    [DisplayName("当前分数")]
    public int CurScore
    {
        get
        {
            if(DailyRankModel.Instance._curDailyRank != null)
                return DailyRankModel.Instance._curDailyRank.CurScore;
            return 0;
        }
        set
        {
            if(DailyRankModel.Instance._curDailyRank != null)
                DailyRankModel.Instance._curDailyRank.CurScore = value;
        }
    }
    
    
    [Category(DailyRank)]
    [DisplayName("当前难度id")]
    public int DifficultyId
    {
        get
        {
            return DailyRankModel.Instance._dailyRankGroup.DifficultyId;
        }
        set
        {
            DailyRankModel.Instance._dailyRankGroup.DifficultyId = value;
        }
    }
    
    [Category(DailyRank)]
    [Sort(-30)]
    [DisplayName("机器人列表")]
    public string RobotInfo
    {
        get
        {
            if (DailyRankModel.Instance._curDailyRank != null)
            {
                string robotName = "";
                
                for (int i = 0; i < DailyRankModel.Instance._curDailyRank.Robots.Count; i++)
                {
                    robotName += $"I:{i} Name: {DailyRankModel.Instance._curDailyRank.Robots[i].RobotName} Score: {DailyRankModel.Instance._curDailyRank.Robots[i].CurScore}";

                    if (i != DailyRankModel.Instance._curDailyRank.Robots.Count - 1)
                        robotName += "\n";
                }

                return robotName;
            }

            return "";
        }
    }
    
}