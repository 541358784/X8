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

public class MultipleScoreConfigManager : Manager<MultipleScoreConfigManager>
{
    private List<TableMultipleScore> _multipleScoreData = null;
    
    private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type, string>
    {
        {typeof(TableMultipleScore), "multiplescore"},
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
                    case "multiplescore":
                    {
                        var data = JsonConvert.DeserializeObject<List<TableMultipleScore>>(JsonConvert.SerializeObject(table[subModule.Value]));
                        if (data != null && data.Count > 0)
                            _multipleScoreData = data;
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogError(ex.ToString());
            }
        }
    }

    public List<TableMultipleScore> GetConfig()
    {
        return _multipleScoreData;
    }
}
