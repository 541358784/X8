using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SignalRCore.Messages;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonPlus.UI;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public static partial class EventEnum
{
    public const string TeamChatUpdate = "TeamChatUpdate";
}

public class EventTeamChatUpdate : BaseEvent
{
    public bool HasChange;
    public EventTeamChatUpdate() : base(EventEnum.TeamChatUpdate) { }

    public EventTeamChatUpdate(bool hasChange) : base(EventEnum.TeamChatUpdate)
    {
        HasChange = hasChange;
    }
}
public partial class UIPopupGuildMainController
{
    public class ChatGroup : MonoBehaviour
    {
        public ScrollRectRecycleController ScrollRecycleController;
        public ScrollTopDetector scrollRectTrigger;
        private UIPopupGuildMainController Controller;

        public void OnChatUpdate(EventTeamChatUpdate e)
        {
            if (e.HasChange)
                InitMessageGroup();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventTeamChatUpdate>(OnChatUpdate);
        }
        public void Init(UIPopupGuildMainController controller)
        {
            EventDispatcher.Instance.RemoveEvent<EventTeamChatUpdate>(OnChatUpdate);
            EventDispatcher.Instance.AddEvent<EventTeamChatUpdate>(OnChatUpdate);
            Controller = controller;
            // contentNode = transform.Find("Scroll View/Viewport/Content") as RectTransform;
            PoolNode = new GameObject("Pool").transform;
            PoolNode.SetParent(transform,false);
            PoolNode.gameObject.SetActive(false);
            DefaultItemMy = transform.Find("Scroll View/Viewport/Content/My");
            DefaultItemMy.gameObject.SetActive(false);
            DefaultItemOther = transform.Find("Scroll View/Viewport/Content/Other");
            DefaultItemOther.gameObject.SetActive(false);
            DefaultItemSystem = transform.Find("Scroll View/Viewport/Content/System");
            DefaultItemSystem.gameObject.SetActive(false);
            // InitMessageGroup();
            InitHeartGroup();
            InitFilterGroup();
            InitInputGroup();
            scrollRectTrigger = transform.Find("Scroll View").gameObject.AddComponent<ScrollTopDetector>();
            scrollRectTrigger.OnOverBottom(() =>
            {
                TeamManager.Instance.GetTeamChats(LoadChatsType.Forward, (s) =>
                {
                    if (!s)
                        return;
                    SetContentToBottom();
                });
            });
            scrollRectTrigger.OnOverTop(() =>
            {
                // var lastTopNode = MessageItemList.First();
                // var lastTopNodePosY = (lastTopNode.transform as RectTransform).anchoredPosition.y;
                TeamManager.Instance.GetTeamChats(LoadChatsType.Backward, (s) =>
                {
                    if (!s)
                        return;
                    // if (lastTopNode != null)
                    // {
                    //     var curTopNodePosY = (lastTopNode.transform as RectTransform).anchoredPosition.y;
                    //     var distance = curTopNodePosY - lastTopNodePosY;
                    //     contentNode.SetAnchorPositionY(contentNode.anchoredPosition.y - distance);
                    // }
                    SetContentToTop();
                });
            });
            ScrollRecycleController = transform.Find("Scroll View").gameObject
                .AddComponent<ScrollRectRecycleController>();
            
            ScrollRecycleController.BindGetItemFunc((message) => CreateMessageItem(message as RecvContent).transform as RectTransform);
            ScrollRecycleController.BindRecycleItemFunc(RecycleMessageItem);

            InitMessageGroup();
            SetContentToBottom(true);
            TeamManager.Instance.GetTeamChats(LoadChatsType.Forward, (s) =>
            {
                if (!s)
                    return;
                // SetContentToBottom();
            });
            TeamManager.Instance.GetBattlePassGiftList((b) =>
            {
                if (!b)
                    return;
                // SetContentToBottom();
            });
        }

