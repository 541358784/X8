using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
using DragonPlus.Config;
using DragonU3DSDK.Storage;


public class UIPopupBoxController : UIWindow
{
    private Slider _slider;
    private LocalizeTextMeshProUGUI _sliderText;
    private Button _playBtn;
    private Button _closeBtn;

    private LocalizeTextMeshProUGUI _popUpText;

    private bool IsUpdateRes;

    // 唤醒界面时调用(创建的时候加载一次)
    public override void PrivateAwake()
    {
        _slider = transform.Find("Root/MiddleGroup/Slider").GetComponent<Slider>();
        _sliderText = transform.Find("Root/MiddleGroup/Slider/Label").GetComponent<LocalizeTextMeshProUGUI>();

        _playBtn = transform.Find("Root/ButtonGroup/ButtonAds").GetComponent<Button>();
        _playBtn.onClick.AddListener(OnPlayButton);

        _closeBtn = transform.Find("Root/BgPopupBoandBig/ButtonClose").GetComponent<Button>();
        _closeBtn.onClick.AddListener(OnCloseButton);

        _popUpText = transform.Find("Root/MiddleGroup/TextHint").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private void OnCloseButton()
    {
        CloseWindowWithinUIMgr(true);
    }

    private void OnPlayButton()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        CloseWindowWithinUIMgr(true);
        SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.Game);
    }

    // 打开界面时调用(每次打开都调用)
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow();
        UpdateUI();
    }

    public void UpdateUI()
    {
        var storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        // var starChest=  GlobalConfigManager.Instance.GetTableStarChest(storageHome.StarBoxCount);
        //
        // _popUpText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_star_chest_desc", starChest.starNum.ToString()));
    }

    private void OnDestroy()
    {
    }
}