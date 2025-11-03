using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using System;
using DragonPlus.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using GamePool;

public partial class UIPopupMergePackageController : UIWindow
{
    List<MergePackageUnit> BuildingBagunits = new List<MergePackageUnit>();

    private void InitBuildingPackageCell()
    {
        for (int i = 0; i < MergeManager.Instance.GetBuildingBagCount(MergeBoardEnum.Main); i++)
        {
            Transform item = AddBuildingPackageItem();
            var script = item.gameObject.GetComponentDefault<MergePackageUnit>();
            script.SetBoardId(MergeBoardEnum.Main);
            BuildingBagunits.Add(script);
        }

        int bagCapacity = MergeManager.Instance.GetBuildingBagCapacity(MergeBoardEnum.Main);

        int leftCount = bagCapacity - MergeManager.Instance.GetBuildingBagCount(MergeBoardEnum.Main);
        leftCount = Mathf.Max(0, leftCount);
        for (int i = 0; i <= leftCount; i++)
        {
            Transform item = AddBuildingPackageItem();
            var script = item.gameObject.GetComponentDefault<MergePackageUnit>();
            script.SetBoardId(MergeBoardEnum.Main);
            BuildingBagunits.Add(script);
        }
    }
    
    public Transform AddBuildingPackageItem()
    {
        Transform item = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergePackageUnit).transform;
        item.parent = _dicBagAreas[BagType.Building].AreaContent.transform;
        item.localScale = Vector3.one;
        item.localPosition = Vector3.zero;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_dicBagAreas[BagType.Building].AreaContent);
        return item;
    }
    
    public void RefreshBuildingBag()
    {
        int m_index = 0;
        var board = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main);
        int bagCapacity = MergeManager.Instance.GetBuildingBagCapacity(MergeBoardEnum.Main);
        int showIndex = -1;


        for (int i = 0; i < MergeManager.Instance.GetBuildingBagCount(MergeBoardEnum.Main); i++)
        {
            m_index = i ;
            int id = board.BuildingBags[i].Id;
            var config = GameConfigManager.Instance.GetItemConfig(id);
            if (m_index >= BuildingBagunits.Count)
                continue;

            BuildingBagunits[m_index].SetItemInfomation(config, m_index, MergePackageUnitType.buildBag);

            if (showIndex < 0 &&
                MergeResourceManager.Instance.MergeResourceHandle(MergeResourceManager.MergeSourcesType.Pack, m_index))
            {
                showIndex = m_index;
                MergeResourceController controller =
                    MergePromptManager.Instance.ShowPrompt(BuildingBagunits[m_index].transform.position);
                if (controller != null)
                {
                    controller.transform.SetParent(BuildingBagunits[m_index].transform);
                    controller.transform.localPosition = Vector3.zero;
                    controller.transform.localScale = Vector3.one;
                    controller.canvas.sortingOrder = canvas.sortingOrder + 1;
                }
            }
        }

        int leftCount = bagCapacity - MergeManager.Instance.GetBuildingBagCount(MergeBoardEnum.Main);
        m_index = BuildingBagunits.Count - leftCount - 1;
        m_index = Math.Max(m_index, 0);

        for (int i = 0; i < leftCount; i++)
        {
            if (m_index + i < BuildingBagunits.Count)
                BuildingBagunits[m_index + i].SetItemInfomation(null, -1, MergePackageUnitType.buildBagUnlock);
        }

        if (MergeManager.Instance.GetBuildingBagCapacity(MergeBoardEnum.Main) >= _maxCount)
        {
            BuildingBagunits[BuildingBagunits.Count - 1].SetItemInfomation(null, -1, MergePackageUnitType.buildBagMax);
            BuildingBagunits[BuildingBagunits.Count - 1].SetBagCost(0,0);
        }
        else
        {
            BuildingBagunits[BuildingBagunits.Count - 1].SetItemInfomation(null, -1, MergePackageUnitType.buildBaglock);
            BuildingBagunits[BuildingBagunits.Count - 1]
                .SetBagCost(GameConfigManager.Instance.BagBuildingList[MergeManager.Instance.GetBuildingBagCapacity(MergeBoardEnum.Main)].CointType,GameConfigManager.Instance.BagBuildingList[MergeManager.Instance.GetBuildingBagCapacity(MergeBoardEnum.Main)].CointCost);
        }
    }
    public void AddBuildBagUnit() // 一次只能解锁一个
    {
        if (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BuildingBagCapacity >= _maxBuildCount)
        {
            //TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text16"), Bagunits[Bagunits.Count - 1].transform);
            return;
        }

        int cost = 0;
        cost = GameConfigManager.Instance.BagBuildingList[MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BuildingBagCapacity].CointCost;
        var type = GameConfigManager.Instance.BagBuildingList[MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BuildingBagCapacity].CointType;
        int needCount;
        if (!UserData.Instance.CanAford((UserData.ResourceId)type, cost,out needCount))
        {
            BuyResourceManager.Instance.TryShowBuyResource((UserData.ResourceId)type, "",
                MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BuildingBagCapacity.ToString(), "diamond_lack_bag",true,needCount);
   
            return;
        }

        var reason = new GameBIManager.ItemChangeReasonArgs
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BuyBag,
            data1 = (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BuildingBagCapacity+1).ToString()
        };
        UserData.Instance.ConsumeRes((UserData.ResourceId)type, cost, reason);
        RefreshBagToken();
        MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BuildingBagCapacity += 1;
        Transform item = AddBuildingPackageItem();
        var script = item.gameObject.GetComponentDefault<MergePackageUnit>();
        script.SetBoardId(MergeBoardEnum.Main);
        script.SetItemInfomation(null, -1, MergePackageUnitType.buildBagUnlock);
        BuildingBagunits.Add(script);
        RefreshBuildingBag();
    }
}