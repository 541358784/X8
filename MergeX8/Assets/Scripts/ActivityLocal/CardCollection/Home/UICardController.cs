using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace ActivityLocal.CardCollection.Home
{
    public class UICardController : UIWindowController
    {
        private Button _closeButton;
        private Button _exchangeButton;
        private Button _superCardButton;

        private GameObject _cardItem;
        private GameObject _content;

        private ExchangeRedPoint TradeButtonRedPoint;

        private List<UICardItemMono> _cardItemsMono = new List<UICardItemMono>();

        private Transform DefaultWildCardItem;

        private Dictionary<int, UICardMainController.WildCardItem> WildCardItemDictionary =
            new Dictionary<int, UICardMainController.WildCardItem>();

        private UICardMainController.WildCardView WildCardViewController;
        private LocalizeTextMeshProUGUI TimeText;

        public override void PrivateAwake()
        {
            _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _closeButton.onClick.AddListener(OnClickClose);

            _exchangeButton = transform.Find("Root/ExchangeButton").GetComponent<Button>();
            _exchangeButton.onClick.AddListener(OnClickExchange);

            _superCardButton = transform.Find("Root/UniversalCardGroup/UniversalCard").GetComponent<Button>();
            _superCardButton.onClick.AddListener(OnClickSuperCard);

            _cardItem = transform.Find("Root/Scroll View/Viewport/Content/Card").gameObject;
            _cardItem.gameObject.SetActive(false);

            _content = transform.Find("Root/Scroll View/Viewport/Content").gameObject;

            TradeButtonRedPoint = transform.Find("Root/ExchangeButton/RedPoint").gameObject
                .AddComponent<ExchangeRedPoint>();
            TradeButtonRedPoint.Init();

            DefaultWildCardItem = transform.Find("Root/UniversalCardGroup/UniversalCard");
            DefaultWildCardItem.gameObject.SetActive(false);
            WildCardViewController = transform.Find("Root/UICardView").gameObject
                .AddComponent<UICardMainController.WildCardView>();
            WildCardViewController.gameObject.SetActive(true);
            WildCardViewController.gameObject.SetActive(false);

            foreach (var pair in WildCardItemDictionary)
            {
                Destroy(pair.Value.gameObject);
            }

            WildCardItemDictionary.Clear();
            foreach (var pair in CardCollectionModel.Instance.StorageCardCollection.WildCards)
            {
                if (pair.Value > 0)
                {
                    var wildCardConfig = CardCollectionModel.Instance.TableCardWildCard[pair.Key];
                    var wildCardItemObj = Instantiate(DefaultWildCardItem.gameObject, DefaultWildCardItem.parent);
                    wildCardItemObj.name = "WildCard" + wildCardConfig.Id;
                    wildCardItemObj.SetActive(true);
                    var cardBookItem = wildCardItemObj.AddComponent<UICardMainController.WildCardItem>();
                    cardBookItem.BindWildCardState(wildCardConfig);
                    WildCardItemDictionary.Add(wildCardConfig.Id, cardBookItem);
                    cardBookItem.BindCardViewController(WildCardViewController);
                }
            }
            TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("UpdateTimeText",0f,1f);
            AddAllEvent();
        }

        private void OnDestroy()
        {
            RemoveAllEvent();
        }

        public void UpdateTimeText()
        {
            TimeText.SetText(CardCollectionActivityModel.Instance.GetActivityLeftTimeString());
        }
        private void Start()
        {
            foreach (var themeId in CardCollectionModel.Instance.StorageCardCollection.OpenThemeList)
            {
                var theme = CardCollectionModel.Instance.GetCardThemeState(themeId).GetUpGradeTheme();
                var config = theme.CardThemeConfig;
                var item = GameObject.Instantiate(_cardItem, _content.transform, false);
                item.gameObject.SetActive(true);

                var mono = item.AddComponent<UICardItemMono>();
                mono.Init(config);

                _cardItemsMono.Add(mono);
            }
            if (CardCollectionReopenActivityModel.Instance.IsInitFromServer())
            {
                var themeState = CardCollectionModel.Instance.GetCardThemeState(CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme();
                var item = _cardItemsMono.Find(a =>
                    a._config.Id == themeState.CardThemeConfig.Id);
                item.transform.SetAsFirstSibling();
            }
            if (CardCollectionActivityModel.Instance.IsInitFromServer())
            {
                var themeState = CardCollectionModel.Instance.GetCardThemeState(CardCollectionActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme();
                var item = _cardItemsMono.Find(a =>
                    a._config.Id == themeState.CardThemeConfig.Id);
                item.transform.SetAsFirstSibling();
            }
            transform.Find("Root/Scroll View/Viewport/Content/BG").SetAsFirstSibling();
        }

        private void OnClickClose()
        {
            AnimCloseWindow();
        }

        private void OnClickExchange()
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                .GameEventGalleryStarchestShow);
            UICardGiftController.Open();
        }

        private void OnClickSuperCard()
        {
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

        public void OnUnLockTheme(EventCardThemeUnLock evt)
        {
            foreach (var themeItem in _cardItemsMono)
            {
                if (themeItem.ThemeState.GetUpGradeTheme() != themeItem.ThemeState)
                {
                    themeItem.Init(themeItem.ThemeState.GetUpGradeTheme().CardThemeConfig);
                }
            }
        }

        // public void OnStarValueChange(EventCardStarValueChange evt)
        // {
        //     StarNumText.SetTermFormats(ThemeState.GetStarCount().ToString());
        // }
        #endregion

        public class ExchangeRedPoint : MonoBehaviour
        {
            private LocalizeTextMeshProUGUI Label;

            private void Awake()
            {
                EventDispatcher.Instance.AddEvent<EventCardCountChange>(OnCardCountChange);
            }

            public void OnCardCountChange(EventCardCountChange evt)
            {
                UpdateView();
            }

            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEvent<EventCardCountChange>(OnCardCountChange);
            }

            public void Init()
            {
                Label = transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
                Label.gameObject.SetActive(false);
                gameObject.SetActive(true);
                UpdateView();
            }

            public bool CanExchange()
            {
                var curStar = CardCollectionModel.Instance.GetTotalStarCount();
                {
                    var themeState = CardCollectionModel.Instance.ThemeInUse;
                    if (themeState != null)
                    {
                        foreach (var packageId in themeState.CardThemeConfig.CardPackages)
                        {
                            var package = CardCollectionModel.Instance.TableCardPackage[packageId];
                            if (package.Cost > 0 && curStar > package.Cost)
                            {
                                return true;
                            }
                        }
                    }
                }
                {
                    var themeState = CardCollectionModel.Instance.ThemeReopenInUse;
                    if (themeState != null)
                    {
                        foreach (var packageId in themeState.CardThemeConfig.CardPackages)
                        {
                            var package = CardCollectionModel.Instance.TableCardPackage[packageId];
                            if (package.Cost > 0 && curStar > package.Cost)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            public void UpdateView()
            {
                gameObject.SetActive(CanExchange());
            }
        }
    }
}