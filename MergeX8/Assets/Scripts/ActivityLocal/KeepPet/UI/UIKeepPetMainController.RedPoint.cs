using System;
using System.Linq;
using DragonPlus;
using Gameplay;
using UnityEngine;

public partial class UIKeepPetMainController
{
    public class SearchTaskBtnRedPoint : MonoBehaviour
    {
        private LocalizeTextMeshProUGUI LabelText;
        public void Init()
        {
            LabelText = transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
            gameObject.SetActive(true);
            gameObject.SetActive(false);
            UpdateViewState();
            EventDispatcher.Instance.AddEvent<EventKeepPetDogHeadChange>(OnDogHeadChange);
            EventDispatcher.Instance.AddEvent<EventKeepPetSearchPropCountChange>(OnDogSteakChange);
        }
        public void OnDogHeadChange(EventKeepPetDogHeadChange evt)
        {
            UpdateViewState();
        }
        public void OnDogSteakChange(EventKeepPetSearchPropCountChange evt)
        {
            UpdateViewState();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventKeepPetDogHeadChange>(OnDogHeadChange);
            EventDispatcher.Instance.RemoveEvent<EventKeepPetSearchPropCountChange>(OnDogSteakChange);
        }

        public void UpdateViewState()
        {
            var taskConfigList = KeepPetModel.Instance.SearchTaskConfig;
            var count = 0;
            foreach (var config in taskConfigList)
            {
                var consumeResData = new ResData(config.ConsumeType, config.ConsumeCount);
                if (UserData.Instance.CanAford(consumeResData))
                {
                    count++;
                }
            }
            gameObject.SetActive(count > 0);
        }
    }
    
    
    public class ExpBarRedPoint : MonoBehaviour
    {
        private LocalizeTextMeshProUGUI LabelText;
        public void Init()
        {
            LabelText = transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
            gameObject.SetActive(true);
            gameObject.SetActive(false);
            UpdateViewState();
            EventDispatcher.Instance.AddEvent<EventKeepPetCollectLevelReward>(OnCollectLevelReward);
            // EventDispatcher.Instance.AddEvent<EventKeepPetExpChange>(OnExpChange);
        }
        public void OnCollectLevelReward(EventKeepPetCollectLevelReward evt)
        {
            UpdateViewState();
        }
        // public void OnExpChange(EventKeepPetExpChange evt)
        // {
        //     UpdateViewState();
        // }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventKeepPetCollectLevelReward>(OnCollectLevelReward);
            // EventDispatcher.Instance.RemoveEvent<EventKeepPetExpChange>(OnExpChange);
        }
        
        public void UpdateViewState()
        {
            var storage = KeepPetModel.Instance.Storage;
            var curLevel = storage.Exp.KeepPetGetCurLevelConfig();
            var count = 0;
            for (var i = curLevel.Id; i >= 1; i--)
            {
                if (!storage.LevelRewardCollectState.ContainsKey(i))
                    count++;
            }
            gameObject.SetActive(count > 0);
        }
    }
}