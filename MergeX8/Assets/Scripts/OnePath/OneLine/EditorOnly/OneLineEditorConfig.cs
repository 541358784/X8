#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace OneLine.Editor
{
    [Serializable]
    public class OneLineEdgeConfig
    {
        public bool Editing;

        public Pixel APosition, BPosition;

        /// <summary>
        /// 边的两个点之一
        /// </summary>
        public int A, B;

        /// <summary>
        /// 这条边所经过的像素坐标
        /// </summary>
        public List<Pixel> Path;

        public Pixel LastPixel => Path[Path.Count - 1];

        public OneLineEdgeConfig Copy()
        {
            return new OneLineEdgeConfig()
            {
                Editing   = false,
                APosition = APosition,
                BPosition = BPosition,
                A         = A,
                B         = B,
                Path      = new List<Pixel>(Path),
            };
        }
    }

    [Serializable]
    public class OneLinePointConfig
    {
        public OneLinePointConfig(Pixel position)
        {
            Position = position;
            Edges    = new List<int>();
        }

        private OneLinePointConfig(OneLinePointConfig other)
        {
            Position = other.Position;
            Edges    = new List<int>(other.Edges);
        }

        /// <summary>
        /// 点像素坐标
        /// </summary>
        public Pixel Position;

        /// <summary>
        /// 所连接的边的index下标
        /// </summary>
        public List<int> Edges;

        public OneLinePointConfig Copy()
        {
            return new OneLinePointConfig(this);
        }
    }
}
#endif