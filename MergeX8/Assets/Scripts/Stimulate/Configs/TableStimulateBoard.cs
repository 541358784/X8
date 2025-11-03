/************************************************
 * Config class : TableStimulateBoard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStimulateBoard : TableBase
{   
    
    // ID
    public int id ;
    
    // 棋盘ID
    public int boardId ;
    
    // MERGEID 
    public int itemId ;
    


    public override int GetID()
    {
        return id;
    }
}
