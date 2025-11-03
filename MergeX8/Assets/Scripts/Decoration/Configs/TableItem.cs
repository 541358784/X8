/************************************************
 * Config class : TableItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableItem : TableBase
    {   
        
        // 建筑（INT32）
        public int id ;
        
        // 名称
        public string comment ;
        
        // 建筑名称
        public string name ;
        
        // 建筑图标
        public string buildingIcon ;
        
        // 品质; 0.初级; 1.高级
        public int qualityLevel ;
        
        // 货币ID; 
        public int openResource ;
        
        // 价格
        public int price ;
        
        // 数值用于筛选
        public string BuildingInArea ;
        
        // 播放展示动画的时间间隔; 默认0.08
        public float animShowStepTime ;
        
        // 播放展示动画类型; 0 先播放SPINE 结束后播放ANIMATOR; 1 先播放ANIMATOR 结束后播放SPINE; 2 同时播放
        public int animShowType ;
        
        // 播放清扫动画的时间间隔; 默认0.08
        public float animCleanStepTime ;
        
        // 播放展示动画 开启的OBJ对象名字
        public string activeObjName ;
        
        // 获取途径; 0.普通建筑; 1.活动获得
        public int source ;
        
        // 来源参数; 1.活动:活动名字; TYPE_ACTIVITY_EASTER 复活节
        public string sourceParam ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
