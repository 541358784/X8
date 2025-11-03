/************************************************
 * Config class : TableUnlockMergeLine
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableUnlockMergeLine : TableBase
{   
    
    // ID
    public int id ;
    
    // 解锁合成链 条件; 0 解锁合成链; 1 完成任务; 2 等级解锁
    public int unlockType ;
    
    // 解锁参数
    public int[] unlockParam ;
    
    // 解锁的MERGEID
    public int unlockMergeId ;
    
    // 描述KEY
    public string desKey ;
    
    // ANIMNAME
    public string animName ;
    


    public override int GetID()
    {
        return id;
    }
}
