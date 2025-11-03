/************************************************
 * Config class : TableStimulateMerge
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStimulateMerge : TableBase
{   
    
    // ID
    public int id ;
    
    // 目标ID
    public int targeMergeId ;
    
    // 棋盘ID
    public int boardId ;
    


    public override int GetID()
    {
        return id;
    }
}
