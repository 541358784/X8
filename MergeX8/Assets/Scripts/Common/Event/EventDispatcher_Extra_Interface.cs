using System;
using System.Collections.Generic;
using System.Threading;
using Scripts.UI;

public partial class EventDispatcher
{
    private Dictionary<string, Action<IEvent>> dicEventI = new Dictionary<string, Action<IEvent>>();

    private Queue<IEvent> queueEventI = new Queue<IEvent>();
    private Dictionary<string, Action<IEvent>> EventICallbackTranslateDictionary = new Dictionary<string, Action<IEvent>>();
    public Action<IEvent> TranslateToBaseEventActionI<T>(Action<T> specialEvent,bool createFlag)where T:IEvent,new()
    {
        var type = new T().GetType().ToString();
        var key = specialEvent.GetHashCode().ToString() + type;
        if (!EventICallbackTranslateDictionary.ContainsKey(key))
        {
            if (createFlag)
                EventICallbackTranslateDictionary.Add(key, (evt) => specialEvent((T)evt));
            else
                return null;
        }
        return EventICallbackTranslateDictionary[key];
    }

    public static void Register<T>(Action<T> specialEvent) where T : IEvent, new()
    {
        Instance.RegisterP(specialEvent);
    }
    public void RegisterP<T>(Action<T> specialEvent)where T:IEvent,new()
    {
        var type = new T().GetType().ToString();
        var baseEvent = TranslateToBaseEventActionI(specialEvent,true);
        if (dicEventI.ContainsKey(type))
        {
            dicEventI[type] += baseEvent;
        }
        else
        {
            dicEventI.Add(type, baseEvent);
        }
    }

    public static void UnRegister<T>(Action<T> specialEvent) where T : IEvent, new()
    {
        Instance.UnRegisterP(specialEvent);
    }
    public void UnRegisterP<T>(Action<T> specialEvent)where T:IEvent,new()
    {
        var type = new T().GetType().ToString();
        var baseEvent = TranslateToBaseEventActionI(specialEvent,false);
        if (baseEvent == null)
            return;
        if (!dicEventI.ContainsKey(type))
            return;
    
        dicEventI[type] -= baseEvent;
        if (dicEventI.ContainsKey(type) && dicEventI[type] == null)
            dicEventI.Remove(type);
    }

    public static bool Send<T>(T evt) where T : IEvent
    {
        return Instance.SendP(evt);
    }
    public bool SendP<T>(T evt) where T:IEvent
    {
        if (evt == null)
            return false;
        var type = evt.GetType().ToString();
        if (!dicEventI.ContainsKey(type) || dicEventI[type] == null)
            return false;

        if (!eventLock.IsWriteLockHeld && !eventLock.TryEnterWriteLock(200))
            return false;
        try
        {
            queueEventI.Enqueue(evt);
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
    
    public bool SendImmediately<T>(T evt)where T:IEvent
    {
        if (evt == null)
            return false;
        var type = evt.GetType().ToString();;
        if (!dicEventI.ContainsKey(type) || dicEventI[type] == null)
            return false;
        Action<IEvent> fun = dicEventI[type];
        fun(evt);
        return true;
    }

}