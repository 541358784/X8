using UnityEngine;
using System;
using System.Collections.Generic;

namespace OnePath
{
    [Serializable]
    public class OnePathChunk
    {
        public int index;
        public int row;
        public int col;
        public float minX;
        public float minY;

        public List<OnePathPixel> _pixels = new List<OnePathPixel>();

        private Rect _rect;
        private bool _isRect = false;
        public Rect GetRect(float width, float height)
        {
            if (!_isRect)
            {
                _rect = new Rect(minX, minY, width, height);
                _isRect = true;
            }

            return _rect;
        }
    }
}