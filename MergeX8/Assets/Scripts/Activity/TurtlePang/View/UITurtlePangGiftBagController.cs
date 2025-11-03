using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UITurtlePangGiftBagController : UIWindowController
{
    public static UITurtlePangGiftBagController Instance;
    private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private List<GiftBagItem> ItemList = new List<GiftBagItem>();
    public TurtlePangModel Model => TurtlePangModel.Instance;

    public static UITurtlePangGiftBagController Open()
    {
        if (Instance && Instance.gameObject.activeSelf)
            return Instance;
        Instance = UIManager.Instance.OpenUI(UINameConst.UITurtlePangGiftBag) as
            UITurtlePangGiftBagController;
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
        TimeText.SetText(Model.Storage.GetLeftTimeText());
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
    }

    public class GiftBagItem : MonoBehaviour
    {
        public TurtlePangModel Model => TurtlePangModel.Instance;
        private Image Icon;
        private LocalizeTextMeshProUGUI NumText;
        private Button BuyBtn;
        private Text PriceText;
        private TurtlePangGiftBagConfig Config;
        private TableShop ShopConfig;
        private void Awake()
        {
            Icon = transform.Find("Item/Icon").GetComponent<Image>();
            NumText = transform.Find("Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
            BuyBtn = transform.Find("Button").GetComponent<Button>();
            BuyBtn.onClick.AddListener(() =>
            {
                StoreModel.Instance.Purchase(ShopConfig.id);
            });
            PriceText = transform.Find("Button/Text").GetComponent<Text>();
        }

        public void Init(TurtlePangGiftBagConfig config)
        {
            Config = config;
            ShopConfig = GlobalConfigManager.Instance.GetTableShopByID(Config.ShopId);
            var rewards = CommonUtils.FormatReward(Config.RewardId, Config.RewardNum);
            Icon.sprite = UserData.GetResourceIcon(rewards[0].id);
            NumText.SetText(rewards[0].count.ToString());
            PriceText.text = StoreModel.Instance.GetPrice(ShopConfig.id);
        }
    }
}