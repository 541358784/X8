/************************************************
 * Config class : StarrySkyCompassGlobalConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class StarrySkyCompassGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 开心时间(分)
    public int HappyTime { get; set; }// 开心累计分
    public int HappyMaxCount { get; set; }// 开心反向保底
    public int HappyFinalLeastTimes { get; set; }// 新手免费火箭个数
    public int InitRocketCount { get; set; }// 新手引导第一次奖励
    public int GuideResult { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
