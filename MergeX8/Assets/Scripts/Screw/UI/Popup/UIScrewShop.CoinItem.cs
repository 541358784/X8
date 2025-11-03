using DragonPlus;
using DragonPlus.Config.Screw;
using DragonU3DSDK.Asset;
using Screw.Module;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public partial class UIScrewShop
    {
        public class StoreCoinItem : MonoBehaviour
        {
            private TableCoinShopConfig Config;
            private Button BuyBtn;
            private LocalizeTextMeshProUGUI PriceText;
            private LocalizeTextMeshProUGUI NumText;
            private Image Icon;

            public bool AwakeFlag;
            public void Awake()
            {
                if (AwakeFlag)
                    return;
                AwakeFlag = true;
                BuyBtn = transform.Find("ButtonStart").GetComponent<Button>();
                BuyBtn.onClick.AddListener(() =>
                {
                    global::StoreModel.Instance.Purchase(Config.Id);
                });
                PriceText = transform.Find("ButtonStart/Text").GetComponent<LocalizeTextMeshProUGUI>();
                NumText = transform.Find("NailIconGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
                Icon = transform.Find("NailIconGroup/Icon").GetComponent<Image>();
                EventDispatcher.Instance.AddEvent<EventScrewBuyShopCoin>(OnBuyShopCoin);
            }

            public void OnBuyShopCoin(EventScrewBuyShopCoin evt)
            {
                if (!this)
                    OnDestroy();
                if (evt.Config == Config)
                {
                    FlyModule.Instance.Fly(Config.RewardId[0], Config.RewardNum[0], Icon.transform.position);
                }
            }
            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEvent<EventScrewBuyShopCoin>(OnBuyShopCoin);
            }
            public void Init(TableCoinShopConfig config)
            {
                Awake();
                Config = config;
                UpdateView();
            }

            public void UpdateView()
            {
                PriceText.SetText(global::StoreModel.Instance.GetPrice(Config.Id));
                NumText.SetText(Config.RewardNum[0].ToString());
                Icon.sprite = ResourcesManager.Instance.GetSpriteVariant("ScrewCommonAtlas", Config.Image);
            }
        }
    }
}