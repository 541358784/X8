using System;
using System.Collections.Generic;

[System.Serializable]
public class PayLevelConfig : TableBase
{   
    // 消费等级
    public int Id { get; set; }// 升级所需连续付费天数
    public int UpGradeContinuePayDays { get; set; }// 升级所需单天付费金额($)
    public int UpGradeSingleDayPayValue { get; set; }// 降级所需连续未付费天数
    public int DownGradeContinueUnPayDays { get; set; }// 三日升档金额
    public int ThreeDayUpValue { get; set; }// 每次升档最高档位
    public int MaxUpCount { get; set; }// 降档周期
    public int DownDayNum { get; set; }// 周期内降档总金额
    public int SevenDayDownValue { get; set; }// 降档后维持天数; （配1在当前档位停留两天）
    public int DownKeepDay { get; set; }// 任务组
    public int OrderGroupId { get; set; }// 新每日礼包组ID
    public int NewDailyPackGroupId { get; set; }// 每日礼包组ID
    public int DailyPackGroupId { get; set; }// 3合1档位
    public int ThreeGiftGroupId { get; set; }// 1+1档位
    public int GiftBagSendOneGroupId { get; set; }// 1+2档位
    public int GiftBagSendTwoGroupId { get; set; }// 1+3档位
    public int GiftBagSendThreeGroupId { get; set; }// 1+4档位
    public int GiftBagSend4GroupId { get; set; }// 1+6档位
    public int GiftBagSend6GroupId { get; set; }// 俩礼包档位
    public int GiftBagDoubleGroupId { get; set; }// 体力礼包档位
    public List<int> EnergyPackageGroupId { get; set; }// 小猪档位
    public List<int> PigBankIdList { get; set; }// 是否开启本地小猪
    public bool LocalPigBank { get; set; }// 进步礼包分层
    public int GiftBagProgressGroupId { get; set; }// 越买越划算档位
    public int GiftBagBuyBetterGroupId { get; set; }// 礼包链档位
    public int GiftBagLinkGroupId { get; set; }// 自选礼包档位
    public int OptionalGiftGroupId { get; set; }// 升级礼包组ID
    public int LevelUpPackageGroupId { get; set; }// 累充活动分层
    public int TotalRechargeGroupId { get; set; }// 新破冰
    public int NewNewIceBreakPackGroupId { get; set; }// 新手累充
    public int TotalRechargeNewGroupId { get; set; }// 蝴蝶组ID
    public int ButterFlyGroupId { get; set; }// 金币RUSH
    public int CoinRushGroupId { get; set; }// 打靶
    public int BiuBiuGroupId { get; set; }// 花田
    public int FlowerFieldGroupId { get; set; }// 丛林探险
    public int JungleAdventureGroupId { get; set; }// 养鱼
    public int FishCultureGroupId { get; set; }// 鹦鹉组
    public int ParrotGroupId { get; set; }// 花园挖虫
    public int GardenTreasureGroupId { get; set; }// 跳格子组
    public int JumpGridGroupId { get; set; }// 幸运砸蛋
    public int LuckyEggGroupId { get; set; }// 海上竞速
    public int SeaRacingGroupId { get; set; }// 主题装修
    public int ThemeDecorationGroupId { get; set; }// BP1
    public int BattlePass1GroupId { get; set; }// BP2
    public int BattlePass2GroupId { get; set; }// 开启旧每日礼包
    public bool OpenOldDailyPack { get; set; }// 卡皮巴拉是否开启
    public bool KaipiOpenFlag { get; set; }// 无尽体力礼包是否开启
    public bool EndlessEnergyGiftBagOpenFlag { get; set; }// 爬塔
    public int ClimbTowerGroupId { get; set; }// 大富翁
    public int MonopolyGroupId { get; set; }// 小狗
    public int KeepPetGroupId { get; set; }// 收集宝石
    public int CollectStone { get; set; }// 公会任务
    public int TeamOrder { get; set; }// 公会任务
    public int PillowWheel { get; set; }// 去广告礼包
    public int NoAdsGiftBag { get; set; }// 订单狂热
    public int CrazeOrder { get; set; }// 限时任务
    public int TimeOrder { get; set; }// 限时任务链
    public int LimitOrderLine { get; set; }// 套娃
    public int Matreshkas { get; set; }// 搬家订单
    public int TrainOrderGroupId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
