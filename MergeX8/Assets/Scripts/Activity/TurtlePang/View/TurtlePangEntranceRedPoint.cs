using System;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;

public class TurtlePangEntranceRedPoint:MonoBehaviour
{
    private StorageTurtlePang Storage;
    private void Awake()
    {
        EventDispatcher.Instance.AddEvent<EventTurtlePangUpdateRedPoint>(UpdateRedPoint);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventTurtlePangUpdateRedPoint>(UpdateRedPoint);
    }
    public void UpdateRedPoint(EventTurtlePangUpdateRedPoint evt)
    {
        UpdateViewState();
    }

    public void Init(StorageTurtlePang storage)
    {
        Storage = storage;
        UpdateViewState();
    }
    public void UpdateViewState()
    {
        gameObject.SetActive(CanShow());
    }
    public bool CanShow()
    {
        return Storage.PackageCount >= TurtlePangModel.Instance.GlobalConfig.PackageType1[0];
    }
}