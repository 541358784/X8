/************************************************
 * Config class : Rewards
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMBP
{
    [System.Serializable]
    public class Rewards
    {   
        
        // 编号
        public int id ;
        
        // 物品ID
        public int[] itemIds ;
        
        // 物品数量
        public int[] itemCounts ;
        
        // 奖励类型（1：道具 2:宝箱）
        public int awardType ;
        
    }
}
