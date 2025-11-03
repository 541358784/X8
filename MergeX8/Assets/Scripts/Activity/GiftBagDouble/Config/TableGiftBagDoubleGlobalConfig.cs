using System;
using System.Collections.Generic;

[System.Serializable]
public class GiftBagDoubleGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
