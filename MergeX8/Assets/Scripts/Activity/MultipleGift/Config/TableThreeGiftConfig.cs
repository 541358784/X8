/************************************************
 * Config class : ThreeGiftConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.MultipleGift
{
    [System.Serializable]
    public class ThreeGiftConfig
    {   
        
        // ID
        public int id ;
        
        // SHOPID
        public int shopId ;
        
        // 包含内容
        public int[] contain ;
        
        // 包含内容数量
        public int[] containCount ;
        
    }
}
