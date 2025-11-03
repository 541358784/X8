/************************************************
 * Config class : TableStimulateSpine
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStimulateSpine : TableBase
{   
    
    // ID
    public int id ;
    
    // 按钮图片名字
    public string[] buttonIcons ;
    
    // 选择正确的ICON 才会出发SPINE 动画
    public string[] correctIcons ;
    
    // 绑定的OBJ
    public string[] linkObjs ;
    
    // SPINE动画名字
    public string[] spineAnims ;
    
    // 预制体名字
    public string prefabName ;
    
    // SPINE路径
    public string spinePath ;
    
    // 声音音效
    public string[] audioNames ;
    
    // 默认动画名字
    public string defaultAnim ;
    
    // 摄像机缩放
    public float cameraSize ;
    
    // 摄像机位置
    public float[] cameraInitPos ;
    


    public override int GetID()
    {
        return id;
    }
}
