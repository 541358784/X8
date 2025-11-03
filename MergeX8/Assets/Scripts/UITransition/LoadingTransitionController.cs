using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;
using DG.Tweening;
using DragonU3DSDK;
using Framework;
using DragonU3DSDK.Asset;
using System.IO;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Config;
using TMPro;
using UnityEngine.UI;

public class UILoadingTransitionController : UIWindowController
{
    private Animator _animator;
    private Status _status;
    private bool _readyToHide;

    private Action _onShowAnimationFinished;
    private Action _onHideAnimationFinish;
    enum Status
    {
        Showing,
        Loading,
        Hiding,
    }

    public static void Show(Action onShowAnimationFinished)
    {
        UIManager.Instance.OpenUI(UINameConst.UILoadingTransition, onShowAnimationFinished);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        if (objs != null && objs.Length > 0)
            _onShowAnimationFinished = (Action)objs[0];
        
        _animator = this.transform.GetComponent<Animator>();

        UIRoot.Instance.EnableEventSystem = false;
        
        _status = Status.Showing;
        _readyToHide = false;
        var animTime = CommonUtils.GetAnimTime(_animator, "LoadingTransition_Appear");
        TimeSheduler.Schedule(animTime, () =>
        {
            _status = Status.Loading;
            _onShowAnimationFinished?.Invoke();
        });
    }
    

    public static void Hide(Action onAnimationFinished)
    {
        var loading = UIManager.Instance.GetOpenedUIByPath<UILoadingTransitionController>(UINameConst.UILoadingTransition);
        if (loading != null)
        {
            loading._onHideAnimationFinish = onAnimationFinished;
            loading._readyToHide = true;
        }
    }

    public void Update()
    {
        if (_status == Status.Loading && _readyToHide)
        {
            switchToHide();
        }
    }
    
    private void switchToHide()
    {
        _status = Status.Hiding;
        _animator.Play("LoadingTransition_Disappear");
        var animTime = CommonUtils.GetAnimTime(_animator, "LoadingTransition_Disappear");

        TimeSheduler.Schedule(animTime, () =>
        {
            UIRoot.Instance.EnableEventSystem = true;
            
            CloseWindowWithinUIMgr(true);

            _onHideAnimationFinish?.Invoke();
        });
    }

    public override void PrivateAwake()
    {
    }
}