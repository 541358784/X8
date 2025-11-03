/************************************************
 * Config class : TableDays
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableDays : TableBase
    {   
        
        // ID
        public int id ;
        
        // 挂点数量
        public int nodeNumber ;
        
        // 奖励序列
        public int[] rewardIndex ;
        
        // 天数奖励
        public string[] planb_rewardId ;
        
        // 天数奖励
        public string[] rewardId ;
        
        // 天数奖励数量
        public string[] rewardNum ;
        
        // 老玩家找回的奖励ID
        public int[] retrieveRewardId ;
        
        // 老玩家找回的奖励数量
        public int[] retrieveRewardNum ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
