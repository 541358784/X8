using System;
using DragonU3DSDK.Storage;
using UnityEngine;

public class PillowWheelEntranceRedPoint:MonoBehaviour
{
    private StoragePillowWheel Storage => PillowWheelModel.Instance.Storage;
    public void Init()
    {
        
        UpdateViewState();
        EventDispatcher.Instance.AddEvent<EventPillowWheelItemChange>(OnEventItemChange);
        EventDispatcher.Instance.AddEvent<EventPillowWheelCollectStateChange>(OnEventCollectStateChange);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventPillowWheelItemChange>(OnEventItemChange);
        EventDispatcher.Instance.RemoveEvent<EventPillowWheelCollectStateChange>(OnEventCollectStateChange);
    }

    public void OnEventItemChange(EventPillowWheelItemChange e)
    {
        UpdateViewState();
    }
    public void OnEventCollectStateChange(EventPillowWheelCollectStateChange e)
    {
        UpdateViewState();
    }
    public void UpdateViewState()
    {
        if (!PillowWheelModel.Instance.IsPrivateOpened())
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.gameObject.SetActive(Storage.CollectState.Count < PillowWheelModel.Instance.ResultConfigList.Count && Storage.ItemCount > 0);
    }
}