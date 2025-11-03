using System.Collections.Generic;
using DragonPlus;
using Gameplay.UI.Setting;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupTeamMemberInfoController : UIWindowController
{
    public static UIPopupTeamMemberInfoController Instance;
    public static UIPopupTeamMemberInfoController Open(PlayerInfoExtra extraData)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupTeamMemberInfo,extraData) as UIPopupTeamMemberInfoController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        _cardNormalText = GetItem<LocalizeTextMeshProUGUI>("Root/Activity2/Content/Card/NormalText");
        _cardSuperText = GetItem<LocalizeTextMeshProUGUI>("Root/Activity2/Content/Card/SuperText");
        _blindBoxText = GetItem<LocalizeTextMeshProUGUI>("Root/Activity2/Content/BlindBox/Text");
        
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }

    private PlayerInfoExtra ExtraData;
    private LocalizeTextMeshProUGUI _cardNormalText;
    private LocalizeTextMeshProUGUI _cardSuperText;
    private LocalizeTextMeshProUGUI _blindBoxText;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        ExtraData = objs[0] as PlayerInfoExtra;

        transform.Find("Root/PlayerGroup/Name/Text").GetComponent<Text>().text = ExtraData.UserName;
        
        transform.Find("Root/PlayerGroup/HeadGroup/BG").gameObject.SetActive(false);
        var avatarState = new AvatarViewState(ExtraData.AvatarIcon, ExtraData.AvatarFrameIcon, ExtraData.UserName, false);
        var _headIconRoot = transform.Find("Root/PlayerGroup/HeadGroup/Head") as RectTransform;
        HeadIconNode.BuildHeadIconNode(_headIconRoot,avatarState);
        
        _cardNormalText.SetText(ExtraData.CardCollectStateNormal);
        _cardSuperText.SetText(ExtraData.CardCollectStateGolden);
        _blindBoxText.SetText(ExtraData.BlindBoxCollectState);
        
        var _vipIcons = new List<Transform>();
        for (int i = 0; i <= 5; i++)
        {
            _vipIcons.Add(transform.Find("Root/PlayerGroup/VIP/Icon/"+i));
        }
        for (var i = 0; i < _vipIcons.Count; i++)
        {
            _vipIcons[i].gameObject.SetActive(i == ExtraData.VipLevel);
        }
        
        var _vipText = GetItem<LocalizeTextMeshProUGUI>("Root/PlayerGroup/VIP/Text");
        _vipText.SetText(LocalizationManager.Instance.GetLocalizedString("ui_battlepass_vip")+ExtraData.VipLevel);
        
        var _vipButton = GetItem<Button>("Root/PlayerGroup/VIP");
        _vipButton.interactable = false;
        transform.Find("Root/PlayerGroup/VIP/Button").gameObject.SetActive(false);
        // _vipButton.onClick.AddListener(() =>
        // {
        //     AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        //     StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
        //         () =>
        //         {
        //             CloseWindowWithinUIMgr(true);
        //             UIPopupSetVipController.Open();
        //         }));
        // });

        transform.Find("Root/PlayerGroup/Slider/Star/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ExtraData.Level.ToString());
        Awake_Activity();
    }
    
}