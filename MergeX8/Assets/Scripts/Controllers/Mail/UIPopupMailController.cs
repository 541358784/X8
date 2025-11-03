using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using DragonPlus.Config;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Game;
using Gameplay;
using Screw;
using Screw.GameLogic;
using TMatch;
using AudioManager = DragonPlus.AudioManager;
using SfxNameConst = DragonPlus.SfxNameConst;

public class UIPopupMailController : UIWindowController
{
    private Button ButtonClaim;
    private LocalizeTextMeshProUGUI ButtonClaimText;
    private Text _textTitle;
    private Text _textInfo;
    private Animator _animator;
    private SystemMail _mail;
    private GameObject rewardObj;
    private Button ButtonClose;

    public override void PrivateAwake()
    {
        ButtonClaim = CommonUtils.Find<Button>(transform, "Root/envelope/WindowsGroup/ButtonsGroup/ReplayButton");
        ButtonClaimText =
            CommonUtils.Find<LocalizeTextMeshProUGUI>(transform, "Root/envelope/WindowsGroup/ButtonsGroup/ReplayButton/Text");

        ButtonClaim.onClick.AddListener(OnButtonClaim);
        ButtonClose = CommonUtils.Find<Button>(transform, "Root/envelope/WindowsGroup/CloseButton");
        ButtonClose.onClick.AddListener(OnClose);
        _textTitle = CommonUtils.Find<Text>(transform, "Root/envelope/WindowsGroup/MiddleGroup/TextTitle");
        _textInfo = CommonUtils.Find<Text>(transform, "Root/envelope/WindowsGroup/MiddleGroup/TextGroup/Viewport/Content/Text");
        _animator = transform.GetComponent<Animator>();
        rewardObj = transform.Find("Root/envelope/WindowsGroup/RewardGroup/Item0").gameObject;
        rewardObj.SetActive(false);
    }

    private void OnClose()
    {
        OnButtonClaim();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
    }

    public void SetData(SystemMail mail)
    {
        _mail = mail;
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMailPop,
        //     MailDataModel.Instance.GetSystemMailTitle(_mail)
        // );
        if (mail.Mail.Rewards == null || mail.Mail.Rewards.Count <= 0)
            ButtonClaimText.SetTerm("UI_button_read");
        _textTitle.text = (MailDataModel.Instance.GetSystemMailTitle(mail));
        _textInfo.text = (MailDataModel.Instance.GetSystemMailMessage(mail));
        for (int i = 0; i < mail.Mail.Rewards.Count; i++)
        {
            Transform m_rewardItem = Instantiate(rewardObj, rewardObj.transform.parent).transform;
            m_rewardItem.gameObject.SetActive(true);
            if (TMatchModel.Instance.IsTMatchResId((int) mail.Mail.Rewards[i].RewardId))
            {
                m_rewardItem.Find("Icon").GetComponent<Image>().sprite =
                    ItemModel.Instance.GetItemSprite(TMatchModel.Instance.ChangeToTMatchId((int) mail.Mail.Rewards[i].RewardId), false);
            }
            else if (ScrewGameModel.Instance.IsScrewResId((int) mail.Mail.Rewards[i].RewardId))
            {
                m_rewardItem.Find("Icon").GetComponent<Image>().sprite =
                    Screw.UserData.UserData.GetResourceIcon(ScrewGameModel.Instance.ChangeToScrewId((int) mail.Mail.Rewards[i].RewardId));
            }
            else if (UserData.Instance.IsResource((int) mail.Mail.Rewards[i].RewardId))
            {
                m_rewardItem.Find("Icon").GetComponent<Image>().sprite =
                    UserData.GetResourceIcon((UserData.ResourceId) mail.Mail.Rewards[i].RewardId,
                        UserData.ResourceSubType.Big);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig((int) mail.Mail.Rewards[i].RewardId);
                m_rewardItem.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }

            if ((UserData.ResourceId) mail.Mail.Rewards[i].RewardId == UserData.ResourceId.Infinity_Energy)
            {
                m_rewardItem.Find("Text").GetComponent<LocalizeTextMeshProUGUI>()
                    .SetText(TimeUtils.GetTimeString((int) mail.Mail.Rewards[i].RewardValue, true));
            }
            else
            {
                m_rewardItem.Find("Text").GetComponent<LocalizeTextMeshProUGUI>()
                    .SetText(mail.Mail.Rewards[i].RewardValue.ToString());
            }
        }
    }

    private void OnButtonClaim()
    {
        MailDataModel.Instance.ClaimMail(_mail.Mail.MailId, () =>
        {
            AudioManager.Instance.PlaySound(SfxNameConst.button_s);
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
                () => { CloseWindowWithinUIMgr(true); }));
        });
    }

    public static bool CanShowUI()
    {
        if (MailDataModel.Instance.HasNoReadMails())
        {
            Open(MailDataModel.Instance.GetAnyNoReadMail());
            return true;
        }

        return false;
    }

    public static void Open(SystemMail mail)
    {
        UIPopupMailController window =
            UIManager.Instance.OpenUI(UINameConst.UIPopupMail) as UIPopupMailController;
        window.SetData(mail);
    }
}