/************************************************
 * Config class : TableBoost
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBoost : TableBase
{   
    
    // 道具ID
    public int id ;
    
    // 道具名字
    public string name ;
    
    // 道具持续时间(单位秒数，-1表示没有时间限制）
    public int duration ;
    
    // 解锁关卡
    public int unlockLevel ;
    
    // 初始数量
    public int initAmount ;
    
    // 金币价格
    public int price ;
    
    // 一次购买道具数量
    public int buyCount ;
    
    // 道具图片
    public string image ;
    
    // 道具描述
    public string desc ;
    


    public override int GetID()
    {
        return id;
    }
}
