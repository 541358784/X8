/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
// 
// 模块名：CircleLinkedList
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：双向循环LinkedList
//-------------------------------------------------------------------------------------------*/

namespace Framework.DataStructure
{
    public class CircleLinkedNode<T>
    {
        public CircleLinkedNode<T> Next { get; set; }
        public CircleLinkedNode<T> Previous { get; set; }
        public T Value { get; set; }

        public CircleLinkedNode(T value)
        {
            Value = value;
        }
    }


    public class CircleLinkedList<T>
    {
        [Unity.Collections.ReadOnly] CircleLinkedNode<T> _headNode;

        [Unity.Collections.ReadOnly] CircleLinkedNode<T> _tailNode;

        [Unity.Collections.ReadOnly] int _count = 0;

        /// <summary>
        /// Gets the head.
        /// </summary>
        /// <value>The head.</value>
        /// 获取头节点
        public CircleLinkedNode<T> headNode
        {
            get { return _headNode; }
        }

        /// <summary>
        /// Gets the tail.
        /// </summary>
        /// <value>The tail.</value>
        /// 获取尾节点
        public CircleLinkedNode<T> tailNode
        {
            get { return _tailNode; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        /// 返回循环列表的节点数
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Add the specified data.
        /// </summary>
        /// <param name="data">Data.</param>
        /// 添加节点
        public void Add(T data)
        {
            CircleLinkedNode<T> node = new CircleLinkedNode<T>(data);
            if (_headNode == null)
            {
                _headNode = node;
            }

            if (_headNode != node)
            {
                node.Previous = _tailNode;
                _tailNode.Next = node;
                _headNode.Previous = node;
            }

            _tailNode = node;
            if (_headNode != _tailNode)
            {
                _tailNode.Next = _headNode;
            }

            ++_count;
        }

        /// <summary>
        /// Inserts the next.
        /// </summary>
        /// <param name="posNode">Position node.</param>
        /// <param name="insertData">Insert data.</param>
        public void InsertNext(CircleLinkedNode<T> posNode, T insertData)
        {
            CircleLinkedNode<T> node = new CircleLinkedNode<T>(insertData);
            node.Next = posNode.Next;
            node.Previous = posNode;

            posNode.Next.Previous = node;
            posNode.Next = node;
            if (posNode == _tailNode)
            {
                _tailNode = posNode;
            }

            ++_count;
        }

        /// <summary>
        /// Inserts the previous.
        /// </summary>
        /// <param name="posNode">Position node.</param>
        /// <param name="insertData">Insert data.</param>
        public void InsertPrevious(CircleLinkedNode<T> posNode, T insertData)
        {
            CircleLinkedNode<T> node = new CircleLinkedNode<T>(insertData);
            node.Next = posNode;
            node.Previous = posNode.Previous;
            node.Previous.Next = node;
            posNode.Previous = node;
            if (posNode == _headNode)
            {
                _tailNode = posNode;
            }

            ++_count;
        }

        /// <summary>
        /// Remove the specified removeNode.
        /// </summary>
        /// <param name="removeNode">Remove node.</param>
        /// 删除节点
        public void Remove(CircleLinkedNode<T> removeNode)
        {
            if (removeNode == null)
            {
                return;
            }

            CircleLinkedNode<T> removePrevNode = removeNode.Previous;

            removePrevNode.Next = removeNode.Next;
            removeNode.Next.Previous = removePrevNode;
            removeNode.Next = null;
            removeNode.Previous = null;
        }

        /// <summary>
        /// Gets the <see cref="T:Extensions.Base.CircleLinkedList`1"/> with the specified findIndex.
        /// </summary>
        /// <param name="findIndex">Find index.</param>
        /// 通过索引找节点
        public CircleLinkedNode<T> this[int findIndex]
        {
            get
            {
                CircleLinkedNode<T> findNode = _headNode;
                int fetchIndex = 0;
                while (findNode != null)
                {
                    if (fetchIndex.Equals(findIndex))
                    {
                        return findNode;
                    }

                    ++fetchIndex;
                    findNode = findNode.Next;

                    if (findNode == _headNode)
                    {
                        findNode = null;
                    }
                }

                return null;
            }
        }
    }
}