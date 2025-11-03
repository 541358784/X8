using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupDiamondRewardController : UIWindowController
{
    private LocalizeTextMeshProUGUI rewardCountText = null;
    private Image rewardIcon = null;

    private bool isAdPlay = false;

    private static string constPlaceId = ADConstDefine.RV_BALLOON;

    private Animator _animator;

    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        rewardCountText = GetItem<LocalizeTextMeshProUGUI>("Root/MidGroup/RewardGroup/Text");
        rewardIcon = GetItem<Image>("Root/MidGroup/RewardGroup/Icon");

        Button buttonAds = GetItem<Button>("Root/PlayButton");
        var rewardList = AdConfigHandle.Instance.GetBonus(constPlaceId);
        if (rewardList != null && rewardList.Count > 0)
        {
            if (UserData.Instance.IsResource(rewardList[0].id))
            {
                rewardIcon.sprite = UserData.GetResourceIcon(rewardList[0].id);
                return;
            }

            TableMergeItem mergeConfig = GameConfigManager.Instance.GetItemConfig(rewardList[0].id);
            if (mergeConfig != null)
                rewardIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeConfig.image);
            rewardCountText.SetText(rewardList[0].count.ToString());
        }

        UIAdRewardButton.Create(ADConstDefine.RV_BALLOON, UIAdRewardButton.ButtonStyle.Disable, buttonAds.gameObject,
            (s) =>
            {
                if (s)
                    CloseWindowWithinUIMgr(true);
            });


        Button buttonClose = GetItem<Button>("Root/CloseButton");
        buttonClose.onClick.AddListener(() =>
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
                () => { CloseWindowWithinUIMgr(true); }));
        });

        var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, constPlaceId);
        if (rv != null && rv.Bonus > 0)
        {
            var bs = AdConfigHandle.Instance.GetBonus(rv.Bonus);
            if (bs == null || bs.Count == 0)
            {
                DebugUtil.LogError("bonus is null " + UserGroupManager.Instance.SubUserGroup + "\t" + constPlaceId);
            }

            if (bs != null && bs.Count >= 1)
            {
                rewardCountText.SetText(bs[0].count.ToString());
            }
        }
    }

    public static void ShowUI()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupDiamondReward);
    }

    private void PlayRvCallBack(bool isSuccess)
    {
    }
}