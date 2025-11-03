using System;
using System.Collections.Generic;

[System.Serializable]
public class EasterConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 初始给的建筑ID
    public int StartBuild { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
