using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using Decoration.DaysManager;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private int _areaId;

    [Category(Decoration)]
    [DisplayName("AreaId")]
    public int AreaId
    {
        get { return _areaId; }
        set { _areaId = value; }
    }

    private int _nodeId;

    [Category(Decoration)]
    [DisplayName("NodeId")]
    public int NodeId
    {
        get { return _nodeId; }
        set { _nodeId = value; }
    }


    [Category(Decoration)]
    [DisplayName("重置区域所有挂点")]
    public void ResetArea()
    {
        var storage = StorageManager.Instance.GetStorage<StorageDecoration>();
        if (storage == null)
            return;

        foreach (var worldKv in storage.WorldMap)
        {
            var world = worldKv.Value;
            var worldId = worldKv.Key;
            foreach (var kv in world.AreasData)
            {
                var areaId = kv.Key;
                var areaData = kv.Value;

                if (areaId != _areaId)
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

    [Category(Decoration)]
    [DisplayName("重置单独挂点")]
    public void ResetNode()
    {
        var storage = StorageManager.Instance.GetStorage<StorageDecoration>();
        if (storage == null)
            return;

        foreach (var worldKv in storage.WorldMap)
        {
            var world = worldKv.Value;
            var worldId = worldKv.Key;
            foreach (var kv in world.AreasData)
            {
                var areaId = kv.Key;
                var areaData = kv.Value;

                if (areaId != _areaId)
                    continue;

                foreach (var stage in areaData.StagesData)
                {
                    foreach (var node in stage.Value.NodesData)
                    {
                        if (_nodeId != node.Key)
                            continue;

                        node.Value.Status = 1;
                        node.Value.CurrentItemId = 0;
                    }
                }
            }
        }
    }


    [Category(Decoration)]
    [DisplayName("DayNum")]
    public int DyaNum
    {
        get { return DaysManager.Instance.DayNum; }
    }

    [Category(Decoration)]
    [DisplayName("DayStep")]
    public int DayStep
    {
        get { return DaysManager.Instance.DayStep; }
    }

    [Category(Decoration)]
    [DisplayName("校验Days表")]
    public void CheckDays()
    {
        Debug.LogError("校验Days表开始------");
        var days=  DaysManager.Instance.DaysConfigs;
        foreach (var day in days)
        {
            if(day.rewardIndex.Length!=day.rewardId.Length)
                Debug.LogError("table id ="+day.id+"  day.rewardIndex.Length!=day.rewardId.Length");
            
            if(day.rewardIndex.Length!=day.rewardNum.Length)
                Debug.LogError("table id ="+day.id+"  day.rewardIndex.Length!=day.rewardNum.Length");            
            
            if(day.rewardId.Length!=day.rewardNum.Length)
                Debug.LogError("table id ="+day.id+"  day.rewardId.Length!=day.rewardNum.Length");

            foreach (var rewardStr in day.rewardId)
            {
                var rewards = rewardStr.Split(';');
                foreach (var reward in rewards)
                {
                   var mergeItem= GameConfigManager.Instance.GetItemConfig(int.Parse(reward));
                   if(mergeItem==null)
                       Debug.LogError("table id ="+day.id+"  reward id err --->"+reward);
                }
            }
        }
        
        Debug.LogError("校验Days表结束------");

    }
}