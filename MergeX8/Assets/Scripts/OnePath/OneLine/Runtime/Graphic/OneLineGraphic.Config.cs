using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OneLine
{
    public sealed partial class OneLineGraphic
    {
        [Serializable]
        private struct PointConfig
        {
            /// <summary>
            /// 点像素坐标
            /// </summary>
            public Pixel Position;

            /// <summary>
            /// 所连接的边的index下标
            /// </summary>
            public int[] Edges;
        }

        [Serializable]
        private struct EdgeConfig
        {
            /// <summary>
            /// 边的两个点之一
            /// </summary>
            public int A, B;

            /// <summary>
            /// 这条边所经过的像素坐标
            /// </summary>
            public Pixel[] Path;
        }

        [SerializeField]
        private int m_BrushSize;

        [SerializeField]
        private Pixel[] m_AllPath = new Pixel[0];

        [SerializeField]
        private PointConfig[] m_PointConfigs = new PointConfig[0];

        [SerializeField]
        private EdgeConfig[] m_EdgeConfigs = new EdgeConfig[0];

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Points = new Point[m_PointConfigs.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = new Point();
            }

            Edges = new Edge[m_EdgeConfigs.Length];
            for (int i = 0; i < Edges.Length; i++)
            {
                Edges[i] = new Edge();
            }

            for (int i = 0; i < Points.Length; i++)
            {
                PointConfig config = m_PointConfigs[i];
                Points[i].SetUp(config.Position, config.Edges.Select(x => Edges[x]).ToArray());
            }

            for (var i = 0; i < Edges.Length; i++)
            {
                EdgeConfig config = m_EdgeConfigs[i];
                Edges[i].SetUp(Points[config.A], Points[config.B], config.Path);
            }
            
            List<Pixel> brushArea = new List<Pixel>();
            for (int x = -m_BrushSize; x <= m_BrushSize; x++)
            {
                for (int y = -m_BrushSize; y <= m_BrushSize; y++)
                {
                    Pixel p = new Pixel(x, y);
                    if (p.sqrMagnitude <= m_BrushSize * m_BrushSize)
                    {
                        brushArea.Add(p);
                    }
                }
            }

            BrushArea = brushArea.ToArray();
        }
    }
}