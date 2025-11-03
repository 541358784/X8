/************************************************
 * Config class : TableAsmrStepNew
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableAsmrStepNew : TableBase
{   
    
    // ID
    public int id ;
    
    // 激活条件（一旦满足条件就会认为该STEP生效）
    public int[] unlock ;
    
    // 操作方式; 1.点击; 2.拖拽; 3.单手滑动; 4.双手滑动; 5.双手长按
    public int actionType ;
    
    // 持续性操作需满足的条件; 滑动以屏幕宽度为一个单位; 持续时间以秒为单位
    public float requireValue ;
    
    // 目标
    public string target ;
    
    // 工具路径
    public string toolPath ;
    
    // 不重置坐标的工具
    public string[] toolsKeepPos ;
    
    // 触发的动画路径
    public string[] spinePath ;
    
    // 触发的动画名字
    public string[] spineAnimName ;
    
    // 挂载动画节点
    public string animatorPath ;
    
    // 挂载在根结点的动画
    public string animatorName ;
    
    // 开始时隐藏的物体
    public string[] hidePaths_Enter ;
    
    // 完成时隐藏的物体
    public string[] hidePaths_Exit ;
    
    // 开始时显示的物体
    public string[] showPaths_Enter ;
    
    // 动画播放延迟隐藏
    public string[] hidePaths_Player_with_Delay ;
    
    // 相机坐标
    public float[] cameraPos ;
    
    // 相机大小
    public float cameraSize ;
    
    // 手指坐标
    public float[] tipPos ;
    
    // 音效
    public string[] sound ;
    
    // 音效延迟
    public float[] soundDelay ;
    
    // 工具挂载到骨骼后需要重置的参数; 默认有问题才配置这一项; 资源制作不统一
    public bool toolResetScale ;
    
    // 工具挂载到骨骼后需要重置的参数; 默认有问题才配置这一项; 资源制作不统一
    public bool toolResetRotation ;
    
    // 工具层级动态调整
    public int toolLayer ;
    
    // 工具骨骼
    public string toolAttachPath ;
    
    // 进入该步骤时; 自动挂载工具
    public bool autoAttachTool ;
    
    // 1.时间点; 2.时间段
    public int vib_time_type ;
    
    // 动画播放延迟震动,关键帧
    public int[] vib_Player_with_Delay ;
    
    // 手指动画
    public string handTipAnim ;
    
    // BI
    public string tiggerBi ;
    
    // 文字指引(KEY)
    public string textGuide ;
    


    public override int GetID()
    {
        return id;
    }
}
