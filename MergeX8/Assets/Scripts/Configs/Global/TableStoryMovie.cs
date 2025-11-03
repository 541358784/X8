/************************************************
 * Config class : TableStoryMovie
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStoryMovie : TableBase
{   
    
    // 剧情电影ID
    public int id ;
    
    // 后置剧情电影ID
    public int next_id ;
    
    // 描述
    public string desc ;
    
    // 触发位置; 只有首个触发的对话 需要配置此参数; 1.游戏开始初始剧情; 2.剧情结束; 3.CG 结束; 4.挂点装修结束; 5.挂点开始装修; 6.主动触发
    public int triggerPosition ;
    
    // 触发参数; 只有首个触发的对话 需要配置此参数
    public string[] triggerParam ;
    
    // 检测挂点
    public bool checkDecoNode ;
    
    // 是否自动保存
    public bool autoSave ;
    
    // 保存的ID
    public int[] saveIds ;
    
    // 暂停角色动画
    public bool stopStatus ;
    
    // 恢复角色动画
    public bool restoreStatus ;
    
    // BI配置
    public string bi ;
    
    // 时间轴 按照时间轴 逐个执行动画
    public float timeLine ;
    
    // 动画时间
    public float movieTime ;
    
    // UI 控制; ; 1 是否隐藏UI 2开启; 1 是否立即隐藏 否则动画
    public int[] uiLogic ;
    
    // 是否恢复UI
    public bool restoreUI ;
    
    // 控制类型; 1.主角 ; 2.小狗; 3.摄像机; 4.男主; ; 10.其他场景对象 ; ; -1.NONE
    public int controlType ;
    
    // 控制对象名字 只对3起作用
    public string controlNames ;
    
    // 相互绑定路径
    public string linkPath ;
    
    // 是否循环
    public bool isLoop ;
    
    // 动画名字
    public string animationName ;
    
    // 动作类型; 1. 原地播放动画; 2. 移动; 3. 渐显出现; 4.挂点预览; 5.挂点恢复; 6.添加奖励; 7.生成任务; 8.显示挂点气泡; 9.摄像机缩放; 10.SETACTIVE; 11.CG
    public int actionType ;
    
    // 动画缓动类型 针对MOVE
    public int easeType ;
    
    // POSITION
    public float[] position ;
    
    // ROTATION
    public float[] rotation ;
    
    // 动作参数
    public string actionParam ;
    


    public override int GetID()
    {
        return id;
    }
}
