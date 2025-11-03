using System;
using System.Collections.Generic;

[System.Serializable]
public class RVshopList : TableBase
{   
    // ID
    public int Id { get; set; }// 组
    public int Group { get; set; }// 等级
    public int Level { get; set; }// RVSHOPRESOURCE组成的组
    public List<int> List { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
