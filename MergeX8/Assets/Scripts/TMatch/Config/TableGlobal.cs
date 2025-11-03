/************************************************
 * Config class : Global
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class Global
    {   
        
        // 名字长度限制
        public int NameLength ;
        
        // 关卡结算星星参数-比例
        public int[] MatchLevelWinStartRate ;
        
        // 关卡结算星星参数-数量
        public int[] MatchLevelWinStartCnt ;
        
        // 关卡警告时间
        public int MatchLevelWarningTime ;
        
        // 闪电解锁等级
        public int MatchLevelBoosterLightningUnlock ;
        
        // 时钟解锁等级
        public int MatchLevelBoosterClockUnlock ;
        
        // 连赢BUFF解锁等级
        public int MatchLevelGoldenHatterUnlock ;
        
        // 磁铁解锁等级
        public int MatchLevelBoosterUnlock1 ;
        
        // 扫帚解锁等级
        public int MatchLevelBoosterUnlock2 ;
        
        // 吹风解锁等级
        public int MatchLevelBoosterUnlock3 ;
        
        // 冰冻解锁等级
        public int MatchLevelBoosterUnlock4 ;
        
        // 程序生成关卡中触发降低难度需要的失败次数（大于等于）
        public int MatchLeveReduceGradeThreshold ;
        
        // 磁铁、扫帚、吹风、冰冻、闪电、时钟单次购买的数量限制
        public int BoosterBuyLimit ;
        
        // 星星宝箱解锁
        public int StarChestUnlcok ;
        
        // 周挑战解锁
        public int WeeklyChallengeUnlcok ;
        
        // 等级宝箱解锁
        public int LevelChestUnlock ;
        
        // 礼包链
        public int GiftBagLineUnlock ;
        
    }
}
