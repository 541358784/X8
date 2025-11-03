using System;
using System.Collections.Generic;

[System.Serializable]
public class ThemeDecorationGlobalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 预热时间(分钟)
    public int PreheatTime { get; set; }// 皮肤名称
    public string SkinName { get; set; }// 预览用的中心装修ITEM
    public int CenterDecoItem { get; set; }// 延期购买等待时间(分钟)
    public int ExtendBuyWaitTime { get; set; }// 延期购买时间(分钟)
    public int ExtendBuyTime { get; set; }// 延期购买SHOPID
    public int ExtendBuyShopId { get; set; }// 奖励ID
    public List<int> ExtendBuyRewardId { get; set; }// 奖励数量
    public List<int> ExtendBuyRewardNum { get; set; }// 是否替换棋盘背景
    public bool ReplaceMergeBoard { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
