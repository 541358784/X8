using System.Collections.Generic;
using UnityEngine;

namespace Screw.Configs
{
    public class LevelLayout
    {
        public int levelId;
        public Vector3Float guidePosition;
        public List<Order> orders = new List<Order>();
        public List<LevelLayer> layers = new List<LevelLayer>();
    }
}