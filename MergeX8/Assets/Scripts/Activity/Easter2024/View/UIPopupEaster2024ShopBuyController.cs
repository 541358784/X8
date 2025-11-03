using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;

public class UIPopupEaster2024ShopBuyController:UIWindowController
{
    private Button CloseBtn;
    private Button BuyBtn;
    private Image Icon;
    private LocalizeTextMeshProUGUI NumText;
    private bool CanBuy => Storage.Score >= StoreItemConfig.Price;
    public override void PrivateAwake()
    {
        BuyBtn = GetItem<Button>("Root/Button");
        BuyBtn.onClick.AddListener(OnClickBuyBtn);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        Icon = GetItem<Image>("Root/Icon");
        NumText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
    }

    public void OnClickBuyBtn()
    {
        if (CanBuy)
        {
            AnimCloseWindow(()=>Storage.BuyStoreItem(StoreItemConfig));  
        }
    }
    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    private Easter2024StoreItemConfig StoreItemConfig;
    private StorageEaster2024 Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        StoreItemConfig = objs[0] as Easter2024StoreItemConfig;
        Storage = objs[1] as StorageEaster2024;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if ((Easter2024StoreItemType) StoreItemConfig.Type == Easter2024StoreItemType.BuildItem)
        {
            Icon.sprite =
                ResourcesManager.Instance.GetSpriteVariant(AtlasName.Easter2024Atlas, StoreItemConfig.Image);
        }
        else
        {
            Icon.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[0]);
            var text = Icon.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            var count = StoreItemConfig.RewardNum[0];
            text.gameObject.SetActive(count > 1);
            text.SetText(count.ToString());
        }
        NumText.SetText(StoreItemConfig.Price.ToString());
        BuyBtn.interactable = CanBuy;
    }

    public static UIPopupEaster2024ShopBuyController Open(Easter2024StoreItemConfig storeItemConfig,StorageEaster2024 storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupEaster2024ShopBuy, storeItemConfig,storage) as
            UIPopupEaster2024ShopBuyController;
    }
}