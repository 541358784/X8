// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TeamIconFrameConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Team
{
    public class TableTeamIconFrameConfig
    {   
        // ID
        public int Id { get; set; }// 头像路径名字
        public string HeadIconFrameName { get; set; }// 头像图集
        public string HeadIconFrameAtlas { get; set; }// 头像类型; ; 按照优先级排序; 0:头像默认携带; 1:单独获得的头像框
        public int HeadIconFrameType { get; set; }// 是否为预制体
        public bool IsPrefab { get; set; }
    }
}