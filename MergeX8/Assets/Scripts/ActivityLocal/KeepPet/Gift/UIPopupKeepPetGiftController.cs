
using System;
using DragonPlus;
using UnityEngine.UI;

public class UIPopupKeepPetGiftController : UIWindowController
{
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _itemCount;
    private Button _buyBtn;
    private Text _buyBtnText;
    private KeepPetStoreConfig currentConfig;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnClose);
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _itemCount = GetItem<LocalizeTextMeshProUGUI>("Root/RewardGroup/Item1/Text");
        _buyBtn = GetItem<Button>("Root/BuyButton");
        _buyBtn.onClick.AddListener(OnBtnBuy);
        _buyBtnText = GetItem<Text>("Root/BuyButton/Text");
        InvokeRepeating("RefreshTime",0,1);
        EventDispatcher.Instance.AddEventListener(EventEnum.KEEPPET_GIFT_PURCHASE,OnPurchase);
        Init();
    }

    private void OnPurchase(BaseEvent obj)
    {
        AnimCloseWindow();
    }

    private void OnBtnBuy()
    {
        StoreModel.Instance.Purchase(currentConfig.ShopId);
    }

    public void Init()
    {
        currentConfig= KeepPetModel.Instance.GetCurrentKeepPetStoreConfig();
        _itemCount.SetText(currentConfig.RewardCount[0].ToString());
        _buyBtnText.text = StoreModel.Instance.GetPrice(currentConfig.ShopId);
    }

    public void RefreshTime()
    {
        _timeText.SetText(KeepPetModel.Instance.GetGiftRestTimeString());
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.KEEPPET_GIFT_PURCHASE,OnPurchase);
    }
}
