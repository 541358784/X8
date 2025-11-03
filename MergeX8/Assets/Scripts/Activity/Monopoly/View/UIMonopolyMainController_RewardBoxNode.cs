using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    private bool InitRewardBoxNodeFlag = false;
    public void InitRewardBoxNode()
    {
        if (InitRewardBoxNodeFlag)
            return;
        InitRewardBoxNodeFlag = true;
        var rewardBoxNode = transform.Find("Root/Reward").gameObject.AddComponent<RewardBoxNode>();
        rewardBoxNode.MainUI = this;
        rewardBoxNode.Init();
    }
    public class RewardBoxNode:MonoBehaviour
    {
        public UIMonopolyMainController MainUI;
        private LocalizeTextMeshProUGUI SliderText;
        private Slider Slider;
        // private Button Btn;
        public MonopolyRewardBoxConfig Config;
        private int ShowCollectNum = 0;
        
        public void Init()
        {
            Config = MainUI.Storage.GetCurRewardBoxConfig();
            ShowCollectNum = MainUI.Storage.RewardBoxCollectNum;
            UpdateViewState();
        }
        private void Awake()
        {
            // Btn = transform.gameObject.GetComponent<Button>();
            // Btn.onClick.AddListener(OnClick);
            SliderText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Slider = transform.Find("Slider").GetComponent<Slider>();
            EventDispatcher.Instance.AddEvent<EventMonopolyUICollectRewardBox>(CollectRewardBox);
            EventDispatcher.Instance.AddEvent<EventMonopolyUIAddRewardBoxScore>(AddProgress);
        }
        public void CollectRewardBox(EventMonopolyUICollectRewardBox evt)
        {
            var nextBoxConfig = MainUI.Storage.GetCurRewardBoxConfig();
            Action<Action> performAction = (callback) =>
            {
                
                if (Config != evt.RewardBoxConfig)
                {
                    Debug.LogError("宝箱收集错误,界面和数据对不上");
                    callback();
                    return;   
                }
                if (IsAuto)
                {
                    Config = nextBoxConfig;
                    ShowCollectNum = 0;
                    UpdateViewState();
                    callback();
                    return;
                }
                var rewards = CommonUtils.FormatReward(evt.RewardBoxConfig.RewardId, evt.RewardBoxConfig.RewardNum);
                CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                    false, new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyGet
                    }, () =>
                    {
                        Config = nextBoxConfig;
                        ShowCollectNum = 0;
                        UpdateViewState();
                        callback();
                    });
            };
            MainUI.PushPerformAction(performAction);
        }

        public void AddProgress(EventMonopolyUIAddRewardBoxScore evt)
        {
            Action<Action> performAction = (callback) =>
            {
                if (Config != evt.RewardBoxConfig || ShowCollectNum != evt.OldValue)
                {
                    Debug.LogError("宝箱收集进度增长错误,界面和数据对不上");
                    callback();
                    return;   
                }
                ShowCollectNum = evt.NewValue;
                Slider.transform.DOKill(true);
                DOTween.To(() => evt.OldValue, (value) =>
                {
                    Slider.value = (float)value / Config.CollectNum;
                    SliderText.SetText(value+"/"+Config.CollectNum);
                }, evt.NewValue, 0.3f).OnComplete(() =>
                {
                    Slider.value = (float)ShowCollectNum / Config.CollectNum;
                    SliderText.SetText(ShowCollectNum+"/"+Config.CollectNum);
                }).SetTarget(Slider.transform).SetEase(Ease.Linear);
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUICollectRewardBox>(CollectRewardBox);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUIAddRewardBoxScore>(AddProgress);
        }

        public void UpdateViewState()
        {
            Slider.transform.DOKill(true);
            Slider.value = (float)ShowCollectNum / Config.CollectNum;
            SliderText.SetText(ShowCollectNum+"/"+Config.CollectNum);
        }
    }
}