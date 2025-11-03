/************************************************
 * Config class : TableOrderFilter
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderFilter : TableBase
{   
    
    // ID
    public int id ;
    
    // 玩家最小等级
    public int playerMin ;
    
    // 玩家最大等级
    public int playerMax ;
    
    // 物品1最小难度
    public int firstDiffMin ;
    
    // 物品1最大难度
    public int firstDiffMax ;
    
    // 物品1额外过滤合成链
    public int[] firstFilterDiffMergeLine ;
    
    // 物品2最大难度
    public int secondDiffMax ;
    
    // 物品3最大难度
    public int thirdDiffMax ;
    
    // 过滤合成链
    public int[] filterDiffMergeLine ;
    
    // 物品1最小等级
    public int firstLevelMin ;
    
    // 物品1最大等级
    public int firstLevelMax ;
    
    // 物品2最大等级
    public int secondLevelMax ;
    
    // 物品3最大等级
    public int thirdLevelMax ;
    
    // 过滤合成链
    public int[] filterLevelMergeLine ;
    


    public override int GetID()
    {
        return id;
    }
}
