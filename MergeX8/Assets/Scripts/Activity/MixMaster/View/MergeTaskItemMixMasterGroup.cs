using System;
using System.Collections.Generic;
using Activity.MixMaster.View;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskItemMixMasterGroup : MonoBehaviour
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
        MixMasterModel.Instance.CreateOrderReward(StorageTask);
        Refresh();
    }

    public void Refresh()
    {
        var reward = MixMasterModel.Instance.GetOrderReward(StorageTask);
        gameObject.SetActive(reward != null);
        if (gameObject.activeSelf)
        {
            Icon.sprite = UserData.GetResourceIcon(reward.id, UserData.ResourceSubType.Big);
            NumText.SetText(reward.count.ToString());
        }
    }

    public void CollectReward( ref List<ResData> resDatas)
    {
        var reward = MixMasterModel.Instance.GetOrderReward(StorageTask);
        if (reward == null)
            return;
        MixMasterModel.Instance.CollectOrderReward(StorageTask,ref resDatas);
        var count = reward.count;
        var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_MixMaster>();
        if (entrance)
        {
            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject icon = Icon.gameObject;
                FlyGameObjectManager.Instance.FlyObject(icon, Icon.transform.position,
                    entrance.transform.position, true, 1f, 0.1f * i, () =>
                    {
                        FlyGameObjectManager.Instance.PlayHintStarsEffect(entrance.transform.position);
                        ShakeManager.Instance.ShakeLight();
                        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MixMasterEntrance))
                        {
                            var Storage = MixMasterModel.Instance.Storage;
                            MixMasterFormulaConfig enableFormula = null;
                            var bagState = new Dictionary<int, int>();
                            foreach (var bag in Storage.Bag)
                            {
                                bagState.TryAdd(bag.Key, 0);
                                bagState[bag.Key] += bag.Value;
                            }

                            foreach (var desktop in Storage.Desktop)
                            {
                                bagState.TryAdd(desktop.Value.Id, 0);
                                bagState[desktop.Value.Id] += desktop.Value.Count;
                            }

                            foreach (var pair in Storage.History)
                            {
                                var formula = MixMasterModel.Instance.FormulaConfig.Find(a => a.Id == pair.Key);
                                if (MixMasterModel.Instance.CheckFormula(formula, bagState))
                                {
                                    enableFormula = formula;
                                    break;
                                }
                            }

                            if (enableFormula != null)
                            {
                                MergeMixMaster tipsItem = DynamicEntryManager.Instance.GetDynamicEntryMonoBehaviour<MergeMixMaster,DynamicEntry_Game_MixMaster>();
                                if (tipsItem)
                                {
                                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MixMasterEntrance,
                                        tipsItem.transform as RectTransform);
                                    if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MixMasterEntrance, null))
                                    {
                                        if (MergeTaskTipsController.Instance != null &&
                                            MergeTaskTipsController.Instance.contentRect != null)
                                        {
                                            MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(
                                                -tipsItem.transform.localPosition.x + 220, 0);
                                        }

                                        return;
                                    }
                                }
                            }
                        }
                    });
            }
        }
    }
}