using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public partial class UIParrotMainController
{
    public Dictionary<int, LevelGroup> GroupDic = new Dictionary<int, LevelGroup>();
    public void InitLevelGroup()
    {
        var group = 1;
        var groupNode = transform.Find("Root/Scroll View/Viewport/Content/" + group);
        while (groupNode)
        {
            var levelGroup = groupNode.gameObject.AddComponent<LevelGroup>();
            levelGroup.Init(group,this);
            GroupDic.Add(group,levelGroup);
            group++;
            groupNode = transform.Find("Root/Scroll View/Viewport/Content/" + group);
        }

        var curState = ParrotModel.Instance.GetLevelState(Storage.TotalScore);
        SetState(curState);
    }

    public void SetState(ParrotLevelState state)
    {
        foreach (var pair in GroupDic)
        {
            pair.Value.gameObject.SetActive(state.Group == pair.Key);
            if (state.Group == pair.Key)
            {
                pair.Value.SetState(state);   
            }
        }
    }
    

    public class LevelGroup : MonoBehaviour
    {
        public int GroupIndex;
        public UIParrotMainController Controller;
        public Dictionary<int, Transform> ParrotPositionDic = new Dictionary<int, Transform>();
        public Dictionary<int, List<CommonRewardItem>> RewardIconDic = new Dictionary<int, List<CommonRewardItem>>();
        public Dictionary<int, Transform> RewardGroupDic = new Dictionary<int, Transform>();
        public SkeletonGraphic ParrotSpine;
        public void Init(int groupIndex,UIParrotMainController controller)
        {
            GroupIndex = groupIndex;
            Controller = controller;
            ParrotSpine = transform.Find("RewardGroup/Spine").GetComponent<SkeletonGraphic>();
            ParrotPositionDic.Add(0,transform.Find("BG/Point"));
            var pointIndex = 1;
            var pointNode = transform.Find("RewardGroup/" + pointIndex);
            while (pointNode)
            {
                var config =
                    ParrotModel.Instance.RewardConfig.Find(
                        a => a.Group == GroupIndex && a.GroupInnerIndex == pointIndex);
                RewardIconDic.Add(pointIndex,new List<CommonRewardItem>());
                ParrotPositionDic.Add(pointIndex,pointNode.Find("Point"));
                RewardGroupDic.Add(pointIndex, pointNode.Find("RewardGroup"));
                var rewardIndex = 1;
                var rewardItem = pointNode.Find("RewardGroup/Item" + rewardIndex);
                while (rewardItem)
                {
                    var reward = rewardItem.gameObject.AddComponent<CommonRewardItem>();
                    RewardIconDic[pointIndex].Add(reward);
                    rewardIndex++;
                    rewardItem = pointNode.Find("RewardGroup/Item" + rewardIndex);
                }
                if (config != null)
                {
                    var configRewards = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
                    var rewardItems = RewardIconDic[pointIndex];
                    if (rewardItems.Count <  configRewards.Count)
                        Debug.LogError("界面奖励数量少于配置奖励数量，有奖励被隐藏"+config.Id);
                    for (var i = 0; i < rewardItems.Count; i++)
                    {
                        if (i < configRewards.Count)
                        {
                            rewardItems[i].Init(configRewards[i]);
                            rewardItems[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            rewardItems[i].gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    Debug.LogError("鹦鹉界面和配置不符,未找到第"+GroupIndex+"组第"+pointIndex+"关的配置");
                    pointNode.gameObject.SetActive(false);
                }
                pointIndex++;
                pointNode = transform.Find("RewardGroup/" + pointIndex);
            }
        }

        public void SetState(ParrotLevelState state)
        {
            if (state.Group != GroupIndex)
                return;
            var positionIndex = state.GroupInnerIndex - 1;
            ParrotSpine.transform.position = ParrotPositionDic[positionIndex].position;
            ParrotSpine.PlaySkeletonAnimation("idle",true);
            foreach (var pair in RewardGroupDic)
            {
                pair.Value.gameObject.SetActive(pair.Key > positionIndex);
            }
            UpdateParrotSpineScaleX(positionIndex);
        }

        public void UpdateParrotSpineScaleX(int positionIndex)
        {
            if (ParrotPositionDic.Count > positionIndex + 1)
            {
                var nextPosition = ParrotPositionDic[positionIndex + 1];
                if (nextPosition.position.x > ParrotSpine.transform.position.x)
                {
                    if (ParrotSpine.transform.localScale.x > 0)
                    {
                        var scale = ParrotSpine.transform.localScale;
                        scale.x *= -1;
                        ParrotSpine.transform.localScale = scale;
                    }
                }
                else
                {
                    if (ParrotSpine.transform.localScale.x < 0)
                    {
                        var scale = ParrotSpine.transform.localScale;
                        scale.x *= -1;
                        ParrotSpine.transform.localScale = scale;
                    }
                }
            }
        }
        
        public async Task PerformJump(ParrotLevelState oldState)
        {
            SetState(oldState);
            var nextPositionIndex = oldState.GroupInnerIndex;
            var nextPosition = ParrotPositionDic[nextPositionIndex].position;
            ParrotSpine.PlaySkeletonAnimation("fly",true);
            XUtility.WaitSeconds(JumpTime - 0.5f, () =>
            {
                if (!this)
                    return;
                ParrotSpine.PlaySkeletonAnimation("down");
                XUtility.WaitSeconds(0.5f, () =>
                {
                    if (!this)
                        return;
                    ParrotSpine.PlaySkeletonAnimation("idle",true);
                });
            });
            ParrotSpine.transform.DOMove(nextPosition, JumpTime).SetEase(Ease.Linear);
            await XUtility.WaitSeconds(JumpTime);
            var rewards = oldState.Rewards;
            var popRewardTask = new TaskCompletionSource<bool>();
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, animEndCall:
                () =>
                {
                    popRewardTask.SetResult(true);
                });
            if (!this)
                return;
            RewardGroupDic[nextPositionIndex].gameObject.SetActive(false);
            await popRewardTask.Task;
            if (!this)
                return;
            UpdateParrotSpineScaleX(nextPositionIndex);
        }
    }
}