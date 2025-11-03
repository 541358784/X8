using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIBlindBoxRecycleController:UIWindowController
{
    public static UIBlindBoxRecycleController Open()
    {
        var openView = UIManager.Instance.GetOpenedUIByPath<UIBlindBoxRecycleController>(UINameConst.UIBlindBoxRecycle);
        if (openView)
            return openView;
        openView = UIManager.Instance.OpenUI(UINameConst.UIBlindBoxRecycle) as UIBlindBoxRecycleController;
        return openView;
    }
    private BlindBoxModel Model => BlindBoxModel.Instance;
    private Button CloseBtn;
    private Transform DefaultShopItem;
    private LocalizeTextMeshProUGUI RecycleValueText;
    private List<RecycleShopItem> ShopItems = new List<RecycleShopItem>();
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        
        DefaultShopItem = transform.Find("Root/Scroll View/Viewport/Content/1");
        DefaultShopItem.gameObject.SetActive(false);
        
        var shopConfigs = Model.RecycleShopConfig;
        foreach (var config in shopConfigs)
        {
            var clone = Instantiate(DefaultShopItem, DefaultShopItem.parent);
            clone.gameObject.SetActive(true);
            var shopItem = clone.gameObject.AddComponent<RecycleShopItem>();
            shopItem.Init(config,this);
            ShopItems.Add(shopItem);
        }
        RecycleValueText = transform.Find("Root/NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RecycleValueText.SetText(Model.StorageGlobal.RecycleValue.ToString());
    }

    public void OnBuyRecycleShopItem()
    {
        RecycleValueText.SetText(Model.StorageGlobal.RecycleValue.ToString());
        foreach (var item in ShopItems)
        {
            item.UpdateBtnState();
        }
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
    }

    public class RecycleShopItem : MonoBehaviour
    {
        private BlindBoxRecycleShopConfig Config;
        private Button BuyBtn;
        private LocalizeTextMeshProUGUI PriceText;
        private LocalizeTextMeshProUGUI PriceTextGrey;
        private Image Icon;

        public void Init(BlindBoxRecycleShopConfig config,UIBlindBoxRecycleController controller)
        {
            Config = config;
            BuyBtn = transform.Find("Button").GetComponent<Button>();
            BuyBtn.onClick.AddListener(() =>
            {
                if (Config.BuyRecycleShopItem())
                {
                    controller.OnBuyRecycleShopItem();
                }
            });
            PriceText = transform.Find("Button/Text").GetComponent<LocalizeTextMeshProUGUI>();
            PriceTextGrey = transform.Find("Button/GreyText").GetComponent<LocalizeTextMeshProUGUI>();
            Icon = transform.Find("Icon").GetComponent<Image>();
            BuyBtn.interactable = BlindBoxModel.Instance.StorageGlobal.RecycleValue >= Config.RecyclePrice;
            PriceText.SetText(Config.RecyclePrice.ToString());
            PriceTextGrey.SetText(Config.RecyclePrice.ToString());
            Icon.sprite = UserData.GetResourceIcon(Config.BoxId, UserData.ResourceSubType.Big);
        }

        public void UpdateBtnState()
        {
            BuyBtn.interactable = BlindBoxModel.Instance.StorageGlobal.RecycleValue >= Config.RecyclePrice;
        }
    }
}