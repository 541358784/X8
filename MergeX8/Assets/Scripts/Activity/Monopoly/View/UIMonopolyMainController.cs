using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public partial class UIMonopolyMainController:UIWindowController
{
    public static bool IsAuto;
    public Button CloseBtn;
    private Button HelpBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private Transform DefaultAddScore;
    public List<Action<Action>> PerformList = new List<Action<Action>>();
    public bool isPlaying = false;
    public bool IsPlaying() => isPlaying;

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
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyBet1) && IsBetUnlock)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(BetBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyBet1, BetBtn.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyBet1, null))
                {
                    GuideTriggerPosition.MonopolyBet1.WaitGuideFinish() .AddCallBack(PlayPerform).WrapErrors();
                    return;
                }
            }

            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyBet2) && IsBetUnlock)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(BetBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyBet2, BetBtn.transform as RectTransform,
                    topLayer: topLayer);

                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyBet2, null);
            }

            if (IsAuto)
            {
                XUtility.WaitFrames(1, Roller.OnClickSpinBtn);
            }
        }
    }

    public void UpdateTimeText()
    {
        TimeText.SetText(MonopolyModel.Instance.GetActivityLeftTimeString());
    }

    public void OnClickCloseBtn()
    {
        if (IsPlaying())
            return;
        AnimCloseWindow(() =>
        {
            // if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
            //     SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
            // {
            //     SceneFsm.mInstance.TransitionGame();
            // }
        });
    }
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        HelpBtn = GetItem<Button>("Root/ButtonHelp");
        HelpBtn.onClick.AddListener(()=>UIMonopolyHelpController.Open(Storage));
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTimeText",0f,1f);
        DefaultAddScore = GetItem<Transform>("Root/Anim");
        DefaultAddScore.gameObject.SetActive(false);
    }

    private StorageMonopoly Storage;
    public bool IsInit => Storage != null;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        AudioManager.Instance.PlayMusic("bgm_monopoly",true);
        Storage = objs[0] as StorageMonopoly;
        InitLeaderBoardEntrance();
        InitShopEntrance();
        InitCardNode();
        InitBetGroup();
        InitGameNode();
        InitRollerView();
        InitGetCardPopup();
        InitRewardBoxNode();
        InitBuyDiceNode();
        EventDispatcher.Instance.AddEvent<EventMonopolyLevelUp>(PerformLevelUp);
        EventDispatcher.Instance.AddEvent<EventMonopolyUIPlayMiniGame>(PerformPlayMiniGame);
        EventDispatcher.Instance.AddEvent<EventMonopolyUIPopBuyBlock>(PerformPopBuyBlock);
        EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,DealGuideFinish);
        SpinBtnGuide();
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
    }

    public void PerformPlayMiniGame(EventMonopolyUIPlayMiniGame evt)
    {
        Action<Action> performAction = (callback) =>
        {
            UIPopupMonopolyMiniGameController.Open(Storage, evt.MiniGameConfig,evt.BetValue);
            callback();
        };
        PushPerformAction(performAction);
    }

    public void OnClickBuyBlockBtn(MonopolyBlockConfig blockConfig)
    {
        if (isPlaying)
            return;
        if (!Storage.CanBuyBlock(blockConfig))
            return;
        Action<Action> performAction = (callback) =>
        {
            UIPopupMonopolyBuyBlockController.Open(Storage, blockConfig,callback);
        };
        PushPerformAction(performAction);
    }

    public void PerformPopBuyBlock(EventMonopolyUIPopBuyBlock evt)
    {
        if (!Storage.CanBuyBlock(evt.BlockConfig))
            return;
        Action<Action> performAction = (callback) =>
        {
            if (IsAuto)
            {
                callback();
                return;
            }
            if (!Storage.CanBuyBlock(evt.BlockConfig))
                callback();
            else if(!UserData.Instance.CanAford(UserData.ResourceId.Coin, Storage.GetBuyBlockPrice(evt.BlockConfig), out var needCount))
                callback();
            else if (!UIPopupMonopolyBuyBlockController.Open(Storage, evt.BlockConfig, () =>
                {
                    callback();
                    if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyContinue, null))
                    {
                    }
                }))
            {
                callback();
            }
        };
        PushPerformAction(performAction);
    }

    private void OnDestroy()
    {
        XUtility.WaitSeconds(0.1f, () =>
        {
            AudioManager.Instance.PlayMusic(1, true);
        });
        ReleaseGetCardPopup();
        EventDispatcher.Instance.RemoveEvent<EventMonopolyLevelUp>(PerformLevelUp);
        EventDispatcher.Instance.RemoveEvent<EventMonopolyUIPlayMiniGame>(PerformPlayMiniGame);
        EventDispatcher.Instance.RemoveEvent<EventMonopolyUIPopBuyBlock>(PerformPopBuyBlock);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,DealGuideFinish);
    }

    public void DealGuideFinish(BaseEvent evt)
    {
        // var guideConfig = evt.datas[0] as TableGuide;
        // if (guideConfig.triggerPosition == (int) GuideTriggerPosition.MonopolyExit)
        // {
        //     MonopolyModel.Instance.AddTurntable(2,"GuideSend");
        //     Storage.TurntableRandomPool.Clear();
        //     Storage.TurntableRandomPool.Add(2);
        // }
    }
    public void FlyCarrot(Vector3 startPos,int addValue,Action callback = null)
    {
        PopupAddScore(startPos, addValue);
        Transform target = ShopGroup.Icon;
        int count = addValue/5;
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

    public static UIMonopolyMainController Instance;
    public static UIMonopolyMainController Open(StorageMonopoly storageMonopoly)
    {
        Instance = UIManager.Instance.OpenUI(UINameConst.UIMonopolyMain, storageMonopoly) as
            UIMonopolyMainController;
        return Instance;
    }
}