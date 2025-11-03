using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFishCultureMainController
{
    private bool InitLevelGroupFlag = false;
    public LevelGroup LevelGroupNode;

    public void InitLevelGroup()
    {
        if (InitLevelGroupFlag)
            return;
        InitLevelGroupFlag = true;
        LevelGroupNode = transform.Find("Root/LevelGroup").gameObject.AddComponent<LevelGroup>();
        LevelGroupNode.Init(this);
    }
    public class LevelGroup : MonoBehaviour
    {
        private UIFishCultureMainController Controller;
        private StorageFishCulture Storage => Controller.Storage;
        private FishCultureModel Model => FishCultureModel.Instance;
        private LocalizeTextMeshProUGUI LevelText;
        private RewardNode FinialNode;
        private List<RewardNode> RewardNodes = new List<RewardNode>();
        private Slider Slider;
        private RectTransform Content;
        private float ViewWidth;
        public void Init(UIFishCultureMainController controller)
        {
            Controller = controller;
            var levelConfigs = FishCultureModel.Instance.LevelConfig;
            var lastLevelConfig = levelConfigs.Last();
            FinialNode = transform.Find("FinallyReward").gameObject.AddComponent<RewardNode>();
            FinialNode.Init(lastLevelConfig,this);
            var defaultItem = transform.Find("Scroll View/Viewport/Content/IconGroup/1");
            defaultItem.gameObject.SetActive(false);
            
            for (var i = 0; i < levelConfigs.Count - 1; i++)
            {
                var levelConfig = levelConfigs[i];
                var rewardNode = Instantiate(defaultItem, defaultItem.parent).gameObject.AddComponent<RewardNode>();
                rewardNode.gameObject.SetActive(true);
                rewardNode.Init(levelConfig,this);
                RewardNodes.Add(rewardNode);
            }
            RewardNodes.Add(FinialNode);

            Slider = transform.Find("Scroll View/Viewport/Content/Slider").GetComponent<Slider>();
            Slider.minValue = 0;
            Slider.maxValue = levelConfigs.Count;
            Slider.value = Storage.CollectList.Count;
            LevelText = transform.Find("Level/Text").GetComponent<LocalizeTextMeshProUGUI>();
            LevelText.SetText((Storage.CollectList.Count).ToString());
            EventDispatcher.Instance.AddEvent<EventFishCultureGetNewFish>(OnGetNewFish);
            ViewWidth = (transform.Find("Scroll View/Viewport").transform as RectTransform).rect.width;
            Content = transform.Find("Scroll View/Viewport/Content") as RectTransform;
            var curCount = Storage.CollectList.Count;
            LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
            XUtility.WaitSeconds(0.3f, () =>
            {
                FocusOnLevel(curCount);
            });
        }

        public void FocusOnLevel(int curCount)
        {
            var time = 0.3f;
            var levelConfigs = FishCultureModel.Instance.LevelConfig;
            if (curCount < levelConfigs.Count - 1)
            {
                var showNode = RewardNodes[curCount];
                var positionX = (showNode.transform as RectTransform).anchoredPosition.x + Content.anchoredPosition.x;
                if (positionX < 0)
                {
                    var x = Content.anchoredPosition.x - positionX;
                    Content.DOAnchorPosX(x, time);
                }
                else if (positionX > ViewWidth)
                {
                    var x = Content.anchoredPosition.x - (positionX - ViewWidth);
                    Content.DOAnchorPosX(x, time);
                }
            }
            else if (curCount == levelConfigs.Count - 1)
            {
                var x = ViewWidth - Content.rect.width;
                Content.DOAnchorPosX(x, time);
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventFishCultureGetNewFish>(OnGetNewFish);
        }

        public void OnGetNewFish(EventFishCultureGetNewFish evt)
        {
            var newValue = Storage.CollectList.Count;
            FocusOnLevel(newValue);
            Slider.DOKill();
            Slider.DOValue(newValue, 1f).OnComplete(() =>
            {
                XUtility.WaitSeconds(0.5f).AddCallBack(() =>
                {
                    var rewards = CommonUtils.FormatReward(evt.FishConfig.RewardId, evt.FishConfig.RewardNum);
                    var reason =
                        new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason
                            .FishCultureGet);
                    CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                        false, reason); 
                }).WrapErrors();
                if (!this)
                    return;
                RewardNodes[newValue-1].UpdateFinishState();
                LevelText.SetText((Storage.CollectList.Count).ToString());
            });
        }

        public class RewardNode : MonoBehaviour
        {
            private Transform NormalNode;
            private Transform FinishNode;
            private bool IsFinish;
            private LevelGroup Controller;
            private FishCultureRewardConfig Config;
            private Button Btn;
            private Transform Tip;
            private List<CommonRewardItem> RewardIcons = new List<CommonRewardItem>();
            public void Init(FishCultureRewardConfig config,LevelGroup controller)
            {
                Config = config;
                Controller = controller;
                var rewards = CommonUtils.FormatReward(config.RewardId,config.RewardNum);
                NormalNode = transform.Find("Normal");
                FinishNode = transform.Find("Finish");
                IsFinish = Controller.Storage.CollectList.Contains(Config.Id);
                NormalNode.gameObject.SetActive(!IsFinish);
                FinishNode.gameObject.SetActive(IsFinish);
                Tip = transform.Find("Tips");
                Tip.gameObject.SetActive(false);
                Btn = transform.GetComponent<Button>();
                Btn.onClick.AddListener(() =>
                {
                    Tip.gameObject.SetActive(!Tip.gameObject.activeSelf);
                    Tip.DOKill(false);
                    if (Tip.gameObject.activeSelf)
                    {
                        DOVirtual.DelayedCall(1f, () =>
                        {
                            Tip.gameObject.SetActive(false);
                        }).SetTarget(Tip);
                    }
                });
                foreach (var rewardItem in RewardIcons)
                {
                    Destroy(rewardItem.gameObject);
                }
                RewardIcons.Clear();
                var defaultItem = transform.Find("Tips/Item");
                defaultItem.gameObject.SetActive(false);
                for (var i = 0; i < rewards.Count; i++)
                {
                    var rewardItem = Instantiate(defaultItem, defaultItem.parent).gameObject
                        .AddComponent<CommonRewardItem>();
                    rewardItem
                        .gameObject.SetActive(true);
                    rewardItem.Init(rewards[i]);
                    RewardIcons.Add(rewardItem);
                }
            }

            public void UpdateFinishState()
            {
                var curIsFinish = Controller.Storage.CollectList.Contains(Config.Id);
                if (curIsFinish == IsFinish)
                    return;
                IsFinish = curIsFinish;
                NormalNode.gameObject.SetActive(!IsFinish);
                FinishNode.gameObject.SetActive(IsFinish);
            }
        } 
    }
}