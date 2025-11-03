using System;
using System.Collections;
using System.Collections.Generic;
using ConnectLine.Model;
using Decoration;
using Difference;
using DigTrench;
using Ditch.Model;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.ABTest;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Farm.Order;
using Filthy.Game;
using FishEatFishSpace;
using Framework;
using Gameplay.UI.MiniGame;
using Makeover;
using OnePathSpace;
using Psychology;
using Psychology.Model;
using Screw.GameLogic;
using Screw.UserData;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using ABTestManager = ABTest.ABTestManager;
using AudioManager = DragonPlus.AudioManager;
using SfxNameConst = DragonPlus.SfxNameConst;
using Utils = Makeover.Utils;

public partial class UIHomeMainController : UIWindowController
{
    private StorageHome storageHome = null;

    public enum UIType
    {
        None,
        Home,
        Theme,
        Room,
    }

    public enum AuxUIType
    {
        None = -1,
        Top,
        Left,
        Right,
    }

    private Dictionary<UIType, IMainController> mainControllers = new Dictionary<UIType, IMainController>();
    private Dictionary<UIType, Animator> animators = new Dictionary<UIType, Animator>();
    private Dictionary<AuxUIType, IMainController> auxiliaryControllers = new Dictionary<AuxUIType, IMainController>();

    public static UIHomeMainController mainController = null;

    public UIType curUIType = UIType.None;

    private static Vector3 showPosition = Vector3.zero;
    private static Vector3 hidePosition = new Vector3(10000, 10000, 0);

    public Transform flyPos;

    public Vector3 FlyPos
    {
        get { return flyPos.localPosition; }
    }

    private GameObject rootGameObject = null;

    private Main_HomeController Main_homeController;

    private Button butPlay;
    public Button butScrew;
    public Button butTmatch;
    public Button butGame;
    public Button ButtonKeepPet;
    private Button butDecorate;
    public Transform _decoRedPoint;
    private GameObject _gameRedPoint;
    private GameObject _gameHand;

    private Transform _shopTranform;
    public Transform MainPlayTransform
    {
        get
        {
            if (FarmModel.Instance.IsFarmModel())
                return FarmModel.Instance.MainPlayTransform;
            
            return butPlay.transform;
        }
    }

    public Transform FarmTransform
    {
        get
        {
            return _farmButton.transform;
        }
    }
    
    private Vector3 _mainPlayPosition;
    public Vector3 MainPlayPosition
    {
        get { return _mainPlayPosition; }
    }
    
    public Transform ShopTransform
    {
        get { return _shopTranform; }
    }
    
    private Vector3 _gamePosition;
    public Vector3 GamePosition
    {
        get { return _gamePosition; }
    }
    
    public Transform GameTransform
    {
        get { return butGame.transform; }
    }

    public Transform DecorateTransform
    {
        get { return butDecorate.transform; }
    }
    
    private Button _farmButton;
    private GameObject _blueBlockObject;
    private GameObject _farmRedPoint;

    private string _guideKey;
    
