/************************************************
 * Config class : TablePsychologyLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TablePsychologyLevel : TableBase
{   
    
    // 关卡ID
    public int id ;
    
    // 关卡内图片
    public string imageName ;
    
    // 按钮上的文字 多语言KEY
    public string[] buttonNameKeys ;
    


    public override int GetID()
    {
        return id;
    }
}
