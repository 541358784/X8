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

public class MergeTaskItemFlowerFieldGroup : MonoBehaviour
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
        gameObject.SetActive(FlowerFieldModel.Instance.IsStart() && !MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId));
        if (gameObject.activeSelf)
        {
            NumText.SetText(FlowerFieldModel.Instance.GetTaskValue(StorageTask).ToString());
            // MultiText.SetText("X"+(couponMulti>1?couponMulti:mul));
            // MultiText.gameObject.SetActive(couponMulti > 1 || mul > 1);
        }
    }

    public void CollectReward( ref List<ResData> resDatas,bool autoAdd = true)
    {
        if (MainOrderManager.Instance.IsSpecialTask(StorageTask.OrgId))
            return;

        if (!FlowerFieldModel.Instance.IsStart())
            return;
        
        var multiValue = ExtraOrderRewardCouponModel.Instance.GetMultiValue(ExtraOrderRewardCouponType.FlowerField);
        var count = FlowerFieldModel.Instance.GetTaskValue(StorageTask);
        count = (int)(count*multiValue);
        
        if (autoAdd && count > 0)
        {
            var addValue = count;
            UserData.Instance.AddRes(FlowerFieldModel.FlowerFieldMergeItemId, addValue,
                new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.FlowerFieldGet}, false); 
            
            resDatas.Add(new ResData(FlowerFieldModel.FlowerFieldMergeItemId, addValue));
        }
        
        
        
        float delayTime = 0.3f;
        if (count >= 5)
            delayTime = 0.1f;
        FlyGameObjectManager.Instance.FlyCurrency(FlowerFieldModel.FlowerFieldMergeItemId, count, transform.position, 1, false, delayTime,
            action:
            () => { });

        for (int i = 0; i < count; i++)
        {
            if (!UserData.Instance.IsResource(FlowerFieldModel.FlowerFieldMergeItemId))
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonFlowerFieldGet,
                    itemAId = FlowerFieldModel.FlowerFieldMergeItemId,
                    isChange = true,
                    data1 = multiValue.ToString(),
                });
            }
        }
    }
}