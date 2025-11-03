using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Network.API;
using Framework;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class HappyGoMergeClickTips : MonoBehaviour
{
    
    private MergeBoardEnum BoardId
    {
        get
        {
            if (!_boardIdSetFlag)
            {
                Debug.LogError("MergeClickTips未设置boardId");    
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
    private LocalizeTextMeshProUGUI titleText;
    private LocalizeTextMeshProUGUI contentText;
    private Button tipsBtn;
    private LocalizeTextMeshProUGUI coinText;
    private LocalizeTextMeshProUGUI diamondsText;
    private Transform cdGroup;
    private LocalizeTextMeshProUGUI cdContentText;
    private LocalizeTextMeshProUGUI cdText;
    private LocalizeTextMeshProUGUI timeText;
    private LocalizeTextMeshProUGUI sellBtnText;

    private LocalizeTextMeshProUGUI speedBtnText;
    private Button sellBtn;
    private Button destoryBtn;
    private Button speedBtn;
    private Button timeBtn;
    private Button discomposeBtn;
    private Button undoBtn;

    private Button rvBtn;
    private LocalizeTextMeshProUGUI rvBtnText;
    private LocalizeTextMeshProUGUI rvBtnDesText;
    private Button rvBtn2;
    private int selecGridIndex = -1;
    private int selecGridId = -1;
    
    private StorageMergeItem sellItem;
    public static HappyGoMergeClickTips Instance;
    float sellColdTime = 0;
    private Coroutine bubbleDissmiss;

    private Image itemImage;
    private GameObject itemObj;

    private TableMergeItem curMergeItem = null;
    private StorageMergeItem boardItem = null;
    public int SelectIndex
    {
        get { return selecGridIndex; }
    }

    private GameObject _hamsterObj;
    private Image _hamsterImage;
    private LocalizeTextMeshProUGUI _hamsterText;
    private Button _hamsterButton;

    private void Awake()
    {
        titleText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        contentText = transform.Find("ContentTextGroup/ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        cdGroup = transform.Find("CdGroup");
        cdContentText= transform.Find("CdGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        cdContentText.SetTerm("item_note_text31");
        cdText = transform.Find("CdGroup/CdText").GetComponent<LocalizeTextMeshProUGUI>();
        
        tipsBtn = transform.Find("TipBtn").GetComponent<Button>();
        
        sellBtn = transform.Find("SellBtn").GetComponent<Button>();
        destoryBtn = transform.Find("DestroyBtn").GetComponent<Button>();
        coinText = transform.Find("SellBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
        sellBtnText = sellBtn.transform.Find("SellText").GetComponent<LocalizeTextMeshProUGUI>();
        
        speedBtn = transform.Find("SpeedBtn").GetComponent<Button>();
        diamondsText = transform.Find("SpeedBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
        speedBtnText = speedBtn.transform.Find("SpeedUpText").GetComponent<LocalizeTextMeshProUGUI>();
        
        timeBtn = transform.Find("TimeBtn").GetComponent<Button>();
        timeText = transform.Find("TimeBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        //分解功能 暂时去掉
        discomposeBtn = this.transform.Find("DecomposBtn").GetComponent<Button>();
        discomposeBtn.gameObject.SetActive(false);
        
        undoBtn = transform.Find("UndoBtn").GetComponent<Button>();

        itemObj = transform.Find("ItemImage").gameObject;
        itemImage = transform.Find("ItemImage/icon").GetComponent<Image>();

        _hamsterObj = transform.Find("ItemGroup").gameObject;
        _hamsterImage = transform.Find("ItemGroup/icon").GetComponent<Image>();
        _hamsterText = transform.Find("ItemGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _hamsterButton = transform.Find("ItemGroup/HelpButton").GetComponent<Button>();
        _hamsterButton.onClick.AddListener(() =>
        {
            TableMergeItem mergeItem = HappyGoModel.Instance.GetBiscuitMergeTable();
            var produceCost = MergeManager.Instance.GetProduceCost(curMergeItem);
            if (produceCost!=null && produceCost.Length > 0)
            {
                mergeItem = GameConfigManager.Instance.GetItemConfig(produceCost[0]);
            }
           
            MergeInfoView.Instance.OpenMergeInfo(mergeItem,isShowGetResource:false);
        });
        
        sellBtn.onClick.AddListener(OnClickSell);
        destoryBtn.onClick.AddListener(OnClickSell);
        speedBtn.onClick.AddListener(OnClickSpeed);
        timeBtn.onClick.AddListener(OnClickTime);
        discomposeBtn.onClick.AddListener(DecomposItem);
        undoBtn.onClick.AddListener(UndoOperate);
        tipsBtn.onClick.AddListener(OnClickTipsBtn);
        
        rvBtn = transform.Find("Rv1").GetComponent<Button>();
        UIAdRewardButton.Create(ADConstDefine.RV_HAPPYGO_BULIDING_CD, UIAdRewardButton.ButtonStyle.Hide, rvBtn.gameObject,
            (s) =>
            {
                if (s) 
                    OnSpeedUp(true);
            }
            , true, 
            onBtnClick: () => { MergeManager.Instance.PauseAllCdTime(MergeBoardEnum.HappyGo); } 
            ,check: () => { return IsOpenRv() && canShowRv;}
            );
        rvBtnText = transform.Find("Rv1/Text").GetComponent<LocalizeTextMeshProUGUI>();
        rvBtnDesText = transform.Find("Rv1/VideoText").GetComponent<LocalizeTextMeshProUGUI>();
        
        rvBtn2 = transform.Find("Rv2").GetComponent<Button>();
        UIAdRewardButton.Create(ADConstDefine.RV_BUBBLE_OPEN, UIAdRewardButton.ButtonStyle.Hide, rvBtn2.gameObject,
            (s) =>
            {
                if (s) 
                    OnSpeedUp(true);
            }
            , true, 
            onBtnClick: () => { MergeManager.Instance.PauseAllCdTime(MergeBoardEnum.HappyGo); } 
            ,check: () => { return canShowBubbleRv;}
        );
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_SELECTED_GRID, OnSelectGrid);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_hamsterObj.transform);
        topLayer.Add(HappyGoMainController.Instance.mMergeBoard.HamsterGrid.board.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.HappyGoInfo, _hamsterObj.transform as RectTransform, topLayer:topLayer);
    }

    private void Start()
    {
        Instance = this;
        
        OnSelectGrid(Vector2Int.zero);
        
        MergeManager.Instance.ResumAllCdTime(MergeBoardEnum.HappyGo);
    }

    private void InitView()
    {
        SetSellButtonActive(false);
        speedBtn.gameObject.SetActive(false);
        timeBtn.gameObject.SetActive(false);
        undoBtn.gameObject.SetActive(false);
        tipsBtn.gameObject.SetActive(false);
        rvBtn.gameObject.SetActive(false);
        itemObj.gameObject.SetActive(false);
        discomposeBtn.gameObject.SetActive(false);
        SetTipContentText(false,"UI_info_text1");
        titleText.SetText("");
   
    }
    void OnSelectGrid(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length == 0)
            return;
        if ((MergeBoardEnum) e.datas[1] != BoardId)
            return;
        Vector2Int arg = (Vector2Int) e.datas[0];
        OnSelectGrid(arg);
    }

    //0 index
    //1 id
    void OnSelectGrid(Vector2Int arg)
    {
        int index = arg[0];
        int id = arg[1];
        
#if DEBUG || UNITY_EDITOR
        float percent = 0;
        int cdTime = MergeManager.Instance.GetLeftProductTime(index, ref percent,MergeBoardEnum.HappyGo);
        string time = $"{cdTime / 3600:00}{"h "}{((cdTime % 3600) / 60):00}{"m"}";
        DebugUtil.LogError("selectIndex:" + arg[0] + "---selectId:" + arg[1] + "---cd:" + time);
#endif
        selecGridIndex = index;
        selecGridId = id;
        HappyGoMainController.Instance.selcedItemId = selecGridId;
        
        RefreshViewInfos();
    }

    private void RefreshViewInfos()
    {
        curMergeItem = GameConfigManager.Instance.GetItemConfig(selecGridId);
        boardItem = MergeManager.Instance.GetBoardItem(selecGridIndex,MergeBoardEnum.HappyGo);
        
        bool isExist = MergeManager.Instance.IsBoardItemExist(selecGridIndex,MergeBoardEnum.HappyGo);
        if (curMergeItem == null || !isExist || (boardItem != null && boardItem.State == (int)MergeItemStatus.box))
        {
            SetNoFocusStatus();
            return;
        }

        RefreshAllStatus();
    }

    private void RefreshAllStatus()
    {
        if(curMergeItem == null || boardItem == null)
            return;
        
        InitView();
        InitHamsterView();
        
        if (curMergeItem != null && boardItem.State != (int) MergeItemStatus.box)
        {
           
            if (itemImage.sprite != null && itemImage.sprite.name != curMergeItem.image || itemImage.sprite == null)
                itemImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(curMergeItem.image);
        }

        if (bubbleDissmiss != null)
        {
            StopCoroutine(bubbleDissmiss);
            bubbleDissmiss = null;
        }
        
        CancelInvoke("Invoke_SetBubbleCdTime");
        CancelInvoke("Invoke_SetActiveCdTime");
        CancelInvoke("Invoke_SetProductCdTime");
        CancelInvoke("Invoke_SetTimeProductCdTime");
        CancelInvoke("Invoke_OtherItemActiveStatus");

        
        InitContentText(curMergeItem);
        SetRvStatus(false);
        switch (boardItem.State)
        {
            case (int)MergeItemStatus.locked:
            {
                SetLockStatus(curMergeItem);
                break;
            }
            case (int)MergeItemStatus.bubble:
            {
                SetBubbleStatus(curMergeItem, boardItem);
                break;
            }
            case (int)MergeItemStatus.open:
            {
                SetSpeedUpStatus(curMergeItem, boardItem);
                break;
            }
            case (int)MergeItemStatus.time:
            {
                SetTimeStatus(curMergeItem);
                break;
            }
        }
    }

    private void InitHamsterView()
    {
        _hamsterObj.gameObject.SetActive(false);
        if(curMergeItem.type != (int) MergeItemType.hamaster && curMergeItem.type != (int) MergeItemType.eatBuild )
            return;
        itemObj.gameObject.SetActive(true);
        if(boardItem.State == 1)
            return;
        
        _hamsterObj.gameObject.SetActive(true);
        TableMergeItem mergeItem = null;
        var produceCost = MergeManager.Instance.GetProduceCost(curMergeItem);
        if (produceCost!=null && produceCost.Length > 0)
        {
            mergeItem = GameConfigManager.Instance.GetItemConfig(produceCost[0]);
        }
        else
        {
            mergeItem =  HappyGoModel.Instance.GetBiscuitMergeTable();
        }
        if (mergeItem != null)
        {
            if (_hamsterImage.sprite == null || _hamsterImage.sprite != null && _hamsterImage.sprite.name != mergeItem.image)
                _hamsterImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
        }

        _hamsterImage.transform.DOScale(1.4f, 0.2f).onComplete = () =>
        {
            _hamsterImage.transform.localScale=Vector3.one;
        };
        _hamsterText.gameObject.SetActive(false);
        _hamsterText.SetText("1/1");
    }
    
    public enum SpeedUpType
    {
        active,
        product,
        timeProduct
    }

    private int _cdCost = 0;
    private int _orgCdCost = 0;
    private SpeedUpType _speedUpType;
    public void SetSpeedUpPrice(SpeedUpType type, int leftCD)
    {
        if (curMergeItem == null)
            return;
        int[] cdCost;
        int cdTime = 0;
        if (type == SpeedUpType.product)
        {
            cdTime =MergeManager.Instance.GetProductCdTime(curMergeItem,boardItem,_boardId);
            cdCost= curMergeItem.cdspeed_cost;
        }
        else if(type==SpeedUpType.active)
        {
            cdTime = curMergeItem.active_cost[1] * 60;
            cdCost= curMergeItem.cdspeed_cost;
        }
        else
        {
            cdTime =MergeManager.Instance.GetTimeProductCdTime(curMergeItem) ;

            cdCost= curMergeItem.time_cdspeed_cost;
        }
        if (cdCost == null || cdCost.Length < 2)
            return;
        
        if (cdCost[0] != 1)
            return;
        if (cdTime == 0)
            return;
        _cdCost =(int) Mathf.Ceil((float)leftCD/cdTime* cdCost[1]);
        _orgCdCost = cdCost[1];
        _speedUpType = type;
        diamondsText.SetText(_cdCost.ToString());
    }

    private void SetLockStatus(TableMergeItem itemConfig)
    {
        tipsBtn.gameObject.SetActive(true);
        
        int cost = itemConfig.unlcok_cost_net;
        speedBtn.gameObject.SetActive(cost > 0);
        
        diamondsText.SetText(cost.ToString());
        SetTipContentText(false,"item_note_vd_text30");
        speedBtnText.SetTerm("button_unlock");
    }
    public bool canShowBubbleRv;
    private void SetBubbleStatus(TableMergeItem itemConfig, StorageMergeItem boardItem)
    {
        int cost = itemConfig.unlcok_cost_bubble;
        speedBtn.gameObject.SetActive(cost > 0);
        diamondsText.SetText(cost.ToString());
        
        InvokeRepeating("Invoke_SetBubbleCdTime", 0f, 1f);
        
        if (boardItem.BubbleType == 1)
        {
            canShowBubbleRv = true;
        }
        else if(boardItem.BubbleType == 0)
        {
            speedBtn.gameObject.SetActive(true);
            canShowBubbleRv = false;
        }
        else
        {       
            canShowBubbleRv = false;
        }
        // SetTipContentText(false,"UI_info_text6");
        speedBtnText.SetTerm("button_open");
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBubbleView);
      
    }

    public bool canShowRv;
    private void SetRvStatus(bool active)
    {
        canShowRv = active;
        if (!active)
        {
            rvBtn.gameObject.SetActive(false);
            return;
        }
        
        bool isOpen = IsOpenRv() && AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_HAPPYGO_BULIDING_CD);
        rvBtn.gameObject.SetActive(isOpen);
    }
    private void SetSpeedUpStatus(TableMergeItem itemConfig, StorageMergeItem boardItem)
    {
        tipsBtn.gameObject.SetActive(true);
        
        int activeLeftCd = MergeManager.Instance.GetLeftActiveTime(selecGridIndex,MergeBoardEnum.HappyGo);
        if (activeLeftCd > 0)
        {
            SetRvStatus(false);
            SetSpeedUpPrice(SpeedUpType.active,activeLeftCd);
            speedBtn.gameObject.SetActive(true);
            SetSellButtonActive(false);
            speedBtnText.SetTerm("button_speedup");
            InvokeRepeating("Invoke_SetActiveCdTime", 0f, 1f);
            SetAdSpeedUpStatus();
            return;
        }
        
        int isInCd = MergeManager.Instance.GetLeftProductTime(selecGridIndex,MergeBoardEnum.HappyGo) ;
        float pre = 0;
        int timeProductCd = MergeManager.Instance.GetTimeProductCd(selecGridIndex, ref pre,MergeBoardEnum.HappyGo);
        if (isInCd > 0 || timeProductCd >0 && boardItem.ProductItems.Count<=0)
        {
            if (isInCd > 0)
            {
                speedBtn.gameObject.SetActive(true);
                SetSellButtonActive(false);
                SetSpeedUpPrice(SpeedUpType.product,isInCd);
                SetRvStatus(true);
                speedBtnText.SetTerm("button_speedup");
                InvokeRepeating("Invoke_SetProductCdTime", 0f, 1f);
                SetAdSpeedUpStatus(isInCd);
            }
            else if (timeProductCd >0&& boardItem.ProductItems.Count<=0)
            {
                speedBtn.gameObject.SetActive(true);
                SetSellButtonActive(false);
                SetSpeedUpPrice(SpeedUpType.timeProduct,timeProductCd);
                SetRvStatus(true);
                speedBtnText.SetTerm("button_speedup");
                InvokeRepeating("Invoke_SetTimeProductCdTime", 0f, 1f);
                SetAdSpeedUpStatus();
            }
        }
        else
        {
            speedBtn.gameObject.SetActive(false);
            sellBtnText.SetTerm("button_sell");
            SetSellButtonActive(true);
            coinText.SetText(itemConfig.sold_gold.ToString());
        }
    }

    private void SetAdSpeedUpStatus(int leftCD=0)
    {
        if (_speedUpType == SpeedUpType.timeProduct)
        {
            rvBtnDesText.SetTerm("UI_common_free");
            return;
        }
        if(leftCD<=0)
            return;
        int skipCd = Mathf.CeilToInt(1.0f * AdConfigHandle.Instance.GetRvSpeedUpFactor() / curMergeItem.cdspeed_cost[1] * MergeManager.Instance.GetProductCdTime(curMergeItem,boardItem,_boardId));
        int totalNum = Mathf.CeilToInt(1.0f * (leftCD+boardItem.PlayRvNum*skipCd) /skipCd);
        int curNum = boardItem.PlayRvNum;
        
        if (totalNum <= 1 )
        {
            rvBtnDesText.SetTerm("UI_common_free");
        }
        else
        {
            rvBtnDesText.SetText(curNum + "/" + totalNum);
        }
    }
    private void InitContentText(TableMergeItem itemConfig)
    {   
        titleText.SetTerm(itemConfig.name_key);
        string txt = LocalizationManager.Instance.GetLocalizedString(itemConfig.item_des);
        SetTipContentText(false,txt);
        if (!txt.Contains("{0}"))
            return;
        string msg = string.Format(txt, itemConfig.value);
        SetTipContentText(false,msg);
        if (itemConfig.type == 2 && itemConfig.active_cost.Length == 2)
        {
            msg = string.Format(txt, itemConfig.active_cost[1]);
            if (itemConfig.active_cost[0] == 4 && MergeManager.Instance.IsHaveOpenBox(MergeBoardEnum.HappyGo) &&
                MergeManager.Instance.IsActiveItem(selecGridIndex,MergeBoardEnum.HappyGo))
            {
                msg = "item_note_text32";
                InvokeRepeating("Invoke_OtherItemActiveStatus", 0f, 1f);
            }
              
            SetTipContentText(false,msg);
            return;
        }
        else if (itemConfig.type == 5 ||itemConfig.type == 6 ||itemConfig.type == 7  ||itemConfig.type == 8 ||itemConfig.type == 14 ||itemConfig.type == 17||itemConfig.type == 902)
        {
            msg = string.Format(txt, itemConfig.value);
            SetTipContentText(false,msg);
            return;
        }
        if (itemConfig.type == 10)
        {
            msg = string.Format(txt, itemConfig.booster_factor/60);
            SetTipContentText(false,msg);
            return;
        }
        var nextItem=GameConfigManager.Instance.GetItemConfig(itemConfig.next_level);
        if (nextItem != null)
            txt = string.Format(txt, LocalizationManager.Instance.GetLocalizedString(nextItem.name_key));
        SetTipContentText(false,txt);
    }
    
    private void SetSellButtonActive(bool active)
    {
        if (!active||!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ItemSell))
        {
            sellBtn.gameObject.SetActive(false);
            destoryBtn.gameObject.SetActive(false);
            return;
        }
        
        sellBtn.gameObject.SetActive( MergeManager.Instance.IsCanSold(selecGridId) && curMergeItem.sold_gold>0 &&GuideSubSystem.Instance.isFinished(1008));
        destoryBtn.gameObject.SetActive(MergeManager.Instance.IsCanSold(selecGridId) && curMergeItem.sold_gold<=0&&GuideSubSystem.Instance.isFinished(1008));

    }
    
    private void SetTimeStatus(TableMergeItem itemConfig)
    {
        ActiveCostType type = MergeConfigManager.Instance.GetActiveCostType(selecGridId);
        switch (type)
        {
            case ActiveCostType.coin:
                SetSellButtonActive(true);
                speedBtn.gameObject.SetActive(false);
                break;
            case ActiveCostType.diamonds:
                SetSellButtonActive(true);
                speedBtn.gameObject.SetActive(true);
                break;
            case ActiveCostType.time_active:
                timeBtn.gameObject.SetActive(false);
                SetSellButtonActive(true);
                break;
            case ActiveCostType.time_inactive:
            {
                SetSellButtonActive(true);
                speedBtn.gameObject.SetActive(false);
                timeBtn.gameObject.SetActive(true && !MergeManager.Instance.IsHaveOpenBox(MergeBoardEnum.HappyGo));
                timeText.SetText(string.Format("{0} "+LocalizationManager.Instance.GetLocalizedString("UI_common_time_min"), itemConfig.active_cost[1]));
                break;
            }
        }

        speedBtnText.SetTerm("button_open");
    }
    
    public void SetNoFocusStatus()
    {
        InitView();
        
        CancelInvoke("Invoke_SetBubbleCdTime");
        CancelInvoke("Invoke_SetActiveCdTime");
        CancelInvoke("Invoke_SetProductCdTime");
        CancelInvoke("Invoke_SetTimeProductCdTime");
        CancelInvoke("Invoke_OtherItemActiveStatus");
        
        selecGridIndex = -1;
        selecGridId = -1;
        curMergeItem = null;
        boardItem = null;
    }

    public void OnSelectGrid()
    {
        OnSelectGrid(new Vector2Int(selecGridIndex, selecGridId));
    }

    private void OnClickSpeed()
    {
        OnSpeedUp();
    }

    public void OnClickTime()
    {
        ClickTime(0);
    }

    private void ClickTime(int subTime = 0)
    {
        var boardItem = MergeManager.Instance.GetBoardItem(selecGridIndex,MergeBoardEnum.HappyGo);
        ActiveCostType type = MergeConfigManager.Instance.GetActiveCostType(boardItem.Id);
        if (type == ActiveCostType.time_inactive && MergeManager.Instance.IsHaveOpenBox(MergeBoardEnum.HappyGo))
        {
            //TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text32"), timeBtn.transform);
            return;
        }

        boardItem.ActiveTime = APIManager.Instance.GetServerTime() / 1000 - (ulong) subTime;
        MergeManager.Instance.SetBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.notDeal,MergeBoardEnum.HappyGo);
        OnSelectGrid();
    }

    public void OnSpeedUp(bool useRv = false)
    {
        int cost = 0;
        string openSr="diamond_lack_building_cd_happygo";
        var boardItem =MergeManager.Instance.GetBoardItem(selecGridIndex,MergeBoardEnum.HappyGo);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(boardItem.Id);
        if (itemConfig == null)
            throw new Exception("加速或者解锁物品id出错：" + boardItem.Id);
        var reason = new GameBIManager.ItemChangeReasonArgs();
        if(useRv)
            boardItem.PlayRvNum++;
        
        if (boardItem.State == (int) MergeItemStatus.locked)
        {
            cost = itemConfig.unlcok_cost_net;
            openSr = "diamond_lack_spider";
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, cost))
            {
                ActiveCostType activeType = MergeConfigManager.Instance.GetActiveCostType(boardItem.Id);
                if (activeType > 0)
                {
                    MergeManager.Instance.SetNewBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.webUnlock,MergeBoardEnum.HappyGo);
                }
                else
                {
                    MergeManager.Instance.SetBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.webUnlock,MergeBoardEnum.HappyGo);
                }
                HappyGoMainController.Instance.mMergeBoard.TryOpenBoxes(selecGridIndex);
                HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlayAnimator("Lock_Open", true);
            }

            // reason.reason = BiEventCooking.Types.ItemChangeReason.UnlockMergeItem;
        }
        else if (boardItem.State == (int) MergeItemStatus.open)
        {
            if (useRv)
            {
                if (itemConfig.type == 2)
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChestClickRv);
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBuildingsCdClickRv);

                var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup,
                    ADConstDefine.RV_HAPPYGO_BULIDING_CD);
                if (rv == null)
                    return;
                float percent = 0;
                if (_speedUpType == SpeedUpType.timeProduct)
                {
                    MergeManager.Instance.SetTimeProductStoreCount(boardItem,true);
                    MergeManager.Instance.ResetTimeProductCD(SelectIndex,MergeBoardEnum.HappyGo);
                    HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () =>
                    {
                    });
                }
                else
                {
                 
                    int cd = MergeManager.Instance.GetLeftProductTime(selecGridIndex, ref percent,MergeBoardEnum.HappyGo);
              
                    int skipCd = Mathf.CeilToInt(1.0f * AdConfigHandle.Instance.GetRvSpeedUpFactor() / curMergeItem.cdspeed_cost[1] *MergeManager.Instance.GetProductCdTime(curMergeItem,boardItem,_boardId));
                    skipCd = skipCd >= cd ? cd : skipCd;
                    MergeManager.Instance.SpeedUpOneItem(boardItem, (ulong) skipCd);
                    HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv);
            
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBuildingsCdSuccessRv,
                        itemConfig.id.ToString()); 
                }
               
            }
            else
            {
                cost = _cdCost;
                if (itemConfig.type == 2)
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChestClickGem);
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBuildingsCdClickGem);
                switch (_speedUpType)
                {
                    case SpeedUpType.product:
                        openSr = "diamond_lack_building_cd";
                        break;  
                    case SpeedUpType.active:
                        openSr = "diamond_lack_box_cd";
                        break; 
                    case SpeedUpType.timeProduct:
                        openSr = "diamond_lack_building_cd";
                        break;
                }
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, _cdCost))
                {
                    if (_speedUpType == SpeedUpType.active)
                    {
                        MergeManager.Instance.ResetActiveCD(SelectIndex,MergeBoardEnum.HappyGo);
                        HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () =>
                        {
                       
                        });
                    }
                    else if (_speedUpType == SpeedUpType.product)
                    {
                        MergeManager.Instance.SetOrginalStoreCount(boardItem, false);
                        MergeManager.Instance.ResetProductCD(SelectIndex,MergeBoardEnum.HappyGo);
                  
                        HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () =>
                        {
                       
                        });
                   
                    }
                    else
                    {
                        MergeManager.Instance.SetTimeProductStoreCount(boardItem);
                        MergeManager.Instance.ResetTimeProductCD(SelectIndex,MergeBoardEnum.HappyGo);
                        HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () =>
                        {
                  
                        });
                    }
               
                 
                    SetSpeedStatus();
                    GameBIManager.Instance.SendGameEvent(
                        BiEventCooking.Types.GameEventType.GameEventBuildingsCdSuccessGem, itemConfig.id.ToString());
                }
            }

            reason.reason = BiEventCooking.Types.ItemChangeReason.SpeedUp;
        }
        else if (boardItem.State == (int) MergeItemStatus.bubble)
        {
            cost = itemConfig.unlcok_cost_bubble;
            string biType = "";
            if (boardItem.BubbleType == 1 || useRv)
            {
                cost = 0;
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBubbleClickRv);
                biType = "rv";
            }
            else
            {
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBubbleClickGem);
                biType = "gem";
            }

            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, cost) || useRv)
            {
                MergeManager.Instance.ResumAllCdTime(MergeBoardEnum.HappyGo);
                MergeManager.Instance.SetBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.webUnlock,MergeBoardEnum.HappyGo);
                if (itemConfig.sold_gold > 0)
                    coinText.transform.parent.gameObject.SetActive(true);
                CancelInvoke("Invoke_SetBubbleCdTime");
                SetSpeedStatus();
                //AudioManager.Instance.PlaySound(HotelSound.mhm_bubble_open);
                HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlayBubbleAnimation("disappear");
                SendBubbleOpen(selecGridId, biType);
            }

            reason.reason = BiEventCooking.Types.ItemChangeReason.BreakBubble;
        }
        else if (boardItem.State == (int) MergeItemStatus.time)
        {
            ActiveCostType type = MergeConfigManager.Instance.GetActiveCostType(boardItem.Id);
            bool isCanSpeed = false;
            if (type == ActiveCostType.diamonds)
            {
                cost = _cdCost;
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, cost))
                {
                    
                    HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv);

                    isCanSpeed = true;
                }
            }
            else if (type == ActiveCostType.coin)
            {
                cost = 0;
                if (UserData.Instance.CanAford(UserData.ResourceId.Coin, itemConfig.active_cost[1]))
                {
                    isCanSpeed = true;
                }
            }

            if (useRv)
            {
                isCanSpeed = true;
            }

            if (isCanSpeed)
            {
                SetSpeedStatus();
    

            }

            reason.reason = BiEventCooking.Types.ItemChangeReason.SpeedUp;
        }

        OnSelectGrid();
        if (useRv)
            cost = 0;
        if (cost <= 0)
            return;
        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, cost))
        {
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                curMergeItem.id.ToString(), openSr,needCount:cost);
        }
        else
        {
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, cost, reason);
        }
        AudioManager.Instance.PlaySound(41);
    }

    private void SetSpeedStatus()
    {
        speedBtn.gameObject.SetActive(false);
        SetSellButtonActive(true);
        SetRvStatus(false);
        timeBtn.gameObject.SetActive(false);
        canShowBubbleRv = false;
        rvBtn2.gameObject.SetActive(false);
    }

    private void OnClickTipsBtn()
    {
        bool isBox = MergeManager.Instance.IsBox(selecGridIndex,MergeBoardEnum.HappyGo);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
        if (selecGridId <= 0 || isBox || itemConfig == null)
            return;
        MergeInfoView.Instance.OpenMergeInfo(itemConfig,isShowGetResource:false);

    }

    private void OnClickSell()
    {
      
        if (Time.time - sellColdTime < 0.8f)
            return;
        if (!MergeManager.Instance.IsBoardItemExist(selecGridIndex,MergeBoardEnum.HappyGo))
            return;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
        bool isOpen =MergeManager.Instance.IsOpen(selecGridIndex,MergeBoardEnum.HappyGo);
        if (!isOpen)
            return;
        if (itemConfig.sold_confirm)
        {
            var warningView = UIManager.Instance.OpenUI(UINameConst.UIPopupMergeWarning) as UIPopupMergeWarningController;

            warningView.InitPackageUnit(selecGridId);
            warningView.OnSellItem = () => { SellItem(); };
        }
        else
        {
            SellItem();
        }
    }

    private void SellItem()
    {
        int sellIndex = selecGridIndex;
        SetUndoStatus();
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELL_ITEM,BoardId, selecGridIndex);
        MergeManager.Instance.RemoveBoardItem(sellIndex,MergeBoardEnum.HappyGo,"HappyGo",false);
        StartCoroutine(OnSellItem(sellIndex));

        sellColdTime = Time.time;
        AudioManager.Instance.PlaySound(19);
    }

    IEnumerator OnSellItem(int index)
    {
        yield return new WaitForSeconds(0.3f);
        SendSellBI(selecGridId);
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardEnum.HappyGo,index, -1,RefreshItemSource.notDeal);
    }

    private void SetUndoStatus()
    {
        var sell = MergeManager.Instance.GetBoardItem(selecGridIndex,MergeBoardEnum.HappyGo);
        sellItem = MergeManager.Instance.GetEmptyItem();
        sellItem.Id = sell.Id;
        sellItem.OpenTime = sellItem.OpenTime;
        sellItem.ProductCount = sell.ProductCount;
        sellItem.StoreMax = sell.StoreMax;
        sellItem.TimeStoreMax = sell.TimeStoreMax;
        for (int i = 0; i < sell.ProductItems.Count; i++)
        {
            sellItem.ProductItems.Add(sell.ProductItems[i]);
        }

        sellItem.ProductTime = sell.ProductTime;
        sellItem.TimProductTime = sell.TimProductTime;
        sellItem.State = sell.State;
        sellItem.InCdTime = sell.InCdTime;

        speedBtn.gameObject.SetActive(false);
        SetRvStatus(false);
        timeBtn.gameObject.SetActive(false);
        discomposeBtn.gameObject.SetActive(false);
        SetSellButtonActive(false);
        undoBtn.gameObject.SetActive(true);
        SetTipContentText(false,"UI_info_text2");
    }

    private void UndoOperate()
    {
        if (Time.time - sellColdTime < 0.8f)
            return;
        if (!UserData.Instance.CanAford(UserData.ResourceId.Coin, curMergeItem.sold_gold))
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString =
                    LocalizationManager.Instance.GetLocalizedString("&key.ui_sell_withdraw_desc"),
                OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok")
            });
            return;
        }
        var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
        if (itemConfig != null)
            CurrencyGroupManager.Instance.CostCurrency(UserData.ResourceId.Coin, itemConfig.sold_gold,
                new GameBIManager.ItemChangeReasonArgs
                    {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.UdosaleMergeItem});
        bool isExist =MergeManager.Instance.IsBoardItemExist(selecGridIndex, MergeBoardEnum.HappyGo);
        if(isExist)
        {
            SetNoFocusStatus();
            MergeManager.Instance.AddRewardItem(sellItem,MergeBoardEnum.HappyGo,1);
            var endTrans = HappyGoMainController.Instance.rewardBtnTrans;
            FlyGameObjectManager.Instance.FlyObject(sellItem.Id, Vector3.zero, endTrans, 1.1f, 0.8f, () =>
            {
                FlyGameObjectManager.Instance.PlayHintStarsEffect(endTrans.position);
                Animator shake = endTrans.transform.GetComponent<Animator>();
                if (shake != null)
                    shake.Play("shake", 0, 0);
               
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
            }, true, 1.0f, 0.7f);
        }
        else
        {
            MergeManager.Instance.SetBoardItem(selecGridIndex, sellItem, RefreshItemSource.notDeal,MergeBoardEnum.HappyGo);
      
            OnSelectGrid(new Vector2Int(selecGridIndex, selecGridId));
            HappyGoMainController.Instance.mMergeBoard.SelectFocus(selecGridIndex, false, true);
            SendUndoBI(selecGridId);
            RefreshAllStatus();
        }

    }

    private void SetTipContentText(bool isInCd,string tex="")
    {
        contentText.transform.parent.gameObject.SetActive(!isInCd);
        cdGroup.gameObject.SetActive(isInCd);
        if (isInCd)
        {
            cdText.SetText(tex);
        }
        else
        {
            if(!string.IsNullOrEmpty(tex))
                contentText.SetTerm(tex);
        }

    }

    

    private void Invoke_SetBubbleCdTime()
    {
        if (selecGridIndex < 0)
        {
            CancelInvoke("Invoke_SetBubbleCdTime");
            return;
        }
        int cd = MergeManager.Instance.GetBubbleLeftCdTime(selecGridIndex,MergeBoardEnum.HappyGo);
        if (cd == 0)
        {
            SetTipContentText(false,"");
            speedBtn.gameObject.SetActive(false);
            SetRvStatus(false);
            bubbleDissmiss = StartCoroutine(SetBubbleDismiss());
            CancelInvoke("Invoke_SetBubbleCdTime");
            RefreshAllStatus();
        }
        else
        {
            string timeString = TimeUtils.GetTimeString(cd);
            SetTipContentText(true,timeString);
        }
    }

    private void Invoke_SetProductCdTime()
    {
        if (selecGridIndex < 0)
        {
            CancelInvoke("Invoke_SetProductCdTime");
            return;
        }
        float percent = 0;
        int cd = MergeManager.Instance.GetLeftProductTime(selecGridIndex, ref percent,MergeBoardEnum.HappyGo);
        if (cd == 0)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
            if (itemConfig != null)
            {
                string txt = LocalizationManager.Instance.GetLocalizedString(itemConfig.item_des);
                var nextItem=GameConfigManager.Instance.GetItemConfig(itemConfig.next_level);
                if (nextItem != null)
                    txt = string.Format(txt, LocalizationManager.Instance.GetLocalizedString(nextItem.name_key));
                    
                SetTipContentText(false,txt);
                RefreshAllStatus();
            }

            CancelInvoke("Invoke_SetProductCdTime");
        }
        else
        {
            string timeString = TimeUtils.GetTimeString(cd);
            SetSpeedUpPrice(SpeedUpType.product, cd);
            SetAdSpeedUpStatus();
            SetTipContentText(true,timeString);
        }
    }

    private void Invoke_OtherItemActiveStatus()
    {
        if (!MergeManager.Instance.IsHaveOpenBox(MergeBoardEnum.HappyGo))
        {
            CancelInvoke("Invoke_OtherItemActiveStatus");
            RefreshAllStatus();
        }
    }

    private void Invoke_SetTimeProductCdTime()
    {
        if (selecGridIndex < 0)
        {
            CancelInvoke("Invoke_SetTimeProductCdTime");
            return;
        }
        float percent = 0;
        int cd = MergeManager.Instance.GetTimeProductCd(selecGridIndex, ref percent,MergeBoardEnum.HappyGo);

        if (cd == 0)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
            if (itemConfig != null)
            {
                string txt = LocalizationManager.Instance.GetLocalizedString(itemConfig.item_des);
                var nextItem=GameConfigManager.Instance.GetItemConfig(itemConfig.next_level);
                if (nextItem != null)
                    txt = string.Format(txt, LocalizationManager.Instance.GetLocalizedString(nextItem.name_key));

                SetTipContentText(false,string.Format(txt, txt));
            
            }

            OnSelectGrid();
            CancelInvoke("Invoke_SetTimeProductCdTime");
          
        }
        else
        {
            string txt = LocalizationManager.Instance.GetLocalizedString("item_note_text29");
            string timeString = TimeUtils.GetTimeString(cd);
            SetSpeedUpPrice(SpeedUpType.timeProduct, cd);
            SetAdSpeedUpStatus();
            SetTipContentText(true, timeString);
        }
    }

    IEnumerator SetBubbleDismiss()
    {
        yield return new WaitForSeconds(0.9f);
        HappyGoMainController.Instance.mMergeBoard.SelectFocus(selecGridIndex, false, false);
        bubbleDissmiss = null;
    }

    private void Invoke_SetActiveCdTime()
    {
        if (selecGridIndex < 0)
        {
            CancelInvoke("Invoke_SetActiveCdTime");
            return;
        }
        int activeCd = MergeManager.Instance.GetLeftActiveTime(selecGridIndex,MergeBoardEnum.HappyGo);
        if (activeCd == 0)
        {
            cdGroup.gameObject.SetActive(false);
            speedBtn.gameObject.SetActive(false);
            SetRvStatus(false);
            OnSelectGrid();
            CancelInvoke("Invoke_SetActiveCdTime");
        }
        else
        {
            string timeString = TimeUtils.GetTimeString(activeCd);
            SetSpeedUpPrice(SpeedUpType.active,activeCd);
            SetAdSpeedUpStatus();
            SetTipContentText(true, timeString);
        }
    }

    private bool IsOpenRv()
    {
        if (MergeConfigManager.Instance.IsOpenRvSpeedUp(selecGridId)) // 该物品不能使用rv加速ß
            return true;
        
        return false;
    }

    private void DecomposItem() // 分解物品  类型为1 并且等级大于2
    {
        if (!IsCanDecompos())
            return;
        int gridIndex = MergeManager.Instance.FindEmptyGrid(1,MergeBoardEnum.HappyGo);
        if (gridIndex == -1)
        {
            //TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text22"), discomposeBtn.transform);
            return;
        }

        var window = UIManager.Instance.OpenUI(UINameConst.MergeSplite) as MergeSpliteController;
        window.SetItemId(selecGridId);
        window.onSpliteCallBack = (rv) =>
        {
            var grid = HappyGoMainController.Instance.mMergeBoard.GetGridByIndex(selecGridIndex);
            grid?.board.PlaySpilteVfx();
            AudioManager.Instance.PlaySound("mhm_slipt");
            grid?.board.SpliteItem();
        };
    }

    private bool IsCanDecompos() // 能否分解  类型为1 并且等级大于2 有1空格子
    {
        return false;
        bool result = false;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
        if (itemConfig == null)
            return result;
        if (itemConfig.level < 2)
            return result;
        bool isOpen = MergeManager.Instance.IsOpen(selecGridIndex,MergeBoardEnum.HappyGo);
        if (!isOpen)
            return result;
        result = itemConfig.Gem_split > 0;
        // if(result)
        //     result = UnlockManager.IsOpen(UnlockManager.MergeUnlockType.SplitUnlockLevel);
        return result;
    }


    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BORAD_SELECTED_GRID, OnSelectGrid);
    }

    private void SendSellBI(int id)
    {
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeSaleItem,
            itemAId = config.id,
            ItemALevel = config.level,
            isChange = true,
            data1 = "1",
            extras = new Dictionary<string, string>
            {
                {"coin", config.sold_gold.ToString()},
            }
        });
    }

    private void SendUndoBI(int id)
    {
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeUdosaleItem,
            itemAId = config.id,
            ItemALevel = config.level,
            isChange = true,
            data1 = "1",
            extras = new Dictionary<string, string>
            {
                {"coin", config.sold_gold.ToString()},
            }
        });
    }

    private void SendBubbleOpen(int id, string biType)
    {
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemConfig == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeBubble,
            itemAId = itemConfig.id,
            ItemALevel = itemConfig.level,
            isChange = true,
            data1 = "1",
            extras = new Dictionary<string, string>
            {
                {"type", biType}
            }
        });
    }

    private void OnApplicationFocus(bool hasFocus)
    {
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (this == null)
            return;

        if (pauseStatus)
            return;
        
        MergeManager.Instance.ResumAllCdTime(MergeBoardEnum.HappyGo);
        HappyGoMainController.Instance.mMergeBoard?.RefreshGridsStatus();
        DebugUtil.LogError("恢复所有气泡的时间");
    }
}