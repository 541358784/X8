// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : ClimbTowerReward
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.ClimbTower
{
    public class TableClimbTowerReward
    {   
        // ID
        public int Id { get; set; }// 是否是付费数据
        public bool IsPay { get; set; }// 分层
        public int PayLevel { get; set; }// 奖励ID
        public List<int> RewardItem { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 权重
        public List<int> Weight { get; set; }// 复活价格(钻石),负数为广告复活
        public int RebornPrice { get; set; }
    }
}