using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dlugin;
using DragonU3DSDK.Storage;
using Framework;
using UnityEngine;


public class MergeResourceManager : Singleton<MergeResourceManager>
{
    public enum MergeSourcesType
    {
        None,
        Board,
        Pack,
        Reward,
        Max
    }

    private MergeSourcesType _mergeSources;
    private TableMergeItem getResourcesMerge = null;

    public TableMergeItem ResourcesTableMerge
    {
        get { return getResourcesMerge; }
    }

    public MergeSourcesType MergeSources
    {
        get { return _mergeSources; }
        private set { _mergeSources = value; }
    }

    private int packResIndex = -1;
    private int rewardResId = -1;

    public bool HaveMergeItem(int mergeId,MergeBoardEnum boardId, bool checkBox = false)
    {
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(mergeId);

        return HaveMergeItem(config, boardId,checkBox);
    }

    public bool HaveMergeItem(TableMergeItem config,MergeBoardEnum boardId, bool checkBox = false)
    {
        List<int> reLine = GetMergeReLineToChild(config);
        if (reLine == null || reLine.Count == 0)
            return false;

        if (config.reSp_line != null && config.reSp_line.Length > 0)
        {
            List<int> mergeProducts = config.reSp_line.ToList();

            if (HaveMergeItemInBoard(mergeProducts, checkBox))
                return true;

            if (HaveMergeItemInPack(mergeProducts,boardId, checkBox))
                return true;

            if (HaveMergeItemInReward(mergeProducts,boardId, checkBox))
                return true;
        }

        foreach (var id in reLine)
        {
            TableMergeItem reConfig = GameConfigManager.Instance.GetItemConfig(id);
            if (reConfig == null)
                continue;

            List<int> mergeProducts = GetMergeProducts(reConfig);
            if (reConfig.reSp_line != null && reConfig.reSp_line.Length > 0)
            {
                mergeProducts = reConfig.reSp_line.ToList();
                if (!mergeProducts.Contains(id))
                    mergeProducts.Add(id);
            }

            if (mergeProducts == null || mergeProducts.Count == 0)
                continue;

            if (HaveMergeItemInBoard(mergeProducts, checkBox))
                return true;

            if (HaveMergeItemInPack(mergeProducts,boardId, checkBox))
                return true;

            if (HaveMergeItemInReward(mergeProducts,boardId, checkBox))
                return true;

            if (reConfig.reSp_line != null && reConfig.reSp_line.Length > 0)
                return false;
        }

        return false;
    }

    public void GetMergeResource(int id, MergeBoardEnum boardId,bool containsSelf = false)
    {
        GetMergeResource(GameConfigManager.Instance.GetItemConfig(id),boardId, containsSelf);
    }

    public void GetMergeResource(TableMergeItem config, MergeBoardEnum boardId,bool containsSelf = false)
    {
        if (GuideSubSystem.Instance.IsShowingGuide())
            return;

        InitMergeResourceData();

        if (config == null)
            return;

        getResourcesMerge = config;

        if (config.reSp_line != null && config.reSp_line.Length > 0)
        {
            List<int> mergeProducts = config.reSp_line.ToList();

            if (MergeResourceInBoard(mergeProducts, true))
            {
                CloseStoreGameView();
                return;
            }

            if (MergeResourceInPack(mergeProducts,boardId))
            {
                CloseStoreGameView();
                return;
            }

            if (MergeResourceInReward(mergeProducts,boardId))
            {
                CloseStoreGameView();
                return;
            }

            if (MergeResourceInBoard(mergeProducts, false))
            {
                CloseStoreGameView();
                return;
            }
        }

        List<int> reLine = GetMergeReLineToChild(config);
        if (containsSelf)
        {
            if (reLine == null)
                reLine = new List<int>();

            if (!reLine.Contains(config.id))
                reLine.Add(config.id);
        }

        if (reLine == null || reLine.Count == 0)
            return;

        foreach (var id in reLine)
        {
            TableMergeItem reConfig = GameConfigManager.Instance.GetItemConfig(id);
            if (reConfig == null)
                continue;

            List<int> mergeProducts = GetMergeProducts(reConfig);
            if (reConfig.reSp_line != null && reConfig.reSp_line.Length > 0)
            {
                mergeProducts = reConfig.reSp_line.ToList();
                if (!mergeProducts.Contains(id))
                    mergeProducts.Add(id);
            }

            if (mergeProducts == null || mergeProducts.Count == 0)
                continue;

            if (MergeResourceInBoard(mergeProducts, true))
            {
                CloseStoreGameView();
                return;
            }

            if (MergeResourceInPack(mergeProducts,boardId))
            {
                CloseStoreGameView();
                return;
            }

            if (MergeResourceInReward(mergeProducts,boardId))
            {
                CloseStoreGameView();
                return;
            }

            if (MergeResourceInBoard(mergeProducts, false))
            {
                CloseStoreGameView();
                return;
            }

            if (reConfig.reSp_line != null && reConfig.reSp_line.Length > 0)
                break;
        }

        InitMergeResourceData();
    }

