using System.Collections.Generic;

namespace SomeWhere
{
    [System.Serializable]
    public class RT_Point
    {
        public RT_Point(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
        public float _x;
        public float _y;
        public float _z;
        public float _angle;
    }
    
    [System.Serializable]
    public class RT_PathSegment
    {
        public float _waitTime;
        public float _moveSpeed;
        public string _playAnimName;
        public string _defaultAnimName;
        public bool _autoRotation;
        public RT_Point _rotateAngle;
        public List<RT_Point> _points = new List<RT_Point>();
    }
}