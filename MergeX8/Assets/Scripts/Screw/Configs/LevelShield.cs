using System.Collections.Generic;

namespace Screw.Configs
{
    public class LevelShield
    {
        public int instanceId;
        public Vector3Float scale;
        public Vector3Float position;
        public Vector3Float rotate;
        public Vector3Float bodyScale;
        public Vector3Float bodyRotate;
        public string bodyImageName;

        public List<int> coverPanelIds = new List<int>();
    }
}