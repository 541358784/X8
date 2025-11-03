// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TeamIconConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Team
{
    public class TableTeamIconConfig
    {   
        // ID
        public int Id { get; set; }// 头像路径名字
        public string HeadIconName { get; set; }// 头像图集
        public string HeadIconAtlas { get; set; }// 默认头像框
        public int DefaultHeadIconFrameId { get; set; }// 是否需要收集
        public bool IsNeedCollect { get; set; }// 是否为预制体
        public bool IsPrefab { get; set; }
    }
}