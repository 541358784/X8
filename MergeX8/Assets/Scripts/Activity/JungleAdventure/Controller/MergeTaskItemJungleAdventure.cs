using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JungleAdventure.Controller
{
    public class MergeTaskItemJungleAdventure : MonoBehaviour
    {
        public LocalizeTextMeshProUGUI NumText;

        public Image Icon;

        private LocalizeTextMeshProUGUI MultiText;

        public void Init()
        {
            NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            Icon = transform.Find("Icon").GetComponent<Image>();
            gameObject.SetActive(false);
            MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
            MultiText.SetText("");
        }

        public StorageTaskItem StorageTask;
        public MergeTaskTipsItem MergeTaskItem;

        public void Init(StorageTaskItem storageTask, MergeTaskTipsItem mergeTaskItem)
        {
            StorageTask = storageTask;
            MergeTaskItem = mergeTaskItem;
            Refresh();
        }

        public void Refresh()
        {
            gameObject.SetActive(JungleAdventureModel.Instance.IsPreheatEnd() && !MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId));
            if (gameObject.activeSelf)
            {
                NumText.SetText(JungleAdventureModel.Instance.GetTaskValue(StorageTask, false).ToString());
            }
        }

        public void CollectReward(ref List<ResData> resDatas, bool autoAdd = true)
        {
            if (MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId))
                return;

            if (!JungleAdventureModel.Instance.IsOpened() || !JungleAdventureModel.Instance.IsPreheatEnd())
                return;

            int multiValue = 1;
            var count = JungleAdventureModel.Instance.GetTaskValue(StorageTask, multiValue == 1);
            count = (int)(count * multiValue);
            if (autoAdd && count > 0)
            {
                JungleAdventureModel.Instance.AddScore(count);
                resDatas.Add(new ResData(UserData.ResourceId.JungleAdventure, count));
                resDatas.Last().isBuilding = true;
            }
            var entrance = MergeTaskTipsController.Instance._mergeRewardItem;
            if (!entrance)
                return;
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = Icon.gameObject;
                FlyGameObjectManager.Instance.FlyObject(icon, icon.transform.position,
                    entrance.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(entrance.transform.position);
                        ShakeManager.Instance.ShakeLight();
                        if (index == 0)
                        {
                        }
                    });
            }
        }
    }
}