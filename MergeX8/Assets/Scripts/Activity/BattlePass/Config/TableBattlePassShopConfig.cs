/************************************************
 * Config class : TableBattlePassShopConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Activity.BattlePass
{
    [System.Serializable]
    public class TableBattlePassShopConfig : TableBase
    {   
        
        // ID
        public int id ;
        
        // 支付组=
        public int payLevelGroup ;
        
        // 老玩家SHOPID组
        public int[] oldUserShopIds ;
        
        // 老玩家1奖励类型
        public int[] oldRewardType_1 ;
        
        // 老玩家1奖励数量
        public int[] oldRewardNum_1 ;
        
        // 老玩家2奖励类型
        public int[] oldRewardType_2 ;
        
        // 老玩家2奖励数量
        public int[] oldRewardNum_2 ;
        
        // 老玩家3奖励类型
        public int[] oldRewardType_3 ;
        
        // 老玩家3奖励数量
        public int[] oldRewardNum_3 ;
        
        // 新玩家SHOPID组
        public int[] newUserShopIds ;
        
        // 老玩家1奖励类型
        public int[] newRewardType_1 ;
        
        // 老玩家2奖励数量
        public int[] newRewardNum_1 ;
        
        // 老玩家2奖励类型
        public int[] newRewardType_2 ;
        
        // 老玩家2奖励数量
        public int[] newRewardNum_2 ;
        
        // 升级券购买的ID; 详见SHOP表
        public int ultimateShopId ;
        
        // 前进格子
        public int ultimateSkipStep ;
        
        // 最终积分倍数
        public int ultimateScoreMultiple ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
