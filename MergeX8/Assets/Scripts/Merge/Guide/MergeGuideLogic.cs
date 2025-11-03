using System.Collections.Generic;
using Activity.JumpGrid;
using Activity.TurtlePang.View;
using ActivityLocal.ClimbTower.Model;
using Cysharp.Threading.Tasks;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonU3DSDK.Storage;
using Dynamic;
using Framework;
using Gameplay;
using Gameplay.UI.EnergyTorrent;
using Merge.Order;
using Scripts.UI;
using SomeWhere;
using ThemeDecorationLeaderBoard;
using UnityEngine;

public class MergeGuideLogic : Manager<MergeGuideLogic>
{
    public void CheckMergeGuide(bool isPointerUp = false)
    {
        if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
            return;

        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnterMerge, "");

        CheckTriggerProduct();
        CheckProduct();
        CheckMergeFinish();
        CheckProductFinish();
        //CheckTaskNeedItem();
        CheckTaskFinish();
        CheckGetReward();
        ChoseCdProduct();
        CheckChoseItemGuide();
        if (!isPointerUp)
            CheckUnLockModule();
        CheckUnLockDecoTask();
        CheckSpecialDecoTask();
        CheckSpecialSealDecoTask();
        ProductBuildLogic();
        PowerLogic();
        EnergyTorrentLogic();
        CheckMermaidLogic();
        CheckDogHopeLogic();
        CheckParrotLogic();
        CheckFlowerFieldLogic();
        CheckClimbTreeLogic();
        CheckRecoverCoinLogic();
        CheckCoinLeaderBoardLogic();
        CheckSummerWatermelonLogic();
        CheckJungleAdventureLogic();
        CheckPhotoAlbumLogic();
        CheckSummerWatermelonBreadLogic();
        CheckCoinCompetitionLogic();
        CheckJumpGridLogic();
        CheckMergePackageGuide();
        CheckEatFood();
        ChoseFoodCdProduct();
        CheckEaster2024Guide();
        CheckSnakeLadderGuide();
        CheckThemeDecorationGuide();
        CheckMonopolyGuide();
        CheckKeepPetGuide();
        CheckTeamTaskGuide();
        CheckPillowWheelGuide();
        //最后处理
        CheckCardCollectionGuide();
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
                var startGrid = MergeMainController.Instance.MergeBoard.Grids[GuideSubSystem.Instance.CurrentConfig.mergeStartIndex];
                var endGrid = MergeMainController.Instance.MergeBoard.Grids[GuideSubSystem.Instance.CurrentConfig.mergeEndIndex];

                if (startGrid.id == endGrid.id && startGrid.id.ToString() == GuideSubSystem.Instance.GetActionParams())
                {
                    MergeMainController.Instance.MergeBoard.mergeTipList.Clear();
                    MergeMainController.Instance.MergeBoard.CancelTip(-1, true);
                    
                    MergeMainController.Instance.MergeBoard.mergeTipList.Clear();
                    MergeMainController.Instance.MergeBoard.mergeTipList.Add(startGrid);
                    MergeMainController.Instance.MergeBoard.mergeTipList.Add(endGrid);

                    MergeMainController.Instance.MergeBoard.PlayTipsAnimation();
                    
                    
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeItem, startGrid.board.transform as RectTransform,
                        endGrid.board.transform as RectTransform);
                    
