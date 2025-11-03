/************************************************
 * Config class : SlotMachineGlobalConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class SlotMachineGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 结果配置(会打乱)
    public List<int> ResultConfigList { get; set; }// 重复结果概率%(只触发一次)
    public List<int> RepeatResultChance { get; set; }// 图标列表(1:钻石 2:体力 3:金币 4:7 5:绿钻)
    public List<int> ElementList { get; set; }// 引导结果
    public List<int> GuideSpinResult { get; set; }// 引导重转结果
    public int GuideReSpinResult { get; set; }// 引导重转的轮带下标(0开始)
    public int GuideReSpinReelIndex { get; set; }// 是否打乱轮带
    public bool MixReel { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
