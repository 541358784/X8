using System.Collections.Generic;
using UnityEngine;

namespace SomeWhere
{
    [System.Serializable]
    public class PathInfo
    {
        public string _pathId;

        public bool _isPlay;
        public List<PathSegment> _segmentLists;
    }
}