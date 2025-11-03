/************************************************
 * Config class : TableMergeLine
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableMergeLine : TableBase
{   
    
    // ID
    public int id ;
    
    // 名称
    public string name ;
    
    // 类型
    public int type ;
    
    // 阶
    public int group ;
    
    // 自定义排序
    public bool customSort ;
    
    // 包含物品数量
    public int amount ;
    
    // 包含物品ID
    public int[] output ;
    
    // 来源链
    public int[] re_line ;
    
    // 预设产出队列
    public int[] presetDropQueue ;
    
    // 预设产出队列1
    public int[] presetDropQueue1 ;
    
    // 销售保护
    public bool Sales_Protect ;
    


    public override int GetID()
    {
        return id;
    }
}
