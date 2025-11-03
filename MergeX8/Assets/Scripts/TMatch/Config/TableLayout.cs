/************************************************
 * Config class : Layout
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class Layout
    {   
        
        // #ID
        public int id ;
        
        // #关卡类型; 1:固定关卡; 2:随机关卡
        public int levelType ;
        
        // #关卡时间（秒）
        public int levelTimes ;
        
        // #关卡设计难度; 1:最简单; 10:最难
        public int designDifficulty ;
        
        // #难度标记; 1:普通; 2:困难; 3:恶魔
        public int difficultyMark ;
        
        // #特殊关卡类型; 0:默认不是特殊关卡; 1:特殊关卡-普通难度; 2:特殊关卡-降低难度
        public int specialType ;
        
        // #仅限特殊关，对应的用户分层
        public int[] specialUserGroup ;
        
        // #特殊关卡降低难度后; 对应的LAYOUTID
        public int specialEasierLayoutId ;
        
        // #目标元素ID
        public int[] targetItemId ;
        
        // #目标元素数量
        public int[] targetItemCnt ;
        
        // #普通元素ID
        public int[] normalItemId ;
        
        // #普通元素数量
        public int[] normalItemCnt ;
        
        // #随机元素ID
        public int[] randomItemId ;
        
        // #随机元素数量基数
        public int[] randomItemCnt ;
        
        // #随机元素数量波动范围; 配置0为不波动; 配置3为在正负3之间波动
        public int[] randomItemCntRange ;
        
        // #随机元素是否出现; 1:一定出现; 0:可能出现
        public int[] randomItemMustHold ;
        
        // #随机元素可能出现; 的元素种类数下限
        public int randomItemIdCntMin ;
        
        // #随机元素可能出现; 的元素种类数上限
        public int randomItemIdCntMax ;
        
    }
}
