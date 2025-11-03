using System.Collections.Generic;
using UnityEngine;

namespace SomeWhere
{
    public partial class PathSegment
    {
        public PathPoint GetBegin(bool reverse)
        {
            if (reverse) return p2;
            return p1;
        }
    }
}