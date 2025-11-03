using System;
using System.Collections.Generic;

[System.Serializable]
public class ThmeDecorationLeaderBoardScheduleConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 开始时间(分钟)
    public int StartTime { get; set; }// 结束时间(分钟)
    public int EndTime { get; set; }// 奖励配置列表
    public List<int> RewardConfigList { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
