/************************************************
 * Config class : TableItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableItem : TableBase
{   
    
    // #
    public int id ;
    
    // 
    public string name ;
    
    // 道具ID
    public int boostId ;
    
    // 图片
    public string image ;
    
    // 图片
    public string imageBig ;
    
    // 图片
    public string imageReward ;
    
    // 类型; 1:装修币, 2:金币,3:体力,4:无限体力,5:道具
    public int type ;
    
    // 图集位置
    public string atlasPath ;
    


    public override int GetID()
    {
        return id;
    }
}
