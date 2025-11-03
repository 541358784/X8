using System;
using System.Collections;
using System.Collections.Generic;
using ABTest;
using Activity.BalloonRacing;
using Activity.RabbitRacing.Dynamic;
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
using Gameplay.UI.EnergyTorrent;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MergeClickTips : MonoBehaviour
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
    private LocalizeTextMeshProUGUI contentText;
    private Button tipsBtn;
    private LocalizeTextMeshProUGUI coinText;
    private LocalizeTextMeshProUGUI diamondsText;
    private Transform cdGroup;
    private LocalizeTextMeshProUGUI cdContentText;
    private LocalizeTextMeshProUGUI cdText;
    private LocalizeTextMeshProUGUI timeText;
    private LocalizeTextMeshProUGUI sellBtnText;
    private LocalizeTextMeshProUGUI titleText;

    private LocalizeTextMeshProUGUI speedBtnText;
    private Button sellBtn;
    private Button destoryBtn;
    private Button speedBtn;
    private Button timeBtn;
    private Button undoBtn;
    private Button thumbtackBtn;

    private Button rvBtn;
    private Button rvBtn2;
    private LocalizeTextMeshProUGUI rvBtnText;
    private LocalizeTextMeshProUGUI rvBtnDesText;

    private int oldSelectIndex = -1;
    private int selecGridIndex = -1;
    private int selecGridId = -1;

    private StorageMergeItem sellItem;
    float sellColdTime = 0;
    private Coroutine bubbleDissmiss;

    private TableMergeItem curMergeItem = null;
    private StorageMergeItem boardItem = null;
    
    private GameObject explainGroup;
    private LocalizeTextMeshProUGUI explainText;
    private LocalizeTextMeshProUGUI explainTitleText;

    private Transform energyBG;
    public int SelectIndex
    {
        get { return selecGridIndex; }
    }

    private GameObject _itemGroup1;
    private GameObject _itemGroup2;
    private Image _itemGroup1Image;
    private Image _itemGroup2Image;
    private LocalizeTextMeshProUGUI _itemGroup1Text;
    private LocalizeTextMeshProUGUI _itemGroup2Text;
    private Button _itemGroup1Button;
    private Button _itemGroup2Button;   
    private GameObject _itemGroup1Finish;
    private GameObject _itemGroup2Finish;

    private void Awake()
    {
        contentText = transform.Find("ContentTextGroup/ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        contentText.transform.parent.gameObject.SetActive(true);
        titleText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        cdGroup = transform.Find("CdGroup");
        cdContentText = transform.Find("CdGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        cdContentText.SetTerm("item_note_text31");
        cdText = transform.Find("CdGroup/CdText").GetComponent<LocalizeTextMeshProUGUI>();
        explainGroup = transform.Find("ExplainGroup").gameObject;
        explainText = transform.Find("ExplainGroup/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        explainTitleText= transform.Find("ExplainGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        tipsBtn = transform.Find("TipBtn").GetComponent<Button>();

        sellBtn = transform.Find("SellBtn").GetComponent<Button>();
        destoryBtn = transform.Find("DestroyBtn").GetComponent<Button>();
        coinText = transform.Find("SellBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
        sellBtnText = sellBtn.transform.Find("SellText").GetComponent<LocalizeTextMeshProUGUI>();
        energyBG = transform.Find("EnergyBG");
        speedBtn = transform.Find("SpeedBtn").GetComponent<Button>();
        diamondsText = transform.Find("SpeedBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
        speedBtnText = speedBtn.transform.Find("SpeedUpText").GetComponent<LocalizeTextMeshProUGUI>();

        timeBtn = transform.Find("TimeBtn").GetComponent<Button>();
        timeText = transform.Find("TimeBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
        undoBtn = transform.Find("UndoBtn").GetComponent<Button>();
        thumbtackBtn = transform.Find("ThumbtackBtn").GetComponent<Button>();
        _itemGroup1 = transform.Find("ItemGroup1").gameObject;
        _itemGroup1Image = transform.Find("ItemGroup1/icon").GetComponent<Image>();
        _itemGroup1Text = transform.Find("ItemGroup1/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _itemGroup1Button = transform.Find("ItemGroup1/HelpButton").GetComponent<Button>();
        _itemGroup1Finish = transform.Find("ItemGroup1/Finish").gameObject;
        _itemGroup1Button.onClick.AddListener(() =>
        {
            var produceCost = MergeManager.Instance.GetProduceCost(curMergeItem);
            if(curMergeItem != null && produceCost != null && produceCost.Length > 0)
                MergeInfoView.Instance.OpenMergeInfo(produceCost[0], isShowGetResource:true,_isShowProbability:true);
        });
        
        _itemGroup2Finish = transform.Find("ItemGroup2/Finish").gameObject;
        _itemGroup2 = transform.Find("ItemGroup2").gameObject;
        _itemGroup2Image = transform.Find("ItemGroup2/icon").GetComponent<Image>();
        _itemGroup2Text = transform.Find("ItemGroup2/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _itemGroup2Button = transform.Find("ItemGroup2/HelpButton").GetComponent<Button>();
        _itemGroup2Button.onClick.AddListener(() =>
        {
            var produceCost = MergeManager.Instance.GetProduceCost(curMergeItem);
            if(curMergeItem != null && produceCost != null && produceCost.Length > 1)
                MergeInfoView.Instance.OpenMergeInfo(produceCost[1], isShowGetResource:true,_isShowProbability:true);
        });
        
        sellBtn.onClick.AddListener(OnClickSell);
        destoryBtn.onClick.AddListener(OnClickSell);
        speedBtn.onClick.AddListener(OnClickSpeed);
        timeBtn.onClick.AddListener(OnClickTime);
        undoBtn.onClick.AddListener(UndoOperate);
        thumbtackBtn.onClick.AddListener(Thumbtack);
        tipsBtn.onClick.AddListener(OnClickTipsBtn);

        rvBtn = transform.Find("Rv1").GetComponent<Button>();
        UIAdRewardButton.Create(ADConstDefine.RV_BULIDING_CD, UIAdRewardButton.ButtonStyle.Hide, rvBtn.gameObject,
            (s) =>
            {
                if (s)
                    OnSpeedUp(true);
            }
            , true,
            onBtnClick: () => { MergeManager.Instance.PauseAllCdTime(BoardId); }
            , check: () =>
            {
                return IsOpenRv() && canShowRv&&!ABTestManager.Instance.IsOpenADTest(); 
            }
        );
        
        rvBtn2 = transform.Find("Rv2").GetComponent<Button>();
        UIAdRewardButton.Create(ADConstDefine.RV_BUBBLE_OPEN, UIAdRewardButton.ButtonStyle.Hide, rvBtn2.gameObject,
            (s) =>
            {
                if (s) 
                    OnSpeedUp(true);
            }
            , true, 
            onBtnClick: () => { MergeManager.Instance.PauseAllCdTime(BoardId); } 
            ,check: () => { return canShowBubbleRv && !ABTestManager.Instance.IsOpenADTest();}
        );
        rvBtnText = transform.Find("Rv1/Text").GetComponent<LocalizeTextMeshProUGUI>();
        rvBtnDesText = transform.Find("Rv1/VideoText").GetComponent<LocalizeTextMeshProUGUI>();

        
        List<Transform> topLayer1 = new List<Transform>();
        topLayer1.Add(timeBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BoxOpen, timeBtn.transform as RectTransform, topLayer:topLayer1);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(speedBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BoxSpeedUp, speedBtn.transform as RectTransform, topLayer:topLayer);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CdProductSpeedUp, speedBtn.transform as RectTransform, topLayer:topLayer);

        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_SELECTED_GRID, OnSelectGrid);
        
        
        List<Transform> topLayer2 = new List<Transform>();
        topLayer2.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClickInfoButton, tipsBtn.transform as RectTransform, topLayer:topLayer2);
    }



    private void Start()
    {
        OnSelectGrid(Vector2Int.zero);

        MergeManager.Instance.ResumAllCdTime(BoardId);
        
        explainTitleText.SetTerm("UI_info_text36");
    }

    private void InitView()
    {
        SetSellButtonActive(false);
        speedBtn.gameObject.SetActive(false);
        timeBtn.gameObject.SetActive(false);
        undoBtn.gameObject.SetActive(false);
        thumbtackBtn.gameObject.SetActive(false);
        tipsBtn.gameObject.SetActive(false);
        rvBtn.gameObject.SetActive(false);
        SetTipContentText(false, "UI_info_text1");
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
        int cdTime = MergeManager.Instance.GetLeftProductTime(index, ref percent,BoardId);
        string time = $"{cdTime / 3600:00}{"h "}{((cdTime % 3600) / 60):00}{"m"}";
        DebugUtil.Log("selectIndex:" + arg[0] + "---selectId:" + arg[1] + "---cd:" + time);
#endif
        oldSelectIndex = selecGridIndex;
        selecGridIndex = index;
        selecGridId = id;
        MergeMainController.Instance.selcedItemId = selecGridId;

        RefreshViewInfos();
    }

    private void RefreshViewInfos()
    {
        curMergeItem = GameConfigManager.Instance.GetItemConfig(selecGridId);
        boardItem = MergeManager.Instance.GetBoardItem(selecGridIndex,BoardId);

        bool isExist = MergeManager.Instance.IsBoardItemExist(selecGridIndex,BoardId);
        if (curMergeItem == null || !isExist || (boardItem != null && boardItem.State == (int) MergeItemStatus.box))
        {
            SetNoFocusStatus();
            return;
        }

        bool isEnergyTorrent = EnergyTorrentModel.Instance.IsOpen() &&
                               MergeConfigManager.Instance.IsEnergyTorrentProduct(curMergeItem);
        energyBG.gameObject.SetActive(isEnergyTorrent);

        RefreshAllStatus();
    }

    private void RefreshAllStatus()
    {
        if (curMergeItem == null || boardItem == null)
            return;

        InitView();
        RefreshEatBuildView();
        CancelInvoke("Invoke_SetBubbleCdTime");
        CancelInvoke("Invoke_SetActiveCdTime");
        CancelInvoke("Invoke_SetProductCdTime");
        CancelInvoke("Invoke_SetTimeProductCdTime");
        CancelInvoke("Invoke_OtherItemActiveStatus");

        if (curMergeItem.canStacking)
        {
            explainGroup.SetActive(true);
            explainText.SetText(boardItem.StackNum.ToString());
        }
        else
        {
            explainGroup.SetActive(false);
        }
        InitContentText(curMergeItem);
        SetRvStatus(false);
        SetBubbleRvStatus(false);
        switch (boardItem.State)
        {
            case (int) MergeItemStatus.locked:
            {
                SetLockStatus(curMergeItem);
                break;
            }
            case (int) MergeItemStatus.bubble:
            {
                SetBubbleStatus(curMergeItem, boardItem);
                break;
            }
            case (int) MergeItemStatus.open:
            {
                SetSpeedUpStatus(curMergeItem, boardItem);
                break;
            }
            case (int) MergeItemStatus.time:
            {
                var type = SetTimeStatus(curMergeItem);
                if (oldSelectIndex > 0 && oldSelectIndex == selecGridIndex && type == ActiveCostType.time_inactive)
                {
                    if (!MergeManager.Instance.IsHaveOpenBox(BoardId) && ExperenceModel.Instance.GetLevel() >= 3)
                    {
                        OnClickTime();
                    }
                }
                break;
            }
        }
    }
    private void RefreshEatBuildView()
    {
        _itemGroup1.gameObject.SetActive(false);
        _itemGroup2.gameObject.SetActive(false);
        if(curMergeItem.type != (int) MergeItemType.hamaster && curMergeItem.type != (int) MergeItemType.eatBuild )
            return;
        if(boardItem.State == 1)
            return;
   
    
        var produceCost = MergeManager.Instance.GetProduceCost(curMergeItem);
        if (produceCost!=null && produceCost.Length > 0)
        {
            TableMergeItem mergeItem1 = GameConfigManager.Instance.GetItemConfig(produceCost[0]);
            _itemGroup1.gameObject.SetActive(true);
            if (_itemGroup1Image.sprite == null || _itemGroup1Image.sprite != null && _itemGroup1Image.sprite.name != mergeItem1.image)
                _itemGroup1Image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem1.image);
            _itemGroup1Image.transform.DOScale(1.4f, 0.2f).onComplete = () =>
            {
                _itemGroup1Image.transform.localScale=Vector3.one;
            };
            _itemGroup1Finish.SetActive(MergeManager.Instance.IsBuildEat(boardItem,mergeItem1.id));
            if (produceCost.Length > 1)
            {
                TableMergeItem mergeItem2 = GameConfigManager.Instance.GetItemConfig(produceCost[1]);
                _itemGroup2.gameObject.SetActive(true);
                if (_itemGroup2Image.sprite == null || _itemGroup2Image.sprite != null && _itemGroup2Image.sprite.name != mergeItem2.image)
                    _itemGroup2Image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem2.image);
                _itemGroup2Image.transform.DOScale(1.4f, 0.2f).onComplete = () =>
                {
                    _itemGroup2Image.transform.localScale=Vector3.one;
                };
                _itemGroup2Finish.SetActive(MergeManager.Instance.IsBuildEat(boardItem,mergeItem2.id));
            }
            _itemGroup1Text.gameObject.SetActive(false);
            _itemGroup2Text.gameObject.SetActive(false);
        }
 
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
    // public bool timeInvokeEnable => MergeManager.Instance.MergeBoardID == (int)MergeBoardEnum.Main;

    public void SetSpeedUpPrice(SpeedUpType type, int leftCD)
    {
        if (curMergeItem == null)
            return;
        int[] cdCost;
        int cdTime = 0;
        if (type == SpeedUpType.product)
        {
            cdTime = MergeManager.Instance.GetProductCdTime(curMergeItem,boardItem,_boardId);
            cdCost = MergeManager.Instance.GetCdSpeedCost(curMergeItem,boardItem);
        }
        else if (type == SpeedUpType.active)
        {
            if (curMergeItem.active_cost.Length >= 2)
                cdTime = curMergeItem.active_cost[1] * 60;

            cdCost = curMergeItem.cdspeed_cost;
        }
        else
        {
            cdTime = MergeManager.Instance.GetTimeProductCdTime(curMergeItem);

            cdCost = curMergeItem.time_cdspeed_cost;
        }

        if (cdCost == null || cdCost.Length < 2)
            return;

        if (cdCost[0] != 1)
            return;
        if (cdTime == 0)
            return;
        _cdCost = (int) Mathf.Ceil((float) leftCD / cdTime * cdCost[1]);
        _orgCdCost = cdCost[1];
        _speedUpType = type;
        diamondsText.SetText(_cdCost.ToString());
        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.BoxSpeedUp) ||
            GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CdProductSpeedUp) || 
            GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.ChoseCdProduct))
        {
            diamondsText.SetText("0");
            _cdCost = 0;
        }
    }

    private void SetLockStatus(TableMergeItem itemConfig)
    {
        tipsBtn.gameObject.SetActive(true);

        int cost = itemConfig.unlcok_cost_net;
        speedBtn.gameObject.SetActive(cost > 0);

        diamondsText.SetText(cost.ToString());
        SetTipContentText(false, "item_note_text30");
        speedBtnText.SetTerm("button_unlock");
    }

    private void SetBubbleStatus(TableMergeItem itemConfig, StorageMergeItem boardItem)
    {
        int cost = itemConfig.unlcok_cost_bubble;
        speedBtn.gameObject.SetActive(cost > 0);
        diamondsText.SetText(cost.ToString());
        thumbtackBtn.gameObject.SetActive(true);
        InvokeRepeating("Invoke_SetBubbleCdTime", 0f, 1f);
        if (boardItem.BubbleType == 1)
        {
            SetBubbleRvStatus(true);
        }
        else if (boardItem.BubbleType == 0)
        {
            SetBubbleRvStatus(false);
        }
        else
        {
            SetBubbleRvStatus(false);
        }

        speedBtnText.SetTerm("button_open");
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBubbleView);
    }
    private void Thumbtack()
    {
        UIPopupThumbtackController controller =
            UIManager.Instance.OpenUI(UINameConst.UIPopupThumbtack) as UIPopupThumbtackController;
        controller.Init(() =>
        {
            SetTipContentText(false, "");
            speedBtn.gameObject.SetActive(false);
            thumbtackBtn.gameObject.SetActive(false);
            SetRvStatus(false);
            bubbleDissmiss =CoroutineManager.Instance.StartCoroutine(SetBubbleDismiss());
            CancelInvoke("Invoke_SetBubbleCdTime");
            MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.BubbleBreak();
            bubbleDissmiss =CoroutineManager.Instance.StartCoroutine(SetBubbleDismiss());
        },curMergeItem);
      
    }
    public bool canShowRv;
    public bool canShowBubbleRv;
    private void SetRvStatus(bool active)
    {
        canShowRv = active;
        if (!active)
        {
            rvBtn.gameObject.SetActive(false);
            return;
        }

        bool isOpen = IsOpenRv() && AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BULIDING_CD);
        rvBtn.gameObject.SetActive(isOpen);
    }
    private void SetBubbleRvStatus(bool active)
    {
        canShowBubbleRv = active;
        if (!active)
        {
            rvBtn2.gameObject.SetActive(false);
            return;
        }

        bool isOpen = AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BUBBLE_OPEN);
        rvBtn2.gameObject.SetActive(isOpen);
    }
    private void SetSpeedUpStatus(TableMergeItem itemConfig, StorageMergeItem boardItem)
    {
        tipsBtn.gameObject.SetActive(true);

        int activeLeftCd = MergeManager.Instance.GetLeftActiveTime(selecGridIndex,BoardId);
        if (activeLeftCd > 0)
        {
            SetRvStatus(false);
            SetSpeedUpPrice(SpeedUpType.active, activeLeftCd);
            speedBtn.gameObject.SetActive(true);
            SetSellButtonActive(false);
            speedBtnText.SetTerm("button_speedup");
            InvokeRepeating("Invoke_SetActiveCdTime", 0f, 1f);
            SetAdSpeedUpStatus();
            return;
        }

        int isInCd = MergeManager.Instance.GetLeftProductTime(selecGridIndex,BoardId);
            float pre = 0;
        int timeProductCd = MergeManager.Instance.GetTimeProductCd(selecGridIndex, ref pre,BoardId);
        if (isInCd > 0 || timeProductCd > 0 && boardItem.ProductItems.Count <= 0)
        {
            if (isInCd > 0)
            {
                speedBtn.gameObject.SetActive(true);
                SetSellButtonActive(false);
                SetSpeedUpPrice(SpeedUpType.product, isInCd);
                SetRvStatus(isInCd<=60*GlobalConfigManager.Instance.GetNumValue("clean_cd_unlock"));
                speedBtnText.SetTerm("button_speedup");
                InvokeRepeating("Invoke_SetProductCdTime", 0f, 1f);
                SetAdSpeedUpStatus(isInCd);
                if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CdProductSpeedUp))
                   GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CdProductSpeedUp, null);
            }
            else if (timeProductCd > 0 && boardItem.ProductItems.Count <= 0)
            {
                speedBtn.gameObject.SetActive(true);
                SetSellButtonActive(false);
                SetSpeedUpPrice(SpeedUpType.timeProduct, timeProductCd);
                SetRvStatus(timeProductCd<=60*GlobalConfigManager.Instance.GetNumValue("clean_cd_unlock"));
                speedBtnText.SetTerm("button_speedup");
                InvokeRepeating("Invoke_SetTimeProductCdTime", 0f, 1f);
                SetAdSpeedUpStatus(timeProductCd);
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

    private void SetAdSpeedUpStatus(int leftCD = 0)
    {
        int cdTime = MergeManager.Instance.GetProductCdTime(curMergeItem, boardItem,_boardId);
        var cost = MergeManager.Instance.GetCdSpeedCost(curMergeItem, boardItem);
        if (_speedUpType == SpeedUpType.timeProduct)
        {
            cost=curMergeItem.time_cdspeed_cost;
            cdTime = MergeManager.Instance.GetTimeProductCdTime(curMergeItem);
        }
       
            
        if (leftCD <= 0)
            return;
        
        int skipCd = Mathf.CeilToInt(1.0f * AdConfigHandle.Instance.GetRvSpeedUpFactor() / cost[1] * cdTime);
        int totalNum = Mathf.CeilToInt(1.0f * (leftCD + boardItem.PlayRvNum * skipCd) / skipCd);
        int curNum = boardItem.PlayRvNum;

        if (totalNum <= 1)
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
        string name = LocalizationManager.Instance.GetLocalizedString(itemConfig.name_key) + " " +
                      string.Format(LocalizationManager.Instance.GetLocalizedString("UI_item_level_value"),
                          itemConfig.level);
        titleText.SetTerm(name);
        string txt = "";
        if(!itemConfig.item_des.IsEmptyString())
            txt = LocalizationManager.Instance.GetLocalizedString(itemConfig.item_des);
        
        SetTipContentText(false, txt);
        if (!txt.Contains("{0}"))
            return;
        string msg = string.Format(txt, itemConfig.value);
        SetTipContentText(false, msg);
        if (itemConfig.type ==  (int) MergeItemType.box && itemConfig.active_cost.Length == 2)
        {
            msg = string.Format(txt, itemConfig.active_cost[1]);
            if (itemConfig.active_cost[0] == 4 && MergeManager.Instance.IsHaveOpenBox(BoardId) &&
                MergeManager.Instance.IsActiveItem(selecGridIndex,BoardId))
            {
                msg = "item_note_text32";
                InvokeRepeating("Invoke_OtherItemActiveStatus", 0f, 1f);
            }

            SetTipContentText(false, msg);
            return;
        }
        else if (itemConfig.type ==  (int) MergeItemType.diamonds || itemConfig.type ==  (int) MergeItemType.decoCoin || 
                 itemConfig.type ==  (int) MergeItemType.energy || itemConfig.type ==  (int) MergeItemType.exp || 
                 itemConfig.type == (int)MergeItemType.activityKey|| itemConfig.type == (int)MergeItemType.dogCookies ||
                 itemConfig.type == (int)MergeItemType.Parrot || itemConfig.type == (int)MergeItemType.FlowerField ||
                 itemConfig.type == (int)MergeItemType.climbTreeBanana || itemConfig.type == (int)MergeItemType.easter)
        {
            msg = string.Format(txt, itemConfig.value);
            SetTipContentText(false, msg);
            return;
        }
        else if (itemConfig.type ==  (int) MergeItemType.timeBooster)
        {
            int t = itemConfig.booster_factor;
            // if( itemConfig.booster_factor>=60)
            //     t=t/60;
            msg = string.Format(txt, t);
            SetTipContentText(false, msg);
            return;
        }

        var nextItem = GameConfigManager.Instance.GetItemConfig(itemConfig.next_level);
        if (nextItem != null)
            txt = string.Format(txt, LocalizationManager.Instance.GetLocalizedString(nextItem.name_key));
        SetTipContentText(false, txt);
    }

    private void SetSellButtonActive(bool active)
    {
        if (!active || !UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ItemSell))
        {
            sellBtn.gameObject.SetActive(false);
            destoryBtn.gameObject.SetActive(false);
            return;
        }
        
        sellBtn.gameObject.SetActive(MergeManager.Instance.IsCanSold(selecGridId) && curMergeItem.sold_gold>0);
        destoryBtn.gameObject.SetActive(MergeManager.Instance.IsCanSold(selecGridId) && curMergeItem.sold_gold<=0);
    }

    private ActiveCostType SetTimeStatus(TableMergeItem itemConfig)
    {         
        tipsBtn.gameObject.SetActive(true);
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
                timeBtn.gameObject.SetActive(true && !MergeManager.Instance.IsHaveOpenBox(BoardId));
                timeText.SetText(string.Format(
                    "{0} " + LocalizationManager.Instance.GetLocalizedString("UI_common_time_min"),
                    itemConfig.active_cost[1]));

                break;
            }
        }

        speedBtnText.SetTerm("button_open");

        return type;
    }

    public void SetNoFocusStatus()
    {
        InitView();

        CancelInvoke("Invoke_SetBubbleCdTime");
        CancelInvoke("Invoke_SetActiveCdTime");
        CancelInvoke("Invoke_SetProductCdTime");
        CancelInvoke("Invoke_SetTimeProductCdTime");
        CancelInvoke("Invoke_OtherItemActiveStatus");

        oldSelectIndex = -1;
        selecGridIndex = -1;
        selecGridId = -1;
        curMergeItem = null;
        boardItem = null;
        explainGroup.SetActive(false);
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
        if ( curMergeItem != null)
        {
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.BoxOpen))
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BoxOpen);
                if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.BoxSpeedUp))
                {
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BoxSpeedUp, null);
                    GuideSubSystem.Instance.SaveStorage(GuideTriggerPosition.BoxSpeedUp);
                }
            }
        }
        
        ClickTime(0);
    }

    private void ClickTime(int subTime = 0)
    {
        var boardItem = MergeManager.Instance.GetBoardItem(selecGridIndex,BoardId);
        ActiveCostType type = MergeConfigManager.Instance.GetActiveCostType(boardItem.Id);
        if (type == ActiveCostType.time_inactive && MergeManager.Instance.IsHaveOpenBox(BoardId))
        {
            //TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text32"), timeBtn.transform);
            return;
        }

        boardItem.ActiveTime = APIManager.Instance.GetServerTime() / 1000 - (ulong) subTime;
        MergeManager.Instance.SetBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.notDeal,BoardId);
        OnSelectGrid();
    }

    public void OnSpeedUp(bool useRv = false)
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BoxSpeedUp);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CdProductSpeedUp);
        MergeGuideLogic.Instance.CheckChoseItemGuide();
        MergeGuideLogic.Instance.CheckProductFinish();

        int cost = 0;
        string openSr = "diamond_lack_building_cd";
        var boardItem = MergeManager.Instance.GetBoardItem(selecGridIndex,BoardId);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(boardItem.Id);
        if (itemConfig == null)
            throw new Exception("加速或者解锁物品id出错：" + boardItem.Id);
        var reason = new GameBIManager.ItemChangeReasonArgs();
        if (useRv)
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
                    MergeManager.Instance.SetNewBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.webUnlock,BoardId);
                }
                else
                {
                    MergeManager.Instance.SetBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.webUnlock,BoardId);
                }

                MergeMainController.Instance.MergeBoard.TryOpenBoxes(selecGridIndex);
                MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlayAnimator("Lock_Open", true);
            }

            // reason.reason = BiEventCooking.Types.ItemChangeReason.UnlockMergeItem;
        }
        else if (boardItem.State == (int) MergeItemStatus.open)
        {
            if (useRv)
            {
                if (itemConfig.type ==  (int) MergeItemType.box)
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChestClickRv);
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBuildingsCdClickRv);

                var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup,
                    ADConstDefine.RV_BULIDING_CD);
                if (rv == null)
                    return;
                float percent = 0;
                if (_speedUpType == SpeedUpType.timeProduct)
                {
                    int cd = MergeManager.Instance.GetTimeProductCd(selecGridIndex, ref percent,BoardId);

                    int skipCd = Mathf.CeilToInt(1.0f * AdConfigHandle.Instance.GetRvSpeedUpFactor() / curMergeItem.time_cdspeed_cost[1] * MergeManager.Instance.GetTimeProductCdTime(curMergeItem));
                    skipCd = skipCd >= cd ? cd : skipCd;
                    
                    if (boardItem.TimProductTime > (ulong) skipCd)
                        boardItem.TimProductTime -= (ulong) skipCd;
                    else
                        boardItem.TimProductTime = 0;
                    if (skipCd >= cd)
                    {
                        MergeManager.Instance.SetTimeProductStoreCount(boardItem, true);
                        MergeManager.Instance.ResetTimeProductCD(SelectIndex,BoardId);
                        boardItem.PlayRvNum = 0;
                    }
                    MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () => { });
                }
                else
                {
                    int cd = MergeManager.Instance.GetLeftProductTime(selecGridIndex, ref percent,BoardId);

                    int skipCd = Mathf.CeilToInt(1.0f * AdConfigHandle.Instance.GetRvSpeedUpFactor() /
                        MergeManager.Instance.GetCdSpeedCost(curMergeItem,boardItem)[1] * MergeManager.Instance.GetProductCdTime(curMergeItem,boardItem,_boardId));
                    skipCd = skipCd >= cd ? cd : skipCd;
                    if (boardItem.InCdTime > (ulong) skipCd)
                        boardItem.InCdTime -= (ulong) skipCd;
                    else
                        boardItem.InCdTime = 0;
                    
                    if (boardItem.ProductTime > (ulong) skipCd)
                        boardItem.ProductTime -= (ulong) skipCd;
                    else
                        boardItem.ProductTime = 0;
                    if (skipCd >= cd)
                    {
                        MergeManager.Instance.SetOrginalStoreCount(boardItem, false);
                        MergeManager.Instance.ResumeBuildCD(boardItem);
                 
                    }

                    MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv);

                    GameBIManager.Instance.SendGameEvent(
                        BiEventCooking.Types.GameEventType.GameEventBuildingsCdSuccessRv,
                        itemConfig.id.ToString());
                }
            }
            else
            {
                cost = _cdCost;
                if (itemConfig.type ==  (int) MergeItemType.box)
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
                        MergeManager.Instance.ResetActiveCD(SelectIndex,BoardId);
                        MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () => { });
                    }
                    else if (_speedUpType == SpeedUpType.timeProduct)
                    {
                        MergeManager.Instance.SetTimeProductStoreCount(boardItem);
                        MergeManager.Instance.ResetTimeProductCD(SelectIndex,BoardId);
                        MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () => { });
                    }
                    else
                    {
                        MergeManager.Instance.SetOrginalStoreCount(boardItem, false);
                        MergeManager.Instance.ResetProductCD(SelectIndex,BoardId);
                        MergeManager.Instance.ResumeBuildCD(boardItem);
                        
                        MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv, () => { });
                    }


                    SetSpeedStatus();
                    GameBIManager.Instance.SendGameEvent(
                        BiEventCooking.Types.GameEventType.GameEventBuildingsCdSuccessGem, itemConfig.id.ToString());
                }
            }

            reason.reason = BiEventCooking.Types.ItemChangeReason.SpeedUp;
            reason.data1 = itemConfig.id.ToString();
        }
        else if (boardItem.State == (int) MergeItemStatus.bubble)
        {
            cost = itemConfig.unlcok_cost_bubble;
            string biType = "";
            if (boardItem.BubbleType == 1 && useRv)
            {
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBubbleClickRv);
                biType = "rv";
            }
            else
            {
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBubbleClickGem);
                biType = "gem";
            }

            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, cost)||useRv)
            {
                MergeManager.Instance.ResumAllCdTime(BoardId);
                MergeManager.Instance.SetBoardItem(selecGridIndex, boardItem.Id, 1, RefreshItemSource.webUnlock,BoardId);
                if (itemConfig.sold_gold > 0)
                    coinText.transform.parent.gameObject.SetActive(true);
                CancelInvoke("Invoke_SetBubbleCdTime");
                SetSpeedStatus();
                //AudioManager.Instance.PlaySound(HotelSound.mhm_bubble_open);
                MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlayBubbleAnimation("disappear");
                SendBubbleOpen(selecGridId, biType);
                EventDispatcher.Instance.SendEventImmediately(new EventBreakBubble());

                BalloonRacingModel.Instance.AddBubbleScore(cost, MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.transform.position);
                RabbitRacingModel.Instance.AddBubbleScore(cost, MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.transform.position);
            }

            reason.reason = BiEventCooking.Types.ItemChangeReason.BreakBubble;
            reason.data1 = itemConfig.id.ToString();
        }
        else if (boardItem.State == (int) MergeItemStatus.time)
        {
            ActiveCostType type = MergeConfigManager.Instance.GetActiveCostType(boardItem.Id);
            bool isCanSpeed = false;
            if (type == ActiveCostType.diamonds)
            {
                cost = _cdCost;
                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.BoxSpeedUp))
                {
                    cost = 0;
                }

                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, cost))
                {
                    MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex).board.PlaySpeedUpAnimator(useRv);

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
            reason.data1 = itemConfig.id.ToString();
        }

        OnSelectGrid();
        if (useRv)
            cost = 0;
        if (cost <= 0)
            return;
        int needCount = cost;
        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, cost))
        {
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                curMergeItem.id.ToString(), openSr,true,needCount);
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
    }

    private void OnClickTipsBtn()
    {
        bool isBox = MergeManager.Instance.IsBox(selecGridIndex,BoardId);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
        if (selecGridId <= 0 || isBox || itemConfig == null)
            return;

        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClickInfoButton, null);

        bool isGetResources = true;
        var produceCost = MergeManager.Instance.GetProduceCost(itemConfig);
        if (produceCost != null && produceCost.Length >= 2)
            isGetResources = false;
        else if (itemConfig.subType == (int)SubType.Matreshkas)
            isGetResources = false;
        
        MergeInfoView.Instance.OpenMergeInfo(itemConfig, isShowGetResource:isGetResources);
    }

    public void OnClickSell()
    {
        if (Time.time - sellColdTime < 0.8f)
            return;
        if (!MergeManager.Instance.IsBoardItemExist(selecGridIndex,BoardId))
            return;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
        bool isOpen = MergeManager.Instance.IsOpen(selecGridIndex,BoardId);
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
        OnSellItem(sellIndex);
        SetUndoStatus();
        sellColdTime = Time.time;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
        if (itemConfig.sold_gold > 0)
        {
            AudioManager.Instance.PlaySound(19);
        }
        else
        {
            AudioManager.Instance.PlaySound(170);
        }
        SendSellBI(selecGridId);
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELL_ITEM,BoardId, selecGridIndex);
        MergeManager.Instance.RemoveBoardItem(selecGridIndex,BoardId,"Sell");
    }

    void OnSellItem(int index)
    {
        var grid =MergeMainController.Instance.MergeBoard.GetGridByIndex(index);
        GameObject clone = grid.board.CloneIconGameObject();
        clone.transform.SetParent(grid.board.transform.parent, false);
        clone.transform.transform.position = grid.board.GetIconPosition();
        Destroy(clone, 0.2f);
    }

    private void SetUndoStatus()
    {
        var sell = MergeManager.Instance.GetBoardItem(selecGridIndex,BoardId);
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
        SetSellButtonActive(false);
        undoBtn.gameObject.SetActive(true);
        SetTipContentText(false, "UI_info_text2");
    }

    private void UndoOperate()
    {
        if (Time.time - sellColdTime < 0.8f)
            return;
        if (curMergeItem == null)
            return;
        if (curMergeItem.sold_gold>0 && !UserData.Instance.CanAford(UserData.ResourceId.Coin, curMergeItem.sold_gold))
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString =
                    LocalizationManager.Instance.GetLocalizedString("&key.ui_sell_withdraw_desc"),
                OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok")
            });
            return;
           
        }
        if (curMergeItem.sold_gold > 0)
        {
            CurrencyGroupManager.Instance.CostCurrency(UserData.ResourceId.Coin, curMergeItem.sold_gold,
                new GameBIManager.ItemChangeReasonArgs
                    {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.UdosaleMergeItem});
        }

        bool isExist = MergeManager.Instance.IsBoardItemExist(selecGridIndex,BoardId);
        if (isExist)
        {
            SetNoFocusStatus();
            MergeManager.Instance.AddRewardItem(sellItem, BoardId,1);
            var endTrans = MergeMainController.Instance.rewardBtnTrans;
            FlyGameObjectManager.Instance.FlyObject(sellItem.Id, Vector3.zero, endTrans, 1.1f, 0.8f, () =>
            {
                FlyGameObjectManager.Instance.PlayHintStarsEffect(endTrans.position);
                Animator shake = endTrans.transform.GetComponent<Animator>();
                if (shake != null)
                    shake.Play("shake", 0, 0);

                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            }, true, 1.0f, 0.7f);
        }
        else
        {
            MergeManager.Instance.SetBoardItem(selecGridIndex, sellItem, RefreshItemSource.notDeal,BoardId);

            OnSelectGrid(new Vector2Int(selecGridIndex, selecGridId));
            MergeMainController.Instance.MergeBoard.SelectFocus(selecGridIndex, false, true);
            SendUndoBI(selecGridId);
            RefreshAllStatus();
        }
    }

    private void SetTipContentText(bool isInCd, string tex = "",bool isBubble=false)
    {
        contentText.transform.parent.gameObject.SetActive(!isInCd);
        cdGroup.gameObject.SetActive(isInCd);
        if (isInCd)
        {
            cdText.SetText(tex);
        }
        else
        {
            if (!string.IsNullOrEmpty(tex))
                contentText.SetTerm(tex);
            int leftTime = MergeManager.Instance.GetLeftActiveTime(selecGridIndex,BoardId);
            if (curMergeItem!=null && curMergeItem.active_cost != null && curMergeItem.active_cost.Length > 1 && 
                !MergeManager.Instance.IsActiveItem(selecGridIndex,BoardId) && leftTime<=0)
            {
                if (curMergeItem.next_level<=0)
                {
                    contentText.SetTerm("item_note_text12");
                }
                else
                {
                    if (curMergeItem.type == (int) MergeItemType.box && boardItem!=null && boardItem.ProductCount>0 )
                    {
                        contentText.SetTerm("item_note_text49");
                    }
                    else
                    {
                        contentText.SetTerm("item_note_text11");
                    }

                }
            }
        }

        if (isBubble)
        {
            cdContentText.SetTerm("item_note_text45");
        }
        else
        {
            if (curMergeItem!=null &&curMergeItem.type == (int) MergeItemType.box && boardItem!=null && boardItem.ProductCount>0 )
            {
                contentText.SetTerm("item_note_text49");
            }
            else
            {
                cdContentText.SetTerm("item_note_text31");
            }

    

        }
    }

    private void Invoke_SetBubbleCdTime()
    {
        // if (!timeInvokeEnable)
        //     return;
        if (selecGridIndex < 0)
        {
            CancelInvoke("Invoke_SetBubbleCdTime");
            return;
        }

        int cd = MergeManager.Instance.GetBubbleLeftCdTime(selecGridIndex,BoardId);
        if (cd == 0)
        {
            SetTipContentText(false, "");
            speedBtn.gameObject.SetActive(false);
            thumbtackBtn.gameObject.SetActive(false);
            SetRvStatus(false);
            bubbleDissmiss =CoroutineManager.Instance.StartCoroutine(SetBubbleDismiss());
            CancelInvoke("Invoke_SetBubbleCdTime");
     
        }
        else
        {
            string timeString = TimeUtils.GetTimeString(cd);
            SetTipContentText(true, timeString,true);
  
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
        int cd = MergeManager.Instance.GetLeftProductTime(selecGridIndex, ref percent,BoardId);
        if (cd == 0)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
            if (itemConfig != null)
            {
                string txt = LocalizationManager.Instance.GetLocalizedString(itemConfig.item_des);
                var nextItem = GameConfigManager.Instance.GetItemConfig(itemConfig.next_level);
                if (nextItem != null)
                    txt = string.Format(txt, LocalizationManager.Instance.GetLocalizedString(nextItem.name_key));

                SetTipContentText(false, txt);
                RefreshAllStatus();
            }

            CancelInvoke("Invoke_SetProductCdTime");
        }
        else
        {
            string timeString = TimeUtils.GetTimeString(cd);
            SetSpeedUpPrice(SpeedUpType.product, cd);
            SetAdSpeedUpStatus();
            SetTipContentText(true, timeString);
        }
    }

    private void Invoke_OtherItemActiveStatus()
    {
        if (!MergeManager.Instance.IsHaveOpenBox(BoardId))
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
        int cd = MergeManager.Instance.GetTimeProductCd(selecGridIndex, ref percent,BoardId);

        if (cd == 0)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(selecGridId);
            if (itemConfig != null)
            {
                string txt = LocalizationManager.Instance.GetLocalizedString(itemConfig.item_des);
                var nextItem = GameConfigManager.Instance.GetItemConfig(itemConfig.next_level);
                if (nextItem != null)
                    txt = string.Format(txt, LocalizationManager.Instance.GetLocalizedString(nextItem.name_key));

                SetTipContentText(false, string.Format(txt, txt));
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
        MergeMainController.Instance.MergeBoard.SelectFocus(selecGridIndex, false, false);
        bubbleDissmiss = null;
    }

    private void Invoke_SetActiveCdTime()
    {
        if (selecGridIndex < 0)
        {
            CancelInvoke("Invoke_SetActiveCdTime");
            return;
        }

        int activeCd = MergeManager.Instance.GetLeftActiveTime(selecGridIndex,BoardId);
        if (activeCd == 0)
        {
            cdGroup.gameObject.SetActive(false);
            speedBtn.gameObject.SetActive(false);
            SetRvStatus(false);
            OnSelectGrid();
            CancelInvoke("Invoke_SetActiveCdTime");

            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.BoxSpeedUp))
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BoxSpeedUp);
                MergeGuideLogic.Instance.CheckChoseItemGuide();
                MergeGuideLogic.Instance.CheckProductFinish();
            }
        }
        else
        {
            string timeString = TimeUtils.GetTimeString(activeCd);
            SetSpeedUpPrice(SpeedUpType.active, activeCd);
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
        int gridIndex = MergeManager.Instance.FindEmptyGrid(1,BoardId);
        if (gridIndex == -1)
        {
            //TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text22"), discomposeBtn.transform);
            return;
        }

        var window = UIManager.Instance.OpenUI(UINameConst.MergeSplite) as MergeSpliteController;
        window.SetItemId(selecGridId);
        window.onSpliteCallBack = (rv) =>
        {
            var grid =MergeMainController.Instance.MergeBoard.GetGridByIndex(selecGridIndex);
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
        bool isOpen = MergeManager.Instance.IsOpen(selecGridIndex,BoardId);
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
            extras = new Dictionary<string, string>
            {
                {"coin", config.sold_gold.ToString()},
            },
            data1 = "0",
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

        MergeManager.Instance.ResumAllCdTime(BoardId);
        MergeMainController.Instance.MergeBoard?.RefreshGridsStatus();
        DebugUtil.LogError("恢复所有气泡的时间");
    }
}