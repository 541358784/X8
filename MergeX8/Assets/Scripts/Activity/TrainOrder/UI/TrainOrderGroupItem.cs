using System;
using UnityEngine;

namespace Activity.TrainOrder
{
    public class TrainOrderGroupItem : MonoBehaviour
    {
        public CommonRewardItem RewardItem => _normalItem;

        private TrainOrderOrderGroup _config;


        private Transform _tranNormal;
        private Transform _tranFinish;

        private CommonRewardItem _normalItem;
        private CommonRewardItem _finishItem;


        private void Awake()
        {
            _tranNormal = transform.Find("Normal");
            _tranFinish = transform.Find("Finish");
            _normalItem = _tranNormal.Find("Item").GetComponentDefault<CommonRewardItem>();
            _finishItem = _tranFinish.Find("Item").GetComponentDefault<CommonRewardItem>();
        }


        public void Init(TrainOrderOrderGroup config)
        {
            _config = config;
            _normalItem.Init(new ResData(_config.GroupReward[0], _config.GroupReward[1]));
            _finishItem.Init(new ResData(_config.GroupReward[0], _config.GroupReward[1]));
            RefreshView();
        }


        public void RefreshView()
        {
            bool groupComplete = TrainOrderModel.Instance.IsOrderGroupComplete(_config);
            _tranNormal.gameObject.SetActive(!groupComplete);
            _tranFinish.gameObject.SetActive(groupComplete);
        }
    }
}