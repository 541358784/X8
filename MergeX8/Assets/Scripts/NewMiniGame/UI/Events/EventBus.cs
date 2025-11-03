using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Utils
{
    public interface IEvent
    {
    }

    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> HandlerMap = new Dictionary<Type, Delegate>();

        public static void Register<T>(Action<T> action) where T : IEvent
        {
            var eventType = typeof(T);
            if (HandlerMap.ContainsKey(eventType))
            {
                var handle = HandlerMap[eventType] as Action<T>;
                if (handle != null && handle.GetInvocationList().Contains(action))
                    return;

                handle += action;
                HandlerMap[eventType] = handle;
            }
            else
            {
                HandlerMap.Add(typeof(T), action);
            }
        }

        public static void UnRegister<T>(Action<T> action)
        {
            var eventType = typeof(T);
            if (!HandlerMap.ContainsKey(eventType))
                return;
            var handle = HandlerMap[eventType];
            if (!handle.GetInvocationList().Contains(action))
                return;
            handle = Delegate.Remove(handle, action);
            if (handle == null)
            {
                HandlerMap.Remove(eventType);
                return;
            }

            HandlerMap[eventType] = handle;
        }
        
        public static void UnRegister(Type t, Delegate action)
        {
            var eventType = t;
            if (!HandlerMap.ContainsKey(eventType))
                return;
            var handle = HandlerMap[eventType];
            if (!handle.GetInvocationList().Contains(action))
                return;
            handle = Delegate.Remove(handle, action);
            if (handle == null)
            {
                HandlerMap.Remove(eventType);
                return;
            }

            HandlerMap[eventType] = handle;
        }
        

        public static void Send<T>(T eventData = default) where T : IEvent
        {
            var t = typeof(T);
            if (!HandlerMap.ContainsKey(t))
                return;
            var handle = HandlerMap[t] as Action<T>;
            handle?.Invoke(eventData);
        }
    }
}