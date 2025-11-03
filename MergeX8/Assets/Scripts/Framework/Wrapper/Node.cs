using System;
using UnityEngine;

namespace Framework.Wrapper
{
    /// <summary>
    /// 空的游戏物体
    /// </summary>
    public sealed class Node : GameObjectWrapper, IDisposable
    {
        public Node() : base(GameObjectFactory.Create(true))
        {
        }

        public static Node Root = new Node();


        /// <summary>
        /// 需要显式调用Dispose，销毁game object，editor模式下，系统会帮你检查node的泄漏问题
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
#if UNITY_EDITOR
            GC.SuppressFinalize(this); //自己释放了，不需要gc帮我调用析构函数
#endif
        }

#if UNITY_EDITOR
        ~Node()
        {
            Dispose(false);
        }
#endif


        private void ReleaseUnmanagedResources()
        {
            GameObjectFactory.Destroy(_gameObject);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                DragonU3DSDK.DebugUtil.LogError(
                    "Node should call dispose method，or the inner game object wont destroy. node name: {0}",
                    Name);
            }

            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }
    }
}