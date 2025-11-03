// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Global
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableGlobal
    {   
        // 活动拉取CD（秒）
        public int SeverActivityDataFetchCD { get; set; }// 体力---最大能量值
        public int MaxUserEnergy { get; set; }// 能量每次填满花费的金币(只会在体力为0的情况下)
        public int EnergyGemPrice { get; set; }// 体力---自动回能量的时间间隔(秒)
        public int EnergyRefillTime { get; set; }// 体力---每次回复多少能量
        public int EnergyRefillAmount { get; set; }// 格子满了关卡失败后继续PLAY ON需要花费的金币
        public List<int> CoinPlayOn { get; set; }// 限时关卡购买时间花费金币
        public List<int> CoinLimitTimeLevel { get; set; }// 炸弹障碍物失败复活
        public List<int> CoinBomb { get; set; }// 冰障碍物失败复活
        public List<int> CoinIce { get; set; }// 开关障碍物失败复活
        public List<int> CoinShutter { get; set; }// 限时关卡花费金币购买时间 单位是 秒
        public int TimeLimitTimeLevel { get; set; }// 等级宝箱解锁
        public int LevelChestUnlcok { get; set; }// 周挑战解锁
        public int WeeklyChallengeUnlcok { get; set; }// 签到解锁
        public int DailyBonusUnlcok { get; set; }// 降低复活礼包档位所需的未购买次数
        public int LowerRevivePackNotBuyTime { get; set; }// 头像解锁
        public int AvatarUnlock { get; set; }// 通过关卡的金币奖励
        public int PassLevel { get; set; }// 通过关卡的CLOVER奖励
        public int PassLevelClover { get; set; }// 锤子 *3 购买需要的金币
        public int HammerCoin { get; set; }// 额外螺丝洞 * 3购买需要的金币
        public int ScrewHoleCoin { get; set; }// 额外工具箱 * 3 购买需要的金币
        public int ExtraBoxCoin { get; set; }// 周排行榜解锁等级
        public int WeekRankLockLevel { get; set; }// 玩家领取战令共享礼物每天上限次数
        public int TeamPassDayLimit { get; set; }// BONUSAD弹窗CD 秒
        public int LuckySpinAdPopCD { get; set; }// LUCKYSPINAD弹窗CD 秒
        public int BonusAdPopCD { get; set; }
    }
}