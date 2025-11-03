/************************************************
 * Config class : TableAvatarFrame
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableAvatarFrame : TableBase
{   
    
    // ID
    public int id ;
    
    // 头像路径名字
    public string headIconFrameName ;
    
    // 头像图集
    public string headIconFrameAtlas ;
    
    // 头像类型(按照优先级排序，0:头像默认携带，1:单独获得的头像框)
    public int headIconFrameType ;
    
    // 是否为预制体
    public bool isPrefab ;
    


    public override int GetID()
    {
        return id;
    }
}
