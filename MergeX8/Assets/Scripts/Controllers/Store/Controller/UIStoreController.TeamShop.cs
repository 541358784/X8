using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK.Network.API;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStoreController
{
    public void InitTeamShopTab()
    {
        var teamShopPage = transform.Find("Root/LevelState3").gameObject.AddComponent<TeamShopPage>();
        teamShopPage.Init();
        InitTab(TabState.TeamShop,"LevelState3",teamShopPage);
        transform.Find("Root/LabelGroup/LevelState3").gameObject.SetActive(TeamManager.Instance.HasOrder());

        CheckTeamShopGuide();
    }

    public void CheckTeamShopGuide()
    {
        if (TeamManager.Instance.HasOrder() &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TeamShopEntrance2))
        {
            var btn = transform.Find("Root/LabelGroup/LevelState3").GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TeamShopEntrance2);
            });
            List<Transform> topLayer1 = new List<Transform>();
            topLayer1.Add(btn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TeamShopEntrance2, btn.transform as RectTransform, topLayer:topLayer1);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TeamShopEntrance2, null))
            {
                
            }
        }
    }
    
    public class TeamShopPage :MonoBehaviour,ITabContent
    {
        public void Show()
        {
            Animator.PlayAnimation("Appear");
        }
        public void Hide()
        {
            Animator.PlayAnimation("Disappear");
        }
        private Transform Root;
        Animator Animator;
        private LocalizeTextMeshProUGUI TeamCoinText;
        LocalizeTextMeshProUGUI TeamLevelText;
        private LocalizeTextMeshProUGUI TimeText;
        private TableTeamLevelConfig TeamLevelConfig => TeamManager.Instance.MyTeamLevelConfig;
        private List<TeamShopLevelGroup> LevelGroupList = new List<TeamShopLevelGroup>();
        private long NowWeekId;
        public void Init()
        {
            Root = transform;
            Animator = transform.GetComponent<Animator>();
            
            transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIStoreController.Instance?.AnimCloseWindow();
            });
            TeamCoinText = transform.Find("NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            TeamCoinText.SetText(TeamManager.Instance.GetCoin().ToString());
            TeamLevelText = transform.Find("LvGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            TeamLevelText.SetTermFormats(TeamLevelConfig!=null?TeamLevelConfig.Id.ToString():"0");
            TimeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("UpdateTime",0,1);

            var allConfig = TeamConfigManager.Instance.ShopConfigList;
            var levelShopDic = new Dictionary<int, List<TableTeamShopConfig>>();
            for (var i = 0; i < allConfig.Count; i++)
            {
                var shopConfig = allConfig[i];
                levelShopDic.TryAdd(shopConfig.RequireLevel, new List<TableTeamShopConfig>());
                levelShopDic[shopConfig.RequireLevel].Add(shopConfig);
            }

            var levelKeys = levelShopDic.Keys.ToList();
            var defaultLevelGroupItem = transform.Find("Viewport/Content/GameGiftTitle/NormalContent");
            defaultLevelGroupItem.gameObject.SetActive(false);
            for (var i = 0; i < levelKeys.Count; i++)
            {
                var levelGroup = Instantiate(defaultLevelGroupItem, defaultLevelGroupItem.parent).gameObject
                    .AddComponent<TeamShopLevelGroup>();
                levelGroup.gameObject.SetActive(true);
                levelGroup.Init(levelKeys[i],levelShopDic[levelKeys[i]],this);
                LevelGroupList.Add(levelGroup);
            }

            NowWeekId = TeamManager.Instance.Storage.WeekId;
        }

        public void UpdateTime()
        {
            var curTime = (long)APIManager.Instance.GetServerTime();
            var leftTime = TeamManager.Instance.NextShopRefreshTime - curTime;
            TimeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
            if (NowWeekId != TeamManager.Instance.Storage.WeekId)
            {
                NowWeekId = TeamManager.Instance.Storage.WeekId;
                UpdateView();
            }
            TeamCoinText.SetText(TeamManager.Instance.GetCoin().ToString());
        }

        public void UpdateView()
        {
            foreach (var group in LevelGroupList)
            {
                group.UpdateView();
            }
        }

        public class TeamShopLevelGroup : MonoBehaviour
        {
            public TeamShopPage Controller;
            private LocalizeTextMeshProUGUI LockText;
            private Transform Lock;
            private List<TableTeamShopConfig> ConfigList;
            private int Level;
            public List<TeamShopItem> ShopItemList = new List<TeamShopItem>();
            public void Init(int level,List<TableTeamShopConfig> configList,TeamShopPage controller)
            {
                Controller = controller;
                Level = level;
                ConfigList = configList;
                LockText = transform.Find("Lock/Text").GetComponent<LocalizeTextMeshProUGUI>();
                Lock = transform.Find("Lock");
                var defaultShopItem = transform.Find("ShopItem");
                defaultShopItem.gameObject.SetActive(false);
                for (var i = 0; i < configList.Count; i++)
                {
                    var shopItem = Instantiate(defaultShopItem, defaultShopItem.parent).gameObject
                        .AddComponent<TeamShopItem>();
                    shopItem.gameObject.SetActive(true);
                    shopItem.Init(configList[i],this);
                    ShopItemList.Add(shopItem);
                }
                Lock.SetAsLastSibling();
                LockText.SetTermFormats(Level.ToString());
                Lock.gameObject.SetActive(TeamManager.Instance.MyTeamLevelConfig==null || TeamManager.Instance.MyTeamLevelConfig.Id < Level);
            }

            public void UpdateView()
            {
                Lock.gameObject.SetActive(TeamManager.Instance.MyTeamLevelConfig==null || TeamManager.Instance.MyTeamLevelConfig.Id < Level);
                foreach (var shopItem in ShopItemList)
                {
                    shopItem.UpdateView();
                }
            }
            
        }

        public class TeamShopItem : MonoBehaviour
        {
            private CommonRewardItem RewardItem;
            private TableTeamShopConfig Config;
            private LocalizeTextMeshProUGUI LeftBuyTimesText;
            private Button BuyBtn;
            private TeamShopLevelGroup Controller;
            public void Init(TableTeamShopConfig config,TeamShopLevelGroup controller)
            {
                Controller = controller;
                Config = config;
                RewardItem = transform.Find("Item").gameObject.AddComponent<CommonRewardItem>();
                var rewards = new ResData(Config.RewardId, Config.RewardCount);
                RewardItem.Init(rewards);
                LeftBuyTimesText = transform.Find("BuyText").GetComponent<LocalizeTextMeshProUGUI>();
                transform.Find("ButtonBuy/Text").GetComponent<Text>().text = Config.Price.ToString();
                BuyBtn = transform.Find("ButtonBuy").GetComponent<Button>();
                BuyBtn.onClick.AddListener(() =>
                {
                    if (TeamManager.Instance.CanBuyItem(Config.Id))
                    {
                        if (TeamManager.Instance.BuyItem(Config.Id))
                        {
                            Controller.Controller.UpdateView();
                        }   
                    }
                });
                UpdateView();
            }

            public void UpdateView()
            {
                var buyTimes = TeamManager.Instance.Storage.BuyState.ContainsKey(Config.Id)
                    ? TeamManager.Instance.Storage.BuyState[Config.Id]
                    : 0;
                LeftBuyTimesText.SetTermFormats((Config.BuyTimes - buyTimes).ToString());
                BuyBtn.interactable = TeamManager.Instance.CanBuyItem(Config.Id);
            }
        }
    }
}