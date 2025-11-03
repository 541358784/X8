using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

public class UICardGiftController:UIWindowController
{
    public class StarStoreNode : MonoBehaviour
    {
        private TableCardCollectionCardPackage StarStoreItemConfig;
        private Button BuyButton;
        private LocalizeTextMeshProUGUI PriceText;
        private LocalizeTextMeshProUGUI PriceTextGrey;
        private UICardGiftController Controller;
        private Image Icon;
        private void Awake()
        {
            BuyButton = transform.Find("Button").GetComponent<Button>();
            BuyButton.onClick.AddListener(OnClickBuyButton);
            PriceText = transform.Find("Button/Text").GetComponent<LocalizeTextMeshProUGUI>();
            PriceTextGrey = transform.Find("Button/GreyText").GetComponent<LocalizeTextMeshProUGUI>();
            Icon = transform.Find("Icon").GetComponent<Image>();
        }

        private void Start()
        {
            BuyButton.GetComponent<ShieldButtonOnClick>().isUse = false;
        }

        public void Init(UICardGiftController controller)
        {
            Controller = controller;
        }
        public void BindStarStoreItemConfig(TableCardCollectionCardPackage starStoreItemConfig)
        {
            StarStoreItemConfig = starStoreItemConfig;
            UpdateViewState();
        }

