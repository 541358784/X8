/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
//
// 模块名：PriorityQueue
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：使用堆实现优先队列
//-------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace Framework.DataStructure
{
    public class PriorityQueue<T> where T : IComparable
    {
        private List<T> __data;

        public PriorityQueue()
        {
            __data = new List<T>();
        }

        public PriorityQueue(int capacity)
        {
            __data = new List<T>(capacity);
        }

        public int Count
        {
            get { return __data.Count; }
        }

        public T Peek()
        {
            if (Count <= 0) return default(T);
            return __data[0];
        }

        public void Enqueue(T value)
        {
            PushHeap(value);
        }

        public T Dequeue()
        {
            return PopHeap();
        }

        public void Clear()
        {
            __data.Clear();
        }

        public bool Contains(T value)
        {
            return __data.Contains(value);
        }

        #region private

        /// <summary>
        /// 将元素压入数组最后，然后进行调整
        /// </summary>
        /// <param name="value"></param>
        private void PushHeap(T value)
        {
            __data.Add(value);
            __PushHeap(value, Count - 1);
        }

        /// <summary>
        /// 从堆顶弹出第一个元素
        /// </summary>
        /// <returns></returns>
        private T PopHeap()
        {
            if (Count <= 0) return default(T);
            //将数组最后一个元素与第一个交换位置
            T result = __data[0];
            __AdjustHeap(__data[Count - 1]);
            __data.RemoveAt(Count - 1);
            return result;
        }

        /*
         * 堆的结构如下：
         *                0
         *               / \
         *              /   \
         *             1     2
         *            / \   / \
         *           3   4 5   6
         * */

        /// <summary>
        /// Heap 从下往上调整
        /// </summary>
        /// <param name="value"></param>
        /// <param name="holeIndex"></param>
        private void __PushHeap(T value, int holeIndex)
        {
            int parent = (holeIndex - 1) >> 1;
            while (holeIndex > 0 && value.CompareTo(__data[parent]) < 0)
            {
                __data[holeIndex] = __data[parent];
                holeIndex = parent;
                parent = (holeIndex - 1) >> 1;
            }

            __data[holeIndex] = value;
        }

        /// <summary>
        /// Heap 从上往下调整
        /// </summary>
        /// <param name="value"></param>
        private void __AdjustHeap(T value)
        {
            int holeIndex = 0;
            int secondChild = (holeIndex << 1) + 2;
            while (secondChild < Count)
            {
                if (__data[secondChild].CompareTo(__data[secondChild - 1]) < 0)
                {
                    secondChild--;
                }

                __data[holeIndex] = __data[secondChild];
                holeIndex = secondChild;
                secondChild = (secondChild + 1) << 1;
            }

            if (secondChild == Count)
            {
                __data[holeIndex] = __data[secondChild - 1];
                holeIndex = secondChild - 1;
            }

            //将最后一个元素压入堆中进行调整
            __PushHeap(value, holeIndex);
        }

        #endregion
    }
}