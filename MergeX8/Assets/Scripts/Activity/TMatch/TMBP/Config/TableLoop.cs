/************************************************
 * Config class : Loop
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMBP
{
    [System.Serializable]
    public class Loop
    {   
        
        // 编号
        public int id ;
        
        // 经验
        public int exp ;
        
        // 奖励
        public int[] reward ;
        
        // 奖励权重
        public int[] rewardValue ;
        
    }
}
