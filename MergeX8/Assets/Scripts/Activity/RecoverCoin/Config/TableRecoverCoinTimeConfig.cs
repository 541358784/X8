using System;
using System.Collections.Generic;

[System.Serializable]
public class RecoverCoinTimeConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 第几周
    public int Week { get; set; }// 开始时间戳豪秒
    public string StarTimeSec { get; set; }// 结束时间戳豪秒
    public string EndTimeSec { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
