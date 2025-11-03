/************************************************
 * Config class : TableOrderRandom2
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderRandom2 : TableBase
{   
    
    // ID 和 TYPE 随机任务ID
    public int id ;
    
    // 支付等级 =
    public int payLevelGroup ;
    
    // 解锁等级
    public int unlockLevel ;
    
    // 选中
    public int[] filters ;
    
    // 刷新时间
    public int refreshDiffTime ;
    
    // 出现几次
    public int diffPoint ;
    
    // 刷新时间
    public int refreshLevelTime ;
    
    // 出现几次
    public int levelPoint ;
    


    public override int GetID()
    {
        return id;
    }
}
