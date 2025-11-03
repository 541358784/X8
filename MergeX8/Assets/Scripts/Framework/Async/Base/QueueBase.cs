/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
//
// 模块名：QueueBase
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：同步队列Api基类
//-------------------------------------------------------------------------------------------*/

using UnityEngine;
using System.Threading;

namespace Framework.Async.Base
{
    public class QueueBase : MonoBehaviour
    {
        bool mAppHasQuit = false;

        void Awake()
        {
            mAppHasQuit = false;
            gameObject.hideFlags = HideFlags.NotEditable;
            Init();
        }

        /// <summary>
        /// Called automatically by Unity when the application quits. Calls the <see cref="ShutDown"/> method.
        /// </summary>
        void OnApplicationQuit()
        {
            mAppHasQuit = true;
        }


        public bool AppHasQuit
        {
            get { return mAppHasQuit; }
        }

        protected bool IsMainThread()
        {
            //		Debug.LogErrorFormat ("Current thread is main : {0}， IsBackground : {1}", !Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.IsBackground);
            return !Thread.CurrentThread.IsThreadPoolThread && !Thread.CurrentThread.IsBackground;
        }

        public virtual void Init()
        {
        }

        public virtual void UpdateByMainThread()
        {
        }

        public virtual void Enqueue(Async.AsyncFunction act, float time = 0)
        {
        }
    }
}