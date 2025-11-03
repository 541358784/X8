using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Difference;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

public partial class GameConfigManager : Manager<GameConfigManager>
{
    public List<TableMergeItem> MergeItemList;
    public List<TableMergeLine> MergeLineList;
    private List<TableBoardGrid> BoardGridList;
    private List<TableBoardGrid> BoardGridList_2;
    private List<TableBoardGrid> BoardGridList_3;
    private List<TableBoardGrid> BoardGridList_4;
    public List<TableChoiceChest> ChoiceChestList;
    public List<TableLevel> LevelList;
    public List<TableBag> BagList;
    public List<TableBagBuilding> BagBuildingList;
    public List<TableRecovery> RecoveryList;
    public List<TableUnlockMergeLine> UnlockMergeLines;
    
    private Dictionary<int, TableMergeItem> mergeItemDir = new Dictionary<int, TableMergeItem>();
    private Dictionary<int, TableChoiceChest> choiceChestDir = new Dictionary<int, TableChoiceChest>();
    private Dictionary<int, TableMergeLine> mergeLineDir = new Dictionary<int, TableMergeLine>();
    private Dictionary<int, List<TableMergeItem>> mergeInLineDir = new Dictionary<int, List<TableMergeItem>>();

    private Dictionary<int, List<TableMergeItem>> mergeSourceItems = new Dictionary<int, List<TableMergeItem>>();

    private Dictionary<int, TableRecovery> RecoveryDir = new Dictionary<int, TableRecovery>();

    public List<TableBoardGrid> BoardGrid
    {
        get
        {
            if (DifferenceManager.Instance.Is_Plan_C)
                return BoardGridList_4;
            
            if (DifferenceManager.Instance.IsOpenSecondGuide)
                return BoardGridList_3;
            
            if (DifferenceManager.Instance.IsOpenDifference)
                return BoardGridList_2;

            return BoardGridList;
        }
    }
    
    
    public List<TableBoardGrid> BoardGrid_1=>BoardGridList;
    public List<TableBoardGrid> BoardGrid_2=>BoardGridList_2;
    public List<TableBoardGrid> BoardGrid_3=>BoardGridList_3;
    public List<TableBoardGrid> BoardGrid_4=>BoardGridList_4;
    
    
    
    public bool InitFlag = false;
    public void InitConfigs()
    {
        InitFlag = false;
        TableManager.Instance.InitLocation("configs/merge");
        MergeItemList = TableManager.Instance.GetTable<TableMergeItem>();
        MergeLineList = TableManager.Instance.GetTable<TableMergeLine>();
        BoardGridList = TableManager.Instance.GetTable<TableBoardGrid>();
        ChoiceChestList = TableManager.Instance.GetTable<TableChoiceChest>();
        UnlockMergeLines = TableManager.Instance.GetTable<TableUnlockMergeLine>();
        LevelList = TableManager.Instance.GetTable<TableLevel>();
        BagList = TableManager.Instance.GetTable<TableBag>();
        BagBuildingList = TableManager.Instance.GetTable<TableBagBuilding>();
        RecoveryList = TableManager.Instance.GetTable<TableRecovery>();

        TextAsset json2 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/merge","boardgrid_2"));
        BoardGridList_2 = TableManager.DeSerialize<TableBoardGrid>(json2.text);
            
        TextAsset json3 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/merge","boardgrid_3"));
        BoardGridList_3 = TableManager.DeSerialize<TableBoardGrid>(json3.text);
        
        TextAsset json4 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/merge","boardgrid_4"));
        BoardGridList_4 = TableManager.DeSerialize<TableBoardGrid>(json4.text);
        
        InitTable(MergeItemList, mergeItemDir);
        InitTable(ChoiceChestList, choiceChestDir);
        InitTable(MergeLineList, mergeLineDir);
        InitTable(RecoveryList, RecoveryDir);
        //DebugUtil.Log("MergeItemList  Count ---->"+MergeItemList.Count);
        //DebugUtil.Log("MergeLineList  Count ---->"+MergeLineList.Count);
        mergeInLineDir.Clear();
        MergeItemList.ForEach(a =>
        {
            List<TableMergeItem> mergeItems = null;
            if (mergeInLineDir.ContainsKey(a.in_line))
            {
                mergeItems = mergeInLineDir[a.in_line];
            }
            else
            {
                mergeItems = new List<TableMergeItem>();
                mergeInLineDir.Add(a.in_line, mergeItems);
            }

            mergeItems.Add(a);
        });

        foreach (var kv in mergeInLineDir)
        {
            kv.Value.Sort((x, y) => x.level - y.level);
        }


        foreach (var merge in MergeItemList)
        {
            List<TableMergeItem> mergeSourceItem = null;
            if (mergeSourceItems.ContainsKey(merge.in_line))
            {
                mergeSourceItem = mergeSourceItems[merge.in_line];
            }
            else
            {
                mergeSourceItem = new List<TableMergeItem>();
                mergeSourceItems.Add(merge.in_line, mergeSourceItem);
            }

            if (mergeSourceItem != null && mergeSourceItem.Count > 0)
                continue;

            List<int> reLine = GetMergeReLine(merge);
            if (reLine == null || reLine.Count == 0)
                continue;

            foreach (var id in reLine)
            {
                TableMergeLine mergeLine = GetMergeLine(id);
                if (mergeLine == null)
                    continue;
                if (mergeLine.output != null && mergeLine.output.Length > 0)
                {
                    foreach (var mergeId in mergeLine.output)
                    {
                        TableMergeItem mergeItem = GetItemConfig(mergeId);
                        if (mergeItem == null)
                            continue;

                        if (!mergeItem.isSpecial)
                            continue;

                        mergeSourceItem.Add(mergeItem);
                    }
                }
            }
        }

        foreach (var kv in mergeSourceItems)
        {
            kv.Value.Sort((x, y) => x.price - y.price);
        }
        InitFlag = true;
    }

