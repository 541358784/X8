using System;
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
    public class MachineCell : MonoBehaviour, IInitContent
    {
        private TableFarmMachineOrder _config;
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
            int id = (int)param[0];
            _node = (DecoNode)param[1];

            _config = FarmConfigManager.Instance.TableFarmMachineOrderList.Find(a => a.Id == id);
            
            UpdateView();
        }

        private void OnClickBuy()
        {
            if(!_node.IsOwned)
                return;
            
            if(_config == null)
                return;

            if (!FarmModel.Instance.HavEnoughProduct(_config.ProductCostId, _config.ProductCostNum))
            {
                UIPopupFarmNoticeController.OpenNotice(_config.ProductCostId, _config.ProductCostNum, _config.ProductItem);
                return;
            }

            FarmModel.Instance.ConsumeProductItem(_config.ProductCostId, _config.ProductCostNum);
            FarmModel.Instance.Production(_node, _config);
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_CANCEL_TOUCH_NODE);
        }

        private void UpdateView()
        {
            bool isUnLock = FarmModel.Instance.GetLevel() >= _config.UnLockLevel;
            _lockText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farminfo_tips_unlock"), _config.UnLockLevel));
            _lockText.gameObject.SetActive(!isUnLock);
            _buyButton.gameObject.SetActive(isUnLock);
            
            _time.SetText(CommonUtils.FormatLongToTimeStr(_config.RipeningTime*1000));
            _costNum.SetText(_config.ProductCostNum.ToString());
            
            var costConfig = FarmConfigManager.Instance.GetFarmProductConfig(_config.ProductCostId);
            if (costConfig != null)
            {
                _costImage.sprite = FarmModel.Instance.GetFarmIcon(costConfig.Image);
            }
             
            _rewardNum.SetText(FarmModel.Instance.GetProductItemNum(_config.ProductItem).ToString());
            
            var productConfig = FarmConfigManager.Instance.GetFarmProductConfig(_config.ProductItem);
            if (productConfig != null)
            {
                _image.sprite = FarmModel.Instance.GetFarmIcon(productConfig.Image);
                //_rewardImage.sprite = FarmModel.Instance.GetFarmIcon(productConfig.Image);
            }
        }
    }
}