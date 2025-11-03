/************************************************
 * Config class : TableExtraOrderRewardCouponConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableExtraOrderRewardCouponConfig : TableBase
{   
    
    // 对应的ITEMID
    public int id ;
    
    // 翻倍的类型; 1:香蕉; 2:金币; 3:狗粮; 4:转盘; 5:主题装修
    public int[] multiType ;
    
    // 翻倍的倍数
    public float[] multiValue ;
    
    // 持续时间(秒)
    public int time ;
    
    // 是否可以持有
    public bool canKeep ;
    
    // 每日刷新时间(小时)
    public int eachDayRefreshTime ;
    
    // 描述翻译表
    public string describeText ;
    


    public override int GetID()
    {
        return id;
    }
}
