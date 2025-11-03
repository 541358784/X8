using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UISeaRacingOpenBoxController:UIWindowController
{
    private SkeletonGraphic Spine;
    private Transform DefaultItem;
    private Button CloseButton;
    public override void PrivateAwake()
    {
        Spine = GetItem<SkeletonGraphic>("Root/BoxGroup/Position/BoxSpine");
        DefaultItem = transform.Find("Root/Reward/Item");
        DefaultItem.gameObject.SetActive(false);
        CloseButton = GetItem<Button>("Root/ButtonClose");
        CloseButton.onClick.AddListener(OnClickCloseButton);
    }

    private List<ResData> Rewards;
    private List<RewardData> RewardData = new List<RewardData>();
    private Action Callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Rewards = objs[0] as List<ResData>;
        if (Rewards == null)
        {
            Debug.LogError("海上竞速开宝箱，奖励为空");
            return;
        }
        var rank = (int)objs[1];
        if (objs.Length > 2)
        {
            Callback = objs[2] as Action;
        }
        RewardData.Clear();
        for (var i = 0; i < Rewards.Count; i++)
        {
            var reward = Rewards[i];
            var rewardItemObj = Instantiate(DefaultItem.gameObject, DefaultItem.parent);
            RewardData rdData = new RewardData();
            rdData.gameObject = rewardItemObj;
            rdData.image = GetItem<Image>("Icon", rewardItemObj);
            rdData.numText = GetItem<LocalizeTextMeshProUGUI>("Num", rewardItemObj);
            rdData.UpdateReward(reward);
            rewardItemObj.SetActive(true);
            RewardData.Add(rdData);
        }
        Spine.Skeleton.SetSkin("box"+rank);
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.REWARD_POPUP);
        ClickEnable = false;
        XUtility.WaitSeconds(2.25f,()=>ClickEnable=true);
    }

    private bool ClickEnable = true;
    public void OnClickCloseButton()
    {
        if (!ClickEnable)
            return;
        ClickEnable = false;
        _animator.Play("disappear", 0, 0);
        FlyGameObjectManager.Instance.FlyObject(RewardData, CurrencyGroupManager.Instance.currencyController, () =>
        {
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);

            CloseWindowWithinUIMgr(true);
            Callback?.Invoke();
            CommonRewardManager.Instance.PopupCacheReward();
            foreach (var resData in RewardData)
            {
                GameObject.Destroy(resData.gameObject);
            }
        });
    }
    public static UISeaRacingOpenBoxController Open(List<ResData> Rewards,int rank,
        System.Action animEndCall = null)
    {
        var popup = UIManager.Instance.OpenUI(UINameConst.UISeaRacingOpenBox,Rewards,rank,animEndCall) as UISeaRacingOpenBoxController;
        return popup;
    }
}