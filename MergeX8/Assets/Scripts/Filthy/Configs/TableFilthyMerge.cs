using System;
using System.Collections.Generic;

namespace DragonPlus.Config.Filthy
{
    [System.Serializable]
    public class FilthyMerge : TableBase
    {   
        // ID
        public int Id { get; set; }// 目标ID
        public int TargeMergeId { get; set; }// 棋盘ID
        public int BoardId { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
