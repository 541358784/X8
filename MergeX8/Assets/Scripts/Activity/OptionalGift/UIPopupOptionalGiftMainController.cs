using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.OptionalGift;
using Gameplay.UI.Store.Vip.Model;
using Google.Protobuf.WellKnownTypes;
using OptionalGift;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupOptionalGiftMainController : UIWindowController
{
    private Button _buttonClose;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _buttonBuy;
    private Text _buttonBuyText;
    private OptionalGiftActivityConfig _optionalGiftActivityConfig;
    private List<OptionalGiftSelectItem> _selectItems;
    
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
        _selectItems = new List<OptionalGiftSelectItem>();
        _optionalGiftActivityConfig = OptionGiftModel.Instance.GetOptionalGiftActivityConfig();
        _buttonBuyText.text = StoreModel.Instance.GetPrice(_optionalGiftActivityConfig.ShopId);
        for (int i = 0; i < 3; i++)
        {
            var item=transform.Find("Root/RewardGroup/" + (i + 1));
            OptionalGiftSelectItem selectItem = item.GetOrCreateComponent<OptionalGiftSelectItem>();
            selectItem.Init(i,_optionalGiftActivityConfig);
            _selectItems.Add(selectItem);
        }

        _buttonBuy.interactable = OptionGiftModel.Instance.IsSelectAll();
        
        transform.Find("Root/Button/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_optionalGiftActivityConfig.ShopId));

    }
    
    private void OnSelect(BaseEvent obj)
    {
        _buttonBuy.interactable = OptionGiftModel.Instance.IsSelectAll();
    }
    
    
    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(_optionalGiftActivityConfig.ShopId);
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
    private void RefreshTime()
    {
        _timeText.SetText(OptionGiftModel.Instance.GetActivityLeftTimeString());
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);

    }
}
