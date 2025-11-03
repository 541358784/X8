/************************************************
 * Config class : TableWorlds
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableWorlds : TableBase
    {   
        
        // 世界ID
        public int id ;
        
        // 区域ID
        public int[] areaIds ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
