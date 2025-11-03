/************************************************
 * Config class : WeeklyChallengeReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class WeeklyChallengeReward
    {   
        
        // 奖励组
        public int RewardGroup ;
        
        // #ID
        public int id ;
        
        // 需要收集的数量
        public int collectNum ;
        
        // 奖励物品ID
        public int rewardId ;
        
        // #奖励数量
        public int rewardCnt ;
        
        // 是否大奖展示; 0：不是，1：是
        public int rewardHugeShow ;
        
    }
}
