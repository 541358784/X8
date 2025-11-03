// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Item
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableItem
    {   
        // #物品ID命名范围; 基础货币：1-9; 道具ID：10-99; 活动物品：100-999
        public int ItemId { get; set; }// #物品类型; 1-金币; 2-体力; 3-星星; 4-额外的螺丝洞; 5-锤子; 6-额外的工具箱; ; 10-限时体力; 11-关卡螺丝元素; 12-卖惨货币; 17-头像; ; ; ; 
        public int ItemType { get; set; }// #描述
        public string Desc { get; set; }// #图集名称
        public string Atlas { get; set; }// #图标名称
        public string Icon { get; set; }// #是否是无限道具
        public bool Infinity { get; set; }// #无限时间
        public int InfinityTime { get; set; }// #无限道具显示规则：; 1：无横幅，如无限体力; 2：横幅+无限标记，如闪电时钟; 3：横幅+X2，如周挑战BUFF道具
        public int InfinityIcon { get; set; }// #客户端用于假显示
        public int Amount { get; set; }
    }
}