using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using System.Collections.Generic;
using DragonPlus.Config;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Game;
using Gameplay;
using TMatch;
using AudioManager = DragonPlus.AudioManager;
using SfxNameConst = DragonPlus.SfxNameConst;

public class UIPopupKeepPetReturnController : UIWindowController
{
    private Button ButtonClaim;
    private LocalizeTextMeshProUGUI ButtonClaimText;
    private LocalizeTextMeshProUGUI TitleText;
    private Text _textTitle;
    private Text _textInfo;
    private Animator _animator;
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
        // _textTitle.text = LocalizationManager.Instance.GetLocalizedString("ui_dog_update_title");
        _textTitle.gameObject.SetActive(false);
        _textInfo.text = LocalizationManager.Instance.GetLocalizedString("ui_dog_update_desc");
        TitleText = transform.Find("Root/envelope/WindowsGroup/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        TitleText.SetTerm("ui_dog_update_title");
    }

    private void OnClose()
    {
        OnButtonClaim();
    }
    List<ResData> Rewards = new List<ResData>
    {
        new ResData((int) UserData.ResourceId.Energy, 100)
    };
    protected override void OnOpenWindow(params object[] objs)
    {
        var rewards = Rewards;
        for (int i = 0; i < rewards.Count; i++)
        {
            Transform m_rewardItem = Instantiate(rewardObj, rewardObj.transform.parent).transform;
            m_rewardItem.gameObject.SetActive(true);
            if (TMatchModel.Instance.IsTMatchResId((int) rewards[i].id))
            {
                m_rewardItem.Find("Icon").GetComponent<Image>().sprite =
                    ItemModel.Instance.GetItemSprite(TMatchModel.Instance.ChangeToTMatchId((int) rewards[i].id), false);
            }
            else if (UserData.Instance.IsResource((int) rewards[i].id))
            {
                m_rewardItem.Find("Icon").GetComponent<Image>().sprite =
                    UserData.GetResourceIcon((UserData.ResourceId) rewards[i].id,
                        UserData.ResourceSubType.Big);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig((int) rewards[i].id);
                m_rewardItem.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }

            if ((UserData.ResourceId) rewards[i].id == UserData.ResourceId.Infinity_Energy)
            {
                m_rewardItem.Find("Text").GetComponent<LocalizeTextMeshProUGUI>()
                    .SetText(TimeUtils.GetTimeString((int) rewards[i].count, true));
            }
            else
            {
                m_rewardItem.Find("Text").GetComponent<LocalizeTextMeshProUGUI>()
                    .SetText(rewards[i].count.ToString());
            }
        }
    }

    private void OnButtonClaim()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        CommonRewardManager.Instance.PopCommonReward(Rewards, CurrencyGroupManager.Instance.currencyController, true,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetUse),animEndCall:
            () =>
            {
                StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
                    () => { CloseWindowWithinUIMgr(true); }));
            });
    }

    public static UIPopupKeepPetReturnController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetReturn) as UIPopupKeepPetReturnController;
    }
}