using System;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIShopPopup")]
    public class ShopWholePopup : ShopBaseView
    {
        public override Action EmptyCloseAction => CloseOnClick;

        [ComponentBinder("CloseButton")] private Button closeButton;
        [ComponentBinder("MoreButton")] private Transform moreButton;

        public override void OnViewOpen(UIViewParam param)
        {
            ShopBaseViewParam drivedParam = new ShopBaseViewParam();
            drivedParam.onlyPopUp = false;
            param = drivedParam;

            base.OnViewOpen(param);

            moreButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(CloseOnClick);

            Init();
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<ShopWholePopup>();
        }
    }
}