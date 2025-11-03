using System.Collections.Generic;
using UnityEngine;

public partial class UIPillowWheelMainController
{
    private Dictionary<int, SpecialRewardGroup> SpecialRewardDic = new Dictionary<int, SpecialRewardGroup>();
    public void InitSpecialReward()
    {
        var specialConfigs = PillowWheelModel.Instance.SpecialRewardConfigList;
        foreach (var config in specialConfigs)
        {
            var specialReward = transform.Find("Root/Top/Reward/" + config.Id).gameObject.AddComponent<SpecialRewardGroup>();
            specialReward.Init(config);
            SpecialRewardDic.Add(config.Id,specialReward);
        }
    }

    public class SpecialRewardGroup : MonoBehaviour
    {
        private PillowWheelSpecialRewardConfig Config;
        private CommonRewardItem RewardItem;
        public List<Transform> HaveList = new List<Transform>();

        public void Init(PillowWheelSpecialRewardConfig config)
        {
            Config = config;
            for (var i = 0; i < config.Count; i++)
            {
                HaveList.Add(transform.Find((i+1)+"/Have"));
            }
            RewardItem = transform.Find("Reward/Item").gameObject.AddComponent<CommonRewardItem>();
            RewardItem.Init(new ResData(Config.RewardId,Config.RewardNum));
            UpdateCollectState();
        }

        public void UpdateCollectState()
        {
            var collectCount = PillowWheelModel.Instance.Storage.SpecialCollectState.TryGetValue(Config.Id, out var count) ? count : 0;
            for (var i = 0; i < HaveList.Count; i++)
            {
                HaveList[i]?.gameObject.SetActive(i < collectCount);
            }
        }
    }
}