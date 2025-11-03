using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework;
using Game;
using Game.Config;
using Gameplay;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIBoxRewardItem : MonoBehaviour
{
    private Transform _styleGroup;
    private Image _styleIcon;

    private Transform _collectionGroup;
    private Image _collectionIcon;

    private Transform _rewardGroup;
    private Image _rewardIcon;
    private LocalizeTextMeshProUGUI _rewardText;

    private void Awake()
    {
    }

    public void SetData(ResData data)
    {
        _styleGroup = CommonUtils.Find<Transform>(transform, "Root/StyleGroup");
        _styleIcon = CommonUtils.Find<Image>(transform, "Root/StyleGroup/Icon");

        _collectionGroup = CommonUtils.Find<Transform>(transform, "Root/CollectionGroup");
        _collectionIcon = CommonUtils.Find<Image>(transform, "Root/CollectionGroup/Icon");

        _rewardGroup = CommonUtils.Find<Transform>(transform, "Root/RewardGroup");
        _rewardIcon = CommonUtils.Find<Image>(transform, "Root/RewardGroup/Icon");
        _rewardText = CommonUtils.Find<LocalizeTextMeshProUGUI>(transform, "Root/RewardGroup/RewardText");

        _styleGroup.gameObject.SetActive(false);
        _collectionGroup.gameObject.SetActive(false);
        _rewardGroup.gameObject.SetActive(true);
        _rewardIcon.sprite = UserData.GetResourceIcon(data.id, UserData.ResourceSubType.Big);
        if ((int) UserData.ResourceId.Infinity_Energy == data.id)
        {
            _rewardText.SetText(TimeUtils.GetTimeString(data.count, true));
        }
        else
        {
            _rewardText.SetText(data.count.ToString());
        }
    }
}