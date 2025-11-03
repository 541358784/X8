using System.Collections.Generic;

namespace Screw.Configs
{
    public class LevelPanel
    {
        public int instanceId;
        public Vector3Float scale;
        public Vector3Float position;
        public Vector3Float rotate;
        public Vector3Float bodyScale;
        public Vector3Float bodyRotate;
        public string bodyImageName;
        public Vector3Float shadowPosition;
        public ColorType colorType;
        public List<LevelHole> holes = new List<LevelHole>();
    }
}