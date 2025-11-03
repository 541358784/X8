using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;

public class UIPopupSnakeLadderShopBuyController:UIWindowController
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
    private SnakeLadderStoreItemConfig StoreItemConfig;
    private StorageSnakeLadder Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        StoreItemConfig = objs[0] as SnakeLadderStoreItemConfig;
        Storage = objs[1] as StorageSnakeLadder;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if ((SnakeLadderStoreItemType) StoreItemConfig.Type == SnakeLadderStoreItemType.BuildItem)
        {
            Icon.sprite =
                ResourcesManager.Instance.GetSpriteVariant(AtlasName.SnakeLadderAtlas, StoreItemConfig.Image);
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

    public static UIPopupSnakeLadderShopBuyController Open(SnakeLadderStoreItemConfig storeItemConfig,StorageSnakeLadder storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupSnakeLadderShopBuy, storeItemConfig,storage) as
            UIPopupSnakeLadderShopBuyController;
    }
}