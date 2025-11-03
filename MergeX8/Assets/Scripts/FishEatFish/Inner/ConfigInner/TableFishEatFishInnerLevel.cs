/************************************************
 * Config class : FishEatFishInnerLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.FishEatFishInner
{
    [System.Serializable]
    public class FishEatFishInnerLevel
    {   
        
        // 关卡ID
        public int id ;
        
        // 玩家初始数值
        public int hp ;
        
        // 关底BOSSID
        public int bossId ;
        
        // 关中敌人列表
        public int[] enemyList ;
        
    }
}
