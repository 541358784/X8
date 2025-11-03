using System;
using System.Collections.Generic;

[System.Serializable]
public class ThemeDecorationStoreLevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 商店购买项列表
    public List<int> StoreItemList { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
