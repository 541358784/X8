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
using DragonPlus;
using DragonU3DSDK.Config;
using TMPro;
using UnityEngine.UI;

public class LoadingTransitionController : UIWindowController
{
    public class Data : UIWindowData
    {
        public Func<float> progressGetter;
        public Action onShowAnimationFinished;

        public Data(Func<float> progressGetter, Action onShowAnimationFinished)
        {
            this.onShowAnimationFinished = onShowAnimationFinished;
        }
    }
    //public override UIWindowLayer WindowLayer => UIWindowLayer.Loading;

    private Animator _animator;
    private Func<float> _progressGetter;
    private LocalizeTextMeshProUGUI _loadingText;
    private Image _loadingImage;
    private Status _status;
    private bool _readyToHide;
    private Action _onHideAnimationFinish;

    #region 需要换色，换图

    private Image _bgImage;
    private Image _roundImage1;
    private Image _roundImage2;
    private Image _roundImage3;
    private Image _roundImage4;
    private Image _roundImage5;
    private Image _outFrameImage;
    private Image _innerFrameImage;
    private Image _textBgImage;

    #endregion

    enum Status
    {
        Showing,
        Loading,
        Hiding,
    }

    public enum UIType
    {
        Normal,
    }

    public enum LoadingType
    {
        Decoration,
        Cooking,
        Dress,
    }

    public class LoadingTransitionPara
    {
        public LoadingType LoadingType;
        public int MapId;
        public UIType uiType;
    }

    public static void Show(LoadingTransitionPara param, Func<float> progressGetter, Action onShowAnimationFinished)
    {
        LoadingTransitionController ctr = UIManager.Instance.OpenUI(UINameConst.LoadingTransition,
            new Data(progressGetter, onShowAnimationFinished)) as LoadingTransitionController;

        ctr.InitLoadingType(param);
    }

    public override void PrivateAwake()
    {
        _animator = this.transform.GetComponent<Animator>();
        _loadingText = this.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _loadingImage = transform.Find("Dish").GetComponent<Image>();

        _bgImage = GetComponent<Image>();
        _roundImage1 = transform.Find("RoundGroup/Round1").GetComponent<Image>();
        _roundImage2 = transform.Find("RoundGroup/Round2").GetComponent<Image>();
        _roundImage3 = transform.Find("RoundGroup/Round3").GetComponent<Image>();
        _roundImage4 = transform.Find("RoundGroup/Round4").GetComponent<Image>();
        _roundImage5 = transform.Find("RoundGroup/Round5").GetComponent<Image>();
        _textBgImage = transform.Find("TextMask").GetComponent<Image>();

        _outFrameImage = transform.Find("LabelGroup/Label1").GetComponent<Image>();
        _innerFrameImage = transform.Find("LabelGroup/Label2").GetComponent<Image>();
    }

    private void InitLoadingType(LoadingTransitionPara param)
    {
        // AudioLogicManager.Instance.StopAllMusic();

        if (param.LoadingType == LoadingType.Cooking)
        {
            setUIType(param.uiType);
            _loadingText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_loading_desc"));
            // var restaurantModel = ClientMgr.Instance.GetRestaurantModel(param.MapId);
            // _loadingText.SetTerm(restaurantModel.StaticData.MapName);
            // _loadingImage.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.LoadingAtlas, restaurantModel.StaticData.LoadingPic);

            // AudioLogicManager.Instance.PlaySound("sfx_ui_transitions_begin");
        }
    }

    private void setUIType(UIType uiType)
    {
        // var config = TableManager.Instance.GetTable<TableLoadingTransitionConfig>().Find(c => c.type == (int) uiType);
        // var color = Color.white;
        // if (ColorUtility.TryParseHtmlString(config.bgColor, out color))
        // {
        //     color.a = _bgImage.color.a;
        //     _bgImage.color = color;
        // }
        //
        // if (ColorUtility.TryParseHtmlString(config.mainColor, out color))
        // {
        //     color.a = _roundImage1.color.a;
        //     _roundImage1.color = color;
        //
        //     color.a = _roundImage2.color.a;
        //     _roundImage2.color = color;
        //
        //     color.a = _roundImage3.color.a;
        //     _roundImage3.color = color;
        //
        //     color.a = _roundImage4.color.a;
        //     _roundImage4.color = color;
        //
        //     color.a = _roundImage5.color.a;
        //     _roundImage5.color = color;
        // }
        //
        // if (ColorUtility.TryParseHtmlString(config.labelBgColor, out color))
        // {
        //     color.a = _textBgImage.color.a;
        //     _textBgImage.color = color;
        // }
        //
        // if (ColorUtility.TryParseHtmlString(config.textColor, out color))
        // {
        //     color.a = _loadingText.GetTmpText().color.a;
        //     _loadingText.GetTmpText().color = color;
        // }

        // _outFrameImage.sprite = ResourcesManager.Instance.GetSpriteVariant("LoadingAtlas", config.outFrame);
        // _innerFrameImage.sprite = ResourcesManager.Instance.GetSpriteVariant("LoadingAtlas", config.innerFrame);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var d = objs[0] as Data;

        _progressGetter = d.progressGetter;
        UIRoot.Instance.EnableTouch(false);
        _status = Status.Showing;
        _readyToHide = false;
        var animTime = CommonUtils.GetAnimTime(_animator, "LoadingTransition_Appear");
        TimeSheduler.Schedule(animTime, () =>
        {
            d.onShowAnimationFinished?.Invoke();
            _status = Status.Loading;
        });
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);

        UIRoot.Instance.EnableTouch(true);
    }

    public static void Hide(Action onAnimationFinished)
    {
        var loading =
            UIManager.Instance.GetOpenedUIByPath<LoadingTransitionController>(UINameConst.LoadingTransition);
        if (loading != null)
        {
            loading._onHideAnimationFinish = onAnimationFinished;
            loading._readyToHide = true;
        }
    }


    private void FixedUpdate()
    {
        if (_progressGetter != null)
        {
            var progress = _progressGetter.Invoke();
            _loadingText.GetTmpText().text = (progress * 100f).ToString("#0.0");
        }

        if (_status == Status.Loading && _readyToHide)
        {
            switchToHide();
        }
    }

    private void switchToHide()
    {
        // AudioLogicManager.Instance.PlaySound("sfx_ui_transitions_begin2");

        _status = Status.Hiding;
        _animator.Play("LoadingTransition_Disappear");
        var animTime = CommonUtils.GetAnimTime(_animator, "LoadingTransition_Disappear");

        TimeSheduler.Schedule(animTime, () =>
        {
            _onHideAnimationFinish?.Invoke();
            CloseWindowWithinUIMgr(true);
            // OpUtils.UnloadSpriteAtlas("LoadingAtlas");
        });
    }
}