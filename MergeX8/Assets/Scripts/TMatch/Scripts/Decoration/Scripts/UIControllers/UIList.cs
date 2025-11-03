using System;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMatch
{


    /// <summary>
    /// UI列表
    /// </summary>
    public class UIList
    {
        private Transform parent;
        private GameObject item;
        private Action<GameObject, int> onRefresh;
        private List<GameObject> items = new List<GameObject>();

        /// <summary>
        /// 列表项
        /// </summary>
        public List<GameObject> Items => items;

        private GameObject GetItem(int index)
        {
            if (items.Count <= index)
            {
                items.Add(Object.Instantiate(item, parent));
            }

            items[index].SetActive(true);
            return items[index];
        }

        private void HideOthers(int count)
        {
            for (int i = items.Count; i > count; i--)
            {
                items[i - 1].SetActive(false);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="item">Item</param>
        /// <param name="onRefresh">刷新回调</param>
        public UIList(Transform parent, GameObject item, Action<GameObject, int> onRefresh)
        {
            this.item = item;
            this.parent = parent;
            this.onRefresh = onRefresh;
            items.Add(item);
        }

        /// <summary>
        /// 展示
        /// </summary>
        /// <param name="count">个数</param>
        public void Show(int count)
        {
            for (int i = 0; i < count; i++)
            {
                try
                {
                    onRefresh(GetItem(i), i);
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.Message);
                }
            }

            HideOthers(count);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i).gameObject;
                if (!items.Contains(child))
                    Object.Destroy(child);
            }
        }

        /// <summary>
        /// 销毁（多数情况下不用，因为会跟随界面有生命周期）
        /// </summary>
        public void Destory()
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                GameObject item = items[i];
                Object.DestroyImmediate(item);
                items.RemoveAt(i);
            }
        }
    }
}
