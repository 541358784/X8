using DragonPlus;
using DragonPlus.Config.LuckyGoldenEgg;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.LuckyGoldenEgg
{
    public class MergeLuckyGoldenEgg : MonoBehaviour
    {
        private LocalizeTextMeshProUGUI _countDownTime;
        private Button _button;
        private Transform _redPoint;
        private LocalizeTextMeshProUGUI _redPointText;
        private LocalizeTextMeshProUGUI _rewardText;
        private Image _slider;
        private LocalizeTextMeshProUGUI _sliderText;

        private void Awake()
        {
            _button = transform.GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
            _redPoint = transform.Find("RedPoint");
            _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
            _countDownTime = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardText = transform.Find("Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardText.gameObject.SetActive(false);
            _slider = transform.Find("Slider").GetComponent<Image>();
            _sliderText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("RefreshCountDown", 0, 1f);
        }

        public void OnClick()
        {
            LuckyGoldenEggModel.Instance.OpenMainPopup();
        }

        private void OnEnable()
        {
            RefreshView();
            StopAllCoroutines();
        }

        public void RefreshView()
        {
            if (!LuckyGoldenEggModel.Instance.IsStart())
                return;
            _redPoint.gameObject.SetActive(LuckyGoldenEggModel.Instance.GetGoldenEgg() > 0);
            _redPointText.SetText(LuckyGoldenEggModel.Instance.GetGoldenEgg().ToString());
            _slider.fillAmount = LuckyGoldenEggModel.Instance.GetCostProgress();
            _sliderText.SetText(LuckyGoldenEggModel.Instance.LuckyGoldenEgg.EnergyCost + "/" + LuckyGoldenEggModel.Instance.GetCastEnergy());
            _rewardText.SetText(LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig.HammerCount.ToString());
        }

        private void RefreshCountDown()
        {
            if (!LuckyGoldenEggModel.Instance.IsOpened())
            {
                gameObject.SetActive(false);
                return;
            }
        
            if (LuckyGoldenEggModel.Instance.IsPreheating())
            {
                gameObject.SetActive(false);
                return;
            }

            RefreshView();
            
            gameObject.SetActive(LuckyGoldenEggModel.Instance.IsOpen());
            
            if (LuckyGoldenEggModel.Instance.IsStart())
                _countDownTime.SetText(LuckyGoldenEggModel.Instance.GetActivityLeftTimeString());
        }
    }
}