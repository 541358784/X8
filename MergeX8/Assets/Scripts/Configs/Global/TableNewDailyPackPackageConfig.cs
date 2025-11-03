/************************************************
 * Config class : TableNewDailyPackPackageConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNewDailyPackPackageConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // SHOPID
    public int shopId ;
    
    // 包含内容
    public int[] contain ;
    
    // 包含内容数量
    public int[] containCount ;
    
    // 权益系数(仅显示)
    public string labelNum ;
    
    // 礼包标题
    public string name ;
    
    // 0-普通; 1-打折促销; 2-最受欢迎
    public int best_deal ;
    


    public override int GetID()
    {
        return id;
    }
}
