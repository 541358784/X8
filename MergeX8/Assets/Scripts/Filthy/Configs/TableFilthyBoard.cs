using System;
using System.Collections.Generic;

namespace DragonPlus.Config.Filthy
{
    [System.Serializable]
    public class FilthyBoard : TableBase
    {   
        // ID
        public int Id { get; set; }// 棋盘ID
        public int BoardId { get; set; }// MERGEID 
        public int ItemId { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
