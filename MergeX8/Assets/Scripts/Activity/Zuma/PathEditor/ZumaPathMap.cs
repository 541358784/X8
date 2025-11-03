using System.Collections.Generic;
using UnityEngine;

namespace Zuma
{
    [System.Serializable]
    public class Vector2Wrapper
    {
        public float x;
        public float y;

        public Vector2Wrapper(Vector2 vector2)
        {
            this.x = vector2.x;
            this.y = vector2.y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, 0);
        }
    }

    [System.Serializable]
    public partial class ZumaPathMap : MonoBehaviour
    {
        public string _pathId;
        public float _radius = 0.3f;
        public float _speed = 1f;

        public List<Segment> _segments;
        public List<Vector2Wrapper> _point = new List<Vector2Wrapper>();
    }
}