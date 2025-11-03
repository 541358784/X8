/************************************************
 * Config class : LoadingTransition
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class LoadingTransition
    {   
        
        // ID
        public int id ;
        
        // 类型; 0:普通和WORLD1; 1:春日活动; 2:夏日活动; 3.WORLD2
        public int type ;
        
        // 图片名
        public string outFrame ;
        
        // 图片名
        public string innerFrame ;
        
        // 主色（5个圈）
        public string mainColor ;
        
        // 背景色(整个背景)
        public string bgColor ;
        
        // 名字背景色
        public string labelBgColor ;
        
        // 名字字体颜色
        public string textColor ;
        
    }
}
