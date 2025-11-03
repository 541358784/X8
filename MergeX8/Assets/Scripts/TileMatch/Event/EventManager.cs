using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class TileMatchEventManager : Manager<TileMatchEventManager>
{
    public void AddEvent(string type, Action<BaseEvent> baseEvent)
    {
        EventDispatcher.Instance.AddEventListener(type,baseEvent);
    }
    public void RemoveEvent(string type, Action<BaseEvent> baseEvent)
    {
        EventDispatcher.Instance.RemoveEventListener(type,baseEvent);
    }
    public bool SendEvent(string type, params object[] datas)
    {
        return EventDispatcher.Instance.DispatchEvent(type, datas);
    }

    public bool SendEventImmediately(string type, params object[] datas)
    {
        return EventDispatcher.Instance.DispatchEventImmediately(type, datas);
    }
}