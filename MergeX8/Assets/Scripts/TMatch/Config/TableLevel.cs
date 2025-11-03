/************************************************
 * Config class : Level
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class Level
    {   
        
        // #ID
        public int id ;
        
        // #关卡ID
        public int levelId ;
        
        // #对应USERGROUP下TM-MATCH里MAPPING; 分组GROUPID; 100=低难度（广告组）; 200=较低难度（默认组）; 300=中难度（低概率付费+广告付费）; 400=难（高概率付费和指定内购）; 500=困难（付费）
        public int levelUserGroup ;
        
        // #对应LAYOUT的ID
        public int layoutId ;
        
        // #触发降低难度需要的失败次数（大于等于）
        public int reduceGradeThreshold ;
        
        // #难度降低后使用的LAYOUT的ID
        public int easierLayoutId ;
        
    }
}
