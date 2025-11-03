/************************************************
 * Config class : FishEatFishInnerEnemy
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.FishEatFishInner
{
    [System.Serializable]
    public class FishEatFishInnerEnemy
    {   
        
        // 关内敌人ID
        public int id ;
        
        // 敌人类型; 1:普通敌人; 2:乘倍气泡
        public int type ;
        
        // 所在列数
        public int line ;
        
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
