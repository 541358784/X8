using System;
using System.Collections.Generic;

[System.Serializable]
public class RecoverCoinSkinConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 皮肤名称
    public string SkinName { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
