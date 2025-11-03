using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskItemMonopolyGroup:MonoBehaviour
{
    public LocalizeTextMeshProUGUI NumText;
    public Image Icon;
    // private LocalizeTextMeshProUGUI MultiText;
    public void Init()
    {
        NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        Icon = transform.Find("Icon").GetComponent<Image>();
        // MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
    }

    private StorageTaskItem StorageTask;
    private MergeTaskTipsItem MergeTaskItem;
    public void Init(StorageTaskItem storageTask,MergeTaskTipsItem mergeTaskItem)
    {
        StorageTask = storageTask;
        MergeTaskItem = mergeTaskItem;
        Refresh();
    }
    public void Refresh()
    {
        gameObject.SetActive(MonopolyModel.Instance.IsStart() && !MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId));
        if (gameObject.activeSelf)
        {
            NumText.SetText(MonopolyModel.Instance.GetTaskValue(StorageTask, false).ToString());
            // MultiText.SetText("X"+(couponMulti>1?couponMulti:mul));
            // MultiText.gameObject.SetActive(couponMulti > 1 || mul > 1);
        }
    }
}