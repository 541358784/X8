/************************************************
 * Config class : TablePsychologyExplain
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TablePsychologyExplain : TableBase
{   
    
    // 关卡ID
    public int id ;
    
    // 解释索引组
    public int[] explainIndex ;
    
    // 解释KEY
    public string explainKey ;
    


    public override int GetID()
    {
        return id;
    }
}
