using System.Drawing;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIPopupFishCultureShopBuyController:UIWindowController
{
    private Button CloseBtn;
    private Button BuyBtn;
    // private LocalizeTextMeshProUGUI IconText;
    private LocalizeTextMeshProUGUI NumText;
    private bool CanBuy => Storage.CurScore >= StoreItemConfig.Price;
    public override void PrivateAwake()
    {
        BuyBtn = GetItem<Button>("Root/Button");
        BuyBtn.onClick.AddListener(OnClickBuyBtn);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        // IconText = GetItem<LocalizeTextMeshProUGUI>("Root/Icon/Text");
        NumText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
    }

    public void OnClickBuyBtn()
    {
        if (CanBuy)
        {
            AnimCloseWindow(()=>FishCultureModel.Instance.BuyFish(StoreItemConfig));  
        }
    }
    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    private FishCultureRewardConfig StoreItemConfig;
    private StorageFishCulture Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        StoreItemConfig = objs[0] as FishCultureRewardConfig;
        Storage = objs[1] as StorageFishCulture;
        UpdateUI();
    }

    public void UpdateUI()
    {
        NumText.SetText(StoreItemConfig.Price.ToString());
        BuyBtn.interactable = CanBuy;
    }

    public static UIPopupFishCultureShopBuyController Open(FishCultureRewardConfig storeItemConfig,StorageFishCulture storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupFishCultureShopBuy, storeItemConfig,storage) as
            UIPopupFishCultureShopBuyController;
    }
}