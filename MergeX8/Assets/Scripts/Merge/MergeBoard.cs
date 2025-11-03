using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Activity.GardenTreasure.Model;
using Activity.LuckyGoldenEgg;
using Activity.Matreshkas.Model;
using Activity.TreasureHuntModel;
using UnityEngine;
using UnityEngine.EventSystems;
using DragonPlus;
using DragonU3DSDK;
using DG.Tweening;
using Dlugin;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Filthy.Game;
using Framework;
using Gameplay;
using Gameplay.UI.EnergyTorrent;
using Merge.Order;
using SomeWhere;
using Stimulate.Model.Guide;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class MergeBoard : MonoBehaviour, IOnApplicationPause
{
    public int _boardID = 0;
    static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();

    public class Grid
    {
        private int _id = -1;
        public MergeBoardItem board;

        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id > 0)
                    oldId = _id;
            }
        }

        public int oldId = -1;
        public int state = -1; // -1-未开启，0-锁定，1-解锁 2 --气泡 3 --激活类型
        public float cd;
        public int xIndex = -1;
        public int yIndex = -1;
        public int index = -1;
        public bool isProduct = false;
        public bool isAnim = false;

        public bool canTip
        {
            get { return state == 0 || state == 1 || state == 3; }
        }
    }

    static SimpleGameObjectPool itemPool;
    private static bool itemPoolInited;
    
    private Grid _hamsterGrid = null;

    public Grid HamsterGrid
    {
        get { return _hamsterGrid; }
        private set { _hamsterGrid = value; }
    }
    public static void InitPool()
    {
        if (itemPoolInited) return;
        itemPoolInited = true;
        GameObject poolRoot = new GameObject("MergeItemPoolRoot");
        poolRoot.transform.localPosition = new Vector3(10000, 10000, 10000);
        GameObject.DontDestroyOnLoad(poolRoot);
        itemPool = new SimpleGameObjectPool(poolRoot.transform);
        itemPool.SetCapacity("Prefabs/Merge/Item1", 120, 120);
        itemPool.CreateCache("Prefabs/Merge/Item1", o =>
        {
            o.transform.SetParent(poolRoot.transform);
            o.AddComponent<MergeBoardItem>();
            o.gameObject.SetActive(false);
        }, MergeManager.Instance.GetBoardHeight(MergeBoardEnum.Main) * MergeManager.Instance.GetBoardWidth(MergeBoardEnum.Main)); //60为棋盘格子数量，大概率是不会变的
    }


    public RectTransform mContent { get; private set; }
    Vector3 originalScale;
    RectTransform mFullShield;
    List<RectTransform> eventObjectList = new List<RectTransform>();
    Transform mDragLayer;
    Rect boardRect;
    Vector2 gridSize;

    public Vector2 GridSize
    {
        get { return gridSize; }
    }

    protected Grid[] grids;
    int boardWidth, boardHeight;
    int selectIndex, focusIndex;
    public int activeIndex { get; set; }
    Vector3 downPos;
    bool dragLock = true;
    Tweener dragTweener;
    Vector3 dragPos;
    public List<Grid> mergeTipList = new List<Grid>();
    
    GameObject firstEventGo;
    GameObject firstDragGo;
    float foucsTime = 1;
    private bool isDraging = false;

    private FocusLogic _focusLogic;
    private BaseEventData _baseEventData = null;
    private Coroutine _dealyAnim = null;
    private Coroutine _dealyGuide = null;

    private HashSet<GameObject> _doAnimationList = new HashSet<GameObject>();
    public Grid[] Grids
    {
        get { return grids; }
    }

    public int SelectIndex
    {
        get { return selectIndex; }
    }

    private int[][] rectBorder = new[]
    {
        new[] {-1, 1},
        new[] {1, 1},
        new[] {-1, -1},
        new[] {1, -1}
    };

    public void SetBoardID(int boardID)
    {
        _boardID = boardID;
    }

    public virtual void InitBoardId()
    {
        SetBoardID((int) MergeBoardEnum.Main);
    }
    void Awake()
    {
        InitBoardId();
        boardWidth = MergeManager.Instance.GetBoardWidth((MergeBoardEnum)_boardID);
        boardHeight = MergeManager.Instance.GetBoardHeight((MergeBoardEnum)_boardID);
        originalScale = transform.localScale;

        Transform mMain = transform.Find("Main");
        mContent = mMain.Find("Content") as RectTransform;

        mFullShield = mMain.Find("FullShield") as RectTransform;
        mDragLayer = mMain.Find("DragLayer");
        _focusLogic = mMain.Find("Focus").gameObject.AddComponent<FocusLogic>();
        boardRect = mContent.rect;
        gridSize.x = boardRect.width / boardWidth;
        gridSize.y = boardRect.height / boardHeight;
        InitGrids(boardWidth * boardHeight);
        activeIndex = -1;

        AddEventObject(mContent);
        CommonUtils.AddEventTrigger(mFullShield.gameObject, EventTriggerType.PointerDown, OnPointerDown);
        CommonUtils.AddEventTrigger(mFullShield.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        CommonUtils.AddEventTrigger(mFullShield.gameObject, EventTriggerType.Drag, OnDrag);
        CommonUtils.AddEventTrigger(mFullShield.gameObject, EventTriggerType.BeginDrag, OnBeginDrag);
        CommonUtils.AddEventTrigger(mFullShield.gameObject, EventTriggerType.EndDrag, OnEndDrag);
        
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_USE_ITEM, OnUseItem);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_NEW_ITEM, OnNewItem);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_REFRESH, OnRefresh);
        EventDispatcher.Instance.AddEventListener(EventEnum.TASK_REFRESH, RefreshTask);
        EventDispatcher.Instance.AddEventListener(EventEnum.GARAGE_CLEANUP_TURNIN, RefreshTask);
        EventDispatcher.Instance.AddEventListener(EventEnum.GARAGE_CLEANUP_LevelFinish, RefreshTask);
        EventDispatcher.Instance.AddEventListener(EventEnum.TRAIN_ORDER_ORDER_REFRESH, RefreshTask);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND, RefreshEnergyTorrendState);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_REFRESH_UNLIMITEDPRODUCT, RefreshUnlimitedProductState);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_SELL_ITEM, OnSellItem);

        InvokeRepeating("RefreshAllProduct", 4f, 1);

        _focusLogic?.Hide();
    }

    public void LockBoard()
    {
        mFullShield.GetComponent<CKEmptyRaycast>().raycastTarget = false;
    }
    public void UnLockBoard()
    {
        mFullShield.GetComponent<CKEmptyRaycast>().raycastTarget = true;
    }

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_USE_ITEM, OnUseItem);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_NEW_ITEM, OnNewItem);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_REFRESH, OnRefresh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TASK_REFRESH, RefreshTask);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GARAGE_CLEANUP_TURNIN, RefreshTask);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GARAGE_CLEANUP_LevelFinish, RefreshTask);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TRAIN_ORDER_ORDER_REFRESH, RefreshTask);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BORAD_SELL_ITEM, OnSellItem);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND, RefreshEnergyTorrendState);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BORAD_REFRESH_UNLIMITEDPRODUCT, RefreshUnlimitedProductState);
    }

    public void RefreshGridsStatus()
    {
        if (grids == null || grids.Length == 0)
            return;

        foreach (var p in grids)
        {
            if (p != null && p.board != null && p.board.gameObject != null)
            {
                p.board.RefreshAllStatus();
            }
        }
    }

    public Grid GetGridByIndex(int index)
    {
        if (grids == null || grids.Length == 0)
            return null;

        if (index < 0 || index >= grids.Length)
            return null;

        return grids[index];
    }

    public Vector3 GetGridPosition(int index)
    {
        Grid grid = GetGridByIndex(index);
        if (grid == null)
            return Vector3.zero;

        if (grid.board == null)
            return Vector3.zero;

        return grid.board.transform.position;
    }

    public Transform GetGridTransform(int index)
    {
        Grid grid = GetGridByIndex(index);
        if (grid == null)
            return null;

        if (grid.board == null)
            return null;

        return grid.board.transform;
    }

    public Grid GetGridById(int id)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i] == null)
                continue;

            if (grids[i].state == 1 && grids[i].id == id)
                return grids[i];
        }

        return null;
    }

    public List<Grid> GetGridsById(int id)
    {
        List<Grid> findList = new List<Grid>();
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i] == null)
                continue;

            if ((grids[i].state != 1 && grids[i].state != 3) || grids[i].id != id || grids[i].board == null)
                continue;

            findList.Add(grids[i]);
        }

        return findList;
    }

    
    public bool CanFeedFood(int feedId, int foodId)
    {
        Grid feedGrid = null;
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i] != null && grids[i].id == feedId)
                feedGrid = grids[i];
        }

        if (feedGrid != null)
        {
            if (feedGrid.board !=null &&feedGrid.board.storageMergeItem!=null &&
                !feedGrid.board.storageMergeItem.EatBuildingDic.ContainsKey(foodId))
                return true;
        }
        
        return false;
    }
    
    public Grid FindOneProductItem(ref int index)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i].state == 1)
            {
                if (MergeConfigManager.Instance.IsCanProductItem(grids[i].id))
                {
                    index = i;
                    return grids[i];
                }
            }
        }

        return null;
    }

    public int GetGridIndex(Grid grid)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i] == grid)
                return i;
        }

        return 0;
    }

    public Grid GetOneBoxItem(ref int index)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i].state == 3)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[i].id);
                if (itemConfig?.type ==  (int) MergeItemType.box && itemConfig?.active_cost != null)
                {
                    index = i;
                    return grids[i];
                }
            }
        }

        return null;
    }

    public Grid GetOneBoxItemActive(ref int index)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i].state == 1)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[i].id);
                if (itemConfig?.type ==  (int) MergeItemType.box && itemConfig?.active_cost != null)
                {
                    index = i;
                    return grids[i];
                }
            }
        }

        return null;
    }

    public Grid GetOneBoxItemInActive(ref int index)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i].state == 1)
            {
                if (MergeManager.Instance.GetLeftActiveTime(i,(MergeBoardEnum)_boardID) > 0)
                {
                    index = i;
                    return grids[i];
                }
            }
        }

        return null;
    }


    int PosToIndex(Vector3 pos)
    {
        int x = (int) ((pos.x - boardRect.x) / gridSize.x);
        int y = (int) ((pos.y - boardRect.y) / gridSize.y);
        if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight && Mathf.Abs(pos.z) < 1)
        {
            return x + y * boardWidth;
        }

        return -1;
    }


    // private int gridSizeSub = 5;
    private float gridSizeScale = 0.1f;

    int FindSameIndex(Vector3 pos, int index)
    {
        int id = -1;
        if (index > 0)
            id = grids[index].id;

        if (id < 0)
            return PosToIndex(pos);
        var w = /*(int)*/(gridSize.x / 2);// - gridSizeSub;
        var h = /*(int)*/(gridSize.y / 2);// - gridSizeSub;
        w *= gridSizeScale;
        h *= gridSizeScale;
        
        for (int i = 0; i < rectBorder.Length; i++)
        {
            Vector3 newPos = pos;
            newPos.x += rectBorder[i][0] * w;
            newPos.y += rectBorder[i][1] * h;
        
            int idx = PosToIndex(newPos);
            if (idx < 0 || index == idx)
                continue;
        
            if (grids[idx] == null || grids[idx].board == null)
                continue;
        
            if (grids[idx].id == id && (grids[idx].state == 1 || grids[idx].state == 3))
                return idx;
        }

        return PosToIndex(pos);
    }

    public Vector3 IndexToPos(int index)
    {
        int x = index % boardWidth;
        int y = index / boardWidth;
        return new Vector3(boardRect.x + gridSize.x * (x + 0.5f), boardRect.y + gridSize.y * (y + 0.5f));
    }

    public Vector3 IndexToPosition(int index)
    {
        return transform.TransformPoint(IndexToPos(index));
    }

    void RefreshGrid(int index, int id, int state, bool autoTips = true,
        RefreshItemSource source = RefreshItemSource.notDeal, int oldIndex = -1, int oldId = -1)
    {
        Grid grid = grids[index];
        grid.isAnim = false;

        if (grid.id != id)
        {
            CancelTip(index);

            if (index == selectIndex)
            {
                SelectFocus(-1);
            }
        }

        if (id > 0 && grid.id == id && grid.board != null)
        {
            var updateItemStorage = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
            MergeItemStatus updateStatus = MergeManager.Instance.GetMergeItemStatus(updateItemStorage);

            if (state != grid.state)
            {
                int oldState = grid.state;
                grid.state = state;
                grid.board.state = updateStatus;
                if (oldState != -1)
                    grid.board.UpdateData(false);

                StartChoseMergeItemGuide();
                RefreshGridState(index);
            }

            if (autoTips)
                AutoTips();
            return;
        }

        if (grid.id != id && grid.board != null)
        {
            grid.board.id = -1;
            grid.board.Reset();
            itemPool.Release(grid.board.gameObject);
            grid.board = null;
        }

        grid.id = id;
        grid.state = state;
        grid.isProduct = false;
        if (grid.id < 0)
        {
            if (autoTips)
                AutoTips();
            
            UpdateSibling(true);
            return;
        }

        var itemStorage = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(grid.id);
        MergeItemStatus status = MergeManager.Instance.GetMergeItemStatus(itemStorage);
        grid.isProduct = MergeConfigManager.Instance.IsProductItem(itemConfig);
        
        if (HamsterGrid == null && itemConfig != null && itemConfig.type != null && itemConfig.type ==  (int)MergeItemType.hamaster)
            HamsterGrid = grid;

        if (grid.board == null)
        {
            var item = itemPool.Create("Prefabs/Merge/Item1").transform.GetComponentDefault<MergeBoardItem>();
            item.SetBoardId((MergeBoardEnum) _boardID);
            item.name = "Item_" + index;
            grid.board = item;
            grid.board.id = id;
            grid.board.state = status;
            grid.board.index = index;
            grid.board.transform.SetParent(mContent);

            grid.board.Reset();
            grid.xIndex = index % boardWidth;
            grid.yIndex = index / boardWidth;
            grid.index = index;
            if (itemConfig == null)
            {
                MergeManager.Instance.RemoveBoardItem(index,(MergeBoardEnum)_boardID, "Refresh_"+source.ToString());
                return;
            }

            
            grid.board.UpdateData(true);
            if(source == RefreshItemSource.mergeOk)
                grid.board.UpdateIconImage(oldId);
        }
        grid.board.transform.localPosition = IndexToPos(index);
        grid.board._orgLocalPosition = grid.board.transform.localPosition;
        
        grid.isAnim = true;
        _doAnimationList.Add(grid.board.gameObject);
        var temp = grid.board.gameObject;
        
        AddSiblingTransform(grid.board.transform);
        DoAnimation(index, oldIndex, source, () =>
        {
            if (autoTips)
                AutoTips();
            grid.isAnim = false;
            RefreshGridState(index);
            _doAnimationList.Remove(temp);
            if(grid!=null && grid.board!=null)
                DelSiblingTransform(grid.board.transform);
               UpdateSibling(true);
        });
        StartChoseMergeItemGuide();
    }

    private void SetAnimationsObjLastSibling()
    {
        foreach (var o in _doAnimationList)
        {
            o.transform.SetAsLastSibling();
        }
    }
    //箱子引导
    private void StartChoseMergeItemGuide()
    {
        if (_dealyGuide != null)
            StopCoroutine(_dealyGuide);

        if(!gameObject.activeInHierarchy)
            return;
        
        _dealyGuide = StartCoroutine(DelayWorkGuideLogic());
    }

    private IEnumerator DelayWorkGuideLogic()
    {
        yield return new WaitForSeconds(0.1f);
        MergeGuideLogic.Instance.CheckChoseItemGuide();
        _dealyGuide = null;
    }

    void RefreshGridState(int index)
    {
        var itemStorage = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
        if (itemStorage == null || itemStorage.Id == -1 || MergeManager.Instance.IsBox(itemStorage) ||
            MergeManager.Instance.IsLockWeb(index,(MergeBoardEnum)_boardID) || MergeManager.Instance.IsBubble(index,(MergeBoardEnum)_boardID))
            return;

        Grid grid = grids[index];
        if (grid == null || grid.board == null)
            return;

        int id = grid.id;
        grid.board.SetTaskStatus(false, false);
        grid.board.SetDailyTaskStatus(false);
        grid.board.SetOrderTaskStatus(false);
        grid.board.SetGarageCleanupStatus(false);

        if (MergeManager.Instance.IsBox(itemStorage))
            return;

        bool isTaskItem = MainOrderManager.Instance.IsTaskNeedItem(id);
        bool isCompleteItem = MainOrderManager.Instance.IsCompleteTaskByItem(id);
        if(_boardID != (int)MergeBoardEnum.Stimulate && _boardID != (int)MergeBoardEnum.Filthy && _boardID != (int)MergeBoardEnum.Ditch&& _boardID != (int)MergeBoardEnum.TrainOrder)
            grid.board.SetTaskStatus(isTaskItem, isCompleteItem);
     
        if (_boardID == (int)MergeBoardEnum.TrainOrder)
        {
            isCompleteItem = Activity.TrainOrder.TrainOrderModel.Instance.IsOrderNeedItem(id);
            grid.board.SetTaskStatus(false, isCompleteItem);
        }
        
        if (!isTaskItem && GarageCleanupModel.Instance.IsReveal())
        {
            bool isGarageCleanupItem = GarageCleanupModel.Instance.IsTaskNeedItem(id);
            grid.board.SetGarageCleanupStatus(isGarageCleanupItem);
        }

    }

    void InitGrids(int number)
    {
        grids = new Grid[number];
        int index = -1;
        for (int y = boardHeight-1; y >=0; y--)
        {
            for (int x = 0; x<boardWidth; x++)
            {
                index = y * boardWidth + x;
                grids[index] = new Grid();
                var data = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                RefreshGrid(index, data.Id, data.State, false);
            } 
        }
        
        UpdateSibling();
    }

    public void RefreshTask(BaseEvent e)
    {
        if ((MergeBoardEnum) e.datas[0] != (MergeBoardEnum)_boardID)
            return;
        for (int i = 0; i < grids.Length; i++)
        {
            RefreshGridState(i);
        }
    }

    void OnRefresh(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length < 3)
            return;
        int mergeBoardID = (int) e.datas[0];
        if (mergeBoardID != _boardID)
            return;
        int index = (int) e.datas[1];
        int oldIndex = (int) e.datas[2];
        RefreshItemSource source = (RefreshItemSource) e.datas[3];

        int oldId = -1; 
        if(e.datas.Length >= 6)
            oldId = (int) e.datas[5];
        
        RefreshGrid(index, MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID).Id,
            MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID).State, true, source, oldIndex, oldId);
    }
    void OnNewItem(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length < 3)
            return;
        int mergeBoardID = (int) e.datas[0];
        if (mergeBoardID != _boardID)
            return;
        int index = (int) e.datas[1];
        int oldIndex = (int) e.datas[2];
        RefreshItemSource source = (RefreshItemSource) e.datas[3];

        int id = (int) e.datas[4];
        
        int oldId = -1; 
        if(e.datas.Length >= 6)
            oldId = (int) e.datas[5];
        OnNewItem(index,oldIndex,id,oldId,source);
    }

    void OnUseItem(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length < 3)
            return;
        int mergeBoardID = (int) e.datas[0];
        if (mergeBoardID != _boardID)
            return;
        int index = (int) e.datas[1];
        int id = (int) e.datas[2];
        OnUseItem(index, id);
    }

    public virtual void OnUseItem(int index,int id)
    {
        
    }

    public virtual void OnNewItem(int index, int oldIndex, int id, int oldId,RefreshItemSource source)
    {
        
    }

    void MoveHome(int index)
    {
        grids[index].board.transform.DOLocalMove(IndexToPos(index), 0.3f);
        grids[index].board._orgLocalPosition = IndexToPos(index);
        grids[index].cd = Time.time + 0.3f;
        grids[index]?.board.SetMergeStatus(false);
    }

    public void SelectFocus(int index, bool isMergeAnim = false, bool isPlayAnim = true, bool isFinishGuide = false)
    {
        foucsTime = Time.time;
        selectIndex = index;
        if (index >= 0 && index < grids.Length && grids[index].id != -1)
        {
            _focusLogic?.Focus(IndexToPos(index), MergeConfigManager.Instance.IsMaxLevel(grids[index].id));

            string animName = isMergeAnim ? "object_shake" : "click";
            if (MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID))
            {
            }
            else if (MergeManager.Instance.IsOpen(index,(MergeBoardEnum)_boardID))
            {
                if (!grids[index].isAnim && isPlayAnim)
                    grids[index].board.PlayAnimator(animName, true);
            }
            else
            {
                if (!grids[index].isAnim && isPlayAnim)
                    grids[index].board.PlayAnimator(animName, true);
            }

            if (MergeConfigManager.Instance.IsEnergyProductItem(grids[index].id))
            {
                //grids[index].board.UpdateOutPutStatus();
            }
        }
        else
        {
            _focusLogic.Hide();
        }

        if (index != -1)
        {
            Grid grid = grids[index];

            int id = grid.id > 0 ? grid.id : grid.oldId;
            if (isFinishGuide)
            {
                if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ChoseItem))
                {
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ChoseItem, id.ToString());
                }

                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ChoseCdProduct, null);
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.HappyGoCdProduct, null);
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ChoseEatCdItem, id.ToString());
            }

            if (grid != null)
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID,
                    new Vector2Int(index, grid.id),(MergeBoardEnum)_boardID);
        }

        PlayTipsAnimation(index);
    }

    public bool OperateItem(int index, RefreshItemSource source = RefreshItemSource.product)
    {
        bool ignoreUse = false;
        Grid grid = grids[index];
        var itemConfig = GameConfigManager.Instance.GetItemConfig(grid.id);
        var storeConfig = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
        bool isOpen = MergeManager.Instance.IsOpen(storeConfig);
        int leftTime = MergeManager.Instance.GetLeftActiveTime(index,(MergeBoardEnum)_boardID);
        if (!isOpen || MergeManager.Instance.IsActiveItem(index,(MergeBoardEnum)_boardID) ||
            leftTime > 0 ||
            !MergeConfigManager.Instance.IsCanProductItem(itemConfig))
        {
            if(isOpen && leftTime > 0)
                MergePromptManager.Instance.ShowBoxOpenNow(IndexToPosition(index));

            if (MergeConfigManager.Instance.IsTimeProductItem(itemConfig)) //时间产出的建筑物
            {
                TimeProductItem(index, itemConfig);

                bool isMax = MergeManager.Instance.IsTimeProductMaxCount(index, itemConfig,(MergeBoardEnum)_boardID);
                // DebugUtil.LogError("时间产出建筑 剩余产出次数:" + MergeManager.Instance.GetTimeProductTimes(index) + "---是否产出队列已满：" + isMax);
                if (!isMax && !MergeManager.Instance.IsTimeProductInCD(index,(MergeBoardEnum)_boardID)) // 目前只支持 产出一个
                {
                    int outputId1 = MergeConfigManager.Instance.GetOneTimeOutput(itemConfig);
                    int pos = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)_boardID);
                    if (pos == -1) // 没有位置了, 暂存到队列中
                    {
                        MergePromptManager.Instance.ShowTextPrompt(IndexToPosition(index));
                        DebugUtil.Log("没有位置了, :" + outputId1);
                    }
                    else
                    {
                        AudioManager.Instance.PlaySound(13);
                        SendProductBi(grid.id, outputId1, -1, 0, true, "not cd");
                        DebugUtil.Log("直接产出物品:" + outputId1 + "---位置:" + pos + "---产出建筑id：" + grid.id);
                        grids[index]?.board?.PlayAnimator("click", true);
                        ProductOneItem(index, pos, outputId1, false, RefreshItemSource.timeProduct);
                    }
                    
                    RefreshOneProduct(index);
                    // DebugUtil.LogError("产出之后剩余次数:" + MergeManager.Instance.GetLeftProductCount(index));
                    if (SelectIndex > 0 && SelectIndex == index)
                        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID,
                            new Vector2Int(index, grid.id),(MergeBoardEnum)_boardID);
                }
                else
                {
                    List<int> items = MergeManager.Instance.GetTimeProductItems(index,(MergeBoardEnum)_boardID);
                    if (items == null || items.Count == 0) //在九宫格范围内产生存贮的item
                    {
                        MergePromptManager.Instance.ShowRecharging(IndexToPosition(index));
                        return ignoreUse;
                    }

                    int pos=MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)_boardID);
                    if (pos != -1)
                    {
                        grids[index]?.board?.PlayAnimator("click", true);

                        AudioManager.Instance.PlaySound(13);
                        ProductOneItem(index, pos, items[0], false, RefreshItemSource.timeProduct,isFreeTimeProduct:true);
                        MergeManager.Instance.FreeTimeProductItems(index, items[0],(MergeBoardEnum)_boardID);
           
                        RefreshOneProduct(index);
                        if (SelectIndex > 0 && SelectIndex == index)
                            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID,
                                new Vector2Int(index, grids[index].id),(MergeBoardEnum)_boardID);
                    }
                    else
                    {
                        MergePromptManager.Instance.ShowTextPrompt(IndexToPosition(index));
                    }
                }
            }
            return ignoreUse;
        }

        int minEnergy = 0;
        if (EnergyTorrentModel.Instance.IsOpen() && MergeConfigManager.Instance.IsEnergyTorrentProduct(itemConfig)
                                                 &&_boardID!=(int)MergeBoardEnum.HappyGo)
            minEnergy = EnergyTorrentModel.Instance.GetMultiply()-1;
        int haveEnergy = _boardID == (int) MergeBoardEnum.HappyGo
            ? UserData.Instance.GetRes(UserData.ResourceId.HappyGo_Energy)
            : UserData.Instance.GetRes(UserData.ResourceId.Energy);
        
        if (haveEnergy <= minEnergy && MergeConfigManager.Instance.IsEnergyProductItem(itemConfig)&&!EnergyModel.Instance.IsEnergyUnlimited())
        {
            if (itemConfig.output_cost.Length == 2)
            {
                if (itemConfig.output_cost[1] > 0)
                {
                    // BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Energy,
                    //     itemConfig.id.ToString(), "", "diamond_lack_energy");
                    if (_boardID != (int) MergeBoardEnum.HappyGo)
                    {
                        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.FreeEnergy))
                        {
                            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.AddEnergy, "0");
                            return ignoreUse;
                        }
                        UIManager.Instance.OpenUI(UINameConst.UIPopupBuyEnergy, "diamond_lack_energy");
                        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyPop,
                            "auto");
                    }
                    else
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoBuyEnergy, "diamond_lack_energy");
                    }
                   
                    return ignoreUse;
                }
            }
        }

        // if (itemConfig.type ==  (int) MergeItemType.box)
        //     GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChestView);
        int productCount = MergeManager.Instance.GetLeftProductCount(index,(MergeBoardEnum)_boardID);
        int status = itemConfig.output_lock == 1 ? 0 : 1;

        int emptyIndex = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)_boardID);
        if (productCount > 0 && emptyIndex == -1)
        {
            MergePromptManager.Instance.ShowTextPrompt(IndexToPosition(index));
            return ignoreUse;
        }
        //建筑产出
        int outputId = 0;
        if (itemConfig.level_box_produce != null && itemConfig.level_box_produce.Length > 0)
        {
            if (storeConfig.ProductCount >= 0 && storeConfig.ProductCount < itemConfig.level_box_produce.Length)
                outputId = itemConfig.level_box_produce[storeConfig.ProductCount];
        }

        if (itemConfig.subType == (int)SubType.Matreshkas)
        {
            var presetQueue = MatreshkasModel.Instance.GetMatreshkasPresetQueue();
            int productNum = storeConfig.ProductCount;
            productNum = Math.Clamp(productNum, 0, presetQueue.Count - 1);

            outputId = presetQueue[productNum];
        }
        
        if (outputId == 0)
        {
            outputId = MergeManager.Instance.GetMergeLineProductItemId(index,(MergeBoardEnum)_boardID); // 次数产出
            if (outputId>0 && itemConfig.outputLimit != null && itemConfig.outputLimit.Length > 0 && itemConfig.outputLimit.Length==itemConfig.output.Length)
            {
                if (ExperenceModel.Instance.GetLevel() < itemConfig.outputLimit[itemConfig.output.IndexOfEx(outputId)])
                    outputId = 0;
            }
        }
     
        if (outputId == 0)
        {
            outputId = MergeManager.Instance.GetDropIntervalOutputId(index,(MergeBoardEnum)_boardID); // 保底产出
            if (outputId == 0)
            {
                if (itemConfig.outputLimit != null && itemConfig.outputLimit.Length > 0 &&
                    itemConfig.outputLimit.Length == itemConfig.output.Length)
                {
                    outputId = MergeConfigManager.Instance.GetOneOutputByLimit(itemConfig); //有限制的产出
                }else if ((itemConfig.outputLimitByTimes != null && itemConfig.outputLimitByTimes.Length > 0)
                    ||itemConfig.dynamicPowerItem != null && itemConfig.dynamicPowerItem.Length > 0)
                {
                    outputId = MergeConfigManager.Instance.GetOneOutputByLimitDynamicPower(itemConfig,index,(MergeBoardEnum)_boardID); //有限制的产出动态权重
                }
                else
                {
                    outputId = MergeConfigManager.Instance.GetOneOutput(itemConfig); //正常产出
                }
            }
              
        }
        if (outputId == -1 || outputId == 0)
        {
            return ignoreUse;
        }
        MergeManager.Instance.RecordDropInterval(index, outputId ,(MergeBoardEnum)_boardID);
        
        if (MergeConfigManager.Instance.IsEnergyTorrentProduct(itemConfig) && EnergyTorrentModel.Instance.IsOpen() && _boardID != (int) MergeBoardEnum.HappyGo)
        {
            var outPutConfig=  GameConfigManager.Instance.GetItemConfig(outputId);
            if (outPutConfig != null && outPutConfig.next_level > 0)
            {
                outputId = outPutConfig.next_level;
                if (EnergyTorrentModel.Instance.GetMultiply() > 2)
                {
                    var outPutConfig2=  GameConfigManager.Instance.GetItemConfig(outputId); 
                    if (outPutConfig2 != null && outPutConfig2.next_level > 0)
                    {
                        outputId = outPutConfig2.next_level;
                        if (EnergyTorrentModel.Instance.GetMultiply() > 4)
                        {
                            var outPutConfig3=  GameConfigManager.Instance.GetItemConfig(outputId); 
                            if (outPutConfig3 != null && outPutConfig3.next_level > 0)
                            {
                                outputId = outPutConfig3.next_level;
                            }
                        }
                    }
                }
            }
        }

        if (productCount == 1)
            storeConfig.ProductWheel++;

        if (productCount <= 0)
        {
            if (MergeConfigManager.Instance.IsDeathProductItem(itemConfig, storeConfig))
            {
                outputId = itemConfig.out_death;
                emptyIndex = index;
                ProductOneItem(index, emptyIndex, outputId, true, source, status);
                AudioManager.Instance.PlaySound(13);

                EventDispatcher.Instance.DispatchEvent(EventEnum.DEATH_PRODUCT_ITEM, itemConfig);
            }
            else
            {
                RefreshOneProduct(index);

                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBuildingsCdView);
                
                if (productCount == 0)
                {
                    MergePromptManager.Instance.ShowRecharging(IndexToPosition(index));
                }
            }

            AudioManager.Instance.PlaySound(42);
            return ignoreUse;
        }

        var costEnergy = false;
        var doubleEnergyTimes = 0;
        int cost = 0;
        if (itemConfig.output_cost.Length >= 2 && itemConfig.output_cost[0] == 1) //体力产出的消耗体力
        {
            costEnergy = true;
            cost = itemConfig.output_cost[1];
            if (MergeConfigManager.Instance.IsEnergyTorrentProduct(itemConfig) && EnergyTorrentModel.Instance.IsOpen()&&(_boardID == (int) MergeBoardEnum.Main||_boardID == (int) MergeBoardEnum.TrainOrder))
            {
                var multiply = EnergyTorrentModel.Instance.GetMultiply();
                cost *= multiply;
                if (multiply > 0)
                {
                    while (multiply > 1)
                    {
                        multiply /= 2;
                        doubleEnergyTimes++;
                    }
                }
            }

            if (_boardID == (int) MergeBoardEnum.HappyGo)
            {
                CurrencyGroupManager.Instance.CostCurrency(UserData.ResourceId.HappyGo_Energy, cost,
                    new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.HgVdEnergyConsume});
            }
            else
            {
                if (EnergyModel.Instance.IsEnergyUnlimited())
                {
                    GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Infinity_Energy,cost, (ulong)UserData.Instance.GetRes(UserData.ResourceId.Energy),
                        new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.UseBuildingUnlimited,
                            data1 = itemConfig.id.ToString(),
                            data2 =MergeManager.Instance.IsInUnlimitedProduct((MergeBoardEnum)_boardID).ToString()
                        });
                }
                CurrencyGroupManager.Instance.CostCurrency(UserData.ResourceId.Energy, cost,
                    new GameBIManager.ItemChangeReasonArgs() {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.UseBuilding,
                        data1 = itemConfig.id.ToString(),
                        data2 =MergeManager.Instance.IsInUnlimitedProduct((MergeBoardEnum)_boardID).ToString(),
                        data3 = outputId.ToString()
                    });
            }
            
           
        }
        var oldId = grid.id;
        SendProductBi(grid.id, outputId, productCount - 1, 0, false, "not cd");
        //GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyBet,EnergyTorrentModel.Instance.GetMultiply().ToString(),itemConfig.id.ToString());

        ProductOneItem(index, emptyIndex, outputId, false, source, status);

        MergeFtueBiManager.Instance.SendFtueBi(MergeFtueBiManager.SendType.TouchProduct, grid.id);

        AudioManager.Instance.PlaySound(13);
        if (productCount == 1)
            RefreshOneProduct(index);

        if (grids[index].board != null && grids[index].board.IsInProductCD())
            storeConfig.PlayRvNum = 0;
        
        if (productCount == 1)
        {
            if (itemConfig != null && itemConfig.type == (int)MergeItemType.hamaster)
            {
                HappyGoModel.Instance.AddRequestIndex();
                MergeManager.Instance.storageBoard.Items[index].ProductCount = 0;
                MergeManager.Instance.SetOrginalStoreCount(MergeManager.Instance.storageBoard.Items[index]);
                MergeManager.Instance.RefreshMaxStoreCount(itemConfig.id, index, itemConfig,(MergeBoardEnum)_boardID);
                storeConfig.State = 3;
            }
            if (itemConfig != null && itemConfig.type == (int)MergeItemType.eatBuild)
            {
                MergeManager.Instance.storageBoard.Items[index].ProductCount = 0;
                MergeManager.Instance.storageBoard.Items[index].EatBuildingDic.Clear();
                grids[index]?.board?.UpdateIconImage();
                grids[index]?.board?.RefreshSlider();
                storeConfig.State = 3;
            }

        }
        ShakeManager.Instance.ShakeLight();
        // end 建筑产出
        if (costEnergy)
        {
            BiuBiuModel.Instance.TryProductActivityItem(index,oldId,this,doubleEnergyTimes);
            ButterflyWorkShopModel.Instance.TryProductActivityItem(index,oldId,this,doubleEnergyTimes);
            SummerWatermelonModel.Instance.TryProductWaterMellon(index,oldId,this,doubleEnergyTimes);
            TreasureHuntModel.Instance.TryProductHammer(index,oldId,this,doubleEnergyTimes);
            LuckyGoldenEggModel.Instance.TryProductHammer(index,oldId,this,doubleEnergyTimes);
            GardenTreasureModel.Instance.TryProductShovel(index,oldId,this,doubleEnergyTimes);
            SummerWatermelonBreadModel.Instance.TryProductWaterMellon(index,oldId,this,doubleEnergyTimes);
            // ClimbTreeModel.Instance.TryProductClimbTree(index,oldId,doubleEnergyTimes,this,out ignoreUse);
            
            AdLocalConfigHandle.Instance.RefreshJudgingData(AdLocalDataType.CostEnergy,add:cost);

        }
        return ignoreUse;
    }

    public void ProductOneItem(int oldIndex, int newIndex, int id, bool isDeath,
        RefreshItemSource source = RefreshItemSource.product, int state = 1,bool isFreeTimeProduct=false)
    {
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
        {
            MergeManager.Instance.RemoveBoardItem(oldIndex,(MergeBoardEnum)_boardID, "Product_1");
            throw new System.Exception("策划配置表产出物Id出错:" + id);
        }

        MergeManager.Instance.RefreshInCdTime(grids[oldIndex].id, oldIndex,(MergeBoardEnum)_boardID);
        if (isDeath)
        {
            //老物品 消失
            Debug.Log("/老物品 消失1");
            MergeManager.Instance.RemoveBoardItem(oldIndex,(MergeBoardEnum)_boardID,"Product_2");

            //产出新物品
            MergeManager.Instance.SetNewBoardItem(oldIndex, id, state, source,(MergeBoardEnum)_boardID, oldIndex);
            MergeEffectManager.Instance.PlayBombEffect(MergeMainController.Instance.MergeBoard.IndexToPosition(oldIndex));
        }
        else
        {
            if (oldIndex != -1 && state != 2 && source != RefreshItemSource.exp && !isFreeTimeProduct) //  时间产出类的建筑 存贮生产的item
                MergeManager.Instance.RecordProductInfo(oldIndex, id, (MergeBoardEnum)_boardID,source);

            TableMergeItem oldTableMerge =
                GameConfigManager.Instance.GetItemConfig(MergeManager.Instance.GetBoardItem(oldIndex,(MergeBoardEnum)_boardID).Id);
            StorageMergeItem oldStorageMerge = MergeManager.Instance.GetBoardItem(oldIndex,(MergeBoardEnum)_boardID);
            if (source==RefreshItemSource.timeProduct)
            {
                AudioManager.Instance.PlaySound(44);
                if (oldStorageMerge!=null && oldStorageMerge.TimeStoreMax <= 0 && oldStorageMerge.ProductItems.Count <= 1)
                    oldStorageMerge.ProductWheel++;
            }
            if (MergeConfigManager.Instance.IsDeathProductItem(oldTableMerge, oldStorageMerge, false))
            {
                MergeManager.Instance.RemoveBoardItem(oldIndex,(MergeBoardEnum)_boardID,"Product_Death");
                MergeManager.Instance.SetNewBoardItem(oldIndex, oldTableMerge.out_death, state, source,(MergeBoardEnum)_boardID, oldIndex);
                MergeEffectManager.Instance.PlayBombEffect(IndexToPosition(oldIndex));
                
                EventDispatcher.Instance.DispatchEvent(EventEnum.DEATH_PRODUCT_ITEM, oldTableMerge);
            }

            MergeManager.Instance.SetNewBoardItem(newIndex, id, state, source,(MergeBoardEnum)_boardID, oldIndex);

            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ProductBuild))
            {
                string param = GuideSubSystem.Instance.GetActionParams();
                if (!param.IsEmptyString())
                {
                    int actionParamId = int.Parse(param);
                    if (actionParamId == grids[oldIndex].id)
                        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ProductBuild);
                }
                else
                {
                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ProductBuild);
                }
            }

            if (_boardID == (int) MergeBoardEnum.HappyGo)
            {
                HappyGoMergeGuideLogic.Instance.CheckMergeGuide();
            }
            else
            {
                MergeGuideLogic.Instance.CheckMergeGuide();
            }
        }
    }

    private void SendProductBi(int id, int newId, int leftCount, int cd, bool isAuto, string state)
    {
        // var product = GameConfigManager.Instance.GetItemConfig(id);
        // var newConfig = GameConfigManager.Instance.GetItemConfig(newId);
        // if (product == null || newConfig == null)
        // {
        //     DebugUtil.LogError("SendProductBi  ERR  id"+id+" newId:"+newId);
        //     return;
        // }
        // GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        // {
        //     MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeBuildingEnergyCreate,
        //     itemAId = product.id,
        //     ItemALevel = product.level,
        //     itemBId = newConfig.id,
        //     itemBLevel = newConfig.level,
        //     isChange = true,
        //     extras = new Dictionary<string, string>
        //     {
        //         {"amount", leftCount.ToString()},
        //         {"is_auto", isAuto.ToString()},
        //         {"state", state},
        //         {"cd", cd.ToString()}
        //     }
        // });
    }

    public bool TryOpenBoxes(int index)
    {
        bool isOpenBox = false;
        int x = index % boardWidth;
        int y = index / boardWidth;
        for (int i = Mathf.Max(x - 1, 0); i <= Mathf.Min(x + 1, boardWidth - 1); i++)
        {
            index = i + y * boardWidth;
            isOpenBox = OpenBox(index);
        }

        for (int j = Mathf.Max(y - 1, 0); j <= Mathf.Min(y + 1, boardHeight - 1); j++)
        {
            index = x + j * boardWidth;
            isOpenBox = OpenBox(index);
        }

        return isOpenBox;
    }

    bool OpenBox(int index)
    {
        var storage = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
        if (storage.Id != -1 && MergeManager.Instance.IsBox(storage))
        {
            storage.State = storage.UnlockState;
            MergeManager.Instance.SetBoardItem(index, storage, RefreshItemSource.unlock,(MergeBoardEnum)_boardID);
            if (grids[index].board != null)
                grids[index].board.PlayBoxBreakAnimtor();
            return true;
        }

        return false;
    }

    public void Merge(int index, int id, int oldId, int productCount, RefreshItemSource source, int extraProduct = 0, int stack = 0)
    {
        if (_boardID != (int) MergeBoardEnum.HappyGo)
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_TASK_REFRESH, TaskType.MergerNum, 1, 1);
            EventDispatcher.Instance.DispatchEvent(EventEnum.BATTLE_PASS_2_TASK_REFRESH, TaskType.MergerNum, 1, 1);
        }

        DailyRankModel.Instance.AddScore(oldId, IndexToPosition(index));
        ShakeManager.Instance.ShakeLight();
        MergeManager.Instance.SetNewBoardItem(index, id, 1, source,(MergeBoardEnum)_boardID, oldId:oldId);
        MergeManager.Instance.SetProductCount(index, productCount,(MergeBoardEnum)_boardID);
        MergeManager.Instance.SetOrginalStoreCount(id, index, (MergeBoardEnum)_boardID,extraProduct, stack);
        var newItemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (newItemConfig != null && newItemConfig.isBuildingBag)
        {
            if (!MergeManager.Instance.GetStorageBoard((MergeBoardEnum)_boardID).MergeCounts.ContainsKey(id))
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventBuildingLevelChange,
                    newItemConfig.id.ToString(), newItemConfig.level.ToString());
            }
        }
        StopMergeAnimations();
        bool isOpenBox = TryOpenBoxes(index);
        bool isProductBubble = MergeToProdcutBubble(index, id, oldId);
        bool isProductXp = false;
        if (!isProductBubble)
            isProductXp = MergeToProdcutExp(index, id, oldId);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        var oldConfig = GameConfigManager.Instance.GetItemConfig(oldId);
        string soudsLevel = itemConfig.level > 15 ? "15" : itemConfig.level.ToString();
        if (itemConfig.next_level == 0)
            soudsLevel = "max";
        AudioManager.Instance.PlaySound("sfx_ui_merge_tone_" + soudsLevel);
        bool isChange = isProductXp || isProductBubble;

        int BiLine = 0;
        int level = 0;
        if (isProductBubble)
        {
            BiLine = itemConfig.in_line;
            level = itemConfig.level;
        }
        else
        {
            var activityItem = MergeToProductActivity(index,itemConfig,oldConfig);
            if (activityItem != null)
            {
                BiLine = activityItem.in_line;
                level = activityItem.level;
                isChange = true;
            }
        }

        if (isProductXp)
        {
            var xpConfig = GameConfigManager.Instance.GetItemConfig(10001);
            BiLine = xpConfig.in_line;
            level = xpConfig.level;
        }

        if (itemConfig.in_line == ClimbTreeModel._climbTreeBananaLineId)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyMergeBubble,itemConfig.level.ToString()); 
        }
        // GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        // {
        //     MergeEventType = BiEventCooking.Types.MergeEventType.MergeEventMerge,
        //     itemAId = oldConfig.id,
        //     ItemALevel = oldConfig.level,
        //     itemBId = itemConfig.id,
        //     itemBLevel = itemConfig.level,
        //     isLock = isOpenBox,
        //     extras = new Dictionary<string, string>
        //     {
        //         {"bonus", isChange.ToString()},
        //         {"merge_line", BiLine.ToString()},
        //         {"level", level.ToString()},
        //     },
        //     data1=_boardID.ToString()
        // });


        MergeFtueBiManager.Instance.SendFtueBi(MergeFtueBiManager.SendType.Merge, id);
    }

    private int GetBoxProductCount(int index1, int index2) // index2 是点击选中的item
    {
        int count = 0;
        var storageItem1 = MergeManager.Instance.GetBoardItem(index1,(MergeBoardEnum)_boardID);
        var storageItem2 = MergeManager.Instance.GetBoardItem(index2,(MergeBoardEnum)_boardID);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(storageItem1.Id);
        if (itemConfig == null)
            return count;
        if (itemConfig.type ==  (int) MergeItemType.box) //宝箱类型开启次数不能随合成重置
        {
            int count1 = storageItem1.ProductCount;
            int count2 = storageItem2.ProductCount;
            count = count1 + count2;
        }

        return count;
    }

    private int GetMergeItemLeftProductCount(int index1, int index2) // index2 是点击选中的item
    {
        int count = 0;
        var storageItem1 = MergeManager.Instance.GetBoardItem(index1,(MergeBoardEnum)_boardID);
        var storageItem2 = MergeManager.Instance.GetBoardItem(index2,(MergeBoardEnum)_boardID);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(storageItem1.Id);
        if (itemConfig == null)
            return count;
        if (MergeConfigManager.Instance.IsStoreItem(itemConfig))
        {
            if (itemConfig.output_rules_task != null && itemConfig.output_rules_task != 0 &&
                !MainOrderManager.Instance.IsCompleteOrder(itemConfig.output_rules_task))
                return 0;
            if (itemConfig.type == (int) MergeItemType.eatBuild)
                return 0;
            count = storageItem1.StoreMax + storageItem2.StoreMax;
            if (count <= 0)
                return 0;
        }

        return count;
    }

    void Swap(int srcIndex, int targetIndex)
    {
        MergeManager.Instance.SwapBoardItem(srcIndex, targetIndex,(MergeBoardEnum)_boardID);
        CommonUtils.Swap(grids, ref srcIndex, ref targetIndex);
        SwapBoardItemIndex(srcIndex, targetIndex);
        if (grids[targetIndex].id != -1) // 物件和物件交换
        {
            grids[targetIndex].board.transform.SetSiblingIndex(grids[srcIndex].board.transform.GetSiblingIndex() - 1);
            MoveHome(targetIndex);
        }

        MoveHome(srcIndex);
    }

    private void SwapBoardItemIndex(int srcIndex, int targetIndex)
    {
        if (srcIndex != -1)
            if (grids[srcIndex].board != null)
            {
                grids[srcIndex].board.index = srcIndex;
                grids[srcIndex].xIndex = srcIndex % boardWidth;
                grids[srcIndex].yIndex = srcIndex / boardWidth;
                grids[srcIndex].index = srcIndex;
            }
        if (targetIndex != -1)
            if (grids[targetIndex].board != null)
            {
                grids[targetIndex].board.index = targetIndex;
                grids[targetIndex].xIndex = targetIndex % boardWidth;
                grids[targetIndex].yIndex = targetIndex / boardWidth;
                grids[targetIndex].index = targetIndex;
            }
    }

    private void OnDisable()
    {
        mergeTipList.Clear();

        if (_dealyAnim != null)
            StopCoroutine(_dealyAnim);
        _dealyAnim = null;

        if (_dealyGuide != null)
            StopCoroutine(_dealyGuide);
        _dealyGuide = null;

        CancelInvoke("RefreshAllProduct");
    }

    protected virtual void OnEnable()
    {
        foreach (var grid in grids)
        {
            if(grid.board != null)
                grid.board.StopTweenAnim();
        }
        mergeTipList.Clear();
        AutoTips();
        InvokeRepeating("RefreshAllProduct", 1f, 1);
    }

    public void StopAllTweenAnim()
    {
        foreach (var grid in grids)
        {
            if(grid.board != null)
                grid.board.StopTweenAnim();
        }
    }


    public void AddEventObject(RectTransform rect)
    {
        eventObjectList.Add(rect);
    }

    GameObject GetEventObject(Vector2 screenPoint)
    {
        foreach (RectTransform eventObject in eventObjectList)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(eventObject, screenPoint, UIRoot.Instance.mUICamera))
            {
                return eventObject.gameObject;
            }
        }

        return mFullShield.gameObject;
    }

    void OnPointerDown(BaseEventData baseEventData)
    {
        _baseEventData = baseEventData;
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        if (firstEventGo != null)
        {
            if (firstEventGo == mContent.gameObject)
            {
                OnDragBoard(pointerEventData);
            }

            return;
        }

        firstEventGo = GetNextEventObject(pointerEventData);
        if (firstEventGo == mContent.gameObject)
        {
            OnPointerDownBoard(pointerEventData);
        }
        else
        {
            ExecuteEvents.Execute(firstEventGo, pointerEventData, ExecuteEvents.pointerDownHandler);
        }
    }

    void OnPointerDownBoard(PointerEventData pointerEventData)
    {
        downPos = transform.InverseTransformPoint(pointerEventData.pointerCurrentRaycast.worldPosition);
        int index = PosToIndex(downPos);

        // if (GuideSubSystem.Instance.InNewPlayerGuideChain)
        //     return;
        if (GuideSubSystem.Instance.MergeStartIndex() > 0)
        {
            if(index != GuideSubSystem.Instance.MergeStartIndex())
                return;
        }
        
        if (index == -1) // 棋牌外
        {
            return;
        }

        if (grids[index].id == -1) // 空格
        {
            return;
        }

        if (Time.time < grids[index].cd) // CD中
        {
            return;
        }

        if (grids[index].state == -1)//锁定的格子
        {
            return;
        }
        
        if (grids[index].board != null && grids[index].board.tableMergeItem != null && grids[index].board.tableMergeItem.subType == (int)SubType.Matreshkas)
        {
            if(!MatreshkasModel.Instance.IsOpened())
                return;
        }
        
        activeIndex = index;
        dragLock = true;
        grids[activeIndex].board.transform.SetAsLastSibling();
        grids[activeIndex].board.RefreshHamsterBubble();
        SetAnimationsObjLastSibling();
        
        CancelTip(-1, true);
        
        MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Board, (MergeBoardEnum)_boardID,true);
    }

    void OnPointerUp(BaseEventData baseEventData)
    {
        if (lastIndex > 0)
        {
            SetMergeStatus(lastIndex, false);
        }

        if (firstEventGo == null)
        {
            _baseEventData = null;
            isDraging = false;
            lastIndex = -1;
            return;
        }

        PointerEventData pointerEventData = baseEventData as PointerEventData;

        if (firstEventGo == mContent.gameObject)
        {
            UpdateSibling(true);
            OnPointerUpBoard(pointerEventData);
            if (_boardID == (int) MergeBoardEnum.HappyGo)
            {
                HappyGoMergeGuideLogic.Instance.CheckMergeGuide(true);
            }
            else if(_boardID == (int) MergeBoardEnum.Stimulate)
            {
                StimulateGuideLogic.Instance.GuideLogic(true);
            }
            else if (_boardID == (int)MergeBoardEnum.Filthy)
            {
                FilthyGuideLogic.Instance.GuideLogic(true);
            }
            else
            {
                MergeGuideLogic.Instance.CheckMergeGuide(true);
            }
        
        }
        else
        {
            ExecuteEvents.Execute(firstEventGo, pointerEventData, ExecuteEvents.pointerUpHandler);
            if (firstDragGo != null)
            {
                if (_baseEventData != null)
                {
                    OnEndDrag(_baseEventData);
                    _baseEventData = null;
                }
            }

            if (GetNextEventObject(pointerEventData) == firstEventGo && firstDragGo == null)
            {
                ExecuteEvents.Execute(firstEventGo, pointerEventData, ExecuteEvents.pointerClickHandler);
            }
        }

        _baseEventData = null;
        firstEventGo = null;
        isDraging = false;
        lastIndex = -1;
    }

    bool isMerge = false;

    void OnPointerUpBoard(PointerEventData pointerEventData)
    {
        isMerge = false;
        dragTweener?.Kill(true);
        if (activeIndex == -1)
        {
            return;
        }

        Vector3 pos = transform.InverseTransformPoint(pointerEventData.pointerCurrentRaycast.worldPosition);
        int index = activeIndex;
        if (lastIndex > 0)
            index = lastIndex;
        else
        {
            if (isDraging)
                index = FindSameIndex(pos, activeIndex);
            else
                index = PosToIndex(pos);
        }

        GameObject eventObject = GetEventObject(pointerEventData.position);
        if (eventObject == MergeMainController.Instance.bagTrans.gameObject)
            index = -1;

        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_POINTERUP, eventObject);
        // 再判断物件是否存在
        if (grids[activeIndex].id == -1)
        {
            if (selectIndex < 0 || grids[selectIndex].id == -1)
                return;

            index = selectIndex;
            activeIndex = selectIndex;
        }

        focusIndex = activeIndex;

        if (GuideSubSystem.Instance.MergeStartIndex() > 0)
        {
            if (index != GuideSubSystem.Instance.MergeEndIndex())
            {
                if (!IsBox())
                {
                    if ((IsGridUnlock() || !IsGridUnlock() && dragLock))
                        SelectFocus(focusIndex, isMerge, true, dragLock);
                }
                if (focusIndex != -1)
                    RefreshGridState(focusIndex);
                
                MoveHome(activeIndex);
                activeIndex = -1;
                return;
            }
        }
        
        do
        {
            if (index == -1) // 棋盘外
            {
                MoveHome(activeIndex);
                break;
            }

            var itemStorage = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);

            if (index == activeIndex) // 起落同一个格子
            {
                if (!dragLock) // 拖拽过
                {
                    MoveHome(activeIndex);
                }

                if (activeIndex == selectIndex) // 第二次点击
                {
                    if (!GuideSubSystem.Instance.CanProduct())
                        break;

                    if (grids[index].board != null)
                        grids[index].board.SetTapImageActive(false);

                    //eatbuild 异常处理修复
                    if (grids[index].board.tableMergeItem.type == (int)MergeItemType.eatBuild)
                    {
                        if (MergeManager.Instance.IsActiveItem(index, (MergeBoardEnum)_boardID))
                        {
                            if (MergeManager.Instance.IsEatAllFood(grids[index].board.storageMergeItem))
                            {
                                grids[index].board.storageMergeItem.State = 1;
                                grids[index].board.storageMergeItem.StoreMax = grids[index].board.tableMergeItem.output_amount;
                                
                                RefreshOneProduct(index);
                            }
                        }
                    }
                    var ignoreUse = OperateItem(index); //建筑产出
                    if (grids[index].board != null)
                    {
                        var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
                        if (itemConfig == null)
                            throw new Exception("道具物品id出错：" + grids[index].id);
                        if (itemConfig.type == (int)MergeItemType.timeBooster)
                        {
                            StartCoroutine(PlayTimeSpeedAnimation(index));
                        }
                        if (!ignoreUse)
                            grids[index].board.DoUseItem(index); //获得道具  金币 体力 钻石
                    }
                }
            }
            else if (Time.time < grids[index].cd) // CD中
            {
                MoveHome(activeIndex);
            }
            else if (grids[activeIndex].id == grids[index].id) // 相同物件
            {
                var activeStorage = MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID);

                if (grids[index].board == null)
                {
                    MoveHome(activeIndex);
                    break;
                }

                var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
                if (itemConfig.type==(int)MergeItemType.box )
                {
                    var storageItem1 = MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID);
                    var storageItem2 = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                    if(storageItem1.ProductCount>0 || storageItem2.ProductCount>0)
                    {
                        Swap(activeIndex, index);
                        break;
                    }
                }
                if (MergeManager.Instance.IsBubble(index,(MergeBoardEnum)_boardID) || MergeManager.Instance.IsBubble(activeStorage))
                {
                    if (!MergeManager.Instance.IsOpen(activeStorage))
                    {
                        MoveHome(activeIndex);
                        break;
                    }

                    focusIndex = index;
                    Swap(activeIndex, index);
                    break;
                }

                if (MergeManager.Instance.IsBox(itemStorage) ||
                    !MergeManager.Instance.IsOpen(activeStorage)) // 未开启 或者锁气泡
                {
                    MoveHome(activeIndex);
                    break;
                }

                var config = GameConfigManager.Instance.GetItemConfig(grids[index].id);
                int upperId = GameConfigManager.Instance.GetItemConfig(grids[index].id).next_level;
                if (GameConfigManager.Instance.GetItemConfig(upperId) != null) // 可合成
                {
                    isMerge = true;
                    focusIndex = index;
                    int count = GetBoxProductCount(index, activeIndex);

                    int extraProduct = GetMergeItemLeftProductCount(index, activeIndex);

                    int oldId = grids[index].id;
                    MergeManager.Instance.SetBoardItem(activeIndex, -1, RefreshItemSource.notDeal,(MergeBoardEnum)_boardID);
                    Merge(index, upperId, grids[index].id, count, RefreshItemSource.mergeOk, extraProduct);

                    if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.MergeItem))
                    {
                        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MergeItem, oldId.ToString());
                    }
                }
                else if (config.canStacking)//可堆叠的
                {
                    isMerge = true;
                    focusIndex = index;
                    var storageItem1 = MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID);
                    var storageItem2 = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                    var count = storageItem1.StackNum + storageItem2.StackNum;
                    MergeManager.Instance.SetBoardItem(activeIndex, -1, RefreshItemSource.notDeal,(MergeBoardEnum)_boardID);
                    Merge(index, config.id, grids[index].id,0,RefreshItemSource.mergeOk,0,count);

                }
                else
                {
                    focusIndex = index;
                    Swap(activeIndex, index);
                    //MoveHome(activeIndex);
                }
            }
            else if (MergeConfigManager.Instance.IsOmnipoten(grids[activeIndex].id)) //万能物品
            {
                bool isBox = MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID);
               
                var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
                // var omnipotenItemConfig = GameConfigManager.Instance.GetItemConfig(grids[activeIndex].id);
                // var omnipotenFactor = omnipotenItemConfig?.booster_factor;
                // var level = itemConfig?.level;
                if (isBox || itemConfig == null ||itemConfig.next_level==-1 ||MergeManager.Instance.IsLockWeb(index,(MergeBoardEnum)_boardID)|| MergeManager.Instance.IsBubble(index,(MergeBoardEnum)_boardID) ||MergeConfigManager.Instance.IsTimeStoreItem(itemConfig)|| !MergeConfigManager.Instance.IsCanOmnipoten(itemConfig)  || MergeConfigManager.Instance.IsCanProductItem(itemConfig))
                {
                    if (itemConfig == null)
                    {
                        focusIndex = index;
                        if (IsGridUnlock())
                        {
                            Swap(activeIndex, index);
                        }
                    }
                    else
                    {
                        MoveHome(activeIndex);
                        break;
                    }
                }
                else
                {
                    int upperId = itemConfig.next_level;
                    int nowActiveIndex = activeIndex;
                    if (GameConfigManager.Instance.GetItemConfig(upperId) != null) // 可合成
                    {
                        var window =  UIManager.Instance.OpenUI(UINameConst.UIPopupMergeIncreaseLevel) as UIPopupMergeIncreaseLevelController;
                        window.SetItemId(grids[index].id,grids[activeIndex].id);
                        window.OnConfirm = () =>
                        {
                            isMerge = true;
                            focusIndex = index;
                            int count = GetBoxProductCount(index,nowActiveIndex);
                       
                            Merge(index, upperId, grids[index].id,count,RefreshItemSource.mergeOk_omnipoten);
                            
                            var boardItem = MergeManager.Instance.GetBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID);
                            if (boardItem.StackNum <= 1)
                            {
                                MergeManager.Instance.SetBoardItem(nowActiveIndex, -1, RefreshItemSource.notDeal,(MergeBoardEnum)_boardID);
                                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,(MergeBoardEnum)_boardID);
                            }
                            else
                            {
                                boardItem.StackNum--;
                                MoveHome(nowActiveIndex);
                                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, new Vector2Int(nowActiveIndex, grids[nowActiveIndex].id),(MergeBoardEnum)_boardID);
                            }
                            // GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            // {
                            //     MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonUniversalUse,
                            //     itemAId = itemConfig.id,
                            //     ItemALevel = itemConfig.level,
                            //     isChange = true,
                            //     data1=boardItem.BoosterFactor.ToString()
                            // });

                        };
                        window.OnDismiss = () =>
                        {
                             MoveHome(nowActiveIndex);
                        };
                    }
                    else
                    {
                        MoveHome(activeIndex);
                    }
                }
            }
              
            else if (MergeConfigManager.Instance.IsMagicWand(grids[activeIndex].id)) //魔杖
            {
                bool isBox = MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID);
                int gridIndex = MergeManager.Instance.FindEmptyGrid(1,(MergeBoardEnum)_boardID);
                bool isOpen = MergeManager.Instance.IsOpen(index,(MergeBoardEnum)_boardID);
                var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
                if (isBox ||gridIndex==-1|| itemConfig == null ||MergeManager.Instance.IsLockWeb(index,(MergeBoardEnum)_boardID)|| MergeManager.Instance.IsBubble(index,(MergeBoardEnum)_boardID) ||MergeConfigManager.Instance.IsTimeStoreItem(itemConfig)|| !MergeConfigManager.Instance.IsCanOmnipoten(itemConfig)  || MergeConfigManager.Instance.IsCanProductItem(itemConfig))
                {
                    if (itemConfig == null)
                    {
                        focusIndex = index;
                        if (IsGridUnlock())
                        {
                            Swap(activeIndex, index);
                        }
                    }
                    else
                    {
                        if(isOpen && MergeConfigManager.Instance.IsMagicWand(grids[activeIndex].id) && gridIndex == -1)
                            MergePromptManager.Instance.ShowTextPrompt(IndexToPosition(index));
                        MoveHome(activeIndex);
                        break;
                    }
                }
                else
                {
                    
                    int nowActiveIndex = activeIndex;
                    var window =  UIManager.Instance.OpenUI(UINameConst.UIPopupMergeCopy) as UIPopupMergeCopyController;
                    window.SetItemId(grids[index].id,grids[activeIndex].id);
                    window.OnConfirm = () =>
                    {
                        isMerge = true;
                        focusIndex = index;
                        MergeManager.Instance.SetNewBoardItem(gridIndex, itemConfig.id, 1, RefreshItemSource.product,(MergeBoardEnum)_boardID, index);
                        var boardItem = MergeManager.Instance.GetBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID);
                        if (boardItem.StackNum <= 1)
                        {
                            MergeManager.Instance.RemoveBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID, "MagicWand");
                            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,(MergeBoardEnum)_boardID);
                        }
                        else
                        {
                            boardItem.StackNum--;
                            MoveHome(nowActiveIndex);
                            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, new Vector2Int(nowActiveIndex, grids[nowActiveIndex].id),(MergeBoardEnum)_boardID);
                        }
                    };
                    window.OnDismiss = () =>
                    {
                         MoveHome(nowActiveIndex);
                    };
                }
            }
            else if (MergeConfigManager.Instance.IsSplit(grids[activeIndex].id)) //分解物品
            {
                bool isBox = MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID);
                var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
                  
                bool isOpen = MergeManager.Instance.IsOpen(index,(MergeBoardEnum)_boardID);
                int gridIndex = MergeManager.Instance.FindEmptyGrid(1,(MergeBoardEnum)_boardID);
             
                if (!isOpen||isBox ||gridIndex == -1 || itemConfig == null || MergeManager.Instance.IsBubble(index,(MergeBoardEnum)_boardID) || !MergeConfigManager.Instance.IsCanSplit(grids[index].id)||MergeConfigManager.Instance.IsTimeStoreItem(itemConfig)|| MergeConfigManager.Instance.IsCanProductItem(itemConfig))
                {
                    if (itemConfig == null)
                    {
                        focusIndex = index;
                        if (IsGridUnlock())
                        {
                            Swap(activeIndex, index);
                        }
                    }
                    else
                    {
                        if(isOpen && MergeConfigManager.Instance.IsCanSplit(grids[index].id) && gridIndex == -1)
                            MergePromptManager.Instance.ShowTextPrompt(IndexToPosition(index));
                            
                        MoveHome(activeIndex);
                        break;
                    }
                }
                else
                {
                    int upperId = itemConfig.pre_level;
                    int nowActiveIndex = activeIndex;
                    if (GameConfigManager.Instance.GetItemConfig(upperId) != null) // 可分解
                    {
                        var window =  UIManager.Instance.OpenUI(UINameConst.UIPopupMergeInSplitLevel) as UIPopupMergeInSplitLevelController;
                        window.SetItemId(grids[index].id,grids[activeIndex].id);
                        window.OnConfirm = () =>
                        {
                            var boardItem = MergeManager.Instance.GetBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID);
                            if (boardItem.StackNum <= 1)
                            {
                                MergeManager.Instance.RemoveBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID, "Split");
                            }
                            else
                            {
                                boardItem.StackNum--;
                                MoveHome(nowActiveIndex);
                            }
                            grids[index]?.board.PlaySpilteVfx();
                            AudioManager.Instance.PlaySound(170);
                            grids[index]?.board.SpliteItem();
                            // GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            // {
                            //     MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonScissorsUse,
                            //     itemAId = itemConfig.id,
                            //     ItemALevel = itemConfig.level,
                            //     isChange = true,
                            //     data1=boardItem.BoosterFactor.ToString()
                            // });
                            
                            if(boardItem.StackNum > 0)
                                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, new Vector2Int(nowActiveIndex, grids[nowActiveIndex].id),(MergeBoardEnum)_boardID);
                        };
                        window.OnDismiss = () =>
                        {
                            MoveHome(nowActiveIndex);
                        };
                    }
                    else
                    {
                        MoveHome(activeIndex);
                    }
                    
                }
            }
            
            else if (grids[index].id != -1 && !MergeManager.Instance.IsUnlock(itemStorage) && !MergeManager.Instance.IsBubble(itemStorage)) // 扫把 有未解锁(未开启)物件占位 并且不是气泡
            {
                if (grids[activeIndex].board != null && grids[activeIndex].board.tableMergeItem.type == 16)
                {
                    if (MergeManager.Instance.IsLockWeb(index,(MergeBoardEnum)_boardID))
                    {
                        int nowActiveIndex = activeIndex;
                        var boardItem = MergeManager.Instance.GetBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID);
                        if (boardItem.StackNum <= 1)
                        {
                            MergeManager.Instance.RemoveBoardItem(activeIndex,(MergeBoardEnum)_boardID,"Broom");
                        }
                        else
                        {
                            boardItem.StackNum--;
                            MoveHome(nowActiveIndex);
                        }
                        
                        MergeManager.Instance.SetBoardItem(index, grids[index].id, 1, RefreshItemSource.webUnlock,(MergeBoardEnum)_boardID);
                        TryOpenBoxes(index);
                        GetGridByIndex(index).board.PlayAnimator("Lock_Open", true);
                        
                        AutoTips();
                        focusIndex = index;
                    }
                    else if(MergeManager.Instance.IsUnlock(itemStorage))
                    {
                        focusIndex = index;
                        if (IsGridUnlock())
                        {
                            Swap(activeIndex, index);
                        }
                    }
                    else
                    {
                        focusIndex = activeIndex;
                        MoveHome(activeIndex);
                    }
                }
                else
                {
                    focusIndex = activeIndex;
                    MoveHome(activeIndex);
                }
            }
            else if (grids[index].id != -1 && !MergeManager.Instance.IsUnlock(itemStorage) &&
                     !MergeManager.Instance.IsBubble(itemStorage)) // 有未解锁(未开启)物件占位 并且不是气泡
            {
                focusIndex = activeIndex;
                MoveHome(activeIndex);
            }
            // else if (_boardID != (int)MergeBoardEnum.Stimulate  && _boardID != (int)MergeBoardEnum.Filthy &&  StorageManager.Instance.GetStorage<StorageHome>().Level == 1)//等级低于1级时不让换位置
            // {
            //     focusIndex = activeIndex;
            //     MoveHome(activeIndex);
            // }
            else // 交换位置(包含物件和物件，物件和空格的交换)
            {
                if (itemStorage != null && itemStorage.Id != -1 && grids[index].id == -1)
                {
                    focusIndex = activeIndex;
                    MoveHome(activeIndex);
                }
                else if (grids[index].id != -1 && grids[index].board == null)
                {
                    focusIndex = activeIndex;
                    MoveHome(activeIndex);
                }
                else if (grids[index].isAnim)
                {
                    focusIndex = activeIndex;
                    MoveHome(activeIndex);
                } 
                else if (grids[index].board != null && grids[index].board.tableMergeItem.type == (int)MergeItemType.eatBuild) 
                {
                    if (MergeManager.Instance.IsActiveItem(index,(MergeBoardEnum)_boardID) && MergeManager.Instance.IsUnlock(MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID)))
                    {
                        var hpMergeItem = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                        var produceCost = MergeManager.Instance.GetProduceCost(grids[index].board.tableMergeItem);
                        if (produceCost!=null &&produceCost.Contains(grids[activeIndex].id) && !MergeManager.Instance.IsBuildEat(hpMergeItem,grids[activeIndex].id))
                        {
                            int id = grids[activeIndex].id;
                            MergeManager.Instance.BuildEat(hpMergeItem,grids[activeIndex].id);
                            MergeManager.Instance.RemoveBoardItem(activeIndex,(MergeBoardEnum)_boardID,"EatBuild");
                            grids[index]?.board.PlayEatMergeAni();
                            focusIndex = index;
                            grids[index]?.board.RefreshSlider();
                            if (MergeManager.Instance.IsEatAllFood(hpMergeItem))
                            {
                                hpMergeItem.State = 1;
                                hpMergeItem.InCdTime = APIManager.Instance.GetServerTime() / 1000;
                                hpMergeItem.ProductTime = APIManager.Instance.GetServerTime() / 1000;
                                hpMergeItem.StoreMax = 0;

                                if (grids[index].board.tableMergeItem.isIgnoreCd)
                                {
                                    hpMergeItem.StoreMax = grids[index].board.tableMergeItem.output_amount;
                                }
                                RefreshOneProduct(index);
                                AutoTips();
                                grids[index]?.board.UpdateIconImage();
                            }
                          
                            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.EatFood, id.ToString());
                           
                        }
                        else
                        {
                            focusIndex = activeIndex;
                            MoveHome(activeIndex);
                        }
                    }
                    else
                    {
                        if (grids[index].board.tableMergeItem.subType == (int)SubType.Matreshkas)
                        {
                            if (!MatreshkasModel.Instance.IsOpened())
                            {
                                focusIndex = activeIndex;
                                MoveHome(activeIndex);
                                break;
                            }
                        }
                        if (grids[activeIndex].board != null && grids[activeIndex].board.tableMergeItem.type == (int)MergeItemType.SpeedUp) //单体加速
                        {
                            if (grids[index] == null || grids[index].board == null)
                            {
                                focusIndex = index;
                                if (IsGridUnlock())
                                {
                                    Swap(activeIndex, index);
                                }
                                return;
                            }
                       
                            bool isTimeProductItem = MergeConfigManager.Instance.IsTimeProductItem(grids[index].board.tableMergeItem);
                            bool isProductItem = MergeConfigManager.Instance.IsProductItem(grids[index].board.tableMergeItem);
                            bool isCanSpeedup = false;
                            if (isProductItem || isTimeProductItem)
                            {
                                float productPercent = 0;
                                int cdTime = MergeManager.Instance.GetLeftProductTime(index, grids[index].board.tableMergeItem, ref productPercent,(MergeBoardEnum)_boardID);
                                if (cdTime > 0)
                                    isCanSpeedup= true;

                                cdTime = MergeManager.Instance.GetLeftActiveTime(grids[index].board.storageMergeItem, grids[index].board.tableMergeItem, ref productPercent);
                                if (cdTime > 0)
                                    isCanSpeedup= true;

                                if (isTimeProductItem)
                                {
                                    cdTime = MergeManager.Instance.GetTimeProductCd(index, grids[index].board.tableMergeItem, ref productPercent,(MergeBoardEnum)_boardID);
                                    if (cdTime > 0 && grids[index].board.storageMergeItem.ProductItems.Count <= 0)
                                        isCanSpeedup= true;
                                }
                                if(isCanSpeedup)
                                {
                                    int nowActiveIndex = activeIndex;
                                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                                    {
                                        DescString = LocalizationManager.Instance.GetLocalizedString("ui_booster_desc"),
                                        HasCancelButton = true,
                                        OKCallback = () =>
                                        {
                                            var item = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                                            grids[index].board.PlaySpeedUpAnimator(false);
                                            AudioManager.Instance.PlaySound(41);
                                            MergeManager.Instance.SpeedUpOneItem(item,(ulong)grids[nowActiveIndex].board.tableMergeItem.booster_factor*60);
                                            MergeManager.Instance.RemoveBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID,"speed");
                                        },
                                        CancelCallback = () =>
                                        {
                                            MoveHome(nowActiveIndex);
                                        }
                                    });
                                }
                                else
                                {
                                    focusIndex = index;
                                    if (IsGridUnlock())
                                    {
                                        Swap(activeIndex, index);
                                    }
                                }
                            }
                            else
                            {
                                focusIndex = index;
                                if (IsGridUnlock())
                                {
                                    Swap(activeIndex, index);
                                }
                            }
                        }
                        else
                        {
                            focusIndex = index;
                            if (IsGridUnlock())
                            {
                                Swap(activeIndex, index);
                            }   
                        }
                    }
                }
                else if (grids[index].board != null && grids[index].board.tableMergeItem.type == (int)MergeItemType.hamaster) //仓鼠
                {
                    if (MergeManager.Instance.IsActiveItem(index,(MergeBoardEnum)_boardID) && MergeManager.Instance.IsUnlock(MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID)))
                    {
                        if (HappyGoModel.Instance.GetRequestProductId() == grids[activeIndex].id)
                        {
                            MergeManager.Instance.RemoveBoardItem(activeIndex,(MergeBoardEnum)_boardID,"HappyGo");
                            var hpMergeItem = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                            hpMergeItem.State = 1;
                            MergeManager.Instance.SetOrginalStoreCount(hpMergeItem);
                            RefreshOneProduct(index);
                            focusIndex = index;
                            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.HappyGoEat);
                            AutoTips();
                            HappyGoModel.Instance.AddRequestCount();
                            grids[index]?.board.PlayEatMergeAni();
                        }
                        else
                        {
                            focusIndex = activeIndex;
                            MoveHome(activeIndex);
                        }
                    }
                    else
                    {
                        focusIndex = index;
                        if (IsGridUnlock())
                        {
                            Swap(activeIndex, index);
                        }
                    }
                }
                else if (grids[activeIndex].board != null && grids[activeIndex].board.tableMergeItem.type == (int)MergeItemType.SpeedUp) //单体加速
                {
                    if (grids[index] == null || grids[index].board == null)
                    {
                        focusIndex = index;
                        if (IsGridUnlock())
                        {
                            Swap(activeIndex, index);
                        }
                        return;
                    }
               
                    bool isTimeProductItem = MergeConfigManager.Instance.IsTimeProductItem(grids[index].board.tableMergeItem);
                    bool isProductItem = MergeConfigManager.Instance.IsProductItem(grids[index].board.tableMergeItem);
                    bool isCanSpeedup = false;
                    if (isProductItem || isTimeProductItem)
                    {
                        float productPercent = 0;
                        int cdTime = MergeManager.Instance.GetLeftProductTime(index, grids[index].board.tableMergeItem, ref productPercent,(MergeBoardEnum)_boardID);
                        if (cdTime > 0)
                            isCanSpeedup= true;

                        cdTime = MergeManager.Instance.GetLeftActiveTime(grids[index].board.storageMergeItem, grids[index].board.tableMergeItem, ref productPercent);
                        if (cdTime > 0)
                            isCanSpeedup= true;

                        if (isTimeProductItem)
                        {
                            cdTime = MergeManager.Instance.GetTimeProductCd(index, grids[index].board.tableMergeItem, ref productPercent,(MergeBoardEnum)_boardID);
                            if (cdTime > 0 && grids[index].board.storageMergeItem.ProductItems.Count <= 0)
                                isCanSpeedup= true;
                        }
                        if(isCanSpeedup)
                        {
                            int nowActiveIndex = activeIndex;
                            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                            {
                                DescString = LocalizationManager.Instance.GetLocalizedString("ui_booster_desc"),
                                HasCancelButton = true,
                                OKCallback = () =>
                                {
                                    var item = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                                    grids[index].board.PlaySpeedUpAnimator(false);
                                    AudioManager.Instance.PlaySound(41);
                                    MergeManager.Instance.SpeedUpOneItem(item,(ulong)grids[nowActiveIndex].board.tableMergeItem.booster_factor*60);
                                    MergeManager.Instance.RemoveBoardItem(nowActiveIndex,(MergeBoardEnum)_boardID,"speed");
                                },
                                CancelCallback = () =>
                                {
                                    MoveHome(nowActiveIndex);
                                }
                            });
                        }
                        else
                        {
                            focusIndex = index;
                            if (IsGridUnlock())
                            {
                                Swap(activeIndex, index);
                            }
                        }
                    }
                    else
                    {
                        focusIndex = index;
                        if (IsGridUnlock())
                        {
                            Swap(activeIndex, index);
                        }
                    }
                }
                else
                {
                    focusIndex = index;
                    if (IsGridUnlock())
                    {
                        Swap(activeIndex, index);
                    }
                }
            }
        } while (false);

        if (!IsBox())
        {
            if ((IsGridUnlock() || !IsGridUnlock() && dragLock))
                SelectFocus(focusIndex, isMerge, true, dragLock);
        }
        if (focusIndex != -1)
            RefreshGridState(focusIndex);
        
        AutoTips();
        
        activeIndex = -1;
    }


    private bool IsBox()
    {
        var itemStorage = MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID);
        return MergeManager.Instance.IsBox(itemStorage);
    }
    private bool IsGridUnlock()
    {
        bool result = true;
        var itemStorage = MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID);

        if (!MergeManager.Instance.IsOpen(itemStorage) && itemStorage.State != (int) MergeItemStatus.bubble) // 未开启
        {
            result = false;
        }

        if (!MergeManager.Instance.IsUnlock(itemStorage) && itemStorage.State != (int) MergeItemStatus.bubble) // 未解锁
        {
            result = false;
        }

        return result;
    }

    void OnBeginDrag(BaseEventData baseEventData)
    {
        if (firstEventGo == null || firstDragGo != null)
        {
            return;
        }

        if (activeIndex != -1 && grids[activeIndex].board!=null)
        {
            grids[activeIndex].board.SetTaskStatus(false, false);
            grids[activeIndex].board.SetGarageCleanupStatus(false);
        }
        _baseEventData = baseEventData;
        firstDragGo = firstEventGo;
        ExecuteEvents.ExecuteHierarchy(firstDragGo, baseEventData, ExecuteEvents.beginDragHandler);
    }

    void OnEndDrag(BaseEventData baseEventData)
    {
        if (firstEventGo != null || firstDragGo == null)
        {
            return;
        }
      
        ExecuteEvents.ExecuteHierarchy(firstDragGo, baseEventData, ExecuteEvents.endDragHandler);
        firstDragGo = null;
    }

    void OnDrag(BaseEventData baseEventData)
    {
        _baseEventData = baseEventData;
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        if (firstDragGo == mContent.gameObject)
        {
            OnDragBoard(pointerEventData);
            if (GuideSubSystem.Instance.IsShowingGuide())
            {
                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.MergeItem) ||
                    GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ProductBuild)||
                    GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.HappyGoEat)||
                    GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.EatFood))
                {
                    GuideSubSystem.Instance.HideCurrent();
                }
            }
        }
        else
        {
            ExecuteEvents.ExecuteHierarchy(firstDragGo, pointerEventData, ExecuteEvents.dragHandler);
        }
    }

    Vector3 pos;
    Vector2 screenPoint = Vector2.zero;
    void OnDragBoard(PointerEventData pointerEventData)
    {
        isDraging = true;
        //判断是否已解锁
        if (activeIndex == -1)
            return;

        if (!IsGridUnlock())
        {
            if (selectIndex != activeIndex&& !IsBox())
                SelectFocus(activeIndex);
            return;
        }

        if (grids[activeIndex].board != null && grids[activeIndex].board.tableMergeItem != null && grids[activeIndex].board.tableMergeItem.subType == (int)SubType.Matreshkas)
        {
            if(!MatreshkasModel.Instance.IsOpened())
                return;
        }

        screenPoint = pointerEventData.position;
        pos = mContent.InverseTransformPoint(pointerEventData.pointerCurrentRaycast.worldPosition);

        if (dragLock)
        {
            if (Mathf.Abs(pos.x - downPos.x) < gridSize.x * 0.2f && Mathf.Abs(pos.y - downPos.y) < gridSize.y * 0.2f)
            {
                return;
            }
            else
            {
                dragLock = false;
            }
        }

        dragTweener?.Kill();
        if (Mathf.Abs(pos.z) < 1 && Mathf.Abs(dragPos.z) < 1)
        {
            if (grids[activeIndex].board != null)
            {
                dragTweener = grids[activeIndex].board.transform.DOLocalMove(pos, 0.1f);
            }
        }
        else
        {
            if (grids[activeIndex].board != null)
            {
                grids[activeIndex].board.transform.localPosition = pos;
            }
        }

        dragPos = pos;

        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_DRAGIN, GetEventObject(screenPoint));
        int index = FindSameIndex(pos, activeIndex);
        MergeAnimator(index);

        SelectFocus(-1);
        
        if (grids[activeIndex].board != null)
        {
            grids[activeIndex].board.transform.SetAsLastSibling();
        }
    }

    GameObject GetNextEventObject(PointerEventData pointerEventData)
    {
        GameObject go = null;
        if (EventSystem.current == null)
            return go;
        EventSystem.current.RaycastAll(pointerEventData, tempRaycastResults);
        GameObject current = pointerEventData.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < tempRaycastResults.Count; i++)
        {
            if (current != tempRaycastResults[i].gameObject)
            {
                go = tempRaycastResults[i].gameObject;
                break;
            }
        }

        tempRaycastResults.Clear();
        return go;
    }

    private void RefreshAllProduct()
    {
        for (int i = 0; i < grids.Length; i++)
        {
            Grid grid = grids[i];
            TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(grid.id);
            if (itemConfig == null)
                continue;

            bool isOpen = MergeManager.Instance.IsOpen(i,(MergeBoardEnum)_boardID);
            bool isProductItem = false;
            if (itemConfig != null)
                isProductItem = MergeConfigManager.Instance.IsTimeProductItem(itemConfig);

            if (isOpen && isProductItem)
            {
                TimeProductItem(i, itemConfig);
                FreeTimeProductItem(i);
            }

            MergeManager.Instance.RefreshMaxStoreCount(grids[i].id, i, itemConfig,(MergeBoardEnum)_boardID);
        }
    }

    private void RefreshOneProduct(int index)
    {
        grids[index].board?.RefreshAllStatus();
    }

    private void RefreshEnergyTorrendState(BaseEvent e)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            grids[i].board?.UpdateData();
        }
    }
 

    private void RefreshUnlimitedProductState(BaseEvent e)
    {
        for (int i = 0; i < grids.Length; i++)
        {
            grids[i].board?.RefreshAllStatus();
        }
    }

    private void TimeProductItem(int index, TableMergeItem itemConfig) // 时间产出建筑
    {
        if (itemConfig == null)
            return;

        Grid grid = grids[index];
        
        bool isMax = MergeManager.Instance.IsTimeProductMaxCount(index, itemConfig,(MergeBoardEnum)_boardID);
        if (isMax)
        {
            //重置时间CD
            grid.board.storageMergeItem.TimProductTime = APIManager.Instance.GetServerTime() / 1000;
        }
        
        // DebugUtil.LogError("时间产出建筑 剩余产出次数:" + MergeManager.Instance.GetTimeProductTimes(index) + "---是否产出队列已满：" + isMax);
        if (!isMax && !MergeManager.Instance.IsTimeProductInCD(index,(MergeBoardEnum)_boardID)) // 目前只支持 产出一个
        {
            //产出的时候清理看RV次数
            grid.board.storageMergeItem.PlayRvNum = 0;
            var timeProductCount = MergeManager.Instance.GetTimeProductCount(index,(MergeBoardEnum)_boardID);
            for (int i = 0; i < timeProductCount; i++)
            {
                int outputId = MergeConfigManager.Instance.GetOneTimeOutput(itemConfig);
                int pos = MergeManager.Instance.GetSudokuEmptIndex(index,(MergeBoardEnum)_boardID);
                if (pos == -1) // 九宫格没有位置了, 暂存到队列中
                {
                    MergeManager.Instance.RecordTimeProductItem(index, outputId, itemConfig,(MergeBoardEnum)_boardID);

                    DebugUtil.Log("九宫格没有位置了, 暂存到队列中:" + outputId);
                }
                else
                {
                    SendProductBi(grid.id, outputId, -1, 0, true, "not cd");
                    DebugUtil.Log("直接产出物品:" + outputId + "---位置:" + pos + "---产出建筑id：" + grid.id);
                    grids[index]?.board?.PlayAnimator("click", true);
                    ProductOneItem(index, pos, outputId, false, RefreshItemSource.timeProduct);
                }
            }

            RefreshOneProduct(index);
            // DebugUtil.LogError("产出之后剩余次数:" + MergeManager.Instance.GetLeftProductCount(index));
            if (SelectIndex > 0 && SelectIndex == index)
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID,
                    new Vector2Int(index, grid.id),(MergeBoardEnum)_boardID);
        }
    }

    private void FreeTimeProductItem(int index) //释放存贮时间产出建筑产出的item
    {
        List<int> items = MergeManager.Instance.GetTimeProductItems(index,(MergeBoardEnum)_boardID);
        int count = items.Count;
        if (items == null || count == 0) //在九宫格范围内产生存贮的item
            return;

        for (int i = 0; i < count; i++)
        {
            int pos = MergeManager.Instance.GetSudokuEmptIndex(index,(MergeBoardEnum)_boardID);
        
            if (pos != -1)
            {
                grids[index]?.board?.PlayAnimator("click", true);
                ProductOneItem(index, pos, items[i], false, RefreshItemSource.timeProduct,isFreeTimeProduct:true);
                MergeManager.Instance.FreeTimeProductItems(index, items[i],(MergeBoardEnum)_boardID);
               
                RefreshOneProduct(index);
                i--;
                count--;
                if (SelectIndex > 0 && SelectIndex == index)
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID,
                        new Vector2Int(index, grids[index].id),(MergeBoardEnum)_boardID);
            }
        }
    }

    #region 动效

    // --------------------------------------------- 动效
    Grid[] tempMergeAnimationsItem = new Grid[2]; // 只会存在2个merge合成的提示
    int lastIndex = -1;

    private void MergeAnimator(int index)
    {
        if (activeIndex == -1)
        {
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
            return;
        }
        if (index == -1)
        {
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
            return;
        }
        if (index >= grids.Length || activeIndex >= grids.Length || MergeManager.Instance.IsBubble(index,(MergeBoardEnum)_boardID) ||
            MergeManager.Instance.IsBubble(activeIndex,(MergeBoardEnum)_boardID))
        {
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
            return;
        }
        TableMergeItem activeConfig = GameConfigManager.Instance.GetItemConfig(grids[activeIndex].id);
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(grids[index].id);
        if (config != null && config.type == (int)MergeItemType.hamaster)//仓鼠
        {
            if (!MergeManager.Instance.IsActiveItem(index,(MergeBoardEnum)_boardID))
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
        
            if (HappyGoModel.Instance.GetRequestProductId() == grids[activeIndex].id)
            {
                if(lastIndex != index)
                    SetMergeStatus(lastIndex, false);
            
                lastIndex = index;
                SetMergeStatus(lastIndex, true);
                return;
            }
        
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
            return;
           
        }
        if (config != null && config.type == (int)MergeItemType.eatBuild)
        {
            if (!MergeManager.Instance.IsActiveItem(index,(MergeBoardEnum)_boardID))
            {
                if (activeConfig != null && activeConfig.type == (int) MergeItemType.SpeedUp)
                {
                    bool isTimeProductItem = MergeConfigManager.Instance.IsTimeProductItem(grids[index].board.tableMergeItem);
                    bool isProductItem = MergeConfigManager.Instance.IsProductItem(grids[index].board.tableMergeItem);
                    bool isCanSpeedup = false;
                    if (isProductItem || isTimeProductItem)
                    {
                        float productPercent = 0;
                        int cdTime = MergeManager.Instance.GetLeftProductTime(index, grids[index].board.tableMergeItem,
                            ref productPercent, (MergeBoardEnum) _boardID);
                        if (cdTime > 0)
                            isCanSpeedup = true;

                        cdTime = MergeManager.Instance.GetLeftActiveTime(grids[index].board.storageMergeItem,
                            grids[index].board.tableMergeItem, ref productPercent);
                        if (cdTime > 0)
                            isCanSpeedup = true;

                        if (isTimeProductItem)
                        {
                            cdTime = MergeManager.Instance.GetTimeProductCd(index, grids[index].board.tableMergeItem,
                                ref productPercent, (MergeBoardEnum) _boardID);
                            if (cdTime > 0 && grids[index].board.storageMergeItem.ProductItems.Count <= 0)
                                isCanSpeedup = true;
                        }

                        if (!isCanSpeedup)
                        {
                            SetMergeStatus(lastIndex, false);
                            lastIndex = -1;
                            return;

                        }

                        lastIndex = index;
                        SetMergeStatus(lastIndex, true);
                        return;
                    }
                    else
                    {
                        SetMergeStatus(lastIndex, false);
                        lastIndex = -1;
                        return;
                    }
                }
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
        
            var produceCost = MergeManager.Instance.GetProduceCost(grids[index].board.tableMergeItem);
            if (produceCost!=null && 
                produceCost.Contains(grids[activeIndex].id) && !MergeManager.Instance.IsBuildEat(grids[index].board.storageMergeItem,grids[activeIndex].id))
            {
                if(lastIndex != index)
                    SetMergeStatus(lastIndex, false);
            
                lastIndex = index;
                SetMergeStatus(lastIndex, true);
                return;
            }
        
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
            return;
           
        }

        if (index != activeIndex && activeConfig != null && activeConfig.canStacking)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
            if (itemConfig != null && itemConfig.type == activeConfig.type && itemConfig.id == activeConfig.id)
            {
                if(lastIndex != index)
                    SetMergeStatus(lastIndex, false);
            
                lastIndex = index;
                SetMergeStatus(lastIndex, true);
                return;
            }
        }
        
        if (activeConfig != null && activeConfig.type == 16)
        {
            if (MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID))
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
            
            if (!MergeManager.Instance.IsLockWeb(index,(MergeBoardEnum)_boardID))
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
            
            if(lastIndex != index)
                SetMergeStatus(lastIndex, false);
            
            lastIndex = index;
            SetMergeStatus(lastIndex, true);
            return;
        }

       

        if (!MergeConfigManager.Instance.IsCanMergeItem(grids[index].id) && !MergeConfigManager.Instance.IsSplit(grids[activeIndex].id)&& !MergeConfigManager.Instance.IsMagicWand(grids[activeIndex].id)) //不可合成的 直接不播放动效
        {
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
            return;
        }

        if (activeIndex == index)
        {
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
            return;
        }
        if (MergeConfigManager.Instance.IsSplit(grids[activeIndex].id))
        {
            bool isBox = MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID);
            var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
            bool isOpen = MergeManager.Instance.IsOpen(index,(MergeBoardEnum)_boardID);
            if(!isOpen||isBox|| itemConfig == null || MergeManager.Instance.IsBubble(index,(MergeBoardEnum)_boardID)||!MergeConfigManager.Instance.IsCanSplit(grids[index].id) && itemConfig.type!=19||MergeConfigManager.Instance.IsTimeStoreItem(itemConfig)||
               MergeConfigManager.Instance.IsCanProductItem(grids[index].id))
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
            
            tempMergeAnimationsItem[0] = grids[activeIndex];
            if (lastIndex != index)
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
            }
            
            lastIndex = index;
            SetMergeStatus(lastIndex, true);
        }
        else if ( MergeConfigManager.Instance.IsOmnipoten(grids[activeIndex].id))
        {
            bool isBox = MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID);
            var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
            if (isBox || grids[index].id == -1 ||MergeManager.Instance.IsLockWeb(index,(MergeBoardEnum)_boardID)
                ||MergeConfigManager.Instance.IsTimeStoreItem(itemConfig)
                ||!MergeConfigManager.Instance.IsCanOmnipoten(itemConfig)
                ||(MergeConfigManager.Instance.IsCanProductItem(grids[index].id) && MergeConfigManager.Instance.IsOmnipoten(grids[activeIndex].id)))
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
            tempMergeAnimationsItem[0] = grids[activeIndex];
            if (lastIndex != index)
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
            }
            
            lastIndex = index;
            SetMergeStatus(lastIndex, true);
        }
        else if ( MergeConfigManager.Instance.IsMagicWand(grids[activeIndex].id))
        {
            bool isBox = MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID);
            var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
            if (isBox || grids[index].id == -1 ||MergeManager.Instance.IsLockWeb(index,(MergeBoardEnum)_boardID)
                ||MergeConfigManager.Instance.IsTimeStoreItem(itemConfig)
                ||!MergeConfigManager.Instance.IsCanOmnipoten(itemConfig)
                ||(MergeConfigManager.Instance.IsCanProductItem(grids[index].id) && MergeConfigManager.Instance.IsMagicWand(grids[activeIndex].id)))
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
            tempMergeAnimationsItem[0] = grids[activeIndex];
            if (lastIndex != index)
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
            }
            
            lastIndex = index;
            SetMergeStatus(lastIndex, true);
        }
        else if (grids[activeIndex].id == grids[index].id )
        {
            bool isBox = MergeManager.Instance.IsBox(index,(MergeBoardEnum)_boardID);
            if (isBox || grids[index].id == -1 )
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
             
            var itemConfig = GameConfigManager.Instance.GetItemConfig(grids[index].id);
            if (itemConfig != null && itemConfig.type == 2)
            {
                var itemStorage1 = MergeManager.Instance.GetBoardItem(index,(MergeBoardEnum)_boardID);
                var itemStorage2 = MergeManager.Instance.GetBoardItem(activeIndex,(MergeBoardEnum)_boardID);
                if (itemStorage1.ProductCount > 0 || itemStorage2.ProductCount > 0)
                {
                    SetMergeStatus(lastIndex, false);
                    lastIndex = -1;
                    return;
                }
            }
            tempMergeAnimationsItem[0] = grids[activeIndex];
            if (lastIndex != index)
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
            }
            
            lastIndex = index;
            SetMergeStatus(lastIndex, true);
        }
        else if (activeConfig != null && activeConfig.type == (int) MergeItemType.SpeedUp)
        {
            if (config == null)
                return;
            bool isTimeProductItem = MergeConfigManager.Instance.IsTimeProductItem(grids[index].board.tableMergeItem);
            bool isProductItem = MergeConfigManager.Instance.IsProductItem(grids[index].board.tableMergeItem);
            bool isCanSpeedup = false;
            if (isProductItem || isTimeProductItem)
            {
                float productPercent = 0;
                int cdTime = MergeManager.Instance.GetLeftProductTime(index, grids[index].board.tableMergeItem,
                    ref productPercent, (MergeBoardEnum) _boardID);
                if (cdTime > 0)
                    isCanSpeedup = true;

                cdTime = MergeManager.Instance.GetLeftActiveTime(grids[index].board.storageMergeItem,
                    grids[index].board.tableMergeItem, ref productPercent);
                if (cdTime > 0)
                    isCanSpeedup = true;

                if (isTimeProductItem)
                {
                    cdTime = MergeManager.Instance.GetTimeProductCd(index, grids[index].board.tableMergeItem,
                        ref productPercent, (MergeBoardEnum) _boardID);
                    if (cdTime > 0 && grids[index].board.storageMergeItem.ProductItems.Count <= 0)
                        isCanSpeedup = true;
                }

                if (!isCanSpeedup)
                {
                    SetMergeStatus(lastIndex, false);
                    lastIndex = -1;
                    return;

                }

                lastIndex = index;
                SetMergeStatus(lastIndex, true);
            }
            else
            {
                SetMergeStatus(lastIndex, false);
                lastIndex = -1;
                return;
            }
        }
        else
        {
            SetMergeStatus(lastIndex, false);
            lastIndex = -1;
        }
       
    }

    private void SetMergeStatus(int index, bool isMerge)
    {
        if (index < 0 || index >= grids.Length)
            return;

        if (grids[index].board == null)
            return;

        grids[index].board.SetMergeStatus(isMerge);
        if(isMerge)
            grids[index].board.transform.SetSiblingIndex(61);
    }

    public void DoAnimation(int index, int oldIndex, RefreshItemSource source, Action tweenEndCall = null)
    {
        Grid grid = grids[index];

        if ((source == RefreshItemSource.rewards && grid.board.tableMergeItem.type == (int) MergeItemType.box) 
            /*|| source == RefreshItemSource.bag*/)
            grid.board.PlayHintEffect();

        if (source == RefreshItemSource.product)
        {
            if (EnergyTorrentModel.Instance.IsOpen() && oldIndex>0 && grids[oldIndex]!=null && 
                grids[oldIndex].board!=null && grids[oldIndex].board.tableMergeItem!=null
                && MergeConfigManager.Instance.IsEnergyTorrentProduct(grids[oldIndex].board.tableMergeItem))
            {
                grid.board.vfx_trail.gameObject.SetActive(true);
            }
            grid.board.DoProductTween(IndexToPos(oldIndex), IndexToPos(index), source, tweenEndCall);
            if (grid.state == 0)
            {
                if(_boardID != (int)MergeBoardEnum.HappyGo)
                    MergeEffectManager.Instance.PlayBombEffect(grid.board.transform);
            }
        }
        else if (source == RefreshItemSource.timeProduct)
        {
            grid.board.DoProductTween(IndexToPos(oldIndex), IndexToPos(index), source, tweenEndCall);
        }
        else if (source == RefreshItemSource.newGuide)
        {
            grid.board.DoProductTween(IndexToPos(activeIndex), IndexToPos(index), source, tweenEndCall);
        }
        else if (source == RefreshItemSource.mergeBubble)
        {
            grid.board.PlayBubbleAnimation("appear");
            grid.board.DoProductTween(IndexToPos(oldIndex), IndexToPos(index), source, tweenEndCall);
        }
        else if (source == RefreshItemSource.exp)
        {
            grid.board.DoProductTween(IndexToPos(oldIndex), IndexToPos(index), source, tweenEndCall);
        }
        else if (source == RefreshItemSource.mergeOk || source==RefreshItemSource.mergeOk_omnipoten)
        {
            CommonUtils.DelayedCall(0.17f, () =>
            {
                if (grid == null || grid.board == null)
                    return;
                grid.board.UpdateIconImage();
                grid.board.PlayMergeEffect();
            });
      
            CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(grid.board.Animator, "hideAnim",
                null,
                () =>
                {
                    tweenEndCall?.Invoke();
                }));
        }
        else
        {
            tweenEndCall?.Invoke();
        }
    }

    private void StopMergeAnimations()
    {
        // if (activeIndex != -1)
        //     grids[activeIndex].transform.Find("vfx_merge").gameObject.SetActive(false);
    }

    private void OnSellItem(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length < 2)
            return;
        if ((MergeBoardEnum) e.datas[0] != (MergeBoardEnum) _boardID)
            return;
        int index = (int) e.datas[1];
        this.transform.localScale = originalScale;
        grids[index].board.OnSellItem();
    }

    #endregion 动效

    public virtual TableMergeItem MergeToProductActivity(int index,TableMergeItem newConfig,TableMergeItem oldConfig)
    {
        int newPos = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)_boardID);
        if (newPos == -1)
            return null;

        int activityProductId = -1;//活动方法
        // activityProductId = ClimbTreeModel.Instance.CreateBananaOnMerge(newConfig);//猴子爬树产出香蕉
        if (activityProductId < 0)
            return null;
        
        var itemConfig = GameConfigManager.Instance.GetItemConfig(activityProductId);
        ProductOneItem(index, newPos, activityProductId, false, RefreshItemSource.product);
        return itemConfig;
    }
    
    
    #region 合成四级以上的物品 产出经验

    private bool MergeToProdcutExp(int index, int id, int oldId)
    {
        if (_boardID == (int)MergeBoardEnum.Stimulate || _boardID == (int)MergeBoardEnum.Filthy || _boardID == (int)MergeBoardEnum.Ditch || _boardID == (int)MergeBoardEnum.TrainOrder)
            return false;
        
        // if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.MergeOutXPLevel))
        //     return false;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemConfig == null || itemConfig.merge_output<=0) // 4级以上产出经验 
            return false;
        int newPos = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)_boardID);
        if (newPos == -1)
            return false;
        ProductOneItem(index, newPos, itemConfig.merge_output, false, RefreshItemSource.exp);
        return true;
    }

    #endregion

    #region 合成产出气泡物品

    private bool MergeToProdcutBubble(int index, int id, int oldId)
    {
        bool isSuccess = false;
        MergeManager.Instance.AddMergeCount(oldId, id,(MergeBoardEnum)_boardID);
        if (_boardID == (int)MergeBoardEnum.Stimulate || _boardID == (int)MergeBoardEnum.Filthy || _boardID == (int)MergeBoardEnum.Ditch || _boardID == (int)MergeBoardEnum.TrainOrder)
            return false;
        
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BubbleOpen))
            return isSuccess;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemConfig == null)
            return isSuccess;
        if (itemConfig.in_line == ClimbTreeModel._climbTreeBananaLineId)
            return isSuccess;
        bool isCan = MergeManager.Instance.IsCanOccurBubble(oldId);
        if (!isCan)
            return isSuccess;
        bool isTodayCanProductBubble = MergeManager.Instance.TodayIsCanProductBubble(id);
        if (!isTodayCanProductBubble)
            return isSuccess;
        int newPos = MergeManager.Instance.FindEmptyGrid(index,(MergeBoardEnum)_boardID);
        if (newPos == -1)
            return isSuccess;
        int count = MergeManager.Instance.GetBoardItemCountByType(MergeItemStatus.bubble,(MergeBoardEnum)_boardID);
        if (count >= 3)
            return isSuccess;
        // 广告不可用时候 不生成气泡 0 钻石 1 广告
        bool rvEable = true;// AdLogicManager.Instance.ShouldShowRV(ADConstDefine.RV_BUBBLE_OPEN);
        int type = MergeManager.Instance.RandomBubbleType(id);
        if ((!rvEable && type == 1))
            return isSuccess;
        ProductOneItem(index, newPos, id, false, RefreshItemSource.mergeBubble, 2);
        MergeManager.Instance.SetBoardItemBubbleType(type, newPos,(MergeBoardEnum)_boardID);
        MergeManager.Instance.ClearMergeCount(oldId,(MergeBoardEnum)_boardID);
        //AudioManager.Instance.PlaySound(HotelSound.mhm_bubble_appearance);
        isSuccess = true;
        var itemOldConfig = GameConfigManager.Instance.GetItemConfig(id);
        string biType = type == 1 ? "rv" : "gem";
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeBubbleCreate,
            itemAId = itemOldConfig.id,
            ItemALevel = itemOldConfig.level,
            itemBId = itemConfig.id,
            itemBLevel = itemConfig.level,
            isChange = true,
            extras = new Dictionary<string, string>
            {
                {"type", biType}
            }
        });

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBubblePop, biType);
        MergeManager.Instance.RecordBubbleInfo(id);
        return isSuccess;
    }

    #endregion

    #region 加速道具动效播放

    private IEnumerator PlayTimeSpeedAnimation(int index)
    {
        for (int i = 1; i <= boardWidth; i++)
        {
            var list = FindNearGrid(index, i);
            yield return PlayTimeTween(list, index);
        }
    }

    private IEnumerator PlayTimeTween(List<int> list, int index)
    {
        for (int j = 0; j < list.Count; j++)
        {
            grids[list[j]].board?.DoTimeSpeedUpTween(IndexToPos(index));
        }

        yield return new WaitForSeconds(0.05f);
    }

    List<int> anmationList = new List<int>();
    List<int> targetAnmationList = new List<int>();

    private List<int> FindNearGrid(int index, int near)
    {
        anmationList.Clear();
        targetAnmationList.Clear();
        int xIndex = index % boardWidth;
        int yIndex = index / boardWidth;
        int leftIndex = xIndex - near;
        int rightIndex = xIndex + near;
        int upIndex = yIndex + near;
        int downIndex = yIndex - near;
        for (int i = -near; i <= near; i++)
        {
            int lefty = yIndex - i;
            int righty = yIndex + i;
            int upx = xIndex + i;
            int downx = xIndex - i;
            int targeIndex = lefty * boardWidth + leftIndex; //(leftIndex,lefty)
            if (IsInArea(leftIndex, lefty))
            {
                anmationList.Add(targeIndex);
            }

            targeIndex = righty * boardWidth + rightIndex; //(rightIndex,righty)
            if (IsInArea(rightIndex, righty))
            {
                anmationList.Add(targeIndex);
            }

            targeIndex = upIndex * boardWidth + upx; //(upx,upIndex)
            if (IsInArea(upx, upIndex))
            {
                anmationList.Add(targeIndex);
            }

            targeIndex = downIndex * boardWidth + downx; //(downx,downIndex)
            if (IsInArea(downx, downIndex))
            {
                anmationList.Add(targeIndex);
            }
        }

        targetAnmationList = anmationList.Distinct().ToList();
        targetAnmationList.Remove(index);
        return targetAnmationList;
    }

    private bool IsInArea(int x, int y)
    {
        return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
    }

    #endregion

    public void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            return;

        RestoreInput();
    }

    public void RestoreInput()
    {
        if (!gameObject.activeSelf)
            return;

        if (_baseEventData == null)
            return;

        OnPointerUp(_baseEventData);
        OnEndDrag(_baseEventData);
        _baseEventData = null;
    }

    private bool EatBuildingTipsAnim()
    {
        Grid eatGrid = null;
        foreach (var grid in grids)
        {
            if (grid != null && grid.board != null && grid.board.storageMergeItem.State == 3 &&
                grid.board.tableMergeItem.type == (int) MergeItemType.eatBuild)
            {
                eatGrid = grid;
                break;
            }
        }
        if(eatGrid == null || eatGrid.board == null)
            return  false;
        
        if (mergeTipList != null && mergeTipList.Count >= 2)
        {
            var produceCost = MergeManager.Instance.GetProduceCost(eatGrid.board.tableMergeItem);
            if (mergeTipList[0] == eatGrid && produceCost != null && produceCost.Contains(mergeTipList[1].id))
                return true;
                
            if (mergeTipList[1] == eatGrid && produceCost!= null && produceCost.Contains(mergeTipList[0].id))
                return true;
        }
        List<Grid> tempgrids =  FindCanEatFoods(eatGrid);
        if (tempgrids == null || tempgrids.Count == 0)
            return false;
        
        CancelTip(-1, true);
        mergeTipList.Clear();
        mergeTipList.Add(tempgrids.RandomPickOne());
        mergeTipList.Add(eatGrid);
        PlayTipsAnimation(-1, 1f);
        return true;
    }

    private bool HamsterFoodTipsAnim()
    {
        if(HamsterGrid == null || HamsterGrid.board == null)
            return  false;

        if (!MergeManager.Instance.IsActiveItem(HamsterGrid.board.index,(MergeBoardEnum)_boardID))
        {
            if (mergeTipList != null && mergeTipList.Count >= 2)
            {
                if (mergeTipList[0] == HamsterGrid || mergeTipList[1] == HamsterGrid)
                    CancelTip(-1, true);
            }

            return false;
        }
        
        if (mergeTipList != null && mergeTipList.Count >= 2)
        {
            if (mergeTipList[0] == HamsterGrid && mergeTipList[1].id == HappyGoModel.Instance.GetRequestProductId())
                return true;
                
            if (mergeTipList[1] == HamsterGrid && mergeTipList[0].id == HappyGoModel.Instance.GetRequestProductId())
                return true;
        }

        List<Grid> grids =  FindHamsterFoods();
        if (grids == null || grids.Count == 0)
            return false;
        
        CancelTip(-1, true);
        mergeTipList.Clear();
        mergeTipList.Add(grids.RandomPickOne());
        mergeTipList.Add(HamsterGrid);
        PlayTipsAnimation(-1, 1f);

        return true;
    }

    
    private List<Grid> _hamsterFoods = new List<Grid>();
    public List<Grid> FindHamsterFoods()
    {
        _hamsterFoods.Clear();
        for (int i = 0; i < grids.Length; i++)
        {
            if(grids[i].id == -1 || grids[i].state != 1 && grids[i].state != 3 && grids[i].state != 0 || grids[i].state == 0)
                continue;
           
            if(grids[i].id != HappyGoModel.Instance.GetRequestProductId())
                continue;
            
            _hamsterFoods.Add(grids[i]);
        }
        return _hamsterFoods;
    }   
    private List<Grid> canEatFoods = new List<Grid>();
    public List<Grid> FindCanEatFoods(Grid eatGrid)
    {
        if(eatGrid==null ||eatGrid.board==null)
            return canEatFoods;
        canEatFoods.Clear();
        for (int i = 0; i < grids.Length; i++)
        {
            var produceCost = MergeManager.Instance.GetProduceCost(eatGrid.board.tableMergeItem);
            
            if (grids[i].id != -1 && grids[i].state == 1 && grids[i].board!=null && 
                produceCost!=null &&
                produceCost.Contains(grids[i].id) &&
                !MergeManager.Instance.IsBuildEat(eatGrid.board.storageMergeItem,grids[i].id))
            {
                canEatFoods.Add(grids[i]);
            }

        }
        return canEatFoods;
    }
    
    protected virtual void AddSiblingTransform(Transform trans)
    {}
    
    protected virtual void DelSiblingTransform(Transform trans)
    {}
    
    protected virtual void UpdateSibling(bool isAdapt = false)
    {}
    
    public void RefreshInitGrids()
    {
        foreach (var grid in grids)
        {
            if(grid.board == null)
                continue;
            
            grid.board.Reset();
            itemPool.Release(grid.board.gameObject);
        }

        grids = null;

        InitGrids(boardWidth * boardHeight);
    }
}
