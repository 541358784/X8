/************************************************
 * Config class : ExtraEnergyActivityConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class ExtraEnergyActivityConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 额外体力数
    public int ExtraEnergy { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
