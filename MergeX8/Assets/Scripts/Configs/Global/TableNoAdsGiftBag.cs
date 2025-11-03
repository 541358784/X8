/************************************************
 * Config class : TableNoAdsGiftBag
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNoAdsGiftBag : TableBase
{   
    
    // ID
    public int id ;
    
    // 道具ID
    public int[] itemId ;
    
    // 道具数量
    public int[] ItemNum ;
    
    // SHOPID
    public int shopId ;
    
    // 打折前SHOPID
    public int fakeShopId ;
    
    // 礼包存活时间(分钟) 
    public int activeTime ;
    
    // 标签字
    public string tagText ;
    
    // 分层组
    public int payLevelGroup ;
    


    public override int GetID()
    {
        return id;
    }
}
