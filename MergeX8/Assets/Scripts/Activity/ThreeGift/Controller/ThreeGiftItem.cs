
    using System;
    using DragonPlus;
    using DragonPlus.Config.ThreeGift;
    using DragonU3DSDK.Network.API.Protocol;
    using Framework;
    using Gameplay;
    using Gameplay.UI.Store.Vip.Model;
    using UnityEngine;
    using UnityEngine.UI;

    namespace ThreeGift
    {
        public class ThreeGiftItem : MonoBehaviour
        {
            private Button _button;
            private Text _priceText;
            private Transform _rewardItem;
            private ThreeGiftConfig _config;
            private LocalizeTextMeshProUGUI _vipText;
            private void Awake()
            {
                _button = transform.Find("Button").GetComponent<Button>();
                _button.onClick.AddListener(OnClickBuy);
                _priceText = transform.Find("Button/Text").GetComponent<Text>();
                _rewardItem = transform.Find("ItemGroup/Item");
                _rewardItem.gameObject.SetActive(false);
                _vipText = gameObject.transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();

            }



            public void Init(ThreeGiftConfig config)
            {
                _config = config;
                _priceText.text = StoreModel.Instance.GetPrice(config.shopId);
                for (int i = 0; i < config.contain.Length; i++)
                {
                    var obj = Instantiate(_rewardItem, _rewardItem.parent);
                    obj.gameObject.SetActive(true);
                    InitItem(obj, config.contain[i], config.containCount[i]);

                }

                _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(config.shopId));
            }

            private void OnTips()
            {

            }

            private void InitItem(Transform item, int itemID, int ItemCount)
            {
                item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID, UserData.ResourceSubType.Big);
                string tex = ItemCount.ToString();
                if (itemID == (int)UserData.ResourceId.Infinity_Energy)
                {
                    tex = TimeUtils.GetTimeString(ItemCount, true);
                }
                item.Find("Text").gameObject.SetActive(ItemCount > 1);
                item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(tex);
                item.gameObject.SetActive(true);

                var tipsBtn = item.Find("TipsBtn");

                if (tipsBtn != null && itemID == 923)
                {
                    tipsBtn.gameObject.SetActive(true);
                    var _tips = item.Find("ReceiveTips");
                    tipsBtn.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        _tips.gameObject.SetActive(true);
                        CommonUtils.DelayedCall(3, () =>
                        {
                            if (_tips != null)
                                _tips.gameObject.SetActive(false);
                        });
                    });
                }

            }

            private void OnClickBuy()
            {
                StoreModel.Instance.Purchase(_config.shopId);
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventOneorallDealsPurchase, _config.shopId.ToString());

            }

        }
    }
