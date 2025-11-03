using System;
using System.Collections.Generic;

[System.Serializable]
public class PillowWheelGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 转动消耗
    public int SpinCost { get; set; }// 分层组
    public int PayLevelGroup { get; set; }// 进榜分数
    public int LeastEnterBoardScore { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
