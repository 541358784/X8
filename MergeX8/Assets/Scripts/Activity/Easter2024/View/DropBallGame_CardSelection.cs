using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class DropBallGame
{
    public Dictionary<Easter2024CardState, CardSelection> CardSelectionList = new Dictionary<Easter2024CardState, CardSelection>();
    private Easter2024CardState _selectCardState=Easter2024CardState.NoCard;
    public Easter2024CardState SelectCardState
    {
        get => _selectCardState;
        set
        {
            if (CardSelectionList.TryGetValue(SelectCardState, out var oldSelection))
            {
                oldSelection.OnUnSelectCard();
            }
            _selectCardState = value;
            if (CardSelectionList.TryGetValue(SelectCardState, out var newSelection))
            {
                newSelection.OnSelectCard();
            }
            MainUI.UpdateAutoBtnEnable();
        }
    }

    public bool IsSelectCard => SelectCardState != Easter2024CardState.NoCard;
    public void OnSelectCard(Easter2024CardState cardState)
    {
        if (SelectCardState == cardState)
        {
            SelectCardState = Easter2024CardState.NoCard;
        }
        else
        {
            if (Storage.GetUnUsedCardCount(cardState) > 0)
                SelectCardState = cardState;
            if (IsAuto)
                MainUI.SetAutoState(false);
        }
    }

    private bool InitCardSelectionFlag = false;
    public void InitCardSelectionGroup()
    {
        if (InitCardSelectionFlag)
            return;
        InitCardSelectionFlag = true;
        var cardSelectionIndex = 0;
        foreach (var cardPair in Easter2024Model.Instance.CardConfig)
        {
            var cardConfig = cardPair.Value;
            var cardType = (Easter2024CardType) cardConfig.CardType;
            if (cardType == Easter2024CardType.MultiScore ||
                cardType == Easter2024CardType.ExtraBall)
            {
                var cardState = new Easter2024CardState(cardConfig);
                var selection = MainUI.CardSelectionList[cardSelectionIndex].gameObject.AddComponent<CardSelection>();
                cardSelectionIndex++;
                selection.Init(this,cardState);
                CardSelectionList.Add(cardState,selection);
            }
        }
    }
    public class CardSelection : MonoBehaviour
    {
        public DropBallGame Game;
        public Easter2024CardState CardState;
        private Button SelectButton;
        private Transform SelectEffect;
        // private Transform CanUseEffect;
        private LocalizeTextMeshProUGUI CountText;
        private Transform RedPoint;
        private Image Icon;
        private Color EnableColor = Color.white;
        private Color DisableColor = Color.grey;
        private int CardCount => Game.Storage.GetUnUsedCardCount(CardState);

        private void Awake()
        {
            SelectButton = transform.GetComponent<Button>();
            SelectButton.onClick.AddListener(OnClick);
            SelectEffect = transform.Find("Selected");
            SelectEffect.gameObject.SetActive(false);
            Icon = transform.Find("Filling").GetComponent<Image>();
            RedPoint = transform.Find("LeftCount");
            CountText = transform.Find("LeftCount/LeftCountText").GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEvent<EventEaster2024CardCountChange>(OnCardCountChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventEaster2024CardCountChange>(OnCardCountChange);
        }

        public void OnCardCountChange(EventEaster2024CardCountChange evt)
        {
            if (evt.CardState == CardState)
            {
                UpdateUI();
            }
        }
        public void Init(DropBallGame game,Easter2024CardState cardState)
        {
            Game = game;
            CardState = cardState;
            UpdateUI();
            
        }
        public bool OnSelectCard()
        {
            if (CardCount == 0)
            {
                return false;
            }
            SelectEffect.gameObject.SetActive(true);
            if (!Game.IsInWaiting)
            {
                if (CardState.CardType == Easter2024CardType.ExtraBall)
                {
                    Game.OnSelectExtraBallCard(CardState.BallCount);
                    if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024MainSelectCardDescribeExtra))
                    {
                        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024MainSelectCardDescribeExtra,null))
                        {
                            GuideSubSystem.Instance.ForceFinished(728);
                        }
                    }
                }
                else if(CardState.CardType == Easter2024CardType.MultiScore)
                {
                    Game.OnSelectMultiScoreCard(CardState.MultiValue);
                    if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024MainSelectCardDescribeMulti))
                    {
                        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024MainSelectCardDescribeMulti,null))
                        {
                            GuideSubSystem.Instance.ForceFinished(729);
                        }
                    }
                }   
            }
            return true;
        }

        public void OnUnSelectCard()
        {
            SelectEffect.gameObject.SetActive(false);
            if (CardState.CardType == Easter2024CardType.ExtraBall)
            {
                Game.OnUnSelectExtraBallCard();
            }
            else if(CardState.CardType == Easter2024CardType.MultiScore)
            {
                Game.OnUnSelectMultiScoreCard();
            }
        }
        public void UpdateUI()
        {
            var cardCount = CardCount;
            RedPoint.gameObject.SetActive(cardCount > 0);
            CountText.SetText(cardCount.ToString());
            // CanUseEffect.gameObject.SetActive(CardCount>0);
            Icon.sprite = CardState.GetSprite();
            Icon.color = cardCount > 0 ? EnableColor : DisableColor;
        }

        public void OnClick()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024MainGuideSelectCard);
            Game.OnSelectCard(CardState);
        }
    }
}