using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using Gameplay;
using Newtonsoft.Json;
using SomeWhere;
using UnityEngine;

public class DailyRankConfigManager : Manager<DailyRankConfigManager>
{
    public List<TableDrDifficulty> _difficultyData = new List<TableDrDifficulty>();
    public TableDrDifficulty defaultData = null;
    public List<TableDrBotscore> _botscoreData = new List<TableDrBotscore>();
    public List<TableDrReward> _rewadData = new List<TableDrReward>();
    
    
    
    private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type, string>
    {
        {typeof(TableDrDifficulty), "drDifficulty"},
        {typeof(TableDrBotscore), "drBotscore"},
        {typeof(TableDrReward), "drReward"},
    };

    public void InitFromServerData(string configJson)
    {
        Hashtable table = null;
        try
        {
            if (string.IsNullOrEmpty(configJson) == false)
            {
                table = JsonConvert.DeserializeObject<Hashtable>(configJson);
            }
        }
        catch (Exception ex)
        {
            DebugUtil.LogError(ex.ToString());
        }


        foreach (var subModule in typeToEnum)
        {
            try
            {
                switch (subModule.Value)
                {
                    case "drDifficulty":
                        _difficultyData = JsonConvert.DeserializeObject<List<TableDrDifficulty>>(JsonConvert.SerializeObject(table["drdifficulty"]));
                        break;
                    case "drBotscore":
                        _botscoreData =JsonConvert.DeserializeObject<List<TableDrBotscore>>(JsonConvert.SerializeObject(table["drbotscore"]));
                        break;
                    case "drReward":
                        _rewadData =JsonConvert.DeserializeObject<List<TableDrReward>>(JsonConvert.SerializeObject(table["drreward"]));
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogError(ex.ToString());
            }
        }
        _difficultyData.ForEach(a =>
        {
            if (a.defaultValue == 100)
                defaultData = a;
        });
    }
    
}
