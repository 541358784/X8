using System;
using DragonU3DSDK.Storage;
using UnityEngine;

public class UIPopupSetRedPoint:MonoBehaviour
{
    private bool isAwake = false;
    private void Awake()
    {
        if (isAwake)
            return;
        isAwake = true;
        EventDispatcher.Instance.AddEventListener(EventEnum.VIEW_NEW_HEAD,RefreshShowState);
        EventDispatcher.Instance.AddEventListener(EventEnum.GET_NEW_HEAD,RefreshShowState);
    }
    public void Init()
    {
        RefreshShowState();
        Awake();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.VIEW_NEW_HEAD,RefreshShowState);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GET_NEW_HEAD,RefreshShowState);
    }

    public void RefreshShowState(BaseEvent evt = null)
    {
        gameObject.SetActive(ShowState());
    }
    public bool ShowState()
    {
        if (StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UnViewedAvatarList.Count > 0)
            return true;
        return false;
    }
}