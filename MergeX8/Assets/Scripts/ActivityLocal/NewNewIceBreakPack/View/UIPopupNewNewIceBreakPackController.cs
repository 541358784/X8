using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupNewNewIceBreakPackController:UIWindowController
{
    public override void PrivateAwake()
    {
        
    }

    public static UIPopupNewNewIceBreakPackController Instance;
    public static UIPopupNewNewIceBreakPackController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupNewNewIceBreakPack) as UIPopupNewNewIceBreakPackController;
        return Instance;
    }

    public Button CloseBtn;
    public Button BuyBtn;
    public LocalizeTextMeshProUGUI EndTimeText;
    public LocalizeTextMeshProUGUI NextUnlockTimeText;
    private List<DayGroup> DayGroups = new List<DayGroup>();
    private StorageNewNewIceBreakPack Storage => NewNewIceBreakPackModel.Instance.Storage;
    
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CloseBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            if (Storage.ShowEndView)
            {
                var rewardList = NewNewIceBreakPackModel.Instance.NewNewIceBreakPackRewardList;
                var collectList = new List<TableNewNewIceBreakPackReward>();
                foreach (var reward in rewardList)
                {
                    if (NewNewIceBreakPackModel.Instance.CanCollectReward(reward))
                    {
                        collectList.Add(reward);
                    }
                }
                AnimCloseWindow(async ()=>
                {
                    await NewNewIceBreakPackModel.Instance.CollectRewards(collectList);
                    if (!Storage.BuyState)
                        UIPopupNewNewIceBreakPackFinishController.Open();
                });
            }
            else
            {
                AnimCloseWindow();   
            }
        });
        BuyBtn = transform.Find("Root/ButtonBuy").GetComponent<Button>();
        BuyBtn.onClick.AddListener(() =>
        {
            StoreModel.Instance.Purchase(NewNewIceBreakPackModel.Instance.GlobalConfig.shopId);
        });
        transform.Find("Root/ButtonBuy/Root/Text").GetComponent<Text>().text =
            StoreModel.Instance.GetPrice(NewNewIceBreakPackModel.Instance.GlobalConfig.shopId);
        EndTimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        NextUnlockTimeText = transform.Find("Root/BG/TextReceiveTime").GetComponent<LocalizeTextMeshProUGUI>();
        NextUnlockTimeText.SetTerm("ui_newIcebreak_cooldown");
        
        transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(NewNewIceBreakPackModel.Instance.GlobalConfig.shopId));
        
        
        InvokeRepeating("UpdateTime",0,1);
        var configs = NewNewIceBreakPackModel.Instance.NewNewIceBreakPackRewardList;
        foreach (var config in configs)
        {
            var timeKey = config.unLockTime;
            if (!UnlockTimeKeyConfigDic.TryGetValue(timeKey,out var configList))
            {
                configList = new List<TableNewNewIceBreakPackReward>();
                UnlockTimeKeyConfigDic.Add(timeKey,configList);
            }
            configList.Add(config);
        }

        var groupCount = 0;
        foreach (var pair in UnlockTimeKeyConfigDic)
        {
            groupCount++;
            var groupNode = transform.Find("Root/Content/" + groupCount);
            if (groupNode)
            {
                var dayGroup = groupNode.gameObject.AddComponent<DayGroup>();
                dayGroup.Init(pair.Value,pair.Key);
                DayGroups.Add(dayGroup);
            }
            else
            {
                break;
            }
        }
        UpdateViewState();
    }

    private Dictionary<long, List<TableNewNewIceBreakPackReward>> UnlockTimeKeyConfigDic =
        new Dictionary<long, List<TableNewNewIceBreakPackReward>>();
    private long NextUnLockTime = -1;
    public void UpdateTime()
    {
        var curTime = (long)APIManager.Instance.GetServerTime();
        var leftTime = Storage.EndTime - curTime;
        if (leftTime < 0)
            leftTime = 0;
        var endTimeText = CommonUtils.FormatLongToTimeStr(leftTime);
        EndTimeText.SetText(endTimeText);

        long curNextUnlockTime = -1;
        var pastMin = (curTime - Storage.StartTime)/(long)XUtility.Min;
        foreach (var pair in UnlockTimeKeyConfigDic)
        {
            if (pastMin < pair.Key)
            {
                curNextUnlockTime = pair.Key;
                break;
            }
        }
        if (NextUnLockTime != curNextUnlockTime)
        {
            NextUnLockTime = curNextUnlockTime;
            UpdateViewState();
        }
        NextUnlockTimeText.gameObject.SetActive(NextUnLockTime >= 0);
        if (NextUnLockTime >= 0)
        {
            var nextUnLockDurationTime = NextUnLockTime  * (long)XUtility.Min + Storage.StartTime - curTime;
            NextUnlockTimeText.SetTermFormats(CommonUtils.FormatLongToTimeStr(nextUnLockDurationTime));
        }
        
    }
    public void OnBuy()
    {
        UpdateViewState();
    }

    public void UpdateViewState()
    {
        BuyBtn.gameObject.SetActive(!Storage.BuyState);
        foreach (var dayGroup in DayGroups)
        {
            dayGroup.Free.Init(dayGroup.Free.Config);
            dayGroup.Pay.Init(dayGroup.Pay.Config);
        }
    }

    public class DayGroup : MonoBehaviour
    {
        public LocalizeTextMeshProUGUI TagText;
        public List<TableNewNewIceBreakPackReward> Configs;
        public RewardItemGroup Free;
        public RewardItemGroup Pay;
        private LocalizeTextMeshProUGUI DayText;

        public void Init(List<TableNewNewIceBreakPackReward> configs,long unlockTime)
        {
            Configs = configs;
            Free = transform.Find("Free").gameObject.AddComponent<RewardItemGroup>();
            Free.Init(Configs.Find(a=>a.isFree));
            Pay = transform.Find("Vip").gameObject.AddComponent<RewardItemGroup>();
            Pay.Init(Configs.Find(a=>!a.isFree));
            TagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            TagText.SetText(Configs.Find(a => !a.isFree)?.tagText);
            DayText = transform.Find("Day").GetComponent<LocalizeTextMeshProUGUI>();
            DayText.SetTermFormats((unlockTime * (long)XUtility.Min /(long)XUtility.DayTime+1).ToString());
        }

    }

    public class RewardItemGroup : MonoBehaviour
    {
        private Button BtnBuy;
        private Button BtnCollect;
        private Button BtnGray;
        private Transform Finish;
        private Transform DefaultRewardItem;
        private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
        public TableNewNewIceBreakPackReward Config;

        public void Init(TableNewNewIceBreakPackReward config)
        {
            DefaultRewardItem = transform.Find("ItemGroup/Item");
            DefaultRewardItem.gameObject.SetActive(false);
            BtnBuy = transform.Find("ButtonBuy")?.GetComponent<Button>();
            if (BtnBuy)
                BtnBuy.onClick.AddListener(() =>
                {
                    StoreModel.Instance.Purchase(NewNewIceBreakPackModel.Instance.GlobalConfig.shopId);
                });
            BtnCollect = transform.Find("ButtonReceive").GetComponent<Button>();
            BtnCollect.onClick.AddListener(() =>
            {
                NewNewIceBreakPackModel.Instance.CollectReward(Config).WrapErrors();
                UpdateBtnState();
            });
            BtnGray = transform.Find("ButtonGray").GetComponent<Button>();
            Finish = transform.Find("Finish");
            
            Config = config;
            if (Config == null)
            {
                gameObject.SetActive(false);
                return;   
            }
            else
            {
                gameObject.SetActive(true);
            }
            foreach (var item in RewardItems)
            {
                Destroy(item.gameObject);
            }
            RewardItems.Clear();
            var rewards = CommonUtils.FormatReward(Config.itemId, Config.ItemNum);
            foreach (var reward in rewards)
            {
                var rewardItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject.AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(reward);
                RewardItems.Add(rewardItem);
            }
            
            UpdateBtnState();
        }

        public void UpdateBtnState()
        {
            var curTime = (long)APIManager.Instance.GetServerTime();
            var isUnlock = curTime > NewNewIceBreakPackModel.Instance.Storage.StartTime +
                Config.unLockTime * (long)XUtility.Min;
            var isCollect = NewNewIceBreakPackModel.Instance.Storage.CollectState.Contains(Config.id);
            var isBuy = NewNewIceBreakPackModel.Instance.Storage.BuyState;
            if (!isUnlock)
            {
                BtnBuy?.gameObject.SetActive(false);
                BtnCollect.gameObject.SetActive(false);
                BtnGray.gameObject.SetActive(true);
                Finish.gameObject.SetActive(false);
            }
            else if(isCollect)
            {
                BtnBuy?.gameObject.SetActive(false);
                BtnCollect.gameObject.SetActive(false);
                BtnGray.gameObject.SetActive(false);
                Finish.gameObject.SetActive(true);
            }
            else
            {
                if (Config.isFree || isBuy)
                {
                    BtnBuy?.gameObject.SetActive(false);
                    BtnCollect.gameObject.SetActive(true);
                    BtnGray.gameObject.SetActive(false);
                    Finish.gameObject.SetActive(false);   
                }
                else
                {
                    BtnBuy?.gameObject.SetActive(true);
                    BtnCollect.gameObject.SetActive(false);
                    BtnGray.gameObject.SetActive(false);
                    Finish.gameObject.SetActive(false);
                }
            }
        }
    }
}