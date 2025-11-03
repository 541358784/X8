/************************************************
 * Config class : FishEatFishInnerTwoEnemy
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.FishEatFishInnerTwo
{
    [System.Serializable]
    public class FishEatFishInnerTwoEnemy
    {   
        
        // 关内敌人ID
        public int id ;
        
        // 敌人类型; 1:普通敌人; 2:乘倍气泡
        public int type ;
        
        // 敌人尺寸
        public float size ;
        
        // 敌人数值
        public int hp ;
        
        // 乘倍气泡倍数
        public float multiply ;
        
        // 使用的资源路径
        public string path ;
        
    }
}
