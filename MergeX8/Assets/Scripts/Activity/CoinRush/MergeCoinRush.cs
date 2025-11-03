using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GoogleMobileAds.Api;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeCoinRush : MonoBehaviour
{
    private StorageCoinRush Storage;
    private void SetStorage(StorageCoinRush storage)
    {
        Storage = storage;
        RefreshView();
    }
    private Slider _slider;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _needText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    // private SkeletonGraphic _skeletonGraphic;
    // private Image _rewardIcon;

    private Button _collectBtn;

    private Image RewardIcon;
    // private Transform _rewardGroup;

    private void Awake()
    {
        _timeGroup = transform.Find("Root/TimeGroup");
        // _rewardGroup = transform.Find("Root/Slider/RewardGroup");
        _collectBtn = transform.Find("Root/Button").GetComponent<Button>();
        _collectBtn.onClick.AddListener(OnClick);
        // _rewardIcon = transform.Find("Root/Slider/rewardIcon").GetComponent<Image>();
        _slider = transform.Find("Root/Slider").GetComponent<Slider>();
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);

        _needText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();

        InvokeRepeating("RefreshCountDown", 0, 1f);
        RewardIcon = transform.Find("Root/Slider/Reward").GetComponent<Image>();
        
        SetStorage(CoinRushModel.Instance.StorageCoinRush);
    }
    public void OnClick()
    {
        if (CoinRushModel.Instance.IsMaxLevel)
        {
            if (!CoinRushModel.Instance.IsOpened())
                return;
            else
            {
                var rewards = CoinRushModel.Instance.FinialRewards;
                for (int i = 0; i < rewards.Count; i++)
                {
                    if (!UserData.Instance.IsResource(rewards[i].id))
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonCoinrushMission,
                            isChange = false,
                            itemAId = rewards[i].id
                        });
                    }
                }

             
                var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CoinrushMission};
                CommonRewardManager.Instance.PopCommonReward(rewards,
                    CurrencyGroupManager.Instance.currencyController,
                    true, reasonArgs);
                CoinRushModel.Instance.CompletedActivity();
            }
        }
        else
        {
            UICoinRushMainController controller =
                (UICoinRushMainController) UIManager.Instance.OpenUI(Storage.GetAssetPathWithSkinName(UINameConst.UICoinRushMain));   
        }
    }

    // private void OnEnable()
    // {
    //     InitUI();
    //     StopAllCoroutines();
    // }

    // private void InitUI()
    // {
    //     if (!CoinRushModel.Instance.IsInitFromServer())
    //         return;
    //     RefreshView();
    // }

    public void RefreshView()
    {
        var curTime = (long)APIManager.Instance.GetServerTime();
        gameObject.SetActive(curTime <= Storage.ActivityEndTime && curTime >= Storage.StartActivityTime && !Storage.IsCollectFinalReward);
        _needText.SetText(CoinRushModel.Instance.Level + "/" + CoinRushModel.Instance.MaxLevel);
        _slider.value = Mathf.Min((float)CoinRushModel.Instance.Level / CoinRushModel.Instance.MaxLevel, 1f);
        _collectBtn.gameObject.SetActive(CoinRushModel.Instance.IsMaxLevel);
        //_slider.gameObject.SetActive(!CoinRushModel.Instance.IsMaxLevel);
        _timeGroup.gameObject.SetActive(!CoinRushModel.Instance.IsMaxLevel);
        RewardIcon.sprite = UserData.GetResourceIcon(CoinRushModel.Instance.FinialRewards[0].id, UserData.ResourceSubType.Big);
    }
    private void RefreshCountDown()
    {
        RefreshView();
        _countDownTime.SetText(CoinRushModel.Instance.GetActivityLeftTimeString());
    }

    private void OnDestroy()
    {
    }
}