        #region 消息显示
        private Transform DefaultItemMy;
        private Transform DefaultItemOther;
        private Transform DefaultItemSystem;
        public List<MessageItemBase> MessageItemTotalPool = new List<MessageItemBase>();
        public List<MessageItemBase> MessageItemVisiblePool = new List<MessageItemBase>();
        public Transform PoolNode;
        public List<MessageItemCard> MessageItemMyCardPool = new List<MessageItemCard>();
        public List<MessageItemCard> MessageItemOtherCardPool = new List<MessageItemCard>();
        public List<MessageItemText> MessageItemMyTextPool = new List<MessageItemText>();
        public List<MessageItemText> MessageItemOtherTextPool = new List<MessageItemText>();
        public List<MessageItemSystem> MessageItemSystemPool = new List<MessageItemSystem>();
        // public RectTransform contentNode;
        public void InitMessageGroup()
        {
            var objectList = new List<object>();
            foreach (var message in TeamManager.Instance.ChatMessages)
            {
                objectList.Add(message);
            }
            ScrollRecycleController.SetData(objectList);
            ScrollRecycleController.RebuildWithFocusIndex(objectList.Count-1,0.2f);
            // return;

            // var removeCount = 0;
            // var newCount = 0;
            // var messages = TeamManager.Instance.ChatMessages;
            // var messageIndex = 0;
            // for (messageIndex = 0; messageIndex < messages.Count; messageIndex++)
            // {
            //     var message = messages[messageIndex];
            //     var findItemIndex = -1;
            //     for (var tempViewItemIndex = messageIndex; tempViewItemIndex < MessageItemList.Count; tempViewItemIndex++)
            //     {
            //         if (MessageItemList[tempViewItemIndex].MessageContent.ChatId == message.ChatId)
            //         {
            //             findItemIndex = tempViewItemIndex;
            //             break;
            //         }
            //     }
            //
            //     if (findItemIndex == -1)//没找到对应的消息块，创建并插入
            //     {
            //         newCount++;
            //         var messageItem = CreateMessageItem(message);
            //         if (messageItem != null)
            //         {
            //             if (messageIndex == MessageItemList.Count)
            //             {
            //                 MessageItemList.Add(messageItem);
            //             }
            //             else
            //             {
            //                 MessageItemList.Insert(messageIndex,messageItem);
            //             }
            //         }
            //         messageItem.transform.SetSiblingIndex(messageIndex);
            //     }
            //     else
            //     {
            //         if (findItemIndex != messageIndex)//有不在消息列表中的消息块，删除
            //         {
            //             for (var i = messageIndex; i < findItemIndex; i++)
            //             {
            //                 removeCount++;
            //                 DestroyImmediate(MessageItemList[i].gameObject);
            //             }
            //             MessageItemList.RemoveRange(messageIndex,findItemIndex-messageIndex);   
            //         }
            //     }
            //     
            // }
            //
            // if (messageIndex < MessageItemList.Count)//界面状态和数据状态完全不衔接，清掉界面残留
            // {
            //     for (var i = messageIndex; i < MessageItemList.Count; i++)
            //     {
            //         removeCount++;
            //         DestroyImmediate(MessageItemList[i].gameObject);
            //     }
            //     MessageItemList.RemoveRange(messageIndex, MessageItemList.Count-messageIndex); 
            // }
            
            // LayoutRebuilder.ForceRebuildLayoutImmediate(contentNode as RectTransform);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(contentNode.parent as RectTransform);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(contentNode.parent.parent as RectTransform);
            ChangeFilterType(CurFilterType);
            Controller.CheckCardPackageGuide();
        }