    private void CloseStoreGameView()
    {
        UIManager.Instance.CloseUI(UINameConst.UIStoreGame, true);
        UIManager.Instance.CloseUI(UINameConst.HappyGoUIStoreGam, true);
        UIManager.Instance.CloseUI(UINameConst.UIPopupPigBox, true);
    }

    public bool MergeResourceHandle(MergeSourcesType type, int index)
    {
        if (MergeSources == MergeSourcesType.None || getResourcesMerge == null)
            return false;

        if (type != MergeSources)
            return false;

        switch (type)
        {
            case MergeSourcesType.Pack:
            {
                if (packResIndex < 0)
                    return false;

                if (index != packResIndex)
                    return false;

                return true;
            }
        }

        return false;
    }

    private bool MergeResourceInBoard(List<int> mergeProducts, bool checkCD)
    {
        return MergeResourceInBoard_Main(mergeProducts, checkCD);
    }

    private bool MergeResourceInBoard_Main(List<int> mergeProducts, bool checkCD)
    {
        List<MergeBoard.Grid> grids = GetMergeGridsInBoard_Main(mergeProducts);
        if (grids == null || grids.Count == 0)
            return false;
        MergeBoard.Grid maxLevelGrid = null;
        foreach (var grid in grids)
        {
            if (grid == null || grid.board == null)
                continue;

            if (checkCD && grid.board.IsInProductCD())
                continue;

            if (maxLevelGrid == null ||
                (GameConfigManager.Instance.GetItemConfig(grid.board.id).level >
                GameConfigManager.Instance.GetItemConfig(maxLevelGrid.board.id).level))
            {
                maxLevelGrid = grid;
            }
        }

        if (maxLevelGrid != null)
        {
            MergeSources = MergeSourcesType.Board;
            MergePromptManager.Instance.ShowPrompt(
                MergeMainController.Instance.MergeBoard.IndexToPosition(MergeMainController.Instance.MergeBoard.GetGridIndex(maxLevelGrid)));
            return true;   
        }
        return false;
    }

    private bool MergeResourceInPack(List<int> mergeProducts,MergeBoardEnum boardId)
    {
        List<int> indexs = GetMergeIndexsInPack(mergeProducts,boardId);
        BagType openType = BagType.Normal;
        if (indexs == null || indexs.Count <= 0)
        {
            indexs = GetMergeIndexsInBuildPack(mergeProducts,boardId);
            openType= BagType.Building;
        }
        if (indexs != null && indexs.Count > 0)
        {
            // packResIndex = indexs[0];
            packResIndex = indexs.Last();
            MergeSources = MergeSourcesType.Pack;
            UIManager.Instance.OpenUI(UINameConst.UIPopupMergePackage,openType);
            return true;
        }

        return false;
    }

    private bool MergeResourceInReward(List<int> mergeProducts,MergeBoardEnum boardId)
    {
        rewardResId = GetMergeItemInReward_Main(mergeProducts,boardId);
        if (rewardResId > 0)
        {
            MergeSources = MergeSourcesType.Reward;
            MergePromptManager.Instance.ShowPrompt(MergeMainController.Instance.rewardBtnPos);
            return true;
        }

        return false;
    }

    public void CancelMergeResource(MergeSourcesType type,MergeBoardEnum boardId, bool mandatory = false)
    {
        if (mandatory)
        {
            CancelMergeResource();
            return;
        }

        if (MergeSources != type)
            return;

        switch (type)
        {
            case MergeSourcesType.Board:
            {
                CancelMergeResource();
                break;
            }
            case MergeSourcesType.Pack:
            {
                CancelMergeResource();
                break;
            }
            case MergeSourcesType.Reward:
            {
                if (rewardResId < 0)
                {
                    CancelMergeResource();
                    return;
                }

                for (int i = 0; i < MergeManager.Instance.GetRewardCount(boardId); i++)
                {
                    StorageMergeItem item = MergeManager.Instance.GetRewardItem(i,boardId);
                    if (item == null)
                        continue;

                    if (item.Id != rewardResId)
                        continue;

                    return;
                }

                CancelMergeResource();
                break;
            }
        }
    }

    private void CancelMergeResource()
    {
        InitMergeResourceData();

        MergePromptManager.Instance.HidePrompt();
    }

    private void InitMergeResourceData()
    {
        getResourcesMerge = null;
        MergeSources = MergeSourcesType.None;
        packResIndex = -1;
        rewardResId = -1;
    }