    public override void PrivateAwake()
    {
        CommonUtils.NotchAdapte(transform.Find("Root"));

        Awake_MiniGame();
        
        mainController = this;
        isPlayDefaultAudio = false;
        rootGameObject = GetItem("Root");

        flyPos = GetItem<Transform>("Root/FlyPos");

        butPlay = GetItem<Button>("Root/BottomGroup/Middle/Play");
        butPlay.onClick.AddListener(() => { OnButtonPlay(); });
        _mainPlayPosition = butPlay.transform.position;
        
        _farmButton= GetItem<Button>("Root/BottomGroup/Middle/Farm");
        _farmButton.onClick.AddListener(() => { OnButtonFarm(); });
            
        _farmRedPoint = GetItem("Root/BottomGroup/Middle/Farm/Root/RedPoint");
        _farmRedPoint.gameObject.SetActive(false);
        
        butTmatch = GetItem<Button>("Root/BottomGroup/Middle/TMatch");
        butTmatch.onClick.AddListener(() => { OnButtonTMatch(); });
            
        butScrew = GetItem<Button>("Root/BottomGroup/Middle/Screw");
        butScrew.onClick.AddListener(() => { OnButtonScrew(); });
        var topScrewLayer = new List<Transform>();
        topScrewLayer.Add(butScrew.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.AsmrPlay, butScrew.transform as RectTransform, topLayer:topScrewLayer);
        
        butGame = GetItem<Button>("Root/BottomGroup/Middle/Game");
        butGame.onClick.AddListener(() => { OnButtonGame(); });
        butGame.gameObject.SetActive(Utils.IsOpen);
        _gamePosition = butGame.transform.position;
        butGame.transform.Find("Root/Icon_BlueBlock").gameObject.SetActive(Utils.IsNewUser());

        ButtonKeepPet = GetItem<Button>("Root/BottomGroup/Middle/KeepPet");
        ButtonKeepPet.gameObject.SetActive(true);
        ButtonKeepPet.gameObject.AddComponent<Aux_KeepPet>();

        _gameRedPoint =  GetItem("Root/BottomGroup/Middle/Game/Root/Image");
        _gameHand =  GetItem("Root/BottomGroup/Middle/Game/Root/Image/Hand");
        _gameHand.gameObject.SetActive(false);
        
        butDecorate = GetItem<Button>("Root/BottomGroup/Middle/Decoration");
        _decoRedPoint = GetItem<Transform>("Root/BottomGroup/Middle/Decoration/RedPoint");
        butDecorate.onClick.AddListener(OnButtonDecorate);

        _shopTranform = GetItem<Transform>("Root/Views/MainGroup/ButtonShop");
            
        InitMainControllers<Main_HomeController>("Root/Views/MainGroup", UIType.Home);
        InitMainControllers<MainDecorationController>("Root/Views/UIDecorationMain", UIType.Room);

        InitAuxiliaryController<MainAux_RightController>(AuxUIType.Right);
        InitAuxiliaryController<MainAux_LeftController>(AuxUIType.Left);
        InitAuxiliaryController<MainAux_TopController>(AuxUIType.Top);

        var playButton = transform.Find("Root/BottomGroup/Middle/Play");
        var topLayer = new List<Transform>();
        topLayer.Add(playButton);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.PlayButton, playButton as RectTransform, topLayer:topLayer);

        var topLayerGame = new List<Transform>();
        topLayerGame.Add(butGame.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Asmr, butGame.transform as RectTransform, topLayer:topLayerGame);
        
        _blueBlockObject = butGame.transform.Find("Root/Icon_BlueBlock").gameObject;
        
        var topLayerMatch = new List<Transform>();
        topLayerMatch.Add(butTmatch.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchPlay, butTmatch.transform as RectTransform, topLayer:topLayerMatch);
        
        var topLayerFarm = new List<Transform>();
        topLayerFarm.Add(_farmButton.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Farm_Enter, _farmButton.transform as RectTransform, topLayer:topLayerFarm);
        
        transform.Find("Root/BottomGroup/Middle/TMatch/TMWinPrizeLabelContainer").gameObject
            .AddComponent<TMWinPrizeLabelCreator>().gameObject.SetActive(true);
        
