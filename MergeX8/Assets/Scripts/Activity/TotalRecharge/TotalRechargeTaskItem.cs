using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TotalRecharge
{
    public class TotalRechargeTaskItem : MonoBehaviour
    {
        private Transform _pressed;
        private Transform _rewardItem;
        private LocalizeTextMeshProUGUI _countText;
        private Button _claimBtn;
        private Button _rechargeBtn;
        private TotalRechargeReward rewardConfig;
        private LocalizeTextMeshProUGUI _titleText;
        private void Awake()
        {
            _pressed=transform.Find("Pressed");
            _rewardItem=transform.Find("RewardGroup/RewardItem");
            _rewardItem.gameObject.SetActive(false);
            _countText = transform.Find("CountText").GetComponent<LocalizeTextMeshProUGUI>();
            _claimBtn = transform.Find("ButtonGroup/ClaimButton").GetComponent<Button>();
            _claimBtn.onClick.AddListener(OnClaim);
            _rechargeBtn = transform.Find("ButtonGroup/TowardsGroupButton").GetComponent<Button>();
            _rechargeBtn.onClick.AddListener(OnRecharge);
            _titleText = transform.Find("TitleGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        }


        public void Init(TotalRechargeReward config)
        {
            rewardConfig = config;
            for (int i = 0; i < config.RewardId.Count; i++)
            {
                var obj= Instantiate(_rewardItem, _rewardItem.parent);
                obj.gameObject.SetActive(true);
                InitItem(obj,config.RewardId[i],config.RewardNum[i]);
            }
            _titleText.SetTermFormats((float)config.Score);
            RefreshStatus();
        }

        public void RefreshStatus()
        {
            float s = (float)TotalRechargeModel.Instance.StorageTotalRecharge.TotalRecharge ;
            float y = (float)rewardConfig.Score;
            _countText.SetText(s+"/"+y);
            bool isClaimed = TotalRechargeModel.Instance.IsClaimed(rewardConfig.Id);
            bool isCanClaim = TotalRechargeModel.Instance.IsCanClaim(rewardConfig.Id);
            if (isClaimed)
            {
                _pressed.gameObject.SetActive(true);
                _claimBtn.gameObject.SetActive(false);
                _rechargeBtn.gameObject.SetActive(false);
            }
            else
            {
                if (isCanClaim)
                {
                    _pressed.gameObject.SetActive(false);
                    _claimBtn.gameObject.SetActive(true);
                    _rechargeBtn.gameObject.SetActive(false);
                }
                else
                {
                    _pressed.gameObject.SetActive(false);
                    _claimBtn.gameObject.SetActive(false);
                    _rechargeBtn.gameObject.SetActive(true);
                }

            }
        }
        
        private void InitItem(Transform item, int itemID, int itemCount)
        {
            LocalizeTextMeshProUGUI text = item.Find("RewardText").GetComponent<LocalizeTextMeshProUGUI>();
            text.SetText(itemCount.ToString());
            if (UserData.Instance.IsResource(itemID))
            {
                item.Find("Icon").GetComponent<Image>().sprite =
                    UserData.GetResourceIcon(itemID, UserData.ResourceSubType.Reward);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
                if (itemConfig != null)
                {
                    item.Find("Icon").GetComponent<Image>().sprite =
                        MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                }
            }
        }
        
        private void OnRecharge()
        {
            UIManager.Instance.CloseUI(UINameConst.UIPopupTotalRecharge,true);
            UIStoreController.OpenUI("TotalRecharge",ShowArea.gem_shop); 
        }

        private void OnClaim()
        {
            TotalRechargeModel.Instance.Claim(rewardConfig.Id);
            RefreshStatus();
        }

    }
}