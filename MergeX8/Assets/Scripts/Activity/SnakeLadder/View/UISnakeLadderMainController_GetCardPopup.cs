using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public partial class UISnakeLadderMainController
{
    private GetCardPopup ShowCard;
    private Transform DefaultCard;
    private bool InitGetCardPopupFlag = false;
    public void InitGetCardPopup()
    {
        if (InitGetCardPopupFlag)
            return;
        InitGetCardPopupFlag = true;
        DefaultCard = GetItem<Transform>("Root/Card");
        DefaultCard.gameObject.SetActive(false);
        EventDispatcher.Instance.AddEvent<EventSnakeLadderUIGetCard>(ShowGetCardPopup);
    }

    public void ShowGetCardPopup(EventSnakeLadderUIGetCard evt)
    {
        var cardConfig = evt.CardConfig;
        Action<Action> performAction = (callback) =>
        {
            PopupGetCard(cardConfig, 1, callback);
        };
        PushPerformAction(performAction);
    }
    public void ReleaseGetCardPopup()
    {
        EventDispatcher.Instance.RemoveEvent<EventSnakeLadderUIGetCard>(ShowGetCardPopup);
    }
    public void PopupGetCard(SnakeLadderCardConfig cardConfig, int multi,Action callback)
    {
        if (ShowCard && ShowCard.WaitClick)
        {
            ShowCard.OnClickCloseBtn();
        }
        var sortingLayerId = canvas.sortingLayerID;
        var sortingOrder = canvas.sortingOrder;
        var newCard = Instantiate(DefaultCard, DefaultCard.parent);
        newCard.gameObject.SetActive(true);
        var cardCanvas = newCard.gameObject.AddComponent<Canvas>();
        cardCanvas.overrideSorting = true;
        cardCanvas.sortingLayerID = sortingLayerId;
        cardCanvas.sortingOrder = sortingOrder + 1;
        newCard.gameObject.AddComponent<GraphicRaycaster>();
        var getCardPopup = newCard.gameObject.AddComponent<GetCardPopup>();
        getCardPopup.PopupCard(cardConfig, multi,this,callback);
        ShowCard = getCardPopup;
        AudioManager.Instance.PlaySound(115);
    }

    public class GetCardPopup : MonoBehaviour
    {
        private Button CloseBtn;
        private Image Icon;
        private LocalizeTextMeshProUGUI TitleText;
        private LocalizeTextMeshProUGUI DescribeText;
        private Animator Animator;
        private Transform ScoreMulti;
        // private LocalizeTextMeshProUGUI ScoreMultiText;
        private Transform StepMulti;
        private LocalizeTextMeshProUGUI StepMultiText;
        private Transform Wild;
        private Transform Score;
        private LocalizeTextMeshProUGUI ScoreText;
        private Transform Defense;
        private LocalizeTextMeshProUGUI DefenseText;
        private void Awake()
        {
            CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
            CloseBtn.onClick.AddListener(OnClickCloseBtn);
            Icon = transform.Find("Root/Content/Icon").GetComponent<Image>();
            ScoreMulti = transform.Find("Root/Content/NumGroup/1");
            // ScoreMultiText = ScoreMulti.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            StepMulti = transform.Find("Root/Content/NumGroup/2");
            StepMultiText = StepMulti.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            Wild = transform.Find("Root/Content/NumGroup/3");
            Score = transform.Find("Root/Content/NumGroup/4");
            ScoreText = Score.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            Defense = transform.Find("Root/Content/NumGroup/5");
            DefenseText = Defense.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            TitleText = transform.Find("Root/Content/TextName").GetComponent<LocalizeTextMeshProUGUI>();
            DescribeText = transform.Find("Root/Content/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Animator = transform.GetComponent<Animator>();
        }

        public bool WaitClick = false;
        public void OnClickCloseBtn()
        {
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SnakeLadderMainGetCard);
            CloseBtn.interactable = false;
            WaitClick = false;

            Action SendEvent = () =>
            {
                Callback?.Invoke();
            };
            var cardType = (SnakeLadderCardType) CardConfig.CardType;
            if (cardType == SnakeLadderCardType.Score)
            {
                MainUI.FlyCarrot(Score.position,CardConfig.Score);
                transform.DOScale(0f, 0.5f).OnComplete(() =>
                {
                    Destroy(gameObject);
                    SendEvent();
                });
            }
            else if (cardType == SnakeLadderCardType.MultiScore ||
                     cardType == SnakeLadderCardType.MultiStep)
            {
                transform.DOScale(0f, 0.5f).OnComplete(() =>
                {
                    Destroy(gameObject);
                    SendEvent();
                });
            }
            else if(cardType == SnakeLadderCardType.Wild || cardType == SnakeLadderCardType.Defense)
            {
                var closeTime = 0.5f;
                transform.DOScale(0.2f, closeTime);
                var cardState = new SnakeLadderCardState(CardConfig);
                var cardSelection = MainUI.CardNodeList[cardState];
                var targetPos = cardSelection.transform.position;
                transform.DOMove(targetPos, closeTime).OnComplete(() =>
                {
                    Destroy(gameObject);
                    if (cardType == SnakeLadderCardType.Wild && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderGuideWildCard))
                    {
                        var wildCardState = new SnakeLadderCardState(SnakeLadderCardType.Wild, 0);
                        var wildCardNode = MainUI.CardNodeList[wildCardState].transform;
                        List<Transform> topLayer = new List<Transform>();
                        topLayer.Add(wildCardNode);
                        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderGuideWildCard, wildCardNode.transform as RectTransform, topLayer: topLayer);
                        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderGuideWildCard,null);
                    }
                    SendEvent();
                });
            }
        }

        public void ResetView()
        {
            ScoreMulti.gameObject.SetActive(false);
            StepMulti.gameObject.SetActive(false);
            Wild.gameObject.SetActive(false);
            Score.gameObject.SetActive(false);
            Defense.gameObject.SetActive(false);
            
            CloseBtn.interactable = true;
            WaitClick = true;
            var cardType = (SnakeLadderCardType) CardConfig.CardType;
            if (cardType == SnakeLadderCardType.Score)
            {
                var scoreValue = CardConfig.Score/* * Multi*/;
                Score.gameObject.SetActive(true);
                ScoreText.SetText(scoreValue.ToString());
            }
            else if (cardType == SnakeLadderCardType.MultiScore)
            {
                ScoreMulti.gameObject.SetActive(true);
                // ScoreMultiText.SetText("x" + CardConfig.ScoreMultiValue);
            }
            else if (cardType == SnakeLadderCardType.MultiStep)
            {
                StepMulti.gameObject.SetActive(true);
                StepMultiText.SetText("x" + CardConfig.StepMultiValue);
            }
            else if (cardType == SnakeLadderCardType.Wild)
            {
                Wild.gameObject.SetActive(true);
            }
            else if (cardType == SnakeLadderCardType.Defense)
            {
                Defense.gameObject.SetActive(true);
                DefenseText.SetText("x1");
            }

            Icon.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.SnakeLadderAtlas, CardConfig.AssetName);
            TitleText.SetTerm(CardConfig.TitleText);
            if (cardType == SnakeLadderCardType.Score)
            {
                var scoreValue = CardConfig.Score/* * Multi*/;
                DescribeText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat(CardConfig.DescribeText,scoreValue.ToString()));
            }
            else
            {
                DescribeText.SetTerm(CardConfig.DescribeText);
            }
        }

        private SnakeLadderCardConfig CardConfig;
        private int Multi;
        public UISnakeLadderMainController MainUI;
        private Action Callback;
        public async void PopupCard(SnakeLadderCardConfig cardConfig, int multi,UISnakeLadderMainController mainUI,Action callback)
        {
            Callback = callback;
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
                    if (UISnakeLadderMainController.IsAuto)
                    {
                        OnClickCloseBtn();
                    }
                    else
                    {
                        var cardType = (SnakeLadderCardType) CardConfig.CardType;
                        if (cardType == SnakeLadderCardType.Wild && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderGetWildCard))
                        {
                            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderGetWildCard,null);
                        }
                    }
                }
            });
            // if ((SnakeLadderCardType) CardConfig.CardType == SnakeLadderCardType.MultiScore || 
            //     (SnakeLadderCardType) CardConfig.CardType == SnakeLadderCardType.ExtraBall)
            // {
            //     if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderMainGetCard))
            //     {
            //         {
            //             List<Transform> topLayer = new List<Transform>();
            //             topLayer.Add(transform);
            //             GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderMainGetCard, transform as RectTransform,
            //                 topLayer: topLayer);
            //         }
            //         if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderMainGetCard, null))
            //         {
            //             var cardState = new SnakeLadderCardState(CardConfig);
            //             {
            //                 List<Transform> topLayer = new List<Transform>();
            //                 topLayer.Add(MainUI.Game.CardSelectionList[cardState].transform);
            //                 GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderMainGuideSelectCard, MainUI.Game.CardSelectionList[cardState].transform as RectTransform,
            //                     topLayer: topLayer);
            //             }
            //         }
            //     }
            // }
        }
    }
}