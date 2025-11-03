using System.Collections.Generic;

namespace Screw.Configs
{
    public class LevelScrew
    {
        public int instanceID;
        public Vector3Float position;

        public ScrewShape shape;
        public ColorType colorType;
        public float radius;

        public List<LevelScrewBlock> screwBlocks = new List<LevelScrewBlock>();
    }
}