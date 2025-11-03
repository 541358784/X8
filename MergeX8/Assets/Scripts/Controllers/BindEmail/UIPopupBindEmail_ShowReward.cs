using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.BindEmail
{
    public class UIPopupBindEmail_ShowReward : MonoBehaviour
    {
        private Button _close;
        private Transform _item;
        private Button _nextStage;
        private UIPopupBindEmailController _rootMono;
        
        private void Awake()
        {
            _item = transform.Find("RewardGroup/Item");
            _item.gameObject.SetActive(false);

            _close = transform.Find("ButtonClose").GetComponent<Button>();
            _close.onClick.AddListener(() =>
            {
                _rootMono.AnimCloseWindow();
            });

            _nextStage = transform.Find("ButtonNext").GetComponent<Button>();
            _nextStage.onClick.AddListener(() =>
            {
                _rootMono.NextStage(1);
            });
        }

        public void Init(UIPopupBindEmailController mono)
        {
            _rootMono = mono;
        }
        
        private void Start()
        {
            string reward = GlobalConfigManager.Instance.GetGlobal_Config_Number_Value("build_email_reward");
            if (reward == null || reward == "")
                return;

            string[] arrayReward = reward.Split(';');
            if (arrayReward == null || arrayReward.Length == 0)
                return;

            for (int i = 0; i < arrayReward.Length; i++)
            {
                string[] rewardStr = arrayReward[i].Split(',');
                if (rewardStr == null || rewardStr.Length < 2)
                    continue;

                int id = int.Parse(rewardStr[0]);
                int num = int.Parse(rewardStr[1]);

                var cloneItem = Instantiate(_item, _item.parent);
                cloneItem.gameObject.SetActive(true);

                cloneItem.transform.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(id);
                cloneItem.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(num.ToString());
            }
        }
    }
}