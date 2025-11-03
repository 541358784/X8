using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{


    public class LifeNumber : ResBar
    {
        private static List<LifeNumber> viewList = new List<LifeNumber>();

        public static Transform GetTopView()
        {
            if (viewList.Count > 0) return viewList[viewList.Count - 1].transform.Find("Icon");
            return null;
        }

        private TextMeshProUGUI countText;
        private LocalizeTextMeshProUGUI timeText;
        private Transform Infinite;
        private Transform AddImage;

        private float _updateTime = 1.0f;

        private void Awake()
        {
            _itemType = ItemType.TMEnergy;
            viewList.Add(this);
            countText = transform.Find("CountText").GetComponent<TextMeshProUGUI>();
            timeText = transform.Find("TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            Infinite = transform.Find("Infinite");
            AddImage = transform.Find("BuyImage");
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

        protected override void OnEnable()
        {
            base.OnEnable();

            EventDispatcher.Instance.AddEventListener(EventEnum.ENERGY_CHANGED, RefreshEnergyNumChanged);
        }

        protected override void OnDisable()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.ENERGY_CHANGED, RefreshEnergyNumChanged);

            base.OnDisable();
        }

        private void Update()
        {
            _updateTime += Time.deltaTime;
            if (_updateTime < 1f)
            {
                return;
            }

            _updateTime = 0f;

            UpdateEnergyTime();
        }

        protected override bool TypeTest(ResourceId resId)
        {
            return false;//resId == ResourceId.Energy || resId == ResourceId.Energy_Infinity;
        }

        protected override int CurrentCount
        {
            get => ItemModel.Instance.GetNum((int) ResourceId.TMEnergy);
        }

        protected override TextMeshProUGUI CountText
        {
            get => countText;
        }

        public void SetTimer(int time)
        {
        }

        public void UpdateEnergyTime()
        {
            UpdateNumber(false);

            var leftTime = EnergyModel.Instance.EnergyUnlimitedLeftTime();
            if (leftTime > 0)
            {
                var leftTimeString = CommonUtils.GetUnlimiteLeftTimeString(leftTime);
                if (timeText.GetText() != leftTimeString)
                {
                    timeText.SetText(leftTimeString);
                }

                Infinite.gameObject.SetActive(true);
                AddImage.gameObject.SetActive(false);
                return;
            }

            var isFull = ItemModel.Instance.IsNumMax((int) ResourceId.TMEnergy);
            if (isFull)
            {
                Infinite.gameObject.SetActive(false);
                var fullText = LocalizationManager.Instance.GetLocalizedString("&key.UI_store_energy_full_text");
                if (timeText.GetText() != fullText)
                {
                    timeText.SetText(fullText);
                }

                AddImage.gameObject.SetActive(false);
                return;
            }

            var newText = DragonU3DSDK.Utils.GetTimeString("%mm:%ss",
                (int) (ItemAutoClaimModel.Instance.GetNextClaimCD((int) ResourceId.TMEnergy) * 0.001));
            if (timeText.GetText() != newText)
            {
                timeText.SetText(newText);
            }

            AddImage.gameObject.SetActive(true);
            Infinite.gameObject.SetActive(false);
        }

        private void RefreshEnergyNumChanged(BaseEvent eEvent)
        {
            countText.SetText(CurrentCount.ToString());
            UpdateEnergyTime();
        }

        private void onAddButtonClick()
        {
            if (AddImage.gameObject.activeSelf)
            {
                DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                    .GameEventTmEnergyPop,data1:"0");
                UIViewSystem.Instance.Open<UiOutLivesController>();
            }
        }

        protected override void CurrencyFlyAniEnd(BaseEvent evt)
        {
            CurrencyFlyAniEnd eventData = evt as CurrencyFlyAniEnd;
            if (eventData.itemType == ItemType.TMEnergy || eventData.itemType == ItemType.TMEnergyInfinity)
            {
                transform.GetComponent<Animator>()?.Play("shake", -1, 0);
                UpdateNumber(true);
            }
        }
    }
}