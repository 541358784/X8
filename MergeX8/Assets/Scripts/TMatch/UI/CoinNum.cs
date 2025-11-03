using System.Collections.Generic;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class CoinNum : ResBar
    {
        private static List<CoinNum> viewList = new List<CoinNum>();

        public static Transform GetTopView()
        {
            if (viewList.Count > 0) return viewList[viewList.Count - 1]._icon.transform;
            return null;
        }

        private void Awake()
        {
            viewList.Add(this);
            _resourceId = ResourceId.TMCoin;
            _itemType = ItemType.TMCoin;
            _icon = transform.Find("Icon").GetComponent<Image>();
            transform.GetComponent<Button>().onClick.AddListener(onAddButtonClick);

            UpdateNumber(false);
            EventDispatcher.Instance.AddEventListener(EventEnum.CurrencyFlyAniEnd, CurrencyFlyAniEnd);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < viewList.Count; i++)
            {
                if (viewList[i] == this)
                {
                    viewList.Remove(this);
                    break;
                }
            }

            EventDispatcher.Instance.RemoveEventListener(EventEnum.CurrencyFlyAniEnd, CurrencyFlyAniEnd);
        }

        protected override int CurrentCount
        {
            get { return ItemModel.Instance.GetNum((int)_resourceId); }
        }

        protected override TextMeshProUGUI CountText
        {
            get => transform.Find("CountText").GetComponent<TextMeshProUGUI>();
        }

        public override void HideAdd()
        {
            transform.GetComponent<Button>().enabled = false;
            transform.Find("BuyImage").gameObject.SetActive(false);
        }

        protected override bool TypeTest(ResourceId resId)
        {
            return resId == _resourceId;
        }

        private void onAddButtonClick()
        {
            AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
            UIViewSystem.Instance.Open<ShopPartPopup>();
            // EventDispatcher.Instance.DispatchEvent(new JumpToLobbyNavigationTypeEvent(UILobbyNavigationBarView.UIType.Shop));
            // IAPController.Instance.SetIAPBiParaPlacement(BiEventMatchFrenzy.Types.MonetizationIAPEventPlacement.PlacementLobbyClickCoin);
        }
    }
}