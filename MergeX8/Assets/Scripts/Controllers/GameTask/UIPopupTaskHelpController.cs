using System;
using System.Collections;
using System.Collections.Generic;
using Deco.Area;
using Deco.Node;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public  class UIPopupTaskHelpController : UIWindowController
{
    private Button _close;
    private Button _show;
    
    private Animator _animator;
    private Image _imageBg;

    private LocalizeTextMeshProUGUI _contentText;
    private LocalizeTextMeshProUGUI _titleText;

    private DecoArea _decoArea;
    private List<DecoNode> _decoNodes;
    
    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        
        _close = GetItem<Button>("Root/CloseButton");
        _close.onClick.AddListener(OnCloseClick);

        _show = GetItem<Button>("Root/Button");
        _show.onClick.AddListener(OnShowClick);
        
        _contentText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        _titleText = GetItem<LocalizeTextMeshProUGUI>("Root/BGGroup/TitleText");
        
        _imageBg = GetItem<Image>("Root/BGGroup/Image");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _decoArea = (DecoArea)objs[0];
        _decoNodes = (List<DecoNode>)objs[1];
        
        if(_decoArea == null)
            return;
        
        _contentText.SetTerm(_decoArea._data._config.areaDesc);
        _titleText.SetTerm(_decoArea._data._config.areaName);

        _imageBg.sprite = CommonUtils.LoadAreaIconSprite(_decoArea._data._config.icon);
    }

    private void OnCloseClick()
    {
        OnCloseView(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupTask);
        });
    }

    private void OnShowClick()
    {
        OnCloseView(() =>
        {
            if(_decoNodes == null || _decoNodes.Count == 0)
                return;
            
            if (AssetCheckManager.Instance.GetAreaResNeedToDownload(_decoNodes[0]._stage.Area.Id).Count > 0)
            {
                CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                {
                    DescString = LocalizationManager.Instance.GetLocalizedString("UI_download_resources_tip"),
                    HasCancelButton = false,
                    HasCloseButton = false,
                });
                return;
            }
            
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Buy, _decoNodes[0].Id);
            }
            else
            {
                DecoManager.Instance.SelectNode(_decoNodes[0]);
            }
        });
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
}