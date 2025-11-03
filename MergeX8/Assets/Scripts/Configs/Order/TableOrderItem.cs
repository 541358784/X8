/************************************************
 * Config class : TableOrderItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderItem : TableBase
{   
    
    // ID 对应的MERGEITEM ID
    public int id ;
    
    // 难度
    public int difficulty ;
    
    // 解锁等级
    public int unlockLevel ;
    
    // 价值
    public int price ;
    
    // 权重
    public int levelWeight ;
    
    // 权重
    public int diffWeight ;
    
    // 权重倍数
    public int weightMultiple ;
    
    // 进度解锁 必须获取当前物品
    public int progressUnlock ;
    
    // 触发回收任务数量
    public int tiggerNumber ;
    
    // 回收任务回收数量
    public int recommendedNumber ;
    
    // 推荐的位置
    public int[] recommendedSlots ;
    
    // 禁止出现的位置
    public int[] forbiddenSlot ;
    
    // 生产建筑
    public int[] generators ;
    
    // 出现以下建筑ID 抛弃该ITEM
    public int[] discardBuilds ;
    
    // 必须完成任务ID
    public int completeOrderId ;
    
    // 是否是回收任务目标
    public bool isRecycle ;
    
    // 是否触发第二回收任务
    public bool triggerSecondRecycle ;
    
    // 二次回收MERGELINE
    public int[] secondRecycleMergeLine ;
    
    // 可回收素材
    public int[] secondRecycleItems ;
    


    public override int GetID()
    {
        return id;
    }
}
