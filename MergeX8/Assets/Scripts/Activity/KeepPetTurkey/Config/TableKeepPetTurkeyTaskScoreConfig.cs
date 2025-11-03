using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetTurkeyTaskScoreConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 火鸡活动积分
    public int TurkeyScore { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
