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

public partial class EventEnum
{
    public const string DogPlayNewLevel = "DogPlayNewLevel";
}
public class EventDogPlayNewLevel : BaseEvent
{
    public EventDogPlayNewLevel() : base(EventEnum.DogPlayNewLevel) { }
}
public class MergeTaskItemDogPlayGroup : MonoBehaviour
{
    public LocalizeTextMeshProUGUI NumText;

    public Image Icon;

    // private LocalizeTextMeshProUGUI MultiText;
    public void Init()
    {
        NumText = transform.Find("Start/Text").GetComponent<LocalizeTextMeshProUGUI>();
        Icon = transform.Find("Start/Icon").GetComponent<Image>();
        // MultiText = transform.Find("Double").GetComponent<LocalizeTextMeshProUGUI>();
        gameObject.SetActive(false);
        EventDispatcher.Instance.AddEvent<EventDogPlayNewLevel>(OnDogPlayNewLevel);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventDogPlayNewLevel>(OnDogPlayNewLevel);
    }

    public void OnDogPlayNewLevel(EventDogPlayNewLevel evt)
    {
        if (!this)
        {
            EventDispatcher.Instance.RemoveEvent<EventDogPlayNewLevel>(OnDogPlayNewLevel);
            return;
        }
        Refresh();
    }

    public StorageTaskItem StorageTask;
    public MergeTaskTipsItem MergeTaskItem;
    private int PropCount;
    public void Init(StorageTaskItem storageTask, MergeTaskTipsItem mergeTaskItem)
    {
        StorageTask = storageTask;
        MergeTaskItem = mergeTaskItem;
        Refresh();
    }
    public void Refresh()
    {
        var propCount = DogPlayModel.Instance.GetPropCount(StorageTask);
        PropCount = propCount;
        if (propCount > 0)
        {
            gameObject.SetActive(!DogPlayModel.Instance.HideTaskItemGroup);
            NumText.SetText(propCount.ToString());
            NumText.gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void CollectReward()
    {
        if (!gameObject.activeSelf)
            return;
        var entrance = DogPlayModel.Instance.GetMergeEntrance();
        if (!entrance)
            return;
        var count = Math.Min(PropCount, 10);
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
                        entrance.OnCollectProp();
                    }
                });
        }
    }
}