/************************************************
 * Config class : ThreeGiftLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.ThreeGift
{
    [System.Serializable]
    public class ThreeGiftLevelConfig
    {   
        
        // ID
        public int id ;
        
        // 礼包列表
        public int[] packageList ;
        
        // SHOPID
        public int shopId ;
        
        // 折扣
        public int discount ;
        
    }
}
