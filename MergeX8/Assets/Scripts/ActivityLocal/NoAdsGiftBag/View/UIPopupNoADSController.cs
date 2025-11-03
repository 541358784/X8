using DragonPlus;
using UnityEngine.UI;

public class UIPopupNoADSController : UIWindowController
{
    public static UIPopupNoADSController Open()
    {
        if (Instance)
            return Instance;
        UIManager.Instance.OpenUI(UINameConst.UIPopupNoADS);
        return Instance;
    }

    public static UIPopupNoADSController Instance;
    public override void PrivateAwake()
    {
        Instance = this;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var config = NoAdsGiftBagModel.Instance.GetConfig();
        var rewards = CommonUtils.FormatReward(config.itemId, config.ItemNum);
        var defaultItem = transform.Find("Root/RewardGroup/Item");
        defaultItem.gameObject.SetActive(false);
        for (var i = 0; i < rewards.Count; i++)
        {
            var item = Instantiate(defaultItem, defaultItem.parent).gameObject.AddComponent<CommonRewardItem>();
            item.gameObject.SetActive(true);
            item.Init(rewards[i]);
        }

        var priceText = transform.Find("Root/BuyButton/Text").GetComponent<Text>();
        priceText.text = StoreModel.Instance.GetPrice(config.shopId);
        
        var fakePriceText = transform.Find("Root/BuyButton/FakeText").GetComponent<Text>();
        fakePriceText.text = StoreModel.Instance.GetPrice(config.fakeShopId);

        var buyButton = transform.Find("Root/BuyButton").GetComponent<Button>();
        buyButton.onClick.AddListener(() =>
        {
            StoreModel.Instance.Purchase(config.shopId);
        });
        
        transform.Find("Root/BG/Text").GetComponent<LocalizeTextMeshProUGUI>().SetTermFormats(config.tagText);
        
        transform.Find("Root/CloseButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });

        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);

    }

    private LocalizeTextMeshProUGUI TimeText;

    public void UpdateTime()
    {
        TimeText.SetText(NoAdsGiftBagModel.Instance.LeftTimeString());
        if (!NoAdsGiftBagModel.Instance.IsActive())
            AnimCloseWindow();
    }
}