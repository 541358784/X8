/************************************************
 * Config class : TableStory
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStory : TableBase
{   
    
    // 剧情起始ID
    public int id ;
    
    // 后置剧情ID
    public int next_id ;
    
    // 是否可以重复触发
    public bool immediatelySave ;
    
    // 是否隐藏UI
    public bool hideUI ;
    
    // 重复触发
    public bool repeatTrigger ;
    
    // 剧情电影结束
    public string desc ;
    
    // 触发位置; 只有首个触发的对话 需要配置此参数
    public int triggerPosition ;
    
    // 触发参数; 只有首个触发的对话 需要配置此参数
    public string triggerParam ;
    
    // BI配置
    public string bi ;
    
    // 人物ID; ; ROLES 表里的
    public int role_id ;
    
    // 位置; 左：1; 右：2; 只有图片 带手 3; 图片 没有手 4; 图片 全屏图片 5
    public int position ;
    
    // 是否允许SKIP
    public bool skip ;
    
    // 剧情文字
    public string en ;
    


    public override int GetID()
    {
        return id;
    }
}
