using System.Collections.Generic;

namespace Screw.Configs
{
    public class LevelScrewBlock
    {
        public ScrewBlocker blockType;
        public bool isOpen;
        public int stageCount;
        public int keyId;
        
        public List<int> connetIds = new List<int>();
        public List<int> tieIds = new List<int>();
    }
}