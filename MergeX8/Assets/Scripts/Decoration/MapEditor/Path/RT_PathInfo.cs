using System.Collections.Generic;

namespace SomeWhere
{
    [System.Serializable]
    public class RT_PathInfo
    {
        public string _pathId;

        public List<RT_PathSegment> _segmentLists = new List<RT_PathSegment>();
    }
}