using System.Collections.Generic;

namespace Screw.Configs
{
    public class LevelLayer
    {
        public List<LevelPanel> panels = new List<LevelPanel>();
        public List<LevelScrew> screws = new List<LevelScrew>();
        public List<LevelShield> shields = new List<LevelShield>();
    }
}