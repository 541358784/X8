/************************************************
 * Config class : ShopConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class ShopConfig
    {   
        
        // ID(按照表顺序显示)
        public int id ;
        
        // # 商品类型; 1.金币（COIN）; 2.礼包（PACK）; 3.去广告; 4复活; 5破冰
        public int shopType ;
        
        // # 物品ID（ITEM表）
        public int[] itemId ;
        
        // # 物品数量
        public int[] itemCnt ;
        
        // # 背景板类型; 0.忽略; 1.黄色; 2.橘色; 
        public int bgType ;
        
    }
}
