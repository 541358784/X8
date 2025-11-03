using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSummerWatermelonBreadGiftController:UIWindowController
{
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private Transform DefaultRewardItem;
    private List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
    private LocalizeTextMeshProUGUI LabelText;
    private Button BuyBtn;
    private Text PriceText;
    private SummerWatermelonBreadPackageConfig PackageConfig;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        DefaultRewardItem = GetItem<Transform>("Root/RewardGroup/Item");
        DefaultRewardItem.gameObject.SetActive(false);
        LabelText = GetItem<LocalizeTextMeshProUGUI>("Root/TagGroup/Text1");
        BuyBtn = GetItem<Button>("Root/BuyButton");
        BuyBtn.onClick.AddListener(OnClickBuyBtn);
        PriceText = GetItem<Text>("Root/BuyButton/Text");
        InvokeRepeating("UpdateTime", 0, 1);
    }

    public void UpdateTime()
    {
        TimeText.SetText(SummerWatermelonBreadModel.Instance.GetPackageLeftTimeText(PackageConfig));
    }
    public void OnClickBuyBtn()
    {
        if (PackageConfig == null)
            return;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMergeactPackagePurchase);
        StoreModel.Instance.Purchase(PackageConfig.ShopId);
    }
    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
        if (objs.Length < 1)
        {
            Debug.LogError("蓝莓礼包没有传入package,默认取当前礼包");
            PackageConfig = SummerWatermelonBreadModel.Instance.GetCurrentPackage();
        }
        else
        {
            PackageConfig = objs[0] as SummerWatermelonBreadPackageConfig;
        }

        if (PackageConfig == null)
            return;
        var rewardList = CommonUtils.FormatReward(PackageConfig.RewardId, PackageConfig.RewardNum);
        for (var i = 0; i < rewardList.Count; i++)
        {
            var rewardConfig = rewardList[i];
            var rewardItem = Instantiate(DefaultRewardItem.gameObject, DefaultRewardItem.parent)
                .AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(rewardConfig);
            RewardItemList.Add(rewardItem);
        }

        var percent = PackageConfig.LabelNum * 100 + "%";
        if ((PackageConfig.LabelNum * 100) % 1f == 0)
        {
            percent = (int)(PackageConfig.LabelNum * 100) + "%";
        }
        LabelText.SetText(percent);
        PriceText.text = StoreModel.Instance.GetPrice(PackageConfig.ShopId);
    }

    public static UIPopupSummerWatermelonBreadGiftController Open(SummerWatermelonBreadPackageConfig packageConfig)
    {
        if (packageConfig == null)
            return null;
        var packagePopup = UIManager.Instance.OpenUI(SummerWatermelonBreadModel.Instance.PackageUIPath,packageConfig) as UIPopupSummerWatermelonBreadGiftController;
        return packagePopup;
    }
}