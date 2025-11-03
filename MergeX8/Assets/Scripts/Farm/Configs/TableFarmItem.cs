// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmItem
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmItem
    {   
        // ID
        public int Id { get; set; }// 多语言
        public string NameKey { get; set; }// 图片
        public string Image { get; set; }// 单个素材价格
        public int Award { get; set; }
    }
}