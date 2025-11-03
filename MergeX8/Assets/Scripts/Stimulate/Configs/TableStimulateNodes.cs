/************************************************
 * Config class : TableStimulateNodes
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStimulateNodes : TableBase
{   
    
    // 挂点ID
    public int id ;
    
    // 关卡ID
    public int levelId ;
    
    // 默认开启
    public bool defatultOpen ;
    
    // 后置解锁挂点
    public int nextNodeId ;
    
    // 挂点类型; 1: 普通SPINE 动画; 2: MERGE 游戏
    public int type ;
    
    // 根据TYPE 配置不同的参数; 1: LEVELID; 2: LEVELID 
    public int param ;
    
    // 默认动画名字
    public string[] spineDefaultAnims ;
    
    // 完成时动画
    public string[] spineFinishAnims ;
    
    // 场景默认动画
    public string screenDefaultAnim ;
    
    // 场景改变动画
    public string screenChangeAnim ;
    
    // 场景完成动画
    public string screenFinishAnim ;
    
    // 默认音效
    public string defaultAudio ;
    
    // 停止音效
    public bool stopAudio ;
    
    // 完成时音效
    public string finishAudio ;
    
    // 挂点路径
    public string nodePath ;
    


    public override int GetID()
    {
        return id;
    }
}
