using UnityEngine;

namespace OneLine
{
    public sealed partial class OneLineGraphic
    {
        public sealed class Point
        {
            public Pixel  Position;
            public Edge[] Edges;

            public void SetUp(Pixel position, Edge[] edges)
            {
                Position = position;
                Edges    = edges;
            }
        }
    }
}