        public void UpdateViewState()
        {
            if (!Controller || StarStoreItemConfig == null)
                return;
            BuyButton.interactable = Controller.StarCount >= StarStoreItemConfig.Cost;
            PriceText.SetText(StarStoreItemConfig.Cost.ToString());
            PriceTextGrey.SetText(StarStoreItemConfig.Cost.ToString());
            Icon.sprite = StarStoreItemConfig.GetCardPackageSprite();
        }
        public void OnClickBuyButton()
        {
            if (CardCollectionModel.Instance.TryCostStar(StarStoreItemConfig.Cost))
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryStarchestExchange,CardCollectionModel.Instance.GetTotalStarCount().ToString(),StarStoreItemConfig.Id.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryStars2Cardpack,
                    StarStoreItemConfig.Id.ToString());
                CardCollectionModel.Instance.AddCardPackage(StarStoreItemConfig.Id,1,"StoreExchange");
                // var rewards = new List<ResData>() {new ResData(910+StarStoreItemConfig.Level, 1)};
                // CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                //     false);
                CardCollectionModel.Instance.TryOpenSingleCardPackage(StarStoreItemConfig.Id).WrapErrors();
                Controller.UpdateStarCount();
                Controller.OnBuy();
                // BuyButton.interactable = Controller.StarCount >= StarStoreItemConfig.cost;
            }
            else
            {
                NotEnoughAction?.Invoke();
            }
        }

        private Action NotEnoughAction;
        public void BindNotEnoughAction(Action notEnoughAction)
        {
            NotEnoughAction = notEnoughAction;
        }
    }
    private Button CloseButton;
    private LocalizeTextMeshProUGUI StarNumText;
    private List<StarStoreNode> StoreNodeList = new List<StarStoreNode>();
    private List<StarStoreNode> ReopenStoreNodeList = new List<StarStoreNode>();
    private Animator TextTipAnimator;
    public override void PrivateAwake()
    {
        CloseButton = GetItem<Button>("Root/ButtonClose");
        CloseButton.onClick.AddListener(OnClickCloseButton);
        StarNumText = GetItem<LocalizeTextMeshProUGUI>("Root/StarNum/Num");
        TextTipAnimator = transform.Find("Root/TextTip")?.GetComponent<Animator>();
        if (TextTipAnimator)
            TextTipAnimator.gameObject.SetActive(false);
        StoreNodeList.Clear();
        for (var i = 1; transform.Find("Root/GiftGroup1/"+i); i++)
        {
            var starStoreNode = transform.Find("Root/GiftGroup1/" + i).gameObject.AddComponent<StarStoreNode>();
            starStoreNode.Init(this);
            StoreNodeList.Add(starStoreNode);
        }
        ReopenStoreNodeList.Clear();
        for (var i = 1; transform.Find("Root/GiftGroup2/"+i); i++)
        {
            var starStoreNode = transform.Find("Root/GiftGroup2/" + i).gameObject.AddComponent<StarStoreNode>();
            starStoreNode.Init(this);
            ReopenStoreNodeList.Add(starStoreNode);
        }
        AddAllEvent();
    }

    private void OnDestroy()
    {
        RemoveAllEvent();
    }

    public void OnClickCloseButton()
    {
        AnimCloseWindow();
    }

    public CardCollectionCardThemeState ThemeState;
    public CardCollectionCardThemeState ThemeReopenState;
    public int StarCount;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        ThemeState = CardCollectionModel.Instance.ThemeInUse.GetUpGradeTheme();
        ThemeReopenState = CardCollectionModel.Instance.ThemeReopenInUse.GetUpGradeTheme();
        StarCount = CardCollectionModel.Instance.GetTotalStarCount();
        StarNumText.SetTermFormats(StarCount.ToString());
        if (ThemeState != null)
        {
            var titleText = transform.Find("Root/GiftGroup1/Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
            titleText.SetTerm(ThemeState.NameKey);
            var starStoreConfig = new List<TableCardCollectionCardPackage>();
            foreach (var packageId in ThemeState.CardThemeConfig.CardPackages)
            {
                var package = CardCollectionModel.Instance.TableCardPackage[packageId];
                if (package.Cost > 0)
                {
                    starStoreConfig.Add(package);
                }
            }

            for (var i = 0; i < starStoreConfig.Count; i++)
            {
                if (i >= StoreNodeList.Count)
                    break;
                StoreNodeList[i].BindStarStoreItemConfig(starStoreConfig[i]);
                StoreNodeList[i].BindNotEnoughAction(() =>
                {
                    if (TextTipAnimator)
                    {
                        TextTipAnimator.gameObject.SetActive(false);
                        TextTipAnimator.gameObject.SetActive(true);
                    }
                });
            }

            for (var i = starStoreConfig.Count; i < StoreNodeList.Count; i++)
            {
                StoreNodeList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            transform.Find("Root/GiftGroup1").gameObject.SetActive(false);
        }
        if (ThemeReopenState != null)
        {
            var titleText = transform.Find("Root/GiftGroup2/Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
            titleText.SetTerm(ThemeReopenState.NameKey);
            var starStoreConfig = new List<TableCardCollectionCardPackage>();
            foreach (var packageId in ThemeReopenState.CardThemeConfig.CardPackages)
            {
                var package = CardCollectionModel.Instance.TableCardPackage[packageId];
                if (package.Cost > 0)
                {
                    starStoreConfig.Add(package);
                }
            }
            for (var i = 0; i < starStoreConfig.Count; i++)
            {
                if (i >= ReopenStoreNodeList.Count)
                    break;
                ReopenStoreNodeList[i].BindStarStoreItemConfig(starStoreConfig[i]);
                ReopenStoreNodeList[i].BindNotEnoughAction(() =>
                {
                    if (TextTipAnimator)
                    {
                        TextTipAnimator.gameObject.SetActive(false);
                        TextTipAnimator.gameObject.SetActive(true);   
                    }
                });
            }
            for (var i = starStoreConfig.Count; i < ReopenStoreNodeList.Count; i++)
            {
                ReopenStoreNodeList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            transform.Find("Root/GiftGroup2").gameObject.SetActive(false);
        }
    }

    public void OnUnLockTheme(EventCardThemeUnLock evt)
    {
        if (ThemeState != ThemeState.GetUpGradeTheme())
        {
            ThemeState = ThemeState.GetUpGradeTheme();
            if (ThemeState != null)
            {
                var titleText = transform.Find("Root/GiftGroup1/Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
                titleText.SetTerm(ThemeState.NameKey);
                var starStoreConfig = new List<TableCardCollectionCardPackage>();
                foreach (var packageId in ThemeState.CardThemeConfig.CardPackages)
                {
                    var package = CardCollectionModel.Instance.TableCardPackage[packageId];
                    if (package.Cost > 0)
                    {
                        starStoreConfig.Add(package);
                    }
                }
                for (var i = 0; i < starStoreConfig.Count; i++)
                {
                    if (i >= StoreNodeList.Count)
                        break;
                    StoreNodeList[i].BindStarStoreItemConfig(starStoreConfig[i]);
                    StoreNodeList[i].BindNotEnoughAction(() =>
                    {
                        if (TextTipAnimator)
                        {
                            TextTipAnimator.gameObject.SetActive(false);
                            TextTipAnimator.gameObject.SetActive(true);
                        }
                    });
                }
                for (var i = starStoreConfig.Count; i < StoreNodeList.Count; i++)
                {
                    StoreNodeList[i].gameObject.SetActive(false);
                }   
            }
        }

        if (ThemeReopenState != ThemeReopenState.GetUpGradeTheme())
        {
            ThemeReopenState = ThemeReopenState.GetUpGradeTheme();
            if (ThemeReopenState != null)
            {
                var titleText = transform.Find("Root/GiftGroup2/Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
                titleText.SetTerm(ThemeReopenState.NameKey);
                var starStoreConfig = new List<TableCardCollectionCardPackage>();
                foreach (var packageId in ThemeReopenState.CardThemeConfig.CardPackages)
                {
                    var package = CardCollectionModel.Instance.TableCardPackage[packageId];
                    if (package.Cost > 0)
                    {
                        starStoreConfig.Add(package);
                    }
                }
                for (var i = 0; i < starStoreConfig.Count; i++)
                {
                    if (i >= ReopenStoreNodeList.Count)
                        break;
                    ReopenStoreNodeList[i].BindStarStoreItemConfig(starStoreConfig[i]);
                    ReopenStoreNodeList[i].BindNotEnoughAction(() =>
                    {
                        if (TextTipAnimator)
                        {
                            TextTipAnimator.gameObject.SetActive(false);
                            TextTipAnimator.gameObject.SetActive(true);   
                        }
                    });
                }
                for (var i = starStoreConfig.Count; i < ReopenStoreNodeList.Count; i++)
                {
                    ReopenStoreNodeList[i].gameObject.SetActive(false);
                }   
            }
        }
    }

    public void UpdateStarCount()
    {
        StarCount = CardCollectionModel.Instance.GetTotalStarCount();
        StarNumText.SetTermFormats(StarCount.ToString());
    }

    public void OnBuy()
    {
        foreach (var node in StoreNodeList)
        {
            node.UpdateViewState();
        }
        foreach (var node in ReopenStoreNodeList)
        {
            node.UpdateViewState();
        }
    }
    #region Event
    public void AddAllEvent()
    {
        // EventDispatcher.Instance.AddEvent<EventCardStarValueChange>(OnStarValueChange);
        EventDispatcher.Instance.AddEvent<EventCardThemeUnLock>(OnUnLockTheme);
    }

    public void RemoveAllEvent()
    {
        // EventDispatcher.Instance.RemoveEvent<EventCardStarValueChange>(OnStarValueChange);
        EventDispatcher.Instance.RemoveEvent<EventCardThemeUnLock>(OnUnLockTheme);
    }
    // public void OnStarValueChange(EventCardStarValueChange evt)
    // {
    //     StarNumText.SetTermFormats(ThemeState.GetStarCount().ToString());
    // }
    #endregion
    public static void Open()
    {
        UIManager.Instance.OpenUI(UINameConst.UIMainCardGift);
    }
}