    public bool HaveMergeItemInBoard(List<int> mergeProducts, bool checkBox = true)
    {
        return HaveMergeItemInBoard_Main(mergeProducts, checkBox);
    }

    private bool HaveMergeItemInBoard_Main(List<int> mergeProducts, bool checkBox = true)
    {
        List<MergeBoard.Grid> grids = GetMergeGridsInBoard_Main(mergeProducts);

        if (grids == null || grids.Count == 0)
            return false;

        if (checkBox)
        {
            foreach (var grid in grids)
            {
                if (MergeManager.Instance.IsBox(grid.board.tableMergeItem))
                    return false;
            }
        }

        return true;
    }
    public bool HaveMergeItemInPack(List<int> mergeProducts, MergeBoardEnum boardId,bool checkBox = true)
    {
        if (mergeProducts == null || mergeProducts.Count == 0)
            return false;

        List<int> indexs = GetMergeIndexsInPack(mergeProducts,boardId);
        if (indexs == null || indexs.Count <= 0)
        {
            indexs = GetMergeIndexsInBuildPack(mergeProducts,boardId);
        }
        if (indexs != null && indexs.Count > 0)
        {
            if (checkBox)
            {
                foreach (var index in indexs)
                {
                    StorageMergeItem mergeItem = MergeManager.Instance.GetBagItem(index,boardId);
                    if (mergeItem == null)
                        continue;

                    TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(mergeItem.Id);
                    if (itemConfig == null)
                        continue;

                    if (MergeManager.Instance.IsBox(itemConfig))
                        return false;
                }
            }

            return true;
        }

        return false;
    }

    public bool HaveMergeItemInReward(List<int> mergeProducts,MergeBoardEnum boardId, bool checkBox = false)
    {
        return HaveMergeItemInReward_Main(mergeProducts,boardId, checkBox);
    }

    private bool HaveMergeItemInReward_Main(List<int> mergeProducts,MergeBoardEnum boardId, bool checkBox = false)
    {
        int productId = GetMergeItemInReward_Main(mergeProducts,boardId);
        if (productId < 0)
            return false;

        if (checkBox)
        {
            TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(productId);
            if (itemConfig == null)
                return false;

            if (MergeManager.Instance.IsBox(itemConfig))
                return false;
        }

        return true;
    }

    private bool HaveMergeItemInReward_HappyGo(List<int> mergeProducts,MergeBoardEnum boardId, bool checkBox = false)
    {
        int productId = GetMergeItemInReward_HappyGo(mergeProducts,boardId);
        if (productId < 0)
            return false;

        if (checkBox)
        {
            TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(productId);
            if (itemConfig == null)
                return false;

            if (MergeManager.Instance.IsBox(itemConfig))
                return false;
        }

        return true;
    }

    public int GetMergeItemInReward_Main(List<int> mergeProducts,MergeBoardEnum boardId)
    {
        if (MergeManager.Instance.GetRewardCount(boardId) == 0)
            return -1;

        foreach (var productId in mergeProducts)
        {
            for (int i = 0; i < MergeManager.Instance.GetRewardCount(boardId); i++)
            {
                StorageMergeItem item = MergeManager.Instance.GetRewardItem(i,boardId);
                if (item == null)
                    continue;

                if (item.Id != productId)
                    continue;

                return productId;
            }
        }

        return -1;
    }

    public int GetMergeItemInReward_HappyGo(List<int> mergeProducts,MergeBoardEnum boardId)
    {
        if (MergeManager.Instance.GetRewardCount(boardId) == 0)
            return -1;

        foreach (var productId in mergeProducts)
        {
            for (int i = 0; i < MergeManager.Instance.GetRewardCount(boardId); i++)
            {
                StorageMergeItem item = MergeManager.Instance.GetRewardItem(i,boardId);
                if (item == null)
                    continue;

                if (item.Id != productId)
                    continue;

                return productId;
            }
        }

        return -1;
    }

