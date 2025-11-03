using System;
using System.Collections.Generic;

namespace DragonPlus.Config.MiniGame
{
    [System.Serializable]
    public class MiniGameSelection : TableBase
    {   
        // ID
        public int Id { get; set; }// 选择结果
        public List<int> SelectResult { get; set; }// ICON
        public List<string> SelectIcons { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
