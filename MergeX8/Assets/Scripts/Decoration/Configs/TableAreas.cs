/************************************************
 * Config class : TableAreas
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableAreas : TableBase
    {   
        
        // 区域ID
        public int id ;
        
        // 显示区域; 1. 情人节 大船公用区域 只能同时显示一个
        public int showArea ;
        
        // 装修列表中隐藏
        public bool hideAreaInDeco ;
        
        // 是否默认显示该区域
        public bool isDefaultShow ;
        
        // 阴影颜色
        public float darkColor ;
        
        // 阶段ID
        public int[] stages ;
        
        // 下一个区域ID
        public int nextAreaId ;
        
        // 完成时剧情
        public int storyId ;
        
        // 是否默认解锁（0不是 1是）
        public int isDefaultUnlock ;
        
        // 区域名称
        public string areaName ;
        
        // 区域未解锁描述
        public string areaUnlockDesc ;
        
        // 区域描述
        public string areaDesc ;
        
        // 区域ICON
        public string icon ;
        
        // 区域ICON
        public string bigIcon ;
        
        // 奖励ID
        public int[] rewardId ;
        
        // 奖励个数
        public int[] rewardNum ;
        
        // 缩放系数，范围[0.6,1.6]
        public float previewScale ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
