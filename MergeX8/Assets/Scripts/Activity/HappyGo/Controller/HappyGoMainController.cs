using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using TMPro;
using System;
using DragonU3DSDK.Network.API.Protocol;

public partial class HappyGoMainController : UIWindowController
{
    Transform mRoot;
    public HappyGoMergeBoard mMergeBoard;
    Button mhappyGoBtn;
    private GameObject mhappyGoBtnRedPoint;
    private LocalizeTextMeshProUGUI mhappyGoBtnText;

    Button mRewardBtn;

    public float RootScale
    {
        get { return mRoot.transform.localScale.x; }
    }
    
    private LocalizeTextMeshProUGUI backNumText;
    private GameObject backRedPoint;
    private Animator backAnimator;
    
    Image rewardIcon;
    private Transform taskGuideRoot;

    private Animator _animator;
    
    public Vector3 rewardBtnPos
    {
        get { return mRewardBtn.transform.position; }
    }

    public Transform rewardBtnTrans
    {
        get { return mRewardBtn.transform; }
    }

    public RectTransform bagTrans
    {
        get { return mhappyGoBtn.transform as RectTransform; }
    }

    public RectTransform boardTrans
    {
        get { return mMergeBoard.transform as RectTransform; }
    }


    GameObject lastDraginGo;
    bool multiTouchEnabled;
    private GameObject noticeGo;
    [NonSerialized] public Vector3 bagItemPostion;
    public HappyGoMergeClickTips mergeClickTips;
    public int selcedItemId { get; set; }
    public static HappyGoMainController Instance;

    public Transform FlyNode { get; private set; }

    private LocalizeTextMeshProUGUI rewardNumText;
    private Animator storeBtnAnimator;
    private Image storeBubbleImage; 
    private GameObject storeRedPoint = null;

    private GameObject guideMask = null;

    private CanvasGroup bagCanvasGroup = null;
    private CanvasGroup rewardCanvasGroup = null;
    private bool isInit = false;

    private RectTransform bottomGroupTransform;
  
    private int curRewardCount = 0;

    public HappyGoReward _happyGoReward;
    private LocalizeTextMeshProUGUI happyGoTimeText;