    public List<int> GetMergeIndexsInPack(List<int> mergeProducts,MergeBoardEnum boardId)
    {
        if (MergeManager.Instance.GetBagCount(boardId) == 0)
            return null;

        List<int> indexs = new List<int>();
        foreach (var productId in mergeProducts)
        {
            // for (int i = 0; i < MergeManager.Instance.GetVipBagCount(boardId); i++)
            // {
            //     StorageMergeItem mergeItem = MergeManager.Instance.GetVipBagItem(i,boardId);
            //     if (mergeItem == null)
            //         continue;
            //
            //     if (productId != mergeItem.Id)
            //         continue;
            //
            //     indexs.Add(i);
            // }
            //
            //
            // int msGagCapacity = MasterCardModel.Instance.GetAddBagNum();
            // int msLeftCount = MergeManager.Instance.GetVipBagCount(boardId);
            // int msCount = msGagCapacity > 0 ? msGagCapacity : msLeftCount;
            // msCount = Math.Max(msCount, 0);

            for (int i = 0; i < MergeManager.Instance.GetBagCount(boardId); i++)
            {
                StorageMergeItem mergeItem = MergeManager.Instance.GetBagItem(i,boardId);
                if (mergeItem == null)
                    continue;

                if (productId != mergeItem.Id)
                    continue;

                indexs.Add(i);
            }
        }

        return indexs;
    }
 public List<int> GetMergeIndexsInBuildPack(List<int> mergeProducts,MergeBoardEnum boardId)
    {
        if (MergeManager.Instance.GetBuildingBagCount(boardId) == 0)
            return null;

        List<int> indexs = new List<int>();
        foreach (var productId in mergeProducts)
        {
          
            for (int i = 0; i < MergeManager.Instance.GetBuildingBagCount(boardId); i++)
            {
                StorageMergeItem mergeItem = MergeManager.Instance.GetBuildingBagItem(i,boardId);
                if (mergeItem == null)
                    continue;

                if (productId != mergeItem.Id)
                    continue;

                indexs.Add(i);
            }
        }

        return indexs;
    }

    private List<MergeBoard.Grid> GetMergeGridsInBoard_Main(List<int> mergeProducts)
    {
        if (mergeProducts == null || mergeProducts.Count == 0)
            return null;

        List<MergeBoard.Grid> mergeGrids = new List<MergeBoard.Grid>();
        foreach (var reId in mergeProducts)
        {
            List<MergeBoard.Grid> grids =MergeMainController.Instance.MergeBoard.GetGridsById(reId);
            if (grids == null || grids.Count == 0)
                continue;

            mergeGrids.AddRange(grids);
        }

        return mergeGrids;
    }

    private List<int> GetMergeReLine(TableMergeItem config)
    {
        if (config == null)
            return null;

        if (config.re_line == null || config.re_line.Length == 0)
            return null;

        List<int> reLineList = new List<int>();
        for (int i = 0; i < config.re_line.Length; i++)
        {
            int id = config.re_line[i];
            TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(id);
            if (itemConfig == null)
                continue;

            reLineList.Add(id);
        }

        return reLineList;
    }

    private List<int> GetMergeReLineToChild(TableMergeItem config)
    {
        List<int> mergeReLine = GetMergeReLine(config);
        if (mergeReLine == null || mergeReLine.Count == 0)
            return mergeReLine;

        List<int> mergeChildReLine = new List<int>();
        foreach (var id in mergeReLine)
        {
            if (id == config.id)
                continue;

            TableMergeItem childConfig = GameConfigManager.Instance.GetItemConfig(id);
            if (childConfig == null)
                continue;

            List<int> childReLine = GetMergeReLine(childConfig);
            if (childReLine == null || childReLine.Count == 0)
                continue;

            mergeChildReLine.AddRange(childReLine);
        }

        if (mergeChildReLine.Count > 0)
        {
            foreach (var id in mergeReLine)
            {
                if (mergeChildReLine.Contains(id))
                    mergeChildReLine.Remove(id);
            }
        }

        if (mergeChildReLine.Count > 0)
        {
            mergeReLine.AddRange(mergeChildReLine);

            foreach (var id in mergeChildReLine)
            {
                if (id == config.id)
                    continue;

                TableMergeItem childConfig = GameConfigManager.Instance.GetItemConfig(id);
                if (childConfig == null)
                    continue;

                List<int> childReLine = GetMergeReLineToChild(childConfig);
                if (childReLine == null || childReLine.Count == 0)
                    continue;

                foreach (var reId in mergeReLine)
                {
                    if (childReLine.Contains(reId))
                        childReLine.Remove(reId);
                }

                mergeReLine.AddRange(childReLine);
            }
        }

        return mergeReLine;
    }


    private List<int> GetMergeProducts(TableMergeItem config)
    {
        if (config == null)
            return null;

        List<int> mergePorducts = new List<int>();
        mergePorducts.Add(config.id);
        var repeatCount = 0;
        while (config.next_level > 0)
        {
            repeatCount++;
            if (repeatCount > 100)
            {
                Debug.LogError(config.id+".nextLevel死循环");
                break;
            }
            config = GameConfigManager.Instance.GetItemConfig(config.next_level);
            if (config != null)
                mergePorducts.Add(config.id);
        }

        return mergePorducts;
    }

    private List<int> GetMergeProducts(int id)
    {
        return GetMergeProducts(GameConfigManager.Instance.GetItemConfig(id));
    }
}