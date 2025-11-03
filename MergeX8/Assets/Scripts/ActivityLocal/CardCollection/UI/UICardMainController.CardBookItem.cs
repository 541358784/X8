using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public partial class UICardMainController
{
    public class CardBookItem:MonoBehaviour
    {
        private Dictionary<int, Transform> BackBoardList = new Dictionary<int, Transform>();
        private CardCollectionCardBookState CardBookState;
        private Image CardBookIcon;
        private Slider CollectProgress;
        private LocalizeTextMeshProUGUI CollectProgressText;
        private LocalizeTextMeshProUGUI CardBookName;
        private Transform CompletedTip;
        private Button CardBookButton;
        private Transform NewCardFlag;
        private CardCollectionCardThemeState _cardThemeState;
        
        private void Awake()
        {
            BackBoardList.Add(1,transform.Find("BGGroup/Blue"));
            BackBoardList.Add(2,transform.Find("BGGroup/Purple"));
            BackBoardList.Add(3,transform.Find("BGGroup/Gold"));
            CardBookIcon = transform.Find("Mask/Icon").GetComponent<Image>();
            CollectProgress = transform.Find("Slider").GetComponent<Slider>();
            CollectProgressText = transform.Find("SliderText").GetComponent<LocalizeTextMeshProUGUI>();
            CardBookName = transform.Find("NameText").GetComponent<LocalizeTextMeshProUGUI>();
            CompletedTip = transform.Find("Completed");
            CardBookButton = transform.GetComponent<Button>();
            CardBookButton.onClick.AddListener(OnClickCardBookButton);
            NewCardFlag = transform.Find("New");
            AddAllEvent();
        }

        private void OnDestroy()
        {
            RemoveAllEvent();
        }

        public void BindCardBookState(CardCollectionCardBookState cardBookState, CardCollectionCardThemeState cardThemeState)
        {
            _cardThemeState = cardThemeState;
            CardBookState = cardBookState;
            UpdateViewState();
        }

        public void UpdateViewState()
        {
            foreach (var pair in BackBoardList)
            {
                pair.Value.gameObject.SetActive(CardBookState.CardBookConfig.Level == pair.Key);
            }
            
            CardBookIcon.sprite = CardBookState.GetIconSprite();
            CollectProgress.maxValue = CardBookState.MaxCardItemCount;
            CollectProgress.value = CardBookState.CollectCardItemCount;
            CollectProgressText.SetText(CardBookState.CollectCardItemCount+"/"+CardBookState.MaxCardItemCount);
            
            
            CardBookName.SetTerm(CardBookState.NameKey);
            CompletedTip.gameObject.SetActive(CardBookState.IsCompleted);
            CollectProgress.gameObject.SetActive(!CardBookState.IsCompleted);
            CollectProgressText.gameObject.SetActive(!CardBookState.IsCompleted);
            NewCardFlag.gameObject.SetActive(CardBookState.GetCardBookUnViewedCardCount() > 0);
        }
        public void OnClickCardBookButton()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CardClickCardBook);
            UICardBookController.Open(CardBookState);
        }
        
        #region Event
        public void AddAllEvent()
        {
            EventDispatcher.Instance.AddEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
            EventDispatcher.Instance.AddEvent<EventCardBookComplete>(OnCardBookComplete);
            EventDispatcher.Instance.AddEvent<EventViewNewCard>(OnViewNewCard);
        }

        public void RemoveAllEvent()
        {
            EventDispatcher.Instance.RemoveEvent<EventCollectNewCardItem>(OnCollectNewCardItem);
            EventDispatcher.Instance.RemoveEvent<EventCardBookComplete>(OnCardBookComplete);
            EventDispatcher.Instance.RemoveEvent<EventViewNewCard>(OnViewNewCard);
        }
        public void OnCardBookComplete(EventCardBookComplete evt)
        {
            if (evt.CardBookState != CardBookState)
                return;
            UpdateViewState();
        }

        public void OnCollectNewCardItem(EventCollectNewCardItem evt)
        {
            if (!CardBookState.CardItemStateList.ContainsKey(evt.CardItemState.CardItemConfig.Id))
                return;
            CollectProgress.value = CardBookState.CollectCardItemCount;
            CollectProgressText.SetText(CardBookState.CollectCardItemCount+"/"+CardBookState.MaxCardItemCount);
            NewCardFlag.gameObject.SetActive(CardBookState.GetCardBookUnViewedCardCount() > 0);
        }

        public void OnViewNewCard(EventViewNewCard evt)
        {
            if (!CardBookState.CardItemStateList.ContainsKey(evt.CardItemState.CardItemConfig.Id))
                return;
            NewCardFlag.gameObject.SetActive(CardBookState.GetCardBookUnViewedCardCount() > 0);
        }
        #endregion
    }
}