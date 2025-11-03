using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay.UI.EnergyTorrent;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupEnergyTorrentMainX8Controller : UIWindowController
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
        EnergyTorrentModel.Instance.SetOpenStateX8();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyFrenzyx8Pop);
        if(EnergyTorrentModel.Instance.IsOpenx8())
         UpdateAni();
    }



    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _offImage.gameObject.SetActive(!EnergyTorrentModel.Instance.IsOpenx8());
        currentIsOpen = EnergyTorrentModel.Instance.IsOpenx8();
    }

    public override void ClickUIMask()
    {
        OnCloseBtn();
    }
    private void UpdateAni()
    {
        if (EnergyTorrentModel.Instance.IsOpenx8())
        {
            _animator.Play("on");
        }
        else
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyFrenzyx8Open);
            _animator.Play("off");
        }
    }
    private void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
            if (! EnergyTorrentModel.Instance.IsOpenx8())
            {
                EnergyTorrentModel.Instance.SetCloseStateX8();
            }

            UIEnergyTorrentTipsController controller =
                UIManager.Instance.OpenUI(UINameConst.UIEnergyTorrentTips) as UIEnergyTorrentTipsController;
            string content = EnergyTorrentModel.Instance.IsOpenx8() ?
                string.Format(LocalizationManager.Instance.GetLocalizedString("ui_energy_frenzy_open_tips"),EnergyTorrentModel.Instance.GetMultiply())
                : "ui_energy_frenzy_close_tips";

            controller.PlayAnim(content, () =>
            {
                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1, () =>
                {
                    UIManager.Instance.CloseUI(UINameConst.UIEnergyTorrentTips);
                }));
            });
        });
    }

    private void OnOpenBtn()
    {
        EnergyTorrentModel.Instance.SetOpenStateX8();
        _offImage.gameObject.SetActive(!EnergyTorrentModel.Instance.IsOpenx8());
        UpdateAni();
    }
}