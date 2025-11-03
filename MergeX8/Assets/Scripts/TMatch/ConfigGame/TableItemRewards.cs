/************************************************
 * Config class : ItemRewards
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class ItemRewards
    {   
        
        // 编号
        public int id ;
        
        // 备注
        public string comment ;
        
        // 包
        public int package ;
        
        // 物品ID
        public int[] itemIds ;
        
        // 物品数量
        public int[] itemNums ;
        
        // 权重
        public int value ;
        
    }
}
