using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupDogPlayExtraRewardController:UIWindowController
{
    public static UIPopupDogPlayExtraRewardController Instance;
    public static UIPopupDogPlayExtraRewardController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance =
            UIManager.Instance.OpenUI(UINameConst.UIPopupDogPlayExtraReward) as
                UIPopupDogPlayExtraRewardController;
        return Instance;
    }
    
    
    
    
    private Slider Slider;
    private Button CollectBtn;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI SliderText;
    private StorageDogPlay Storage => DogPlayModel.Instance.Storage;
    private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
    private Transform DefaultRewardItem;
    private LocalizeTextMeshProUGUI TimeText;
    
    public override void PrivateAwake()
    {
        Slider = transform.Find("Root/Slider").GetComponent<Slider>();
        SliderText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        CollectBtn = transform.Find("Root/Button").GetComponent<Button>();
        CollectBtn.onClick.AddListener(async () =>
        {
            AnimCloseWindow();
            DogPlayModel.Instance.HideTaskItemGroup = true;
            await DogPlayModel.Instance.CollectRewards();
            DogPlayModel.Instance.HideTaskItemGroup = false;
            MergeDogPlay.Instance?.PerformInitFly();
        });
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
    }

    public void UpdateTime()
    {
        TimeText.SetText(DogPlayExtraRewardModel.Instance.GetActivityLeftTimeString());
    }
    public void RefreshView()
    {
        Slider.value = (float)Storage.CurCount / Storage.MaxCount;
        SliderText.SetText(Storage.CurCount + "/" + Storage.MaxCount);
        CollectBtn.interactable = Storage.CurCount >= Storage.MaxCount;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        RefreshView();
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }

        var extraRewards = DogPlayExtraRewardModel.Instance.GetExtraRewards(Storage);
        // var rewards = CommonUtils.FormatReward(Storage.Rewards);
        // rewards.AddRange(extraRewards);
        var rewards = extraRewards;
        
        DefaultRewardItem = transform.Find("Root/RewardGroup/Item");
        DefaultRewardItem.gameObject.SetActive(false);
        foreach (var reward in rewards)
        {
            var rewardItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject.AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(reward);
            RewardItems.Add(rewardItem);
        }
    }
    
}