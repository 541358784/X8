/************************************************
 * Config class : TableBundle
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBundle : TableBase
{   
    
    // #
    public int id ;
    
    // 购买的ID; 详见SHOP表
    public int shopItemId ;
    
    // 商品名称（多语言ID）
    public string name ;
    
    // 商品描述
    public string description ;
    
    // 显示的图标
    public string icon ;
    
    // 商品ICON背景图片
    public string image ;
    
    // 商品ID列表; 参考ITMS表
    public int[] bundleItemList ;
    
    // 商品对应的数量列表
    public int[] bundleItemCountList ;
    
    // 物品类型; 1=ITEMS; 2=MERGEITEMS
    public int[] bundleItemType ;
    
    // 终身购买次数
    public int LifeNum ;
    
    // 有效时间(小时)
    public int ValidTime ;
    
    // 下一个礼包的ID
    public int nextID ;
    
    // 间隔时间(小时)
    public int cdTime ;
    
    // 解锁需要的挂点数
    public int UnlockLevel ;
    


    public override int GetID()
    {
        return id;
    }
}
