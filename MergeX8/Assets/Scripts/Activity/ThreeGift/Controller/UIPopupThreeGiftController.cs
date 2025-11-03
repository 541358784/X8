
using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine.UI;

namespace ThreeGift
{
    public class UIPopupThreeGiftController : UIWindowController
    {
        private LocalizeTextMeshProUGUI _timeText;
        private Button _button;
        private Text _text;
        private LocalizeTextMeshProUGUI _text1;
        private List<ThreeGiftItem> _items;
        private Button _buttonClose;
        private LocalizeTextMeshProUGUI _vipText;

        public override void PrivateAwake()
        {
            _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
            _items = new List<ThreeGiftItem>();
            for (int i = 1; i < 4; i++)
            {
                var tran = transform.Find("Root/Gift" + i);
                var item = tran.gameObject.AddComponent<ThreeGiftItem>();
                _items.Add(item);
            }

            InvokeRepeating("UpdateTime", 0, 1);
            _button = GetItem<Button>("Root/Button");
            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _buttonClose.onClick.AddListener(() => { AnimCloseWindow(); });
            _button.onClick.AddListener(OnClickBtn);
            _text = GetItem<Text>("Root/Button/Text");
            _text1 = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text1");
            _vipText = gameObject.transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEventListener(EventEnum.THREE_GIFT_PURCHASE_SUCCESS, OnPurchase);
        }

        private void OnPurchase(BaseEvent obj)
        {
            AnimCloseWindow();
        }

        private void OnClickBtn()
        {
            StoreModel.Instance.Purchase(ThreeGiftModel.Instance.ThreeGiftLevelConfigList.shopId);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventOneorallDealsPurchase, ThreeGiftModel.Instance.ThreeGiftLevelConfigList.shopId.ToString());

        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.THREE_GIFT_PURCHASE_SUCCESS, OnPurchase);

        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            var config = ThreeGiftModel.Instance.ThreeGiftLevelConfigList;
            for (int i = 0; i < config.packageList.Length; i++)
            {
                var giftConfig = ThreeGiftModel.Instance.GetThreeGiftConfigById(config.packageList[i]);
                _items[i].Init(giftConfig);
            }

            _text.text = StoreModel.Instance.GetPrice(config.shopId);
            _text1.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_3in1_deal_desc"), config.discount));
            _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(config.shopId));
        }

        public void UpdateTime()
        {
            _timeText.SetText(ThreeGiftModel.Instance.GetActivityLeftTimeString());
        }
    }
}
