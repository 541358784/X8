using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetClueConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 
    public string ImageBigName { get; set; }// 线索图片
    public string ImageName { get; set; }// 多语言名字
    public string Name { get; set; }// 说明多语言
    public string Explain { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
