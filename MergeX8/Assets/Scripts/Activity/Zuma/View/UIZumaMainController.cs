using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using FishEatFishSpace;
using Gameplay;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public partial class UIZumaMainController:UIWindowController
{
    public static UIZumaMainController Instance;
    public static UIZumaMainController Open(StorageZuma storageZuma)
    {
        if (!Instance || Instance.gameObject == null)
        {
            Instance = UIManager.Instance.OpenUI(UINameConst.UIZumaMain, storageZuma) as
                UIZumaMainController;
        }
        return Instance;
    }

    private ZumaModel Model => ZumaModel.Instance;
    private StorageZuma Storage;
    private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private Button HelpBtn;
    private ZumaGameController Game;
    private Button BombBtn;
    private Transform BombSelect;
    private Transform BombSelectEffect;
    private LocalizeTextMeshProUGUI BombCountText;
    private Button LightBtn;
    private Transform LightSelect;
    private Transform LightSelectEffect;
    private LocalizeTextMeshProUGUI LightCountText;
    private Button BallCountBtn;
    private LocalizeTextMeshProUGUI BallCountText;
    private ZumaClickArea ClickArea;
    public ZumaClickArea GuideClickArea;
    public Transform EffectLayer;
    public Transform EffectLayer2;
    public GameObjectPoolManager Pool;
    public Camera GameCamera;
    public override void PrivateAwake()
    {
        GameCamera = transform.Find("Root/GameRoot/GameCamera").GetComponent<Camera>();
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        HelpBtn = GetItem<Button>("Root/ButtonHelp");
        HelpBtn.onClick.AddListener(() =>
        {
            UIZumaHelpController.Open(Storage);
        });
        BombBtn = GetItem<Button>("Root/ButtonBomb");
        BombSelect = transform.Find("Root/ButtonBomb/Select");
        BombSelect.gameObject.SetActive(false);
        BombSelectEffect = transform.Find("Root/ButtonBomb/fx_ring_hint");
        BombSelectEffect.gameObject.SetActive(false);
        BombBtn.onClick.AddListener(() =>
        {
            Game.ClickBomb();
            UpdateBombViewState();
        });
        BombCountText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonBomb/RedPoint/Label");
        EventDispatcher.Instance.AddEvent<EventZumaBombCountChange>(OnBombCountChange);
        LightBtn = GetItem<Button>("Root/ButtonClolr");
        LightSelect = transform.Find("Root/ButtonClolr/Select");
        LightSelect.gameObject.SetActive(false);
        LightSelectEffect = transform.Find("Root/ButtonClolr/fx_ring_hint");
        LightSelectEffect.gameObject.SetActive(false);
        LightBtn.onClick.AddListener(() =>
        {
            Game.ClickLine();
            UpdateLightViewState();
        });
        LightCountText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonClolr/RedPoint/Label");
        EventDispatcher.Instance.AddEvent<EventZumaLineCountChange>(OnLightCountChange);
        BallCountBtn = GetItem<Button>("Root/Ball");
        BallCountBtn.onClick.AddListener(()=>UIPopupZumaNoDiceController.Open(Storage));
        BallCountText = GetItem<LocalizeTextMeshProUGUI>("Root/Ball/Text");
        EventDispatcher.Instance.AddEvent<EventZumaDiceCountChange>(OnBallCountChange);
        ClickArea = transform.Find("ClickArea").GetComponent<ZumaClickArea>();
        GuideClickArea = transform.Find("Root/GuideClickArea").GetComponent<ZumaClickArea>();
        GuideClickArea.gameObject.SetActive(false);
        EffectLayer = transform.Find("Root/EffectLayer");
        EffectLayer2 = transform.Find("Root/EffectLayer2");
        Pool = new GameObjectPoolManager("ZumaObjectPoolRoot");

        AwakeReward();
    }

    public void UpdateBombViewState()
    {
        BombSelect.gameObject.SetActive(Game.UseBomb);
        BombSelectEffect.gameObject.SetActive(Game.UseBomb);
    }
    public void UpdateLightViewState()
    {
        LightSelect.gameObject.SetActive(Game.UsingLine);
        LightSelectEffect.gameObject.SetActive(Game.UsingLine);
    }

    private int flySoundFrameCount = 0;
    public void PlayFlySound()
    {
        if (Time.frameCount-flySoundFrameCount == 0)
        {
            return;
        }
        flySoundFrameCount = Time.frameCount;
        AudioManager.Instance.PlaySoundById(193);
    }

    private List<Task> AddScoreTaskList = new List<Task>();
    public void PerformAddScore(Vector3 position)
    {
        var task = new TaskCompletionSource<bool>();
        AddScoreTaskList.Add(task.Task);
        var addScoreEffect1 = Pool.SpawnGameObject("Prefabs/Activity/Zuma/+100");
        addScoreEffect1.transform.DOKill();
        addScoreEffect1.transform.SetParent(EffectLayer2,false);
        addScoreEffect1.transform.position = position;
        addScoreEffect1.gameObject.SetActive(false);
        addScoreEffect1.gameObject.SetActive(true);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            PlayFlySound();
            var addScoreEffect2 = Pool.SpawnGameObject("Prefabs/Activity/Zuma/fx_trail_0");
            addScoreEffect2.transform.DOKill();
            addScoreEffect2.transform.SetParent(EffectLayer2,false);
            addScoreEffect2.transform.position = addScoreEffect1.transform.position;
            addScoreEffect2.gameObject.SetActive(false);
            addScoreEffect2.gameObject.SetActive(true);
            addScoreEffect1.transform.DOKill();
            Pool.RecycleGameObject(addScoreEffect1);
            var level = Model.GetLevel(Storage.LevelId);
            var target = _rewardRoot.transform;
            addScoreEffect2.transform.DOMove(target.position, 0.5f).OnComplete(() =>
            {
                CheckAddScoreGuide();
                TriggerWaitAddValue();
                // ShopGroup.TriggerWaitAddValue();
                addScoreEffect2.transform.DOKill();
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    addScoreEffect2.transform.DOKill();
                    Pool.RecycleGameObject(addScoreEffect2);
                }).SetTarget(addScoreEffect2.transform);
                task.SetResult(true);
                AddScoreTaskList.Remove(task.Task);
            });
        }).SetTarget(addScoreEffect1.transform);

    }

    public void CheckAddScoreGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaAddScore))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_winScoreText.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ZumaAddScore, _winScoreText.transform as RectTransform,topLayer:topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ZumaAddScore, null))
            {
                Game.IsGuide = true;
                GuideTriggerPosition.ZumaAddScore.WaitGuideFinish().AddCallBack(() =>
                {
                    Game.IsGuide = false;
                }).WrapErrors();
            }
        }
    }
    public void OnBombCountChange(EventZumaBombCountChange evt)
    {
        BombCountText.SetText(evt.TotalValue.ToString());
    }
    public void OnLightCountChange(EventZumaLineCountChange evt)
    {
        LightCountText.SetText(evt.TotalValue.ToString());
    }
    public void OnBallCountChange(EventZumaDiceCountChange evt)
    {
        BallCountText.SetText(evt.TotalValue.ToString());
    }

    private void OnDestroy()
    {
        XUtility.WaitSeconds(0.1f, () =>
        {
            AudioManager.Instance.PlayMusic(1, true);
        });
        Pool.Release();
        EventDispatcher.Instance.RemoveEvent<EventZumaBombCountChange>(OnBombCountChange);
        EventDispatcher.Instance.RemoveEvent<EventZumaLineCountChange>(OnLightCountChange);
        EventDispatcher.Instance.RemoveEvent<EventZumaDiceCountChange>(OnBallCountChange);
        
        OnRewardDestroy();
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        AudioManager.Instance.PlayMusic("bgm_zuma",true);
        Storage = objs[0] as StorageZuma;
        
        //InitShopEntrance();
        InitLeaderBoardEntrance();
        InitGiftBagEntrance();
        InitReward();
        BombCountText.SetText(Storage.BombCount.ToString());
        LightCountText.SetText(Storage.WildCount.ToString());
        BallCountText.SetText(Storage.BallCount.ToString());
        var curLevel = Model.GetLevel(Storage.LevelId);
        var newLevelAsset =
            ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/Zuma/" + curLevel.PrefabAsset);
        Game = Instantiate(newLevelAsset, transform.Find("Root/GameRoot"),false).gameObject.GetComponent<ZumaGameController>();
        Game.transform.localPosition = Vector3.zero;
        Game.StartGame(Storage,this);
        UpdateEnterOutEffect();
        UpdateLevelTextPosition();
        ClickArea.BindGame(Game);
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
    }

    private List<GameObject> EnterOutEffectList = new List<GameObject>();
    public void UpdateEnterOutEffect()
    {
        foreach (var effect in EnterOutEffectList)
        {
            Pool.RecycleGameObject(effect);
        }   
        EnterOutEffectList.Clear();
        for (var i = 0; i < Game.RoadList.Count; i++)
        {
            var road = Game.RoadList[i];
            var startLocalPosition = road.Points.First();
            var  startPosition = road.transform.TransformPoint(startLocalPosition.ToVector3());
            var endLocalPosition = road.Points.Last();
            var  endPosition = road.transform.TransformPoint(endLocalPosition.ToVector3());
            var startEffect = Pool.SpawnGameObject("Prefabs/Activity/Zuma/FX_Enter");
            startEffect.transform.SetParent(EffectLayer,false);
            startEffect.transform.position = startPosition;
            var endEffect = Pool.SpawnGameObject("Prefabs/Activity/Zuma/FX_Out");
            endEffect.transform.SetParent(EffectLayer,false);
            endEffect.transform.position = endPosition;
            EnterOutEffectList.Add(startEffect);
            EnterOutEffectList.Add(endEffect);
        }
    }
    public void UpdateTime()
    {
        if (Storage == null)
            return;
        TimeText.SetText(Storage.GetLeftTimeText());
    }
    
    public List<Action<Action>> PerformList = new List<Action<Action>>();
    public bool isPlaying = false;
    public bool IsPlaying() => isPlaying || Game.IsPlaying();

    public void PushPerformAction(Action<Action> performAction)
    {
        PerformList.Add(performAction);
        if (!isPlaying)
        {
            isPlaying = true;
            XUtility.WaitFrames(1, PlayPerform);
        }
    }
    public void PlayPerform()
    {
        if (PerformList.Count > 0)
        {
            var performAction = PerformList[0];
            PerformList.RemoveAt(0);
            performAction(PlayPerform);
        }
        else
        {
            isPlaying = false;
        }
    }

    public void UpdateLevelTextPosition()
    {
        var position = Game.transform.Find("LevelTextPosition").position;
        BallCountBtn.transform.position = position;
        
        UpdateRewardStatus();
    }

    public async void OnGameWin(int levelId)
    {
        Action<Action> scoreAddTaskAction = (callback) =>
        {
            if (AddScoreTaskList.Count > 0)
                Task.WhenAll(AddScoreTaskList).AddCallBack(callback).WrapErrors();
            else
                callback();
        };
        PushPerformAction(scoreAddTaskAction);
        var level = Model.GetLevel(levelId);
        if (!level.IsLoopLevel)
        {
            var normalLevel = level;
            var rewards = CommonUtils.FormatReward(normalLevel.RewardId, normalLevel.RewardNum);
            var bi = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ZumaGet
            };
            UserData.Instance.AddRes(rewards,bi);
            Action<Action> winScoreBoardEffectAction = async (callback) =>
            {
                var effect = transform.Find("Root/NumGroup/fx_num_hint");
                effect.DOKill();
                effect.gameObject.SetActive(false);
                effect.gameObject.SetActive(true);
                DOVirtual.DelayedCall(1f, () =>
                {
                    callback();
                }).SetTarget(effect);
                DOVirtual.DelayedCall(2f, () =>
                {
                    effect.gameObject.SetActive(false);
                }).SetTarget(effect);
            };
            PushPerformAction(winScoreBoardEffectAction);
            Action<Action> openBoxAction = (callback) =>
            {
                PerformFlyKey(level.Id).AddCallBack(callback).WrapErrors();
            };
            PushPerformAction(openBoxAction);
            Action<Action> popRewardAction = (callback) =>
            {
                CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                    false, bi,
                    callback,uiPath:normalLevel.Id == 10?UINameConst.UIPopupZumaReward:UINameConst.UIPopupZumaReward1);
            };
            PushPerformAction(popRewardAction);
        }
        Action<Action> startGameAction = (callback) =>
        {
            var curLevel = Model.GetLevel(Storage.LevelId);
            var lastGameUseBomb = Game?Game.UseBomb:false;
            var lastGameUseLight =  Game?Game.UsingLine:false;
            if (Game)
                DestroyImmediate(Game.gameObject);
            var newLevelAsset =
                ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/Zuma/" + curLevel.PrefabAsset);
            Game = Instantiate(newLevelAsset, transform.Find("Root/GameRoot"),false).gameObject.GetComponent<ZumaGameController>();
            Game.transform.localPosition = Vector3.zero;
            Game.StartGame(Storage,this);
            UpdateEnterOutEffect();
            UpdateLevelTextPosition();
            ClickArea.BindGame(Game);
            if (lastGameUseBomb)
                Game.ClickBomb();
            if (lastGameUseLight)
                Game.ClickLine();
            EventDispatcher.Instance.SendEventImmediately(new EventZumaNewLevel());
            BombSelect.gameObject.SetActive(Game.UseBomb);
            LightSelect.gameObject.SetActive(Game.UsingLine);
            BombSelectEffect.gameObject.SetActive(Game.UseBomb);
            LightSelectEffect.gameObject.SetActive(Game.UsingLine);
            callback();
        };
        PushPerformAction(startGameAction);
    }
}