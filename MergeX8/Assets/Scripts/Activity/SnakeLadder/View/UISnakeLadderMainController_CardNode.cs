using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class UISnakeLadderMainController
{
    private Dictionary<SnakeLadderCardState,CardNode> CardNodeList = new Dictionary<SnakeLadderCardState,CardNode>();
    private bool InitCardNodeFlag = false;
    public void InitCardNode()
    {
        if (InitCardNodeFlag)
            return;
        InitCardNodeFlag = true;
        {
            var cardNode = transform.Find("Root/CardSlot /1").gameObject.AddComponent<CardNode>();
            var cardState = new SnakeLadderCardState(SnakeLadderCardType.Wild, 0);
            cardNode.MainUI = this;
            cardNode.SetCardState(cardState);
            CardNodeList.Add(cardState,cardNode);
        }
        {
            var cardNode = transform.Find("Root/CardSlot /2").gameObject.AddComponent<CardNode>();
            var cardState = new SnakeLadderCardState(SnakeLadderCardType.Defense, 0);
            cardNode.MainUI = this;
            cardNode.SetCardState(cardState);
            CardNodeList.Add(cardState,cardNode);
        }
    }
    public class CardNode:MonoBehaviour
    {
        public UISnakeLadderMainController MainUI;
        private SnakeLadderCardState CardState;
        private LocalizeTextMeshProUGUI CardCountText;
        private Button CardBtn;
        private int CardCount => MainUI.Storage.GetUnUsedCardCount(CardState);

        private void Awake()
        {
            CardBtn = transform.gameObject.GetComponent<Button>();
            CardBtn.onClick.AddListener(OnClickCard);
            CardCountText = transform.Find("LeftCount/LeftCountText").GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEvent<EventSnakeLadderCardCountChange>(CardCountChange);
        }

        public void CardCountChange(EventSnakeLadderCardCountChange evt)
        {
            Action<Action> performAction = (callback) =>
            {
                if (evt.CardState == CardState)
                {
                    CardCountText.SetText(evt.TotalCount.ToString());
                }
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderCardCountChange>(CardCountChange);
        }

        public void OnClickCard()
        {
            if (CardState.CardType == SnakeLadderCardType.Wild)
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SnakeLadderGuideWildCard);
            }
            if (CardState.CardType == SnakeLadderCardType.Wild && CardCount > 0)
            {
                UIPopupSnakeLadderUseCardController.Open(MainUI.Storage);
            }
        }

        public void SetCardState(SnakeLadderCardState cardState)
        {
            CardState = cardState;
            CardCountText.SetText(CardCount.ToString());
        }
    }
}