    //事件
    //end 事件
    public override void PrivateAwake()
    {
        Instance = this;
        isPlayDefaultAudio = false;
        _animator = transform.GetComponent<Animator>();
        
        mRoot = transform.Find("Root");
        FlyNode = transform.Find("Root/FlyNode");
        mMergeBoard = mRoot.GetComponentDefault<HappyGoMergeBoard>("Board");
        mhappyGoBtn = mRoot.GetComponentDefault<Button>("BottomGroup/BagBtn");
        mhappyGoBtnRedPoint = mRoot.Find("BottomGroup/BagBtn/RedPoint").gameObject;
        mhappyGoBtnText = mRoot.GetComponentDefault<LocalizeTextMeshProUGUI>("BottomGroup/BagBtn/Text");
        bagCanvasGroup = mhappyGoBtn.gameObject.GetComponent<CanvasGroup>();
        happyGoTimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();

        mRewardBtn = mRoot.GetComponentDefault<Button>("TopGroup/RewardBtn");
        
        rewardNumText = mRewardBtn.GetComponentDefault<LocalizeTextMeshProUGUI>("Icon/RedPoint/Label");
        rewardCanvasGroup = mRewardBtn.gameObject.GetComponent<CanvasGroup>();

        guideMask = transform.Find("Root/Board/GuideMask").gameObject;
        SetGuideMaskActive(false);

        rewardIcon = mRewardBtn.GetComponentDefault<Image>("Icon");
        mRewardBtn.gameObject.SetActive(true);
        
        backNumText = transform.Find("Root/BottomGroup/BackBtn/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        backRedPoint = transform.Find("Root/BottomGroup/BackBtn/RedPoint").gameObject;
        backRedPoint.SetActive(false);
        backAnimator = transform.Find("Root/BottomGroup").GetComponent<Animator>();
        
        bottomGroupTransform = transform.Find("Root/BottomGroup") as RectTransform;
        _happyGoReward = mRoot.Find("TopGroup/HappyGoReward").gameObject.AddComponent<HappyGoReward>();
        _happyGoReward.Init();
        taskGuideRoot = mRoot.Find("TaskList/taskGuideArray");
        noticeGo = mRoot.transform.Find("TaskBtn").gameObject;
        mergeClickTips = noticeGo.AddComponent<HappyGoMergeClickTips>();
        mergeClickTips.SetBoardId(MergeBoardEnum.HappyGo);
        if (!CommonUtils.IsLE_16_10())
        {
            var ratio = CommonUtils.GetScreenRatio();
            float ratioLocal = ratio / (1720 / 768);
            ratioLocal = ratioLocal > 1 ? 1 : ratioLocal;
            mRoot.localScale = new Vector3(ratioLocal, ratioLocal, ratioLocal);
        }

        CommonUtils.NotchAdapte(mRoot.transform);

        mMergeBoard.AddEventObject(mhappyGoBtn.transform as RectTransform);
        CommonUtils.AddClickListener(mhappyGoBtn, OnClickBagBtn);
        CommonUtils.AddClickListener(mRewardBtn, OnClickRewardBtn);
        Button backBtn = mRoot.GetComponentDefault<Button>("BottomGroup/BackBtn");
        CommonUtils.AddClickListener(backBtn, OnClickBackBtn);
        EventDispatcher.Instance.AddEventListener(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH, RefreshRewards);
        // RefreshShopRedPoint();
      
        InvokeRepeating("RefreshRedpointByTime", 0, 1f);
        // UpdateTaskRedPoint();
        OnRefreshOnCompleteTask(null);
        
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(mhappyGoBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.HappyGoLevelUp, mhappyGoBtn.transform as RectTransform, topLayer: topLayer);
    }

  
    IEnumerator Start()
    {
        curRewardCount = MergeManager.Instance.GetRewardCount(MergeBoardEnum.HappyGo);
        
        // SetCanvas("Root/BottomGroup/RewardBtn/vfx_BG",canvas.sortingOrder + 1);
        // SetCanvas("Root/BottomGroup/RewardBtn/Icon",canvas.sortingOrder + 2);
        // SetCanvas("Root/BottomGroup/BagBtn/vfx_mergeing",canvas.sortingOrder + 3);
        // SetCanvas("Root/BottomGroup/BagBtn/vfx_merge",canvas.sortingOrder + 4);
        // SetCanvas("Root/BottomGroup/RVBtn/BubbleGroup",canvas.sortingOrder + 5);

        yield return new WaitForEndOfFrame();
        CommonUtils.SetShieldButTime(mRewardBtn.gameObject);
        RefreshRewards(false);
        isInit = true;
    }

    private void SetCanvas(string path, int order)
    {
        Canvas canvas = GetItem<Canvas>(path);
        if(canvas == null)
            return;

        canvas.sortingOrder = order;
    }
    
    protected override void OnOpenWindow(params object[] objs)
    {
    }
    
    private void OnEnable()
    {
        // UpdateTaskRedPoint();
        InvokeRepeating("Invoke_SetHappyGoCDTime", 0f, 1f);

        if(!isInit)
            return;
        
        RefreshRewards(false);
        mMergeBoard.RefreshTask(new BaseEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.HappyGo));
        curRewardCount = MergeManager.Instance.GetRewardCount(MergeBoardEnum.HappyGo);

        // rvAnimObj.gameObject.SetActive(HappyGoDailyRVModel.Instance.IsRVShopOpen());
    }

