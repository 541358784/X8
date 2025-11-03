using System;
using System.Collections.Generic;
using System.Threading;

public partial class EventDispatcher
{
    private Dictionary<string, Action<BaseEvent>> EventCallbackTranslateDictionary = new Dictionary<string, Action<BaseEvent>>();

    public Action<BaseEvent> TranslateToBaseEventAction<T>(Action<T> specialEvent,bool createFlag)where T:BaseEvent,new()
    {
        var type = new T().type;
        var key = specialEvent.GetHashCode().ToString() + type;
        if (!EventCallbackTranslateDictionary.ContainsKey(key))
        {
            if (createFlag)
                EventCallbackTranslateDictionary.Add(key, (evt) => specialEvent(evt as T));
            else
                return null;
        }
        return EventCallbackTranslateDictionary[key];
    }
    public void AddEvent<T>(Action<T> specialEvent)where T:BaseEvent,new()
    {
        var type = new T().type;
        var baseEvent = TranslateToBaseEventAction(specialEvent,true);
        if (dicEvent.ContainsKey(type))
        {
            dicEvent[type] += baseEvent;
        }
        else
        {
            dicEvent.Add(type, baseEvent);
        }
    }
    public void RemoveEvent<T>(Action<T> specialEvent)where T:BaseEvent,new()
    {
        var type = new T().type;
        var baseEvent = TranslateToBaseEventAction(specialEvent,false);
        if (baseEvent == null)
            return;
        if (!dicEvent.ContainsKey(type))
            return;
    
        dicEvent[type] -= baseEvent;
        if (dicEvent[type] == null)
            dicEvent.Remove(type);
    }
    public bool SendEvent<T>(T evt) where T:BaseEvent
    {
        if (evt == null)
            return false;
        var type = evt.type;
        if (!dicEvent.ContainsKey(type) || dicEvent[type] == null)
            return false;

        if (!eventLock.IsWriteLockHeld && !eventLock.TryEnterWriteLock(200))
            return false;
        try
        {
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
    
    public bool SendEventImmediately<T>(T evt)where T:BaseEvent
    {
        if (evt == null)
            return false;
        var type = evt.type;
        if (!dicEvent.ContainsKey(type) || dicEvent[type] == null)
            return false;
        Action<BaseEvent> fun = dicEvent[type];
        fun(evt);
        return true;
    }

}