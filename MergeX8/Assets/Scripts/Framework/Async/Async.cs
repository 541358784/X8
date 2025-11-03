/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
//
// 模块名：Async
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：异步Api
//-------------------------------------------------------------------------------------------*/

using System.Threading;
using Framework.Async.Base;

namespace Framework.Async
{
    public enum AsyncType
    {
        AsyncQueue,
        SyncQueue
    }

    //异步执行工具
    public sealed class Async
    {
        #region private

        static void Enqueue(AsyncType type, AsyncFunction act, float time = 0f)
        {
            if (QueueManager.Instance != null)
            {
                QueueManager.Instance.Enqueue(type, act, time);
            }
        }

        #endregion

        #region Async API

        public delegate void AsyncFunction();

        /// <summary>
        /// 在主线程运行
        /// </summary>
        /// <param name="act"></param>
        public static void RunOnMainThread(AsyncFunction act)
        {
            Enqueue(AsyncType.SyncQueue, act);
        }

        /// <summary>
        /// 在异步线程运行
        /// </summary>
        /// <param name="act"></param>
        public static void RunOnAsyncThread(AsyncFunction act)
        {
            Enqueue(AsyncType.AsyncQueue, act);
        }

        /// <summary>
        /// 休眠
        /// </summary>
        /// <param name="millisecondsTimeout">单位毫秒</param>
        public static void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        #endregion
    }

    public static class AsyncUtils
    {
        #region Async API

        public static void RunOnMainThread(this object obj, Async.AsyncFunction act)
        {
            Async.RunOnMainThread(act);
        }

        public static void RunOnAsyncThread(this object obj, Async.AsyncFunction act)
        {
            Async.RunOnAsyncThread(act);
        }

        #endregion
    }
}