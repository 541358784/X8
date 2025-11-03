using System.Collections.Generic;
using UnityEngine;

namespace SomeWhere
{
    [System.Serializable]
    public partial class PathSegment
    {
        public PathPoint p1;
        public PathPoint p2;
        public Transform cp1;
        public Transform cp2;

        private float _distance;
        public Dictionary<float, Vector2> CacheDic = new Dictionary<float, Vector2>();
        public Dictionary<float, Vector2> CacheReverseDic = new Dictionary<float, Vector2>();

        public void CacheResult(float percent, Vector3 result, bool reverse)
        {
#if UNITY_EDITOR
            return;
#endif
            if (reverse)
            {
                CacheReverseDic[percent] = result;
            }
            else
            {
                CacheDic[percent] = result;
            }
        }

        public PathPoint GetEnd(bool reverse)
        {
            if (reverse) return p1;
            return p2;
        }

        public bool ConnectFlow(PathSegment segment)
        {
            var connected = false;

            if (!connected)
                if (segment.p2 != null && p1 != null && segment.p2 == p1)
                    connected = true;

            return connected;
        }

        public float Distance()
        {
            if (_distance == 0)
            {
                if (cp1 == null && cp2 == null)
                {
                    _distance = Vector2.Distance(p1.Position, p2.Position);
                }
                else
                {
                    if (cp1 && cp2)
                    {
                        _distance = Vector2.Distance(p1.Position, cp1.position);
                        _distance += Vector2.Distance(cp1.position, cp2.position);
                        _distance += Vector2.Distance(p2.Position, cp2.position);
                    }
                    else
                    {
                        if (cp1)
                        {
                            _distance = Vector2.Distance(p1.Position, cp1.position);
                            _distance += Vector2.Distance(p2.Position, cp1.position);
                        }
                        else if (cp2)
                        {
                            _distance = Vector2.Distance(p1.Position, cp2.position);
                            _distance += Vector2.Distance(p2.Position, cp2.position);
                        }
                    }
                }
            }

            return _distance;
        }
    }
}