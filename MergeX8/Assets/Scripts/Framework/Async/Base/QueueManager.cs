/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
//
// 模块名：QueueManager
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：队列管理类
//-------------------------------------------------------------------------------------------*/

using UnityEngine;

namespace Framework.Async.Base
{
    public class QueueManager : MonoBehaviour
    {
        private QueueBase mSyncQueue = null;
        private QueueBase mAsyncQueue = null;

        public void Enqueue(AsyncType type, Async.AsyncFunction act, float time = 0f)
        {
            if (type == AsyncType.AsyncQueue)
            {
                if (mAsyncQueue == null)
                {
                    return;
                }

                mAsyncQueue.Enqueue(act, time);
            }
            else if (type == AsyncType.SyncQueue)
            {
                if (mSyncQueue == null)
                {
                    return;
                }

                mSyncQueue.Enqueue(act, time);
            }
        }

        // Use this for initialization
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.NotEditable;
            Init();
        }

        void Init()
        {
            mAsyncQueue = AddObj<AsyncQueue>(AsyncType.AsyncQueue);
            mSyncQueue = AddObj<SyncQueue>(AsyncType.SyncQueue);
        }

        QueueBase AddObj<T>(AsyncType type) where T : QueueBase
        {
            GameObject obj = new GameObject(type.ToString());
            obj.transform.parent = gameObject.transform;
            return obj.AddComponent<T>();
        }

        // Update is called once per frame
        void Update()
        {
            if (mSyncQueue != null)
            {
                mSyncQueue.UpdateByMainThread();
            }

            if (mAsyncQueue != null)
            {
                mAsyncQueue.UpdateByMainThread();
            }
        }

        void OnApplicationQuit()
        {
            DestroyImmediate(gameObject, false);
        }

        #region Static

        private static QueueManager s_instance;

        [RuntimeInitializeOnLoadMethod]
        static void InitAsyncObject()
        {
            // In case the singleton is referenced somewhere in a scene that doesnt have the singleton component on an object
            // we create one and hide it.
            var obj = new GameObject("AsyncManager");
            s_instance = obj.AddComponent<QueueManager>(); // Calls ctor of T
        }

        /// <summary>
        /// Makes sure only one instance of the type exists.
        /// </summary>
        public static QueueManager Instance
        {
            get { return s_instance; }
        }

        #endregion
    }
}