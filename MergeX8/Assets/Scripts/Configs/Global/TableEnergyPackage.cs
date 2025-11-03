/************************************************
 * Config class : TableEnergyPackage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableEnergyPackage : TableBase
{   
    
    // ID
    public int id ;
    
    // SHOPID
    public int shopId ;
    
    // 礼包展示时间 
    public int packageCD ;
    
    // 日短CD【分钟】
    public int shortCd ;
    
    // 日弹出次数
    public int dayPop ;
    
    // 跨天长CD【分钟】
    public int longCd ;
    
    // 付费后CD【分钟】
    public int payCd ;
    
    // 道具ID
    public int[] itemId ;
    
    // 道具数量
    public int[] ItemNum ;
    
    // 折扣标记
    public string discount ;
    
    // 礼包名称
    public string name ;
    


    public override int GetID()
    {
        return id;
    }
}
