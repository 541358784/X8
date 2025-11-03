using System;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIShopPopup")]
    public class ShopPartPopup : ShopBaseView
    {
        public override Action EmptyCloseAction => CloseOnClick;

        [ComponentBinder("CloseButton")] private Button closeButton;
        [ComponentBinder("MoreButton")] private Button moreButton;

        [ComponentBinder("CurrencyCoin")] private Transform coin;

        public override void OnViewOpen(UIViewParam param)
        {
            ShopBaseViewParam drivedParam = new ShopBaseViewParam();
            drivedParam.onlyPopUp = true;
            param = drivedParam;

            base.OnViewOpen(param);

            closeButton.onClick.AddListener(CloseOnClick);
            moreButton.onClick.AddListener(MoreOnClick);

            coin.gameObject.AddComponent<CoinNum>();

            Init(true);
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<ShopPartPopup>();
        }

        private void MoreOnClick()
        {
            CloseOnClick();
            UIViewSystem.Instance.Open<ShopWholePopup>(new ShopBaseViewParam() { onlyPopUp = false });
        }
    }
}