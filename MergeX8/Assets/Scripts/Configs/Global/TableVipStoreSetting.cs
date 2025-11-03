/************************************************
 * Config class : TableVipStoreSetting
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableVipStoreSetting : TableBase
{   
    
    // ID
    public int id ;
    
    // VIP 充值总额
    public int vipPrice ;
    
    // VIP 保级周期内需要充值的金额
    public int vipCyclePrice ;
    
    // VIP 商店物品刷新时间 秒
    public int vipRefreshTime ;
    
    // VIP 降级周期时间 秒
    public int vipCycleTime ;
    


    public override int GetID()
    {
        return id;
    }
}
