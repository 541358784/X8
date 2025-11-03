using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class UIFlowerFieldMainController
{
    public Slider Slider;
    public LocalizeTextMeshProUGUI SliderText;
    public List<RewardBox> RewardBoxList = new List<RewardBox>();
    
    public void InitTopGroup()
    {
        var rewardConfigs = FlowerFieldModel.Instance.RewardConfig;
        Slider = GetItem<Slider>("Root/LevelGroup/Scroll View/Viewport/Content/Slider");
        Slider.maxValue = rewardConfigs.Count;
        SliderText = GetItem<LocalizeTextMeshProUGUI>("Root/LevelGroup/Level/Text");

        var defaultRewardItem = transform.Find("Root/LevelGroup/Scroll View/Viewport/Content/IconGroup/1");
        defaultRewardItem.gameObject.SetActive(false);
        for (var i=0;i<rewardConfigs.Count-1;i++)
        {
            var config = rewardConfigs[i];
            var rewardBox = Instantiate(defaultRewardItem, defaultRewardItem.parent).gameObject.AddComponent<RewardBox>();
            rewardBox.gameObject.SetActive(true);
            rewardBox.Init(config);
            RewardBoxList.Add(rewardBox);    
        }

        var finialRewardBox = transform.Find("Root/LevelGroup/FinallyReward").gameObject.AddComponent<RewardBox>();
        finialRewardBox.Init(rewardConfigs.Last());
        RewardBoxList.Add(finialRewardBox);
        var curState = FlowerFieldModel.Instance.GetLevelState(Storage.TotalScore);
        SetTopGroupState(curState);
    }

    public void SetTopGroupState(FlowerFieldLevelState state)
    {
        Slider.DOKill();
        Slider.value = state.GroupInnerIndex-1 + (float)state.CurScore/state.MaxScore;
        SliderText.SetText(state.TotalScore.ToString());
        // if (state.CurScore >= state.MaxScore)
        // {
        //     SliderText.SetText("Max");
        // }
        
        for (var i = 0; i < RewardBoxList.Count; i++)
        {
            RewardBoxList[i].SetScore(state.TotalScore);
        }
    }

    public void TopGroupPerformToMax(FlowerFieldLevelState state1,FlowerFieldLevelState state2)
    {
        SetTopGroupState(state1);
        var scoreDoValue = state1.TotalScore;
        DOTween.To(()=>state1.TotalScore , value =>
        {
            SliderText.SetText(value.ToString());
        }, state2.TotalScore, JumpTime).SetTarget(Slider);
        Slider.DOValue(state2.GroupInnerIndex-1 + (float)state2.CurScore/state2.MaxScore, JumpTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetTopGroupState(state2);
        });
    }

    public class RewardBox : MonoBehaviour
    {
        public Transform Normal;
        public LocalizeTextMeshProUGUI NormalText;
        public Transform Finish;
        public LocalizeTextMeshProUGUI FinishText;
        public Transform TipsNode;
        public Button TipsBtn;
        public List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
        public Transform DefaultRewardItem;
        public FlowerFieldRewardConfig Config;
        public void Init(FlowerFieldRewardConfig config)
        {
            Config = config;
            var rewards = CommonUtils.FormatReward(Config.RewardId, Config.RewardNum);
            Normal = transform.Find("Normal");
            NormalText = transform.Find("Normal/Text").GetComponent<LocalizeTextMeshProUGUI>();
            NormalText.SetText(Config.Score.ToString());
            Finish = transform.Find("Finish");
            FinishText = transform.Find("Finish/Text").GetComponent<LocalizeTextMeshProUGUI>();
            FinishText.SetText(Config.Score.ToString());
            TipsNode = transform.Find("Tips");
            TipsBtn = transform.gameObject.GetComponent<Button>();
            TipsBtn.onClick.AddListener(() =>
            {
                TipsNode.DOKill();
                TipsNode.gameObject.SetActive(!TipsNode.gameObject.activeSelf);
                if (TipsNode.gameObject.activeSelf)
                {
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        if (!this)
                            return;
                        TipsNode.gameObject.SetActive(false);
                    }).SetTarget(TipsNode);
                }
            });
            foreach (var rewardItem in RewardItems)
            {
                DestroyImmediate(rewardItem.gameObject);
            }
            RewardItems.Clear();
            DefaultRewardItem = transform.Find("Tips/Item");
            DefaultRewardItem.gameObject.SetActive(false);
            foreach (var reward in rewards)
            {
                var rewardItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject.AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(reward);
                RewardItems.Add(rewardItem);
            }
        }

        public void SetScore(int score)
        {
            if (score >= Config.Score)
            {
                Normal.gameObject.SetActive(false);
                Finish.gameObject.SetActive(true);
            }
            else
            {
                Normal.gameObject.SetActive(true);
                Finish.gameObject.SetActive(false);
            }
        }
    }
}