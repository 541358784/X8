using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;

public class UIPopupZumaShopBuyController:UIWindowController
{
    private Button CloseBtn;
    private Button BuyBtn;
    private Image Icon;
    private LocalizeTextMeshProUGUI IconText;
    private LocalizeTextMeshProUGUI NumText;
    private bool CanBuy => Storage.Score >= StoreItemConfig.Price;
    public override void PrivateAwake()
    {
        BuyBtn = GetItem<Button>("Root/Button");
        BuyBtn.onClick.AddListener(OnClickBuyBtn);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        Icon = GetItem<Image>("Root/Icon");
        IconText = GetItem<LocalizeTextMeshProUGUI>("Root/Icon/Text");
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
    private ZumaStoreItemConfig StoreItemConfig;
    private StorageZuma Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        StoreItemConfig = objs[0] as ZumaStoreItemConfig;
        Storage = objs[1] as StorageZuma;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if ((ZumaStoreItemType) StoreItemConfig.Type == ZumaStoreItemType.BuildItem)
        {
            Icon.sprite =
                ResourcesManager.Instance.GetSpriteVariant(AtlasName.ZumaAtlas, StoreItemConfig.Image);
            IconText.gameObject.SetActive(false);
        }
        else
        {
            Icon.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[0],UserData.ResourceSubType.Big);
            if (StoreItemConfig.RewardNum[0] == 1)
            {
                IconText.gameObject.SetActive(false);
            }
            else
            {
                IconText.gameObject.SetActive(true);
                IconText.SetText(StoreItemConfig.RewardNum[0].ToString());   
            }
        }
        NumText.SetText(StoreItemConfig.Price.ToString());
        BuyBtn.interactable = CanBuy;
    }

    public static UIPopupZumaShopBuyController Open(ZumaStoreItemConfig storeItemConfig,StorageZuma storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupZumaShopBuy, storeItemConfig,storage) as
            UIPopupZumaShopBuyController;
    }
}