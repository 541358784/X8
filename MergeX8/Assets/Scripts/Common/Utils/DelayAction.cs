using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using DragonU3DSDK;

namespace DragonPlus
{
    public class DelayAction
    {
        Timer _timerDbc;
        Timer _timerTrt;

        static Dictionary<string, DelayAction> DebounceDic = new Dictionary<string, DelayAction>();
        static Dictionary<string, DelayAction> ThrottleDic = new Dictionary<string, DelayAction>();

        /// <summary>
        /// 延迟timesMs后执行。 在此期间如果再次调用，则重新计时
        /// </summary>
        /// <param name="invoker">同步对象，一般为Control控件。 如不需同步可传null</param>
        void Debounce(string key, int timeMs, ISynchronizeInvoke invoker, Action action)
        {
            lock (this)
            {
                if (_timerDbc == null)
                {
                    _timerDbc = new Timer(timeMs);
                    _timerDbc.AutoReset = false;
                    _timerDbc.Elapsed += (o, e) =>
                    {
                        _timerDbc.Stop();
                        _timerDbc.Close();
                        _timerDbc = null;
                        if (DebounceDic.ContainsKey(key))
                            DebounceDic.Remove(key);

                        InvokeAction(action, invoker);
                    };
                }

                _timerDbc.Stop();
                _timerDbc.Start();
            }
        }

        /// <summary>
        /// 即刻执行，执行之后，在timeMs内再次调用无效
        /// </summary>
        /// <param name="timeMs">不应期，这段时间内调用无效</param>
        /// <param name="invoker">同步对象，一般为控件。 如不需同步可传null</param>
        void Throttle(string key, int timeMs, ISynchronizeInvoke invoker, Action action)
        {
            System.Threading.Monitor.Enter(this);
            bool needExit = true;
            try
            {
                if (_timerTrt == null)
                {
                    _timerTrt = new Timer(timeMs);
                    _timerTrt.AutoReset = false;
                    _timerTrt.Elapsed += (o, e) =>
                    {
                        _timerTrt.Stop();
                        _timerTrt.Close();
                        _timerTrt = null;
                        if (ThrottleDic.ContainsKey(key))
                            ThrottleDic.Remove(key);
                    };
                    _timerTrt.Start();
                    System.Threading.Monitor.Exit(this);
                    needExit = false;
                    InvokeAction(action, invoker); //这个过程不能锁
                }
            }
            finally
            {
                if (needExit)
                    System.Threading.Monitor.Exit(this);
            }
        }

        static void InvokeAction(Action action, ISynchronizeInvoke invoker)
        {
            if (invoker == null)
            {
                action();
            }
            else
            {
                if (invoker.InvokeRequired)
                {
                    invoker.Invoke(action, null);
                }
                else
                {
                    action();
                }
            }
        }

        public static void Debounce(string key, int timeMs, Action action)
        {
            if (!DebounceDic.ContainsKey(key))
                DebounceDic.Add(key, new DelayAction());

            DebounceDic[key].Debounce(key, timeMs, null, action);
        }

        public static void Throttle(string key, int timeMs, Action action)
        {
            if (!ThrottleDic.ContainsKey(key))
                ThrottleDic[key] = new DelayAction();

            ThrottleDic[key].Throttle(key, timeMs, null, action);
        }
    }
}