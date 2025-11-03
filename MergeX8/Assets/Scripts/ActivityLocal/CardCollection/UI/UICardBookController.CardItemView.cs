using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class UICardBookController
{
    public class CardItemView:MonoBehaviour
    {
        private Button CloseButton;
        private CardBookPage.CardItem.BaseGroup BindCardGroup;
        private ViewGroup ViewCardGroup;
        private Vector3 ViewCardInitGroupLocalPosition;
        private Vector3 ViewCardInitGroupLocalScale;
        private Image BlackMask;
        private float MaskInitAlpha;
        private float FadeTime = 0.5f;
        private float MoveTime = 0.5f;
        private void Awake()
        {
            CloseButton = transform.Find("Root/CloseButton").GetComponent<Button>();
            CloseButton.onClick.AddListener(OnClickCloseButton);
            ViewCardGroup = transform.Find("Root/Have").gameObject.AddComponent<ViewGroup>();
            ViewCardInitGroupLocalPosition = ViewCardGroup.transform.localPosition;
            ViewCardInitGroupLocalScale = ViewCardGroup.transform.localScale;
            BlackMask = transform.GetComponent<Image>();
            MaskInitAlpha = BlackMask.color.a;
        }
        
        public void ShowCardItemView(CardBookPage.CardItem.BaseGroup haveGroup)
        {
            if (BindCardGroup != null)
            {
                return;
            }
            BindCardGroup = haveGroup;
            gameObject.SetActive(true);
            ViewCardGroup.BindCardItemState(BindCardGroup.CardItemState);
            ViewCardGroup.gameObject.SetActive(true);
            ViewCardGroup.transform.localScale = Vector3.one;
            ViewCardGroup.transform.position = BindCardGroup.transform.position;
            var tempColor = BlackMask.color;
            tempColor.a = 0;
            BlackMask.color = tempColor;
            BlackMask.DOFade(MaskInitAlpha, FadeTime);
            ViewCardGroup.transform.DOLocalMove(ViewCardInitGroupLocalPosition, MoveTime);
            ViewCardGroup.transform.DOScale(ViewCardInitGroupLocalScale, MoveTime);
            CloseButton.gameObject.SetActive(true);
            CardCollectionModel.Instance.OnViewCard(BindCardGroup.CardItemState);
            BindCardGroup.gameObject.SetActive(false);
        }
        public void OnClickCloseButton()
        {
            CloseButton.gameObject.SetActive(false);
            BlackMask.DOKill();
            BlackMask.DOFade(0, FadeTime);
            ViewCardGroup.transform.DOKill();
            ViewCardGroup.transform.DOScale(Vector3.one, MoveTime);
            var lastProgress = 0f;
            DOTween.To(() => 0f, progress =>
            {
                var distance = BindCardGroup.transform.position - ViewCardGroup.transform.position;
                var percent = (progress - lastProgress) / (1 - lastProgress);
                ViewCardGroup.transform.position += distance * percent;
                lastProgress = progress;
            }, 1f, MoveTime).OnComplete(() =>
            {
                ViewCardGroup.transform.position = BindCardGroup.transform.position;
                BindCardGroup.gameObject.SetActive(true);
                gameObject.SetActive(false);
                BindCardGroup = null;
            }).SetTarget(ViewCardGroup.transform);
        }
        
        
        public class ViewGroup : MonoBehaviour
            {
                private Image Icon;
                private Dictionary<int, Transform> BGList = new Dictionary<int, Transform>();
                private Dictionary<int, Transform> BGEmptyList = new Dictionary<int, Transform>();
                // private Transform ExtraCardView;
                public CardCollectionCardItemState CardItemState;
                public List<Transform> StarList = new List<Transform>();
                // private LocalizeTextMeshProUGUI NameText;

                public void Awake()
                {
                    for (var i = 1; transform.Find("Star/" + i); i++)
                    {
                        StarList.Add(transform.Find("Star/" + i));
                    }
                    // NameText = transform.Find("Name").GetComponent<LocalizeTextMeshProUGUI>();
                    Icon = transform.Find("Mask/Icon").GetComponent<Image>();
                    for (var i = 1; i <= 5; i++)
                    {
                        BGList.Add(i,transform.Find("BGGroup/"+i));
                    }
                    for (var i = 1; i <= 5; i++)
                    {
                        BGEmptyList.Add(i,transform.Find("BGGroupEmpty/"+i));
                    }
                    EventDispatcher.Instance.AddEvent<EventViewNewCard>(OnViewNewCard);
                    // ExtraCardView = transform.Find("Many");
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
                public void BindCardItemState(CardCollectionCardItemState cardItemState)
                {
                    CardItemState = cardItemState;
                    UpdateViewState();
                }
                public void UpdateViewState()
                {
                    if (gameObject.activeInHierarchy)
                    {
                        for (var i = 0; i < StarList.Count; i++)
                        {
                            StarList[i].gameObject.SetActive(CardItemState.CardItemConfig.Level == i+1);
                        }   
                    }
                    // NameText.SetTerm(CardItemState.NameKey);
                    Icon.sprite = CardItemState.GetCardSprite();
                    foreach (var pair in BGList)
                    {
                        pair.Value.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level && CardItemState.CollectCount > 0);
                    }   
                    foreach (var pair in BGEmptyList)
                    {
                        pair.Value?.gameObject.SetActive(pair.Key == CardItemState.CardItemConfig.Level && CardItemState.CollectCount == 0);
                    }   
                }
            }
    }
}