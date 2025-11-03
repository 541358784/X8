/************************************************
 * Config class : TurtlePangStoreLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TurtlePangStoreLevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 商店购买项列表
    public List<int> StoreItemList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
