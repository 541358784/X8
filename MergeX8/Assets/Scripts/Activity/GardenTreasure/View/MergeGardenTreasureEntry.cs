using Activity.GardenTreasure.Model;
using DragonPlus;
using DragonPlus.Config.GardenTreasure;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.GardenTreasure.View
{
    public class MergeGardenTreasureEntry : MonoBehaviour
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
            if (GardenTreasureModel.Instance.IsPreheatEnd())
            {
                UIManager.Instance.OpenUI(UINameConst.UIGardenTreasureMain);
            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupGardenTreasureStart);
            }
        }

        private void OnEnable()
        {
            RefreshView();
            StopAllCoroutines();
        }
        
        public void RefreshView()
        {
            gameObject.SetActive(GardenTreasureModel.Instance.IsOpened());
            
            if (!GardenTreasureModel.Instance.IsOpened())
                return;

            int shovelNum = UserData.Instance.GetRes(UserData.ResourceId.GardenShovel);
            _redPoint.gameObject.SetActive(shovelNum > 0);
            _redPointText.SetText(shovelNum.ToString());
            _slider.fillAmount = GardenTreasureModel.Instance.GetCostProgress();
            _sliderText.SetText(GardenTreasureModel.Instance.GardenTreasure.EnergyCost + "/" + GardenTreasureConfigManager.Instance.GetSettingConfig()[0].Energy);
            _rewardText.SetText("1");
        }

        private void RefreshCountDown()
        {
            RefreshView();
            
            if (!GardenTreasureModel.Instance.IsPreheatEnd())
            {
                _countDownTime.SetText(GardenTreasureModel.Instance.GetPreheatEndTimeString());
            }
            else
            {
                _countDownTime.SetText(GardenTreasureModel.Instance.GetEndTimeString());
            }
        }
    }
}