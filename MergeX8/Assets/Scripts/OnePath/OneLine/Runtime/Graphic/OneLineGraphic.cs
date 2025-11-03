using System;
using UnityEngine;

namespace OneLine
{
    [Serializable]
    public sealed partial class OneLineGraphic : ISerializationCallbackReceiver
    {
        [NonSerialized]
        public Point[] Points;

        [NonSerialized]
        public Edge[] Edges;

        [NonSerialized]
        public Pixel[] BrushArea;

        public Pixel[] AllPath => m_AllPath;
    }
}