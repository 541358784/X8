/************************************************
 * Config class : StarrySkyCompassResultConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class StarrySkyCompassResultConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 分数
    public int Score { get; set; }// 特殊玩法积累值
    public int HappyValue { get; set; }// 权重
    public int Weight { get; set; }// 特殊玩法分数
    public int HappyScore { get; set; }// 特殊玩法权重
    public int HappyWeight { get; set; }// 等级
    public int Level { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
