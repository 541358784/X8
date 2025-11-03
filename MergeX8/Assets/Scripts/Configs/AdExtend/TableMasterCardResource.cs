using System;
using System.Collections.Generic;

[System.Serializable]
public class MasterCardResource : TableBase
{   
    // #
    public int Id { get; set; }// 奖励类型; ; 1 去插屏; 2 充值钻石翻倍; 3 增加背包格数; 4 建筑CD减少; 5 体力上限增加; 6建筑产出增加; 101 金币; 102 钻石; 201 体力
    public int Type { get; set; }// 奖励参数
    public int RewardParam { get; set; }// 扩展参数
    public int ExtraParam { get; set; }// 
    public string Icon { get; set; }// 界面描述 多语言KEY
    public string Description { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
