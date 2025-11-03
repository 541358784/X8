/************************************************
 * Config class : Item
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatch
{
    [System.Serializable]
    public class Item
    {   
        
        // #ID
        public int id ;
        
        // # 禁止使用
        public bool forbid ;
        
        // #层级值（1-4）; 越小位置越低; 1底：底板最底层; 2中：底板之上一层; 3上：随机落在1-3之中; 4顶：天花板最上层
        public int layer ;
        
        // #预制件名称
        public string prefabName ;
        
        // #下方放置时的朝向; (0,0,0)则不填
        public string layRot ;
        
        // #道具模型对应连赢黄金帽使用的道具ID（便于程序查找）TM-GAME_ITEM
        public int boosterId ;
        
        // #模型缩放比例; （不填默认为1,1,1）
        public string scalingRatio ;
        
        // #特效名称
        public string effectName ;
        
        // 主题ID（1水果 2动物 3食物 4金属 5奢侈品 6 日用品 7 交通工具 8 自然 9 数字字母  10七彩物品）
        public int themeId ;
        
        // 
        public int isRed ;
        
        // 
        public int isPurple ;
        
        // 
        public int isWhite ;
        
        // 
        public int isOrange ;
        
        // 
        public int isYellow ;
        
        // 
        public int isGreen ;
        
        // 
        public int isBrown ;
        
        // 
        public int isBlue ;
        
        // 
        public int isBlack ;
        
        // 
        public int isPink ;
        
    }
}
