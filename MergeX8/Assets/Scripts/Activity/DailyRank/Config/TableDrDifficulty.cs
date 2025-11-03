/************************************************
 * Config class : TableDrDifficulty
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableDrDifficulty : TableBase
{   
    
    // ID
    public int id ;
    
    // 基础值系数
    public int defaultValue ;
    
    // 连续失败
    public int loseCount ;
    
    // 档位变化
    public int loseChangeValue ;
    
    // 连续成功
    public int winCount ;
    
    // 档位变化
    public int winChangeValue ;
    


    public override int GetID()
    {
        return id;
    }
}
