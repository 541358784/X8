using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Farm;
using Farm.Model;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class FarmBagItem
    {
        public Image _icon;
        public LocalizeTextMeshProUGUI _numText;
        public Button _sellButton;

        public GameObject _root;

        private int _id;
        private int _num;

        public int Id
        {
            get => _id;
        }
        
        

        private UIPopupFarmBagController _content;

        private TableFarmProduct _config;
        
        public FarmBagItem(UIPopupFarmBagController content, GameObject root, int id, int num)
        {
            _content = content;
            _root = root;

            _id = id;
            _num = num;

            _config = FarmConfigManager.Instance.GetFarmProductConfig(id);
            
            InitView();
        }

        private void InitView()
        {
            _sellButton = _root.transform.GetComponent<Button>();
            _sellButton.onClick.AddListener(() =>
            {
                if(_config.SellPrice <= 0)
                    return;
                
                _content.OnClickItem(_id);
            });

            _icon = _root.transform.Find("Icon").GetComponent<Image>();
            _numText = _root.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _numText.SetText(_num.ToString());

            _icon.sprite = FarmModel.Instance.GetFarmIcon(_config.Image);
        }

        public void UpdateView(int num)
        {
            _num = num;
            _numText.SetText(_num.ToString());
        }
    }
    
    
    public class UIPopupFarmBagController : UIWindowController
    {
        private Button _close;
        private GameObject _item;
        private Button _expansion;
        private LocalizeTextMeshProUGUI _numText;
        
        private List<FarmBagItem> _items = new List<FarmBagItem>();

        private Animator _animator;

        private int[] _propResourceIds = new[]
        {
            (int)UserData.ResourceId.Farm_SFertilizer,
            (int)UserData.ResourceId.Farm_SKettle,
            (int)UserData.ResourceId.Farm_Gear,
            (int)UserData.ResourceId.Farm_Clock,
        };
        
        public override void PrivateAwake()
        {
            _item = transform.Find("Root/Scroll View/Viewport/Content/Item").gameObject;
            _item.gameObject.SetActive(false);

            _numText = transform.Find("Root/numText/numText").GetComponent<LocalizeTextMeshProUGUI>();
            
            _animator = transform.Find("Root/numText").GetComponent<Animator>();
            
            _close = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _close.onClick.AddListener(() =>
            {
                AnimCloseWindow();
            });
            
            _expansion = transform.Find("Root/Button").GetComponent<Button>();
            _expansion.onClick.AddListener(OnClickExpansion);

            Event_RefreshWarehouse(null);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_REFRESH_PRODUCT, Event_RefreshProduct);
            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_REFRESH_WAREHOUSE, Event_RefreshWarehouse);
            
            for (var i = 0; i < _propResourceIds.Length; i++)
            {
                var textMeshProUGUI = transform.Find($"Root/GameObject/Item{i + 1}/Text").GetComponent<LocalizeTextMeshProUGUI>();
                if(textMeshProUGUI != null)
                    textMeshProUGUI.SetText(FarmModel.Instance.GetProductItemNum(_propResourceIds[i]).ToString());
                
                var itemButton = transform.Find($"Root/GameObject/Item{i + 1}/Add").GetComponent<Button>();
                itemButton.onClick.AddListener(() =>
                {
                    UIStoreController.OpenUI("",ShowArea.flash_sale);
                });
            }

            RefreshPropResource();
        }

        private void Start()
        {
            _expansion.gameObject.SetActive(!FarmModel.Instance.IsExpansionWarehouseFill());
            
            foreach (var config in FarmConfigManager.Instance.TableFarmProductList)
            {
                if(!FarmModel.Instance.storageFarm.ProductItems.ContainsKey(config.Id))
                    continue;

                if(FarmModel.Instance.storageFarm.ProductItems[config.Id] == 0)
                    continue;
                
                var item = Instantiate(_item, _item.transform.parent);
                item.gameObject.SetActive(true);
                FarmBagItem mono = new FarmBagItem(this, item, config.Id, FarmModel.Instance.storageFarm.ProductItems[config.Id]);
                _items.Add(mono);
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_REFRESH_PRODUCT, Event_RefreshProduct);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_REFRESH_WAREHOUSE, Event_RefreshWarehouse);
        }

        private void OnClickExpansion()
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupWarehouseUpgrade);
        }

        public void OnClickItem(int id)
        {
            UIManager.Instance.OpenUI(UINameConst.FarmSellUI, id);
        }

        private void RefreshPropResource()
        {
            for (var i = 0; i < _propResourceIds.Length; i++)
            {
                var itemButton = transform.Find($"Root/GameObject/Item{i + 1}/Add").GetComponent<Button>();
                int num = FarmModel.Instance.GetProductItemNum(_propResourceIds[i]);
                itemButton.gameObject.SetActive(false);

                var textMeshProUGUI = transform.Find($"Root/GameObject/Item{i + 1}/Text").GetComponent<LocalizeTextMeshProUGUI>();
                if(textMeshProUGUI != null)
                    textMeshProUGUI.SetText(FarmModel.Instance.GetProductItemNum(_propResourceIds[i]).ToString());
                
                if (num <= 0 && StoreModel.Instance.CanBuyFarmSale(_propResourceIds[i]))
                {
                    itemButton.gameObject.SetActive(true);
                }
            }
        }
        private void Event_RefreshProduct(BaseEvent e)
        {
            if(e == null || e.datas == null || e.datas.Length < 2)
                return;
            
            int productId = (int)e.datas[0];
            int num = (int)e.datas[1];

            RefreshPropResource();
            
            var mono = _items.Find(a => a.Id == productId);
            if(mono == null)
                return;
            
            mono.UpdateView(num);
            mono._root.gameObject.SetActive(num > 0);

            Event_RefreshWarehouse(null);
        }

        private void Event_RefreshWarehouse(BaseEvent e)
        {
            if (e != null && e.datas != null)
            {
                _animator.Play("appear", 0, 0);
            }
            
            _expansion.gameObject.SetActive(!FarmModel.Instance.IsExpansionWarehouseFill());
            
            _numText.SetText(FarmModel.Instance.WarehouseOccupyNum() + "/" +FarmModel.Instance.WarehouseNum());
        }
    }
}