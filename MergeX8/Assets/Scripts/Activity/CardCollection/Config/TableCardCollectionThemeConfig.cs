/************************************************
 * Config class : CardCollectionThemeConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class CardCollectionThemeConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 当期卡册主题ID
    public int ThemeId { get; set; }// 权重1持续时间(H)
    public int WeightTime1 { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
