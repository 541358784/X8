using System;
using System.Collections.Generic;
using Deco.Node;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API.Protocol;
using Farm.Model;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class SeedCell : MonoBehaviour, IInitContent
    {
        private TableFarmSeed _config;
        private DecoNode _node;
        
        private Image _image;
        private LocalizeTextMeshProUGUI _time;

        private Image _rewardImage;
        private LocalizeTextMeshProUGUI _rewardNum;

        private Image _costImage;
        private LocalizeTextMeshProUGUI _costNum;
        
        private Button _buyButton;

        private LocalizeTextMeshProUGUI _lockText;
        private void Awake()
        {
            _time = transform.Find("Time/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _image= transform.Find("Icon").GetComponent<Image>();
            
            _rewardNum = transform.Find("Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardImage= transform.Find("Num/Icon").GetComponent<Image>();

            _costNum = transform.Find("ButtonBuy/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _costImage= transform.Find("ButtonBuy/Icon").GetComponent<Image>();
            
            _buyButton = transform.Find("ButtonBuy").GetComponent<Button>();
            _buyButton.onClick.AddListener(OnClickBuy);

            _lockText = transform.Find("LockText").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void InitContent(object content)
        {
        }

        public void UpdateData(params object[] param)
        {
            _config = (TableFarmSeed)param[0];
            _node = (DecoNode)param[1];

            UpdateView();
            
            
            bool isUnLock = FarmModel.Instance.GetLevel() >= _config.UnlockLevel;
            if (isUnLock)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(_buyButton.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Farm_BuySeed, _buyButton.transform as RectTransform, topLayer:topLayer);

                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Farm_BuySeed, "");
            }
        }

        private void OnClickBuy()
        {
            if(!_node.IsOwned)
                return;

            if (!UserData.Instance.CanAford((UserData.ResourceId)_config.BuyType, _config.BuyCost))
            {
                UIPopupNoMoneyController.ShowUI(_config.BuyType);
                return;
            }

            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Farm_BuySeed);
            
            UserData.Instance.ConsumeRes((UserData.ResourceId)_config.BuyType, _config.BuyCost, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.BuyItemFarm,
                data1 = _config.Id.ToString(),
            });

            FarmModel.Instance.CultivateSeed(_node, _config);

            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_CANCEL_TOUCH_NODE);
            
            FarmModel.Instance.TriggerFarmGuide();
        }

        private void UpdateView()
        {
            bool isUnLock = FarmModel.Instance.GetLevel() >= _config.UnlockLevel;
            _lockText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farminfo_tips_unlock"), _config.UnlockLevel));
            _lockText.gameObject.SetActive(!isUnLock);
            _buyButton.gameObject.SetActive(isUnLock);
            
            _time.SetText(CommonUtils.FormatLongToTimeStr(_config.RipeningTime*1000));
            
            _costNum.SetText(_config.BuyCost.ToString());
            _costImage.sprite = UserData.GetResourceIcon(_config.BuyType);

            _image.sprite = FarmModel.Instance.GetFarmIcon(_config.Image);
            
            _rewardNum.SetText(FarmModel.Instance.GetProductItemNum(_config.ProductItem).ToString());

            var config = FarmConfigManager.Instance.GetFarmProductConfig(_config.ProductItem);
            if(config == null)
                return;
            
            //_rewardImage.sprite = FarmModel.Instance.GetFarmIcon(config.Image);
        }
    }
}