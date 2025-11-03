/************************************************
 * Config class : TableBattlePassConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Activity.BattlePass
{
    [System.Serializable]
    public class TableBattlePassConfig : TableBase
    {   
        
        // 编号
        public int id ;
        
        // 活动币ID
        public int eventCoinId ;
        
        // 升级券 多久后弹出 分钟
        public int ultimateShowHour ;
        
        // 循环宝箱分数
        public int loopRewardScore ;
        
        // 循环宝箱ID
        public int[] loopRewardList ;
        
        // 购买金券的ID; 详见SHOP表
        public int shopItemId ;
        
        // 购买彩券的ID; 详见SHOP表
        public int shopGoldId ;
        
        // 彩券积分
        public int goldGetScore ;
        
        // 积分倍数
        public int goldScoreMultiple ;
        
        // 购买BP附赠物品
        public int[] buyReward ;
        
        // 延期时长（天）
        public int extraDays ;
        
        // 延期商品ID
        public int extraDaysShopId ;
        
        // 延期展示时长
        public int extraShowDays ;
        
        // 产出经验概率小于4级
        public int mergeRateLittle ;
        
        // 产出经验概率大于4级
        public int mergeRateBig ;
        
        // 产出经验保底小于4级
        public int mergeRateLittleLimit ;
        
        // 产出经验保底大于4级
        public int mergeRateBigLimit ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
