/************************************************
 * Config class : HGVDConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.HappyGo
{
    [System.Serializable]
    public class HGVDConfig
    {   
        
        // 编号
        public int id ;
        
        // 预热时间
        public int preheatTime ;
        
        // 延期购买等待时间
        public int extendBuyWaitTime ;
        
        // 延期购买时间
        public int extendBuyTime ;
        
        // 延期购买商品ID
        public int extendBuyProductId ;
        
        // 闪购刷新时间(秒)
        public int flashShopFreshTime ;
        
        // 闪购刷新钻石消耗
        public int flashShopFreshCost ;
        
    }
}
