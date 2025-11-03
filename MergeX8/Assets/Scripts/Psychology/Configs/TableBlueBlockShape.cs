/************************************************
 * Config class : TableBlueBlockShape
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBlueBlockShape : TableBase
{   
    
    // 形状ID
    public int id ;
    
    // 形状配置
    public int[] shape ;
    
    // 宽
    public int width ;
    
    // 高
    public int height ;
    


    public override int GetID()
    {
        return id;
    }
}
