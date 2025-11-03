/************************************************
 * Config class : ItemConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class ItemConfig
    {   
        
        // ID; 
        public int id ;
        
        // 道具类型; 
        public int type ;
        
        // 分类
        public int[] param ;
        
        // 子ID
        public int subId ;
        
        // 道具名字
        public string name ;
        
        // 货币小图片资源
        public string pic_res ;
        
        // 货币大图片资源
        public string pic_res_big ;
        
        // 货币特殊图片资源
        public string pic_res_special ;
        
        // 最大持有数量
        public int max ;
        
        // 无视上限
        public bool ignoreMax ;
        
        // 初始获得
        public int initialNum ;
        
        // 自动获得
        public int autoClaim ;
        
        // 自动获得间隔（秒）
        public int autoClaimInterval ;
        
        // 兑换货币
        public int exchange ;
        
        // 价格
        public int price ;
        
        // 获取途径描述
        public string sourceDesc ;
        
        // 活动来源
        public string eventSource ;
        
        // #效果数值; 磁铁：消除几组目标; 扫把：扫除最大格子数; 风车：重新排列次数; 冰封：禁锢时间.秒; 闪电：消除几组模型; 时钟：增加时间.秒
        public int effectValue1 ;
        
        // #是否是无限道具
        public bool infinity ;
        
        // #无限时间
        public int infiniityTime ;
        
        // #无限道具显示规则：; 1：无横幅，如无限体力; 2：横幅+无限标记，如闪电时钟; 3：横幅+X2，如周挑战BUFF道具
        public int infinityIcon ;
        
    }
}
