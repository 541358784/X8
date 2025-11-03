using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIZumaGiftBagController : UIWindowController
{
    public static UIZumaGiftBagController Instance;
    private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private List<GiftBagItem> ItemList = new List<GiftBagItem>();
    public ZumaModel Model => ZumaModel.Instance;

    public static UIZumaGiftBagController Open()
    {
        if (Instance && Instance.gameObject.activeSelf)
            return Instance;
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupZumaGift) as
            UIZumaGiftBagController;
        return Instance;
    }
    
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTimeText",0,1);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        foreach (var config in Model.GiftBagConfig)
        {
            var item = transform.Find("Root/Gift"+config.Id).gameObject.AddComponent<GiftBagItem>();
            item.Init(config);
        }
    }

    public void UpdateTimeText()
    {
        TimeText.SetText(Model.CurStorageZumaWeek.GetLeftTimeText());
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
    }

    public class GiftBagItem : MonoBehaviour
    {
        public ZumaModel Model => ZumaModel.Instance;
        private Transform DefaultItem;
        // private Image Icon;
        // private LocalizeTextMeshProUGUI NumText;
        private Button BuyBtn;
        private Text PriceText;
        private ZumaGiftBagConfig Config;
        private TableShop ShopConfig;
        private void Awake()
        {
            DefaultItem = transform.Find("Item");
            DefaultItem.gameObject.SetActive(false);
            BuyBtn = transform.Find("Button").GetComponent<Button>();
            BuyBtn.onClick.AddListener(() =>
            {
                StoreModel.Instance.Purchase(ShopConfig.id);
            });
            PriceText = transform.Find("Button/Text").GetComponent<Text>();
        }

        public void Init(ZumaGiftBagConfig config)
        {
            Config = config;
            ShopConfig = GlobalConfigManager.Instance.GetTableShopByID(Config.ShopId);
            var rewards = CommonUtils.FormatReward(Config.RewardId, Config.RewardNum);
            for (var i = 0; i < rewards.Count; i++)
            {
                var item = Instantiate(DefaultItem, DefaultItem.parent);
                item.gameObject.SetActive(true);
                var Icon = item.Find("Icon").GetComponent<Image>();
                var NumText = item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                Icon.sprite = UserData.GetResourceIcon(rewards[i].id);
                NumText.SetText(rewards[i].count.ToString());
            }
            PriceText.text = StoreModel.Instance.GetPrice(ShopConfig.id);
        }
    }
}