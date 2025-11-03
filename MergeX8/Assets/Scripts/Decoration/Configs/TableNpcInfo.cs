/************************************************
 * Config class : TableNpcInfo
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableNpcInfo : TableBase
    {   
        
        // ID
        public int id ;
        
        // 是否显示
        public bool isShow ;
        
        // 位置
        public float[] position ;
        
        // 旋转
        public float[] rotation ;
        
        // 动画名字
        public string animName ;
        
        // 装修时位置
        public float[] decoPosition ;
        
        // 装修时旋转
        public float[] decoRotation ;
        
        // 装修动画
        public string decoAnimName ;
        
        // 动作播放几次
        public int playAnimNum ;
        
        // 等待主角动作时间
        public float waitHeroTime ;
        
        // 装修完成时位置
        public float[] finishPosition ;
        
        // 装修完成时旋转
        public float[] finishRotation ;
        
        // 装修完成播放的动画名字
        public string finishAnimName ;
        
        // 依赖路径
        public string linkPath ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
