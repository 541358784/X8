/************************************************
 * Config class : OutRate
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.WinStreak
{
    [System.Serializable]
    public class OutRate
    {   
        
        // #ID 
        public int id ;
        
        // 当前完成关卡最大值
        public int levelRange ;
        
        // 机器人淘汰概率范围 %
        public int[] outRateMin ;
        
        // 机器人淘汰概率范围 %
        public int[] outRateMax ;
        
        // 使用道具数量达到多少次提高淘汰率
        public int[] useBoost ;
        
        // 达成条件时提升的淘汰率 % 增加
        public int upRate ;
        
    }
}
