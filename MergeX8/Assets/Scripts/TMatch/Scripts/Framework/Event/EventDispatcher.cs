using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace TMatch
{


    public class EventDispatcher : Manager<EventDispatcher>, IEventDispatcher
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

        public bool DispatchEvent(BaseEvent evt)
        {
            if (dicEvent.ContainsKey(evt.type) && dicEvent[evt.type] != null)
            {
                if (eventLock.IsWriteLockHeld || eventLock.TryEnterWriteLock(200))
                {
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

                return false;
            }

            return false;
        }

        public bool DispatchEvent(string type, params object[] datas)
        {
            return DispatchEvent(new BaseEvent(type, datas));
        }

        /// <summary>
        /// 立即派发事件(不走update)
        /// </summary>
        public void DispatchEventImmediately(BaseEvent evt)
        {
            if (dicEvent.ContainsKey(evt.type) && dicEvent[evt.type] != null)
            {
                // Action<BaseEvent> fun = dicEvent[evt.type];
                // fun?.Invoke(evt);

                var delegates = dicEvent[evt.type].GetInvocationList();
                foreach (var listener in delegates)
                {
                    try
                    {
                        ((Action<BaseEvent>) listener)?.Invoke(evt);
                    }
                    catch (Exception e)
                    {
                        DragonU3DSDK.DebugUtil.LogError($"Dispatch event exception: {e}");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        CommonUtils.CatchErrorLog($"Dispatch event exception: {e}");
#endif
                    }
                }
            }
        }

        public bool DispatchEventImmediately(string type, params object[] datas)
        {
            if (dicEvent.ContainsKey(type) && dicEvent[type] != null)
            {
                // BaseEvent evt = new BaseEvent(type, datas);
                // Action<BaseEvent> fun = dicEvent[type];
                // fun(evt);

                DispatchEventImmediately(new BaseEvent(type, datas));
                return true;
            }

            return false;
        }

        public void clearEvent()
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
                        // BaseEvent qEvent = queueEvent.Dequeue();
                        // if (dicEvent.ContainsKey(qEvent.type) && dicEvent[qEvent.type] != null)
                        // {
                        //     Action<BaseEvent> fun = dicEvent[qEvent.type];
                        //     fun(qEvent);
                        // }

                        DispatchEventImmediately(queueEvent.Dequeue());
                    }
                }
                finally
                {
                    try
                    {
                        if (eventLock.IsWriteLockHeld) eventLock.ExitWriteLock();
                    }
                    catch (SynchronizationLockException e)
                    {
                        DragonU3DSDK.DebugUtil.Log("SynchronizationLockException : " + e.Message);
                    }
                }
            }
        }
    }
}