using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using Framework;
using Gameplay.UI.EnergyTorrent;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupEnergyTorrentMainController : UIWindowController
{
    private Button _button;
    private Image _offImage;
    private Button _buttonClose;
    private Animator _animator;
    private bool currentIsOpen;
    public override void PrivateAwake()
    {
        _button = GetItem<Button>("Root/Button");
        _offImage = GetItem<Image>("Root/Button/Off");
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _button.onClick.AddListener(OnOpenBtn);
        _animator = GetItem<Animator>("Root");
        var topLayer = new List<Transform>();
        topLayer.Add(_button.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.EnergyTorrent2, _button.transform as RectTransform, topLayer:topLayer);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.EnergyTorrent3, _button.transform as RectTransform);
        if(EnergyTorrentModel.Instance.IsOpen())
         UpdateAni();
    }



    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _offImage.gameObject.SetActive(!EnergyTorrentModel.Instance.IsOpen());
        currentIsOpen = EnergyTorrentModel.Instance.IsOpen();
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.EnergyTorrent2, null);
    }

    public override void ClickUIMask()
    {
        OnCloseBtn();
    }
    private void UpdateAni()
    {
        if (EnergyTorrentModel.Instance.IsOpen())
        {
            _animator.Play("on");
        }
        else
        {
            _animator.Play("off");
        }
    }
    private void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
          
            if (currentIsOpen != EnergyTorrentModel.Instance.IsOpen())
            {
                UIEnergyTorrentTipsController controller =
                    UIManager.Instance.OpenUI(UINameConst.UIEnergyTorrentTips) as UIEnergyTorrentTipsController;
                string content = EnergyTorrentModel.Instance.IsOpen() ?
                    string.Format(LocalizationManager.Instance.GetLocalizedString("ui_energy_frenzy_open_tips"),EnergyTorrentModel.Instance.GetMultiply())
                    : "ui_energy_frenzy_close_tips";

                controller.PlayAnim(content, () =>
                {
                    CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1, () =>
                    {
                        UIManager.Instance.CloseUI(UINameConst.UIEnergyTorrentTips);
                    }));
                });
            
            }
          
        });
    }

    private void OnOpenBtn()
    {
        EnergyTorrentModel.Instance.SetOpenState();
        _offImage.gameObject.SetActive(!EnergyTorrentModel.Instance.IsOpen());
        UpdateAni();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.EnergyTorrent2, null);
    }
}