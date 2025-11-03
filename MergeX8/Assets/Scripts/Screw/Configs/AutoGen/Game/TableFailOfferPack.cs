// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FailOfferPack
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableFailOfferPack
    {   
        // 编号
        public int Id { get; set; }// 分组
        public int Group { get; set; }// 礼包等级
        public int Level { get; set; }// 商品ID
        public int ShopId { get; set; }// 显示折扣百分比（1-100）; 配置100表示没有折扣
        public int Discount { get; set; }// 道具ID组
        public List<int> ItemIds { get; set; }// 道具数量组
        public List<int> ItemNums { get; set; }// 价格
        public string Price { get; set; }
    }
}