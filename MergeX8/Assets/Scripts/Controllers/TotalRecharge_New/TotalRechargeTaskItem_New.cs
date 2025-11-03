using System;
using Decoration;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace TotalRecharge_New
{
    public class TotalRechargeTaskItem_New : MonoBehaviour
    {
        private Transform _pressed;
        private Transform _rewardItem;
        private LocalizeTextMeshProUGUI _countText;
        private Button _claimBtn;
        private Button _rechargeBtn;
        private TableTotalRechargeNew rewardConfig;
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


        public void Init(TableTotalRechargeNew config)
        {
            rewardConfig = config;
            for (int i = 0; i < config.rewardId.Length; i++)
            {
                var obj= Instantiate(_rewardItem, _rewardItem.parent);
                obj.gameObject.SetActive(true);
                InitItem(obj,config.rewardId[i],config.rewardNum[i], false);
            }

            if (config.decoRewardId != null)
            {
                for (int i = 0; i < config.decoRewardId.Length; i++)
                {
                    var obj= Instantiate(_rewardItem, _rewardItem.parent);
                    obj.gameObject.SetActive(true);
                    InitItem(obj,config.decoRewardId[i], 1, true);
                }
            }
            
            _titleText.SetTermFormats(config.score);
            RefreshStatus();
        }

        public void RefreshStatus()
        {
            int s = TotalRechargeModel_New.Instance.StorageTotalRechargeNew.TotalRecharge;
            int y = rewardConfig.score;
            _countText.SetText(s+"/"+y);
            bool isClaimed = TotalRechargeModel_New.Instance.IsClaimed(rewardConfig.id);
            bool isCanClaim = TotalRechargeModel_New.Instance.IsCanClaim(rewardConfig.id);
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
        
        private void InitItem(Transform item, int itemID, int itemCount, bool isDeco)
        {
            LocalizeTextMeshProUGUI text = item.Find("RewardText").GetComponent<LocalizeTextMeshProUGUI>();
            text.SetText(itemCount.ToString());
            if (isDeco)
            {
                var decoItem = DecoManager.Instance.FindItem(itemID);
                if (decoItem != null)
                {
                    item.Find("Icon").GetComponent<Image>().sprite = CommonUtils.LoadDecoItemIconSprite(decoItem._node._stage.Area.Id,
                        decoItem._data._config.buildingIcon);
                }
                return;
            }
            
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
            UIManager.Instance.CloseUI(UINameConst.UIPopupTotalRecharge_New,true);
            UIStoreController.OpenUI("TotalRecharge",ShowArea.gem_shop); 
        }

        private void OnClaim()
        {
            TotalRechargeModel_New.Instance.Claim(rewardConfig.id);
            RefreshStatus();
        }


    }
}