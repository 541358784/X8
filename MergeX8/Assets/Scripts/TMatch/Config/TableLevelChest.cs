/************************************************
 * Config class : LevelChest
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class LevelChest
    {   
        
        // #距离下阶段领奖的等级增量，该等级会以配置的最后一行数据一直循环往下递增，奖励也以最后一行为准
        public int level ;
        
        // #随机奖励ID
        public int[] randomRewardID1 ;
        
        // #随机奖励数量
        public int[] randomRewardCnt1 ;
        
        // #随机概率(概率和为100，则必定随机一个奖励处理，小于100，则不一定能随机到奖励)
        public int[] randomRewardPro1 ;
        
        // #随机奖励ID
        public int[] randomRewardID2 ;
        
        // #随机奖励数量
        public int[] randomRewardCnt2 ;
        
        // #随机概率
        public int[] randomRewardPro2 ;
        
        // #随机奖励ID
        public int[] randomRewardID3 ;
        
        // #随机奖励数量
        public int[] randomRewardCnt3 ;
        
        // #随机概率
        public int[] randomRewardPro3 ;
        
    }
}
