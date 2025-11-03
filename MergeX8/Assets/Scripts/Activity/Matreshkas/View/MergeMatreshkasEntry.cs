using Activity.Matreshkas.Model;
using DragonPlus;
using DragonPlus.Config.Matreshkas;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.Matreshkas.View
{
    public class MergeMatreshkasEntry : MonoBehaviour
    {
        private Image Slider;
        private LocalizeTextMeshProUGUI SliderText;
        private LocalizeTextMeshProUGUI _countDownTime;
        private Button _butCoinRush;
        private GameObject _redPoint;
        private Image _rewardImage;
        
        private void Awake()
        {
            Slider = transform.Find("Root/Slider").GetComponent<Image>();
            SliderText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _butCoinRush = transform.GetComponent<Button>();
            _butCoinRush.onClick.AddListener(OnClick);
            _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _redPoint = transform.Find("Root/RedPoint").gameObject;
            _redPoint.gameObject.SetActive(false);
            InvokeRepeating("RefreshCountDown", 0, 1f);
            _rewardImage = transform.Find("Root/Reward/Icon").GetComponent<Image>();


            if(MatreshkasConfigManager.Instance.MatreshkasSettingList != null)
                _rewardImage.sprite = UserData.GetResourceIcon(MatreshkasConfigManager.Instance.MatreshkasSettingList[0].RewardIds[0]);
        }
        
        public void OnClick()
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupMatreshkas);
        }
        
        private void RefreshView()
        {
            gameObject.SetActive(MatreshkasModel.Instance.IsJoin() && MatreshkasModel.Instance.IsOpened() && !MatreshkasModel.Instance.IsTimeEnd());

            MatreshkasModel.Instance.CheckJoinEnd();
            
            if(!gameObject.activeSelf)
                return;
            
            int stage = MatreshkasModel.Instance.Stage;
            int totalStage = MatreshkasModel.Instance.GetTotalStage();
            
            SliderText.SetText(stage+"/"+totalStage);
            Slider.fillAmount = stage / (float) totalStage;
        }
        
        private void RefreshCountDown()
        {
            RefreshView();
            
            _countDownTime.SetText(MatreshkasModel.Instance.GetJoinEndTimeString());
        }
    }
}