/************************************************
 * Config class : TableLevelUpPackageContentConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableLevelUpPackageContentConfig : TableBase
{   
    
    // 升级礼包ID
    public int id ;
    
    // 持续时间(分钟)
    public int activeTime ;
    
    // SHOPID
    public int shopId ;
    
    // 礼包名翻译表
    public string packageName ;
    
    // 奖励ID
    public int[] rewardId ;
    
    // 奖励数量
    public int[] rewardNum ;
    
    // 购买次数
    public int buyTimes ;
    
    // 标签字
    public string labelText ;
    
    // 价格倍数
    public float originalPriceMultiple ;
    


    public override int GetID()
    {
        return id;
    }
}
