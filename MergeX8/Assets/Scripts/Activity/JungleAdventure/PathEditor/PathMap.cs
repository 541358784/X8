using System.Collections.Generic;
using UnityEngine;

namespace JungleAdventure
{
    [System.Serializable]
    public class SerializablePointList
    {
        public List<Vector2> points = new List<Vector2>();
    }

    [System.Serializable]
    public partial class PathMap : MonoBehaviour
    {
        public float _speed = 1f;

        public List<Segment> _segments;
        [SerializeField]
        public List<SerializablePointList> _points = new List<SerializablePointList>();
        public List<float> _pathLength = new List<float>();
        
    }
}