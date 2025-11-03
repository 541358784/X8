using System.Collections.Generic;
using DragonPlus;
using UnityEngine.UI;

public class UIPopupPillowWheelGiftController:UIWindowController
{
    public static UIPopupPillowWheelGiftController Instance;

    public static UIPopupPillowWheelGiftController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupPillowWheelGift) as UIPopupPillowWheelGiftController;
        return Instance;
    }

    public Button CloseBtn;
    private Text PriceText;
    private Button BuyBtn;
    private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
    private LocalizeTextMeshProUGUI TimeText;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        PriceText = transform.Find("Root/BuyButton/Text").GetComponent<Text>();
        var shopConfig = PillowWheelModel.Instance.ShopConfigList[0];
        PriceText.text = StoreModel.Instance.GetPrice(shopConfig.ShopId);
        BuyBtn = transform.Find("Root/BuyButton").GetComponent<Button>();
        BuyBtn.onClick.AddListener(() =>
        {
            StoreModel.Instance.Purchase(shopConfig.ShopId);
        });
        var rewards = CommonUtils.FormatReward(shopConfig.RewardId, shopConfig.RewardNum);
        var defaultITem = transform.Find("Root/RewardGroup/Item");
        foreach (var reward in rewards)
        {
            var rewardItem = Instantiate(defaultITem, defaultITem.parent).gameObject.AddComponent<CommonRewardItem>();
            rewardItem.Init(reward);
            RewardItems.Add(rewardItem);
        }
        defaultITem.gameObject.SetActive(false);
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
    }

    public void UpdateTime()
    {
        TimeText.SetText(PillowWheelModel.Instance.Storage.GetLeftTimeText());
    }
}