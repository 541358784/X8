using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;

public class UIPopupPhotoAlbumShopBuyController:UIWindowController
{
    private Button CloseBtn;
    private Button BuyBtn;
    private Image Icon;
    private LocalizeTextMeshProUGUI IconText;
    private LocalizeTextMeshProUGUI NumText;

    private Image Icon2;
    private LocalizeTextMeshProUGUI IconText2;
    private Image Icon3;
    private LocalizeTextMeshProUGUI IconText3;
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
        
        Icon2 = GetItem<Image>("Root/Icon2");
        IconText2 = GetItem<LocalizeTextMeshProUGUI>("Root/Icon2/Text");
        Icon2.gameObject.SetActive(false);
        Icon3 = GetItem<Image>("Root/Icon3");
        IconText3 = GetItem<LocalizeTextMeshProUGUI>("Root/Icon3/Text");
        Icon3.gameObject.SetActive(false);
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
    private PhotoAlbumStoreItemConfig StoreItemConfig;
    private StoragePhotoAlbum Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        StoreItemConfig = objs[0] as PhotoAlbumStoreItemConfig;
        Storage = objs[1] as StoragePhotoAlbum;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if ((PhotoAlbumStoreItemType) StoreItemConfig.Type == PhotoAlbumStoreItemType.BuildItem)
        {
            Icon.sprite = ResourcesManager.Instance.GetSpriteVariant("PhotoAlbumAtlas",
                "PhotoAlbumMain14");
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

            if (StoreItemConfig.RewardId.Count > 1)
            {
                Icon2.gameObject.SetActive(true);
                Icon2.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[1],UserData.ResourceSubType.Big);
                if (StoreItemConfig.RewardNum[1] == 1)
                {
                    IconText2.gameObject.SetActive(false);
                }
                else
                {
                    IconText2.gameObject.SetActive(true);
                    IconText2.SetText(StoreItemConfig.RewardNum[1].ToString());   
                }
            }
            if (StoreItemConfig.RewardId.Count > 2)
            {
                Icon3.gameObject.SetActive(true);
                Icon3.sprite = UserData.GetResourceIcon(StoreItemConfig.RewardId[2],UserData.ResourceSubType.Big);
                if (StoreItemConfig.RewardNum[2] == 1)
                {
                    IconText3.gameObject.SetActive(false);
                }
                else
                {
                    IconText3.gameObject.SetActive(true);
                    IconText3.SetText(StoreItemConfig.RewardNum[2].ToString());   
                }
            }
        }
        NumText.SetText(StoreItemConfig.Price.ToString());
        BuyBtn.interactable = CanBuy;
    }

    public static UIPopupPhotoAlbumShopBuyController Instance;
    public static UIPopupPhotoAlbumShopBuyController Open(PhotoAlbumStoreItemConfig storeItemConfig,StoragePhotoAlbum storage)
    {
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupPhotoAlbumShopBuy, storeItemConfig,storage)as
            UIPopupPhotoAlbumShopBuyController;
        return Instance;
    }
}