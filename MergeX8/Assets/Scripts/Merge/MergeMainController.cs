using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonU3DSDK.Storage;
using System;
using System.Security.Cryptography;
using Deco.Node;
using Decoration;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.ABTest;
using Framework.Async;
using Gameplay;
using Manager;
using Merge.Order;
using MiniGame;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MergeMainController : UIWindowController
{
    
    public MergeBoard MergeBoard
    {
        get { return _mergeBoard; }
    }
    
    MergeBoard _mergeBoard;
    Transform _mRoot;
    public Button _mBagBtn;
    Transform _mBagMasterIcon;
    Button backBtn;
    public GuideArrowController BackBtnGuideArrow;
    private Animator _bagAnimator;
    
    public float RootScale
    {
        get { return _mRoot.transform.localScale.x; }
    }

    private LocalizeTextMeshProUGUI backNumText;
    private GameObject backRedPoint;
    private Animator backAnimator;

    private Transform taskGuideRoot;

    private Animator _animator;

    public Vector3 rewardBtnPos
    {
        get { return MergeTaskTipsController.Instance._mergeRewardItem.transform.position; }
    }

    public Transform rewardBtnTrans
    {
        get { return MergeTaskTipsController.Instance._mergeRewardItem.transform; }
    }

    public RectTransform bagTrans
    {
        get { return _mBagBtn.transform as RectTransform; }
    }
    public RectTransform backTrans
    {
        get { return backBtn.transform as RectTransform; }
    }   
   
    GameObject lastDraginGo;
    bool multiTouchEnabled;
    private GameObject noticeGo;
    [NonSerialized] public Vector3 bagItemPostion;
    public MergeClickTips mergeClickTips;
    public int selcedItemId { get; set; }
    public static MergeMainController Instance;

    public Transform FlyNode { get; private set; }

    private Animator storeBtnAnimator;
    private Image storeBubbleImage;
    private GameObject storeRedPoint = null;

    private GameObject intoBagEffectObj = null;
    private GameObject intoBagMergeObj = null;

    private GameObject guideMask = null;

    public MergeMainBalloon _balloon;
    private bool isInit = false;

    private RectTransform bottomGroupTransform;
    
    public MergePigBoxController PigBoxController;
    
    private Transform _topBG;
    private Transform _taskBG;
    private Transform _boardBG;
    private Transform _boardMainBG;
       
    private Transform mermaid_topBG;
    private Transform mermaid_taskBG;
    private Transform mermaid_boardBG;
    private Transform mermaid_boardMainBG;

    public Transform BubbleHangPoint;
    //事件
    //end 事件
    public override void PrivateAwake()
    {
        Instance = this;
        isPlayDefaultAudio = false;
        _animator = transform.GetComponent<Animator>();

        _mRoot = transform.Find("Root");
        FlyNode = transform.Find("Root/FlyNode");
        _mergeBoard = _mRoot.GetComponentDefault<MergeBoard>("Board");
        // MergeBoard.SetBoardID((int)MergeBoardEnum.Main);
        _mBagBtn = _mRoot.GetComponentDefault<Button>("BottomGroup/BagBtn");
        _bagAnimator = _mBagBtn.transform.Find("Icon").GetComponent<Animator>();
        
        _mBagMasterIcon = _mRoot.GetComponentDefault<Transform>("BottomGroup/BagBtn/SpecialIcon");
        _mBagMasterIcon.gameObject.SetActive(MasterCardModel.Instance.IsBuyMasterCard);

        guideMask = transform.Find("Root/GuideMask").gameObject;
        SetGuideMaskActive(false);

        backNumText = transform.Find("Root/BottomGroup/BackBtn/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        backRedPoint = transform.Find("Root/BottomGroup/BackBtn/RedPoint").gameObject;
        backAnimator = transform.Find("Root/BottomGroup").GetComponent<Animator>();

        bottomGroupTransform = transform.Find("Root/BottomGroup") as RectTransform;

        noticeGo = bottomGroupTransform.transform.Find("TaskBtn").gameObject;
        mergeClickTips = noticeGo.AddComponent<MergeClickTips>();
        mergeClickTips.SetBoardId(MergeBoardEnum.Main);
        _mRoot.Find("TaskList").gameObject.AddComponent<MergeTaskTipsController>();
        _balloon = transform.Find("BoxWithBalloon").gameObject.AddComponent<MergeMainBalloon>();
        _balloon.Init();

        PigBoxController = transform.Find("Root/PigBoxIcon").gameObject.AddComponent<MergePigBoxController>();

        BubbleHangPoint = transform.Find("Root/BubblePoint");
            
        CommonUtils.NotchAdapte(_mRoot.transform);

        MergeBoard.AddEventObject(_mBagBtn.transform as RectTransform);
        CommonUtils.AddClickListener(_mBagBtn, OnClickBagBtn);
        backBtn = _mRoot.GetComponentDefault<Button>("BottomGroup/BackBtn");
        CommonUtils.AddClickListener(backBtn, OnClickBackBtn);
        BackBtnGuideArrow = _mRoot.Find("BottomGroup/BackBtn/GuideArrow").gameObject.AddComponent<GuideArrowController>();
        BackBtnGuideArrow.Init();
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_DRAGIN, OnMergeBoardDragIn);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_POINTERUP, OnMergeBoardPointerUp);
        EventDispatcher.Instance.AddEventListener(EventEnum.TASK_REFRESH, RefreshTask);
        EventDispatcher.Instance.AddEventListener(EventEnum.TASK_COMPLETE_REFRESH, OnRefreshOnCompleteTask);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_REFRESH, RefreshMergeBoard);
        
        UpdateTaskRedPoint();
        OnRefreshOnCompleteTask(null);
        intoBagEffectObj = transform.Find("Root/BottomGroup/BagBtn/vfx_merge").gameObject;
        intoBagEffectObj.gameObject.SetActive(false);

        intoBagMergeObj = transform.Find("Root/BottomGroup/BagBtn/vfx_mergeing").gameObject;
        intoBagMergeObj.gameObject.SetActive(false);

        _topBG = transform.Find("BG/Normal");
        _taskBG = transform.Find("Root/TaskList/BG/Normal");
        _boardBG = transform.Find("Root/Board/BG/Normal");
        _boardMainBG = transform.Find("Root/Board/Main/BG/Normal");
    }

    private MergeBoardTheme curTheme;
    public class MergeBoardTheme
    {
        public Func<string> TopPath;
        public Func<string> TaskPath;
        public Func<string> BoardPath;
        public Func<string> MainPath;

        public Transform _top;
        public Transform Top
        {
            get
            {
                if (_top == null)
                {
                    var obj=ResourcesManager.Instance.LoadResource<GameObject>(TopPath());
                    _top = Instantiate(obj, _mainController._topBG.parent).transform;
                    _top.SetSiblingIndex(_mainController._topBG.GetSiblingIndex()+1);
                    // Destroy(obj);
                }
                return _top;
            }
        }
        public Transform _task;
        public Transform Task
        {
            get
            {
                if (_task == null)
                {
                    var obj=ResourcesManager.Instance.LoadResource<GameObject>(TaskPath());
                    _task = Instantiate(obj, _mainController._taskBG.parent).transform;
                    _task.SetSiblingIndex(_mainController._taskBG.GetSiblingIndex()+1);
                    // Destroy(obj);
                }
                return _task;
            }
        }
        
        public Transform _board;
        public Transform Board
        {
            get
            {
                if (_board == null)
                {
                    var obj=ResourcesManager.Instance.LoadResource<GameObject>(BoardPath());
                    _board = Instantiate(obj, _mainController._boardBG.parent).transform;
                    _board.SetSiblingIndex(_mainController._boardBG.GetSiblingIndex()+1);
                    // Destroy(obj);
                }
                return _board;
            }
        }
        
        public Transform _main;
        public Transform Main
        {
            get
            {
                if (_main == null)
                {
                    var obj=ResourcesManager.Instance.LoadResource<GameObject>(MainPath());
                    _main = Instantiate(obj, _mainController._boardMainBG.parent).transform;
                    _main.SetSiblingIndex(_mainController._boardMainBG.GetSiblingIndex()+1);
                    // Destroy(obj);
                }
                return _main;
            }
        }
        
        private MergeMainController _mainController;
        
        public void BindMergeMainController(MergeMainController mainController)
        {
            if (_mainController != mainController)
            {
                if (_top && TopPath!=null)
                    Destroy(_top.gameObject);
                if (_task && TaskPath!=null)
                    Destroy(_task.gameObject);
                if (_board && BoardPath!=null)
                    Destroy(_board.gameObject);
                if (_main && MainPath!=null)
                    Destroy(_main.gameObject);
                
                _mainController = mainController;   
            }
        }
    }

    IEnumerator Start()
    {

        SetCanvas("Root/BottomGroup/BagBtn/vfx_mergeing",canvas.sortingOrder + 1);
        SetCanvas("Root/BottomGroup/BagBtn/vfx_merge",canvas.sortingOrder + 2);
        
        curTheme = GetMergeBoardTheme(MergeBoardThemeEnum.Default);
        curTheme.BindMergeMainController(this);
        curTheme.Top.gameObject.SetActive(true);
        curTheme.Task.gameObject.SetActive(true);
        curTheme.Board.gameObject.SetActive(true);
        curTheme.Main.gameObject.SetActive(true);   
        
        int ballOrder = canvas.sortingOrder + 6;
        if (CurrencyGroupManager.Instance != null && CurrencyGroupManager.Instance.currencyController != null)
            ballOrder = CurrencyGroupManager.Instance.currencyController.canvas.sortingOrder + 1;
        SetCanvas("BoxWithBalloon", ballOrder);

        yield return new WaitForEndOfFrame();
        MergeManager.Instance.RefreshBubbleInfo();
        isInit = true;

        UpdateUI();
        UpdateGuideMaskOrder();
    }

    private void SetCanvas(string path, int order)
    {
        Canvas canvas = GetItem<Canvas>(path);
        if (canvas == null)
            return;

        canvas.sortingOrder = order;
    }

    private void UpdateGuideMaskOrder()
    {
        int order = 100;
        if (CurrencyGroupManager.Instance.currencyController != null)
            order = CurrencyGroupManager.Instance.currencyController.canvas.sortingOrder + 1;
        
        guideMask.GetComponent<Canvas>().sortingOrder = order;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
    }
    
    private void OnEnable()
    {
        OnRefreshOnCompleteTask(null);
        UpdateTaskRedPoint();
        InvokeRepeating("CheckBalloon", 60, 60);
        PlayBagAnim("normal");
        
        if (!isInit)
            return;

        UpdateUI();

    }

    private void UpdateUI()
    {
        if (_mBagMasterIcon != null)
            _mBagMasterIcon.gameObject.SetActive(MasterCardModel.Instance.IsBuyMasterCard);

        MergeMainController.Instance.MergeBoard.RefreshTask(new BaseEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main));
        
        UpdateGuideMaskOrder();
        
        if (MainOrderManager.Instance.IsFinishSealTask())
            MergeManager.Instance.CheckSealReplaceItem(MergeBoardEnum.Main);
        
        if (MainOrderManager.Instance.IsFinishCapybaraTask())
            MergeManager.Instance.CheckCapybaraReplaceItem(MergeBoardEnum.Main);
        
        if (MainOrderManager.Instance.IsFinishDolphinTask())
            MergeManager.Instance.CheckDolphinReplaceItem(MergeBoardEnum.Main);
        
        if(EasterModel.Instance.IsCanClearEaster())
            MergeManager.Instance.CheckEasterReplaceItem(MergeBoardEnum.Main);
        
        if(DogHopeModel.Instance.IsCanClearDogCookie())
            DogHopeModel.Instance.RemoveAllDogCookies();
        
        if(ClimbTreeModel.Instance.IsCanClearBananas())
            ClimbTreeModel.Instance.RemoveAllClimbTreeBanana();
        
        if(ParrotModel.Instance.IsCanClearParrotItem())
            ParrotModel.Instance.RemoveAllParrotItem();
        
        if(FlowerFieldModel.Instance.IsCanClearFlowerFieldItem())
            FlowerFieldModel.Instance.RemoveAllFlowerFieldItem();
        
        UpdateBgGroup();
    }

    public enum MergeBoardThemeEnum
    {
        Default = 0,
        Mermaid = 1,
        RecoverCoin = 2,
        CoinLeaderBoard = 3,
        ThemeDecoration = 4,
    }

    private Dictionary<MergeBoardThemeEnum, MergeBoardTheme> _themeDic;
    public MergeBoardTheme GetMergeBoardTheme(MergeBoardThemeEnum themeEnum)
    {
        if (_themeDic == null)
            _themeDic = new Dictionary<MergeBoardThemeEnum, MergeBoardTheme>();
        if (!_themeDic.ContainsKey(themeEnum))
            _themeDic.Add(themeEnum,CreateMergeBoardTheme(themeEnum));
        return _themeDic[themeEnum];
    }
    public MergeBoardTheme CreateMergeBoardTheme(MergeBoardThemeEnum themeEnum)
    {
        switch (themeEnum)
        {
            case MergeBoardThemeEnum.Default:
            {
                _topBG.gameObject.SetActive(false);
                _taskBG.gameObject.SetActive(false);
                _boardBG.gameObject.SetActive(false);
                _boardMainBG.gameObject.SetActive(false);
                
                if (ABTest.ABTestManager.Instance.IsOpenNewUserMergeMainTest())
                {
                    return new MergeBoardTheme()
                    {
                        TopPath = () =>"Prefabs/Merge/Merge1BG"+(CommonUtils.IsLE_16_10()?"_Pad":""),
                        TaskPath = () =>"Prefabs/Merge/Merge1TaskListBG",
                        BoardPath=()=>"Prefabs/Merge/Merge1BoardBG",
                        MainPath=()=>"Prefabs/Merge/Merge1BoardMainBG",
                    };
                }
                else if (ABTest.ABTestManager.Instance.IsOpenOldUserMergeMainTest())
                {
                    return new MergeBoardTheme()
                    {
                        TopPath = () =>"Prefabs/Merge/Merge2BG"+(CommonUtils.IsLE_16_10()?"_Pad":""),
                        TaskPath = () =>"Prefabs/Merge/Merge2TaskListBG",
                        BoardPath=()=>"Prefabs/Merge/Merge2BoardBG",
                        MainPath=()=>"Prefabs/Merge/Merge2BoardMainBG",
                    };
                }
                else
                {
                    _topBG.gameObject.SetActive(true);
                    _taskBG.gameObject.SetActive(true);
                    _boardBG.gameObject.SetActive(true);
                    _boardMainBG.gameObject.SetActive(true);
                    
                    return new MergeBoardTheme()
                    {
                        _top = _topBG,
                        _task = _taskBG,
                        _board = _boardBG,
                        _main = _boardMainBG
                    };
                }
            }
            case MergeBoardThemeEnum.Mermaid:
            {
                return new MergeBoardTheme()
                {
                    TopPath = () =>"Prefabs/Activity/Mermaid/MermaidBG"+(CommonUtils.IsLE_16_10()?"_Pad":""),
                    TaskPath = () =>"Prefabs/Activity/Mermaid/MermaidTaskListBG",
                    BoardPath=()=>"Prefabs/Activity/Mermaid/MermaidBoardBG",
                    MainPath=()=>"Prefabs/Activity/Mermaid/MermaidBoardMainBG",
                };
            }
            case MergeBoardThemeEnum.RecoverCoin:
            {
                return new MergeBoardTheme()
                {
                    TopPath = () =>RecoverCoinModel.GetAssetPathWithSkinName("Prefabs/Activity/RecoverCoin/RecoverCoinBG"+(CommonUtils.IsLE_16_10()?"_Pad":"")),
                    TaskPath = () =>RecoverCoinModel.GetAssetPathWithSkinName("Prefabs/Activity/RecoverCoin/RecoverCoinTaskListBG"),
                    BoardPath=()=>RecoverCoinModel.GetAssetPathWithSkinName("Prefabs/Activity/RecoverCoin/RecoverCoinBoardBG"),
                    MainPath=()=>RecoverCoinModel.GetAssetPathWithSkinName("Prefabs/Activity/RecoverCoin/RecoverCoinBoardMainBG"),
                };
            }
            case MergeBoardThemeEnum.ThemeDecoration:
            {
                var storage = ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek;
                return new MergeBoardTheme()
                {
                    TopPath = () =>storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/ThemeDecorationBG"+(CommonUtils.IsLE_16_10()?"_Pad":"")),
                    TaskPath = () =>storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/ThemeDecorationTaskListBG"),
                    BoardPath=()=>storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/ThemeDecorationBoardBG"),
                    MainPath=()=>storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/ThemeDecorationBoardMainBG"),
                };   
            }
            case MergeBoardThemeEnum.CoinLeaderBoard:
            {
                return new MergeBoardTheme()
                {
                    TopPath = () =>"Prefabs/Activity/CoinLeaderBoard/CoinLeaderBoardBG"+(CommonUtils.IsLE_16_10()?"_Pad":""),
                    TaskPath = () =>"Prefabs/Activity/CoinLeaderBoard/CoinLeaderBoardTaskListBG",
                    BoardPath=()=>"Prefabs/Activity/CoinLeaderBoard/CoinLeaderBoardBoardBG",
                    MainPath=()=>"Prefabs/Activity/CoinLeaderBoard/CoinLeaderBoardBoardMainBG",
                };
            }
            default:
            {
                return new MergeBoardTheme()
                {
                    _top = _topBG,
                    _task = _taskBG,
                    _board = _boardBG,
                    _main = _boardMainBG
                };
            }
        }
    }
    private void UpdateBgGroupByTheme(MergeBoardTheme theme)
    {
        if (curTheme == theme)
            return;
        if (curTheme != null)
        {
            curTheme.Top.gameObject.SetActive(false);
            curTheme.Task.gameObject.SetActive(false);
            curTheme.Board.gameObject.SetActive(false);
            curTheme.Main.gameObject.SetActive(false);   
        }
        theme.BindMergeMainController(this);
        curTheme = theme;
        curTheme.Top.gameObject.SetActive(true);
        curTheme.Task.gameObject.SetActive(true);
        curTheme.Board.gameObject.SetActive(true);
        curTheme.Main.gameObject.SetActive(true);
    }

    public void UpdateBgGroupByTheme(MergeBoardThemeEnum themeEnum)
    {
        UpdateBgGroupByTheme(GetMergeBoardTheme(themeEnum));
    }
    private void UpdateBgGroup()
    {
        if (ThemeDecorationModel.Instance.IsStart() && ThemeDecorationModel.Instance.GlobalConfig.ReplaceMergeBoard)
        {
            UpdateBgGroupByTheme(MergeBoardThemeEnum.ThemeDecoration);
        }
        else if (MermaidModel.Instance.IsStart() )
        {
            UpdateBgGroupByTheme(MergeBoardThemeEnum.Mermaid);
        }
        else if (RecoverCoinModel.Instance.IsStart())
        {
            UpdateBgGroupByTheme(MergeBoardThemeEnum.RecoverCoin);
        }
        else if (CoinLeaderBoardModel.Instance.IsStart())
        {
            UpdateBgGroupByTheme(MergeBoardThemeEnum.CoinLeaderBoard);
        }
        else
        {
            UpdateBgGroupByTheme(MergeBoardThemeEnum.Default);
        }
    }

    private void OnDisable()
    {
        CancelInvoke("CheckBalloon");
    }

    private void CheckBalloon()
    {
        _balloon?.Check();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_DRAGIN, OnMergeBoardDragIn);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_POINTERUP, OnMergeBoardPointerUp);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TASK_REFRESH, RefreshTask);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TASK_COMPLETE_REFRESH, OnRefreshOnCompleteTask);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_REFRESH, RefreshMergeBoard);
    }

    void RefreshMergeBoard(BaseEvent e)
    {
        if(e == null || e.datas == null || e.datas.Length < 3)
            return;
        
        RefreshItemSource itemSource = RefreshItemSource.none;
        if (e.datas != null && e.datas.Length > 3)
        {
            itemSource = (RefreshItemSource)e.datas[3];
            if (itemSource != RefreshItemSource.product && itemSource != RefreshItemSource.timeProduct &&
                itemSource != RefreshItemSource.deathAdd && itemSource != RefreshItemSource.rewards)
                return;
        }

        if(!MergeManager.Instance.MergeBoardIsFull(MergeBoardEnum.Main))
            return;
        
        DragonPlus.GameBIManager.Instance.SendGameEvent(
            BiEventCooking.Types.GameEventType.GameEventCheckerboard1,
            data1:EnergyModel.Instance.EnergyNumber().ToString(),
            data2:UserData.Instance.GetRes(UserData.ResourceId.Diamond).ToString(),
            data3:UserData.Instance.GetRes(UserData.ResourceId.Coin).ToString());//棋盘满的时候发BI
        
        if(MergeManager.Instance.IsBagFull(MergeBoardEnum.Main))
            return;
        MergeGuideLogic.Instance.CheckMergePackageGuide();
        PlayBagAnim("loop");
    }
    
    void RefreshTask(BaseEvent e)
    {
        if ((MergeBoardEnum)e.datas[0] != MergeBoardEnum.Main)
            return;
        UpdateTaskRedPoint();
    }

    void OnClickBagBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MergeBoardFullGuideBag);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MergeBoardGuideBag);
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Bag))
            return;

        UIManager.Instance.OpenUI(UINameConst.UIPopupMergePackage);
    }

    private float clickRewardsTime = 0;
    

    private void OnRefreshOnCompleteTask(BaseEvent obj)
    {
        bool openBag = UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Bag);
        _mBagBtn.gameObject.SetActive(openBag);
    }
    void OnClickBackBtn()
    {
        BackBtnGuideArrow?.Hide();
        if (!GuideSubSystem.Instance.CanBackHome())
            return;
        DecoNode _node =  DecoManager.Instance.FindNode(101003);
        DecoNode _node2 =  DecoManager.Instance.FindNode(101002);

        if (_node!=null && !_node.IsOwned &&_node2.IsOwned &&GuideSubSystem.Instance.isFinished(235) && !GuideSubSystem.Instance.isFinished(260) )
        {
   
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Buy, _node.Id);
        }
        else
        {
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
        }

        if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventMiniGame.MINIGAME_BGM);
        }
    }

    void OnMergeBoardDragIn(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length < 1)
            return;

        OnMergeBoardDragIn((GameObject) e.datas[0]);
    }

    void OnMergeBoardDragIn(GameObject collision)
    {
        if (lastDraginGo == collision)
            //if (lastDraginGo == collision || HotelWorldGuideManager.Instance.IsInForceGuideDuration())
        {
            return;
        }

        if (MergeBoard.activeIndex < 0)
            return;

        MergeBoard.Grid grid =MergeMainController.Instance.MergeBoard.GetGridByIndex(MergeBoard.activeIndex);
        if (grid == null || grid.id < 0)
            return;

        if (!MergeManager.Instance.IsOpen(MergeBoard.activeIndex,MergeBoardEnum.Main)) // 棋盘为解锁的 
            return;

        if (collision == _mBagBtn.gameObject)
        {
            _mBagBtn.transform.localScale = Vector3.one * 1.2f;
            intoBagMergeObj.gameObject.SetActive(true);
        }
        else if (collision == MergeBoard.mContent.gameObject)
        {
            _mBagBtn.transform.localScale = Vector3.one;
            intoBagMergeObj.gameObject.SetActive(false);
        }
        else
        {
            _mBagBtn.transform.localScale = Vector3.one;
            intoBagMergeObj.gameObject.SetActive(false);
        }

        lastDraginGo = collision;
    }

    void OnMergeBoardPointerUp(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length < 1)
            return;

        OnMergeBoardPointerUp((GameObject) e.datas[0]);
    }

    void OnMergeBoardPointerUp(GameObject collision)
    {
        intoBagMergeObj.gameObject.SetActive(false);

        bool isOpen = UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Bag);
        if (!isOpen)
            return;

        if (collision != _mBagBtn.gameObject)
            return;

        // if(UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGuidePortrait) != null || GuideSubSystem.Instance.IsShowingGuide())
        //     return;

        if (MergeBoard.activeIndex < 0)
            return;

        MergeBoard.Grid grid = MergeMainController.Instance.MergeBoard.GetGridByIndex(MergeBoard.activeIndex);
        if (grid == null || grid.id < 0)
            return;

        if (!MergeManager.Instance.IsOpen(MergeBoard.activeIndex, MergeBoardEnum.Main)) // 棋盘为解锁的 
            return;

        StorageMergeItem board = MergeManager.Instance.GetBoardItem(MergeBoard.activeIndex, MergeBoardEnum.Main);
        if (board == null || board.Id < 0)
            return;

        var itemConfig = GameConfigManager.Instance.GetItemConfig(board.Id);
        ActiveCostType type = MergeManager.Instance.GetActiveCostType(itemConfig);
        //正在打开的 主动时间解锁物品不能放入背包
        if (type == ActiveCostType.time_inactive &&
            MergeManager.Instance.GetLeftActiveTime(MergeBoard.activeIndex, MergeBoardEnum.Main) > 0)
        {
            MergePromptManager.Instance.ShowBagFullTextPrompt(_mBagBtn.transform.position + new Vector3(0.5f, 0, 0),
                1.5f);
            return;
        }

        _mergeBoard.Grids[0]?.board?.SetMergeStatus(false);
        // 手动还原
        OnMergeBoardDragIn(MergeBoard.mContent.gameObject);

        if (itemConfig.isBuildingBag && MergeManager.Instance.GetBuildingBagCount(MergeBoardEnum.Main) < MergeManager.Instance.GetBuildingBagCapacity(MergeBoardEnum.Main)) //优先放入建材背包
        {
            StorageMergeItem item = board;
            MergeManager.Instance.PauseCd(MergeBoard.activeIndex,MergeBoardEnum.Main);
            MergeManager.Instance.AddBuildingBagItem(item,MergeBoardEnum.Main);
            MergeManager.Instance.RemoveBoardItem(MergeBoard.activeIndex, out item,"buildingBag", MergeBoardEnum.Main);

            intoBagEffectObj.gameObject.SetActive(false);
            intoBagEffectObj.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(WaitHideBagEffect());
            return;
        }

        if (MergeManager.Instance.GetBagCount(MergeBoardEnum.Main) < MergeManager.Instance.GetBagCapacity(MergeBoardEnum.Main))
        {
            StorageMergeItem item = board;
            MergeManager.Instance.PauseCd(MergeBoard.activeIndex,MergeBoardEnum.Main);
            MergeManager.Instance.AddBagItem(item,MergeBoardEnum.Main);
            MergeManager.Instance.RemoveBoardItem(MergeBoard.activeIndex, out item,"BagCapacity", MergeBoardEnum.Main);
            AudioManager.Instance.PlaySound(3);
            intoBagEffectObj.gameObject.SetActive(false);
            intoBagEffectObj.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(WaitHideBagEffect());
        }
        else
        {
            MergePromptManager.Instance.ShowBagTextPrompt(_mBagBtn.transform.position+new Vector3(0.3f,0,0), 1f);
        }
    }

    private IEnumerator WaitHideBagEffect()
    {
        yield return new WaitForSeconds(2f);
        intoBagEffectObj.gameObject.SetActive(false);
    }

    public void UpdateTaskRedPoint()
    {
        // int num = RoomManager.Instance.GetCanDecorationNodeNum();
        // taskRedPoint.SetActive(num > 0);
        // taskNumText.SetText(num.ToString());
        //
        // string stateName = num > 0 ? "loop" : "normal";
        // taskAnimator.Play(stateName);

        int num = 0;
        if (MainOrderManager.Instance.CompleteTaskNum < 10)
            num = 0;

        backNumText?.SetText(num.ToString());
        backRedPoint?.SetActive(num > 0);


        string stateName = "normal";
        if (num > 0)
            stateName = "appear01";

        backAnimator?.Play(stateName);
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

    public void SetGuideMaskActive(bool active)
    {
        guideMask.gameObject.SetActive(active);
    }

    public bool RewardItemIsShow()
    {
        if (!gameObject.activeSelf)
            return false;

        if (!MergeTaskTipsController.Instance.gameObject.activeSelf)
            return false;

        return true;
    }

    private void PlayBagAnim(string animName)
    {
        if(_bagAnimator == null)
            return;
        
        _bagAnimator.Play(animName);
    }
}