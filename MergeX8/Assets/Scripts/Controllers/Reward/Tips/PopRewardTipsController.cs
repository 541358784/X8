using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;


public class PopRewardTipsController : UIWindowController
{
    private Animator animator;
    private LocalizeTextMeshProUGUI contentText;
    private Image _icon;
    public override void PrivateAwake()
    {
        contentText = GetItem<LocalizeTextMeshProUGUI>("Reward/Text");
        animator = GetItem<Animator>("Reward");
        _icon = GetItem<Image>("Reward/Text/Image");
    }

    public void PlayAnim(int rewardID,int rewardCount, Action endCall)
    {
        if (animator == null)
        {
            endCall?.Invoke();
            return;
        }
        if (UserData.Instance.IsResource(rewardID))
        {
            _icon.sprite = UserData.GetResourceIcon(rewardID, UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(rewardID);
            _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
        }
        contentText?.SetTerm("+"+rewardCount);
        StartCoroutine(CommonUtils.PlayAnimation(animator, "appear", "", endCall));
    }

}