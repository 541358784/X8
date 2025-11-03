using System;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.SaveTheWhales
{
    public class MergeSaveTheWhales : MonoBehaviour
    {
        private Image _slider;
        private Image _rewardIcon;
        private LocalizeTextMeshProUGUI _timeText;
        private LocalizeTextMeshProUGUI _sliderText;
        private Button _button;
        private bool _isAnim = false;
        private string sprName;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnBtn);
            _slider = transform.Find("Root/Slider").GetComponent<Image>();
            _sliderText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardIcon = transform.Find("Root/Slider/RewardIcon").GetComponent<Image>();
            _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("RefreshCountDown", 0, 1f);
        }

        private void OnBtn()
        {
            UIManager.Instance.OpenUI(UINameConst.UISaveTheWhalesReward);            
        }
        private void RefreshCountDown()
        {
            RefreshView();
            
            if (SaveTheWhalesModel.Instance.IsJoin())
                _timeText.SetText(SaveTheWhalesModel.Instance.GetLeftTimeStr());
        }
        public void RefreshView()
        {
            gameObject.SetActive(SaveTheWhalesModel.Instance.IsJoin());
            if (!SaveTheWhalesModel.Instance.IsJoin())
                return;
            RefreshReward();
            if (!_isAnim)
            {
                _sliderText.SetText(UserData.Instance.GetRes(UserData.ResourceId.Water) + "/" +
                                    SaveTheWhalesModel.Instance.SaveTheWhalesActivityConfig.CollectCount);
                _slider.DOFillAmount((float)UserData.Instance.GetRes(UserData.ResourceId.Water)/SaveTheWhalesModel.Instance.SaveTheWhalesActivityConfig.CollectCount, 1);
            }
        }

        public void RefreshReward()
        {
            int item = SaveTheWhalesModel.Instance.SaveTheWhalesActivityConfig.RewardId[0];
            if (UserData.Instance.IsResource(item))
            {
                if (sprName != item.ToString())
                {
                    sprName = item.ToString();
                    _rewardIcon.sprite = UserData.GetResourceIcon(item);
                }
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(item);
                if (itemConfig != null && sprName!=itemConfig.image)
                {
                    sprName = itemConfig.image;
                    _rewardIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                }
            }
            
        }
        public void SetText(int oldValue)
        {
            _isAnim = true;
            var newValue = UserData.Instance.GetRes(UserData.ResourceId.Water);
            _slider.DOFillAmount((float)newValue/SaveTheWhalesModel.Instance.SaveTheWhalesActivityConfig.CollectCount, 1);
            DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
            {
                _isAnim = false;
                _sliderText.SetText(oldValue+"/"+SaveTheWhalesModel.Instance.SaveTheWhalesActivityConfig.CollectCount);
            }).SetDelay(1f);

        }
    }
}