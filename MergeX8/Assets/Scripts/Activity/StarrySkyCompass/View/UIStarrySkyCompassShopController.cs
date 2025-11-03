using System;
using System.Collections.Generic;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStarrySkyCompassShopController:UIWindowController
{
    // private LocalizeTextMeshProUGUI TimeText;
    private StorageStarrySkyCompass Storage;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI ScoreText;
    private List<ShopItem> ShopItemList = new List<ShopItem>();
    private StarrySkyCompassModel Model => StarrySkyCompassModel.Instance;
    public override void PrivateAwake()
    {
        // TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        // InvokeRepeating("UpdateTime",0f,01f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        ScoreText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        EventDispatcher.Instance.AddEvent<EventStarrySkyCompassScoreChange>(OnScoreChange);
        EventDispatcher.Instance.AddEvent<EventStarrySkyCompassBuyShopItem>(OnBuyStoreItem);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassScoreChange>(OnScoreChange);
        EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassBuyShopItem>(OnBuyStoreItem);
    }

    public void OnBuyStoreItem(EventStarrySkyCompassBuyShopItem evt)
    {
        UpdateScoreText();
        foreach (var shopItem in ShopItemList)
        {
            shopItem.UpdateBtnState();
        }
    }

    public void OnScoreChange(EventStarrySkyCompassScoreChange evt)
    {
        UpdateScoreText();
        foreach (var shopItem in ShopItemList)
        {
            shopItem.UpdateBtnState();
        }
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageStarrySkyCompass;
        UpdateScoreText();
        var shopConfigList = StarrySkyCompassModel.Instance.ShopConfig;
        foreach (var shopConfig in shopConfigList)
        {
            var shopItem = transform.Find("Root/GiftGroup/" + shopConfig.Id).gameObject.AddComponent<ShopItem>();
            shopItem.Init(shopConfig,this);
            ShopItemList.Add(shopItem);
        }

        var shieldBtn = transform.gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var btn in shieldBtn)
        {
            btn.isUse = false;
        }
    }

    public void OnClickBuyBtn(StarrySkyCompassShopConfig shopConfig)
    {
        if (Storage.Score < shopConfig.Price)
            return;
        Model.AddScore(-shopConfig.Price,"BuyShopItem");
        EventDispatcher.Instance.SendEventImmediately(new EventStarrySkyCompassBuyShopItem(shopConfig));
        var rewards = CommonUtils.FormatReward(shopConfig.RewardId, shopConfig.RewardNum);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.StarrySkyCompassGet
        };
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            reason);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStarrySkyCompassRadishExchange,shopConfig.Id.ToString(),Storage.Score.ToString());
    }

    public void UpdateScoreText()
    {
        ScoreText.SetText(Storage.Score.ToString());
    }

    public void OnClickCloseBtn()
    {
        CloseBtn.interactable = false;
        AnimCloseWindow();
    }

    // public void UpdateTime()
    // {
    //     TimeText.SetText(Storage.GetLeftTimeText());
    // }
    public static UIStarrySkyCompassShopController Instance;
    public static UIStarrySkyCompassShopController Open(StorageStarrySkyCompass storageStarrySkyCompass)
    {
        Instance = UIManager.Instance.OpenUI(UINameConst.UIStarrySkyCompassShop, storageStarrySkyCompass) as
            UIStarrySkyCompassShopController;
        return Instance;
    }

    public class ShopItem : MonoBehaviour
    {
        private UIStarrySkyCompassShopController Controller;
        private StarrySkyCompassShopConfig ShopConfig;
        // private CommonRewardItem RewardItem;
        private LocalizeTextMeshProUGUI RewardCountText;
        private Button BuyButton;
        private LocalizeTextMeshProUGUI PriceText;
        private LocalizeTextMeshProUGUI GreyPriceText;
        private StorageStarrySkyCompass Storage => Controller.Storage;
        private void Awake()
        {
            // RewardItem = transform.Find("Item").gameObject.AddComponent<CommonRewardItem>();
            RewardCountText = transform.Find("Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
            BuyButton = transform.Find("Button").gameObject.GetComponent<Button>();
            BuyButton.onClick.AddListener(() =>
            {
                Controller.OnClickBuyBtn(ShopConfig);
            });
            PriceText = transform.Find("Button/Text").GetComponent<LocalizeTextMeshProUGUI>();
            GreyPriceText = transform.Find("Button/GreyText").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void Init(StarrySkyCompassShopConfig shopConfig, UIStarrySkyCompassShopController controller)
        {
            Controller = controller;
            ShopConfig = shopConfig;
            BuyButton.interactable = Storage.Score >= ShopConfig.Price;
            PriceText.SetText(ShopConfig.Price.ToString());
            GreyPriceText.SetText(ShopConfig.Price.ToString());
            var rewards = CommonUtils.FormatReward(shopConfig.RewardId, shopConfig.RewardNum);
            RewardCountText.SetText("x"+rewards[0].count);
            // RewardItem.Init(rewards[0]);
        }

        public void UpdateBtnState()
        {
            BuyButton.interactable = Storage.Score >= ShopConfig.Price;
        }
    }
}