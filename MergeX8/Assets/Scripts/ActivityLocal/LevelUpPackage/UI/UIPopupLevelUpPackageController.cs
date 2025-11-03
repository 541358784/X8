using System.Collections.Generic;
using System.Security.Cryptography;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLevelUpPackageController:UIWindowController
{
    private Button BuyBtn;
    private Button CloseBtn;
    private Text PriceText;
    private Text OriginalPriceText;
    private LocalizeTextMeshProUGUI TitleText;
    private LocalizeTextMeshProUGUI TimeText;
    private Transform DefaultRewardItem;
    private List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
    private LocalizeTextMeshProUGUI LabelText;
    private GridLayoutGroup RewardLayoutGroup;
    public override void PrivateAwake()
    {
        BuyBtn = GetItem<Button>("Root/BuyButton");
        BuyBtn.onClick.AddListener(OnClickBuyBtn);
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        PriceText = GetItem<Text>("Root/BuyButton/Text");
        OriginalPriceText = GetItem<Text>("Root/BuyButton/Text1");
        OriginalPriceText.gameObject.SetActive(false);
        TitleText = GetItem<LocalizeTextMeshProUGUI>("Root/BG/TitleText");
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        RewardLayoutGroup = transform.Find("Root/RewardGroup").GetComponent<GridLayoutGroup>();
        DefaultRewardItem = transform.Find("Root/RewardGroup/Item");
        DefaultRewardItem.gameObject.SetActive(false);
        LabelText = GetItem<LocalizeTextMeshProUGUI>("Root/TagGroup/Text1");
        InvokeRepeating("UpdateTime",0f,1f);
    }

    public void UpdateTime()
    {
        if (PackageStorage != null)
        {
            TimeText.SetText(CommonUtils.FormatLongToTimeStr((long)PackageStorage.GetLeftTime()));
        }
    }
    public void OnClickBuyBtn()
    {
        if (PackageStorage == null)
            return;
        StoreModel.Instance.Purchase(PackageConfig.shopId,"LevelUpPackage",param1:PackageStorage);
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    
    private StorageLevelUpPackageSinglePackage PackageStorage;

    private TableLevelUpPackageContentConfig PackageConfig =>
        LevelUpPackageModel.Instance.ContentConfig[PackageStorage.PackageId];

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        PackageStorage = objs[0] as StorageLevelUpPackageSinglePackage;
        var priceText = StoreModel.Instance.GetPrice(PackageConfig.shopId);
        PriceText.text = priceText;
        
        transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(PackageConfig.shopId));

        // var numPart = priceText.GetNumParts();
        // var originalPriceText = "";
        // foreach (var pair in numPart)
        // {
        //     if (pair.Item1)
        //     {
        //         var price = float.Parse(pair.Item2);
        //         var originalPrice = PackageConfig.originalPriceMultiple * price;
        //         originalPriceText += originalPrice.ToString().SplitNumberStr();
        //     }
        //     else
        //     {
        //         originalPriceText += pair.Item2;
        //     }
        // }
        // OriginalPriceText.text = originalPriceText;
        TitleText.SetTerm(PackageConfig.packageName);
        TitleText.SetTermFormats( PackageStorage.Level.ToString());
        LabelText.SetText(PackageConfig.labelText);
        foreach (var rewardItem in RewardItemList)
        {
            DestroyImmediate(rewardItem.gameObject);
        }
        RewardItemList.Clear();
        var rewardList = CommonUtils.FormatReward(PackageConfig.rewardId, PackageConfig.rewardNum);
        for (var i = 0; i < rewardList.Count; i++)
        {
            var rewardConfig = rewardList[i];
            var rewardItem = Instantiate(DefaultRewardItem.gameObject, DefaultRewardItem.parent)
                .AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(rewardConfig);
            RewardItemList.Add(rewardItem);
        }

        var spacing = RewardLayoutGroup.spacing;
        spacing.x = rewardList.Count > 4 ? 100 : 50;
        RewardLayoutGroup.spacing = spacing;
        RewardLayoutGroup.constraintCount = rewardList.Count > 4 ? 3 : 4;
    }

    public static UIPopupLevelUpPackageController Open(StorageLevelUpPackageSinglePackage package)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupLevelUpPackage,package) as UIPopupLevelUpPackageController;
    }
}