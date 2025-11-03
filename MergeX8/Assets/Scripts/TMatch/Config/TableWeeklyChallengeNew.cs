/************************************************
 * Config class : WeeklyChallengeNew
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class WeeklyChallengeNew
    {   
        
        // ID
        public int id ;
        
        // 主题名称
        public string weekName ;
        
        // 主题图片
        public string weekPicture ;
        
        // 奖励组
        public int rewardGroup ;
        
        // 收集的ITEMID
        public int collectItemId ;
        
        // 普通：每关掉落ITEM的数量（等概率）
        public int[] collectItemRandomCnt ;
        
        // BUFF加成：每关掉落ITEM的数量（等概率）
        public int[] collectItemRandomCntBuff ;
        
    }
}
