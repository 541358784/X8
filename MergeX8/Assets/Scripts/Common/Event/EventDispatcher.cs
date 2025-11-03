using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using Scripts.UI;

public partial class EventDispatcher : Manager<EventDispatcher>, IEventDispatcher
{
    private ReaderWriterLockSlim eventLock = new ReaderWriterLockSlim();

    private Dictionary<string, Action<BaseEvent>> dicEvent = new Dictionary<string, Action<BaseEvent>>();

    private Queue<BaseEvent> queueEvent = new Queue<BaseEvent>();


    public void AddEventListener(string type, Action<BaseEvent> listener)
    {
        if (dicEvent.ContainsKey(type))
        {
            dicEvent[type] += listener;
        }
        else
        {
            dicEvent.Add(type, listener);
        }
    }

    public void RemoveEventListener(string type, Action<BaseEvent> listener)
    {
        if (dicEvent.ContainsKey(type))
        {
            dicEvent[type] -= listener;
            if (dicEvent[type] == null)
            {
                dicEvent.Remove(type);
            }
        }
    }

    public bool DispatchEvent(string type, params object[] datas)
    {
        if (!dicEvent.ContainsKey(type) || dicEvent[type] == null)
            return false;

        if (!eventLock.IsWriteLockHeld && !eventLock.TryEnterWriteLock(200))
            return false;

        try
        {
            BaseEvent evt = new BaseEvent(type, datas);
            queueEvent.Enqueue(evt);
        }
        finally
        {
            try
            {
                eventLock.ExitWriteLock();
            }
            catch (SynchronizationLockException e)
            {
                DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
            }
        }

        return true;
    }

    public bool DispatchEventImmediately(string type, params object[] datas)
    {
        if (dicEvent.ContainsKey(type) && dicEvent[type] != null)
        {
            BaseEvent evt = new BaseEvent(type, datas);
            Action<BaseEvent> fun = dicEvent[type];
            fun(evt);
            return true;
        }

        return false;
    }

    public bool DispatchEvent(BaseEvent evt)
    {
        if (evt == null)
            return false;

        return DispatchEvent(evt.type, evt.datas);
    }

    public void ClearEvent()
    {
        dicEvent.Clear();
    }

    void Update()
    {
        if (eventLock.IsWriteLockHeld || eventLock.TryEnterWriteLock(200))
        {
            try
            {
                while (queueEvent.Count > 0)
                {
                    BaseEvent qEvent = queueEvent.Dequeue();
                    if (dicEvent.ContainsKey(qEvent.type) && dicEvent[qEvent.type] != null)
                    {
                        Action<BaseEvent> fun = dicEvent[qEvent.type];
                        fun(qEvent);
                    }
                }
                while (queueEventI.Count > 0)
                {
                    IEvent qEvent = queueEventI.Dequeue();
                    var typeStr = qEvent.GetType().ToString();
                    if (dicEventI.ContainsKey(typeStr) && dicEventI[typeStr] != null)
                    {
                        Action<IEvent> fun = dicEventI[typeStr];
                        fun(qEvent);
                    }
                }
            }
            finally
            {
                try
                {
                    if(eventLock.IsWriteLockHeld)
                        eventLock.ExitWriteLock();
                }
                catch (SynchronizationLockException e)
                {
                    DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
                }
            }
        }
    }
}