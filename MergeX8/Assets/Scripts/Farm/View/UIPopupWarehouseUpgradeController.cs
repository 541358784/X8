using System;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API.Protocol;
using Farm.Model;
using Farm.Order;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class UIPopupWarehouseUpgradeController : UIWindowController
    {
        private Button _closeButton;
        private Button _buyButton;

        private GameObject item;

        private bool _isEnough;
        private TableFarmWarehouse _config;

        private GameObject _noEnoughObj;
        
        public override void PrivateAwake()
        {
            item = transform.Find("Root/Content/Item").gameObject;
            item.gameObject.SetActive(false);
            
            _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _closeButton.onClick.AddListener(()=>AnimCloseWindow());
            
            _buyButton = transform.Find("Root/Button").GetComponent<Button>();
            _buyButton.onClick.AddListener(OnClickBuy);

            _noEnoughObj = transform.Find("Root/Text").gameObject;
        }

        private void Start()
        {
            _config = FarmModel.Instance.GetExpansionWarehouseConfig();
            if(_config == null)
                return;

            _isEnough = true;
            for (var i = 0; i < _config.OpenCostIds.Count; i++)
            {
                int id = _config.OpenCostIds[i];
                int num = _config.OpenCostNums[i];
                
                var obj = Instantiate(item, item.transform.parent);
                obj.gameObject.SetActive(true);

                int itemNum = FarmModel.Instance.GetProductItemNum(id);
                if (itemNum < num)
                    _isEnough = false;
                
                var productConfig = FarmConfigManager.Instance.GetFarmProductConfig(id);
                if(productConfig != null)
                    obj.transform.Find("Icon").GetComponent<Image>().sprite = FarmModel.Instance.GetFarmIcon(productConfig.Image);
                obj.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(itemNum + "/" + num);

                var button = obj.transform.Find("Add").GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    AnimCloseWindow(() =>
                    {
                        UIStoreController.OpenUI("",ShowArea.flash_sale);
                    });
                });
                button.gameObject.SetActive(StoreModel.Instance.CanBuyFarmSale(id));
            }
            
            _noEnoughObj.gameObject.SetActive(!_isEnough);
            _buyButton.gameObject.SetActive(_isEnough);
        }

        private void OnClickBuy()
        {
            if(!_isEnough && !FarmModel.Instance.Debug_OpenModule)
                return;
            
            AnimCloseWindow();
            
            if(_config == null)
                return;

            FarmModel.Instance.ExpansionWarehouse(_config.Id);
            
            for (var i = 0; i < _config.OpenCostIds.Count; i++)
            {    
                int id = _config.OpenCostIds[i];
                int num = _config.OpenCostNums[i];
                
                FarmModel.Instance.ConsumeProductItem(id, num);
                
                GameBIManager.Instance.SendItemChangeEvent((UserData.ResourceId)id, -num, (ulong)FarmModel.Instance.GetProductItemNum(id), new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.StoreUpdateFarm,
                    data1 = FarmModel.Instance.WarehouseNum().ToString(),
                });
                
            }
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_REFRESH_WAREHOUSE, true);
        }
    }
}