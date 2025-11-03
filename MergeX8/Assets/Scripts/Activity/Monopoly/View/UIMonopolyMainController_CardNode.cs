using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    private Dictionary<MonopolyCardState,CardNode> CardNodeList = new Dictionary<MonopolyCardState,CardNode>();
    private bool InitCardNodeFlag = false;
    public void InitCardNode()
    {
        if (InitCardNodeFlag)
            return;
        InitCardNodeFlag = true;
        {
            var cardNode = transform.Find("Root/ButtonFixed").gameObject.AddComponent<CardNode>();
            var cardState = new MonopolyCardState(MonopolyCardType.Wild, 0);
            cardNode.MainUI = this;
            cardNode.SetCardState(cardState);
            CardNodeList.Add(cardState,cardNode);
        }
    }
    public class CardNode:MonoBehaviour
    {
        public UIMonopolyMainController MainUI;
        private MonopolyCardState CardState;
        private LocalizeTextMeshProUGUI CardCountText;
        private Button CardBtn;
        private int CardCount => MainUI.Storage.GetUnUsedCardCount(CardState);

        private void Awake()
        {
            CardBtn = transform.gameObject.GetComponent<Button>();
            CardBtn.onClick.AddListener(OnClickCard);
            CardCountText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEvent<EventMonopolyCardCountChange>(CardCountChange);
        }

        public void CardCountChange(EventMonopolyCardCountChange evt)
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
            EventDispatcher.Instance.RemoveEvent<EventMonopolyCardCountChange>(CardCountChange);
        }

        public void OnClickCard()
        {
            // if (CardState.CardType == MonopolyCardType.Wild)
            // {
            //     GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyGuideWildCard);
            // }
            if (CardState.CardType == MonopolyCardType.Wild)
            {
                if (CardCount > 0)
                {
                    if (MainUI.isPlaying)
                        return;
                    UIPopupMonopolyUseCardController.Open(MainUI.Storage);   
                }
                else
                {
                    var tip = transform.Find("Tip");
                    tip.DOKill();
                    tip.gameObject.SetActive(false);
                    tip.gameObject.SetActive(true);
                    DOVirtual.DelayedCall(2f, () => tip.gameObject.SetActive(false)).SetTarget(tip);
                }
            }
        }

        public void SetCardState(MonopolyCardState cardState)
        {
            CardState = cardState;
            CardCountText.SetText(CardCount.ToString());
        }
    }
}