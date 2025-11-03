using System.Collections.Generic;
using Decoration;
using DragonU3DSDK;
using UnityEngine;

namespace SomeWhere
{
    public partial class PathPoint : MonoBehaviour
    {
        public float _waitTime;
        public float _moveSpeed;
        public string _playAnimName;
        public string _defaultAnimName;
        public Vector3 _rotateAngle;
        public bool _autoRotation;
        public Vector3 Position
        {
            get { return transform.position; }
        }
    }
}