using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using System;
using System.Linq;
using System.Threading.Tasks;
using Activity.JungleAdventure.Controller;
using Activity.LimitTimeOrder;
using Activity.SaveTheWhales;
using Activity.TimeOrder;
using Activity.Turntable.Model;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using GamePool;
using Merge.Order;
using Spine;
using Spine.Unity;
using Animation = UnityEngine.Animation;
using Random = System.Random;

public class TaskCompleteObj
{
    public GameObject _completeObj;
    public GameObject _noCompleteObj;

    public void SetComplete(bool complete)
    {
        _completeObj?.gameObject.SetActive(complete);
        _noCompleteObj?.gameObject.SetActive(!complete);
    }
}

public class TaskRewardInfo
{
    public GameObject _rewardObj;
    public Image _rewardImage;
    public LocalizeTextMeshProUGUI _rewardText;
    public LocalizeTextMeshProUGUI _multipleText;
}


public partial class MergeTaskTipsItem : MonoBehaviour
{
    private string[][] _completePaths = new string[][]
    {
        new []{"vfx_BG_soft_clip", ""},
    };
    
    private Transform content;
    private List<TaskRewardInfo> _rewardInfos = new List<TaskRewardInfo>();
    private List<TaskCompleteObj> _completeObjs = new List<TaskCompleteObj>();
    private List<MergeTaskTipsGoods> m_itemList = new List<MergeTaskTipsGoods>();
    protected Transform personHead;
    private SkeletonGraphic _skeletonGraphic;

    public Button completButton;

    private bool isComplteTask = false;
    private bool isComplteSubTask = false;
    private int siblingIndex = 0;
    private Animator _animator;
    private GameObject portraitSpineObj = null;
    private int headIndex = -1;

    private GameObject completeEf_hint;

    private StorageTaskItem storageTaskItem = null;

    public GameObject completeObj;
    public GameObject guideObj;
    private Animator guideObjAnimator;

    private GameObject arrow = null;

    private bool canTouochComplete = false;

    private GameObject _completeObj;
    private GameObject _noCompleteObj;

    public GameObject _dogHopeObj;
    private LocalizeTextMeshProUGUI _dogCookiesText;
    
    public GameObject _turntableObj;
    public GameObject _turntableIcon;
    private LocalizeTextMeshProUGUI _turntableText;
    
    public LocalizeTextMeshProUGUI _mermaidText;
    public Image _mermaidIcon;
    public Transform _easter2024EggGroup;
    public LocalizeTextMeshProUGUI _easter2024EggText;
    public Image _easter2024EggIcon;
    public Transform _snakeLadderTurntableGroup;
    public LocalizeTextMeshProUGUI _snakeLadderTurntableText;
    public Image _snakeLadderTurntableIcon;
    public MergeTaskItemExtraOrderRewardCouponGroup ExtraOrderRewardCouponGroup;
    public MergeTaskItemThemeDecorationGroup ThemeDecorationGroup;
    public MergeTaskItemSlotMachineGroup SlotMachineGroup;
    public MergeTaskItemMonopolyGroup MonopolyGroup;
    public MergeTaskItemMixMasterGroup MixMasterGroup;
    public MergeTaskItemTurtlePangGroup TurtlePangGroup;
    public MergeTaskItemStarrySkyCompassGroup StarrySkyCompassGroup;
    public MergeTaskItemZumaGroup ZumaGroup;
    public MergeTaskItemDogPlayGroup DogPlayGroup;
    public GameObject _ZumaIcon;
    public MergeTaskItemFishCultureGroup FishCultureGroup;
    public MergeTaskItemPhotoAlbumGroup PhotoAlbumGroup;
    public MergeTaskItemParrotGroup ParrotGroup;
    public MergeTaskItemFlowerFieldGroup FlowerFieldGroup;
    public MergeTaskItemPillowWheelGroup PillowWheelGroup;
    public MergeTaskItemCatchFishGroup CatchFishGroup;
    
    public MergeTaskItemClimbTreeGroup ClimbTreeGroup;
    
    public MergeTaskItemJungleAdventure _jungleAdventure;
    
    private GameObject _normalBg;
    private GameObject _specialBg;
    
    public Transform _saveTheWhalesGroup;
    public LocalizeTextMeshProUGUI _saveTheWhalesText;
    public Image _saveTheWhalesIcon;
    
    private GameObject _timeCoolDown;
    private LocalizeTextMeshProUGUI _cooldownText;
    
    public bool CanTouchComplete
    {
        get { return canTouochComplete; }
        set { canTouochComplete = value; }
    }


    public int SiblingIndex
    {
        get { return siblingIndex; }
        set { siblingIndex = value; }
    }

    public int HeadIndex
    {
        get { return headIndex; }
    }

    public bool IsComplteTask
    {
        get { return isComplteTask; }
        private set { isComplteTask = value; }
    }

