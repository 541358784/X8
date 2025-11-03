/************************************************
 * Config class : FishEatFishInnerTwoLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.FishEatFishInnerTwo
{
    [System.Serializable]
    public class FishEatFishInnerTwoLevel
    {   
        
        // 关卡ID
        public int id ;
        
        // 玩家初始数值
        public int hp ;
        
        // 关中敌人列表
        public int[] enemyList ;
        
        // 初始敌人数量
        public int enemyCount ;
        
    }
}
