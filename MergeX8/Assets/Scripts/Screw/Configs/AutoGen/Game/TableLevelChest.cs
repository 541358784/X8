// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : LevelChest
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableLevelChest
    {   
        // #距离下阶段领奖的等级增量，该等级会以配置的最后一行数据一直循环往下递增，奖励也以最后一行为准
        public int Level { get; set; }// #随机奖励ID
        public List<int> RandomRewardID1 { get; set; }// #随机奖励数量
        public List<int> RandomRewardCnt1 { get; set; }// #随机概率(概率和为100，则必定随机一个奖励处理，小于100，则不一定能随机到奖励)
        public List<int> RandomRewardPro1 { get; set; }// #随机奖励ID
        public List<int> RandomRewardID2 { get; set; }// #随机奖励数量
        public List<int> RandomRewardCnt2 { get; set; }// #随机概率
        public List<int> RandomRewardPro2 { get; set; }
    }
}