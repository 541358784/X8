/************************************************
 * Config class : SlotMachineRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class SlotMachineRewardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 中奖所需的图标
    public List<int> ResultList { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 至少重转次数
    public int LeastReSpinTimes { get; set; }// 保底次数
    public int MaxReSpinTimes { get; set; }// 是否显示规则
    public bool ShowRuleFlag { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
