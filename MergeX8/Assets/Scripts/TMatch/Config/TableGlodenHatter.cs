/************************************************
 * Config class : GlodenHatter
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class GlodenHatter
    {   
        
        // #ID
        public int id ;
        
        // #需要的连赢累计次数
        public int times ;
        
        // #奖励ID（对应TM-BOOST表中的ID）
        public int[] rewardID ;
        
        // #奖励数量
        public int[] rewardCnt ;
        
    }
}
