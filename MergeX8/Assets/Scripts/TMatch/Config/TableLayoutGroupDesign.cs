/************************************************
 * Config class : LayoutGroupDesign
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class LayoutGroupDesign
    {   
        
        // #对应USERGROUP下TM-MATCH里MAPPING; 分组GROUPID; 100=低难度（广告组）; 200=较低难度（默认组）; 300=中难度（低概率付费+广告付费）; 400=难（高概率付费和指定内购）; 500=困难（付费）
        public int levelUserGroup ;
        
        // 非目标总量系数
        public float untargetCntValue ;
        
        // 混色系数：普通
        public float garbleNormalValue ;
        
        // 混色系数：困难
        public float garbleHardValue ;
        
        // 混色系数：超级困难
        public float garbleDemonValue ;
        
    }
}
