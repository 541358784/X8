/************************************************
 * Config class : HGVDLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.HappyGo
{
    [System.Serializable]
    public class HGVDLevel
    {   
        
        // ID
        public int id ;
        
        // 等级
        public int lv ;
        
        // 升级需要经验; （累计总数）
        public int xp ;
        
        // 奖励物品
        public int[] reward ;
        
        // 物品数量
        public int[] amount ;
        
        // 是否补发
        public bool reissue ;
        
        // 标签多语言
        public string label ;
        
    }
}
