using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupNewNewIceBreakPackFinishController:UIWindowController
{
    public override void PrivateAwake()
    {
        
    }

    public static UIPopupNewNewIceBreakPackFinishController Instance;
    public static UIPopupNewNewIceBreakPackFinishController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupNewNewIceBreakPackFinish) as UIPopupNewNewIceBreakPackFinishController;
        return Instance;
    }
    public void OnBuy()
    {
        UpdateBtnState();
        NewNewIceBreakPackModel.Instance.CollectRewards(PayRewards).AddCallBack(()=>AnimCloseWindow()).WrapErrors();
    }

    public Button CloseBtn;
    public Button BuyBtn;
    public Button CollectBtn;
    public List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
    private Transform DefaultRewardItem;
    private List<TableNewNewIceBreakPackReward> PayRewards = new List<TableNewNewIceBreakPackReward>();
    public StorageNewNewIceBreakPack Storage => NewNewIceBreakPackModel.Instance.Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var rewardConfigs = NewNewIceBreakPackModel.Instance.NewNewIceBreakPackRewardList;
        var rewards = new List<ResData>();
        foreach (var config in rewardConfigs)
        {
            if (!config.isFree && !Storage.CollectState.Contains(config.id))
            {
                PayRewards.Add(config);
                rewards.AddRange(CommonUtils.FormatReward(config.itemId,config.ItemNum));
            }
        }

        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        BuyBtn = transform.Find("Root/BuyButton").GetComponent<Button>();
        BuyBtn.onClick.AddListener(() =>
        {
            StoreModel.Instance.Purchase(NewNewIceBreakPackModel.Instance.GlobalConfig.shopId);
        });
        transform.Find("Root/BuyButton/Text1").GetComponent<Text>().text =
            StoreModel.Instance.GetPrice(NewNewIceBreakPackModel.Instance.GlobalConfig.shopId);
        CollectBtn = transform.Find("Root/ReceiveButton").GetComponent<Button>();
        CollectBtn.onClick.AddListener(() =>
        {
            NewNewIceBreakPackModel.Instance.CollectRewards(PayRewards).AddCallBack(() =>
            {
                AnimCloseWindow();
            }).WrapErrors();
        });

        DefaultRewardItem = transform.Find("Root/Scroll View/Viewport/Content/Item");
        DefaultRewardItem.gameObject.SetActive(false);

        foreach (var reward in rewards)
        {
            var rewardItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject
                .AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(reward);
            RewardItems.Add(rewardItem);
        }

        UpdateBtnState();

    }

    public void UpdateBtnState()
    {
        BuyBtn.gameObject.SetActive(!Storage.BuyState);
        CollectBtn.gameObject.SetActive(Storage.BuyState);
    }
}