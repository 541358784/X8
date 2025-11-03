using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.MixMaster;
using Gameplay.UI.Store.Vip.Model;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMixMasterShopController : UIWindowController
{
    public static UIPopupMixMasterShopController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMixMasterShop) as UIPopupMixMasterShopController;
    }
    private Button _buttonClose;
    private Button _buttonBuy;
    private Text _buttonBuyText;
    private MixMasterGiftBagConfig _MixMasterActivityConfig;
    public List<MixMasterSelectItem> _selectItems;
    
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonBuy = GetItem<Button>("Root/Button");
        _buttonBuy.onClick.AddListener(OnBtnBuy);
        _buttonBuyText = GetItem<Text>("Root/Button/Root/Text");
        CommonUtils.SetShieldButUnEnable(_buttonBuy.gameObject);
        EventDispatcher.Instance.AddEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _selectItems = new List<MixMasterSelectItem>();
        _MixMasterActivityConfig = MixMasterModel.Instance.GiftBagConfig[0];
        _buttonBuyText.text = StoreModel.Instance.GetPrice(_MixMasterActivityConfig.ShopId);
        for (int i = 0; i < 3; i++)
        {
            var item=transform.Find("Root/RewardGroup/" + (i + 1));
            MixMasterSelectItem selectItem = item.GetOrCreateComponent<MixMasterSelectItem>();
            selectItem.Init(i,_MixMasterActivityConfig,this);
            _selectItems.Add(selectItem);
        }

        _buttonBuy.interactable = MixMasterModel.Instance.IsSelectAll();
        transform.Find("Root/Button/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_MixMasterActivityConfig.ShopId));
    }
    
    private void OnSelect(BaseEvent obj)
    {
        _buttonBuy.interactable = MixMasterModel.Instance.IsSelectAll();
    }
    
    
    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(_MixMasterActivityConfig.ShopId);
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);

    }
}
