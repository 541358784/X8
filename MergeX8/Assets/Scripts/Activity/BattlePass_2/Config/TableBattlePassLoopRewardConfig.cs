/************************************************
 * Config class : TableBattlePassLoopRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Activity.BattlePass_2
{
    [System.Serializable]
    public class TableBattlePassLoopRewardConfig : TableBase
    {   
        
        // ID
        public int id ;
        
        // 奖励ID
        public int[] rewardId ;
        
        // 奖励数量
        public int[] rewardNum ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
