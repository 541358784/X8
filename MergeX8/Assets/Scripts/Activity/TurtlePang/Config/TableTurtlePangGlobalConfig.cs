/************************************************
 * Config class : TurtlePangGlobalConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TurtlePangGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 幸运色补包数
    public int LuckyColorSendCount { get; set; }// 连线补包数
    public int DrawLineSendCount { get; set; }// 对对碰补包数
    public int SameColorSendCount { get; set; }// 清场补包数
    public int CleanBoardSendCount { get; set; }// 填满棋盘补包数
    public int FillBoardSendCount { get; set; }// 乌龟兑换积分比例
    public List<int> TurtleExchangeScore { get; set; }// 棋盘尺寸
    public int BoardSize { get; set; }// 包数1
    public List<int> PackageType1 { get; set; }// 包数2
    public List<int> PackageType2 { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
