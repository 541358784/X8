using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK.Network.API;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGuildCardGetController:UIWindowController
{
    
    public static UIPopupGuildCardGetController Instance;
    public static UIPopupGuildCardGetController Open(PassGiftData passGiftData)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildCardGet,passGiftData) as UIPopupGuildCardGetController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        
    }

    private PassGiftData passData;
    private HeadIconNode HeadIcon;
    
    
    public int ContentCount = 4;
    public List<Transform> ContentList = new List<Transform>();
    public List<CardItem> CardItemList = new List<CardItem>();
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        passData = objs[0] as PassGiftData;
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        var viewState = new AvatarViewState(passData.PlayerInfo.AvatarIcon, passData.PlayerInfo.AvatarFrameIcon,passData.PlayerInfo.UserName,false);
        HeadIcon = HeadIconNode.BuildHeadIconNode(transform.Find("Root/Content/HeadGroup/Head") as RectTransform,viewState);
        transform.Find("Root/Content/HeadGroup/BG").gameObject.SetActive(false);
        transform.Find("Root/Content/NameText").GetComponent<Text>().text = passData.PlayerInfo.UserName;


        ContentList.Add(transform.Find("Root/Content/CardGroup/1-4"));
        ContentList.Add(transform.Find("Root/Content/CardGroup/5-7"));

        var defaultCardItem = transform.Find("Root/Content/CardGroup/1-4/Card");
        defaultCardItem.gameObject.SetActive(false);
        
        var cardData = passData;
        for (var i = 0; i < cardData.ExtraData.CardList.Count; i++)
        {
            var cardId = cardData.ExtraData.CardList[i];
            var contentIndex = i / ContentCount;
            var content = ContentList[contentIndex];
            var cardObj = Instantiate(defaultCardItem, content);
            content.gameObject.SetActive(true);
            cardObj.gameObject.SetActive(true);
            var cardItem = cardObj.gameObject.AddComponent<CardItem>();
            cardItem.Init(cardId);
            CardItemList.Add(cardItem);
        }

        transform.Find("Root/Content/Button/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(passData.ExtraData.Price.ToString());
        transform.Find("Root/Content/Button/GreyText").GetComponent<LocalizeTextMeshProUGUI>().SetText(passData.ExtraData.Price.ToString());

        InitHeartGroup();
        GetBtn = transform.Find("Root/Content/Button").GetComponent<Button>();
        GetBtn.onClick.AddListener(() =>
        {
            if (TeamManager.Instance.GetLife() >= passData.ExtraData.Price)
            {
                TeamManager.Instance.AddLife(-passData.ExtraData.Price,"BuyCard");
                var index = TeamManager.Instance.OpenTeamCardPackage(passData);
                UIPopupGuildCardOpenController.Open(passData, index, () =>
                {
                    CardCollectionModel.Instance.DoAllUndoAction();
                });
            }
        });
        GetBtn.interactable = TeamManager.Instance.GetLife() >= passData.ExtraData.Price;
        XUtility.WaitFrames(1, CheckCardGuide);
    }

    private Button GetBtn;

    public void CheckCardGuide()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TeamCardPackageDesc) ||
            GuideSubSystem.Instance.IsShowingGuide())
            return;
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TeamCardPackageDesc, null))
        {
            var btn = transform.Find("Root/Content/Button").GetComponent<Button>();
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(btn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TeamCardPackageOpen, btn.transform as RectTransform,
                topLayer: topLayer);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TeamCardPackageOpen);
                TeamManager.Instance.AddLife(-passData.ExtraData.Price,"BuyCardGuide");
                var index = TeamManager.Instance.OpenTeamCardPackage(passData,true);
                UIPopupGuildCardOpenController.Open(passData, index, () =>
                {
                    CardCollectionModel.Instance.DoAllUndoAction();
                });
            });
        }
    }
    
    public class CardItem : MonoBehaviour
    {
        private Image Icon;
        private Dictionary<int, Transform> BGDic = new Dictionary<int, Transform>();
        private Dictionary<int, Transform> StarDic = new Dictionary<int, Transform>();
        private Transform HaveFlag;
        private CardCollectionCardItemState CardItemState;

        public void Init(int cardId)
        {
            CardItemState = CardCollectionModel.Instance.GetCardItemState(cardId);
            Icon = transform.Find("Mask/Icon").GetComponent<Image>();
            Icon.sprite = CardItemState.GetCardSprite();

            var starIndex = 1;
            while (true)
            {
                var bg = transform.Find("BGGroup/" + starIndex);
                var star = transform.Find("Star/" + starIndex);
                if (bg == null || star == null)
                {
                    break;
                }
                BGDic.Add(starIndex,bg);
                StarDic.Add(starIndex,star);
                bg.gameObject.SetActive(CardItemState.CardItemConfig.Level == starIndex);
                star.gameObject.SetActive(CardItemState.CardItemConfig.Level == starIndex);
                starIndex++;
            }
            HaveFlag = transform.Find("Have");
            HaveFlag.gameObject.SetActive(CardItemState.CollectCount > 0);
        }
    }
    
    
    #region 小心心
    // private List<Transform> HeartList = new List<Transform>();
    private LocalizeTextMeshProUGUI HeartText;
    public void InitHeartGroup()
    {
        // for (var i = 1; i <= TeamConfigManager.Instance.LocalTeamConfig.MaxLife; i++)
        // {
        //     HeartList.Add(transform.Find("Root/Content/Heart/Group/"+i+"/Full"));
        // }
        HeartText = transform.Find("Root/Content/Heart/Group/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateHeart",0,1);
    }

    public void UpdateHeart()
    {
        var curLife = TeamManager.Instance.GetLife();
        // for (var i = 0; i < HeartList.Count; i++)
        // {
        //     HeartList[i].gameObject.SetActive(curLife > i);
        // }
        HeartText.SetText(curLife.ToString());
        GetBtn.interactable = curLife >= passData.ExtraData.Price;
    }
    #endregion
}