    private void OnDisable()
    {
        CancelInvoke("Invoke_SetHappyGoCDTime");
    }
    private void Invoke_SetHappyGoCDTime()
    {
        var cd= HappyGoModel.Instance.GetActivityLeftTime();
        if (cd == 0)
        {
            OnClickBackBtn();
        }
        else
        {
            happyGoTimeText.SetText(HappyGoModel.Instance.GetActivityLeftTimeString());
        }
    }
   
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH, RefreshRewards);
    }

 
    void RefreshRewards(BaseEvent e)
    {
        if(e != null)
            curRewardCount = MergeManager.Instance.GetRewardCount(MergeBoardEnum.HappyGo);
        
        RefreshRewards(true);
        
        EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BOARD_REFRESH);
    }

    void RefreshTask(BaseEvent e)
    {
        // UpdateTaskRedPoint();
    }

    void RefreshRewards(bool isCheckGuide)
    {
        bool isShow = curRewardCount > 0;

        bool openStore = true;// UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GameStore);
        bool openBag = true;// UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Bag); 

        if (openStore && openBag)
        {
            rewardCanvasGroup.alpha = 1;
            mRewardBtn.gameObject.SetActive(isShow);
        }
        else
        {
            if (rewardCanvasGroup.alpha == 0 && isShow)
            {
                mRewardBtn.gameObject.SetActive(false);
                mRewardBtn.gameObject.SetActive(true);
            }
            rewardCanvasGroup.alpha = isShow ? 1 : 0;
        }
        
        
        rewardNumText.SetText(MergeManager.Instance.GetRewardCount(MergeBoardEnum.HappyGo).ToString());
        RefreshRewardIcon();

        if (isShow && isCheckGuide && gameObject.activeSelf)
        {
            MergeGuideLogic.Instance.CheckGetReward();
        }
    }

    private void RefreshRewardIcon()
    {
        if (MergeManager.Instance.GetRewardCount(MergeBoardEnum.HappyGo) > 0)
        {
            StorageMergeItem item =MergeManager.Instance.GetRewardItem(0,MergeBoardEnum.HappyGo);
            var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
            if (itemConfig == null)
                return;
            rewardIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            rewardIcon.gameObject.SetActive(true);
        }
    }

    void OnClickBagBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.HappyGoLevelUp);
        UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoReward);
        
    }

    private float clickRewardsTime = 0;

    public void OnClickRewardBtn()
    {
        if(rewardCanvasGroup.alpha == 0)
            return;
        
        if (curRewardCount <= 0)
            return;

        int emptyIndex = MergeManager.Instance.FindEmptyGrid(mMergeBoard.activeIndex,MergeBoardEnum.HappyGo);
        if (emptyIndex == -1)
        {
            MergePromptManager.Instance.ShowTextPrompt(mRewardBtn.transform.position, 1.5f);
            return;
        }

        // 3级前 加入2秒冷却时间
        if (ExperenceModel.Instance.GetLevel() < 3)
        {
            if (Time.time - clickRewardsTime < 2) 
                return;
            clickRewardsTime = Time.time;
        }
        
        curRewardCount--;
        StorageMergeItem storageMergeItem;
        MergeManager.Instance.RemoveRewardItem(0, out storageMergeItem, MergeBoardEnum.HappyGo,false);
        if (HappyGoMergeClickTips.Instance != null)
            HappyGoMergeClickTips.Instance.SetNoFocusStatus();

        rewardNumText.SetText(MergeManager.Instance.GetRewardCount(MergeBoardEnum.HappyGo).ToString());
        MergeManager.Instance.SetNewBoardItem(emptyIndex, storageMergeItem.Id, 1, RefreshItemSource.rewards, MergeBoardEnum.HappyGo, -1, false);
        RefreshRewards(true);

        TableMergeItem mergeItem = MergeResourceManager.Instance.ResourcesTableMerge;
        FlyGameObjectManager.Instance.FlyObject(storageMergeItem.Id, rewardBtnPos,
            mMergeBoard.IndexToPosition(emptyIndex), 5f,
            () =>
            {
                ShakeManager.Instance.ShakeLight();
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardEnum.HappyGo,emptyIndex, -1, RefreshItemSource.rewards);
                if(mergeItem != null)
                    MergeResourceManager.Instance.GetMergeResource(mergeItem,MergeBoardEnum.HappyGo);
            });
        RefreshRewardIcon();
        AudioManager.Instance.PlaySound(13);
        //AudioManager.Instance.PlaySound(HotelSound.sfx_ui_Common_ClickButton);
        SendUseAwardBagBi(storageMergeItem.Id);
        MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Reward,MergeBoardEnum.HappyGo);
    }

    private void SendUseAwardBagBi(int id)
    {
        var product = GameConfigManager.Instance.GetItemConfig(id);
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemMoveStorageToGame,
            itemAId = product.id,
            ItemALevel = product.level,
            isChange =  false,
            data1 = "1",
            extras = new Dictionary<string, string>
            {
                {"from","storage"},
                {"to","game"},
            }
        });
        if (product.type == 2)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventChestPop);
        }
    }
    private void OnRefreshOnCompleteTask(BaseEvent obj)
    {
        bool openBag = true;//UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Bag);

        float bagAlpha = bagCanvasGroup.alpha;
        bagCanvasGroup.alpha = openBag ? 1 : 0;
        
    }

    public void OnClickBackBtn()
    {
        // if(!HappyGoGuideSubSystem.Instance.CanBackHome())
        //     return;
        //
        // // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBackHg);
        //
        // HappyGoGuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClickBackButton);
        SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
    }

    private void RefreshShopRedPoint()
    {
        mhappyGoBtnRedPoint.SetActive(HappyGoModel.Instance.IsCanGetReward());
      
    }

    private void RefreshRedpointByTime()
    {
        RefreshShopRedPoint();
    }

 
    public void PlayAnimation(bool isAppear, Action callBack)
    {
        if (_animator == null)
        {
            callBack?.Invoke();
            return;
        }

        string animName = isAppear ? "appear" : "disappear";
        
        StartCoroutine(CommonUtils.PlayAnimation(_animator, animName, "", callBack));
    }

    public void SetGuideMaskActive(bool active, int order = 0)
    {
        guideMask.gameObject.SetActive(active);
        
        if(active)
            SetCanvas("Root/Board/GuideMask",order);
    }

    public bool RewardItemIsShow()
    {
        if (!gameObject.activeSelf)
            return false;

        if (!mRewardBtn.gameObject.activeSelf)
            return false;

        if (rewardCanvasGroup.alpha == 0)
            return false;

        return true;
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}