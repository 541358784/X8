/************************************************
 * Config class : TurntableSetingConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TurntableSetingConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 押注值
    public List<int> BetValue { get; set; }// 转盘布局
    public List<int> TurntableResultList { get; set; }// 加速时间
    public float AddSpinSpeedTime { get; set; }// 最大转速(角度/S)
    public float MaxSpinSpeed { get; set; }// 最大转速持续时间
    public float KeepMaxSpinSpeedTime { get; set; }// 回弹角度
    public float BounceBackRotation { get; set; }// 回弹时间
    public float BounceBackTime { get; set; }// 减速时间
    public float ReduceSpinSpeedTime { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
