using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskItemThemeDecorationGroup:MonoBehaviour
{
    public LocalizeTextMeshProUGUI NumText;
    public Image Icon;
    private LocalizeTextMeshProUGUI MultiText;
    public void Init()
    {
        NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        Icon = transform.Find("Icon").GetComponent<Image>();
        MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (ThemeDecorationModel.Instance.IsStart() && 
                ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ThemeDecoration) == 1 &&
                MultipleScoreModel.Instance.IsOpenActivity() &&
                MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.ThemeDecoration) > 1)
                UIPopupThemeDecorationMultipleScoreController.Open(ThemeDecorationModel.Instance.CurStorageThemeDecorationWeek);
        });
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
        gameObject.SetActive(ThemeDecorationModel.Instance.IsStart() && !MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId));
        if (gameObject.activeSelf)
        {
            NumText.SetText(ThemeDecorationModel.Instance.GetTaskValue(StorageTask, false).ToString());
            var mul = MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.ThemeDecoration);
            var couponMulti =
                ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ThemeDecoration);
            MultiText.SetText("X"+(couponMulti>1?couponMulti:mul));
            MultiText.gameObject.SetActive(couponMulti > 1 || mul > 1);
        }
    }
}