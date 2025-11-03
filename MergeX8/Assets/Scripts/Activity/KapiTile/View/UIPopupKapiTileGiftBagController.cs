using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine.UI;

public class UIPopupKapiTileGiftBagController : UIWindowController
{
    public static UIPopupKapiTileGiftBagController Instance;

    public static UIPopupKapiTileGiftBagController Open()
    {
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupKapiTileGiftBag) as
            UIPopupKapiTileGiftBagController;
        return Instance;
    }
    private LocalizeTextMeshProUGUI _timeText;
    private Button _button;
    private Text _text;
    private LocalizeTextMeshProUGUI _text1;
    private List<KapiTileGiftBagItem> _items;
    private Button _buttonClose;

    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _items = new List<KapiTileGiftBagItem>();
        for (int i = 1; i < 4; i++)
        {
            var tran = transform.Find("Root/Gift" + i);
            var item = tran.gameObject.AddComponent<KapiTileGiftBagItem>();
            _items.Add(item);
        }

        InvokeRepeating("UpdateTime", 0, 1);
        _button = GetItem<Button>("Root/Button");
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(() => { AnimCloseWindow(); });
        _button.onClick.AddListener(OnClickBtn);
        _text = GetItem<Text>("Root/Button/Text");
        _text1 = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text1");
        EventDispatcher.Instance.AddEventListener(EventEnum.THREE_GIFT_PURCHASE_SUCCESS, OnPurchase);
    }

    private void OnPurchase(BaseEvent obj)
    {
        AnimCloseWindow();
    }

    private void OnClickBtn()
    {
        StoreModel.Instance.Purchase(KapiTileModel.Instance.GlobalConfig.TotalShopId);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.THREE_GIFT_PURCHASE_SUCCESS, OnPurchase);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var config = KapiTileModel.Instance.GlobalConfig;
        for (int i = 0; i < config.PackageList.Count; i++)
        {
            var giftConfig = KapiTileModel.Instance.GiftBagConfig.Find(a => a.Id == config.PackageList[i]);
            _items[i].Init(giftConfig);
        }

        _text.text = StoreModel.Instance.GetPrice(config.TotalShopId);
        _text1.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_3in1_deal_desc"),
            config.Discount));
        
        transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(config.TotalShopId));
    }

    public void UpdateTime()
    {
        _timeText.SetText(KapiTileModel.Instance.GetActivityLeftTimeString());
    }
}