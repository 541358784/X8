using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using UnityEngine;
using UnityEngine.UI;

public partial class UICardMainController
{
    public class WildCardSingleView : MonoBehaviour
    {
        private Image Icon;
        private TableCardCollectionWildCard CardItemState;
        private LocalizeTextMeshProUGUI NumText;
        public virtual void Awake()
        {
            Icon = transform.Find("Icon").GetComponent<Image>();
            NumText = transform.Find("Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void BindCardItemState(TableCardCollectionWildCard cardItemState)
        {
            CardItemState = cardItemState;
            UpdateViewState();
        }
        public virtual void UpdateViewState()
        {
            var count = CardCollectionModel.Instance.GetWildCardCount(CardItemState.Id);
            var maxLevel = CardItemState.GetWildCardMaxLevel();
            Icon.sprite = CardItemState.GetWildCardSprite();
            NumText.SetText("X"+(count));
        }
    }
    public class WildCardView:MonoBehaviour
    {
        private Button CloseButton;
        private WildCardItem BindCardGroup;
        private WildCardSingleView ViewCardGroup;
        private Vector3 ViewCardInitGroupLocalPosition;
        private Vector3 ViewCardInitGroupLocalScale;
        private Vector3 SmallScale;
        private Image BlackMask;
        private float MaskInitAlpha;
        private float FadeTime = 0.5f;
        private float MoveTime = 0.5f;
        private LocalizeTextMeshProUGUI TextTitle;
        private LocalizeTextMeshProUGUI Text;
        
        private void Awake()
        {
            TextTitle = transform.Find("Root/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
            Text = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
            CloseButton = transform.Find("Root/CloseButton").GetComponent<Button>();
            CloseButton.onClick.AddListener(OnClickCloseButton);
            ViewCardGroup = transform.Find("Root/Have").gameObject.AddComponent<WildCardSingleView>();
            ViewCardInitGroupLocalPosition = ViewCardGroup.transform.localPosition;
            ViewCardInitGroupLocalScale = ViewCardGroup.transform.localScale;
            BlackMask = transform.GetComponent<Image>();
            MaskInitAlpha = BlackMask.color.a;
            SmallScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        
        public void ShowCardItemView(WildCardItem wildCardItem)
        {
            if (BindCardGroup != null)
            {
                return;
            }
            BindCardGroup = wildCardItem;
            gameObject.SetActive(true);
            BindCardGroup.gameObject.SetActive(false);
            ViewCardGroup.BindCardItemState(BindCardGroup.WildCardConfig);
            ViewCardGroup.gameObject.SetActive(true);
            ViewCardGroup.transform.localScale = SmallScale;
            ViewCardGroup.transform.position = BindCardGroup.transform.position;
            {
                var tempColor = BlackMask.color;
                tempColor.a = 0;
                BlackMask.color = tempColor;
                BlackMask.DOFade(MaskInitAlpha, FadeTime);
            }
            {
                var tempColor = TextTitle.GetTmpText().color;
                tempColor.a = 0;
                TextTitle.GetTmpText().color = tempColor;
                TextTitle.GetTmpText().DOFade(1, FadeTime);
            }
            {
                var tempColor = Text.GetTmpText().color;
                tempColor.a = 0;
                Text.GetTmpText().color = tempColor;
                Text.GetTmpText().DOFade(1, FadeTime);
            }
            ViewCardGroup.transform.DOLocalMove(ViewCardInitGroupLocalPosition, MoveTime);
            ViewCardGroup.transform.DOScale(ViewCardInitGroupLocalScale, MoveTime);
            CloseButton.gameObject.SetActive(true);
        }
        public void OnClickCloseButton()
        {
            CloseButton.gameObject.SetActive(false);
            BlackMask.DOKill();
            BlackMask.DOFade(0, FadeTime);
            TextTitle.GetTmpText().DOKill();
            TextTitle.GetTmpText().DOFade(0, FadeTime);
            Text.GetTmpText().DOKill();
            Text.GetTmpText().DOFade(0, FadeTime);
            ViewCardGroup.transform.DOKill();
            ViewCardGroup.transform.DOScale(SmallScale, MoveTime);
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
    }
}