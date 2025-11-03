using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public partial class UIEaster2024MainController
{
    private GetCardPopup ShowCard;
    public void PopupGetCard(Easter2024CardConfig cardConfig, int multi)
    {
        if (ShowCard && ShowCard.WaitClick)
        {
            ShowCard.OnClickCloseBtn();
        }
        var newCard = Instantiate(DefaultCard, DefaultCard.parent);
        newCard.gameObject.SetActive(true);
        newCard.GetComponent<Canvas>().overrideSorting = true;
        newCard.GetComponent<Canvas>().sortingOrder = DefaultCard.GetComponent<Canvas>().sortingOrder;
        var getCardPopup = newCard.gameObject.AddComponent<GetCardPopup>();
        getCardPopup.PopupCard(cardConfig, multi,this);
        ShowCard = getCardPopup;
    }

    public class GetCardPopup : MonoBehaviour
    {
        private Button CloseBtn;
        private Image Icon;
        private Transform Carrot;
        private Transform PuroleEgg;
        private Transform GoldEgg;
        private LocalizeTextMeshProUGUI NumText;
        private LocalizeTextMeshProUGUI TitleText;
        private LocalizeTextMeshProUGUI DescribeText;
        private Animator Animator;

        private void Awake()
        {
            CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
            CloseBtn.onClick.AddListener(OnClickCloseBtn);
            Icon = transform.Find("Root/Content/Icon").GetComponent<Image>();
            Carrot = transform.Find("Root/Content/NumGroup/1");
            PuroleEgg = transform.Find("Root/Content/NumGroup/2");
            GoldEgg = transform.Find("Root/Content/NumGroup/3");
            NumText = transform.Find("Root/Content/NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            TitleText = transform.Find("Root/Content/TextName").GetComponent<LocalizeTextMeshProUGUI>();
            DescribeText = transform.Find("Root/Content/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Animator = transform.GetComponent<Animator>();
        }

        public bool WaitClick = false;
        public void OnClickCloseBtn()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Easter2024MainGetCard);
            CloseBtn.interactable = false;
            WaitClick = false;

            Action SendEvent = () =>
            {
                switch ((Easter2024CardType)CardConfig.CardType)
                {
                    case Easter2024CardType.ExtraBall:
                    {
                        var cardState = new Easter2024CardState(Easter2024CardType.ExtraBall, CardConfig.BallCount);
                        EventDispatcher.Instance.SendEvent(new EventEaster2024CardCountChange(cardState,1));
                        break;
                    }
                    case Easter2024CardType.MultiScore:
                    {
                        var cardState = new Easter2024CardState(Easter2024CardType.MultiScore, CardConfig.MultiValue);
                        EventDispatcher.Instance.SendEvent(new EventEaster2024CardCountChange(cardState,1));
                        break;
                    }
                }
            };
            var cardType = (Easter2024CardType) CardConfig.CardType;
            if (cardType == Easter2024CardType.Score)
            {
                MainUI.FlyCarrot(Carrot.position,CardConfig.Score);
                transform.DOScale(0f, 0.5f).OnComplete(() =>
                {
                    Destroy(gameObject);
                    SendEvent();
                });
            }
            else
            {
                var closeTime = 0.5f;
                transform.DOScale(0.2f, closeTime);
                var cardState = new Easter2024CardState(CardConfig);
                var cardSelection = MainUI.Game.CardSelectionList[cardState];
                var targetPos = cardSelection.transform.position;
                transform.DOMove(targetPos, closeTime).OnComplete(() =>
                {
                    Destroy(gameObject);
                    SendEvent();
                });
            }
        }

        public void ResetView()
        {
            Carrot.gameObject.SetActive(false);
            PuroleEgg.gameObject.SetActive(false);
            GoldEgg.gameObject.SetActive(false);
            CloseBtn.interactable = true;
            WaitClick = true;
            var cardType = (Easter2024CardType) CardConfig.CardType;
            if (cardType == Easter2024CardType.Score)
            {
                var scoreValue = CardConfig.Score/* * Multi*/;
                Carrot.gameObject.SetActive(true);
                NumText.SetText(scoreValue.ToString());
            }
            else if (cardType == Easter2024CardType.MultiScore)
            {
                Carrot.gameObject.SetActive(true);
                NumText.SetText("x" + CardConfig.MultiValue);
            }
            else if (cardType == Easter2024CardType.ExtraBall)
            {
                GoldEgg.gameObject.SetActive(true);
                NumText.SetText("x" + CardConfig.BallCount);
            }

            Icon.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.Easter2024Atlas, CardConfig.AssetName);
            TitleText.SetTerm(CardConfig.TitleText);
            if (cardType == Easter2024CardType.Score)
            {
                var scoreValue = CardConfig.Score/* * Multi*/;
                DescribeText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat(CardConfig.DescribeText,scoreValue.ToString()));
            }
            else
            {
                DescribeText.SetTerm(CardConfig.DescribeText);
            }
        }

        private Easter2024CardConfig CardConfig;
        private int Multi;
        public UIEaster2024MainController MainUI;

        public async void PopupCard(Easter2024CardConfig cardConfig, int multi,UIEaster2024MainController mainUI)
        {
            MainUI = mainUI;
            CardConfig = cardConfig;
            Multi = multi;
            ResetView();
            CloseBtn.interactable = false;
            Animator.PlayAnimation("open",()=>
            {
                if (!this)
                    return;
                if (WaitClick)
                {
                    CloseBtn.interactable = true;   
                }
            });
            if ((Easter2024CardType) CardConfig.CardType == Easter2024CardType.MultiScore || 
                (Easter2024CardType) CardConfig.CardType == Easter2024CardType.ExtraBall)
            {
                if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Easter2024MainGetCard))
                {
                    {
                        List<Transform> topLayer = new List<Transform>();
                        topLayer.Add(transform);
                        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024MainGetCard, transform as RectTransform,
                            topLayer: topLayer);
                    }
                    if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Easter2024MainGetCard, null))
                    {
                        var cardState = new Easter2024CardState(CardConfig);
                        {
                            List<Transform> topLayer = new List<Transform>();
                            topLayer.Add(MainUI.Game.CardSelectionList[cardState].transform);
                            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Easter2024MainGuideSelectCard, MainUI.Game.CardSelectionList[cardState].transform as RectTransform,
                                topLayer: topLayer);
                        }
                    }
                }
            }
        }
    }
}