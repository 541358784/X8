using System;
using System.Collections.Generic;

[System.Serializable]
public class KapiScrewNameConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 名称
    public string Name { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
