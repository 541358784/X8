/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
//
// 模块名：AsyncQueue
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：异步队列
//-------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

namespace Framework.Async.Base
{
    using Framework.Async;

    public class AsyncQueue : QueueBase
    {
        //最大线程数
        private int mThreadsMaxNum = 4;

        //当前线程数
        private int mCurrentThreadsNum = 0;

        //当前异步队列
        private Queue<Async.AsyncFunction> mAsyncQueue = new Queue<Async.AsyncFunction>();

        //防止主线程阻塞 当主线程调用被原子变量锁住的mAsyncQueue队列时，先缓存之后Update中执行RunOnAsyncThread
        private Queue<Async.AsyncFunction> mMainThreadAsyncQueue = new Queue<Async.AsyncFunction>();

        private int mAsyncQueueAtom = 0;

        public override void Enqueue(Async.AsyncFunction act, float time = 0)
        {
            if (mCurrentThreadsNum >= mThreadsMaxNum)
            {
                if (AsyncStatus.Success == Interlocked.Exchange(ref mAsyncQueueAtom, AsyncStatus.Lock))
                {
                    //空闲
                    mAsyncQueue.Enqueue(act);
                    Interlocked.Exchange(ref mAsyncQueueAtom, AsyncStatus.UnLock);
                }
                else
                {
                    //被占用
                    if (IsMainThread())
                    {
                        //主线程执行
                        if (!mMainThreadAsyncQueue.Contains(act))
                        {
                            mMainThreadAsyncQueue.Enqueue(act);
                        }
                    }
                    else
                    {
                        //异步线程执行
                        while (AsyncStatus.Success != Interlocked.Exchange(ref mAsyncQueueAtom, AsyncStatus.Lock) &&
                               AppHasQuit != true)
                        {
                            //休眠10ms再尝试获取锁
                            Thread.Sleep(10);
                        }

                        mAsyncQueue.Enqueue(act);
                        Interlocked.Exchange(ref mAsyncQueueAtom, AsyncStatus.UnLock);
                    }
                }
            }
            else
            {
                Interlocked.Increment(ref mCurrentThreadsNum);
                ThreadPool.QueueUserWorkItem(AsyncExecute, act);
            }
        }

        void AsyncExecute(object action)
        {
            try
            {
                ((Async.AsyncFunction) action)();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                Interlocked.Decrement(ref mCurrentThreadsNum);
                if (mCurrentThreadsNum < mThreadsMaxNum)
                {
                    while (AsyncStatus.Success != Interlocked.Exchange(ref mAsyncQueueAtom, AsyncStatus.Lock) &&
                           AppHasQuit != true)
                    {
                        //休眠10ms再尝试获取锁
                        Thread.Sleep(10);
                    }

                    if (mAsyncQueue.Count > 0)
                    {
                        if (AppHasQuit != true)
                        {
                            Async.AsyncFunction act = mAsyncQueue.Dequeue();
                            Interlocked.Increment(ref mCurrentThreadsNum);
                            ThreadPool.QueueUserWorkItem(AsyncExecute, act);
                        }
                    }

                    Interlocked.Exchange(ref mAsyncQueueAtom, AsyncStatus.UnLock);
                }
            }
        }


        // Update is called once per frame
        public override void UpdateByMainThread()
        {
            while (mMainThreadAsyncQueue.Count > 0)
            {
                Enqueue(mMainThreadAsyncQueue.Dequeue());
            }
        }
    }
}