        InvokeRepeating("InvokeUpdate", 0, 1);
        TeamEntrance = transform.Find("Root/BottomGroup/Middle/Guild").gameObject.AddComponent<TeamHomeEntrance>();
        
        
        _guideKey = ($"Click_Task_{UnlockManager.GetUnlockParam(UnlockManager.MergeUnlockType.MiniGame_Deo)}");
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Button, butDecorate.transform as RectTransform, targetParam:_guideKey, topLayer:butDecorate.transform);

    }

    public TeamHomeEntrance TeamEntrance;
    private void OnButtonDecorate()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, _guideKey);
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        UIManager.Instance.OpenUI(UINameConst.UIPopupTask);
    }

    public void OnButtonPlay()
    {
        if (!GuideSubSystem.Instance.CanEnterGame())
            return;
        AudioManager.Instance.PlayMusic(1,true);
        // if (ExperenceModel.Instance.GetLevel() <= 1 && !DifferenceManager.Instance.IsDiffPlan_C() && DifferenceManager.Instance.IsDiffPlan_B())
        // {
        //   if(GuideSubSystem.Instance.isFinished(250) && !GuideSubSystem.Instance.isFinished(260))  
        //       return;
        // }
        
        UIRoot.Instance.EnableEventSystem = false;
        ShakeManager.Instance.ShakeSelection();
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        
        SceneFsm.mInstance.TransitionGame();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PlayButton);
        
        GuideSubSystem.Instance.ForceFinished(4420);
    }

    public void OnButtonTMatch()
    {
        UIRoot.Instance.EnableEventSystem = false;
        TMatchModel.Instance.Enter();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchPlay);
    }

    private void OnButtonFarm()
    {
        // SceneFsm.mInstance.ChangeState(StatusType.TileMatch, 1);
        //
        // return;
        if(!DecoManager.Instance.IsWorldReady)
            return;

        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.Farm_Enter))
        {
            StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.Initiative, "10001");
        }
        else
        {
            SceneFsm.mInstance.ChangeState(StatusType.EnterFarm);
        }
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Farm_Enter);
    }

    private void OnButtonScrew()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.AsmrPlay);
        SceneFsm.mInstance.EnterScrewHome();
    }

    private void OnButtonGame()
    {
        // if (StorageManager.Instance.GetStorage<StorageHome>().MakeOver.IsFinish)
        // {
        //     CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
        //     {
        //         DescString = LocalizationManager.Instance.GetLocalizedString("ui_asmr_pop_desc1"),
        //         HasCancelButton = false,
        //         HasCloseButton = false,
        //     });
        //     return;
        // }
        bool isAsmr = GuideSubSystem.Instance.CurrentConfig != null && GuideSubSystem.Instance.CurrentConfig.id == 10;
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Asmr);

        bool isNew = Utils.IsUseNewMiniGame();
        if (isAsmr)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameIcon);
            if (isNew)
            {
                switch (Utils.GetMiniGroup())
                {
                    case MiniGroup.Puzzle:
                    {
                        SceneFsm.mInstance.ChangeState(StatusType.EnterPsychology, PsychologyConfigManager.Instance._configs[0], true);
                        break;
                    }
                    case MiniGroup.DigTrench:
                    {
                        SceneFsm.mInstance.ChangeState(StatusType.DigTrench, DigTrenchConfigManager.Instance.DigTrenchLevelList[0], true);
                        break;
                    }
                }
                return;
            }
        }
        
        if (isNew)
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupMiniGame);
        }
        else
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupGameTabulation);
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (CommonUtils.IsNotch())
            {
                Transform group = GetItem("Root/BottomGroup").transform;
                group.transform.localPosition = new Vector3(group.transform.localPosition.x,
                    group.transform.localPosition.y + 34, group.transform.localPosition.z);
            }
        }

        AnimControlManager.Instance.InitAnimControl(AnimKey.Main_Group, GetItem("Root/Views/MainGroup"), true);
        AnimControlManager.Instance.InitAnimControl(AnimKey.Main_Bottom, GetItem("Root/BottomGroup"));
        UpdateRedPoint();
        EventDispatcher.Instance.AddEventListener(EventEnum.ASMR_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.AddEventListener(EventEnum.DIG_TRENCH_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.AddEventListener(EventEnum.FISH_EAT_FISH_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.AddEventListener(EventEnum.ONE_PATH_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.AddEventListener(EventEnum.CONNECT_LINE_REFRESH_REDPOINT, UpdateGameRedPoint);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateGameRedPoint(null);
    }

    private void OnDisable()
    {
    }

    public void UpdateRedPoint()
    {
        _decoRedPoint?.gameObject.SetActive(DecoManager.Instance.CanBuyOrGet() || UIPopupTaskController.IsShowWishingRedPoint());
        if (Makeover.Utils.IsOpen && DitchModel.Instance.Ios_Ditch_Plan_D())
        {
            if(_decoRedPoint.gameObject.activeSelf)
                return;
            
            _decoRedPoint?.gameObject.SetActive(DitchModel.Instance.HaveNoFinish());
        }
        
        UpdateMiniGame();
    }
    
    public void InvokeUpdate()
    {
        UpdateRedPoint();
        _blueBlockObject.SetActive(Utils.IsNewUser());

        bool isFarmUnLock = FarmModel.Instance.IsUnLock();
        _farmButton.gameObject.SetActive(isFarmUnLock);
        _farmRedPoint.gameObject.SetActive(FarmModel.Instance.HaveFinishProduct());

        if (DitchModel.Instance.Is_Ios_Plan_D)
        {
            butTmatch.gameObject.SetActive(false);   
            butScrew.gameObject.SetActive(false);
            butGame.gameObject.SetActive(false);
        }
        else if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.68"))
        {
            butTmatch.gameObject.SetActive(false);   
            butScrew.gameObject.SetActive(false);

#if UNITY_ANDROID || UNITY_EDITOR
            butGame.gameObject.SetActive(false);
#elif UNITY_IOS
            butGame.gameObject.SetActive(Utils.IsOpen);
#endif
        }
        else
        {
            if (Utils.IsOpen && FilthyGameLogic.Instance.IsOpenFilthy())
            {
                butTmatch.gameObject.SetActive(false);   
                butScrew.gameObject.SetActive(true);
                butGame.gameObject.SetActive(false);
            }
            else
            {
                butGame.gameObject.SetActive(Utils.IsOpen);
                butScrew.gameObject.SetActive(false);
            }
        }
        
        if(StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey("CloseTM"))
            butTmatch.gameObject.SetActive(false);
        else
        {
            butTmatch.gameObject.SetActive(TMatchModel.Instance.IsUnlock() && ABTestManager.Instance.IsOpenTMatch());
        }
    }

    private void UpdateGameRedPoint(BaseEvent e)
    {
        _gameRedPoint.gameObject.SetActive(false);
        _gameHand.gameObject.SetActive(false);
    }
    
    private void InitMainControllers<T>(string path, UIType type) where T : SuperMainController
    {
        GameObject uiObj = GetItem(path);

        T component = uiObj.AddComponent<T>();
        component.Init(this);
        mainControllers.Add(type, component);
    }


    private void InitAuxiliaryController<T>(AuxUIType type) where T : SuperMainController
    {
        T component = gameObject.AddComponent<T>();
        component.Init(this);

        auxiliaryControllers.Add(type, component);
    }

    public IMainController GetMainController(UIType type)
    {
        if (mainControllers.ContainsKey(type))
            return mainControllers[type];

        return null;
    }

    private void OnDestroy()
    {
        mainController = null;
     
        AnimControlManager.Instance.Destory();
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ASMR_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.DIG_TRENCH_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FISH_EAT_FISH_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ONE_PATH_REFRESH_REDPOINT, UpdateGameRedPoint);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.CONNECT_LINE_REFRESH_REDPOINT, UpdateGameRedPoint);
    }


    public void AnimShowMainUI(bool isShow, bool isImmediately = false, Action endCall = null)
    {
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, isShow, isImmediately);
        AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, isShow, isImmediately);
        AnimControlManager.Instance.AnimShow(AnimKey.Farm_Top, isShow, isImmediately);
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, isShow, isImmediately, endCall);
    }

    public bool IsMainUIShow()
    {
        return AnimControlManager.Instance.IsShow(AnimKey.Main_Group) && AnimControlManager.Instance.IsShow(AnimKey.Main_ResBar) && AnimControlManager.Instance.IsShow(AnimKey.Main_Bottom);
    }

    #region static function

    public static void ShowUI(bool isAnim = false, bool isImmediately = false, Action endCall = null)
    {
        if (mainController == null)
        {
            UIManager.Instance.OpenUI(UINameConst.UIMainHome);
            mainController.GetMainController(UIType.Home).Show();
        }

        UIRoot.Instance.EnableEventSystem = true;
        mainController.gameObject.transform.localPosition = showPosition;
        if (!isAnim)
        {
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true, true);
        }
        if (!isAnim)
            return;
        mainController.AnimShowMainUI(true, isImmediately, endCall);
        mainController.UpdateRedPoint();
    }
    
    public static void HideUI(bool isAnim = false, bool isImmediately = false, Action endCall = null)
    {
        if (mainController == null)
            return;

        if (!isAnim)
        {
            mainController.gameObject.transform.localPosition = hidePosition;
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false, true);
        }

        if (!isAnim)
            return;

        mainController.AnimShowMainUI(false, isImmediately, () =>
        {
            endCall?.Invoke();
        });
    }

    public static void UpdateAuixControllers()
    {
        if (mainController == null)
            return;

        foreach (var kv in mainController.auxiliaryControllers)
        {
            kv.Value.Show();
        }
    }

    public static IMainController GetAuxController(AuxUIType type)
    {
        if (mainController == null || !mainController.auxiliaryControllers.ContainsKey(type))
            return null;

        return mainController.auxiliaryControllers[type];
    }

    public static Transform GetMainTransform(UIType type)
    {
        if (mainController == null)
            return null;
        if (mainController.mainControllers.ContainsKey(type))
            return mainController.mainControllers[type].mainController.transform;

        return null;
    }

    public static Transform GetRootTransform()
    {
        if (mainController == null)
            return null;

        return mainController.rootGameObject.transform;
    }

    public static void PlayUnLockRoomEffect(float delay = 0)
    {
        if (mainController == null)
            return;

        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(delay, () =>
        {
            var prefabs = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Effects/vfx_Unlock_001");
            if (prefabs == null)
                return;

            var obj = GameObject.Instantiate(prefabs, mainController.rootGameObject.transform);
            if (obj == null)
                return;
            obj.transform.SetAsFirstSibling();

            obj.SetActive(true);
            GameObject.Destroy(obj, 5);
        }));
    }

    #endregion
   
}