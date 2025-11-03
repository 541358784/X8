using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKapiScrewShopController : UIWindowController
{
    private Button _buttonClose;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _buttonBuy;
    private Text _buttonBuyText;
    private KapiScrewGiftBagConfig _KapiScrewOptionalGiftActivityConfig;
    private List<KapiScrewOptionalGiftSelectItem> _selectItems;
    
    public static UIPopupKapiScrewShopController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKapiScrewShop) as UIPopupKapiScrewShopController;
    }
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _buttonBuy = GetItem<Button>("Root/Button");
        _buttonBuy.onClick.AddListener(OnBtnBuy);
        _buttonBuyText = GetItem<Text>("Root/Button/Root/Text");
        InvokeRepeating("RefreshTime",0,1);
        CommonUtils.SetShieldButUnEnable(_buttonBuy.gameObject);
        EventDispatcher.Instance.AddEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _selectItems = new List<KapiScrewOptionalGiftSelectItem>();
        _KapiScrewOptionalGiftActivityConfig = KapiScrewModel.Instance.GetOptionalGiftActivityConfig();
        _buttonBuyText.text = StoreModel.Instance.GetPrice(_KapiScrewOptionalGiftActivityConfig.ShopId);
        for (int i = 0; i < 3; i++)
        {
            var item=transform.Find("Root/RewardGroup/" + (i + 1));
            KapiScrewOptionalGiftSelectItem selectItem = item.GetOrCreateComponent<KapiScrewOptionalGiftSelectItem>();
            selectItem.Init(i,_KapiScrewOptionalGiftActivityConfig);
            _selectItems.Add(selectItem);
        }

        _buttonBuy.interactable = KapiScrewModel.Instance.IsSelectAll();
        transform.Find("Root/Button/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_KapiScrewOptionalGiftActivityConfig.ShopId));
    }
    
    private void OnSelect(BaseEvent obj)
    {
        _buttonBuy.interactable = KapiScrewModel.Instance.IsSelectAll();
    }
    
    
    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(_KapiScrewOptionalGiftActivityConfig.ShopId);
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
    private void RefreshTime()
    {
        _timeText.SetText(KapiScrewModel.Instance.GetActivityLeftTimeString());
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);

    }
}
