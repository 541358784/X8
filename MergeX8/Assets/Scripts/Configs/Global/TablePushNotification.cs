/************************************************
 * Config class : TablePushNotification
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TablePushNotification : TableBase
{   
    
    // #
    public int id ;
    
    // 类型，ENUM
    public string type ;
    
    // 内容
    public string text ;
    
    // 按天
    public int timeInDay ;
    
    // 按秒
    public int timeInSecond ;
    


    public override int GetID()
    {
        return id;
    }
}
