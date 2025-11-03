using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.BindEmail
{
    public class UIPopupBindEmail_GetReward : MonoBehaviour
    {
        private Button _close;
        private Transform _item;
        private Button _nextStage;
        private UIPopupBindEmailController _rootMono;

        private List<ResData> _resDatas = new List<ResData>();
        private void Awake()
        {
            _item = transform.Find("RewardGroup/Item");
            _item.gameObject.SetActive(false);

            _close = transform.Find("ButtonClose").GetComponent<Button>();
            _close.onClick.AddListener(() =>
            {
                GetReward();
            });

            _nextStage = transform.Find("ButtonNext").GetComponent<Button>();
            _nextStage.onClick.AddListener(() =>
            {
                GetReward();
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
                
                _resDatas.Add(new ResData(id, num));
            }
        }

        private void GetReward()
        {
            StorageManager.Instance.GetStorage<StorageHome>().BuildEmail.IsGetReward = true;
            CommonRewardManager.Instance.PopCommonReward(_resDatas, CurrencyGroupManager.Instance.currencyController, true, animEndCall: () =>
            {
                _rootMono.AnimCloseWindow();
            });
        }
    }
}