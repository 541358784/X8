/************************************************
 * Config class : TableRoleColor
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableRoleColor : TableBase
{   
    
    // ID
    public int id ;
    
    //  头像底板
    public string headBg ;
    
    // 名字底板
    public string nameBg ;
    
    // 内容底板
    public string contentBg ;
    
    // 名字底板2
    public string nameBg2 ;
    
    // 内容底板2
    public string contentBg2 ;
    
    // 名字字体材质
    public string nameMaterial ;
    
    // 内容字体材质
    public string contentMaterial ;
    


    public override int GetID()
    {
        return id;
    }
}
