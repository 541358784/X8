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
public enum BagType
{
    Normal = 1,
    Building =2,
}
public partial class UIPopupMergePackageController : UIWindow
{
    public static UIPopupMergePackageController Instance;
    int _maxCount = 20;
    int _maxBuildCount = 20;
    List<MergePackageUnit> Bagunits = new List<MergePackageUnit>();
    private Animator _animator;
    private Dictionary<BagType, BagAreaChoice> _dicBagChoice = new Dictionary<BagType, BagAreaChoice>();
    private Dictionary<BagType, BagArea> _dicBagAreas = new Dictionary<BagType, BagArea>();
    private LocalizeTextMeshProUGUI _buildTokenText;
    private Button _currencyBtn;
    private Transform _currencyTip;
    public override void PrivateAwake()
    {
        Instance = this;
        _maxCount = GameConfigManager.Instance.BagList.Count;
        _maxBuildCount = GameConfigManager.Instance.BagBuildingList.Count;
        BindClick("Root/ContentGroup/CloseButton", OnCloseClick);
        _animator = gameObject.GetComponent<Animator>();
        
        BagAreaChoice bagAreaChoice = new BagAreaChoice();
        bagAreaChoice.Button = transform.Find("Root/LabelGroup/UnitList").GetComponent<Button>();
        bagAreaChoice.Normal = transform.Find("Root/LabelGroup/UnitList/Normal");
        bagAreaChoice.Selected = transform.Find("Root/LabelGroup/UnitList/Selected");
        _dicBagChoice.Add(BagType.Normal,bagAreaChoice);

        BagAreaChoice bagAreaChoice2 = new BagAreaChoice();
        bagAreaChoice2.Button = transform.Find("Root/LabelGroup/UnitListBuilding").GetComponent<Button>();
        bagAreaChoice2.Normal = transform.Find("Root/LabelGroup/UnitListBuilding/Normal");
        bagAreaChoice2.Selected = transform.Find("Root/LabelGroup/UnitListBuilding/Selected");
        _dicBagChoice.Add(BagType.Building,bagAreaChoice2);
        
        
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
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MergeBoardGuideBagTag,bagAreaChoice2.Button.transform as RectTransform);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MergeBoardGuideBagTag,null);
            }   
        }
        
        BagArea bagArea = new BagArea();
        bagArea.AreaTrans = transform.Find("Root/ContentGroup/UnitList/Viewport/Content/Normal/GameObject");
        bagArea.AreaContent = transform.Find("Root/ContentGroup/UnitList/Viewport/Content/Normal");
        bagArea.AreaInitPosition = transform.Find("Root/ContentGroup/UnitList/Viewport/Content/Normal/GameObject") as RectTransform;
        bagArea._scrollRect = transform.Find("Root/ContentGroup/UnitList").GetComponent<ScrollRect>();
        _dicBagAreas.Add(BagType.Normal,bagArea);    
        
        BagArea bagArea2 = new BagArea();
        bagArea2.AreaTrans = transform.Find("Root/ContentGroup/UnitList/Viewport/Content/Building/GameObject");
        bagArea2.AreaContent = transform.Find("Root/ContentGroup/UnitList/Viewport/Content/Building");
        bagArea2.AreaInitPosition = transform.Find("Root/ContentGroup/UnitList/Viewport/Content/Building/GameObject") as RectTransform;
        bagArea2._scrollRect = transform.Find("Root/ContentGroup/UnitList").GetComponent<ScrollRect>();
        _dicBagAreas.Add(BagType.Building,bagArea2);
        _buildTokenText= transform.Find("Root/ContentGroup/Currency/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _currencyBtn = transform.Find("Root/ContentGroup/Currency").GetComponent<Button>();
        _currencyTip = transform.Find("Root/ContentGroup/Currency/0");
        _currencyBtn.onClick.AddListener(() =>
        {
            _currencyTip.gameObject.SetActive(false);
            _currencyTip.gameObject.SetActive(true);
        });
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        BagType openType = BagType.Normal;
        if (objs != null && objs.Length>0)
        {
            openType = (BagType) objs[0];
        }
        InitArea(openType);
        GetOutItemPositionList.Clear();
        RefreshBagToken();

    }

    public void RefreshBagToken()
    {
        _buildTokenText.SetText(UserData.Instance.GetRes(UserData.ResourceId.BagToken).ToString());
    }
    private void InitArea(BagType type)
    {
        for (BagType i = BagType.Normal; i <=BagType.Building; i++)
        {
            BagType index = i;
            _dicBagChoice[i].Button.onClick.AddListener(() =>
            {
                ChoiceArea(index);
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MergeBoardGuideBagTag);
            });
            if (i == BagType.Normal )
            {
                InitPackageCell();
                RefreshBag(0);
            }     
            else if (i == BagType.Building)
            {
                InitBuildingPackageCell();
                RefreshBuildingBag();
            }      
            
            //_dicBagAreas[i].AreaTrans.gameObject.SetActive(type==i);
            _dicBagChoice[i].Selected.gameObject.SetActive(type==i);
            _dicBagChoice[i].Normal.gameObject.SetActive(type!=i);
        }

        StartCoroutine(CommonUtils.DelayWorkFrame(1, () =>
        {
            ScrollToTarget(_dicBagAreas[type]._scrollRect, _dicBagAreas[type].AreaInitPosition);
        }));
    }
    public void ChoiceArea(BagType type)
    {
        AudioManager.Instance.PlaySound("button_tab_click");
        
        for (BagType i = BagType.Normal; i <=BagType.Building; i++)
        {
            //_dicBagAreas[i].AreaTrans.gameObject.SetActive(type==i);
            _dicBagChoice[i].Selected.gameObject.SetActive(type==i);
            _dicBagChoice[i].Normal.gameObject.SetActive(type!=i);
        }

        ScrollToTarget(_dicBagAreas[type]._scrollRect, _dicBagAreas[type].AreaInitPosition);
    }

    void ScrollToTarget(ScrollRect scrollRect, RectTransform target)
    {
        if (scrollRect == null || target == null)
        {
            Debug.LogError("ScrollRect 或目标元素未设置！");
            return;
        }

        RectTransform content = scrollRect.content;

        Vector2 targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(content.position)
                                 - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        Vector2 normalizedPosition = new Vector2(0,
            Mathf.Clamp01(1-Mathf.Abs(targetPosition.y / (content.rect.height - scrollRect.viewport.rect.height))));

        scrollRect.normalizedPosition = normalizedPosition;
    }
    
    private void InitPackageCell()
    {
        for (int i = 0; i < MergeManager.Instance.GetBagCount(MergeBoardEnum.Main); i++)
        {
            Transform item = AddPackageItem();
            var script = item.gameObject.GetComponentDefault<MergePackageUnit>();
            script.SetBoardId(MergeBoardEnum.Main);
            Bagunits.Add(script);
        }

        int bagCapacity = MergeManager.Instance.GetBagCapacity(MergeBoardEnum.Main);

        int leftCount = bagCapacity - MergeManager.Instance.GetBagCount(MergeBoardEnum.Main);
        leftCount = Mathf.Max(0, leftCount);
        for (int i = 0; i <= leftCount; i++)
        {
            Transform item = AddPackageItem();
            var script = item.gameObject.GetComponentDefault<MergePackageUnit>();
            script.SetBoardId(MergeBoardEnum.Main);
            Bagunits.Add(script);
        }
    }

    public List<int> GetOutItemPositionList = new List<int>();
    private void OnDestroy()
    {
        foreach (var item in Bagunits)
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.MergePackageUnit, item.gameObject);
        }

        MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Pack,MergeBoardEnum.Main);
        foreach (var index in GetOutItemPositionList)
        {
            MergeMainController.Instance.MergeBoard.GetGridByIndex(index)?.board.PlayHintEffect();
        }
        GetOutItemPositionList.Clear();
    }

    public void AddBagUnit() // 一次只能解锁一个
    {
        if (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BagCapacity >= _maxCount)
        {
            //TipBoxController.ShowTip(LocalizationManager.Instance.GetLocalizedString("UI_info_text16"), Bagunits[Bagunits.Count - 1].transform);
            return;
        }

        int cost = 0;
        cost = GameConfigManager.Instance.BagList[MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BagCapacity].CointCost;
        var type = GameConfigManager.Instance
            .BagList[MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BagCapacity].CointType;
        int needCount;
        if (!UserData.Instance.CanAford((UserData.ResourceId)type, cost,out needCount))
        {
            BuyResourceManager.Instance.TryShowBuyResource((UserData.ResourceId)type, "",
                MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BagCapacity.ToString(), "diamond_lack_bag",true,needCount);
            return;
        }

        var reason = new GameBIManager.ItemChangeReasonArgs
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BuyBag,
            data1 = (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BagCapacity+1).ToString()
        };
        UserData.Instance.ConsumeRes((UserData.ResourceId)type, cost, reason);
        RefreshBagToken();
        MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).BagCapacity += 1;
        Transform item = AddPackageItem();
        var script = item.gameObject.GetComponentDefault<MergePackageUnit>();
        script.SetBoardId(MergeBoardEnum.Main);
        script.SetItemInfomation(null, -1, MergePackageUnitType.bagUnlock);
        Bagunits.Add(script);
        RefreshBag(0);
    }

    public Transform AddPackageItem()
    {
        Transform item = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.MergePackageUnit).transform;
        item.parent = _dicBagAreas[BagType.Normal].AreaContent.transform;
        item.localScale = Vector3.one;
        item.localPosition = Vector3.zero;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_dicBagAreas[BagType.Normal].AreaContent);
        return item;
    }

    private void OnCloseClick(GameObject obj)
    {
        ClickUIMask();
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MergeBoardGuideBagTag);
        canClickMask = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }
    
    public void RefreshBag(int index)
    {
        int m_index = 0;
        var board = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main);
        int bagCapacity = MergeManager.Instance.GetBagCapacity(MergeBoardEnum.Main);
        int showIndex = -1;

        for (int i = 0; i < MergeManager.Instance.GetBagCount(MergeBoardEnum.Main); i++)
        {
            m_index = i ;
            int id = board.Bags[i].Id;
            var config = GameConfigManager.Instance.GetItemConfig(id);
            if (m_index >= Bagunits.Count)
                continue;

            Bagunits[m_index].SetItemInfomation(config, m_index, MergePackageUnitType.bag);

            if (showIndex < 0 &&
                MergeResourceManager.Instance.MergeResourceHandle(MergeResourceManager.MergeSourcesType.Pack, m_index))
            {
                showIndex = m_index;
                MergeResourceController controller =
                    MergePromptManager.Instance.ShowPrompt(Bagunits[m_index].transform.position);
                if (controller != null)
                {
                    controller.transform.SetParent(Bagunits[m_index].transform);
                    controller.transform.localPosition = Vector3.zero;
                    controller.transform.localScale = Vector3.one;
                    controller.canvas.sortingOrder = canvas.sortingOrder + 1;
                }
            }
        }

        if (showIndex > 0 && showIndex < Bagunits.Count)
        {
            RectTransform currentItem = Bagunits[showIndex].transform as RectTransform;
            
            ScrollToTarget(_dicBagAreas[BagType.Normal]._scrollRect, currentItem);
        }

        int leftCount = bagCapacity - MergeManager.Instance.GetBagCount(MergeBoardEnum.Main);
        m_index = Bagunits.Count - leftCount - 1;
        m_index = Math.Max(m_index, 0);

        for (int i = 0; i < leftCount; i++)
        {
            if (m_index + i < Bagunits.Count)
                Bagunits[m_index + i].SetItemInfomation(null, -1, MergePackageUnitType.bagUnlock);
        }

        if (MergeManager.Instance.GetBagCapacity(MergeBoardEnum.Main) >= _maxCount)
        {
            Bagunits[Bagunits.Count - 1].SetItemInfomation(null, -1, MergePackageUnitType.bagMax);
            Bagunits[Bagunits.Count - 1].SetBagCost(0,0);
        }
        else
        {
            Bagunits[Bagunits.Count - 1].SetItemInfomation(null, -1, MergePackageUnitType.baglock);
            Bagunits[Bagunits.Count - 1]
                .SetBagCost(GameConfigManager.Instance.BagList[MergeManager.Instance.GetBagCapacity(MergeBoardEnum.Main)].CointType,GameConfigManager.Instance.BagList[MergeManager.Instance.GetBagCapacity(MergeBoardEnum.Main)].CointCost);
        }
    }
    private class BagAreaChoice
    {
        public  Transform Normal;
        public Transform Selected;
        public Button Button;
    }
    
    private class BagArea
    {
        public Transform AreaTrans;
        public RectTransform AreaInitPosition;
        public Transform AreaContent;
        public ScrollRect _scrollRect;
    }
}
