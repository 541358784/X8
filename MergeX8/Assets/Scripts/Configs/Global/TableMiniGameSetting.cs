/************************************************
 * Config class : TableMiniGameSetting
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableMiniGameSetting : TableBase
{   
    
    // ID
    public int id ;
    
    // 组
    public int group ;
    
    // 类型; 1:挖沟; 2:接水管; 3:心理学
    public int type ;
    
    // 普通位选中图片
    public string imageName ;
    
    // IDS
    public int[] childIds ;
    


    public override int GetID()
    {
        return id;
    }
}
