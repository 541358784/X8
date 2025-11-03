/************************************************
 * Config class : TableBattlePassReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Activity.BattlePass
{
    [System.Serializable]
    public class TableBattlePassReward : TableBase
    {   
        
        // ID
        public int id ;
        
        // 支付组=
        public int payLevelGroup ;
        
        // 普通奖励ID
        public int[] normalRewardId ;
        
        // 普通奖励数量
        public int[] normalRewardNum ;
        
        // 金钥匙奖励ID
        public int[] keyRewardId ;
        
        // 金钥匙奖励数量
        public int[] keyRewardNum ;
        
        // 兑换消耗数量
        public int exchangeItemNum ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
