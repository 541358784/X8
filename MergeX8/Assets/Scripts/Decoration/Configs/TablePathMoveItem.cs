/************************************************
 * Config class : TablePathMoveItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TablePathMoveItem : TableBase
    {   
        
        // 世界ID
        public int id ;
        
        // 路径
        public string[] movePath ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
