using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGiftBagDoubleMainController:UIWindowController
{
    public static UIPopupGiftBagDoubleMainController Open(StorageGiftBagDouble storage)
    {
        if (storage == null)
            return null;
        if (storage.IsFinish())
            return null;
        return UIManager.Instance.OpenUI(UINameConst.UIPopupGiftBagDoubleMain, storage) as
            UIPopupGiftBagDoubleMainController;
    }

    public StorageGiftBagDouble Storage;
    public Button CloseBtn;
    public LocalizeTextMeshProUGUI PercentText;
    public LocalizeTextMeshProUGUI TimeText;
    public List<ProductView> ProductList = new List<ProductView>();
    private Button BuyAllBtn;
    private Text BuyAllPriceText;
    private LocalizeTextMeshProUGUI BuyAllDiscountText;
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        PercentText = transform.Find("Root/TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
        for (var i = 1; transform.Find("Root/Gift"+i); i++)
        {
            ProductList.Add(transform.Find("Root/Gift"+i).gameObject.AddComponent<ProductView>());
        }
        EventDispatcher.Instance.AddEvent<EventGiftBagDoubleBuyStateChange>(OnBuy);
    }

    public void OnBuy(EventGiftBagDoubleBuyStateChange evt)
    {
        if (BuyAllBtn)
        {
            var group = Storage.GetGroupConfig();
            var buyState = false;
            for (var i = 0; i < group.ProductList.Count; i++)
            {
                if (Storage.BuyState.Contains(group.ProductList[i]))
                {
                    buyState = true;
                }
            }
            BuyAllBtn.interactable = !buyState;   
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventGiftBagDoubleBuyStateChange>(OnBuy);
    }

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetLeftTimeText());
        if (Storage.IsTimeOut())
            AnimCloseWindow();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageGiftBagDouble;
        var productList = Storage.GetProductList();
        for (var i = 0; i < ProductList.Count; i++)
        {
            if (productList.Count <= i)
            {
                ProductList[i].gameObject.SetActive(false);
            }
            else
            {
                ProductList[i].Init(productList[i],this);
            }
        }
        PercentText.SetText(productList.Last().TagText);
        var group = Storage.GetGroupConfig();
        if (group != null && group.BuyAll)
        {
            BuyAllBtn = transform.Find("Root/Button").GetComponent<Button>();
            BuyAllBtn.gameObject.SetActive(true);
            BuyAllBtn.onClick.AddListener(() =>
            {
                var cutBuyState = false;
                for (var i = 0; i < group.ProductList.Count; i++)
                {
                    if (Storage.BuyState.Contains(group.ProductList[i]))
                    {
                        cutBuyState = true;
                    }
                }
                if (cutBuyState)
                    return;
                StoreModel.Instance.Purchase(group.BuyAllShopId);
            });
            var buyState = false;
            for (var i = 0; i < group.ProductList.Count; i++)
            {
                if (Storage.BuyState.Contains(group.ProductList[i]))
                {
                    buyState = true;
                }
            }
            BuyAllBtn.interactable = !buyState;
            BuyAllPriceText = transform.Find("Root/Button/Text").GetComponent<Text>();
            BuyAllDiscountText = transform.Find("Root/Button/Text1").GetComponent<LocalizeTextMeshProUGUI>();   
            BuyAllPriceText.text = StoreModel.Instance.GetPrice(group.BuyAllShopId);
            BuyAllDiscountText.SetTermFormats(group.Discount.ToString());
            
            transform.Find("Root/Button/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(group.BuyAllShopId));

        }
        else
        {
            transform.Find("Root/Button")?.gameObject.SetActive(false);
            transform.Find("Root/Button/Vip").gameObject.SetActive(false);
        }
    }

    public class ProductView : MonoBehaviour
    {
        public Transform DefaultItem;
        public List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
        public Transform FinishGroup;
        public Button BuyBtn;
        public Text PriceText;
        public GiftBagDoubleProductConfig ProductConfig;
        public UIPopupGiftBagDoubleMainController Controller;
        
        private void Awake()
        {
            DefaultItem = transform.Find("Item");
            DefaultItem.gameObject.SetActive(false);
            FinishGroup = transform.Find("ButtonFinish");
            BuyBtn = transform.Find("Button").GetComponent<Button>();
            BuyBtn.onClick.AddListener(() =>
            {
                var buyState = Controller.Storage.BuyState.Contains(ProductConfig.Id);
                if (!buyState)
                    StoreModel.Instance.Purchase(ProductConfig.ShopId);
            });
            PriceText = transform.Find("Button/Root/Text").GetComponent<Text>();
            EventDispatcher.Instance.AddEvent<EventGiftBagDoubleBuyStateChange>(OnBuy);
        }

        public void OnBuy(EventGiftBagDoubleBuyStateChange evt)
        {
            if (evt.Product == ProductConfig)
            {
                var buyState = Controller.Storage.BuyState.Contains(ProductConfig.Id);
                BuyBtn.gameObject.SetActive(!buyState);
                FinishGroup.gameObject.SetActive(buyState);
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventGiftBagDoubleBuyStateChange>(OnBuy);
        }

        public void Init(GiftBagDoubleProductConfig config,UIPopupGiftBagDoubleMainController controller)
        {
            gameObject.SetActive(true);
            ProductConfig = config;
            Controller = controller;
            PriceText.text = StoreModel.Instance.GetPrice(ProductConfig.ShopId);
            var buyState = Controller.Storage.BuyState.Contains(ProductConfig.Id);
            BuyBtn.gameObject.SetActive(!buyState);
            FinishGroup.gameObject.SetActive(buyState);
            foreach (var rewardItem in RewardItemList)
            {
                DestroyImmediate(rewardItem.gameObject);
            }
            RewardItemList.Clear();
            var rewards = CommonUtils.FormatReward(ProductConfig.RewardId, ProductConfig.RewardNum);
            foreach (var reward in rewards)
            {
                var rewardItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                    .AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(reward);
                RewardItemList.Add(rewardItem);
            }
            
            transform.Find("Button/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(ProductConfig.ShopId));

        }
    }
}