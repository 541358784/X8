using System;
using System.Collections.Generic;

[System.Serializable]
public class CatchFishGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
