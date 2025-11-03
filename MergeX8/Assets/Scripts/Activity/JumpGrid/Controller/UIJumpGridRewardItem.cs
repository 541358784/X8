
using System;
using System.Collections.Generic;
using DragonPlus.Config.JumpGrid;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JumpGrid
{
    public class UIJumpGridRewardItem : MonoBehaviour
    {
        private GameObject _finish;
        private List<RewardData> _rewardDatas = new List<RewardData>();
        private TableJumpGridReward _rewardConfig;
        private int _index;
        
        private void Awake()
        {
            _finish = transform.Find("Finish").gameObject;
        }

        public void Init(TableJumpGridReward reward, int index)
        {
            _rewardConfig = reward;
            _index = index;
            
            for (int i = 0; i < reward.RewardId.Count; i++)
            {
                RewardData rewardData = new RewardData();
                rewardData.gameObject = transform.Find($"Reward/Item{i + 1}").gameObject;
                rewardData.image = transform.Find($"Reward/Item{i + 1}/Icon").GetComponent<Image>();
                
                rewardData.UpdateReward(reward.RewardId[i], 1);
                
                _rewardDatas.Add(rewardData);
            }

            UpdateStatus();
        }

        public void UpdateStatus()
        {
            if(_rewardConfig == null)
                return;

            bool isClaim = JumpGridModel.Instance.IsClaimed(_rewardConfig.Id);
            _finish.SetActive(isClaim);
            
            _rewardDatas.ForEach(a=>a.gameObject.SetActive(!isClaim));
        }
    }
}
