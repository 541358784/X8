using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneLine
{
    public sealed partial class OneLineGraphic
    {
        public sealed class Edge
        {
            public Point   A;
            public Point   B;
            public Pixel[] Path; // 始终从A到B

            public void SetUp(Point a, Point b, Pixel[] path)
            {
                A    = a;
                B    = b;
                Path = path;
            }

            public Point GetEndPoint(Point startPoint)
            {
                if (A == startPoint) return B;
                if (B == startPoint) return A;
                throw new ArgumentException("输入点不是该边中的任意一点", nameof(startPoint));
            }

            /// <summary>
            /// 获取从输入的起点开始迭代所有路径
            /// </summary>
            public Iterator GetPathIterator(Point startPoint)
            {
                if (A == startPoint)
                {
                    return new Iterator(Path, true);
                }

                if (B == startPoint)
                {
                    return new Iterator(Path, false);
                }

                throw new ArgumentException("输入点不是该边中的任意一点", nameof(startPoint));
            }

            /// <summary>
            /// 获取从当前位置偏移一定值后的路径点位置
            /// </summary>
            /// <param name="startPoint">边当前绘制方向的起点</param>
            /// <param name="currentPosition">当前位置</param>
            /// <param name="deltaPosition">偏移量</param>
            /// <returns></returns>
            public Pixel GetBestOffsetPositionOnPath(Point startPoint, Pixel currentPosition, Pixel deltaPosition)
            {
                if (Path.Length == 0) throw new Exception("边所包含的坐标点为空，这种情况是不合理的！不应该出现。");

                int currentIndex = -1;
                for (var i = Path.Length - 1; i >= 0; i--)
                {
                    if (Path[i] == currentPosition)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex == -1) throw new ArgumentOutOfRangeException();

                const float AllowThreshold = 1.5f;

                float absDeltaX = Mathf.Abs(deltaPosition.x);
                float absDeltaY = Mathf.Abs(deltaPosition.y);

                int toAIndex = currentIndex;
                for (; toAIndex > 0; toAIndex--)
                {
                    var   tempDelta = Path[toAIndex] - currentPosition;
                    float xRatio    = 1f * tempDelta.x / (deltaPosition.x == 0 ? 1 : deltaPosition.x);
                    float yRatio    = 1f * tempDelta.y / (deltaPosition.y == 0 ? 1 : deltaPosition.y);

                    if (absDeltaX >= absDeltaY)
                    {
                        yRatio = Mathf.Abs(yRatio);
                        if (IsPositiveAndLessThan(xRatio, 1f) && IsPositiveAndLessThan(yRatio, AllowThreshold))
                        {
                            continue;
                        }
                    }

                    if (absDeltaY >= absDeltaX)
                    {
                        xRatio = Mathf.Abs(xRatio);
                        if (IsPositiveAndLessThan(yRatio, 1f) && IsPositiveAndLessThan(xRatio, AllowThreshold))
                        {
                            continue;
                        }
                    }

                    break;
                }

                int toBIndex = currentIndex;
                for (; toBIndex < Path.Length - 1; toBIndex++)
                {
                    var   tempDelta = Path[toBIndex] - currentPosition;
                    float xRatio    = 1f * tempDelta.x / (deltaPosition.x == 0 ? 1 : deltaPosition.x);
                    float yRatio    = 1f * tempDelta.y / (deltaPosition.y == 0 ? 1 : deltaPosition.y);

                    if (absDeltaX >= absDeltaY)
                    {
                        yRatio = Mathf.Abs(yRatio);
                        if (IsPositiveAndLessThan(xRatio, 1f) && IsPositiveAndLessThan(yRatio, AllowThreshold))
                        {
                            continue;
                        }
                    }

                    if (absDeltaY >= absDeltaX)
                    {
                        xRatio = Mathf.Abs(xRatio);
                        if (IsPositiveAndLessThan(yRatio, 1f) && IsPositiveAndLessThan(xRatio, AllowThreshold))
                        {
                            continue;
                        }
                    }

                    break;
                }

                if (toAIndex == currentIndex) return Path[toBIndex];
                if (toBIndex == currentIndex) return Path[toAIndex];

                Pixel offsetToA = Path[toAIndex] - currentPosition;
                Pixel offsetToB = Path[toBIndex] - currentPosition;
                // 如果两者偏移相同，则朝前
                if (offsetToA == offsetToB) return startPoint == A ? Path[toBIndex] : Path[toAIndex];
                // 否则找到目标点偏移与输入偏移差距小的一方，往那个方向移动
                return (offsetToA - deltaPosition).sqrMagnitude < (offsetToB - deltaPosition).sqrMagnitude ? Path[toAIndex] : Path[toBIndex];
            }

            /// <summary>
            /// 获取当前绘制的路径数量
            /// </summary>
            /// <param name="startPoint">起点</param>
            /// <param name="currentPosition">当前位置</param>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException">当前位置不在该边路径上</exception>
            /// <exception cref="Exception">输入起点不是该边的两个终点之一</exception>
            public int GetPixelIndex(Point startPoint, Pixel currentPosition)
            {
                int currentIndex = -1;
                for (var i = Path.Length - 1; i >= 0; i--)
                {
                    if (Path[i] == currentPosition)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex == -1) throw new ArgumentOutOfRangeException();

                // 需要排除最后一个点
                if (startPoint == A) return currentIndex;
                if (startPoint == B) return Path.Length - currentIndex - 1;
                throw new Exception("边不包含输入的起点，这种情况是不合理的！不应该出现。");
            }

            /// <summary>
            /// 获取从起点开始，第index个路径点像素
            /// </summary>
            /// <param name="startPoint"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public Pixel GetPixelAtIndex(Point startPoint, int index)
            {
                if (startPoint == A) return Path[index];
                if (startPoint == B) return Path[Path.Length - index - 1];
                throw new Exception("边不包含输入的起点，这种情况是不合理的！不应该出现。");
            }

            /// <summary>
            /// 获取距离目标点最小的距离
            /// </summary>
            /// <returns></returns>
            public int GetSmallestDistance(Pixel targetPosition)
            {
                int min = int.MaxValue;
                for (int i = 0; i < Path.Length; i++)
                {
                    int temp = (Path[i] - targetPosition).sqrMagnitude;
                    if (temp < min)
                    {
                        min = temp;
                    }
                }

                return min;
            }

            private static bool IsPositiveAndLessThan(float value, float b)
            {
                return value >= 0f && value <= b;
            }

            public struct Iterator : IEnumerator<Pixel>
            {
                public Iterator(Pixel[] positions, bool order)
                {
                    m_Positions = positions;
                    m_Order     = order;
                    m_Index     = order ? -1 : positions.Length;
                }

                private Pixel[] m_Positions;
                private bool    m_Order;
                private int     m_Index;

                public bool MoveNext()
                {
                    if (m_Order) return ++m_Index < m_Positions.Length;
                    return --m_Index >= 0;
                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }

                public Pixel Current => m_Positions[m_Index];

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }
            }
        }
    }
}