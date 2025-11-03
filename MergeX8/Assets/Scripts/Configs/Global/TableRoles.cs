/************************************************
 * Config class : TableRoles
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableRoles : TableBase
{   
    
    // ID
    public int id ;
    
    // 角色名
    public string roleName ;
    
    // 图片名
    public string rolePicName ;
    
    // 
    public string rolePrefabName ;
    
    // 
    public string roleAniName ;
    
    // 角色颜色
    public int roleColor ;
    


    public override int GetID()
    {
        return id;
    }
}
