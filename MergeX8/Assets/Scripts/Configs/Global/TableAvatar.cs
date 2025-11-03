/************************************************
 * Config class : TableAvatar
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableAvatar : TableBase
{   
    
    // ID
    public int id ;
    
    // 头像路径名字
    public string headIconName ;
    
    // 头像图集
    public string headIconAtlas ;
    
    // 默认头像框
    public int defaultHeadIconFrameId ;
    
    // 是否需要收集
    public bool isNeedCollect ;
    
    // 是否为预制体
    public bool isPrefab ;
    


    public override int GetID()
    {
        return id;
    }
}
