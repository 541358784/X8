using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskItemSlotMachineGroup:MonoBehaviour
{
    public LocalizeTextMeshProUGUI NumText;
    public Image Icon;
    // private LocalizeTextMeshProUGUI MultiText;
    public void Init()
    {
        NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        Icon = transform.Find("Icon").GetComponent<Image>();
        // MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
        // transform.GetComponent<Button>().onClick.AddListener(() =>
        // {
        //     if (SlotMachineModel.Instance.IsOpened() && 
        //         ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.SlotMachine) == 1 &&
        //         MultipleScoreModel.Instance.IsOpenActivity() &&
        //         MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.SlotMachine) > 1)
        //         UIPopupSlotMachineMultipleScoreController.Open(SlotMachineModel.Instance.CurStorageSlotMachineWeek);
        // });
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
        gameObject.SetActive(SlotMachineModel.Instance.IsOpened() && !MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId));
        if (gameObject.activeSelf)
        {
            var count = SlotMachineModel.Instance.GetTaskValue(StorageTask, false);
            NumText.SetText(count.ToString());
            // var mul = MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.SlotMachine);
            // var couponMulti =
            //     ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.SlotMachine);
            // MultiText.SetText("X"+mul);
            // MultiText.gameObject.SetActive(couponMulti == 1 && mul > 1);
            gameObject.SetActive(count > 0);
        }
    }
}