using System;
using System.Collections.Generic;

namespace OnePath
{
    [Serializable]
    public class OnePathConfig
    {        
        public float _width;
        public float _height;
        
        public int _gridRow;//行
        public int _gridCol;//列

        public float _x;
        public float _y;
        
        public List<OnePathChunk> _chunks = new List<OnePathChunk>();

        public int GetIndex(int row, int col)
        {
            int index = row * _gridCol + col;
            return index;
        }
        public OnePathChunk GetChunk(int row, int col)
        {
            int index = GetIndex(row, col);

            return GetChunk(index);
        }

        public OnePathChunk GetChunk(int index)
        {
            if (index < 0 || index >= _chunks.Count)
                return null;

            return _chunks[index];
        }
    }
}