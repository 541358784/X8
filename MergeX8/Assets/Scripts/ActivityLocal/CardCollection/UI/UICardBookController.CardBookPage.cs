using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UICardBookController : UIWindowController
{
    public class CardBookPage : MonoBehaviour
    {
        public class TopGroupNode : MonoBehaviour
        {
            private CardCollectionCardBookState CardBookState;
            private RewardGroupNode RewardGroup;
            // private Image Icon;
            // private LocalizeTextMeshProUGUI NameText;
            // private Dictionary<int,Transform> BGGroup = new Dictionary<int,Transform>();
            // private Slider Slider;
            // private LocalizeTextMeshProUGUI SliderText;
            private TopCardBookGroup NormalGroup;
            private TopCardBookGroup EmptyGroup;

            public void Awake()
            {
                RewardGroup = transform.Find("RewardGroup").gameObject.AddComponent<RewardGroupNode>();
                NormalGroup = transform.Find("CardBook").gameObject.AddComponent<TopCardBookGroup>();
                EmptyGroup = transform.Find("CardBookEmpty")?.gameObject.AddComponent<TopCardBookGroup>();
                // BGGroup.Add(1,transform.Find("CardBook/BGGroup/Blue"));
                // BGGroup.Add(2,transform.Find("CardBook/BGGroup/Purple"));
                // BGGroup.Add(3,transform.Find("CardBook/BGGroup/Gold"));
                // Icon = transform.Find("CardBook/Mask/Icon").GetComponent<Image>();
                // NameText = transform.Find("CardBook/NameText").GetComponent<LocalizeTextMeshProUGUI>();
                // Slider = transform.Find("CardBook/Slider").GetComponent<Slider>();
                // SliderText = transform.Find("CardBook/SliderText").GetComponent<LocalizeTextMeshProUGUI>();
            }

            public void BindCardBookState(CardCollectionCardBookState cardBookState)
            {
                CardBookState = cardBookState;
                RewardGroup.BindCardBookState(CardBookState);
                RewardGroup.gameObject.SetActive(CardBookState.CompletedReward.Count > 0);
                NormalGroup.BindCardBookState(CardBookState);
                EmptyGroup?.BindCardBookState(CardBookState);
                NormalGroup.gameObject.SetActive(CardBookState.CompletedReward.Count > 0);
                EmptyGroup?.gameObject.SetActive(CardBookState.CompletedReward.Count == 0);
                // Icon.sprite = CardBookState.GetIconSprite();
                // NameText.SetTerm(CardBookState.NameKey);
                // foreach (var pair in BGGroup)
                // {
                //     pair.Value.gameObject.SetActive(pair.Key == CardBookState.CardBookConfig.Level);
                // }
                // SliderText.gameObject.SetActive(!CardBookState.IsCompleted);
                // Slider.gameObject.SetActive(!CardBookState.IsCompleted);
                // SliderText.SetText(CardBookState.CollectCardItemCount+"/"+CardBookState.MaxCardItemCount);
                // Slider.value = (float) CardBookState.CollectCardItemCount / CardBookState.MaxCardItemCount;
                transform.Find("Text").gameObject.SetActive(CardBookState.CompletedReward.Count > 0 && !CardBookState.IsCompleted);
            }

            public class TopCardBookGroup:MonoBehaviour
            {
                private CardCollectionCardBookState CardBookState;
                private Image Icon;
                private LocalizeTextMeshProUGUI NameText;
                private Dictionary<int,Transform> BGGroup = new Dictionary<int,Transform>();
                private Slider Slider;
                private LocalizeTextMeshProUGUI SliderText;
                public void Awake()
                {
                    BGGroup.Add(1,transform.Find("BGGroup/Blue"));
                    BGGroup.Add(2,transform.Find("BGGroup/Purple"));
                    BGGroup.Add(3,transform.Find("BGGroup/Gold"));
                    Icon = transform.Find("Mask/Icon").GetComponent<Image>();
                    NameText = transform.Find("NameText").GetComponent<LocalizeTextMeshProUGUI>();
                    Slider = transform.Find("Slider").GetComponent<Slider>();
                    SliderText = transform.Find("SliderText").GetComponent<LocalizeTextMeshProUGUI>();
                }
                public void BindCardBookState(CardCollectionCardBookState cardBookState)
                {
                    CardBookState = cardBookState;
                    Icon.sprite = CardBookState.GetIconSprite();
                    NameText.SetTerm(CardBookState.NameKey);
                    foreach (var pair in BGGroup)
                    {
                        pair.Value.gameObject.SetActive(pair.Key == CardBookState.CardBookConfig.Level);
                    }
                    SliderText.gameObject.SetActive(!CardBookState.IsCompleted);
                    Slider.gameObject.SetActive(!CardBookState.IsCompleted);
                    SliderText.SetText(CardBookState.CollectCardItemCount+"/"+CardBookState.MaxCardItemCount);
                    Slider.value = (float) CardBookState.CollectCardItemCount / CardBookState.MaxCardItemCount;
                }
            }
        }
        public class RewardGroupNode : MonoBehaviour
        {
            private CardCollectionCardBookState CardBookState;
            private Transform DefaultRewardItem;
            private List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
            private Transform CompletedFlag;
            private bool IsAwake = false;
            private void Awake()
            {
                if (IsAwake)
                    return;
                IsAwake = true;
                DefaultRewardItem = transform.Find("Item");
                DefaultRewardItem.gameObject.SetActive(false);
                CompletedFlag = transform.Find("Completed");
            }

            public void BindCardBookState(CardCollectionCardBookState cardBookState)
            {
                Awake();
                CardBookState = cardBookState;
                // foreach (var item in RewardItemList)
                //     Destroy(item.gameObject);
                // RewardItemList.Clear();
                for (var i = RewardItemList.Count; i < CardBookState.CompletedReward.Count; i++)
                {
                    var rewardItem = Instantiate(DefaultRewardItem.gameObject, DefaultRewardItem.parent)
                        .AddComponent<CommonRewardItem>();
                    rewardItem.gameObject.SetActive(true);
                    RewardItemList.Add(rewardItem);
                }
                for (var i = CardBookState.CompletedReward.Count; i < RewardItemList.Count; i++)
                {
                    RewardItemList[i].gameObject.SetActive(false);
                }

                var rewardIndex = 0;
                foreach (var reward in CardBookState.CompletedReward)
                {
                    var rewardItem = RewardItemList[rewardIndex];
                    rewardIndex++;
                    rewardItem.gameObject.SetActive(true);
                    rewardItem.Init(reward);
                    rewardItem.gameObject.SetActive(!CardBookState.IsCompleted);
                }
                if (CompletedFlag)
                    CompletedFlag.gameObject.SetActive(CardBookState.IsCompleted);
            }
        }

        public class CardItem : MonoBehaviour
        {
            public class BaseGroup : MonoBehaviour
            {
                public CardCollectionCardItemState CardItemState;
                public List<Transform> StarList = new List<Transform>();
                // private LocalizeTextMeshProUGUI NameText;

                public virtual void Awake()
                {
                    for (var i = 1; transform.Find("Star/" + i); i++)
                    {
                        StarList.Add(transform.Find("Star/" + i));
                    }

                    // NameText = transform.Find("Name").GetComponent<LocalizeTextMeshProUGUI>();
                }

                public void BindCardItemState(CardCollectionCardItemState cardItemState)
                {
                    CardItemState = cardItemState;
                    UpdateViewState();
                }

                public virtual void UpdateViewState()
                {
                    if (gameObject.activeInHierarchy)
                    {
                        for (var i = 0; i < StarList.Count; i++)
                        {
                            StarList[i].gameObject.SetActive(CardItemState.CardItemConfig.Level == i+1);
                        }   
                    }
                    // NameText.SetTerm(CardItemState.NameKey);
                }
            }

            public class NotHaveGroup : BaseGroup
            {
                private Image Icon;
                private Dictionary<int, Transform> BGList = new Dictionary<int, Transform>();
                public override void Awake()
                {
                    base.Awake();
                    Icon = transform.Find("Mask/Icon")?.GetComponent<Image>();
                    for (var i = 1; i <= 5; i++)
                    {
                        BGList.Add(i,transform.Find("BGGroup/"+i));
                    }
                }
                public override void UpdateViewState()
                {
                    var changeable = CardCollectionModel.Instance.GetChangeableWildCard(CardItemState) != null;
                    gameObject.SetActive((CardItemState.CollectCount == 0 && !changeable)/* || CardItemState.CollectCount > 0*/);
                    base.UpdateViewState();
                    if (gameObject.activeInHierarchy)
                    {
                        if (Icon)
                            Icon.sprite = CardItemState.GetCardSprite();
                        foreach (var pair in BGList)
                        {
                            pair.Value?.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level);
                        }   
                    }
                }
            }
            public class WildCardGroup : BaseGroup
            {
                private Image Icon;
                private Dictionary<int, Transform> BGList = new Dictionary<int, Transform>();
                public override void Awake()
                {
                    base.Awake();
                    Icon = transform.Find("Mask/Icon")?.GetComponent<Image>();
                    for (var i = 1; i <= 5; i++)
                    {
                        BGList.Add(i,transform.Find("BGGroup/"+i));
                    }
                }
                public override void UpdateViewState()
                {
                    var changeable = CardCollectionModel.Instance.GetChangeableWildCard(CardItemState) != null;
                    gameObject.SetActive(CardItemState.CollectCount == 0 && changeable);
                    base.UpdateViewState();
                    if (gameObject.activeInHierarchy)
                    {
                        if (Icon)
                            Icon.sprite = CardItemState.GetCardSprite();
                        foreach (var pair in BGList)
                        {
                            pair.Value?.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level);
                        }   
                    }
                }
            }

            public class HaveGroup : BaseGroup
            {
                private Image Icon;
                private Transform NumNode;
                private LocalizeTextMeshProUGUI NumText;
                private Transform NewFlag;
                private Dictionary<int, Transform> BGList = new Dictionary<int, Transform>();

                private Transform BGNode;
                private Transform BGUnViewNode;
                private Dictionary<int, Transform> BGUnViewList = new Dictionary<int, Transform>();
                public Button UpGradeBtn;

                public override void Awake()
                {
                    base.Awake();
                    Icon = transform.Find("Mask/Icon").GetComponent<Image>();
                    NumNode = transform.Find("Num");
                    NumText = transform.Find("Num/Text")?.GetComponent<LocalizeTextMeshProUGUI>();
                    NewFlag = transform.Find("New");
                    for (var i = 1; i <= 5; i++)
                    {
                        BGList.Add(i,transform.Find("BGGroup/"+i));
                    }
                    EventDispatcher.Instance.AddEvent<EventViewNewCard>(OnViewNewCard);

                    for (var i = 1; i <= 5; i++)
                    {
                        BGUnViewList.Add(i,transform.Find("BGGroupNotHave/"+i));
                    }
                    BGNode = transform.Find("BGGroup");
                    BGUnViewNode = transform.Find("BGGroupNotHave");
                    UpGradeBtn = transform.Find("Button")?.GetComponent<Button>();
                }

                private void OnDestroy()
                {
                    EventDispatcher.Instance.RemoveEvent<EventViewNewCard>(OnViewNewCard);
                }

                public void OnViewNewCard(EventViewNewCard evt)
                {
                    if (evt.CardItemState != CardItemState)
                        return;
                    UpdateViewState();
                }

                public override void UpdateViewState()
                {
                    gameObject.SetActive(CardItemState.CollectCount > 0);
                    base.UpdateViewState();
                    if (gameObject.activeInHierarchy)
                    {
                        if (CardItemState.CardItemConfig.MergeView)
                        {
                            Icon.sprite = CardItemState.GetCardSprite();
                            NewFlag?.gameObject.SetActive(false);
                            NumNode?.gameObject.SetActive(CardItemState.CollectCount > 1 && !CardItemState.IsUnViewed());
                            NumText?.SetText("+" + (CardItemState.CollectCount - 1));
                            BGNode.gameObject.SetActive(!CardItemState.IsUnViewed());
                            BGUnViewNode?.gameObject.SetActive(CardItemState.IsUnViewed());
                            foreach (var pair in BGList)
                            {
                                pair.Value.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level);
                            }  
                            foreach (var pair in BGUnViewList)
                            {
                                pair.Value?.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level);
                            }  
                            UpGradeBtn?.gameObject.SetActive(CardItemState.IsUnViewed());
                        }
                        else
                        {
                            Icon.sprite = CardItemState.GetCardSprite();
                            NewFlag?.gameObject.SetActive(CardItemState.IsUnViewed());
                            NumNode?.gameObject.SetActive(CardItemState.CollectCount > 1 && !CardItemState.IsUnViewed());
                            NumText?.SetText("+" + (CardItemState.CollectCount - 1));
                            foreach (var pair in BGList)
                            {
                                pair.Value.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level);
                            }      
                        }
                    }
                }
            }

            private CardCollectionCardItemState CardItemState;
            private NotHaveGroup NotHaveNode;
            private HaveGroup HaveNode;
            private WildCardGroup WildCardNode;
            private Button CardViewButton;
            private bool Changeable;
            private Transform NewCardEffectNode;
            private void Awake()
            {
                NewCardEffectNode = transform.Find("Root");
                NewCardEffectNode.gameObject.SetActive(false);
                NotHaveNode = transform.Find("NotHave").gameObject.AddComponent<NotHaveGroup>();
                HaveNode = transform.Find("Have").gameObject.AddComponent<HaveGroup>();
                WildCardNode = transform.Find("UniversalCard").gameObject.AddComponent<WildCardGroup>();
                CardViewButton = transform.GetComponent<Button>();
                if (CardViewButton)
                    CardViewButton.onClick.AddListener(OnClickCardViewButton);
                AddAllEvent();
            }

            public void PlayNewCardEffect()
            {
                NewCardEffectNode.gameObject.SetActive(true);
                XUtility.WaitSeconds(3f, () =>
                {
                    if (!this)
                        return;
                    NewCardEffectNode.gameObject.SetActive(false);
                });
            }

            private void OnDestroy()
            {
                RemoveAllEvent();
            }

            #region Event
            public void AddAllEvent()
            {
                EventDispatcher.Instance.AddEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
                EventDispatcher.Instance.AddEvent<EventWildCardCountChange>(OnWildCardCountChange);
                EventDispatcher.Instance.AddEvent<EventCardCountChange>(OnCardItemCountChange);
            }

            public void RemoveAllEvent()
            {
                EventDispatcher.Instance.RemoveEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
                EventDispatcher.Instance.RemoveEvent<EventWildCardCountChange>(OnWildCardCountChange);
                EventDispatcher.Instance.RemoveEvent<EventCardCountChange>(OnCardItemCountChange);
            }
            public void OnCardItemCountChange(EventCardCountChange evt)
            {
                if (evt.CardItem != CardItemState)
                    return;
                UpdateViewState();
            }
            public void OnCollectNewCardItem(EventCollectNewCardItem evt)
            {
                if (evt.CardItemState != CardItemState)
                    return;
                if (evt.Source == GetCardSource.WildCard)
                    PlayNewCardEffect();
                UpdateViewState();
            }
            public void OnWildCardCountChange(EventWildCardCountChange evt)
            {
                var changeable = CardItemState.CollectCount == 0 && CardCollectionModel.Instance.GetChangeableWildCard(CardItemState) != null;
                if (Changeable == changeable)
                    return;
                UpdateViewState();
            }
            #endregion

            public void UpdateViewState()
            {
                Changeable = CardItemState.CollectCount == 0 && CardCollectionModel.Instance.GetChangeableWildCard(CardItemState) != null;
                NotHaveNode.BindCardItemState(CardItemState);
                HaveNode.BindCardItemState(CardItemState);
                if (HaveNode.UpGradeBtn)
                {
                    HaveNode.UpGradeBtn.onClick.RemoveAllListeners();
                    HaveNode.UpGradeBtn.onClick.AddListener(OnClickCardViewButton);
                }
                WildCardNode.BindCardItemState(CardItemState);
            }
            public void BindCardItemState(CardCollectionCardItemState cardItemState)
            {
                CardItemState = cardItemState;
                UpdateViewState();
            }

            CardItemView CardViewController;

            public void BindCardViewController(CardItemView cardViewController)
            {
                CardViewController = cardViewController;
            }

            public void OnClickCardViewButton()
            {
                if (CardItemState.CollectCount > 0)
                {
                    if (CardItemState.CardItemConfig.MergeView && CardItemState.IsUnViewed())//表演合成
                    {
                        CardCollectionModel.Instance.OnViewCard(CardItemState);
                        //预览卡片时触发领奖
                        {
                            foreach (var cardBookState in CardItemState.CardBookStateList)
                            {
                                cardBookState.OnCollectNewCard();
                            }
                            EventDispatcher.Instance.SendEvent(new EventCollectNewCardItem(CardItemState,GetCardSource.MergeOnView));
                            // CardCollectionModel.Instance.DoAllUndoAction();
                        }
                        UIUpGradeCardController.Open(CardItemState,()=>CardCollectionModel.Instance.DoAllUndoAction());
                    }
                    else
                    {
                        CardViewController.ShowCardItemView(HaveNode);   
                    }
                }
                else
                {
                    var wildCardConfig = CardCollectionModel.Instance.GetChangeableWildCard(CardItemState);
                    if (wildCardConfig != null)
                    {
                        CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
                        {
                            DescString = LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_cardcollection_wildcard_redeem",wildCardConfig.GetWildCardMaxLevel().ToString()),
                            OKCallback = () =>
                            {
                                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryCollectCardsExchange,
                                    wildCardConfig.Id.ToString(),CardItemState.CardItemConfig.Id.ToString());
                                CardCollectionModel.Instance.ResumeWildCard(wildCardConfig.Id, 1);
                                CardItemState.CollectOneCard(GetCardSource.WildCard,"WildExchange"+wildCardConfig.Id);
                                CardCollectionModel.Instance.DoAllUndoAction();
                                
                            },
                            HasCloseButton = true,
                            HasCancelButton = true,
                            IsHighSortingOrder = true,
                        });
                    }
                    else if (CardItemState.CardItemConfig.MergeView)
                    {
                        CardViewController.ShowCardItemView(NotHaveNode);   
                    }
                }
            }
        }

        private CardCollectionCardBookState CardBookState;
        public TopGroupNode TopGroup;
        private Transform DefaultCardItem;
        private List<CardItem> CardBookItemList = new List<CardItem>();

        public void Awake()
        {
            TopGroup = transform.Find("TopGroup").gameObject.AddComponent<TopGroupNode>();
            DefaultCardItem = transform.Find("Card");
            DefaultCardItem.gameObject.SetActive(false);
            AddAllEvent();
        }

        private void OnDestroy()
        {
            RemoveAllEvent();
        }

        #region Event
        public void AddAllEvent()
        {
            EventDispatcher.Instance.AddEvent<EventCardBookComplete>(OnCardBookComplete);
            EventDispatcher.Instance.AddEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
        }

        public void RemoveAllEvent()
        {
            EventDispatcher.Instance.RemoveEvent<EventCardBookComplete>(OnCardBookComplete);
            EventDispatcher.Instance.RemoveEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
        }
        public void OnCardBookComplete(EventCardBookComplete evt)
        {
            if (evt.CardBookState != CardBookState)
                return;
            // UpdateViewState();
            TopGroup.BindCardBookState(CardBookState);
        }
        public void OnCollectNewCardItem(EventCollectNewCardItem evt)
        {
            if (!CardBookState.CardItemStateList.ContainsKey(evt.CardItemState.CardItemConfig.Id))
                return;
            TopGroup.BindCardBookState(CardBookState);
        }
        #endregion
        
        public CardItemView CardViewController;
        public void BindCardViewController(CardItemView cardViewController)
        {
            CardViewController = cardViewController;
        }
        public void BindCardBookState(CardCollectionCardBookState cardBookState)
        {
            CardBookState = cardBookState;
            UpdateViewState();
        }

        public void UpdateViewState()
        {
            // foreach (var pair in CardBookItemDictionary)
            //     Destroy(pair.Value.gameObject);
            // CardBookItemDictionary.Clear();
            
            for (var i = CardBookItemList.Count; i < CardBookState.CardBookConfig.Cards.Count; i++)
            {
                var cardItemObj = Instantiate(DefaultCardItem.gameObject, DefaultCardItem.parent);
                cardItemObj.name = "CardItem" + i;
                cardItemObj.SetActive(true);
                var cardItem = cardItemObj.AddComponent<CardItem>();
                cardItem.BindCardViewController(CardViewController);
                CardBookItemList.Add( cardItem);
            }
            for (var i = CardBookState.CardBookConfig.Cards.Count; i < CardBookItemList.Count; i++)
            {
                CardBookItemList[i].gameObject.SetActive(false);
            }

            var cardItemIndex = 0;
            for (var i = 0; i < CardBookState.CardBookConfig.Cards.Count; i++)
            {
                var cardItemId = CardBookState.CardBookConfig.Cards[i];
                var cardItemState = CardBookState.CardItemStateList[cardItemId];
                var cardItem = CardBookItemList[cardItemIndex];
                cardItemIndex++;
                cardItem.gameObject.SetActive(true);
                cardItem.BindCardItemState(cardItemState);
                // CardCollectionModel.Instance.OnViewCard(cardItemState);
            }

            TopGroup.BindCardBookState(CardBookState);
        }

        // private bool IsView = false;
        // public void OnViewPage()
        // {
        //     // if (IsView)
        //     //     return;
        //     // IsView = true;
        //     for (var i = 0; i < CardBookState.CardBookConfig.cards.Length; i++)
        //     {
        //         var cardItemId = CardBookState.CardBookConfig.cards[i];
        //         var cardItemState = CardBookState.CardItemStateList[cardItemId];
        //         CardCollectionModel.Instance.OnViewCard(cardItemState);
        //     }
        // }
    }
}