
using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UICoinCompetitionRewardItem : MonoBehaviour
{
    private GameObject _normal;
    private GameObject _finish;
    private GameObject _current;
    private GameObject _rewardItem;
    private void Awake()
    {
        _normal = transform.Find("Normal").gameObject;
        _finish = transform.Find("Finish").gameObject;
        _current = transform.Find("Current").gameObject;
        _rewardItem=transform.Find("Reward/Item").gameObject;
        _rewardItem.SetActive(false);
    }

    public void Init(CoinCompetitionReward reward)
    {
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
        var cur = CoinCompetitionModel.Instance.GetCurrentReward();
        _current.SetActive(cur!=null && reward.Id==cur.Id);
        _normal.SetActive(cur!=null && reward.Id!=cur.Id);
        // _normal.SetActive(cur!=null && reward.Id==cur.Id && !CoinCompetitionModel.Instance.IsClaimed(reward.Id));
    }

    public void HideReward()
    {
        _rewardItem.transform.parent.gameObject.SetActive(false);
        _current.gameObject.SetActive(false);
    }
    
}
