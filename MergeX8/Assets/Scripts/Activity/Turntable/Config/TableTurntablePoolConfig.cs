/************************************************
 * Config class : TurntablePoolConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TurntablePoolConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 结果池
    public List<int> ResultPool { get; set; }// 权重池
    public List<int> WeightPool { get; set; }// 数量池
    public List<int> NumPool { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
