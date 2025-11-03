using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using SnakeLadderLeaderBoard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public partial class UISnakeLadderMainController:UIWindowController
{
    public static bool IsAuto;
    public Button CloseBtn;
    private Button HelpBtn;
    private LocalizeTextMeshProUGUI TimeText;
    public SnakeLadderLevelConfig CurLevelConfig;
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
            if (IsAuto)
            {
                XUtility.WaitFrames(1, Roller.OnClickSpinBtn);
            }
        }
    }
    public void UpdateTimeText()
    {
        TimeText.SetText(SnakeLadderModel.Instance.GetActivityLeftTimeString());
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
        HelpBtn.onClick.AddListener(()=>UISnakeLadderHelpController.Open(Storage));
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTimeText",0f,1f);
        DefaultAddScore = GetItem<Transform>("Root/Anim");
        DefaultAddScore.gameObject.SetActive(false);
    }

    private StorageSnakeLadder Storage;
    public bool IsInit => Storage != null;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        AudioManager.Instance.PlayMusic("bgm_snake1",true);
        Storage = objs[0] as StorageSnakeLadder;
        CurLevelConfig = Storage.GetCurLevel();
        InitLeaderBoardEntrance();
        InitShopEntrance();
        InitCardNode();
        InitGameNode();
        InitRollerView();
        InitGetCardPopup();
        InitDefenseEffect();
        EventDispatcher.Instance.AddEvent<EventSnakeLadderLevelUp>(PerformLevelUp);
        EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,DealGuideFinish);
        SpinBtnGuide();
    }

    private void OnDestroy()
    {
        AudioManager.Instance.PlayMusic(1,true);
        ReleaseGetCardPopup();
        ReleaseDefenseEffect();
        EventDispatcher.Instance.RemoveEvent<EventSnakeLadderLevelUp>(PerformLevelUp);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,DealGuideFinish);
    }

    public void DealGuideFinish(BaseEvent evt)
    {
        var guideConfig = evt.datas[0] as TableGuide;
        if (guideConfig.triggerPosition == (int) GuideTriggerPosition.SnakeLadderExit)
        {
            SnakeLadderModel.Instance.AddTurntable(2,"GuideSend");
            Storage.TurntableRandomPool.Clear();
            Storage.TurntableRandomPool.Add(2);
        }
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

    public static UISnakeLadderMainController Open(StorageSnakeLadder storageSnakeLadder)
    {
        return UIManager.Instance.OpenUI(UINameConst.UISnakeLadderMain, storageSnakeLadder) as
            UISnakeLadderMainController;
    }
}