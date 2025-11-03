using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupTaskController : UIWindowController
{
    private Button _close;
    private GameObject _content;
    private Animator _animator;
    private GameObject _comingSoon;
    
    private const string GuideKey = "Click_Deco";
    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();

        _comingSoon = transform.Find("Root/Task/comingSoon").gameObject;
        
        _close = GetItem<Button>("Root/ButtonClose");
        _close.onClick.AddListener(OnCloseClick);

        _content = GetItem("Root/Task/Scroll View/Viewport/Content");

        Awake_Deco();
        Awake_Lock();
        Awake_Day();
        Awake_Buttons();
        //Awake_Ditch();
        Awake_MiniGame();
        Awake_Wishing();
        
        //string param = "task_" + TaskModuleManager.Instance.CompleteTaskNum;
        
        if (ExperenceModel.Instance.GetLevel() >= UnlockManager.GetUnlockParam(UnlockManager.MergeUnlockType.MiniGame_Deo))
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, GuideKey, GuideKey);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GoDeco, "");
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GoDeco, ((int)UserData.ResourceId.RareDecoCoin).ToString(), ((int)UserData.ResourceId.RareDecoCoin).ToString());
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GoDeco, ((int)UserData.ResourceId.Seal).ToString(), ((int)UserData.ResourceId.Seal).ToString());
        
        EventDispatcher.Instance.AddEventListener(EventEnum.GET_DECORATION_REWARD, GetDecorationReward);
    }

    void Start()
    {
        InitDecoView();
        InitLockView();
        InitDayView();
        //InitDitch();
        InitMiniGameView();
        Init_Wishing();
        
        _comingSoon.gameObject.SetActive(_taskDecoCells.Count == 0 && _taskLockCells.Count == 0);
        
        if(GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame)
            OnClickButton(_buttonDatas[1]._root);
        else
            OnClickButton(_buttonDatas[0]._root);
    }

    private void OnCloseClick()
    {
        OnCloseView();
    }

    public void OnCloseView(Action onEnd = null)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            onEnd?.Invoke();
        }));
    }
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseClick();
    }

    public static void CloseView(Action onEnd)
    {
        var crl = UIManager.Instance.GetOpenedUIByPath<UIPopupTaskController>(UINameConst.UIPopupTask);
        if (crl == null)
        {
            onEnd?.Invoke();
            return;
        }

        crl.OnCloseView(onEnd);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GET_DECORATION_REWARD, GetDecorationReward);
    }
}