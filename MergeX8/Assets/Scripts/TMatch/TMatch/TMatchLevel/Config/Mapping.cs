// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Mapping
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.ConfigHub.TMatchLevel
{
    public class Mapping
    {   
        
        /// <summary>
        /// #分组映射表
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// #分组GROUPID
        /// </summary>
        public int UserGroup { get; set; }
        
        /// <summary>
        /// 分组GROUPID; 100=低难度（广告组）; 200=较低难度（默认组）; 300=中难度（低概率付费+广告付费）; 400=难（高概率付费和指定内购）; 500=困难（付费）
        /// </summary>
        public int LevelUserGroup { get; set; }
        
    }
}