    private void InitTable<T>(List<T> tableData, Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            try
            {
                config.Add(kv.GetID(), kv);
            }
            catch (Exception e)
            {
                Debug.LogError("重复的key 表: " + typeof(T).ToString() + "\t ID: " + kv.GetID().ToString());
            }
        }
    }
    public TableChoiceChest GetChoiceChest(int id)
    {
        if (choiceChestDir.ContainsKey(id))
            return choiceChestDir[id];

        return null;
    }

    public TableMergeItem GetItemConfig(int id)
    {
        if (mergeItemDir.ContainsKey(id))
            return mergeItemDir[id];

        return null;
    }

    public TableMergeLine GetMergeLine(int id)
    {
        if (mergeLineDir.ContainsKey(id))
            return mergeLineDir[id];

        return null;
    }

    public List<TableMergeItem> GetMergeInLineItems(int inLine)
    {
        if (mergeInLineDir == null)
            return null;

        if (mergeInLineDir.ContainsKey(inLine))
            return mergeInLineDir[inLine];

        return null;
    }

    public List<int> GetMergeReLine(TableMergeItem mergeConfig)
    {
        if (mergeConfig == null)
            return null;

        TableMergeLine mergeLine = GetMergeLine(mergeConfig.in_line);
        if (mergeLine == null)
            return null;

        if (mergeLine.re_line == null || mergeLine.re_line.Length == 0)
            return null;

        List<int> reLines = new List<int>();
        reLines.Add(mergeConfig.in_line);

        foreach (var kv in mergeLineDir)
        {
            if (mergeLine == kv.Value)
                continue;

            if (kv.Value == null)
                continue;

            if (kv.Value.re_line == null)
                continue;

            foreach (var reLine in mergeLine.re_line)
            {
                if (kv.Value.re_line.Contains(reLine))
                    reLines.Add(kv.Key);
            }
        }

        return reLines;
    }

    public List<TableMergeItem> GetMergeResources(int resourceLine)
    {
        if (mergeSourceItems == null)
            return null;

        if (!mergeSourceItems.ContainsKey(resourceLine))
            return null;

        return mergeSourceItems[resourceLine];
    }

   

   
    public TableRecovery GetRecovery(int id)
    {
        if (RecoveryDir == null)
            return null;

        if (!RecoveryDir.ContainsKey(id))
            return null;

        return RecoveryDir[id];
    }
}