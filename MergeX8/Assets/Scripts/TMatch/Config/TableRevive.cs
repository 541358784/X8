/************************************************
 * Config class : Revive
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class Revive
    {   
        
        // #ID
        public int id ;
        
        // #1：超时，2：格子满
        public int type ;
        
        // #单局.累计购买复活次数
        public int times ;
        
        // #复活花费（COIN）
        public int reviveCost ;
        
        // #奖励ID（对应TM-GAME表中的ID）
        public int[] rewardID ;
        
        // #奖励数量
        public int[] rewardCnt ;
        
        // #广告复活奖励ID（对应TM-GAME表中的ID）
        public int[] adRewardID ;
        
        // #广告复活奖励数量
        public int[] adRewardCnt ;
        
    }
}
