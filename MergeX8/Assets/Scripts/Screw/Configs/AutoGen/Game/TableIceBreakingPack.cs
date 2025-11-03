// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : IceBreakingPack
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableIceBreakingPack
    {   
        // 编号; 
        public int Id { get; set; }// 商品ID
        public int ShopId { get; set; }// 显示折扣百分比（1-100）; 配置100表示没有折扣
        public int Discount { get; set; }// 道具ID组（固定8个，影响排版）
        public List<int> ItemIds { get; set; }// 道具数量组
        public List<int> ItemNums { get; set; }// 限购次数
        public int BuyLimit { get; set; }// 每天最多展示次数
        public int ShowDayLimit { get; set; }// 展示条件（最低在线时长，单位 秒）
        public string MinOnlineTime { get; set; }
    }
}