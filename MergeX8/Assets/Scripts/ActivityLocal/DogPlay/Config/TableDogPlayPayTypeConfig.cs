using System;
using System.Collections.Generic;

[System.Serializable]
public class DogPlayPayTypeConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 付费阈值
    public int MinPay { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
