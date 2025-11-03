using UnityEngine.UI;

public partial class UIPillowWheelMainController
{
    private Button ShopBtn;

    public void InitShopEntrance()
    {
        ShopBtn = transform.Find("Root/ButtonShop").GetComponent<Button>();
        ShopBtn.onClick.AddListener(() => { UIPopupPillowWheelGiftController.Open(); });
    }
}