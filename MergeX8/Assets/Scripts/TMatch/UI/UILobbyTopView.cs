using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class UILobbyTopView : UIView
    {
        [ComponentBinder("CurrencyLive")] private Transform live;
        [ComponentBinder("CurrencyCoin")] private Transform coin;
        [ComponentBinder("CurrencyStars")] private Transform stars;
        [ComponentBinder("ButtonSet")] public Button setButton;

        protected override bool IsChildView => true;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            live.gameObject.AddComponent<LifeNumber>();
            coin.gameObject.AddComponent<CoinNum>();
            stars.gameObject.SetActive(false);
            setButton.onClick.AddListener(OnSetButtonClicked);

            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyMainShowState, Show);
        }

        public override void OnViewDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyMainShowState, Show);
            base.OnViewDestroy();
        }

        private void OnSetButtonClicked()
        {
            // UIViewSystem.Instance.Open<UISettingPopup>();
        }

        public void Show(BaseEvent evt)
        {
            LobbyMainShowStateEvent realEvt = evt as LobbyMainShowStateEvent;
            if (realEvt.enable)
            {
                DOTween.Kill(transform.GetComponent<RectTransform>());
                transform.GetComponent<RectTransform>().DOAnchorPosY(0.0f, 0.3f);
            }
            else
            {
                DOTween.Kill(transform.GetComponent<RectTransform>());
                transform.GetComponent<RectTransform>().DOAnchorPosY(180f, 0.3f);
            }
        }
    }
}