using Screw;
using Screw.Configs;
using UnityEngine;

namespace Screw
{
    public class HoleModel
    {
        public int HoleId { get; }

        public Vector3 Position { get; }

        public HoleModel(int holeId, Vector3Float position)
        {
            HoleId = holeId;
            Position = new Vector3(position.x, position.y, position.z);
        }
    }
}