using System;
using System.Linq;
using System.Resources;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public partial class UICardMainController
{
    public class WildCardItem:MonoBehaviour
    {
        
        public TableCardCollectionWildCard WildCardConfig;
        private Button Button;
        private Image IconImage;
        private Transform RedPoint;
        private LocalizeTextMeshProUGUI RedPointText;
        private int WildCardCount => CardCollectionModel.Instance.GetWildCardCount(WildCardConfig.Id);
        private void Awake()
        {
            Button = transform.gameObject.GetComponent<Button>();
            Button.onClick.AddListener(OnClickButton);
            IconImage = transform.Find("Icon").GetComponent<Image>();
            RedPoint = transform.Find("RedPoint");
            RedPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
            AddAllEvent();
        }
        WildCardView CardViewController;
        public void BindCardViewController(WildCardView cardViewController)
        {
            CardViewController = cardViewController;
        }

        public void OnClickButton()
        {
            CardViewController.ShowCardItemView(this);
        }

        private void OnDestroy()
        {
            RemoveAllEvent();
        }

        public void BindWildCardState(TableCardCollectionWildCard wildCardConfig)
        {
            WildCardConfig = wildCardConfig;
            UpdateViewState();
        }

        public void UpdateViewState()
        {
            gameObject.SetActive(WildCardCount > 0);
            RedPoint.gameObject.SetActive(WildCardCount > 0);
            RedPointText.SetText(WildCardCount.ToString());
            IconImage.sprite = WildCardConfig.GetWildCardSprite();
        }
        #region Event
        public void AddAllEvent()
        {
            EventDispatcher.Instance.AddEvent<EventWildCardCountChange>(OnWildCardCountChange);
        }
        public void RemoveAllEvent()
        {
            EventDispatcher.Instance.RemoveEvent<EventWildCardCountChange>(OnWildCardCountChange);
        }
        public void OnWildCardCountChange(EventWildCardCountChange evt)
        {
            if (evt.WildCard != WildCardConfig)
                return;
            UpdateViewState();
        }
        #endregion
    }
}