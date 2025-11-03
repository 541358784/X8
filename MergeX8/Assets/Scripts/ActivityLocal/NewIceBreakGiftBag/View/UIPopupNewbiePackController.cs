using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupNewbiePackController : UIWindowController
{
    public static UIPopupNewbiePackController Instance;
    public static UIPopupNewbiePackController Open()
    {
        if (!NewIceBreakGiftBagModel.Instance.IsOpen)
            return null;
        var giftBagConfig =
            NewIceBreakGiftBagModel.Instance.GetGiftBagConfig(NewIceBreakGiftBagModel.Instance.Storage.GiftBagId);
        if (giftBagConfig == null)
            return null;
        if (Instance)
        {
            Instance.CloseWindowWithinUIMgr(true);
            Instance = null;
        }
        if (!Instance)
        {
            Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupNewbiePack,giftBagConfig) as UIPopupNewbiePackController;
        }
        return Instance;
    }

    private StorageNewIceBreakGiftBag Storage => NewIceBreakGiftBagModel.Instance.Storage;
    private TableNewIceBreakGiftBag Config;
    private Text PriceText;
    private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
    private Transform Icon300;
    private Transform Icon500;
    private Button BuyBtn;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI TimeText;
    public override void PrivateAwake()
    {
        PriceText = GetItem<Text>("Root/RewardItem/BuyButton/Text");
        for (var i=1;i<=3;i++)
        {
            RewardItems.Add(transform.Find("Root/RewardItem/RewardGroup/Item"+i).gameObject.AddComponent<CommonRewardItem>());
        }

        Icon300 = transform.Find("Root/BGGroup/300Icon");
        Icon500 = transform.Find("Root/BGGroup/500Icon");
        BuyBtn = GetItem<Button>("Root/RewardItem/BuyButton");
        BuyBtn.onClick.AddListener(() =>
        {
            StoreModel.Instance.Purchase(Config.shopId);
        });
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0,1);
    }

    public void UpdateTime()
    {
        if (Storage.GiftBagId != Config.id)
            AnimCloseWindow();
        if (NewIceBreakGiftBagModel.Instance.GetLeftTime()<=0)
            AnimCloseWindow();
        TimeText.SetText(NewIceBreakGiftBagModel.Instance.GetLeftTimeText());
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Config = objs[0] as TableNewIceBreakGiftBag;
        var rewards = CommonUtils.FormatReward(Config.itemId, Config.ItemNum);
        for (var i = 0; i < rewards.Count; i++)
        {
            if (i >= RewardItems.Count)
                break;
            RewardItems[i].Init(rewards[i]);
        }
        for (var i = rewards.Count; i < RewardItems.Count; i++)
        {
            RewardItems[i].gameObject.SetActive(false);
        }
        PriceText.text = (StoreModel.Instance.GetPrice(Config.shopId));
        Icon300.gameObject.SetActive(Config.id == 1);
        Icon500.gameObject.SetActive(Config.id == 2);
    }
}