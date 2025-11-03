/************************************************
 * Config class : LayoutDesign
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class LayoutDesign
    {   
        
        // 关卡等级下限
        public int levelMin ;
        
        // 关卡等级上限
        public int levelMax ;
        
        // 特殊关概率（百分率）
        public int specialRatio ;
        
        // 难度计算：N值
        public int[] difficultyValue ;
        
        // 目标数量下限
        public int targetCntMin ;
        
        // 目标数量上限
        public int targetCntMax ;
        
        // 目标种类-普通
        public int[] targetTypeNormal ;
        
        // 目标种类-困难
        public int[] targetTypeHard ;
        
        // 目标种类-超级困难
        public int[] targetTypeDemon ;
        
        // 非目标种类-普通
        public int[] untargetTypeNormal ;
        
        // 非目标种类-困难
        public int[] untargetTypeHard ;
        
        // 非目标种类-超级困难
        public int[] untargetTypeDemon ;
        
        // 非目标数量-普通
        public int[] untargetCntNormal ;
        
        // 非目标数量-困难
        public int[] untargetCntHard ;
        
        // 非目标数量-超级困难
        public int[] untargetCntDemon ;
        
        // 非目标数量-普通-降纬系数
        public float untargetNormalLower ;
        
        // 非目标数量-困难-降纬系数
        public float untargetHardLower ;
        
        // 非目标数量-超级困难-降纬系数
        public float untargetDemonLower ;
        
        // 关卡时间-普通
        public float timeNormal ;
        
        // 关卡时间-困难
        public float timeHard ;
        
        // 关卡时间-超级困难
        public float timeDemon ;
        
    }
}
