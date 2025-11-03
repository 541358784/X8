// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : DitchLevel
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Ditch
{
    public class TableDitchLevel
    {   
        // 关卡ID
        public int Id { get; set; }// 解锁挂点
        public int UnlockNodeId { get; set; }// 累积完成装修任务数
        public int UnlockNodeNum { get; set; }// 即将到来
        public bool IsComingSoon { get; set; }// 对应的挖沟关卡ID
        public int DitchLevel { get; set; }// MERGE关卡ID
        public List<int> MergeId { get; set; }// 通关获得道具图片
        public string PropIcon { get; set; }// 关卡图片
        public string LevelIcon { get; set; }// 关卡名字
        public string LevelName { get; set; }// 局内阶段ICON列表
        public List<string> PointIcons { get; set; }// 进度条
        public bool HasSlider { get; set; }// 进度条PAD
        public bool HasSliderPad { get; set; }// 进度条PAD
        public string DigEffectAsset { get; set; }
    }
}