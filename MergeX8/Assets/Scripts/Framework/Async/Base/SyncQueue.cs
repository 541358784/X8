/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
//
// 模块名：SyncQueue
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：同步队列，用于将异步线程执行同步到主线
//-------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading;

namespace Framework.Async.Base
{
    public class SyncQueue : QueueBase
    {
        //立即列表
        private List<Async.AsyncFunction> mActions = new List<Async.AsyncFunction>();

        //当前立即执行列表
        private List<Async.AsyncFunction> mCurrentActions = new List<Async.AsyncFunction>();

        //防止主线程阻塞 当主线程调用被原子变量锁住的mActions列表时，先缓存之后Update中在加入mActions列表
        private Queue<Async.AsyncFunction> mMainThreadActionsQueue = new Queue<Async.AsyncFunction>();

        private int mActionAtom;

        public override void Enqueue(Async.AsyncFunction act, float time = 0)
        {
            if (AsyncStatus.Success == Interlocked.Exchange(ref mActionAtom, AsyncStatus.Lock))
            {
                //空闲
                mActions.Add(act);
                Interlocked.Exchange(ref mActionAtom, AsyncStatus.UnLock);
            }
            else
            {
                //被占用
                if (IsMainThread())
                {
                    //主线程执行
                    mMainThreadActionsQueue.Enqueue(act);
                }
                else
                {
                    //异步线程执行
                    while (AsyncStatus.Success != Interlocked.Exchange(ref mActionAtom, AsyncStatus.Lock) &&
                           AppHasQuit != true)
                    {
                        Thread.Sleep(10);
                    }

                    mActions.Add(act);
                    Interlocked.Exchange(ref mActionAtom, AsyncStatus.UnLock);
                }
            }
        }

        // Update is called once per frame
        public override void UpdateByMainThread()
        {
            if (AsyncStatus.Success == Interlocked.Exchange(ref mActionAtom, AsyncStatus.Lock))
            {
                mCurrentActions.Clear();
                while (mMainThreadActionsQueue.Count > 0)
                {
                    mActions.Add(mMainThreadActionsQueue.Dequeue());
                }

                if (mActions.Count > 0)
                {
                    mCurrentActions.AddRange(mActions);
                }

                mActions.Clear();
                Interlocked.Exchange(ref mActionAtom, AsyncStatus.UnLock);
            }

            for (int i = 0; i < mCurrentActions.Count; ++i)
            {
                mCurrentActions[i]();
            }

            if (mCurrentActions.Count > 0)
            {
                mCurrentActions.Clear();
            }
        }
    }
}