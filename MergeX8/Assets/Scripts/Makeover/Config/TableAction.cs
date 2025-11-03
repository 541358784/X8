/************************************************
 * Config class : TableAction
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableAction : TableBase
{   
    
    // ID
    public int id ;
    
    // 类型:; 1:显/隐藏对象组(参数:0-隐藏 1-显示,参1:对象组名); 2:渐隐对象组(参数:持续时间,参数1:对象组名); 3:渐显对象组(参数:持续时间,参数1:对象组名)); 4:SPINE对象组动画(参数:0不循环,1循环,-1暂停在首帧，-2暂停在末帧,参数1:对象组名,参数2:对象组动画名); 5:SPINE对象组换肤(参数1:对象组名,参数2:皮肤名); 6:动画控制器对象组(参数:0不循环,1循环,参数1:对象组名,参数2:对象组动画名); 7:移动位置(参数:0-瞬间移动，数值MS移动持续时间,参数1:对象名,参数2:新位置坐标X,Y,Z); 8:旋转(参数:新欧拉角值X,Y,Z,参数1:对象名); 9:缩放(参数:新缩放值X,Y,Z,参数1:对象名); 10:播放音效(参数:0-不循环 1-循环，参数1:音效名); 11.SPINE暂停对象组动画(参数1:对象组名); 12.SPINE恢复对象组动画(参数1:对象组名); 13.停止音效(参数1:音效名); 14.SPINE对象播放指定时长动画(参数：0-开始时间MS,1-结束时间MS,参数1:对象名，参数2:动画名; 15.延迟停止ACTION
    public int type ;
    
    // 参数
    public int[] parameters ;
    
    // 参数1
    public string[] parameters1 ;
    
    // 参数2
    public string[] parameters2 ;
    
    // 完成后触发：; 0.无; 1.触发TIMLINE结束检查
    public int finishedTrigger ;
    


    public override int GetID()
    {
        return id;
    }
}
