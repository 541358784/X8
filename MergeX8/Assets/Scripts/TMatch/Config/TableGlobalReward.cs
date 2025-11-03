/************************************************
 * Config class : GlobalReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class GlobalReward
    {   
        
        // 奖励类型
        public string rewardType ;
        
        // #奖励ID（对应ITEM表中的ID）
        public int[] rewardID ;
        
        // #奖励数量
        public int[] rewardCnt ;
        
    }
}
