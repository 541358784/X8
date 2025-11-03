/************************************************
 * Config class : TurtlePangItemConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TurtlePangItemConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 权重
    public int Weight { get; set; }// 图片名
    public string Image { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
