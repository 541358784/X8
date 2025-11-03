using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupMixMasterListController
{
    public class MixTaskView : MonoBehaviour
    {
        private UIPopupMixMasterListController Controller;
        private StorageMixMaster Storage;
        private List<MixTaskItem> ItemList = new List<MixTaskItem>();
        private Transform DefaultItem;
        public void Init(UIPopupMixMasterListController controller, StorageMixMaster storage)
        {
            Controller = controller;
            Storage = storage;
            DefaultItem = transform.Find("Viewport/Content/Task");
            DefaultItem.gameObject.SetActive(false);
            var taskList = MixMasterModel.Instance.MixTaskConfig;
            for (var i = 0; i < taskList.Count; i++)
            {
                var task = taskList[i];
                var taskItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject.AddComponent<MixTaskItem>();
                taskItem.gameObject.SetActive(true);
                taskItem.Init(this,Storage,task);
                ItemList.Add(taskItem);
            }

            UpdateSiblingIndex();
        }

        public void UpdateSiblingIndex()
        {
            var canCollectList = new List<MixTaskItem>();
            var alreadyCollectList = new List<MixTaskItem>();
            var unFinishList = new List<MixTaskItem>();
            for (var i = 0; i < ItemList.Count; i++)
            {
                var item = ItemList[i];
                if (item.IsFinish)
                    alreadyCollectList.Insert(0,item);
                else if(item.CanCollect)
                    canCollectList.Insert(0,item);
                else
                    unFinishList.Insert(0,item);
            }
            foreach (var item in alreadyCollectList)
            {
                item.transform.SetAsFirstSibling();
            }
            foreach (var item in unFinishList)
            {
                item.transform.SetAsFirstSibling();
            }
            foreach (var item in canCollectList)
            {
                item.transform.SetAsFirstSibling();
            }
        }
    }

    public class MixTaskItem : MonoBehaviour
    {
        private MixMasterMixTaskConfig TaskConfig;
        private MixTaskView Controller;
        private StorageMixMaster Storage;
        private MixTaskItemView Normal;
        private MixTaskItemView Finish;
        public bool IsFinish=>Storage.AlreadyCollectLevels.Contains(TaskConfig.Id);
        public bool CanCollect=>Storage.CanCollectLevels.Contains(TaskConfig.Id);
        public void Init(MixTaskView controller, StorageMixMaster storage,MixMasterMixTaskConfig taskConfig)
        {
            Controller = controller;
            Storage = storage;
            TaskConfig = taskConfig;
            Normal = transform.Find("Normal").gameObject.AddComponent<MixTaskItemView>();
            Normal.Init(this,Storage,TaskConfig,false);
            Finish = transform.Find("Complete").gameObject.AddComponent<MixTaskItemView>();
            Finish.Init(this,Storage,TaskConfig,true);
            Normal.gameObject.SetActive(!IsFinish);
            Finish.gameObject.SetActive(IsFinish);
        }

        public void OnCollect()
        {
            Normal.gameObject.SetActive(!IsFinish);
            Finish.gameObject.SetActive(IsFinish);
            Controller.UpdateSiblingIndex();
        }
    }

    public class MixTaskItemView : MonoBehaviour
    {
        private MixMasterMixTaskConfig TaskConfig;
        private MixTaskItem Controller;
        private StorageMixMaster Storage;
        private bool FinishView;
        private Slider Slider;
        LocalizeTextMeshProUGUI SldierText;
        private Image RewardIcon;
        private LocalizeTextMeshProUGUI RewardNumText;
        private LocalizeTextMeshProUGUI Text;
        private Button CollectBtn;
        public void Init(MixTaskItem controller, StorageMixMaster storage,MixMasterMixTaskConfig taskConfig,bool finishView)
        {
            Controller = controller;
            Storage = storage;
            TaskConfig = taskConfig;
            FinishView = finishView;
            if (!FinishView)
            {
                Slider = transform.Find("Slider").GetComponent<Slider>();
                SldierText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
                Slider.minValue = 0;
                Slider.maxValue = TaskConfig.CollectCount;
                Slider.value = Storage.FirstMixCount;
                SldierText.SetText(Slider.value+"/"+Slider.maxValue);
                CollectBtn = transform.Find("ReceiveButton").GetComponent<Button>();
                CollectBtn.interactable = Controller.CanCollect;
                CollectBtn.onClick.AddListener(() =>
                {
                    MixMasterModel.Instance.CollectTask(TaskConfig);
                    Controller.OnCollect();
                });
                RewardIcon = transform.Find("RewardIcon").GetComponent<Image>();
                RewardNumText = transform.Find("RewardIcon/Num").GetComponent<LocalizeTextMeshProUGUI>();
                if (taskConfig.FormulaId > 0 && !Storage.History.ContainsKey(taskConfig.FormulaId))
                {
                    var formulaConfig = MixMasterModel.Instance.FormulaConfig.Find(a => a.Id == taskConfig.FormulaId);
                    RewardIcon.sprite = formulaConfig.GetFormulaIcon();
                    RewardNumText.SetText("");
                }
                else
                {
                    var rewards = CommonUtils.FormatReward(taskConfig.RewardId, taskConfig.RewardNum);
                    RewardIcon.sprite = UserData.GetResourceIcon(rewards[0].id, UserData.ResourceSubType.Big);
                    RewardNumText.SetText(rewards[0].count.ToString());
                }
            }
            Text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            Text.SetTerm(TaskConfig.LabelText);
            Text.SetTermFormats(TaskConfig.CollectCount.ToString());
        }
    }
}