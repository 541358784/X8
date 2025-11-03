using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DragonU3DSDK
{
    public class SingleActionManager
    {
        private static SingleActionManager instance = null;
        private static readonly object syslock = new object();

        public static SingleActionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new SingleActionManager();
                        }
                    }
                }
                return instance;
            }
        }

        private ConcurrentDictionary<string, long> timeRecord = null;

        private static float CD = 0.1f;
        private float current_time = 0;

        private SingleActionManager()
        {
            timeRecord = new ConcurrentDictionary<string, long>();
            TimerManager.Instance.AddDelegate(Update);
        }

        void Update(float delta)
        {
            if (current_time < CD)
            {
                current_time += delta;
                return;
            }

            List<string> removedKeys = new List<string>();
            foreach (var record in timeRecord)
            {
                if (Utils.GetTimeStamp() < record.Value)
                {
                    continue;
                }
                removedKeys.Add(record.Key);
            }

            long time;
            foreach (string key in removedKeys)
            {
                timeRecord.TryRemove(key, out time);
            }
            current_time = 0;
        }

        /// <summary>
        /// 防抖，立即执行，在此期间如果再次调用会屏蔽
        /// </summary>
        /// <param name="key">任务key</param>
        /// <param name="freezeTime">冻结时间</param>
        /// <param name="action"></param>
        public void ExecuteOnceInFreezeTime(string key, int freezeTime, Action action)
        {
            if (string.IsNullOrEmpty(key) || action == null)
            {
                return;
            }

            if (timeRecord.ContainsKey(key))
            {
                DebugUtil.Log("SingleActionManager: Task is frozen: key {0}", key);
                return;
            }
            
            action();

            if (freezeTime > 0)
            {
                timeRecord[key] = Utils.GetTimeStamp() + freezeTime;
            }
        }
    }
}
