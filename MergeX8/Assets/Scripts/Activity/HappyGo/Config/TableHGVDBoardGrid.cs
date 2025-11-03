/************************************************
 * Config class : HGVDBoardGrid
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.HappyGo
{
    [System.Serializable]
    public class HGVDBoardGrid
    {   
        
        // ID
        public int id ;
        
        // 
        public int itemId ;
        
        // -1-未开启，0-锁定，1-解锁
        public int state ;
        
        // 0-开启后为蛛网，1-开启后不是蛛网
        public int unlockstate ;
        
    }
}
