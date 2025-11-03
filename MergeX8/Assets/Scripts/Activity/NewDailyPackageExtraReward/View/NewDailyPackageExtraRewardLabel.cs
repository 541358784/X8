using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NewDailyPackageExtraRewardLabel:MonoBehaviour
{
    private void Awake()
    {
        EventDispatcher.Instance.AddEvent<EventNewDailyPackageExtraRewardEnd>(OnFinish);
    }

    public void OnFinish(EventNewDailyPackageExtraRewardEnd evt)
    {
        var extraRewards = NewDailyPackageExtraRewardModel.Instance.GetExtraReward(PackageId);
        if (extraRewards.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventNewDailyPackageExtraRewardEnd>(OnFinish);
    }

    private Button Btn;
    private CommonRewardItem RewardItem;
    private int PackageId;
    public void Init(int packageId,int nodeName)
    {
        PackageId = packageId;
        var tips = transform.Find("Tip");
        tips.gameObject.SetActive(false);
        Btn = transform.GetComponent<Button>();
        Btn.onClick.AddListener(()=>
        {
            tips.DOKill();
            tips.gameObject.SetActive(false);
            tips.gameObject.SetActive(true);
            DOVirtual.DelayedCall(1f, () =>
            {
                tips.gameObject.SetActive(false);
            }).SetTarget(tips);
        });
        RewardItem = transform.Find("Item").gameObject.AddComponent<CommonRewardItem>();
        var extraRewards = NewDailyPackageExtraRewardModel.Instance.GetExtraReward(packageId);
        if (extraRewards.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        if (NewDailyPackageExtraRewardModel.Instance.GlobalConfig.NodeName != nodeName)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        RewardItem.Init(extraRewards[0]);
    }
}