
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using DragonU3DSDK.Storage;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;

public class HappyGoMergeGuideLogic:Manager<HappyGoMergeGuideLogic>
{
    public void CheckMergeGuide(bool isPointerUp = false)
    {
        if (!(SceneFsm.mInstance.GetCurrSceneType() == StatusType.HappyGoGame||SceneFsm.mInstance.GetCurrSceneType() == StatusType.Transition))
            return;
        
        CheckChoseItemGuide();
        CheckTriggerProduct();
        CheckMergeFinish();
        CheckProductFinish();
        CheckHamsterEat();
        ChoseCdProduct();
    }
    private void CheckMergeFinish()
    {
       if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MergeFinish))
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
            return;
        }

        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MergeFinish, "");

        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.MergeItem))
        {
            if (GuideSubSystem.Instance.CurrentConfig.mergeStartIndex > 0 && GuideSubSystem.Instance.CurrentConfig.mergeEndIndex > 0)
            {
                var startGrid = HappyGoMainController.Instance.mMergeBoard.Grids[GuideSubSystem.Instance.CurrentConfig.mergeStartIndex];
                var endGrid = HappyGoMainController.Instance.mMergeBoard.Grids[GuideSubSystem.Instance.CurrentConfig.mergeEndIndex];

                if (startGrid.id == endGrid.id && startGrid.id.ToString() == GuideSubSystem.Instance.GetActionParams())
                {
                    HappyGoMainController.Instance.mMergeBoard.mergeTipList.Clear();
                    HappyGoMainController.Instance.mMergeBoard.CancelTip(-1, true);
                    
                    HappyGoMainController.Instance.mMergeBoard.mergeTipList.Clear();
                    HappyGoMainController.Instance.mMergeBoard.mergeTipList.Add(startGrid);
                    HappyGoMainController.Instance.mMergeBoard.mergeTipList.Add(endGrid);

                    HappyGoMainController.Instance.mMergeBoard.PlayTipsAnimation();
                    
                    
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeItem, startGrid.board.transform as RectTransform,
                        endGrid.board.transform as RectTransform);
                    
                    return;
                }
            }
        }
        
        if (HappyGoMainController.Instance.mMergeBoard.mergeTipList == null ||HappyGoMainController.Instance.mMergeBoard.mergeTipList.Count == 0)
        {
            HappyGoMainController.Instance.mMergeBoard.mergeTipList.Clear();
            HappyGoMainController.Instance.mMergeBoard.AutoTips();
        }

        if (MergeMainController.Instance.MergeBoard.mergeTipList == null ||HappyGoMainController.Instance.mMergeBoard.mergeTipList.Count == 0)
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
            return;
        }

        MergeBoard.Grid grid_1 = null;
        MergeBoard.Grid grid_2 = null;

        if (!GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.MergeItem))
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
            return;
        }

        if (GuideSubSystem.Instance.GetActionParams().IsEmptyString())
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
            return;
        }

        bool isCorrectTips = false;
        if (HappyGoMainController.Instance.mMergeBoard.mergeTipList != null &&HappyGoMainController.Instance.mMergeBoard.mergeTipList.Count > 0)
        {
            int id =HappyGoMainController.Instance.mMergeBoard.mergeTipList[0].id;
            if (GuideSubSystem.Instance.IsActionParams((id).ToString()))
            {
                isCorrectTips = true;
            }
        }

        if (!isCorrectTips)
        {
            int acParams = GuideSubSystem.Instance.GetIntActionParams();
            //优先找任务需要的
            var mergeTipsItem =HappyGoMainController.Instance.mMergeBoard.CalculateAutoTip(false, false, false, acParams);
            for (int i = 0; i < mergeTipsItem.Count; i = i + 2)
            {
                int id = mergeTipsItem[i].id;
                if (GuideSubSystem.Instance.IsActionParams((id).ToString()))
                {
                    grid_1 = mergeTipsItem[i];
                    grid_2 = mergeTipsItem[i + 1];
                    break;
                }
            }

            if (grid_1 != null && grid_2 != null)
            {
                if (grid_1 !=HappyGoMainController.Instance.mMergeBoard.mergeTipList[0] || grid_2 !=HappyGoMainController.Instance.mMergeBoard.mergeTipList[1])
                {
                    for (int i = 0; i <HappyGoMainController.Instance.mMergeBoard.mergeTipList.Count; i++)
                    {
                        HappyGoMainController.Instance.mMergeBoard.mergeTipList[i].board.GetComponent<Animator>().Play("Normal");
                        HappyGoMainController.Instance.mMergeBoard.mergeTipList[i].board.transform.localScale = Vector3.one;
                    }

                    HappyGoMainController.Instance.mMergeBoard.CancelTip(-1, true);
                    
                    HappyGoMainController.Instance.mMergeBoard.mergeTipList.Clear();
                    HappyGoMainController.Instance.mMergeBoard.mergeTipList.Add(grid_1);
                    HappyGoMainController.Instance.mMergeBoard.mergeTipList.Add(grid_2);

                    HappyGoMainController.Instance.mMergeBoard.PlayTipsAnimation();
                }
            }
        }
        else
        {
            grid_1 =HappyGoMainController.Instance.mMergeBoard.mergeTipList[0];
            grid_2 =HappyGoMainController.Instance.mMergeBoard.mergeTipList[1];
        }


        if (grid_1 == null || grid_2 == null)
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
            return;
        }

        bool isSwap = false;
        if (HappyGoMainController.Instance.mMergeBoard.SelectIndex > 0)
        {
            var selectGrid =HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(HappyGoMainController.Instance.mMergeBoard.SelectIndex);
            if (selectGrid != null)
            {
                if (grid_2 == selectGrid)
                    isSwap = true;
            }
        }

        if (grid_1.state == 0) //蛛网交换
            isSwap = true;

        if (isSwap)
        {
            var tempData = grid_1;
            grid_1 = grid_2;
            grid_2 = tempData;
        }

        if (GuideSubSystem.Instance.CurrentConfig.autoSetIndex)
        {
            GuideSubSystem.Instance.CurrentConfig.mergeStartIndex = grid_1.board.index;
            GuideSubSystem.Instance.CurrentConfig.mergeEndIndex = grid_2.board.index;
        }
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeItem, grid_1.board.transform as RectTransform,
            grid_2.board.transform as RectTransform);
    }

    private void CheckHamsterEat()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.HappyGoEat))
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.HappyGoEat);
            return;
        }
        
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.HappyGoEat,"");

        if (!GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.HappyGoEat))
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.HappyGoEat);
            return;
        }
        
        HappyGoMergeBoard.Grid grid_1 = null;
        HappyGoMergeBoard.Grid grid_2 = null;

        var mergeTipsItem = HappyGoMainController.Instance.mMergeBoard.FindHamsterFoods();

        bool isFind = false;
        if (mergeTipsItem != null && mergeTipsItem.Count > 0)
        {
            grid_1 = mergeTipsItem.Random();
            isFind = true;
        }

        grid_2 = HappyGoMainController.Instance.mMergeBoard.HamsterGrid;

        if (!isFind)
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.HappyGoEat);
            return;
        }
        
        if (HappyGoMainController.Instance.mMergeBoard.mergeTipList.Count == 0 || grid_1 != HappyGoMainController.Instance.mMergeBoard.mergeTipList[0] || grid_2 != HappyGoMainController.Instance.mMergeBoard.mergeTipList[1])
        {
            for (int i = 0; i < HappyGoMainController.Instance.mMergeBoard.mergeTipList.Count; i++)
            {
                HappyGoMainController.Instance.mMergeBoard.mergeTipList[i].board.GetComponent<Animator>().Play("Normal");
                HappyGoMainController.Instance.mMergeBoard.mergeTipList[i].board.transform.localScale = Vector3.one;
            }

            HappyGoMainController.Instance.mMergeBoard.mergeTipList.Clear();
            HappyGoMainController.Instance.mMergeBoard.mergeTipList.Add(grid_1);
            HappyGoMainController.Instance.mMergeBoard.mergeTipList.Add(grid_2);

            HappyGoMainController.Instance.mMergeBoard.PlayTipsAnimation();
        }
        else
        {
            grid_1 = HappyGoMainController.Instance.mMergeBoard.mergeTipList[0];
            grid_2 = HappyGoMainController.Instance.mMergeBoard.mergeTipList[1];
        }
        
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.HappyGoEat,grid_1.board.transform as RectTransform ,grid_2.board.transform as RectTransform);
    }
      
    private int guideGridIndex = -1;
    public void CheckProductFinish()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ProductFinish))
            return;

        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.ProductFinish);
        if (actionParams == null || actionParams.Count == 0)
            return;

        for (int i = 0; i < actionParams.Count; i++)
        {
            if (int.TryParse(actionParams[i], out var value))
            {
                if(value >= 900000 && value <= 990009)
                    continue;
            }
            
            actionParams.RemoveAt(i);
            i--;
        }
        if (actionParams == null || actionParams.Count == 0)
            return;
            
        int gridId = -1;
        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ProductBuild))
        {
            string param = GuideSubSystem.Instance.GetActionParams();
            if (param != null)
                gridId = int.Parse(param);
            else
            {
                gridId = int.Parse(actionParams[0]);
            }
        }
        else
        {
            gridId = int.Parse(actionParams[0]);
        }

        List<HappyGoMergeBoard.Grid> grids =HappyGoMainController.Instance.mMergeBoard.GetGridsById(gridId);
        if (grids == null || grids.Count == 0)
            return;

        int emptyIndex = MergeManager.Instance.FindEmptyGrid(1,MergeBoardEnum.HappyGo);
        if(emptyIndex < 0)
            return;
        
        foreach (var grid in grids)
        {
            if (grid.board == null)
                continue;

            if (grid.board.IsInProductCD())
                continue;

            TriggerProductFinish(grid);
            return;
        }

        TriggerProductFinish(grids[0]);
    }

    private void TriggerProductFinish(HappyGoMergeBoard.Grid grid)
    {
        int index = grid.board.index;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(grid.board.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ProductBuild, grid.board.transform as RectTransform, topLayer:topLayer);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ProductFinish, "");

        if (guideGridIndex < 0)
            guideGridIndex = index;
        else if (guideGridIndex != index)
        {
            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ProductBuild))
            {
                GuideSubSystem.Instance.TriggerCurrent();
            }

            guideGridIndex = index;
        }

        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ProductBuild))
        {
            grid.board.SetTapImageActive(true);
        }
    }

    public void CheckChoseItemGuide()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ChoseItem))
            return;
        
        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.ChoseItem);
        if(actionParams == null || actionParams.Count == 0)
            return;
        
        for (int i = 0; i < HappyGoMainController.Instance.mMergeBoard.Grids.Length; i++)
        {
            if (HappyGoMainController.Instance.mMergeBoard.Grids[i].id < 0 || HappyGoMainController.Instance.mMergeBoard.Grids[i].board == null)
                continue;
    
            if (HappyGoMainController.Instance.mMergeBoard.Grids[i].state != 1 && HappyGoMainController.Instance.mMergeBoard.Grids[i].state != 3)
                continue;
    
    
            if(!actionParams.Contains(HappyGoMainController.Instance.mMergeBoard.Grids[i].id.ToString()))
                continue;
            
            
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(HappyGoMainController.Instance.mMergeBoard.Grids[i].board.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ChoseItem,HappyGoMainController.Instance.mMergeBoard.Grids[i].board.transform as RectTransform, topLayer:topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ChoseItem, HappyGoMainController.Instance.mMergeBoard.Grids[i].id.ToString()))
            {
                if (GuideSubSystem.Instance.CurrentConfig.autoSetIndex)
                {
                    GuideSubSystem.Instance.CurrentConfig.mergeStartIndex = HappyGoMainController.Instance.mMergeBoard.Grids[i].board.index;
                    GuideSubSystem.Instance.CurrentConfig.mergeEndIndex = HappyGoMainController.Instance.mMergeBoard.Grids[i].board.index;
                }
            }
            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseItem))
            {
                GuideSubSystem.Instance.TriggerCurrent();
            }
            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseItem) && GuideSubSystem.Instance.AutoChoose())
            {
                HappyGoMainController.Instance.mMergeBoard.SelectFocus(HappyGoMainController.Instance.mMergeBoard.GetGridIndex(HappyGoMainController.Instance.mMergeBoard.Grids[i]), false, true, false);
            }
            return;
        }
    }

    public void CheckTriggerProduct()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ProductFinish))
            return;
        
        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.ProductFinish);
        if(actionParams == null || actionParams.Count == 0)
            return;

        for (int i = 0; i < actionParams.Count; i++)
        {
            if (int.TryParse(actionParams[i], out var value))
            {
                if(value >= 900000 && value <= 990009)
                    continue;
            }
            
            actionParams.RemoveAt(i);
            i--;
        }
        if(actionParams == null || actionParams.Count == 0)
            return;
        
        int id = int.Parse(actionParams[0]);
        TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(id);
        if(mergeItem == null)
            return;
        
        HappyGoMergeBoard.Grid grid = HappyGoMainController.Instance.mMergeBoard.GetGridById(id);
        if(grid == null)
            return;
        
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ProductBuild, grid.board.transform as RectTransform);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ProductFinish, null);
    }
    
     private MergeBoard.Grid cdGridItem = null;
    public void ChoseCdProduct()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.HappyGoCdProduct))
            return;

        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.HappyGoCdProduct);
        if(actionParams == null || actionParams.Count == 0)
            return;
        
        if (cdGridItem != null && cdGridItem.board != null && cdGridItem.board.IsInProductCD())
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.HappyGoCdProduct, null);
            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.HappyGoCdProduct))
            {
                GuideSubSystem.Instance.TriggerCurrent();
            }

            return;
        }

        List< HappyGoMergeBoard.Grid> inCdGrids = new List<HappyGoMergeBoard.Grid>();
        for (int i = 0; i <HappyGoMainController.Instance.mMergeBoard.Grids.Length; i++)
        {
            if(HappyGoMainController.Instance.mMergeBoard.Grids[i].id != 920001)
                continue;

            if (HappyGoMainController.Instance.mMergeBoard.Grids[i].board.storageMergeItem.State != 3)
                continue;

            inCdGrids.Add(HappyGoMainController.Instance.mMergeBoard.Grids[i]);
            break;
        }

        if (inCdGrids.Count == 0)
            return;

        foreach (MergeBoard.Grid grid in inCdGrids)
        {
            if(grid.board == null || grid.board.tableMergeItem == null)
                continue;
            
            if(!actionParams.Contains(grid.board.id.ToString()))
                continue;
            cdGridItem = grid;
            break;
        }

        if (cdGridItem == null)
            return;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(cdGridItem.board.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.HappyGoCdProduct,
            cdGridItem.board.transform as RectTransform,topLayer:topLayer);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.HappyGoCdProduct, null))
        {
            GuideSubSystem.Instance.CurrentConfig.mergeStartIndex = cdGridItem.board.index;
            GuideSubSystem.Instance.CurrentConfig.mergeEndIndex = cdGridItem.board.index;
        }
    }
}
