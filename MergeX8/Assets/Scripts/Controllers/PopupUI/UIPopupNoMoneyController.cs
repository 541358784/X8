using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupNoMoneyController : UIWindowController
{
    private Button ButtonOkButton { get; set; }
    private Button ButtonCloseButton { get; set; }
    private Animator _animator;
    private Image _costImage;
    private Transform role_coin;
    private Transform role_gem;
    private Transform role_seal;
    private Transform role_capybara;
    private Transform role_dolphin;
    private LocalizeTextMeshProUGUI _titleText;
    private LocalizeTextMeshProUGUI _descText;
    private const string _guideKey = "EnterMerge";
    public override void PrivateAwake()
    {
        ButtonOkButton = transform.Find("Root/WindowsGroup/ButtonStart").GetComponent<Button>();
        ButtonCloseButton = transform.Find("Root/WindowsGroup/ButtonClose").GetComponent<Button>();
        _animator = transform.GetComponent<Animator>();
        ButtonOkButton.onClick.AddListener(OnButtonOkButtonClick);
        ButtonCloseButton.onClick.AddListener(OnButtonCloseClick);
        _costImage = transform.Find("Root/WindowsGroup/MiddleGroup/CurrencyIcon").GetComponent<Image>();
        role_coin = transform.Find("Root/WindowsGroup/MiddleGroup/Role/Coin");
        role_gem = transform.Find("Root/WindowsGroup/MiddleGroup/Role/Gem");
        role_seal = transform.Find("Root/WindowsGroup/MiddleGroup/Role/Seal");
        role_dolphin= transform.Find("Root/WindowsGroup/MiddleGroup/Role/Dolphin");
        role_capybara= transform.Find("Root/WindowsGroup/MiddleGroup/Role/Capybara");
        _titleText = transform.Find("Root/WindowsGroup/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        _descText = transform.Find("Root/WindowsGroup/DecorationText").GetComponent<LocalizeTextMeshProUGUI>();
        
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Button, ButtonOkButton.transform as RectTransform, targetParam:_guideKey, topLayer:ButtonOkButton.transform);

    }

    private void OnButtonCloseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        int costId = (int)objs[0];

        _costImage.sprite = UserData.GetResourceIcon(costId, UserData.ResourceSubType.Big);
        role_coin.gameObject.SetActive(costId==(int) UserData.ResourceId.Coin || costId==(int) UserData.ResourceId.Mermaid);
        role_gem.gameObject.SetActive(costId==(int) UserData.ResourceId.RareDecoCoin);
        role_seal.gameObject.SetActive(costId==(int) UserData.ResourceId.Seal);
        role_dolphin.gameObject.SetActive(costId==(int) UserData.ResourceId.Dolphin);
        role_capybara.gameObject.SetActive(costId==(int) UserData.ResourceId.Capybara);
        
        if (costId == (int) UserData.ResourceId.RareDecoCoin)
        {
            _titleText.SetTerm("ui_tip_crystal_notenough_title");
            _descText.SetTerm("ui_tip_crystal_notenough_desc");
        }else if (costId == (int) UserData.ResourceId.Seal)
        {
            _titleText.SetTerm("ui_tip_seal_card_notenough_title");
            _descText.SetTerm("ui_tip_seal_card_notenough_desc");
        }
        else if (costId == (int) UserData.ResourceId.Dolphin)
        {
            _titleText.SetTerm("ui_tip_dolphin_card_notenough_title");
            _descText.SetTerm("ui_tip_dolphin_card_notenough_desc");
        }
        else if (costId == (int) UserData.ResourceId.Capybara)
        {
            _titleText.SetTerm("ui_tip_capybara_card_notenough_title");
            _descText.SetTerm("ui_tip_capybara_card_notenough_desc");
        }
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, _guideKey, _guideKey);
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnButtonCloseClick();
    }
    
    private void OnButtonOkButtonClick()
    {
        // if (LevelGroupSystem.Instance.IsReachLevelMax())
        // {
        //     CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
        //     {
        //         DescString = LocalizationManager.Instance.GetLocalizedString("Home_tx_room_0"),
        //     });
        //     return;
        // }

        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, _guideKey);
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
            {
                SceneFsm.mInstance.TransitionGame();
            }

            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIDailyRV)?.CloseWindowWithinUIMgr(true);
            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIStore)?.CloseWindowWithinUIMgr(true);
            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupTask)?.CloseWindowWithinUIMgr(true);
            var mermaidBuild = UIManager.Instance.GetOpenedUIByPath(UINameConst.MermaidMapBuild);
            if (mermaidBuild != null)
            {
                ((MermaidMapBuildController)mermaidBuild).EndPreview();
                mermaidBuild.CloseWindowWithinUIMgr(true);
            }
        }));
    }

    public static void ShowUI(int costId)
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupNoMoney, costId);
    }
}