    public bool IsComplteSubTask
    {
        get { return isComplteSubTask; }
        private set { isComplteSubTask = value; }
    }

    public List<MergeTaskTipsGoods> ItemList
    {
        get { return m_itemList; }
    }


    public StorageTaskItem StorageTaskItem
    {
        get { return storageTaskItem; }
    }

    public int TableTaskId()
    {
        if (storageTaskItem == null)
            return -1;
        return storageTaskItem.Id;
    }

    public bool IsRareDecoCoin()
    {
        if (storageTaskItem == null)
            return false;

        return storageTaskItem.RewardTypes.Contains((int)UserData.ResourceId.RareDecoCoin);
    }
    public Transform StarTransform(int index)
    {
        if (index < 0 || index >= _rewardInfos.Count)
            return null;
        
        return _rewardInfos[index]._rewardObj.transform;
    }

    public Transform RewardTransform()
    {
        return _rewardGroup.transform;
    }
    
    public Animator animator
    {
        get { return _animator; }
    }

    private bool isInit = false;

    public GameObject _rewardGroup;
    private Button _rewardGroupButton;
    private GameObject _rewardGroupSeal;
    private GameObject _rewardGroupCapybara;
    private GameObject _rewardGroupDolphin;

    private LocalizeTextMeshProUGUI _multipleScoreMermaid;
    private LocalizeTextMeshProUGUI _multipleCouponDogHope;
    private LocalizeTextMeshProUGUI _multipleCouponSnakeLadder;
    protected virtual void Awake()
    {
        content = transform.Find("RewardList");

        for (int i = 0; i < 2; i++)
        {
            TaskRewardInfo info = new TaskRewardInfo();
            string infoKey = i == 0 ? "" : "1";

            string path = "StartGroup/Start" + infoKey;
            info._rewardObj = transform.Find(path).gameObject;
            info._rewardText = transform.GetComponentDefault<LocalizeTextMeshProUGUI>(path + "/Text");
            info._rewardImage = transform.GetComponent<Image>(path + "/Icon");
            info._multipleText = transform.GetComponentDefault<LocalizeTextMeshProUGUI>(path + "/Double");
            info._multipleText.gameObject.SetActive(false);
            info._rewardObj.SetActive(false);
            _rewardInfos.Add(info);
        }
        
        transform.Find("StartActivity/MermaidIcon").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MermaidModel.Instance.IsOpened() && MultipleScoreModel.Instance.IsOpenActivity() && MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.Mermaid) > 1)
                UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidDouble);
        });

        _multipleScoreMermaid = transform.Find("StartActivity/MermaidIcon/Double").GetComponent<LocalizeTextMeshProUGUI>();
        _multipleScoreMermaid.gameObject.SetActive(false);
        
        _multipleCouponDogHope = transform.Find("StartActivity/DogIcon/Double").GetComponent<LocalizeTextMeshProUGUI>();
        _multipleCouponDogHope.gameObject.SetActive(false);
        
        _multipleCouponSnakeLadder = transform.Find("StartActivity/SnakeLadderIcon/Double").GetComponent<LocalizeTextMeshProUGUI>();
        _multipleCouponSnakeLadder.gameObject.SetActive(false);

        for (int i = 0; i < _completePaths.Length; i++)
        {
            TaskCompleteObj completeObj = new TaskCompleteObj();
            if(!_completePaths[i][0].IsEmptyString())
                completeObj._completeObj = transform.Find(_completePaths[i][0]).gameObject;
            
            if(!_completePaths[i][1].IsEmptyString())
                completeObj._noCompleteObj = transform.Find(_completePaths[i][1]).gameObject;
            
            _completeObjs.Add(completeObj);
        }
        
        m_itemList.Clear();
        for (int i = 0; i < 3; i++)
        {
            GameObject childObj = content.Find("MergeTaskRewardItem"+i).gameObject;
            childObj.SetActive(true);
            m_itemList.Add(childObj.AddComponent<MergeTaskTipsGoods>());

            childObj.SetActive(false);
        }

        completeObj = transform.Find("Button").gameObject;
        guideObj = transform.Find("guide").gameObject;
        //guideObjAnimator = guideObj.GetComponent<Animator>();

        completButton = transform.Find("Button").GetComponent<Button>();
        completButton.onClick.AddListener(OnCompleteButton);
        completButton.gameObject.SetActive(false);

        personHead = transform.Find("Person/PortraitSpine");

        _animator = transform.GetComponent<Animator>();

        _normalBg = transform.Find("BG").gameObject;
        _specialBg = transform.Find("BG1").gameObject;
        
        completeEf_hint = transform.Find("VFX_Hint_01").gameObject;

        arrow = transform.Find("Arrow").gameObject;
        arrow.GetComponent<Canvas>().sortingOrder = MergeMainController.Instance.canvas.sortingOrder + 1;

        _rewardGroup = transform.Find("RewardButton").gameObject;
        _rewardGroupButton = _rewardGroup.GetComponent<Button>();
        _rewardGroupButton.onClick.AddListener(ShowRewardGroup);
        _rewardGroupButton.gameObject.SetActive(false);

        _rewardGroupSeal = transform.Find("RewardButton/seal").gameObject;
        _rewardGroupSeal.gameObject.SetActive(false);
        _rewardGroupDolphin = transform.Find("RewardButton/dolphin").gameObject;
        _rewardGroupDolphin.gameObject.SetActive(false);
        _rewardGroupCapybara= transform.Find("RewardButton/capybara").gameObject;
        _rewardGroupCapybara.gameObject.SetActive(false);
        
        isInit = true;

        _dogHopeObj = transform.Find("StartActivity/DogIcon").gameObject;
        _dogCookiesText = transform.Find("StartActivity/DogIcon/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _turntableObj = transform.Find("StartActivity/TurntableIcon").gameObject;
        _turntableIcon = transform.Find("StartActivity/TurntableIcon/Icon").gameObject;
        _turntableText = transform.Find("StartActivity/TurntableIcon/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _mermaidText = transform.Find("StartActivity/MermaidIcon/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _mermaidIcon = transform.Find("StartActivity/MermaidIcon/Icon").GetComponent<Image>();

        _easter2024EggGroup = transform.Find("StartActivity/Easter2024Icon");
        _easter2024EggText = transform.Find("StartActivity/Easter2024Icon/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _easter2024EggIcon = transform.Find("StartActivity/Easter2024Icon/Icon").GetComponent<Image>();
        
        _saveTheWhalesGroup = transform.Find("ActivityIcon/SaveTheWhales");
        _saveTheWhalesText = transform.Find("ActivityIcon/SaveTheWhales/Start/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _saveTheWhalesIcon = transform.Find("ActivityIcon/SaveTheWhales/Start/Icon").GetComponent<Image>();
        
        _snakeLadderTurntableGroup = transform.Find("StartActivity/SnakeLadderIcon");
        _snakeLadderTurntableText = transform.Find("StartActivity/SnakeLadderIcon/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _snakeLadderTurntableIcon = transform.Find("StartActivity/SnakeLadderIcon/Icon").GetComponent<Image>();

        ThemeDecorationGroup = transform.Find("StartActivity/ThemeDecorationIcon").gameObject.AddComponent<MergeTaskItemThemeDecorationGroup>();
        ThemeDecorationGroup.Init();
        
        SlotMachineGroup = transform.Find("StartActivity/SlotMachineIcon").gameObject.AddComponent<MergeTaskItemSlotMachineGroup>();
        SlotMachineGroup.Init();
        
        MonopolyGroup = transform.Find("StartActivity/MonopolyIcon").gameObject.AddComponent<MergeTaskItemMonopolyGroup>();
        MonopolyGroup.Init();

        MixMasterGroup = transform.Find("StartActivity/MixMasterIcon").gameObject.AddComponent<MergeTaskItemMixMasterGroup>();
        MixMasterGroup.Init();
        
        TurtlePangGroup = transform.Find("StartActivity/TurtlePangIcon").gameObject.AddComponent<MergeTaskItemTurtlePangGroup>();
        TurtlePangGroup.Init();
        
        StarrySkyCompassGroup = transform.Find("StartActivity/StarrySkyCompassIcon").gameObject.AddComponent<MergeTaskItemStarrySkyCompassGroup>();
        StarrySkyCompassGroup.Init();

        ZumaGroup = transform.Find("StartActivity/ZumaIcon").gameObject.AddComponent<MergeTaskItemZumaGroup>();
        ZumaGroup.Init();
        
        FishCultureGroup = transform.Find("StartActivity/FishCultureIcon").gameObject.AddComponent<MergeTaskItemFishCultureGroup>();
        FishCultureGroup.Init();
        
        PhotoAlbumGroup = transform.Find("StartActivity/PhotoAlbumIcon").gameObject.AddComponent<MergeTaskItemPhotoAlbumGroup>();
        PhotoAlbumGroup.Init();
        
        ParrotGroup = transform.Find("StartActivity/ParrotIcon").gameObject.AddComponent<MergeTaskItemParrotGroup>();
        ParrotGroup.Init();
        
        FlowerFieldGroup = transform.Find("StartActivity/FlowerFieldIcon").gameObject.AddComponent<MergeTaskItemFlowerFieldGroup>();
        FlowerFieldGroup.Init();
        
        PillowWheelGroup = transform.Find("StartActivity/PillowWheelIcon").gameObject.AddComponent<MergeTaskItemPillowWheelGroup>();
        PillowWheelGroup.Init();
        
        CatchFishGroup = transform.Find("StartActivity/CatchFishIcon").gameObject.AddComponent<MergeTaskItemCatchFishGroup>();
        CatchFishGroup.Init();
        
        _jungleAdventure = transform.Find("StartActivity/JungleAdventure").gameObject.AddComponent<MergeTaskItemJungleAdventure>();
        _jungleAdventure.Init();
            
        ClimbTreeGroup = transform.Find("StartActivity/ClimbTreeIcon").gameObject.AddComponent<MergeTaskItemClimbTreeGroup>();
        ClimbTreeGroup.Init();
        
        DogPlayGroup = transform.Find("ActivityIcon/DogPlay").gameObject.AddComponent<MergeTaskItemDogPlayGroup>();
        DogPlayGroup.Init();

        _ZumaIcon = transform.Find("StartActivity/ZumaIcon/Icon").gameObject;
        
        ExtraOrderRewardCouponGroup = transform.Find("ExtraOrderRewardCouponGroup").gameObject.AddComponent<MergeTaskItemExtraOrderRewardCouponGroup>();
        
        
        _timeCoolDown = transform.Find("TimeGroup").gameObject;
        _cooldownText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        
        AwakeTimeOrder();
        AwakeLimitOrder();
        AwakeCrazeOrder();
        AwakeDebug();
        AwakeBalloonRacing();
        AwakeRabbitRacing();
        AwakeKeepPetOrder();
        AwakeTeamOrder();
        InvokeRepeating("RefreshRepeating", 0, 1f);
        EventDispatcher.Instance.AddEvent<EventKeepPetChangeSkin>(OnChangeDogSkin);
    }
    public virtual void Init(StorageTaskItem storageItem)
    {
        if (!isInit)
        {
            Awake();
        }

        animator.Play("Normal", 0, 0);
        
        isComplteTask = false;
        CanTouchComplete = false;
        storageTaskItem = storageItem;
        if (storageTaskItem == null)
            return;
        
        PlayCompleteEf(false);

        _timeCoolDown.gameObject.SetActive(false);
        
        _dogCookiesText.SetText(storageItem.DogCookiesNum.ToString());
        _mermaidText.SetText(MermaidModel.Instance.GetTaskValue(storageItem, false).ToString());
        _turntableText.SetText(TurntableModel.Instance.GetTaskValue(storageItem, false).ToString());
        
        _easter2024EggText.SetText(Easter2024Model.Instance.GetTaskValue(storageItem, false).ToString());
        
        _saveTheWhalesText.SetText(SaveTheWhalesModel.Instance.GetTaskValue(storageItem, false).ToString());
        
        _snakeLadderTurntableText.SetText(SnakeLadderModel.Instance.GetTaskValue(storageItem, false).ToString());
        
        bool isSpecial = false;
        bool isSeal = false;
        bool isDolphin = false;
        bool isCapybara = false;
        
        for (int i = 0; i < storageItem.RewardTypes.Count; i++)
        {
            if(i >= 2)
                break;
            
            _rewardInfos[i]._rewardObj.gameObject.SetActive(true);
            _rewardInfos[i]._rewardText.SetText(storageTaskItem.RewardNums[i].ToString());
            var rewardType = storageItem.RewardTypes[i];
            rewardType = MainOrderManager.ChangeTaskRewardType(rewardType);
            _rewardInfos[i]._rewardImage.sprite = UserData.GetResourceIcon(rewardType);
            if (storageItem.RewardTypes[i] == (int)UserData.ResourceId.RareDecoCoin)
                isSpecial = true;
            
            if (storageItem.RewardTypes[i] == (int)UserData.ResourceId.Seal)
                isSeal = true;
            
            if (storageItem.RewardTypes[i] == (int)UserData.ResourceId.Dolphin)
                isDolphin = true;
            
            if (storageItem.RewardTypes[i] == (int)UserData.ResourceId.Capybara)
                isCapybara = true;
            
        }

        if (isSeal || isDolphin || isCapybara)
        {
            _rewardInfos.ForEach(a=>a._rewardObj.gameObject.SetActive(false));
        }
        
        _rewardGroup.gameObject.SetActive(isSeal || isDolphin || isCapybara);
        _normalBg.gameObject.SetActive(!isSpecial);
        _specialBg.gameObject.SetActive(isSpecial);
        _rewardGroupSeal.gameObject.SetActive(isSeal);
        _rewardGroupDolphin.gameObject.SetActive(isDolphin);
        _rewardGroupCapybara.gameObject.SetActive(isCapybara);
            
        for (int i = 0; i < m_itemList.Count; i++)
            m_itemList[i].gameObject.SetActive(false);

        for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
        {
            int id = storageTaskItem.ItemIds[i];

            var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
            if (itemConfig == null)
            {
                DebugUtil.LogError("物品id 配置错误  " + id);
                continue;
            }

            var item = m_itemList[i];
            item.gameObject.SetActive(true);
            item.SetImageIcon(itemConfig);
        
        }

        InitTeamOrder(storageItem);
        InitTimeOrder(storageItem);
        InitLimitOrder(storageItem);
        InitBalloonRacing(storageItem);
        InitRabbitRacing(storageItem);
        InitCrazeOrder(storageItem);
        InitKeepPetOrder(storageItem);
        InitHeadIcon();
        RefreshView(isInit:true);
        UpdateDebugInfo();
        InitExtraOrderRewardCouponGroup();
        InitThemeDecorationGroup();
        InitSlotMachineGroup();
        InitMonopolyGroup();
        InitMixMasterGroup();
        InitTurtlePangGroup();
        InitStarrySkyCompassGroup();
        InitZumaGroup();
        InitFishCultureGroup();
        InitPhotoAlbumGroup();
        InitParrotGroup();
        InitFlowerFieldGroup();
        InitPillowWheelGroup();
        InitCatchFishGroup();
        InitClimbTreeGroup();
        InitDogPlayGroup();
        InitJungleAdventureGroup();
        _completeObjs.ForEach(a=>a.SetComplete(false));
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    public void InitExtraOrderRewardCouponGroup()
    {
        ExtraOrderRewardCouponGroup.Init(storageTaskItem,this);
    }
    public void RefreshExtraOrderRewardCouponGroup()
    {
        ExtraOrderRewardCouponGroup.Refresh();
    }
    public void InitThemeDecorationGroup()
    {
        ThemeDecorationGroup.Init(storageTaskItem, this);
    }
    public void InitSlotMachineGroup()
    {
        SlotMachineGroup.Init(storageTaskItem, this);
    }
    public void InitMonopolyGroup()
    {
        MonopolyGroup.Init(storageTaskItem, this);
    }

    public void InitMixMasterGroup()
    {
        MixMasterGroup.Init(storageTaskItem, this);
    }
    
    public void InitTurtlePangGroup()
    {
        TurtlePangGroup.Init(storageTaskItem, this);
    }
    public void InitStarrySkyCompassGroup()
    {
        StarrySkyCompassGroup.Init(storageTaskItem, this);
    }

    public void InitZumaGroup()
    {
        ZumaGroup.Init(storageTaskItem,this);
    }
    public void InitFishCultureGroup()
    {
        FishCultureGroup.Init(storageTaskItem,this);
    }
    public void InitPhotoAlbumGroup()
    {
        PhotoAlbumGroup.Init(storageTaskItem,this);
    }
    public void InitParrotGroup()
    {
        ParrotGroup.Init(storageTaskItem,this);
    }
    public void InitFlowerFieldGroup()
    {
        FlowerFieldGroup.Init(storageTaskItem,this);
    }
    public void InitPillowWheelGroup()
    {
        PillowWheelGroup.Init(storageTaskItem,this);
    }
    public void InitCatchFishGroup()
    {
        CatchFishGroup.Init(storageTaskItem,this);
    }
    public void InitClimbTreeGroup()
    {
        ClimbTreeGroup.Init(storageTaskItem,this);
    }
    public void InitJungleAdventureGroup()
    {
        _jungleAdventure.Init(storageTaskItem,this);
    }
    public void InitDogPlayGroup()
    {
        DogPlayGroup.Init(storageTaskItem,this);
    }
    
    public void RefreshThemeDecorationGroup()
    {
        ThemeDecorationGroup.Refresh();
    }
    public void RefreshSlotMachineGroup()
    {
        SlotMachineGroup.Refresh();
    }
    public void RefreshMonopolyGroup()
    {
        MonopolyGroup.Refresh();
    }
    public void RefreshMixMasterGroup()
    {
        MixMasterGroup.Refresh();
    }
    
    public void RefreshTurtlePangGroup()
    {
        TurtlePangGroup.Refresh();
    }
    public void RefreshStarrySkyCompassGroup()
    {
        StarrySkyCompassGroup.Refresh();
    }
    public void RefreshZumaGroup()
    {
        ZumaGroup.Refresh();
    }
    public void RefreshFishCultureGroup()
    {
        FishCultureGroup.Refresh();
    }
    public void RefreshPhotoAlbumGroup()
    {
        PhotoAlbumGroup.Refresh();
    }
    public void RefreshParrotGroup()
    {
        ParrotGroup.Refresh();
    }
    public void RefreshFlowerFieldGroup()
    {
        FlowerFieldGroup.Refresh();
    }
    public void RefreshPillowWheelGroup()
    {
        PillowWheelGroup.Refresh();
    }
    public void RefreshCatchFishGroup()
    {
        CatchFishGroup.Refresh();
    }
    public void RefreshClimbTreeGroup()
    {
        ClimbTreeGroup.Refresh();
    }
    public void RefreshDogPlayGroup()
    {
        // DogPlayGroup.Refresh();
    }
    private void InitHeadIcon()
    {
        if (personHead == null)
            return;

        if (portraitSpineObj != null)
            return;

        StorageTaskItem taskItem = storageTaskItem;
        if (taskItem == null)
            return;

        if (storageTaskItem.Type == (int)MainOrderType.Limit || storageTaskItem.Type == (int)MainOrderType.Craze|| storageTaskItem.Type == (int)MainOrderType.Team)
        {
            _skeletonGraphic = null;
            return;
        }
        
        headIndex = taskItem.HeadIndex;
        if (headIndex <= 0 || headIndex > OrderConfigManager.Instance._orderHeadSpines.Count)
        {
            headIndex = taskItem.HeadIndex = MainOrderManager.Instance.RandomHeadIndex();
        }

        portraitSpineObj = GamePool.ObjectPoolManager.Instance.Spawn(string.Format(ObjectPoolName.PortraitSpine, OrderConfigManager.Instance.GetSpineName(headIndex)));
        portraitSpineObj.transform.SetParent(personHead);
        portraitSpineObj.transform.localScale = Vector3.one;
        portraitSpineObj.transform.localRotation = Quaternion.identity;
        ((RectTransform)portraitSpineObj.transform).anchoredPosition = Vector3.zero;

        _skeletonGraphic = portraitSpineObj.transform.GetComponentInChildren<SkeletonGraphic>();
        PlaySkeletonAnimation(OrderConfigManager.Instance.GetSpineName(headIndex, 1));
        UpdateSkin();
    }

    public void RefreshWaringStatus()
    {
        if (storageTaskItem == null)
            return;

        if (m_itemList == null || m_itemList.Count == 0)
            return;

        for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
        {
            int id = storageTaskItem.ItemIds[i];

            if (m_itemList.Count <= i)
                break;

            var item = m_itemList[i];
            bool haveMergeItem = MergeResourceManager.Instance.HaveMergeItem(id, MergeBoardEnum.Main,true);
            item.SetWarningStatus(!haveMergeItem);
        }
    }

    private bool CompleteTaskUseBagItem;//是否需要通过消耗背包里的item来完成任务
    private List<int> CompleteTaskUseBagItemList = new List<int>();

    public void AddUseBagItem(int id, int count)
    {
        for (var i = 0; i < count; i++)
        {
            CompleteTaskUseBagItemList.Add(id);
        }
    }
    public virtual bool RefreshView(int mergeId = -1, bool isInit = false)
    {
        //RefreshWaringStatus();

        if (storageTaskItem == null)
            return false;
        
        if (storageTaskItem.Type != (int)MainOrderType.Limit && MainOrderManager.Instance.CurMergeTask == this && storageTaskItem.Type != (int)MainOrderType.Craze)
            return false;

        if (mergeId > 0)
        {
            if(!storageTaskItem.ItemIds.Contains(mergeId))
                return false;
        }

        Dictionary<int, int> mergeItemCounts = MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.Main);
        Dictionary<int, int> bagItemCounts = MergeManager.Instance.GetBagItemCounts(MergeBoardEnum.Main);
        Dictionary<int, int> vipBagItemCounts = MergeManager.Instance.GetVipBagItemCounts(MergeBoardEnum.Main);
        Dictionary<int, int> needItemCounts = new Dictionary<int, int>();
        Dictionary<int, int> needItemCounts2 = new Dictionary<int, int>();
        for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
        {
            int id = storageTaskItem.ItemIds[i];
            var count = 0;
            if (mergeItemCounts.TryGetValue(id, out var count1))
                count += count1;
            if (bagItemCounts.TryGetValue(id,out var count2))
                count += count2;
            if (vipBagItemCounts.TryGetValue(id,out var count3))
                count += count3;
            if (count == 0)
                continue;
            
            if (!needItemCounts.ContainsKey(id))
                needItemCounts.Add(id, count);
            
            if (!needItemCounts2.ContainsKey(id))
                needItemCounts2.Add(id, 1);
            else
                needItemCounts2[id] += 1;
        }

        bool isComplte = true;
        IsComplteSubTask = false;
        CompleteTaskUseBagItem = false;
        CompleteTaskUseBagItemList.Clear();
        bool checkBag = false;
        for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
        {
            int id = storageTaskItem.ItemIds[i];

            var item = m_itemList[i];

            int curCount = CommonUtils.GetValue(needItemCounts, id);
            int totalCount = storageTaskItem.ItemNums[i];
            item.SetCountActive(totalCount > 1);
            item.SetShopTipStatus();
            if (needItemCounts.ContainsKey(id))
            {
                needItemCounts[id] -= totalCount;
                needItemCounts[id] = Math.Max(needItemCounts[id], 0);
            }

            item.SetCountText(string.Format("{0}/{1}", curCount, totalCount));
            if (curCount < totalCount)
            {
                isComplte = false;
            }
            else
            {
                if (mergeItemCounts.TryGetValue(id, out var count1))
                {
                    checkBag = true;
                    if (count1 < totalCount)
                    {
                        CompleteTaskUseBagItem = true;
                        AddUseBagItem(id,totalCount - count1);
                    }
                }
                else
                {
                    CompleteTaskUseBagItem = true;
                    AddUseBagItem(id,totalCount);
                }
                IsComplteSubTask = true;
            }

            bool oldDone = item.IsDone();
            bool isDone = curCount >= totalCount;
            item.SetDoneStatus(isDone);

            if (!isInit && isDone && !oldDone)
            {
                if(storageTaskItem.Type == (int)MainOrderType.Time)
                    TimeOrderModel.Instance.CanShowGift(i);
                else if(storageTaskItem.Type == (int)MainOrderType.Limit)
                    LimitTimeOrderModel.Instance.CanShowGift(i);
            }
        }
        if (checkBag)
        {
            for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
            {
                int id = storageTaskItem.ItemIds[i];
                if (mergeItemCounts.TryGetValue(id, out var count1))
                {
                    if (count1 < needItemCounts2[id])
                    {
                        CompleteTaskUseBagItem = true;
                        AddUseBagItem(id, needItemCounts2[id] - count1);
                        break;
                    }
                }
            }
        }
        bool isDebugComplete = MainOrderManager.Instance.IsDebugCompleteTask(TableTaskId());
        if (isDebugComplete)
            isComplte = isDebugComplete;

        // if (personHead.gameObject.activeSelf != !isComplte)
        //     personHead.gameObject.SetActive(!isComplte);

        if (completButton.gameObject.activeSelf != isComplte)
            completButton.gameObject.SetActive(isComplte);

        if(isComplte && isComplteTask !=  isComplte)
            PlayCompleteEf(isComplte, isInit);
        
        isComplteTask = isComplte;
        PlaySkeletonAnimation(isComplteTask?OrderConfigManager.Instance.GetSpineName(headIndex, 2):OrderConfigManager.Instance.GetSpineName(headIndex, 1));
        
        _completeObjs.ForEach(a=>a.SetComplete(isComplteTask));
        
        if (isComplteTask)
            MergeFtueBiManager.Instance.SendFtueBi(MergeFtueBiManager.SendType.FinishTask, storageTaskItem.Id);

        return isComplteTask;
    }

    public bool ContainsItemId(int id)
    {
        return storageTaskItem.ItemIds.Contains(id);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetChangeSkin>(OnChangeDogSkin);
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_JOIN, UpdateBalloonRacing);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_JOIN, UpdateRabbitRacing);
    }

    private void ShowRewardGroup()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupOrderReward,storageTaskItem);   
    }
    public void OnCompleteButton()
    {
        if(MainOrderManager.Instance.CurMergeTask != null)
            return;
        
        if (storageTaskItem == null)
            return;

        if (!CanTouchComplete)
            return;
        if(MergeTaskTipsController.Instance._isMoveing)
            return;
        Action completeTask = () =>
        {
            completButton.gameObject.SetActive(false);
            SetArrowActive(false);
            MergeTaskTipsController.Instance.MoveToIndex(siblingIndex, () =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Task, storageTaskItem.OrgId.ToString());
                MainOrderManager.Instance.CompleteTask(this);
            });
        };
        if (CompleteTaskUseBagItem)
        {
            UIPopupCompleteOrderController.ShowEnsurePopup(CompleteTaskUseBagItemList,completeTask);
            return;
        }
        completeTask();
    }

    public void PlayShake()
    {
        if (animator == null)
            return;

        animator.Play("shake", 0, 0f);
    }

    public void Complete()
    {
        isComplteTask = false;
        isComplteSubTask = false;
        CanTouchComplete = false;
        SetArrowActive(false);
        PlayCompleteEf(false);

        completeObj?.gameObject.SetActive(true);
        //guideObjAnimator.Play("appear", 0, 0f);

        if (_teamIconNode != null)
        {
            GameObject.Destroy(_teamIconNode.gameObject);
            _teamIconNode = null;
        }
        if (portraitSpineObj == null)
            return;

        if (headIndex <= 0)
            return;

        GamePool.ObjectPoolManager.Instance.DeSpawn(string.Format(ObjectPoolName.PortraitSpine, OrderConfigManager.Instance.GetSpineName(headIndex)), portraitSpineObj);
        portraitSpineObj = null;
        _skeletonGraphic = null;
        headIndex = -1;
    }

    public void PlayCompleteEf(bool playBomEf, bool delayAnim = false)
    {
        if(playBomEf)
            completeEf_hint?.gameObject.SetActive(false);
            
        completeEf_hint?.gameObject.SetActive(playBomEf);
        
        if(!CanTouchComplete)
            return;
        
        if(!playBomEf)
            return;
        
        if (delayAnim)
        {
            StartCoroutine(CommonUtils.DelayWork(0.1f, () =>
            {
                animator.Play("Finish_01", 0, 0);
            }));
            
            return;
        }
        
        animator.Play("Finish_01", 0, 0);
    }

    public void CheckTaskGuide()
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return;

        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ClickTaskNeedItem))
            return;

        StartCoroutine(CommonUtils.DelayWork(0.2f, () =>
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(m_itemList[0].gameObject.transform);
            
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClickTaskNeedItem,
                m_itemList[0].gameObject.transform as RectTransform, topLayer:topLayer);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClickTaskNeedItem, m_itemList[0].id.ToString());
        }));
    }


    public int GetFirstNeedItemId()
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return -1;

        return m_itemList[0].id;
    }

    public void SetArrowActive(bool isActive)
    {
        arrow.SetActive(isActive);
    }

    private void RefreshRepeating()
    {
        
        for (int i = 0; i < storageTaskItem.RewardTypes.Count; i++)
        {
            if(i >= 2)
                break;
            var rewardType = storageTaskItem.RewardTypes[i];
            rewardType = MainOrderManager.ChangeTaskRewardType(rewardType);
            if (rewardType == (int) UserData.ResourceId.RecoverCoinStar ||
                rewardType == (int) UserData.ResourceId.Coin)
            {
                var multiValue = ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.Coin);
                _rewardInfos[i]._multipleText.SetText("X"+multiValue);
                _rewardInfos[i]._multipleText.gameObject.SetActive(multiValue > 1);
            }
        }
        
        _dogHopeObj.gameObject.SetActive(DogHopeModel.Instance.IsOpenActivity() && storageTaskItem?.DogCookiesNum > 0);
        _mermaidText.transform.parent.gameObject.SetActive(!MainOrderManager.Instance.IsSpecialTask(storageTaskItem.OrgId)&&  MermaidModel.Instance.IsStart() && !MermaidModel.Instance.IsFinished());
        _easter2024EggGroup.gameObject.SetActive(Easter2024Model.Instance.IsStart() && !MainOrderManager.Instance.IsSpecialTask(storageTaskItem.OrgId));
        _snakeLadderTurntableGroup.gameObject.SetActive(SnakeLadderModel.Instance.IsStart() && !MainOrderManager.Instance.IsSpecialTask(storageTaskItem.OrgId));
            
        _saveTheWhalesGroup.gameObject.SetActive(SaveTheWhalesModel.Instance.IsJoin() && !MainOrderManager.Instance.IsSpecialTask(storageTaskItem.OrgId));

        _turntableObj.gameObject.SetActive(TurntableModel.Instance.IsOpened());
        _multipleScoreMermaid.gameObject.SetActive(false);
        if (MermaidModel.Instance.IsStart() && MultipleScoreModel.Instance.IsOpenActivity())
        {
            var mul = MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.Mermaid);
            if (mul > 1)
            {
                _multipleScoreMermaid.SetText("X"+mul);
                _multipleScoreMermaid.gameObject.SetActive(true);
            }
        }

        {
            var multiValue = ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.DogHope);
            _multipleCouponDogHope.SetText("X"+multiValue);
            _multipleCouponDogHope.gameObject.SetActive(multiValue > 1);
        }
        {
            var multiValue = ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.SnakeLadder);
            _multipleCouponSnakeLadder.SetText("X"+multiValue);
            _multipleCouponSnakeLadder.gameObject.SetActive(multiValue > 1);
        }

        RefreshRepeatingTeamOrder();
        RefreshRepeatingTimeOrder();
        RefreshRepeatingLimitOrder();
        RefreshRepeatingCrazeOrder();
        RefreshExtraOrderRewardCouponGroup();
        RefreshThemeDecorationGroup();
        RefreshSlotMachineGroup();
        RefreshRepeatingKeepPetOrder();
        RefreshMonopolyGroup();
        RefreshMixMasterGroup();
        RefreshTurtlePangGroup();
        RefreshStarrySkyCompassGroup();
        RefreshZumaGroup();
        RefreshFishCultureGroup();
        RefreshPhotoAlbumGroup();
        RefreshParrotGroup();
        RefreshFlowerFieldGroup();
        RefreshPillowWheelGroup();
        RefreshCatchFishGroup();
        RefreshClimbTreeGroup();
        RefreshDogPlayGroup();
        RefreshRepeatingBalloonRacing();
        RefreshRepeatingRabbitRacing();
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private void PlaySkeletonAnimation(string animName)
    {
        if(_skeletonGraphic == null)
            return;

        TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
        if(trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
            return;
        
        _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
        _skeletonGraphic.Update(0);
    }

    public void UpdateSkin()
    {
        if (storageTaskItem != null && storageTaskItem.HeadIndex == 13)//狗头
        {
            if (_skeletonGraphic == null)
                return;
            var skinName = KeepPetModel.Instance.Storage.SkinName;
            _skeletonGraphic.Skeleton.SetSkin(skinName);
            _skeletonGraphic.Skeleton.SetSlotsToSetupPose();
            _skeletonGraphic.AnimationState.Apply(_skeletonGraphic.Skeleton);
        }
    }
    public void OnChangeDogSkin(EventKeepPetChangeSkin evt)
    {
        UpdateSkin();
    }
}
