using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;


public class MergePackageUnit : MonoBehaviour
{
    private MergeBoardEnum BoardId
    {
        get
        {
            if (!_boardIdSetFlag)
            {
                Debug.LogError("MergeBoardItem未设置boardId");    
            }
            return _boardId;
        }
    }

    private MergeBoardEnum _boardId;
    private bool _boardIdSetFlag = false;
    public void SetBoardId(MergeBoardEnum boardId)
    {
        _boardIdSetFlag = true;
        _boardId = boardId;
    }
    private MergePackageUnitState state1, state2, state3, state4;
    private Image icon;
    private Transform masterCardIcon;
    private Button useBtn;
    private Button tipBtn;
    [NonSerialized] private int index;
    [NonSerialized] private MergePackageUnitType mType;

    [NonSerialized] private TableMergeItem mergeItemConfig;
    private Transform Bg;
    private Transform BgBuilding;

    private void Awake()
    {
        state1 = this.transform.Find("State1").gameObject.GetComponentDefault<MergePackageUnitState>();
        state2 = this.transform.Find("State2").gameObject.GetComponentDefault<MergePackageUnitState>();
        state3 = this.transform.Find("State3").gameObject.GetComponentDefault<MergePackageUnitState>();
        state4 = this.transform.Find("State4").gameObject.GetComponentDefault<MergePackageUnitState>();
        icon = GetComponent<Image>();
        masterCardIcon = transform.Find("SpecialIcon");
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, MergeInfoProductEvent);
        Bg = transform.Find("BG");
        BgBuilding = transform.Find("BGBuilding");
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, MergeInfoProductEvent);
    }

    public void SetItemInfomation(TableMergeItem itemConfig, int choosedIndex, MergePackageUnitType type,
        int parentId = 0, int count = 0)
    {
        useBtn = this.transform.GetComponent<Button>();
        useBtn.onClick.RemoveAllListeners();
        useBtn?.onClick.AddListener(OnClickItem);

        tipBtn = this.transform.Find("State2/Tip").GetComponent<Button>();
        tipBtn.onClick.RemoveAllListeners();
        tipBtn?.onClick.AddListener(OnClickItem);
        bool isBuild = type == MergePackageUnitType.buildBag || type == MergePackageUnitType.buildBaglock
                                                             || type == MergePackageUnitType.buildBagMax
                                                             || type == MergePackageUnitType.buildBagUnlock;
        BgBuilding.gameObject.SetActive(isBuild);
        Bg.gameObject.SetActive(!isBuild);
        this.mergeItemConfig = itemConfig;
        this.index = choosedIndex;
        this.mType = type;
        ClearStatus();
        switch (type)
        {
            case MergePackageUnitType.buildBagMax:
            case MergePackageUnitType.bagMax:
                state2.gameObject.SetActive(true);
                state2.SetItemStatus(itemConfig, choosedIndex, type, parentId);
                break;
            case MergePackageUnitType.buildBagUnlock:
            case MergePackageUnitType.bagUnlock:
                state2.SetItemStatus(itemConfig, choosedIndex, type, parentId);
                break;
            case MergePackageUnitType.baglock:
            case MergePackageUnitType.buildBaglock:
                state2.gameObject.SetActive(true);
                state2.SetItemStatus(itemConfig, choosedIndex, type, parentId);
                break;
            case MergePackageUnitType.info:
                state3.gameObject.SetActive(true);
                state3.SetItemStatus(itemConfig, choosedIndex, type, parentId);
                break;
            case MergePackageUnitType.productExplain:
                state4.gameObject.SetActive(true);
                state4.SetItemStatus(itemConfig, choosedIndex, type, parentId);
                break;
            case MergePackageUnitType.product:
            {
                state1.gameObject.SetActive(true);
                state1.SetItemStatus(itemConfig, choosedIndex, type, parentId);
                break;
            }
            case MergePackageUnitType.bag:
            case MergePackageUnitType.buildBag:
            case MergePackageUnitType.bigInfo:
            case MergePackageUnitType.infoTips:
            case MergePackageUnitType.taskRewards:
            case MergePackageUnitType.taskXpRewards:
            case MergePackageUnitType.warning:
            case MergePackageUnitType.splite:
            case MergePackageUnitType.buildBundle:
            case MergePackageUnitType.increasItem:
                state1.gameObject.SetActive(true);
                state1.SetItemStatus(itemConfig, choosedIndex, type, parentId, count);
                break;
        }
    }

    public void SetItemInfomation(int itemId, MergePackageUnitType type)
    {
        this.mType = type;
        ClearStatus();
        switch (type)
        {
            case MergePackageUnitType.notMergeIcon:
                state1.gameObject.SetActive(true);
                state1.SetItemStatus(itemId, type);
                break;
        }
    }

    public void SetNameText(string txt)
    {
        state1.SetNameText(txt);
    }

    public void SetLevelText(string txt)
    {
        state1.SetLevelText(txt);
    }

    private void ClearStatus()
    {
        state1.gameObject.SetActive(false);
        state2.gameObject.SetActive(false);
        state3.gameObject.SetActive(false);
        state4.gameObject.SetActive(false);
        masterCardIcon.gameObject.SetActive(false);
    }

    public void SetTaskStatus(int curCount, int needCount)
    {
        state1.SetTaskStatus(curCount, needCount);
    }

    public void SetTaskText(string msg)
    {
        state1.SetTaskText(msg);
    }

    public void SetBagCost(int type,int cost)
    {
        state2.SetBagCost(type,cost);
    }

    private void OnClickItem()
    {
        switch (mType)
        {
            case MergePackageUnitType.bag:
            {
                int emptyIndex = MergeManager.Instance.FindEmptyGrid(index,BoardId);
                if (emptyIndex != -1)
                {
                    UseBagItem(emptyIndex);
                }
                else
                {
                    MergePromptManager.Instance.ShowTextPrompt(transform.position);
                }

                break;
            }     
            case MergePackageUnitType.buildBag:
            {
                int emptyIndex = MergeManager.Instance.FindEmptyGrid(index,BoardId);
                if (emptyIndex != -1)
                {
                    UseBuildBagItem(emptyIndex);
                }
                else
                {
                    MergePromptManager.Instance.ShowTextPrompt(transform.position);
                }

                break;
            }
            case MergePackageUnitType.baglock:
            {
                UIPopupMergePackageController.Instance.AddBagUnit();
                break;
            }        
            case MergePackageUnitType.buildBaglock:
            {
                UIPopupMergePackageController.Instance.AddBuildBagUnit();
                break;
            }
            case MergePackageUnitType.infoTips:
            case MergePackageUnitType.product:
            case MergePackageUnitType.taskRewards:
            {
                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CloseItemInfo))
                    return;

                MergeInfoView.Instance.OpenMergeInfo(mergeItemConfig);
                break;
            }
            case MergePackageUnitType.productExplain:
            {
                EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, mergeItemConfig);
                break;
            }
        }
    }

    private void UseBagItem(int emptyIndex)
    {
        if (emptyIndex == -1)
            return;
        AudioManager.Instance.PlaySound(13);
        MergeMainController.Instance.bagItemPostion =
            this.transform.parent.TransformPoint(this.transform.localPosition);
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,BoardId);

        int itemIndex = transform.GetSiblingIndex()-1;
        StorageMergeItem mergeItem = MergeManager.Instance.GetBagItem(itemIndex,BoardId);

        // FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, MergeMainController.Instance.bagItemPostion,
        //     MergeMainController.Instance.MergeBoard.IndexToPosition(emptyIndex), 5f,
        //     () =>
        //     {
        //         EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH,BoardId, emptyIndex, -1,
        //             RefreshItemSource.bag, mergeItem.Id);
        //     });

        MergeManager.Instance.UseBgItem(itemIndex, emptyIndex,BoardId);
        UIPopupMergePackageController.Instance.RefreshBag(itemIndex);
        
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH,BoardId, emptyIndex, -1, RefreshItemSource.bag, mergeItem.Id);
        UIManager.Instance.GetOpenedUIByPath<UIPopupMergePackageController>(UINameConst.UIPopupMergePackage)?.GetOutItemPositionList.Add(emptyIndex);
        MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Pack,MergeBoardEnum.Main);
        // UIManager.Instance.CloseUI(UINameConst.UIPopupMergePackage, true);
    }

    private void UseBuildBagItem(int emptyIndex)
    {
        if (emptyIndex == -1)
            return;
        AudioManager.Instance.PlaySound(13);
        MergeMainController.Instance.bagItemPostion =
            this.transform.parent.TransformPoint(this.transform.localPosition);
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELECTED_GRID, Vector2Int.zero,BoardId);

        int itemIndex = transform.GetSiblingIndex()-1;
        StorageMergeItem mergeItem = MergeManager.Instance.GetBuildingBagItem(itemIndex,BoardId);

        // FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, MergeMainController.Instance.bagItemPostion,
        //     MergeMainController.Instance.MergeBoard.IndexToPosition(emptyIndex), 5f,
        //     () =>
        //     {
        //         EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH,BoardId, emptyIndex, -1,
        //             RefreshItemSource.bag, mergeItem.Id);
        //     });

        MergeManager.Instance.UseBuildingBgItem(itemIndex, emptyIndex,BoardId);
        UIPopupMergePackageController.Instance.RefreshBuildingBag();
        
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH,BoardId, emptyIndex, -1, RefreshItemSource.bag, mergeItem.Id);
        UIManager.Instance.GetOpenedUIByPath<UIPopupMergePackageController>(UINameConst.UIPopupMergePackage)?.GetOutItemPositionList.Add(emptyIndex);
        MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Pack,MergeBoardEnum.Main);
        // UIManager.Instance.CloseUI(UINameConst.UIPopupMergePackage, true);
    }



    private void MergeInfoProductEvent(BaseEvent e)
    {
        if (mType != MergePackageUnitType.productExplain)
            return;

        if (e == null || e.datas == null || e.datas.Length == 0)
            return;

        TableMergeItem config = (TableMergeItem) e.datas[0];
        state4.UpdateSelectedBg(config);
    }
}

public enum MergePackageUnitType
{
    bag, //背包
    baglock, //未开启的背包
    bagUnlock, //开启的背包
    bagMax, //已达到解锁最大容量
    buildBag,
    buildBaglock, 
    buildBagUnlock, 
    buildBagMax, 
    vipBagUnlock, //开启的背包
    vipBag, //开启的背包
    info, // 合成链
    product, // 产出
    productExplain, // 产出信息
    bigInfo, //信息界面大item
    infoTips, //详细信息
    taskRewards, // 任务奖励
    taskXpRewards, // 经验奖励
    warning, // 出售物品提示
    splite, //分解
    buildBundle, // 建筑礼包
    notMergeIcon, // 非棋盘物品
    increasItem, //万能合成二次弹窗
}