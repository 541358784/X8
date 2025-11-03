using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Easter2024LeaderBoard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public partial class UIEaster2024MainController:UIWindowController
{
    private DropBallGame Game;
    private Button PlayBtn;
    public Button CloseBtn;
    private Button HelpBtn;
    private LocalizeTextMeshProUGUI TimeText;
    public List<Transform> CardSelectionList = new List<Transform>();
    public Transform LuckyPointGroup;
    public LocalizeTextMeshProUGUI BallCountText;
    public Slider LevelSlider;
    // public LocalizeTextMeshProUGUI LevelSliderText;
    // public LocalizeTextMeshProUGUI LevelSliderMaxText;
    public Easter2024LevelConfig CurLevelConfig;
    public bool IsAuto;
    public Button AutoBtn;
    private Transform AutoFlagTrans;
    private Transform AutoDisableTrans;
    public List<Transform> ResultTransList = new List<Transform>();
    private int MaxLevelScore => CurLevelConfig.GetLevelMaxScore();
    private float ShowTotalScoreF = 0;
    private int ShowTotalScore => (int)ShowTotalScoreF;
    private int ShowLevelScore => Math.Min(ShowTotalScore - CurLevelConfig.ScoreLimit,MaxLevelScore);
    private float SliderUpdateStep => (MaxLevelScore - CurLevelConfig.ScoreLimit) / 30f;
    private Transform DefaultCard;
    public Button RecycleFreezingBallBtn;
    private Button PlayBtn2;
    private Transform DefaultAddScore;
    public void UpdateTimeText()
    {
        TimeText.SetText(Easter2024Model.Instance.GetActivityLeftTimeString());
    }

    public override void UpdateCanvasSortOrder()
    {
        base.UpdateCanvasSortOrder();
        InitSortingLayer();
    }

    public override void PrivateAwake()
    {
        CommonUtils.NotchAdapte(transform.Find("Root") as RectTransform);
        PlayBtn = GetItem<Button>("Root/DropRoot/Button");
        PlayBtn.onClick.AddListener(OnClickPlayBtn);
        PlayBtn2 = GetItem<Button>("Root/ButtonPlay");
        PlayBtn2.onClick.AddListener(OnClickPlayBtn);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        HelpBtn = GetItem<Button>("Root/ButtonHelp");
        HelpBtn.onClick.AddListener(()=>UIEaster2024HelpController.Open(Storage));
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTimeText",0f,1f);
        Game = GetItem("Root/DropRoot/DropBall").AddComponent<DropBallGame>();
        for (var i = 0; i < 4; i++)
        {
            CardSelectionList.Add(transform.Find("Root/CardSlot /"+(i+1)));
        }
        LuckyPointGroup = GetItem("Root/Luck").transform;
        BallCountText = GetItem<LocalizeTextMeshProUGUI>("Root/Num/Text");
        LevelSlider = GetItem<Slider>("Root/Slider");
        // LevelSliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        // LevelSliderMaxText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/MaxText");
        AutoBtn = GetItem<Button>("Root/Button");
        AutoBtn.onClick.AddListener(OnClickAutoBtn);
        AutoFlagTrans = GetItem<Transform>("Root/Button/Full");
        AutoDisableTrans = GetItem<Transform>("Root/Button/X");
        for (var i = 0; i < 7; i++)
        {
            var resultTrans = GetItem<Transform>("Root/DropRoot/RewardGroup/"+(i+1));
            ResultTransList.Add(resultTrans);
        }
        DefaultCard = GetItem<Transform>("Root/Card");
        DefaultCard.gameObject.SetActive(false);
        RecycleFreezingBallBtn = GetItem<Button>("Root/ButtonReset");
        DefaultAddScore = GetItem<Transform>("Root/Anim");
        DefaultAddScore.gameObject.SetActive(false);
    }

    private StorageEaster2024 Storage;
    public bool IsInit => Storage != null;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        AudioManager.Instance.PlayMusic("bgm_doughnut",true);
        Storage = objs[0] as StorageEaster2024;
        CurLevelConfig = Storage.GetCurLevel();
        Game.Init(Storage,this);
        ShowTotalScoreF = Storage.TotalScore;
        UpdateSliderUI();
        IsAuto = false;
        UpdateAutoBtnUI();
        UpdateAutoBtnEnable();
        InitLeaderBoardEntrance();
        InitShopEntrance();
        InitSortingLayer();

        XUtility.WaitFrames(2, () =>
        {
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024MainGuideDropBall))
            {
                // List<Transform> topLayer = new List<Transform>();
                // topLayer.Add(PlayBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024MainGuideDropBall,
                    PlayBtn.transform as RectTransform);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024MainGuideDropBall, null);
            }
        });
    }

    private void OnDestroy()
    {
        AudioManager.Instance.PlayMusic(1,true);
    }

    public void InitSortingLayer()
    {
        var sortingLayerId = canvas.sortingLayerID;
        var sortingOrder = canvas.sortingOrder;
        {
            var sortingGroup = Game.gameObject.GetComponent<SortingGroup>();
            sortingGroup.enabled = true;
            sortingGroup.sortingLayerID = sortingLayerId;
            sortingGroup.sortingOrder = sortingOrder + 2;
        }
        {
            var rewardGroup = GetItem<Transform>("Root/DropRoot/RewardGroup");
            var sortingGroup = rewardGroup.gameObject.GetComponent<SortingGroup>();
            if (sortingGroup == null)
                sortingGroup = rewardGroup.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerID = sortingLayerId;
            sortingGroup.sortingOrder = sortingOrder + 1;
        }
        {
            
            var sortingGroup = DefaultCard.gameObject.GetComponent<Canvas>();
            if (sortingGroup == null)
                sortingGroup = DefaultCard.gameObject.AddComponent<Canvas>();
            sortingGroup.overrideSorting = true;
            sortingGroup.sortingLayerID = sortingLayerId;
            sortingGroup.sortingOrder = sortingOrder + 3;
            if (DefaultCard.gameObject.GetComponent<GraphicRaycaster>() == null)
                DefaultCard.gameObject.AddComponent<GraphicRaycaster>();
        }
        {
            var sortingGroup = DefaultAddScore.gameObject.GetComponent<Canvas>();
            if (sortingGroup == null)
                sortingGroup = DefaultAddScore.gameObject.AddComponent<Canvas>();
            sortingGroup.overrideSorting = true;
            sortingGroup.sortingLayerID = sortingLayerId;
            sortingGroup.sortingOrder = sortingOrder + 4;
            if (DefaultAddScore.gameObject.GetComponent<GraphicRaycaster>() == null)
                DefaultAddScore.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    public void UpdatePlayBtnUI()
    {
        var unlock = !Game.IsInWaiting;
    }
    public void UpdateAutoBtnUI()
    {
        AutoFlagTrans.gameObject.SetActive(IsAuto);
        PlayBtn2.interactable = !IsAuto;
    }

    public void UpdateAutoBtnEnable()
    {
        AutoBtn.interactable = !Game.IsSelectCard;
        AutoDisableTrans.gameObject.SetActive(Game.IsSelectCard);
    }

    public void UpdateSliderUI()
    {
        if (!LevelSlider)
            return;
        if (MaxLevelScore < 0)
        {
            LevelSlider.value = 1f;
            // LevelSliderText.SetText(CurLevelConfig.ScoreLimit.ToString());
            // LevelSliderMaxText.SetText(CurLevelConfig.ScoreLimit.ToString());
        }
        else
        {
            LevelSlider.value = (float)ShowLevelScore/MaxLevelScore;
            // LevelSliderText.SetText(ShowLevelScore.ToString());
            // LevelSliderMaxText.SetText(MaxLevelScore.ToString());
        }
    }

    public void CheckSliderUpdate()
    {
        if (MaxLevelScore < 0)
            return;
        if (ShowLevelScore >= MaxLevelScore && !IsPerformLevelUp)
        {
            CurLevelConfig = CurLevelConfig.GetNextLevel();
            PerformLevelUp();
        }
        else if (Storage.TotalScore > ShowTotalScore && !IsPerformLevelUp)
        {
            var addValue = Math.Min(SliderUpdateStep, Storage.TotalScore - ShowTotalScoreF);
            ShowTotalScoreF += addValue;
            if (!LevelSlider)
                return;
            LevelSlider.value = (float)ShowLevelScore/MaxLevelScore;
            // LevelSliderText.SetText(ShowLevelScore.ToString());
            // LevelSliderMaxText.SetText(MaxLevelScore.ToString());
        }
    }

    private bool IsPerformLevelUp = false;
    public async void PerformLevelUp()
    {
        IsPerformLevelUp = true;
        await XUtility.WaitSeconds(0.4f);
        Game.UpdateRewardText(true);
        await XUtility.WaitSeconds(1.5f);
        if (!this)
            return;
        UpdateSliderUI();
        IsPerformLevelUp = false;
    }

    private void Update()
    {
        if (!IsInit)
            return;
        CheckSliderUpdate();
    }

    public void OnClickPlayBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024MainGuideDropBall);
        if (IsAuto)
        {
            OnClickAutoBtn();
            return;
        }
        Game.OnClickPlayBtn();
    }
    public void OnClickCloseBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024MainGuideClose);
        if (Game.IsPlaying())
            return;
        AnimCloseWindow();
    }

    public void OnClickAutoBtn()
    {
        if (Game.IsSelectCard)
            return;
        SetAutoState(!IsAuto);
    }

    public void SetAutoState(bool state)
    {
        if (IsAuto != state)
        {
            IsAuto = state;
            UpdateAutoBtnUI();
        }
    }

    public void FlyCarrot(Vector3 startPos,int addValue,Action callback = null)
    {
        PopupAddScore(startPos, addValue);
        Transform target = ShopGroup.Icon;
        int count = addValue/100;
        if (count == 0)
            count = 1;
        if (count > 10)
            count = 10;
        float delayTime = 0.05f;
        var triggerAddValue = false;
        for (int i = 0; i < count; i++)
        {
            int index = i;

            Vector3 position = target.position;

            FlyGameObjectManager.Instance.FlyObject(target.gameObject, startPos, position, true, 0.5f,
                delayTime * i, () =>
                {
                    if (!triggerAddValue)
                    {
                        triggerAddValue = true;
                        ShopGroup.TriggerWaitAddValue();
                    }
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                    ShakeManager.Instance.ShakeLight();
                    if (index == count - 1)
                    {
                        callback?.Invoke();
                    }
                });
        }
    }

    public static UIEaster2024MainController Open(StorageEaster2024 storageEaster2024)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIEaster2024Main, storageEaster2024) as
            UIEaster2024MainController;
    }
}