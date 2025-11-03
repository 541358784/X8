using System;
using System.Collections.Generic;

[System.Serializable]
public class KapiScrewGlobalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 生命值上限
    public int MaxLife { get; set; }// 生命值恢复时间(分钟)
    public int LifeRecoverTime { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
