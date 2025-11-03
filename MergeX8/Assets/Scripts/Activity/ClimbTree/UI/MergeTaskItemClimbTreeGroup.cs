using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskItemClimbTreeGroup : MonoBehaviour
{
    public LocalizeTextMeshProUGUI NumText;

    public Image Icon;

    private LocalizeTextMeshProUGUI MultiText;

    // private LocalizeTextMeshProUGUI MultiText;
    public void Init()
    {
        NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        Icon = transform.Find("Icon").GetComponent<Image>();
        // MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
        gameObject.SetActive(false);
        MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
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
        gameObject.SetActive(ClimbTreeModel.Instance.IsPrivateOpened() && !MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId));
        if (gameObject.activeSelf)
        {
            NumText.SetText(ClimbTreeModel.Instance.GetTaskValue(StorageTask, false).ToString());
            // MultiText.SetText("X"+(couponMulti>1?couponMulti:mul));
            // MultiText.gameObject.SetActive(couponMulti > 1 || mul > 1);
            var couponMulti =
                ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ClimbTree);
            MultiText.SetText("X"+couponMulti);
            MultiText.gameObject.SetActive(couponMulti > 1);
        }
    }

    public void CollectReward(ref List<ResData> resDatas, bool autoAdd = true)
    {
        if (MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId))
            return;

        if (!ClimbTreeModel.Instance.IsPrivateOpened())
            return;
        var multiValue = ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.ClimbTree);
        var count = ClimbTreeModel.Instance.GetTaskValue(StorageTask, multiValue == 1);
        count = (int)(count*multiValue);
        if (autoAdd && count > 0)
        {
            UserData.Instance.AddRes(ClimbTreeModel._climbTreeBananaId, count,
                new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward,
                    data1 = StorageTask.Id.ToString(),
                    data2 = multiValue.ToString(),
                }, false);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyBubble,count.ToString());
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMonkeyBubble,
                itemAId = ClimbTreeModel._climbTreeBananaId,
                isChange = true,
                data1 = count.ToString(),
            });
            
            resDatas.Add(new ResData(ClimbTreeModel._climbTreeBananaId, count));
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