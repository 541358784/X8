using System;
using DragonU3DSDK.Storage;
using UnityEngine;

public class UIPopupSetHeadCellRedPoint:MonoBehaviour
{
    private void Awake()
    {

    }

    private int AvatarId;
    private bool IsInit = false;
    public void Init(int avatarId)
    {
        if (IsInit)
            return;
        IsInit = true;
        AvatarId = avatarId;
        RefreshShowState();
        EventDispatcher.Instance.AddEventListener(EventEnum.VIEW_NEW_HEAD,RefreshShowState);
        EventDispatcher.Instance.AddEventListener(EventEnum.GET_NEW_HEAD,RefreshShowState);
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
        if (StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UnViewedAvatarList.Contains(AvatarId))
            return true;
        return false;
    }
}