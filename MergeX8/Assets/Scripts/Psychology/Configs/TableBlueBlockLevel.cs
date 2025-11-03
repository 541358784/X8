/************************************************
 * Config class : TableBlueBlockLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBlueBlockLevel : TableBase
{   
    
    // 关卡ID
    public int id ;
    
    // 背景形状
    public int map ;
    
    // 填充形状
    public int[] shapes ;
    
    // 填充形状名字 多语言KEY
    public string[] shapeNames ;
    


    public override int GetID()
    {
        return id;
    }
}
