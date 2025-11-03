// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Common
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.ConfigHub.Ad
{
    public class Common
    {   
        // 100100-默认; 200100-广告; 300100-付费 ; 400100-高付费 ; 500100-老用户; 600100-低付费
        public int Id { get; set; }// 渠道、国家; 11-IOS美国; 12-IOS非美国; 21-安卓美国; 22-安卓非美国
        public List<int> PlatformGroup { get; set; }// 对应COMMON的ID; 101. 首次进入游戏; 102. 新增3日内未付费; 201. 96小时未付费; 301. 首次付费大于24小时小于等于48; 302. 首次付费大于48小时小于等于72; 303. 首次付费大于72小时小于等于96; 304. 首次付费大于96小时; 401. 首次付费小于24小时
        public List<int> SubUserGroup { get; set; }// SERVER用户组
        public List<int> UserGroup { get; set; }// 单次最大付费金额; （用区间表示，左闭右开）
        public List<int> MaxpayGroup { get; set; }// 用户分层ID
        public int UserTypeId { get; set; }// 活跃度; 101-活跃时长高、完成任务高; 102-活跃时长高、完成任务低; 103-活跃时长低、完成任务高; 104-活跃时长低、完成任务低
        public List<int> ActiveGroup { get; set; }// 任务类型; 1.FREE; 2.PAY; ; 对应任务表里的数据 用FREE 还是PAY
        public int TaskType { get; set; }// RVSHOP解锁条件类型; 1. 关卡
        public int RvshopOpenType { get; set; }// RV解锁等级
        public int RvUnlock { get; set; }// RVSHOP解锁条件参数; 通过本关
        public int RvshopParameter { get; set; }// 主动弹出CD
        public int RvshopCD { get; set; }// RVSHOP广告链; 使用哪些组; （按顺序循环）
        public int RvshopList { get; set; }// 使用哪些组; （按顺序循环）; HAPPY GO RV SHOP
        public int HgRvShopList { get; set; }// 每天插屏总数
        public int IntTotalPerDay { get; set; }// 插屏广告整体间隔时间; (单位秒); COMMON_MONETIZATION_EVENT_REASON_AD_COMMON_COOLDOWN
        public int AdsMinInterval { get; set; }// 每天登陆时间(单位秒); COMMON_MONETIZATION_EVENT_REASON_AD_DAILY_START_COOLDOWN
        public int AdOpenTime { get; set; }// 插屏解锁等级; ; 当前使用的配置20250702
        public int InterstitialUnlock { get; set; }// RV加速 系数
        public int RvSpeedUpFactor { get; set; }// 越买越划算礼包ID; ; -1次层不可见改活动
        public int GiftBagBuyBetterId { get; set; }// 买一赠一礼包ID; ; -1次层不可见改活动
        public int GiftBagSendOneId { get; set; }// 礼包链ID; ; -1次层不可见改活动
        public int GiftBagLinkId { get; set; }// 小猪配置 对应PIGBANK 表; 3个ID
        public List<int> PigBank { get; set; }// 神秘礼包配置
        public int MysteryGift { get; set; }// 闪购配置
        public int FlashSale { get; set; }// 闪购箱子对应PIGSALE表
        public int FlashSaleBox { get; set; }// 
        public int DilayShop { get; set; }// 0 RV; 1 FREE
        public int DailyRewardType { get; set; }// 气球广告消耗; 对应CONSUMEEXTEND
        public int LuckyBalloon { get; set; }// 气球出现资源的比率
        public int LuckBalloonResRatio { get; set; }// 气球可出现的资源
        public List<int> LuckBalloonResIds { get; set; }// 气球广告每日限制
        public int LuckyBalloonLimit { get; set; }// 气球情况1价值区间
        public List<int> LuckyBalloonValue1 { get; set; }// 气球情况3价值区间
        public List<int> LuckyBalloonValue2 { get; set; }// 气球情况3计算数量价值
        public int LuckyBalloonPrice { get; set; }// HG气球广告每日限制
        public int HgLuckyBalloonLimit { get; set; }// HAPPY GO气球广告消耗; 对应CONSUMEEXTEND
        public int HgLuckyBalloon { get; set; }// HAPPY GO气球广告CD 单位秒
        public int HgLuckyBalloonCD { get; set; }// HAPPY GO气球广告产出等级限制
        public int HgLuckyBalloonLevel { get; set; }// HAPPY GO气球最大数
        public int HgLuckyBalloonLevelMAX { get; set; }// 神秘礼物广告消耗; 对应CONSUMEEXTEND
        public int MysteryGiftConsume { get; set; }// 每日任务分组; 对应DAILYTASK
        public int DailyTask { get; set; }// 礼包分组破冰; 对应TABLEPACK
        public int PackData { get; set; }// 购买资源分组; ; -1 不开启充值购买资源; ID 对应BUYRESOURCE 表ID
        public int BuyResource { get; set; }// MASTERCARDID; ; -1 不可见
        public int MasterCardId { get; set; }// 任务助手礼包分组
        public int TaskAssist { get; set; }// 每日礼包内容
        public int DailyPackContain { get; set; }// 动态价格配置
        public int DailyPackPrice { get; set; }// 弹出间隔（分钟）
        public int DailyPackInterval { get; set; }// 日弹出次数上限
        public int DailyPackTimes { get; set; }// 每日排行
        public List<int> DailyRank { get; set; }// TMATCH解锁(任务数)
        public int TMatchUnlock { get; set; }// 气球礼包; ; 101-0.99; 201-1.99; 301-3.99
        public int BalloonPack { get; set; }// 神秘礼物礼包
        public int MysteryPack { get; set; }// TM破冰礼包
        public int TMIceBreakPack { get; set; }// TM复活礼包
        public int TMRevivePack { get; set; }// TM去插屏礼包
        public int TmRemoveAd { get; set; }// 功能开关; ;     "SIGNIN": 1,;     "DAILYTASK": 0,;     "RVSHOP": 1,;     "RVBALLOON": 1,;     "RVGIFT": 1
        public List<string> FunctionOpenType { get; set; }// 动态价格配置组ID
        public int NewDailyPackPrice { get; set; }// 弹出间隔（分钟）
        public int NewDailyPackInterval { get; set; }// 日弹出次数上限
        public int NewDailyPackTimes { get; set; }// 局内广告入口是否显示
        public bool MergeAdEntrance { get; set; }// 首日付费分层
        public int FirstDayPack { get; set; }// 对应的PAYLEVEL等级
        public int PayLevelPack { get; set; }// 是否开启被动触发插屏; ; 0、不开启; 1、开启
        public int PassiveInterOpen { get; set; }// 被动触发插屏开启等级
        public int PassiveInterLevel { get; set; }// 被动插屏触发间隔条件：; 数组1 玩家净游戏时长（秒）; 数组2 玩家完成任务次数; ; 两个条件任意一个≥阈值，即可触发被动插屏; ; 每次重新进入游戏时清零
        public List<int> PassiveInterShowCD { get; set; }// 玩家播放激励视频后，PASSIVEINTERSHOWCD计数暂停时间（秒）; ; 每次重新进入游戏时清零
        public int RvShowPause { get; set; }// 是否开启主动插屏入口替换激励视频入口功能; ; 0、不开启; 1、开启
        public int ActiveInterOpen { get; set; }// 主动插屏入口每日最多替换次数
        public int ActiveInterTotalPerDay { get; set; }// 判断主动插屏入口替换激励视频入口的条件阈值：; ; 计数条件为本日主动插屏入口的播放次数; 1、≤数组1 取 ACTIVEINTERSHOWRVCD和 ACTIVEINTERSHOWTIMECD的数组1参数作为判断条件，二者有一项≥阈值，即可将当前展示的激励视频入口替换为主动插屏入口。; ; 2、＞数组1 ≤数组2 取 ACTIVEINTERSHOWRVCD和 ACTIVEINTERSHOWTIMECD的数组2参数作为判断条件，二者有一项≥阈值，即可将当前展示的激励视频入口替换为主动插屏入口。; ; 3、＞数组2  取 ACTIVEINTERSHOWRVCD 和 ACTIVEINTERSHOWTIMECD的数组3参数作为判断条件，二者有一项≥阈值，即可将当前展示的激励视频入口替换为主动插屏入口。
        public List<int> ActiveInterShowNum { get; set; }// 主动插屏入口替换激励视频入口的替换间隔时长限制（秒）; ; 每次广告入口替换为主动插屏后，清零重记; ; 每次重新进入游戏时清零
        public List<int> ActiveInterShowRvCD { get; set; }// 主动插屏入口替换激励视频入口的观看激励视频次数限制; ; 每次广告入口替换为主动插屏后，清零重记; ; 每次重新进入游戏时清零
        public List<int> ActiveInterShowTimeCD { get; set; }// 出现激励视频幸运气泡的时候，是否会弹出广告入口弹窗。; ; 0  不会 ; 1 会
        public int LuckyBubblePopup { get; set; }// 存在气泡的情况下，不触发激励视频气球; ; 0  不开启 ; 1 开启
        public int BubbleRuleoutBalloon { get; set; }// 钻石折叠显示
        public List<int> DiamondGoodsFold { get; set; }// 折叠下显示钻石导购标签
        public List<int> ShowSaleInFold { get; set; }// 展开下显示钻石导购标签
        public List<int> ShowSaleInAll { get; set; }// 小费奖励; ; 1触发类型1; 2触发类型2; 0关闭; 
        public int TipReward { get; set; }
    }
}