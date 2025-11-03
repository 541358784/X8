/************************************************
 * Config class : TableSound
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableSound : TableBase
{   
    
    // ID
    public int id ;
    
    // 命名
    public string sound_name ;
    
    // 音效路径
    public string sound_path ;
    
    // 对应位置
    public string DESC ;
    


    public override int GetID()
    {
        return id;
    }
}
