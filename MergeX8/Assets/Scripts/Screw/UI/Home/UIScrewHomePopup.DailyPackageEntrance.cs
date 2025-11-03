using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public partial class UIScrewHomePopup
    {
        public void InitDailyPackageEntrance()
        {
            transform.Find("Root/LeftGroup/Viewport/Content/StarReward").gameObject
                .AddComponent<DailyPackageEntrance>().Init();
        }

        public class DailyPackageEntrance : MonoBehaviour
        {
            private Button Btn;
            private LocalizeTextMeshProUGUI TimeText;
            private bool IsAwake = false;

            private void Awake()
            {
                if (IsAwake)
                    return;
                IsAwake = true;
                TimeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
                InvokeRepeating("UpdateTimeText", 0, 1);
                Btn = transform.GetComponent<Button>();
                Btn.onClick.AddListener(() =>
                {
                    UIScrewShop.Open(UIScrewShop.ShopViewGroupType.DailyPackage);
                });
                EventDispatcher.Instance.AddEvent<EventScrewBuyDailyPackage>(OnBuyDailyPackage);
                EventDispatcher.Instance.AddEvent<EventScrewRefreshDailyPackage>(OnRefreshDailyPackage);
            }

            public void UpdateTimeText()
            {
                TimeText.SetText(DailyPackageModel.Instance.GetLeftTimeText());
            }
            public void OnRefreshDailyPackage(EventScrewRefreshDailyPackage evt)
            {
                UpdateVisible();
            }
            public void OnBuyDailyPackage(EventScrewBuyDailyPackage evt)
            {
                UpdateVisible();
            }

            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEvent<EventScrewBuyDailyPackage>(OnBuyDailyPackage);
                EventDispatcher.Instance.RemoveEvent<EventScrewRefreshDailyPackage>(OnRefreshDailyPackage);
            }

            public void Init()
            {
                Awake();
                UpdateVisible();
            }

            public void UpdateVisible()
            {
                var dailyPackageConfigs = DailyPackageModel.Instance.Configs;
                var visible = false;
                foreach (var config in dailyPackageConfigs)
                {
                    if (DailyPackageModel.Instance.CanBuy(config))
                    {
                        visible = true;
                        break;
                    }
                }
                gameObject.SetActive(visible);
            }
        }
    }
}