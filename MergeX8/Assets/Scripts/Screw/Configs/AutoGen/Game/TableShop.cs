// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Shop
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableShop
    {   
        // #
        public int Id { get; set; }// 付费点类型; 1.金币档位; 2.每日礼包; 3.复活礼包
        public int ProductType { get; set; }// 商店中显示组类别; 0.不显示; 1.金币档位; 2.每日礼包
        public int ShopViewGroupType { get; set; }
    }
}