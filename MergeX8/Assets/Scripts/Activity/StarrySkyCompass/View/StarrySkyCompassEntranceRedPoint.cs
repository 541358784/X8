using System;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;

public class StarrySkyCompassEntranceRedPoint:MonoBehaviour
{
    private StorageStarrySkyCompass Storage;
    private void Awake()
    {
        EventDispatcher.Instance.AddEvent<EventStarrySkyCompassUpdateRedPoint>(UpdateRedPoint);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventStarrySkyCompassUpdateRedPoint>(UpdateRedPoint);
    }
    public void UpdateRedPoint(EventStarrySkyCompassUpdateRedPoint evt)
    {
        UpdateViewState();
    }

    private bool CheckRocketCount;
    private bool CheckShopCanBuy;
    public void Init(StorageStarrySkyCompass storage,bool checkRocketCount,bool checkShopCanBuy)
    {
        Storage = storage;
        UpdateViewState();
        CheckRocketCount = checkRocketCount;
        CheckShopCanBuy = checkShopCanBuy;
    }
    public void UpdateViewState()
    {
        gameObject.SetActive(CanShow());
    }
    public bool CanShow()
    {
        if (CheckRocketCount)
        {
            if (Storage.RocketCount > 0)
                return true;   
        }

        if (CheckShopCanBuy)
        {
            foreach (var config in StarrySkyCompassModel.Instance.ShopConfig)
            {
                if (Storage.Score >= config.Price)
                    return true;
            }   
        }
        return false;
    }
}