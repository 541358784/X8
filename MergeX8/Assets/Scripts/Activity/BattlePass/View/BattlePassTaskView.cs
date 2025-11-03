using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;


namespace Activity.BattlePass
{
    public class BattlePassTaskView
    {
        public enum TaskUIType
        {
            Daily,
            Challenge,
            Fixed,
        }


        public GameObject _gameObject;
        public GameObject _lockObject;
        public GameObject _normalObject;
        private GameObject _normalFinishObject;

        private LocalizeTextMeshProUGUI _lockText;
        private LocalizeTextMeshProUGUI _lockTime;

        private LocalizeTextMeshProUGUI _normalTime;
        private LocalizeTextMeshProUGUI _normalName;
        private LocalizeTextMeshProUGUI _normalNum;
        private LocalizeTextMeshProUGUI _sliderText;

        private Image _normalIcon;

        private Slider _slider;

        public TableBattlePassTask _config;
        public StorageBattlePassTaskInfo _info;

        private TaskUIType _uiType;

        public void Init(GameObject gameObject, TaskUIType uiType)
        {
            _gameObject = gameObject;
            _uiType = uiType;

            _lockObject = gameObject.transform.Find("Lock").gameObject;
            _normalObject = gameObject.transform.Find("Normal").gameObject;
            _normalFinishObject = gameObject.transform.Find("Normal/Finish").gameObject;

            _lockText = gameObject.transform.Find("Lock/Text").gameObject.GetComponent<LocalizeTextMeshProUGUI>();
            _lockTime = gameObject.transform.Find("Lock/TimeGroup/TimeText").gameObject.GetComponent<LocalizeTextMeshProUGUI>();

            _normalTime = gameObject.transform.Find("Normal/TimeGroup/TimeText").gameObject.GetComponent<LocalizeTextMeshProUGUI>();
            _normalName = gameObject.transform.Find("Normal/Text").gameObject.GetComponent<LocalizeTextMeshProUGUI>();
            _normalNum = gameObject.transform.Find("Normal/Num").gameObject.GetComponent<LocalizeTextMeshProUGUI>();
            _sliderText = gameObject.transform.Find("Normal/Slider/Text").gameObject.GetComponent<LocalizeTextMeshProUGUI>();

            _normalIcon = gameObject.transform.Find("Normal/Icon").gameObject.GetComponent<Image>();

            _slider = gameObject.transform.Find("Normal/Slider").gameObject.GetComponent<Slider>();
        }

        public void UpdateView(StorageBattlePassTaskInfo info)
        {
            _info = info;
            _config = BattlePassConfigManager.Instance.GetTaskConfig(info.Id);

            _sliderText.SetText(_info.TotalNum + "/" + _config.number);
            _slider.value = 1.0f * _info.TotalNum / _config.number;

            _normalNum.SetText(_config.reward.ToString());

            _lockObject.gameObject.SetActive(false);
            _normalObject.gameObject.SetActive(true);

            _normalFinishObject.gameObject.SetActive(_info.TotalNum >= _config.number);

            string content = null;
            if (_config.type == (int)TaskType.MergerItem)
            {
                TableMergeItem mergeItem = GameConfigManager.Instance.GetItemConfig(_config.mergeid);
                content = string.Format(LocalizationManager.Instance.GetLocalizedString(_config.contentKey), _config.number, LocalizationManager.Instance.GetLocalizedString(mergeItem.name_key));

                _normalIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
            }
            else
            {
                content = string.Format(LocalizationManager.Instance.GetLocalizedString(_config.contentKey), _config.number);
                _normalIcon.sprite = ResourcesManager.Instance.GetSpriteVariant("BattlePassAtlas", _config.image);
            }

            _normalName.SetText(content);
            UpdateActivityTime();
        }

        public void UpdateActivityTime()
        {
            _lockTime.SetText(BattlePassTaskModel.Instance.GetActiveTime());
            _normalTime.SetText(BattlePassTaskModel.Instance.GetActiveTime());
        }
    }
}