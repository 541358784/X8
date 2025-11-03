using System;
using System.Collections.Generic;
using Activity.PillowWheel.UI;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskItemPillowWheelGroup : MonoBehaviour
{
    public LocalizeTextMeshProUGUI NumText;

    public Image Icon;

    // private LocalizeTextMeshProUGUI MultiText;
    public void Init()
    {
        NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        Icon = transform.Find("Icon").GetComponent<Image>();
        // MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
        gameObject.SetActive(false);
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
        gameObject.SetActive(PillowWheelModel.Instance.IsStart() && !MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId));
        if (gameObject.activeSelf)
        {
            NumText.SetText(PillowWheelModel.Instance.GetTaskValue(StorageTask).ToString());
            // MultiText.SetText("X"+(couponMulti>1?couponMulti:mul));
            // MultiText.gameObject.SetActive(couponMulti > 1 || mul > 1);
        }
    }

    public void CollectReward( ref List<ResData> resDatas, bool autoAdd = true)
    {
        if (MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId))
            return;

        if (!PillowWheelModel.Instance.IsStart())
            return;
        
        var num = UserData.Instance.GetRes(UserData.ResourceId.PillowWheel);
        var multiValue = 1f;
        var count = PillowWheelModel.Instance.GetTaskValue(StorageTask);
        count = (int)(count*multiValue);
        if (autoAdd && count > 0)
        {
            UserData.Instance.AddRes((int) UserData.ResourceId.PillowWheel, count,
                new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward,
                    data1 = StorageTask.Id.ToString(),
                    data2 = multiValue.ToString(),
                }, false);
            
            resDatas.Add(new ResData(UserData.ResourceId.PillowWheel, count));
        }
        var storage = PillowWheelModel.Instance.Storage;
        var entrance = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergePillowWheel, DynamicEntry_Game_PillowWheel>();
        if (!entrance)
            return;
        entrance.LockNum();
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
                        entrance.UnlockNum();   
                    }
                });
        }
    }
}