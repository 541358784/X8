/************************************************
 * Config class : TableDrBotscore
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableDrBotscore : TableBase
{   
    
    // ID
    public int id ;
    
    // <80%_MIN
    public int le_Min_80 ;
    
    // <80%_MAX
    public int le_Max_80 ;
    
    // >100%_MIN
    public int gr_Min_100 ;
    
    // >100%_MAX
    public int gr_Max_100 ;
    
    // 0_MIN
    public int defaultMin ;
    
    // 0_MAX
    public int defaultMax ;
    


    public override int GetID()
    {
        return id;
    }
}
