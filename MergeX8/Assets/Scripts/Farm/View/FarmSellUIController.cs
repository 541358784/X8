using System;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API.Protocol;
using Farm.FarmFly;
using Farm.Model;
using Gameplay;
using UnityEngine.UI;

namespace Farm.View
{
    public class FarmSellUIController: UIWindowController
    {
        private Button _addBtn;
        private Button _subBtn;

        private Image _icon;
        private Image _sellIcon;
        
        private LocalizeTextMeshProUGUI _tileText;
        private LocalizeTextMeshProUGUI _nameText;
        private LocalizeTextMeshProUGUI _numText;
        private LocalizeTextMeshProUGUI _sellPriceText;

        private Button _sellBtn;

        private int _productId;
        private TableFarmProduct _config;
        private int _productNum;

        private int sellNum = 1;
        
        public override void PrivateAwake()
        {
            _addBtn = transform.Find("AddBtn").GetComponent<Button>();
            _addBtn.onClick.AddListener(OnClickAddButton);
            
            _subBtn = transform.Find("MinusBtn").GetComponent<Button>();
            _subBtn.onClick.AddListener(OnClickSubButton);
            
            _sellBtn = transform.Find("Button").GetComponent<Button>();
            _sellBtn.onClick.AddListener(OnClickSellButton);
            
            _icon = transform.Find("Item/Icon").GetComponent<Image>();
            _sellIcon = transform.Find("Button/Icon").GetComponent<Image>();
            
            _tileText = transform.Find("TextName").GetComponent<LocalizeTextMeshProUGUI>();
            _nameText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _numText = transform.Find("Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _sellPriceText = transform.Find("Button/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        }

        private void Start()
        {
            CommonUtils.SetShieldButUnEnable(_addBtn.gameObject);
            CommonUtils.SetShieldButUnEnable(_subBtn.gameObject);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _productId = (int)objs[0];

            _config = FarmConfigManager.Instance.GetFarmProductConfig(_productId);

            _productNum = FarmModel.Instance.GetProductItemNum(_productId);

            _sellIcon.sprite = UserData.GetResourceIcon(_config.SellType);
            _tileText.SetTerm(_config.NameKey);

            _icon.sprite = FarmModel.Instance.GetFarmIcon(_config.Image);
            
            UpdateView();
        }

        private void OnClickAddButton()
        {
            sellNum++;
            sellNum = Math.Min(sellNum, _productNum);

            UpdateView();
        }

        private void OnClickSubButton()
        {
            sellNum--;
            sellNum = Math.Max(sellNum, 1);

            UpdateView();
        }

        private void OnClickSellButton()
        {
            int exp = sellNum * _config.SellPrice;
            FarmModel.Instance.ConsumeProductItem(_productId, sellNum);
            FarmModel.Instance.AddExp(exp);
            
            GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Farm_Exp, exp, (ulong)FarmModel.Instance.storageFarm.Exp, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SaleItemFarm,
                data1 = _productId+","+sellNum,
            });
            
            FarmFlyManager.Instance.FlyItem((int)UserData.ResourceId.Farm_Exp, exp, _sellIcon.transform.position, () =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_REFRESH_LEVEL_EXP);
            });
            AnimCloseWindow();
        }

        private void UpdateView()
        {
            _numText.SetText(sellNum.ToString());
            _sellPriceText.SetText((sellNum*_config.SellPrice).ToString());
        }
    }
}