                    return;
                }
            }
        }
        
        if (MergeMainController.Instance.MergeBoard.mergeTipList == null ||MergeMainController.Instance.MergeBoard.mergeTipList.Count == 0)
        {
            MergeMainController.Instance.MergeBoard.mergeTipList.Clear();
            MergeMainController.Instance.MergeBoard.AutoTips();
        }

        if (MergeMainController.Instance.MergeBoard.mergeTipList == null ||MergeMainController.Instance.MergeBoard.mergeTipList.Count == 0)
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
        if (MergeMainController.Instance.MergeBoard.mergeTipList != null &&MergeMainController.Instance.MergeBoard.mergeTipList.Count > 0)
        {
            int id =MergeMainController.Instance.MergeBoard.mergeTipList[0].id;
            if (GuideSubSystem.Instance.IsActionParams((id).ToString()))
            {
                isCorrectTips = true;
            }
        }

        if (!isCorrectTips)
        {
            int acParams = GuideSubSystem.Instance.GetIntActionParams();
            //优先找任务需要的
            var mergeTipsItem =MergeMainController.Instance.MergeBoard.CalculateAutoTip(false, false, false, acParams);
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
                if (grid_1 !=MergeMainController.Instance.MergeBoard.mergeTipList[0] || grid_2 !=MergeMainController.Instance.MergeBoard.mergeTipList[1])
                {
                    for (int i = 0; i <MergeMainController.Instance.MergeBoard.mergeTipList.Count; i++)
                    {
                        MergeMainController.Instance.MergeBoard.mergeTipList[i].board.GetComponent<Animator>().Play("Normal");
                        MergeMainController.Instance.MergeBoard.mergeTipList[i].board.transform.localScale = Vector3.one;
                    }

                    MergeMainController.Instance.MergeBoard.CancelTip(-1, true);
                    
                    MergeMainController.Instance.MergeBoard.mergeTipList.Clear();
                    MergeMainController.Instance.MergeBoard.mergeTipList.Add(grid_1);
                    MergeMainController.Instance.MergeBoard.mergeTipList.Add(grid_2);

                    MergeMainController.Instance.MergeBoard.PlayTipsAnimation();
                }
            }
        }
        else
        {
            grid_1 =MergeMainController.Instance.MergeBoard.mergeTipList[0];
            grid_2 =MergeMainController.Instance.MergeBoard.mergeTipList[1];
        }


        if (grid_1 == null || grid_2 == null)
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.MergeItem);
            return;
        }

        bool isSwap = false;
        if (MergeMainController.Instance.MergeBoard.SelectIndex > 0)
        {
            var selectGrid =MergeMainController.Instance.MergeBoard.GetGridByIndex(MergeMainController.Instance.MergeBoard.SelectIndex);
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

    private int guideGridIndex = -1;

    public void CheckProductFinish()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ProductFinish))
            return;

        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.ProductFinish);
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

        List<MergeBoard.Grid> grids =MergeMainController.Instance.MergeBoard.GetGridsById(gridId);
        if (grids == null || grids.Count == 0)
            return;

        int emptyIndex = MergeManager.Instance.FindEmptyGrid(1,MergeBoardEnum.Main);
        if(emptyIndex < 0)
            return;
        
        foreach (var grid in grids)
        {
            if (grid.board == null)
                continue;

            if (grid.board.IsInProductCD())
                continue;

            if (grid.board.tableMergeItem != null && grid.board.tableMergeItem.type == (int)MergeItemType.eatBuild && grid.board.storageMergeItem.State != 1)
                continue;
          
            TriggerProductFinish(grid);
            return;
        }

        if (grids[0].board.tableMergeItem != null && grids[0].board.tableMergeItem.type == (int)MergeItemType.eatBuild && grids[0].board.storageMergeItem.State != 1)
            return;
        
        TriggerProductFinish(grids[0]);
    }

    private void TriggerProductFinish(MergeBoard.Grid grid)
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

    private void CheckTaskFinish()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TaskFinish))
            return;

        GameObject guideObj = MergeTaskTipsController.Instance.GetFirstCompleteTaskGuide();
        string id = MergeTaskTipsController.Instance.FirstTaskCompleteId();
        if (guideObj != null)
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(guideObj.transform.parent);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Task, guideObj.transform as RectTransform, topLayer:topLayer, targetParam:id);
        }

        if (MergeTaskTipsController.Instance.FirstTaskComplete() && MainOrderManager.Instance.CurMergeTask == null)
        {
            List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.TaskFinish);
            if (actionParams == null || actionParams.Count == 0)
                return;

            foreach (var actionParam in actionParams)
            {
                if (!actionParam.Equals(id))
                    continue;
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TaskFinish, id, id))
                {
                    // GuideSubSystem.Instance.InNewPlayerGuideChain = true;   
                }
                break;
            }
        }
    }

    public void CheckChoseItemGuide()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ChoseItem))
            return;

        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.ChoseItem);
        if (actionParams == null || actionParams.Count == 0)
            return;

        for (int i = 0; i <MergeMainController.Instance.MergeBoard.Grids.Length; i++)
        {
            if (MergeMainController.Instance.MergeBoard.Grids[i].id < 0 ||MergeMainController.Instance.MergeBoard.Grids[i].board == null)
                continue;

            if (MergeMainController.Instance.MergeBoard.Grids[i].state != 1 &&MergeMainController.Instance.MergeBoard.Grids[i].state != 3)
                continue;

            foreach (var param in actionParams)
            {
                if (!param.Equals(MergeMainController.Instance.MergeBoard.Grids[i].id.ToString()))
                    continue;

                if (!GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ChoseItem, param))
                    continue;
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(MergeMainController.Instance.MergeBoard.Grids[i].board.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ChoseItem,
                    MergeMainController.Instance.MergeBoard.Grids[i].board.transform as RectTransform, topLayer:topLayer);
                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseItem))
                {
                    GuideSubSystem.Instance.TriggerCurrent();
                }

                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseItem) &&
                    GuideSubSystem.Instance.AutoChoose())
                {
                    MergeMainController.Instance.MergeBoard.SelectFocus(MergeMainController.Instance.MergeBoard.GetGridIndex(MergeMainController.Instance.MergeBoard.Grids[i]),
                        false, true, false);
                }

                return;
            }
        }
    }

    public void CheckGetReward()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.GetReward))
            return;

        if (!MergeMainController.Instance.RewardItemIsShow())
            return;

        if (MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main) == 0)
            return;

        int emptyIndex = MergeManager.Instance.FindEmptyGrid(1,MergeBoardEnum.Main);
        if (emptyIndex == -1)
            return;

        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.GetReward);
        if (actionParams == null || actionParams.Count == 0)
            return;

        if (MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main) == 0)
            return;

        int emptyCount = MergeManager.Instance.GetEmptBoardItemCount(MergeBoardEnum.Main);

        for (int j = 0; j < actionParams.Count; j++)
        {
            string param = actionParams[j];

            if (param.Equals("700001"))
            {
                if(!StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey($"success{2}"))
                    continue;
            }
            
            for (int i = 0; i < MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main); i++)
            {
                StorageMergeItem mergeItem = MergeManager.Instance.GetRewardItem(i,MergeBoardEnum.Main);
                if (!param.Equals(mergeItem.Id.ToString()))
                    continue;

                if (emptyCount < i + 1)
                    continue;

                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GetReward, param))
                {
                    if(MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(0, 0);
                    return;
                }
            }
        }
    }

    // private void CheckTaskNeedItem()
    // {
    //     if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ClickTaskNeedItem))
    //         return;
    //
    //     if (MergeTaskTipsController.Instance == null)
    //         return;
    //     if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClickTaskNeedItem,
    //             MergeTaskTipsController.Instance.GetFirstTaskNeedItemId().ToString()))
    //     {
    //         // GuideSubSystem.Instance.InNewPlayerGuideChain = false;   
    //     }
    //
    // }

    private MergeBoard.Grid cdGridItem = null;

    public void ChoseCdProduct()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ChoseCdProduct))
            return;

        if (cdGridItem != null && cdGridItem.board != null && cdGridItem.board.IsInProductCD())
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ChoseCdProduct, null);
            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseCdProduct))
            {
                GuideSubSystem.Instance.TriggerCurrent();
            }

            return;
        }

        List<MergeBoard.Grid> inCdGrids = new List<MergeBoard.Grid>();
        for (int i = 0; i <MergeMainController.Instance.MergeBoard.Grids.Length; i++)
        {
            if (MergeMainController.Instance.MergeBoard.Grids[i].id < 0 ||MergeMainController.Instance.MergeBoard.Grids[i].board == null)
                continue;

            if (!MergeMainController.Instance.MergeBoard.Grids[i].board.IsInProductCD())
                continue;

            inCdGrids.Add(MergeMainController.Instance.MergeBoard.Grids[i]);
        }

        if (inCdGrids.Count == 0)
            return;

        int cost = int.MaxValue;
        foreach (MergeBoard.Grid grid in inCdGrids)
        {
            if(grid.board == null || grid.board.tableMergeItem == null)
                continue;
            
            if(grid.board.tableMergeItem.type == (int)MergeItemType.box)
                continue;
            
            if (!MergeConfigManager.Instance.IsProductItem(grid.board.tableMergeItem))
                continue;
            
            int[] cdCost = grid.board.tableMergeItem.cdspeed_cost;
            if (cdCost == null || cdCost.Length < 2)
                continue;

            if (cost < cdCost[1])
                continue;

            cost = cdCost[1];
            cdGridItem = grid;
        }

        if (cdGridItem == null)
            return;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(cdGridItem.board.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ChoseCdProduct,
            cdGridItem.board.transform as RectTransform,topLayer:topLayer);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ChoseCdProduct, null);
    }

    public void CheckTriggerProduct()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ProductFinish))
            return;

        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.ProductFinish);
        if (actionParams == null || actionParams.Count == 0)
            return;
        
        MergeBoard.Grid grid =MergeMainController.Instance.MergeBoard.GetGridById(int.Parse(actionParams[0]));
        if (grid == null)
            return;

        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(grid.board.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ProductBuild, grid.board.transform as RectTransform, topLayer:topLayer);

        string param = "task_" + MainOrderManager.Instance.CompleteTaskNum;
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ProductFinish, param);
    }

    private Coroutine _coroutineProduct;
    private void CheckProduct()
    {
        if(MainOrderManager.Instance.CompleteTaskNum < 2 || MainOrderManager.Instance.CompleteTaskNum >= 5)
            return;

        if(_coroutineProduct != null)
            CoroutineManager.Instance.StopCoroutine(_coroutineProduct);
        
        _coroutineProduct = CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(2f, () =>
        {
            _coroutineProduct = null;
            if (MergeMainController.Instance.MergeBoard.mergeTipList.Count >= 2)
            {
                MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.None, MergeBoardEnum.Main,true);
                return;
            }
        
            if(SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                MergeResourceManager.Instance.GetMergeResource(100004,MergeBoardEnum.Main, true);
        }));
    }
    
    public void CheckUnLockModule()
    {
        CheckUnLockStore();
        CheckUnLockDecoTask();
        CheckSpecialDecoTask();
        CheckSpecialSealDecoTask();
    }

    private void CheckUnLockStore()
    {
        // int itemId = 0;
        // bool show = StoreModel.Instance.IsCanBuyFreeStoreItem(out itemId);
        // if (!show)
        //     return;
        //
        // if(MainOrderManager.Instance.CompleteTaskNum < GlobalConfigManager.Instance.GetNumValue("StoreGuideTaskNum"))
        //     return;
        //     
        // if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ClickStore))
        //     return;

        bool isCanEnter = ClimbTowerModel.Instance.IsCanEnter();
        if(!isCanEnter)
            return;
        
        if (!UnLockCheckUI())
            return;
        if (!GuideSubSystem.Instance.IsShowingGuide())
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClickStore, null);
    }

    public bool CheckUnLockDecoTask()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.UnlockDeco))
            return false;

        //string param = "task_" + TaskModuleManager.Instance.CompleteTaskNum;
        if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance._mergeDailyTaskItem.gameObject.activeSelf)
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.UnlockDeco, "");
            return true;
        }

        return false;
    }
    
    public bool CheckSpecialDecoTask()
    {
        if (DecoWorld.NodeLib.ContainsKey(101011))
        {
            if(DecoWorld.NodeLib[101011].IsFinish)
                return false;
        }
        
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.UnlockDeco, "103"))
            return false;
        
        if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance._mergeDailyTaskItem.gameObject.activeSelf)
        {
            var areaNodes = DecoManager.Instance.CurrentWorld.GetUnlockAndNotOwnedNodes();
            if (areaNodes != null && areaNodes.Count > 0)
            {
                foreach (var area in areaNodes)
                {
                    for(int i = 0; i < area.Value.Count; i++)
                    {
                        if(area.Value[i]._data._config.costId != 103)
                            continue;
                        
                        if (!area.Value[i].IsOwned && !UserData.Instance.CanAford((UserData.ResourceId)area.Value[i]._data._config.costId, area.Value[i]._data._config.price))
                            continue;

                        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.UnlockDeco, "103"))
                        {
                            if(MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                                MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(0, 0);
                        }
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    
    public bool CheckSpecialSealDecoTask()
    {
        if (DecoWorld.NodeLib.ContainsKey(101099))
        {
            if(DecoWorld.NodeLib[101099].IsFinish)
                return false;
        }
        
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.UnlockDeco, "900"))
            return false;
        
        if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance._mergeDailyTaskItem.gameObject.activeSelf)
        {
            var areaNodes = DecoManager.Instance.CurrentWorld.GetUnlockAndNotOwnedNodes();
            if (areaNodes != null && areaNodes.Count > 0)
            {
                foreach (var area in areaNodes)
                {
                    for(int i = 0; i < area.Value.Count; i++)
                    {
                        if(area.Value[i]._data._config.costId != 900)
                            continue;
                        
                        if (!area.Value[i].IsOwned && !UserData.Instance.CanAford((UserData.ResourceId)area.Value[i]._data._config.costId, area.Value[i]._data._config.price))
                            continue;

                        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.UnlockDeco, "900"))
                        {
                            if(MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                                MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(0, 0);
                        }
            
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private bool UnLockCheckUI()
    {
        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMysteryGift))
            return false;

        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIStoreGame))
            return false;

        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupLuckyBalloon))
            return false;

        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupReward))
            return false;

        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergePackage))
            return false;

        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupDailyChallenge))
            return false;

        return true;
    }

    private void ProductBuildLogic()
    {
        // if (TaskModuleManager.Instance.CompleteTaskNum < 5 || TaskModuleManager.Instance.CompleteTaskNum >= 9)
        //     return;
        //
        // MergeBoard.Grid grid =MergeMainController.Instance.MergeBoard.GetGridById(productId);
        // if (grid == null)
        //     return;
        //
        // if (grid.board == null)
        //     return;
        //
        // if (grid.board.IsInProductCD())
        //     return;
        //
        // grid.board.SetTapImageActive(true);
    }

    private void PowerLogic()
    {
        // if (ExperenceModel.Instance.GetLevel() > 2)
        //     return;
        //
        // MergeBoard.Grid grid =MergeMainController.Instance.MergeBoard.GetGridById(10303);
        // if (grid == null)
        //     return;
        //
        // if (grid.board == null)
        //     return;
        //
        // grid.board.SetTapImageActive(true);
    }

    private void EnergyTorrentLogic()
    {
        if(!EnergyTorrentModel.Instance.IsShowStart())
            return;
        
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.EnergyTorrent4))
            return;
        
        if(GuideSubSystem.Instance.IsShowingGuide())
            return;
        if (EnergyTorrentModel.Instance.IsUnlock4Multiply())
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnergyTorrent4, null);
        }
        else
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnergyTorrent1, null);
        }
    }

    private void CheckRecoverCoinLogic()
    {
        if (GuideSubSystem.Instance.InGuideChain)
        {
            if (RecoverCoinModel.InGuideChain)
            {
                GuideSubSystem.Instance.InGuideChain = false;
                RecoverCoinModel.InGuideChain = false;
            }
            else
            {
                return;
            }
        }
        
        if(!RecoverCoinModel.Instance.IsStart())
            return;

        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.RecoverCoinTask))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.RecoverCoinTask, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.RecoverCoinTask, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
       
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.RecoverCoinInfo))
        {
            MergeRecoverCoin tipsItem = MergeTaskTipsController.Instance._mergeRecoverCoin;
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.RecoverCoinInfo, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.RecoverCoinInfo, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
        
    }
    private void CheckCoinLeaderBoardLogic()
    {
        if (GuideSubSystem.Instance.InGuideChain)
        {
            if (CoinLeaderBoardModel.InGuideChain)
            {
                GuideSubSystem.Instance.InGuideChain = false;
                CoinLeaderBoardModel.InGuideChain = false;
            }
            else
            {
                return;
            }
        }
        
        if(!CoinLeaderBoardModel.Instance.IsStart())
            return;

        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CoinLeaderBoardTask))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CoinLeaderBoardTask, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CoinLeaderBoardTask, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
       
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CoinLeaderBoardInfo))
        {
            MergeCoinLeaderBoard tipsItem = MergeTaskTipsController.Instance._mergeCoinLeaderBoard;
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CoinLeaderBoardInfo, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CoinLeaderBoardInfo, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
        
    }
    private void CheckSummerWatermelonLogic()
    {
        if (GuideSubSystem.Instance.InGuideChain)
        {
            if (SummerWatermelonModel.InGuideChain)
            {
                GuideSubSystem.Instance.InGuideChain = false;
                SummerWatermelonModel.InGuideChain = false;
            }
            else
            {
                return;
            }
        }
        if (!SummerWatermelonModel.Instance.IsStart)
        {
            return;
        }
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SummerWatermelonGameEntrance))
        {
            MergeSummerWatermelon tipsItem = MergeTaskTipsController.Instance._mergeSummerWatermelon;
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SummerWatermelonGameEntrance, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SummerWatermelonGameEntrance, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
    }
    
    private async UniTask CheckJungleAdventureLogic()
    {
        if (GuideSubSystem.Instance.InGuideChain)
            return;
        
        if (!JungleAdventureModel.Instance.IsOpened())
            return;
        
        if (!JungleAdventureModel.Instance.IsPreheatEnd())
            return;

        await UniTask.WaitForSeconds(0.5f);
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.JungleAdventureGame))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JungleAdventureGame,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JungleAdventureGame, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    return;
                }
            }
        }
    }
    private void CheckPhotoAlbumLogic()
    {
        if (GuideSubSystem.Instance.InGuideChain)
            return;
        if (!PhotoAlbumModel.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.PhotoAlbumGame))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.PhotoAlbumGame,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.PhotoAlbumGame, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    return;
                }
            }
        }
    }
    
    private void CheckSummerWatermelonBreadLogic()
    {
        if (GuideSubSystem.Instance.InGuideChain)
        {
            if (SummerWatermelonBreadModel.InGuideChain)
            {
                GuideSubSystem.Instance.InGuideChain = false;
                SummerWatermelonBreadModel.InGuideChain = false;
            }
            else
            {
                return;
            }
        }
        if (!SummerWatermelonBreadModel.Instance.IsStart)
        {
            return;
        }
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SummerWatermelonBreadGameEntrance))
        {
            MergeSummerWatermelonBread tipsItem = MergeTaskTipsController.Instance._mergeSummerWatermelonBread;
            if (tipsItem)
            {
             GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SummerWatermelonBreadGameEntrance, tipsItem.transform as RectTransform);
             if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SummerWatermelonBreadGameEntrance, null))
             {
                 if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                 {
                     MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                 }
                 return;
             }   
            }
        }
    }
    
    private void CheckMermaidLogic()
    {
        if(!MermaidModel.Instance.IsOpened())
            return;

        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MermaidTask))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MermaidTask, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MermaidTask, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
       
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MermaidInfo))
        {
            MergeMermaid tipsItem = MergeTaskTipsController.Instance._MergeMermaid;
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MermaidInfo, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MermaidInfo, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
        
    }

    public void CheckDogHopeLogic()
    {
        if(!DogHopeModel.Instance.IsOpenActivity())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.DogHopeLeaderBoardMergeEntrance) &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            DogHopeModel.Instance.CurStorageDogHopeWeek.LeaderBoardStorage.IsInitFromServer())
        {
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.DogHopeLeaderBoardMergeEntrance, MergeTaskTipsController.Instance._mergeDogHope.transform as RectTransform);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.DogHopeLeaderBoardMergeEntrance, null))
            {
                GuideSubSystem.Instance.ForceFinished(731);
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance._mergeDogHope.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
    }
    public void CheckParrotLogic()
    {
        if(!ParrotModel.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ParrotGameDes))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ParrotGameDes,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ParrotGameDes, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    return;
                }
            }
        }
    }

    public void CheckFlowerFieldLogic()
    {
        if(!FlowerFieldModel.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.FlowerFieldGameDes))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.FlowerFieldGameDes,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.FlowerFieldGameDes, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    return;
                }
            }
        }
    }
    public void CheckClimbTreeLogic()
    {
        if(!ClimbTreeModel.Instance.IsPrivateOpened())
            return;
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ClimbTreeGameDes) && MergeTaskTipsController.Instance._mergeClimbTree)
        {
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreeGameDes, MergeTaskTipsController.Instance._mergeClimbTree.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreeGameDes, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance._mergeClimbTree.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ClimbTreeLeaderBoardMergeEntrance) &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            ClimbTreeModel.Instance.CurStorageClimbTreeWeek.LeaderBoardStorage.IsInitFromServer())
        {
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreeLeaderBoardMergeEntrance, MergeTaskTipsController.Instance._mergeClimbTree.transform as RectTransform);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreeLeaderBoardMergeEntrance, null))
            {
                GuideSubSystem.Instance.ForceFinished(734);
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance._mergeClimbTree.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
    }
    
    private void CheckCoinCompetitionLogic()
    {
        if(!CoinCompetitionModel.Instance.IsOpened())
            return;
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CoinCompetitionTask) && 
            MergeTaskTipsController.Instance.mergeCoinCompetition)
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(MergeTaskTipsController.Instance.mergeCoinCompetition.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CoinCompetitionTask, MergeTaskTipsController.Instance.mergeCoinCompetition.transform  as RectTransform, topLayer: topLayer);

            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CoinCompetitionTask, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance.mergeCoinCompetition.transform.localPosition.x+220, 0);
                }
            }
        }
    }

    private void CheckJumpGridLogic()
    {
        if(!JumpGridModel.Instance.IsOpened())
            return;
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.JumpGridTask) && 
            MergeTaskTipsController.Instance._MergeJumpGrid)
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(MergeTaskTipsController.Instance._MergeJumpGrid.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JumpGridTask, MergeTaskTipsController.Instance._MergeJumpGrid.transform  as RectTransform, topLayer: topLayer);

            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JumpGridTask, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-MergeTaskTipsController.Instance._MergeJumpGrid.transform.localPosition.x+220, 0);
                }
            }
        }
    }
    
    public void CheckMergePackageGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MergeBoardFullGuideBag) &&
            ExperenceModel.Instance.GetLevel() >= 3 &&
            MergeManager.Instance.MergeBoardIsFull(MergeBoardEnum.Main) &&
            !MergeManager.Instance.IsBagFull(MergeBoardEnum.Main))
        {
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeBoardFullGuideBag,MergeMainController.Instance._mBagBtn.transform as RectTransform);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MergeBoardFullGuideBag,null);
            return;
        }

        {
            var buildingChipCount = 0;
            var items = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Items;
            for (var i=0;i<items.Count;i++)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(items[i].Id);
                if (itemConfig != null && (items[i].State == 1 || items[i].State == 3) &&
                    itemConfig.isBuildingBag &&
                    (itemConfig.output.Length == 0 || itemConfig.output[0] == 0))
                {
                    buildingChipCount++;
                }
            }
            if (buildingChipCount >=4 && 
                ExperenceModel.Instance.GetLevel() >= 5 && 
                MergeManager.Instance.GetBuildingBagCount(MergeBoardEnum.Main) == 0 && 
                MergeManager.LeftEmptyGridCount(MergeBoardEnum.Main)<1)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeBoardGuideBag,MergeMainController.Instance._mBagBtn.transform as RectTransform);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MergeBoardGuideBag,null);
            }   
        }
    }

    public void CheckCardCollectionGuide()
    {
        // if (!(GuideSubSystem.Instance.InGuideChain && CardCollectionModel.Instance.InGuideChain))
        // {
        //     return;
        // }
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CardPackageEntrance))
        {
            var cardPackageIcon = MergeTaskTipsController.Instance.MergeCardPackage;
            if (cardPackageIcon == null || !cardPackageIcon.gameObject.activeInHierarchy)
                return;
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(cardPackageIcon.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardPackageEntrance, cardPackageIcon.transform as RectTransform, topLayer:topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardPackageEntrance,null))
            {
                if(MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-cardPackageIcon.transform.localPosition.x+220, 0);
            }
            return;
        }
    }
    
    private void CheckEatFood()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.EatFood))
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.EatFood);
            return;
        }

        if (!MainOrderManager.Instance.IsCompleteOrder(50000001))
            return;
        
        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.EatFood);
        if (actionParams == null || actionParams.Count == 0)
            return;

        List<int> foodIds = new List<int>();
        
        foreach (var actionParam in actionParams)
        {
            int foodId = int.Parse(actionParam);
            if(!MergeMainController.Instance.MergeBoard.CanFeedFood(700001, foodId))
                continue;
         
            foodIds.Add(foodId);
        }

        if (foodIds.Count == 0)
        {
            GuideSubSystem.Instance.ClearTarget(GuideTargetType.EatFood);
            return;
        }
        
        List<MergeBoard.Grid> feeds =MergeMainController.Instance.MergeBoard.GetGridsById(700001);
        if (feeds == null || feeds.Count == 0)
            return;
        
        foreach (var foodId in foodIds)
        {
            List<MergeBoard.Grid> foods =MergeMainController.Instance.MergeBoard.GetGridsById(foodId);
            if (foods == null || foods.Count == 0)
                continue;
            
            HappyGoMergeBoard.Grid grid_1 = null;
            HappyGoMergeBoard.Grid grid_2 = null;
            
            
            grid_1 = foods.Random();
            grid_2 = feeds.Random();
            
            if (MergeMainController.Instance.MergeBoard.mergeTipList.Count == 0 || grid_1 != MergeMainController.Instance.MergeBoard.mergeTipList[0] || grid_2 != MergeMainController.Instance.MergeBoard.mergeTipList[1])
            {
                for (int i = 0; i < MergeMainController.Instance.MergeBoard.mergeTipList.Count; i++)
                {
                    MergeMainController.Instance.MergeBoard.mergeTipList[i].board.GetComponent<Animator>().Play("Normal");
                    MergeMainController.Instance.MergeBoard.mergeTipList[i].board.transform.localScale = Vector3.one;
                }

                MergeMainController.Instance.MergeBoard.mergeTipList.Clear();
                MergeMainController.Instance.MergeBoard.mergeTipList.Add(grid_1);
                MergeMainController.Instance.MergeBoard.mergeTipList.Add(grid_2);

                MergeMainController.Instance.MergeBoard.PlayTipsAnimation();
            }
            else
            {
                grid_1 = MergeMainController.Instance.MergeBoard.mergeTipList[0];
                grid_2 = MergeMainController.Instance.MergeBoard.mergeTipList[1];
            }
        
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.EatFood,grid_1.board.transform as RectTransform ,
                grid_2.board.transform as RectTransform, targetParam:foodId.ToString());

            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EatFood, foodId.ToString(), foodId.ToString());
            
            if (!GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.EatFood))
            {
                GuideSubSystem.Instance.ClearTarget(GuideTargetType.EatFood);
                return;
            }
            return;
        }
        
        GuideSubSystem.Instance.ClearTarget(GuideTargetType.EatFood);
    }
    
    
     private MergeBoard.Grid cdFoodGridItem = null;
    public void ChoseFoodCdProduct()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ChoseEatCdItem))
            return;

        List<string> actionParams = GuideSubSystem.Instance.GetActionParams(GuideTriggerPosition.ChoseEatCdItem);
        if(actionParams == null || actionParams.Count == 0)
            return;
        
        if (cdFoodGridItem != null && cdFoodGridItem.board != null && cdFoodGridItem.board.IsInProductCD())
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ChoseEatCdItem, null);
            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseEatCdItem))
            {
                GuideSubSystem.Instance.TriggerCurrent();
            }

            return;
        }

        List<MergeBoard.Grid> inCdGrids = new List<MergeBoard.Grid>();
        for (int i = 0; i <MergeMainController.Instance.MergeBoard.Grids.Length; i++)
        {
            if(MergeMainController.Instance.MergeBoard.Grids[i].id != 700001)
                continue;
            
            if (!MergeManager.Instance.IsEatAllFood(MergeMainController.Instance.MergeBoard.Grids[i].board.storageMergeItem))
                continue;
                
            if (!MergeMainController.Instance.MergeBoard.Grids[i].board.IsInProductCD())
                continue;

            inCdGrids.Add(MergeMainController.Instance.MergeBoard.Grids[i]);
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
            cdFoodGridItem = grid;
            break;
        }

        if (cdFoodGridItem == null)
            return;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(cdFoodGridItem.board.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ChoseEatCdItem,
            cdFoodGridItem.board.transform as RectTransform,topLayer:topLayer);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ChoseEatCdItem, null))
        {
            GuideSubSystem.Instance.CurrentConfig.mergeStartIndex = cdFoodGridItem.board.index;
            GuideSubSystem.Instance.CurrentConfig.mergeEndIndex = cdFoodGridItem.board.index;
        }
    }
    public void CheckEaster2024Guide()
    {
        if(!Easter2024Model.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024MergeTask))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024MergeTask, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024MergeTask, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
    }
    public void CheckSnakeLadderGuide()
    {
        if(!SnakeLadderModel.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderMerge))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderMerge, tipsItem.transform as RectTransform);
            
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderMerge, null))
            {
                if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                {
                    MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                }
                return;
            }
        }
    }

    public void CheckThemeDecorationGuide()
    {
        if (GuideSubSystem.Instance.IsShowingGuide())
            return;
        if (!ThemeDecorationModel.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ThemeDecorationTask))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ThemeDecorationTask, tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ThemeDecorationTask, null))
                {
                    if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                    }
                    return;
                }   
            }
        }
       
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ThemeDecorationInfo2))
        {
            MergeThemeDecoration tipsItem = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeThemeDecoration, DynamicEntry_Game_ThemeDecoration>();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ThemeDecorationInfo2, tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ThemeDecorationInfo2, null))
                {
                    if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-tipsItem.transform.localPosition.x+220, 0);
                    }
                    return;
                }   
            }
        }
    }

    public void CheckMonopolyGuide()
    {
        // if (GuideSubSystem.Instance.IsShowingGuide())
        //     return;
        if (!MonopolyModel.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyTask))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyTask,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyTask, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    return;
                }
            }
        }
    }

    public void CheckKeepPetGuide()
    {
        if (GuideSubSystem.Instance.IsShowingGuide())
            return;
        if (!KeepPetModel.Instance.IsOpen())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetFeed3))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemBySlot(SlotDefinition.KeepPet);
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetFeed3,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetFeed3, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    return;
                }
            }
        }
    }
    
    public void CheckTeamTaskGuide()
    {
        if (GuideSubSystem.Instance.IsShowingGuide())
            return;
        if (!TeamManager.Instance.HasOrder())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TeamTask))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemBySlot(SlotDefinition.Team);
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TeamTask,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TeamTask, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    
                    var shopEntrance = (RectTransform)CurrencyGroupManager.Instance.currencyController.ShopTransform;
                    List<Transform> topLayer1 = new List<Transform>();
                    topLayer1.Add(shopEntrance.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TeamShopEntrance1, shopEntrance, topLayer:topLayer1, isReplace:false);
                    return;
                }
            }
        }
    }
    
    
    public void CheckPillowWheelGuide()
    {
        // if (GuideSubSystem.Instance.IsShowingGuide())
        //     return;
        if (!PillowWheelModel.Instance.IsStart())
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.PillowWheelGameDes))
        {
            MergeTaskTipsItem tipsItem = MergeTaskTipsController.Instance.GetTaskItemByIndex();
            if (tipsItem)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.PillowWheelGameDes,
                    tipsItem.transform as RectTransform);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.PillowWheelGameDes, null))
                {
                    if (MergeTaskTipsController.Instance != null &&
                        MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                            -tipsItem.transform.localPosition.x + 220, 0);
                    }
                    return;
                }
            }
        }
    }
}