        public MessageItemSystem GetSystemItem()
        {
            if (MessageItemSystemPool.Count > 0)
            {
                var item = MessageItemSystemPool.Pop();
                MessageItemVisiblePool.Add(item);
                return item;
            }
            else
            {
                var messageItem = Instantiate(DefaultItemSystem, PoolNode).gameObject.AddComponent<MessageItemSystem>();
                messageItem.gameObject.SetActive(true);
                MessageItemTotalPool.Add(messageItem);
                MessageItemVisiblePool.Add(messageItem);
                return messageItem;
            }
        }
        public MessageItemText GetMyTextItem()
        {
            if (MessageItemMyTextPool.Count > 0)
            {
                var item = MessageItemMyTextPool.Pop();
                MessageItemVisiblePool.Add(item);
                return item;
            }
            else
            {
                var messageItem = Instantiate(DefaultItemMy, PoolNode).gameObject.AddComponent<MessageItemText>();
                messageItem.gameObject.SetActive(true);
                MessageItemTotalPool.Add(messageItem);
                MessageItemVisiblePool.Add(messageItem);
                return messageItem;
            }
        }
        public MessageItemText GetOtherTextItem()
        {
            if (MessageItemOtherTextPool.Count > 0)
            {
                var item = MessageItemOtherTextPool.Pop();
                MessageItemVisiblePool.Add(item);
                return item;
            }
            else
            {
                var messageItem = Instantiate(DefaultItemOther, PoolNode).gameObject.AddComponent<MessageItemText>();
                messageItem.gameObject.SetActive(true);
                MessageItemTotalPool.Add(messageItem);
                MessageItemVisiblePool.Add(messageItem);
                return messageItem;
            }
        }
        public MessageItemCard GetMyCardItem()
        {
            if (MessageItemMyCardPool.Count > 0)
            {
                var item = MessageItemMyCardPool.Pop();
                MessageItemVisiblePool.Add(item);
                return item;
            }
            else
            {
                var messageItem = Instantiate(DefaultItemMy, PoolNode).gameObject.AddComponent<MessageItemCard>();
                messageItem.gameObject.SetActive(true);
                MessageItemTotalPool.Add(messageItem);
                MessageItemVisiblePool.Add(messageItem);
                return messageItem;
            }
        }
        public MessageItemCard GetOtherCardItem()
        {
            if (MessageItemOtherCardPool.Count > 0)
            {
                var item = MessageItemOtherCardPool.Pop();
                MessageItemVisiblePool.Add(item);
                return item;
            }
            else
            {
                var messageItem = Instantiate(DefaultItemOther, PoolNode).gameObject.AddComponent<MessageItemCard>();
                messageItem.gameObject.SetActive(true);
                MessageItemTotalPool.Add(messageItem);
                MessageItemVisiblePool.Add(messageItem);
                return messageItem;
            }
        }

