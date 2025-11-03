using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Merge.Order;
using SomeWhere;
using UnityEngine.UI;

public class UIStoreExchangeItem : UIStoreBaseItem
{
    public override UIStoreItemType ItemType { get; } = UIStoreItemType.Exchange;

    protected override Button BuyButton
    {
        get { return _btnBuy; }
    }

    private Button _btnBuy;

    private Image _icon;
    private Image _background;
    private Image _buyIcon;
    private Text _priceText;

    private LocalizeTextMeshProUGUI _contentText;
    private DailyShop _dailyShop;

    protected override void PrivateAwake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _background = transform.Find("BG").GetComponent<Image>();
        _buyIcon = transform.Find("ButtonBuy/Icon").GetComponent<Image>();
        _btnBuy = transform.Find("ButtonBuy").GetComponent<Button>();
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _contentText = transform.Find("ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        _btnBuy.onClick.AddListener(OnBtnBuy);
    }

    private bool SetPrice()
    {
        bool canBuy = false;
        var storageItem = StoreModel.Instance.GetStorageItem(ID);
        if (_dailyShop == null)
            return canBuy;
        switch (_dailyShop.Price[0])
        {
            case 1: //钻石
                _priceText.text = string.Format("{0}", _dailyShop.Price[1] + storageItem.PriceAdd);
                canBuy = storageItem.BuyCount < _dailyShop.Amount;
                if (!canBuy)
                    _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
                _btnBuy.gameObject.SetActive(true);
                _btnBuy.interactable = canBuy;
                _buyIcon.sprite = UserData.GetResourceIcon(UserData.ResourceId.Diamond);
                break;
            case 2: // 金币
                _priceText.text = string.Format("{0}", _dailyShop.Price[1] + storageItem.PriceAdd);
                canBuy = storageItem.BuyCount < _dailyShop.Amount;
                if (!canBuy)
                    _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
                _btnBuy.interactable = canBuy;
                _btnBuy.gameObject.SetActive(true);
                _buyIcon.sprite = UserData.GetResourceIcon(UserData.ResourceId.Coin);
                break;
        }

        return canBuy;
    }

    private void OnBtnBuy()
    {
        var extras = new Dictionary<string, string>();
        if (_dailyShop != null)
        {
            var storageItem = StoreModel.Instance.GetStorageItem(ID);
            int price = _dailyShop.Price[1] + storageItem.PriceAdd;

            switch (_dailyShop.Price[0])
            {
                case 1: //钻石
                    extras.Clear();
                    extras.Add("type", "diamond");
                    extras.Add("amount", price.ToString());
                    if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
                    {
                        var reson = new GameBIManager.ItemChangeReasonArgs()
                            {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy};
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess, OpenSrc,
                            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), ID.ToString(), extras);
                        UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, price, reson);
                        StoreModel.Instance.AddItemCount(ID, _dailyShop);
                        AddReward();
                        SetPrice();
                    }
                    else
                    {
                        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                        {
                            DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_info_text14"),
                            HasCloseButton = true,
                        });
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreLackDiamondPop);
                    }

                    break;
                case 2: // 金币
                    extras.Clear();
                    extras.Add("type", "coin");
                    extras.Add("amount", price.ToString());
                    if (UserData.Instance.CanAford(UserData.ResourceId.Coin, price))
                    {
                        var reson = new GameBIManager.ItemChangeReasonArgs()
                            {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy};
                        UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, price, reson);
                        StoreModel.Instance.AddItemCount(ID, _dailyShop);
                        AddReward();
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess, OpenSrc,
                            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), ID.ToString(), extras);
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess, OpenSrc,
                            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), ID.ToString(), extras);
                        SetPrice();
                    }
                    else
                    {
                        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                        {
                            DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_info_text14"),
                            HasCloseButton = true,
                        });
                    }

                    break;
            }

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuy, OpenSrc,
                MainOrderManager.Instance.GetCurMaxTaskID().ToString(), ID.ToString(), extras);
        }
        else
        {
            extras.Clear();
            extras.Add("type", "diamond");
            extras.Add("amount", Cfg.diamondPrice.ToString());
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, Cfg.diamondPrice))
            {
                var reason = new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug
                };
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, Cfg.diamondPrice, reason);
                AddReward();
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuySuccess,
                    OpenSrc,
                    MainOrderManager.Instance.GetCurMaxTaskID().ToString(), ID.ToString(), extras);
            }
            else
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreLackDiamondPop);
                CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                {
                    DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_info_text14"),
                    HasCloseButton = true,
                });
            }

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuy, OpenSrc,
                MainOrderManager.Instance.GetCurMaxTaskID().ToString(), ID.ToString(), extras);
        }
    }

    private void AddReward()
    {
        GameBIManager.ItemChangeReasonArgs reasonArgs = new GameBIManager.ItemChangeReasonArgs();


        UserData.ResourceId resId = UserData.ResourceId.None;
        switch (showArea)
        {
            case ShowArea.coin_shop:
                resId = UserData.ResourceId.Coin;
                // reasonArgs.reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyCoin;
                break;
            case ShowArea.energy:
                resId = UserData.ResourceId.Energy;
                reasonArgs.reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy;
                break;
        }

        if (resId == UserData.ResourceId.None)
            return;

        UserData.Instance.AddRes((int) resId, Cfg.amount, reasonArgs, false);
        FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.currencyController, resId, Cfg.amount,
            transform.position, 0.8f, true, true, 0.15f);
    }

    protected override void PrivateStart()
    {
    }

    public override void Refresh()
    {
        _dailyShop = AdConfigHandle.Instance.GetDailyShops().Find(x => x.ShopItemId == ID);
        _icon.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, Cfg.image);
        _icon.SetNativeSize();

        // if (!string.IsNullOrEmpty(Cfg.background))
        //     _background.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, Cfg.background);
        if (_dailyShop != null)
        {
            SetPrice();
        }
        else
        {
            _priceText.text = Cfg.diamondPrice.ToString();
        }

        _contentText.SetText(Cfg.amount.ToString());
    }
}