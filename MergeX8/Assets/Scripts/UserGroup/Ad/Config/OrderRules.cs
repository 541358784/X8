// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : OrderRules
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.ConfigHub.Ad
{
    public class OrderRules
    {   
        // #
        public int Id { get; set; }// CAMPAIGN
        public List<int> Campaign { get; set; }// 国家 NULL 没有找到国家的保底
        public List<string> Country { get; set; }// PLATFORM 平台
        public string Platform { get; set; }// 任务AB是否开启
        public bool OpenOrderAB { get; set; }// 指定AB 分组 1 默认 2 简单 针对OPEN FALSE用户
        public int AssignedABGroup { get; set; }
    }
}