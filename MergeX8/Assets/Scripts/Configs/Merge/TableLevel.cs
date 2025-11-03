/************************************************
 * Config class : TableLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableLevel : TableBase
{   
    
    // ID
    public int id ;
    
    // 等级
    public int lv ;
    
    // 升级需要经验
    public int xp ;
    
    // 奖励物品
    public int[] planb_reward ;
    
    // 奖励物品
    public int[] reward ;
    
    // 物品数量
    public int[] amount ;
    
    // 是否有升级礼包
    public bool levelUpPackage ;
    


    public override int GetID()
    {
        return id;
    }
}
