
using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UICoinCompetitionRewardItemExtra : MonoBehaviour
{
    private GameObject _finish;
    private GameObject _rewardItem;
    private Animator _animator;
    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();
        _finish = transform.Find("Activation/Finish").gameObject;
        _rewardItem=transform.Find("Activation/Reward/Item").gameObject;
        _rewardItem.SetActive(false);
    }

    private CoinCompetitionReward CurrentReward;
    public void Init(CoinCompetitionReward reward)
    {
        CurrentReward = reward;
        if (reward == null)
        {
            gameObject.SetActive(false);
            return;
        }
        for (int i = 0; i < reward.RewardId.Count; i++)
        {
            var item=Instantiate(_rewardItem, _rewardItem.transform.parent);
            item.SetActive(true);
            if (UserData.Instance.IsResource(reward.RewardId[i]))
            {
                item.GetComponent<Image>().sprite = UserData.GetResourceIcon(reward.RewardId[i]);
            }
            else
            {
                var itemConfig= GameConfigManager.Instance.GetItemConfig(reward.RewardId[i]);
                item.GetComponent<Image>().sprite=  MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);   
            }
        }

        UpdateStatus(reward);
    }

    public void UpdateStatus(CoinCompetitionReward reward)
    {
        _finish.SetActive(CoinCompetitionModel.Instance.IsClaimed(reward.Id));
    }

    public void HideReward()
    {
        _rewardItem.transform.parent.gameObject.SetActive(false);
    }

    public void PlayAppearAnimation(Action callback)
    {
        _animator.PlayAnimation("appear");
        XUtility.WaitSeconds(0.5f, callback);
    }

    public void PlayLockAnimation()
    {
        _animator.PlayAnimation("normal");
    }

    public void PlayUnLockAnimation()
    {
        _animator.Play("appear", -1, 1f);
        _animator.speed = 0f;
    }

}