        public void RecycleMessageItem(RectTransform messageRect)
        {
            var messageItem = messageRect.GetComponent<MessageItemBase>();
            MessageItemVisiblePool.Remove(messageItem);
            messageRect.SetParent(PoolNode,false);
            var messageItemSystem = messageItem as MessageItemSystem;
            if (messageItemSystem != null)
            {
                MessageItemSystemPool.Add(messageItemSystem);
                return;
            }
            var messageItemText = messageItem as MessageItemText;
            if (messageItemText != null)
            {
                if (messageItemText.IsMyMessage)
                {
                    MessageItemMyTextPool.Add(messageItemText);
                }
                else
                {
                    MessageItemOtherTextPool.Add(messageItemText);
                }
                return;
            }
            var messageItemCard = messageItem as MessageItemCard;
            if (messageItemCard != null)
            {
                if (messageItemCard.IsMyMessage)
                {
                    MessageItemMyCardPool.Add(messageItemCard);
                }
                else
                {
                    MessageItemOtherCardPool.Add(messageItemCard);
                }
                return;
            }
        }
        public MessageItemBase CreateMessageItem(RecvContent message)
        {
            MessageItemBase messageItem = null;
            if (message.ChatType == ChatType.SystemMessage)
            {
                messageItem = GetSystemItem();
                messageItem.Init(message);
            }
            else if (message.ChatType == ChatType.UserMessage)
            {
                if (message.UserId == TeamManager.Instance.MyPlayerId)
                {
                    messageItem = GetMyTextItem();
                    messageItem.Init(message);
                }
                else
                {
                    messageItem = GetOtherTextItem();
                    messageItem.Init(message);
                }
                if (messageItem != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(messageItem.transform.Find("Text") as RectTransform);
                }
            }
            else if(message.ChatType == ChatType.TeamPassGift)
            {
                if (message.UserId == TeamManager.Instance.MyPlayerId)
                {
                    messageItem = GetMyCardItem();
                    messageItem.Init(message);
                }
                else
                {
                    messageItem = GetOtherCardItem();
                    messageItem.Init(message);
                }
                if (messageItem != null)
                { 
                    LayoutRebuilder.ForceRebuildLayoutImmediate(messageItem.transform.Find("Card/CardGroup/1-4") as RectTransform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(messageItem.transform.Find("Card/CardGroup/5-7") as RectTransform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(messageItem.transform.Find("Card/CardGroup") as RectTransform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(messageItem.transform.Find("Card") as RectTransform);
                }
            }

            if (messageItem != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(messageItem.transform as RectTransform);
            }
            return messageItem;
        }
        
        public class MessageItemSystem : MessageItemBase
        {
            public LinkImageText Text;
            public override void Init(RecvContent message)
            {
                base.Init(message);
                Text = transform.Find("Text").GetComponent<LinkImageText>();
                // Text.text = message.Content;
                var systemMessageType = (TeamChatMessageType)message.ContentType;
                if (systemMessageType == TeamChatMessageType.TeamCreate)
                {
                    Text.text = message.Content + LocalizationManager.Instance.GetLocalizedString("ui_team_system_info_CreatTeam");
                }
                else if (systemMessageType == TeamChatMessageType.TeamJoin)
                {
                    Text.text = message.Content + LocalizationManager.Instance.GetLocalizedString("ui_team_system_info_JoinTeam");
                }
                else if (systemMessageType == TeamChatMessageType.TeamKick)
                {
                    Text.text = message.Content + LocalizationManager.Instance.GetLocalizedString("ui_team_system_info_KickedTeam");
                }
                else if (systemMessageType == TeamChatMessageType.TeamLeave)
                {
                    Text.text = message.Content + LocalizationManager.Instance.GetLocalizedString("ui_team_system_info_QuitTeam");
                }
                else if (systemMessageType == TeamChatMessageType.TeamChangeLeader)
                {
                    Text.text = message.Content + LocalizationManager.Instance.GetLocalizedString("ui_team_system_info_BecomeLead");
                }
                else
                {
                    Text.text = message.Content + systemMessageType;
                }
            }
        }
        public class MessageItemBase:MonoBehaviour
        {

            public RecvContent MessageContent;
            public ChatType ChatType => MessageContent.ChatType;
            public bool IsMyMessage => MessageContent.UserId == TeamManager.Instance.MyPlayerId;

            public virtual void Init(RecvContent message)
            {
                MessageContent = message;
            }
        }

        public class MessageItemMember : MessageItemBase
        {
            public HeadIconNode HeadIconNode;
            public Transform TextGroup;
            public Transform CardGroup;
            public PlayerInfoExtra ExtraData;
            public override void Init(RecvContent message)
            {
                base.Init(message);
                ExtraData = PlayerInfoExtra.FromJson(message.Extra);
                var avatar = new AvatarViewState(ExtraData.AvatarIcon, ExtraData.AvatarFrameIcon, ExtraData.UserName, IsMyMessage);
                if (HeadIconNode)
                {
                    
                    HeadIconNode.SetAvatarViewState(avatar);
                }
                else
                {
                    HeadIconNode =
                        HeadIconNode.BuildHeadIconNode(transform.Find("HeadGroup/Head") as RectTransform, avatar);   
                }
                if (!TextGroup)
                    TextGroup = transform.Find("Text");
                if (!CardGroup)
                    CardGroup = transform.Find("Card");
            }
        }

        public class MessageItemText : MessageItemMember
        {
            public LinkImageText NameText;
            public LinkImageText Text;
            public override void Init(RecvContent message)
            {
                base.Init(message);
                CardGroup.gameObject.SetActive(false);
                TextGroup.gameObject.SetActive(true);
                if (!NameText)
                    NameText = transform.Find("Text/NameText").GetComponent<LinkImageText>();
                if (!Text)
                    Text = transform.Find("Text/Text").GetComponent<LinkImageText>();
                NameText.text = ExtraData.UserName;
                Text.text = MessageContent.Content;

            }
        }
        public class MessageItemCard : MessageItemMember
        {
            public int ContentCount = 4;
            public LinkImageText NameText;
            public List<Transform> ContentList = new List<Transform>();
            public List<CardItem> CardItemList = new List<CardItem>();
            private LocalizeTextMeshProUGUI PriceText;
            public override void Init(RecvContent message)
            {
                base.Init(message);
                CardGroup.gameObject.SetActive(true);
                TextGroup.gameObject.SetActive(false);
                if (!NameText)
                    NameText = transform.Find("Card/NameText").GetComponent<LinkImageText>();
                NameText.text = ExtraData.UserName;

                if (ContentList.Count == 0)
                {
                    ContentList.Add(transform.Find("Card/CardGroup/1-4"));
                    ContentList.Add(transform.Find("Card/CardGroup/5-7"));   
                }

                var defaultCardItem = transform.Find("Card/CardGroup/1-4/Card");
                defaultCardItem.gameObject.SetActive(false);

                var giftBagId = TeamManager.Instance.ConvertChatIdToPassGiftId(message.ChatId);
                var cardData = TeamManager.Instance.GetPassGift(giftBagId);
                for (var i = 0; i < cardData.ExtraData.CardList.Count; i++)
                {
                    var cardId = cardData.ExtraData.CardList[i];
                    
                    
                    
                    var cardDataExtra = cardData.ExtraData;
                    var theme =  CardCollectionModel.Instance.GetCardThemeState(CardCollectionModel.Instance.TableCardPackage[cardDataExtra.CardPackageId].ThemeId);
                    var themeList = CardCollectionModel.Instance.GetCardThemeLink(theme.CardThemeConfig.Id);
                    var cutTheme = CardCollectionModel.Instance.ThemeInUse.GetUpGradeTheme();

                    // var cardList = cardDataExtra.CardList.DeepCopy();
                    if (themeList.Contains(cutTheme.CardThemeConfig.Id))//重定位cardId到当前开启卡册主题
                    {
                        cardId = cardId - theme.CardThemeConfig.Id * 1000 + cutTheme.CardThemeConfig.Id * 1000;
                    }
                    
                    
                    
                    
                    var contentIndex = i / ContentCount;
                    var content = ContentList[contentIndex];
                    content.gameObject.SetActive(true);
                    if (i < CardItemList.Count)
                    {
                        CardItemList[i].gameObject.SetActive(true);
                        CardItemList[i].Init(cardId);
                    }
                    else
                    {
                        var cardObj = Instantiate(defaultCardItem, content);
                        cardObj.gameObject.SetActive(true);
                        var cardItem = cardObj.gameObject.AddComponent<CardItem>();
                        cardItem.Init(cardId);
                        CardItemList.Add(cardItem);   
                    }
                }
                for (var i = cardData.ExtraData.CardList.Count; i < CardItemList.Count; i++)
                {
                    CardItemList[i].gameObject.SetActive(false);
                }
                
                transform.GetComponent<Button>()?.onClick.RemoveAllListeners();
                transform.GetComponent<Button>()?.onClick.AddListener(() =>
                {
                    if (cardData.PlayerId != (long)TeamManager.Instance.MyPlayerId)
                    {
                        UIPopupGuildCardGetController.Open(cardData);   
                    }
                });
                PriceText = transform.Find("Card/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
                PriceText.SetText(cardData.ExtraData.Price.ToString());
                transform.Find("Card/Num").gameObject.SetActive(!IsMyMessage);
            }


            public class CardItem : MonoBehaviour
            {
                private Image Icon;
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
                        bg.gameObject.SetActive(CardItemState.CardItemConfig.Level == starIndex);
                        star.gameObject.SetActive(CardItemState.CardItemConfig.Level == starIndex);
                        starIndex++;
                    }
                    HaveFlag = transform.Find("Have");
                    HaveFlag.gameObject.SetActive(CardItemState.CollectCount > 0);
                }
            }
        }
        #endregion

        #region 小心心

        private LocalizeTextMeshProUGUI HeartTimeText;
        private LocalizeTextMeshProUGUI HeartExtraText;
        private List<Transform> HeartList = new List<Transform>();
        public void InitHeartGroup()
        {
            HeartTimeText = transform.Find("Heart/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            for (var i = 1; i <= TeamConfigManager.Instance.LocalTeamConfig.MaxLife; i++)
            {
                HeartList.Add(transform.Find("Heart/Group/"+i+"/Full"));
            }
            HeartExtraText = transform.Find("Heart/Group/Text").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("UpdateHeart",0,1);
        }

        public void UpdateHeart()
        {
            var curLife = TeamManager.Instance.GetLife();
            for (var i = 0; i < HeartList.Count; i++)
            {
                HeartList[i].gameObject.SetActive(curLife > i);
            }

            var extraLife = curLife - TeamConfigManager.Instance.LocalTeamConfig.MaxLife;
            HeartExtraText.gameObject.SetActive(extraLife > 0);
            HeartExtraText.SetText("+"+extraLife);
            if (TeamManager.Instance.Storage.Life >= TeamConfigManager.Instance.LocalTeamConfig.MaxLife)
                HeartTimeText.SetText("Full");
            else
            {
                var leftAddLifeTime = TeamManager.Instance.Storage.LifeUpdateTime +
                                      TeamConfigManager.Instance.LocalTeamConfig.LifeRecoverTime * (long)XUtility.Min -
                                      (long)APIManager.Instance.GetServerTime();
                HeartTimeText.SetText(CommonUtils.FormatLongToTimeStr(leftAddLifeTime));   
            }
        }
        #endregion

        #region 过滤

        public Transform FilterTipNode;
        public FilterType CurFilterType = FilterType.None;
        private Dictionary<FilterType, FilterItem> FilterItemDic = new Dictionary<FilterType, FilterItem>();
        public void InitFilterGroup()
        {
            FilterTipNode = transform.Find("FilterTip");
            FilterTipNode.gameObject.SetActive(false);
            transform.Find("Filter").GetComponent<Button>().onClick.AddListener(() =>
            {
                FilterTipNode.gameObject.SetActive(!FilterTipNode.gameObject.activeSelf);
            });
            {
                var filterItem = transform.Find("FilterTip/All").gameObject.AddComponent<FilterItem>();
                filterItem.Init(FilterType.All,new List<ChatType>(){ChatType.SystemMessage,ChatType.UserMessage,ChatType.TeamPassGift},this);
                filterItem.Select(false);
                FilterItemDic.Add(FilterType.All,filterItem);
            }
            {
                var filterItem = transform.Find("FilterTip/Card").gameObject.AddComponent<FilterItem>();
                filterItem.Init(FilterType.Card,new List<ChatType>(){ChatType.TeamPassGift},this);
                filterItem.Select(false);
                FilterItemDic.Add(FilterType.Card,filterItem);
            }
            {
                var filterItem = transform.Find("FilterTip/System").gameObject.AddComponent<FilterItem>();
                filterItem.Init(FilterType.System,new List<ChatType>(){ChatType.SystemMessage},this);
                filterItem.Select(false);
                FilterItemDic.Add(FilterType.System,filterItem);
            }
            ChangeFilterType(FilterType.All);
        }

        public void ChangeFilterType(FilterType type)
        {
            if (CurFilterType != type)
            {
                if (FilterItemDic.ContainsKey(CurFilterType))
                    FilterItemDic[CurFilterType]?.Select(false);
                CurFilterType = type;
                if (FilterItemDic.ContainsKey(CurFilterType))
                    FilterItemDic[CurFilterType]?.Select(true);   
            }
            if (FilterItemDic.ContainsKey(CurFilterType))
            {
                var chatTypes = FilterItemDic[CurFilterType].ChatTypes;
                foreach (var messageItem in MessageItemTotalPool)
                {
                    messageItem.gameObject.SetActive(chatTypes.Contains(messageItem.ChatType));
                }  
            }
            else
            {
                foreach (var messageItem in MessageItemTotalPool)
                {
                    messageItem.gameObject.SetActive(true);
                }  
            }
            
        }
        public enum FilterType
        {
            None,
            All,
            Card,
            System,
        }
        public class FilterItem : MonoBehaviour
        {
            public FilterType FilterType;
            public List<ChatType> ChatTypes;
            public LocalizeTextMeshProUGUI Lebel;
            public Transform SelectFlag;

            public void Init(FilterType filterType,List<ChatType> chatTypes,ChatGroup group)
            {
                FilterType = filterType;
                ChatTypes = chatTypes;
                Lebel = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                SelectFlag = transform.Find("Selected");
                SelectFlag.gameObject.SetActive(group.CurFilterType == FilterType);
                transform.GetComponent<Button>()?.onClick.AddListener(() =>
                {
                    group.ChangeFilterType(FilterType);
                });
            }

            public void Select(bool isSelect)
            {
                SelectFlag.gameObject.SetActive(isSelect);
            }

        }

        #endregion

        #region 输入框

        private InputField Input;
        public void InitInputGroup()
        {
            Input = transform.Find("Send/InputField").GetComponent<InputField>();
            transform.Find("Send/X").GetComponent<Button>().onClick.AddListener(() =>
            {
                Input.SetTextWithoutNotify("");
            });
            transform.Find("Send/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!Input.text.IsEmptyString())
                {
                    TeamManager.Instance.SendTeamChat(Input.text, (b) =>
                    {
                        if (b)
                        {
                            TeamManager.Instance.GetTeamChats(LoadChatsType.Forward, (s) =>
                            {
                                if (!s)
                                    return;
                                SetContentToBottom();
                            });   
                        }
                    });
                    Input.SetTextWithoutNotify("");
                }
            });
        }

        #endregion

        private float MoveTime = 0.3f;
        public void SetContentToTop(bool quick = false)
        {
            if (GuideSubSystem.Instance.IsShowingGuide())
                return;
            // scrollRectTrigger.scrollRect.enabled = false;
            // var content = contentNode;
            // var anchorPosY = 0;
            // content.DOKill();
            // if (quick)
            // {
            //     content.SetAnchorPositionY(anchorPosY);
            // }
            // else
            // {
            //     content.DOAnchorPosY(anchorPosY,MoveTime).SetEase(Ease.Linear);   
            // }
            // scrollRectTrigger.scrollRect.enabled = true;
            ScrollRecycleController.RebuildWithFocusIndex(0,1);
        }

        public void SetContentToBottom(bool quick = false)
        {
            if (GuideSubSystem.Instance.IsShowingGuide())
                return;
            // scrollRectTrigger.scrollRect.enabled = false;
            // var content = contentNode;
            // var scrollRectHeight = (transform.Find("Scroll View") as RectTransform).GetHeight();
            // var anchorPosY = content.GetHeight() - scrollRectHeight;
            // if (anchorPosY < 0)
            //     anchorPosY = 0;
            // content.DOKill();
            // if (quick)
            // {
            //     content.SetAnchorPositionY(anchorPosY);
            // }
            // else
            // {
            //     content.DOAnchorPosY(anchorPosY,MoveTime).SetEase(Ease.Linear);   
            // }
            // scrollRectTrigger.scrollRect.enabled = true;
            ScrollRecycleController.RebuildWithFocusIndex(ScrollRecycleController.DataList.Count-1,0.2f);
        }

        // public void SetContentToTarget(MessageItemBase messageItem,bool quick = false)
        // {
        //     var content = contentNode;
        //     var scrollRectHeight = (transform.Find("Scroll View") as RectTransform).GetHeight();
        //     var anchorPosY = -(messageItem.transform as RectTransform).anchoredPosition.y - scrollRectHeight;
        //     if (anchorPosY < 0)
        //         anchorPosY = 0;
        //     content.DOKill();
        //     if (quick)
        //     {
        //         content.SetAnchorPositionY(anchorPosY);
        //     }
        //     else
        //     {
        //         content.DOAnchorPosY(anchorPosY,MoveTime).SetEase(Ease.Linear);   
        //     }
        // }
    }
}