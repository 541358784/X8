using System;
using DragonU3DSDK.Storage;
using UnityEngine;

public class CatchFishEntranceRedPoint:MonoBehaviour
{
    private StorageCatchFish Storage => CatchFishModel.Instance.Storage;
    public void Init()
    {
        
        UpdateViewState();
        EventDispatcher.Instance.AddEvent<EventCatchFishItemChange>(OnEventItemChange);
        EventDispatcher.Instance.AddEvent<EventCatchFishCollectStateChange>(OnEventCollectStateChange);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventCatchFishItemChange>(OnEventItemChange);
        EventDispatcher.Instance.RemoveEvent<EventCatchFishCollectStateChange>(OnEventCollectStateChange);
    }

    public void OnEventItemChange(EventCatchFishItemChange e)
    {
        UpdateViewState();
    }
    public void OnEventCollectStateChange(EventCatchFishCollectStateChange e)
    {
        UpdateViewState();
    }
    public void UpdateViewState()
    {
        if (!CatchFishModel.Instance.IsPrivateOpened())
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.gameObject.SetActive(true);
    }
}