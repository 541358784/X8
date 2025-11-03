using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Deco.Node;
using Decoration;
using DragonU3DSDK.Config;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    // private const string AutoMerge = "自动合成";
    // [Category(AutoMerge)]
    // [DisplayName("自动合成")]
    // public bool IsAutoMerge
    // {
    //     get
    //     {
    //         return AutoMergeController.Instance.Enable;
    //     }
    //     set
    //     {
    //         AutoMergeController.Instance.Enable = value;
    //     }
    // }
}

public class AutoRunTaskController : Manager<AutoRunTaskController>
{
    private void Awake()
    {
        
    }

    public void OnEnable()
    {
        
    }

    private bool _enable = false;
    public bool Enable
    {
        get
        {
            return _enable;
        }
        set
        {
            _enable = value;
        }
    }

    public static DecoNode GetCanAffordNode()
    {
        var areaNodes = DecoManager.Instance.CurrentWorld.GetUnlockAndNotOwnedNodes();
        foreach (var area in areaNodes)
        {
            if(area.Value == null || area.Value.Count == 0)
                continue;
            var nodes = area.Value;
            foreach (var node in nodes)
            {
                if (node.CanAfford())
                {
                    return node;
                }
            }
        }
        return null;
    }
    public async void CheckCanBuyNode()
    {
        while (true)
        {
            var canAffordDecoNode = GetCanAffordNode();
            if (canAffordDecoNode == null)
            {
                break;
            }
            await CompletedNextDecorTask(canAffordDecoNode);
        }
        if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
        {
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.Game);
            await XUtility.WaitSeconds(waitTime);
        }
    }
    public async Task CompletedNextDecorTask(DecoNode node)
    {
        var waitTime = 0.5f;
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        {
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
            await XUtility.WaitSeconds(waitTime);
        }
        DecoManager.Instance.SelectNode(node);
        await XUtility.WaitSeconds(waitTime);
        if (node != MainDecorationController.mainController.DecoBuyNodeView.DecoNode)
        {
            return;   
        }
        
        var performOverTask = new TaskCompletionSource<bool>();
        Action<BaseEvent> buyNodeCallback = (e) => performOverTask.SetResult(true);
        EventDispatcher.Instance.AddEventListener(EventEnum.HIDE_NODE_BUY,buyNodeCallback);
        Log("开始买点");
        MainDecorationController.mainController.DecoBuyNodeView.OnBuyNode();
        await performOverTask.Task;
        Log("买点结束回调");
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HIDE_NODE_BUY,buyNodeCallback);
        
    }
    public void Update()
    {
        if (!Enable)
            return;
        if (!MergeMainController.Instance.gameObject.activeInHierarchy)
            return;
        BeginAutoMerge();
    }

    private float waitTime = 0.3f;
    private bool inAutoMerge = false;
    public async void BeginAutoMerge()
    {
        if (inAutoMerge)
            return;
        inAutoMerge = true;
        int taskIndex = 0;
        while (true)
        {
            if (taskIndex > 10)
            {
                LogError("没有合适的任务");
                break;   
            }
            var nextTask = FindTask(taskIndex);
            var nextTaskTarget = nextTask.GetNextNeedItemConfig();
            if (nextTaskTarget != null)
            {
                try
                {
                    await AutoMerge(nextTaskTarget,FindGrid(nextTaskTarget, 999));
                    break;
                }
                catch (AutoMergeException autoMergeE)
                {
                    if (autoMergeE.EType == AutoMergeExceptionEnum.NoOperate)
                    {
                        taskIndex++;
                        continue;
                    }

                    if (autoMergeE.EType == AutoMergeExceptionEnum.FullBoard)
                    {
                        CleanAGrid(nextTaskTarget);
                        break;
                    }
                    LogError("未知错误"+autoMergeE);
                    break;
                }
            }
            var completeTaskItem = MergeTaskTipsController.Instance.GetTaskItem(nextTask.Id);
            Log("完成任务");
            if (completeTaskItem != null)
            {
                completeTaskItem.OnCompleteButton();   
            }
            // Enable = false;
            await XUtility.WaitSeconds(waitTime);
            break;
        }
        inAutoMerge = false;
    }

    private enum AutoMergeExceptionEnum
    {
        None,
        NoOperate,
        FullBoard,
    }
    class AutoMergeException : Exception
    {
        public AutoMergeExceptionEnum EType;
        public AutoMergeException(AutoMergeExceptionEnum exEnum)
        {
            EType = exEnum;
        }
    }

    public async void CleanAGrid(TableMergeItem ignoreItemConfig = null)
    {
        if (await AutoMergeAny(ignoreItemConfig) != null)//找空闲的合成
            return;
        SaleAnyItem(ignoreItemConfig);
    }
    public async Task<MergeBoard.Grid> AutoMergeAny(TableMergeItem ignoreItemConfig = null)
    {
        var grids = MergeMainController.Instance.MergeBoard.Grids;
        for (var i = 0; i < grids.Length; i++)
        {
            var grid1 = grids[i];
            if (grid1.board is null)
                continue;
            var boardItemStorage = MergeManager.Instance.GetBoardItem(grid1.index,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1);
            if (Time.time < grid1.cd)
                continue;
            if (ignoreItemConfig !=null && ignoreItemConfig.in_line == grid1.board.tableMergeItem.in_line)
                continue;
            if (grid1.board.tableMergeItem.next_level <= 0)
                continue;
            if (!StorageAvailable(boardItemStorage))
                continue;
            var grid2Container = FindGrid(grid1.board.tableMergeItem, 1, new List<MergeBoard.Grid>() {grid1});
            if (grid2Container.Count == 0)
                continue;
            var grid2 = grid2Container[0];
            return await MergeGrids(grid1, grid2);
        }
        return null;
    }

    public void SaleAnyItem(TableMergeItem ignoreItemConfig = null)
    {
        var currentLevel = 1;
        var grids = MergeMainController.Instance.MergeBoard.Grids;
        while (true)
        {
            if (currentLevel > 8)
            {
                Log("未找到能够卖出的棋子");
                return;
            }
            for (var i = 0; i < grids.Length; i++)
            {
                var grid1 = grids[i];
                if (grid1.board is null)
                    continue;
                var boardItemStorage = MergeManager.Instance.GetBoardItem(grid1.index,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1);
                var boardItemConfig = grid1.board.tableMergeItem;
                if (Time.time < grid1.cd)
                    continue;
                if (ignoreItemConfig !=null && ignoreItemConfig.in_line == boardItemConfig.in_line)
                    continue;
                if (boardItemConfig.sold_confirm)
                    continue;
                if (!StorageAvailable(boardItemStorage))
                    continue;
                if (boardItemConfig.level != currentLevel)
                    continue;
                Log("卖出");
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, new Vector2Int(grid1.index, grid1.id),MergeManager.Instance.MergeBoardID1);
                MergeMainController.Instance.mergeClickTips.OnClickSell(); //卖出
                return;
            }
            currentLevel++;
        }
    }
    public StorageTaskItem FindTask(int taskIndex = 0)
    {
        // if (MainOrderManager.Instance.CurTaskList.Count > 0)
        // {
        //     var realIndex = taskIndex % MainOrderManager.Instance.CurTaskList.Count;
        //     return MainOrderManager.Instance.CurTaskList[realIndex];
        // }
        return null;
    }

    public void Log(string s)
    {
        Debug.Log("AutoMerge "+s);
    }
    public void LogError(string s)
    {
        Debug.LogError("AutoMerge "+s);
    }
    public async Task<MergeBoard.Grid> AutoMerge(TableMergeItem itemConfig,List<MergeBoard.Grid> ignoreList=null)
    {
        var result = FindGrid(itemConfig,1,ignoreList:ignoreList);
        if (result.Count > 0)
            return result[0];
        var preLevelItemConfig =  GameConfigManager.Instance.GetItemConfig(itemConfig.pre_level);
        if (preLevelItemConfig != null)
        {
            var preLevelItemList = FindGrid(preLevelItemConfig,2);
            if (preLevelItemList.Count < 2)
            {
                await AutoMerge(preLevelItemConfig, preLevelItemList);
                return null;
            }
            return await MergeGrids(preLevelItemList[0], preLevelItemList[1]);
        }
        var operateGrids = FindOperateGrid(itemConfig, 1);
        if (operateGrids.Count == 0)
            throw new AutoMergeException(AutoMergeExceptionEnum.NoOperate);
        var index = operateGrids[0].index;
        int productCount = MergeManager.Instance.GetLeftProductCount(index,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1);
        int emptyIndex = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1);
        if (productCount > 0 && emptyIndex == -1)
            throw new AutoMergeException(AutoMergeExceptionEnum.FullBoard);
        Log("生成");
        MergeMainController.Instance.MergeBoard.OperateItem(operateGrids[0].index);
        await XUtility.WaitSeconds(waitTime);
        result = FindGrid(itemConfig,1,ignoreList:ignoreList);
        if (result.Count > 0)
            return result[0];
        return null;
    }

    public List<MergeBoard.Grid> FindGrid(TableMergeItem itemConfig,int count=1,List<MergeBoard.Grid> ignoreList=null)
    {
        var resultGrids = new List<MergeBoard.Grid>();
        var grids = MergeMainController.Instance.MergeBoard.Grids;
        for (var i = 0; i < grids.Length; i++)
        {
            var grid = grids[i];
            if (grid.board is null)
                continue;
            var boardItemStorage = MergeManager.Instance.GetBoardItem(grid.index,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1);
            if (grid.board.tableMergeItem != itemConfig)
                continue;
            if (Time.time < grid.cd)
                continue;
            if (ignoreList !=null && ignoreList.Contains(grid))
                continue;
            if (!StorageAvailable(boardItemStorage))
                continue;
            resultGrids.Add(grid);
            if (resultGrids.Count == count)
                return resultGrids;
        }
        return resultGrids;
    }

    public static bool StorageAvailable(StorageMergeItem itemStorage)
    {
        return !(MergeManager.Instance.IsBubble(itemStorage) ||
                MergeManager.Instance.IsBox(itemStorage) ||
                !MergeManager.Instance.IsOpen(itemStorage));
    }
    public List<MergeBoard.Grid> FindOperateGrid(TableMergeItem OperateItemConfig,int count=1)
    {
        var resultGrids = new List<MergeBoard.Grid>();
        var grids = MergeMainController.Instance.MergeBoard.Grids;
        foreach (var grid in grids)
        {
            if (grid.board is null)
                continue;
            var boardItemStorage = MergeManager.Instance.GetBoardItem(grid.index,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1);
            if (Time.time < grid.cd)
                continue;
            if (!StorageAvailable(boardItemStorage))
                continue;
            var itemConfig = grid.board.tableMergeItem;
            if (itemConfig.output == null || itemConfig.output.Length == 0 || itemConfig.output[0] <= 0)
                continue;
            if (MergeManager.Instance.GetLeftProductCount(grid.index,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1) == 0)
                continue;
            var canOperateFlag = false;
            for (int i = 0; i < itemConfig.output.Length; i++)
            {
                int id = itemConfig.output[i];
                var config = GameConfigManager.Instance.GetItemConfig(id);
                if (config == OperateItemConfig)
                {
                    canOperateFlag = true;
                    break;
                }
            }
            if (!canOperateFlag)
                continue;
            resultGrids.Add(grid);
            if (resultGrids.Count == count)
                return resultGrids;
        }
        return resultGrids;
    }
    public async Task<MergeBoard.Grid> MergeGrids(MergeBoard.Grid grid1,MergeBoard.Grid grid2)
    {
        var index = grid1.index;
        var activeIndex = grid2.index;
        var oldId = grid1.id;
        int upperId = GameConfigManager.Instance.GetItemConfig(oldId).next_level;
        var config = GameConfigManager.Instance.GetItemConfig(oldId);
        if (GameConfigManager.Instance.GetItemConfig(upperId) != null) // 可合成
        {
            Log("合成");
            MergeManager.Instance.SetBoardItem(activeIndex, -1, RefreshItemSource.notDeal,(MergeBoardEnum)MergeManager.Instance.MergeBoardID1);
            MergeMainController.Instance.MergeBoard.Merge(index, upperId, oldId, 0, RefreshItemSource.mergeOk, 0);
            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.MergeItem))
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MergeItem, oldId.ToString());
            }
        }
        await XUtility.WaitSeconds(waitTime);
        return grid1;
    }
}