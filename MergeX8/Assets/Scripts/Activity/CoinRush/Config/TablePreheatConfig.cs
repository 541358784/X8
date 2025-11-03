using System;
using System.Collections.Generic;

[System.Serializable]
public class PreheatConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 皮肤
    public string SkinName { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
