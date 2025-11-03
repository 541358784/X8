using DragonPlus;
using UnityEngine.UI;

public class UIPopupChristmasBlindBoxController : UIWindowController
{
    public override void PrivateAwake()
    {
    }

    public static  UIPopupChristmasBlindBoxController Instance;
    public static UIPopupChristmasBlindBoxController Open()
    {
        if (!ChristmasBlindBoxModel.Instance.IsPrivateOpened())
            return null;
        if (!Instance)
            Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupChristmasBlindBox) as UIPopupChristmasBlindBoxController;
        return Instance;
    }

    private Button CloseBtn;
    private Button BuyBtn;
    private LocalizeTextMeshProUGUI PriceText;
    private int ShopId => ChristmasBlindBoxModel.Instance.GlobalConfig.ShopId;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        BuyBtn = transform.Find("Root/Button").GetComponent<Button>();
        BuyBtn.onClick.AddListener(() =>
        {
            StoreModel.Instance.Purchase(ShopId);
        });
        PriceText = transform.Find("Root/Button/Text").GetComponent<LocalizeTextMeshProUGUI>();
        PriceText.SetText(StoreModel.Instance.GetPrice(ShopId));
    }
}