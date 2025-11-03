// triggerPosition 触发教程位置

public enum GuideTriggerPosition
{
    GuideEnd = 1, //引导结束
    StoryEnd = 2, //剧情结束
    EnterMerge = 3, //进入游戏
    MergeFinish = 4, //Merge完成
    ProductFinish = 5, //生成完成
    TaskFinish = 6, //引导任务
    ClickTaskNeedItem = 10, //点击任务需求物品
    CloseItemInfo = 11, //关闭info信息
    ClickInfoButton = 12, //点击info按钮
    InfoDes = 13, //info描述
    ASMR_Play = 15, //点击asmrplay按钮
    
    UnlockDeco = 20, //解锁装修
    GoDeco = 21, //去装修
    
    TouchBubble = 30, //点击挂点泡泡
    BuyNode = 31, //购买挂点
    ConfirmNode=32,//确认购买
    
    GetReward = 40, //临时物品序列
    ChoseItem = 41, //选中物品
    
    EnterHome = 50, //进入home
    EnterAsmr = 51, //asmr
    
    AddEnergy = 59, //点击+号增加体力
    FreeEnergy = 60, //免费体力
    
    BoxOpen = 70, //开启箱子
    BoxSpeedUp = 71, //点击箱子加速
    
    EatFood=80, //喂食食物
    ChoseEatCdItem=81,//选择cd需要喂食的建筑
    
    ChoseCdProduct = 100, //选中cd物品
    CdProductSpeedUp = 101, //钻石秒cd
    
    ClickStore = 120, //点击商店
    GetFreeReward = 121, //领取免费物品
    
    ClickTaskCenter = 130, //点击商店
    EnterTaskCenter = 131, //领取免费物品
    
    Bp_Reward = 140, //bp reward
    Bp_Task = 141, //bp task
    Bp_Pay = 142, //bp pay
    Bp_PayButton = 143, //bp pay   
    
    
    EnergyTorrent1 = 151, 
    EnergyTorrent2 = 152,   
    EnergyTorrent3 = 153,   
    
    EnergyTorrent4 = 505, 
    EnergyTorrent5 = 506, 
    
    MermaidStart = 160, //美人鱼开始按钮
    MermaidPreview = 161, //美人鱼展示
    MermaidPlay = 162, //美人鱼 PLAY
    MermaidTask = 163, //美人鱼 Task任务介绍
    MermaidInfo= 164, //美人鱼 介绍
    
    ClimbTreeStart = 170, //猴子开始按钮
    ClimbTreePreview= 171, //猴子展示
    ClimbTreeDes= 172, //猴子描述
    ClimbTreePlay= 173, //猴子点击play
    ClimbTreeGameDes= 174, //猴子游戏介绍
    
    RecoverCoinDes=180,//回收金币活动描述
    RecoverCoinStart=181,//回收金币开始按钮
    RecoverCoinTask = 182, //美人鱼 Task任务介绍
    RecoverCoinInfo= 183, //美人鱼 介绍
    RecoverCoinBuy=184,//回收金币购买按钮
    
    CoinLeaderBoardDes=185,//金币排行榜活动描述
    CoinLeaderBoardStart=186,//金币排行榜开始按钮
    CoinLeaderBoardTask= 187, //金币排行榜任务介绍
    CoinLeaderBoardInfo= 188, //金币排行榜局内入口
    
    CoinCompetitionStart = 190, 
    CoinCompetitionPreview = 191,
    CoinCompetitionPlay = 192,
    CoinCompetitionTask = 193, 
    
    
    JumpGridStart = 1910, 
    JumpGridPreview = 1911,
    JumpGridPlay = 1912,
    JumpGridTask = 1913, 
    
    
    SummerWatermelonStart = 200, //夏日西瓜开始按钮
    SummerWatermelonDes=201,//夏日西瓜活动描述
    SummerWatermelonEnterGame=202,//夏日西瓜进入游戏按钮
    SummerWatermelonGameEntrance=203,//夏日西瓜游戏内入口按钮
    SummerWatermelonProgressInfo=204,//夏日西瓜进度条介绍
    SummerWatermelonProductInfo=205,//夏日西瓜棋子产出介绍
    SummerWatermelonUnSetItem=206,//夏日西瓜引导棋子放入棋盘按钮
    
    SummerWatermelonBreadStart = 250, //夏日西瓜开始按钮
    SummerWatermelonBreadDes=251,//夏日西瓜活动描述
    SummerWatermelonBreadEnterGame=252,//夏日西瓜进入游戏按钮
    SummerWatermelonBreadGameEntrance=253,//夏日西瓜游戏内入口按钮
    SummerWatermelonBreadProgressInfo=254,//夏日西瓜进度条介绍
    SummerWatermelonBreadProductInfo=255,//夏日西瓜棋子产出介绍
    SummerWatermelonBreadUnSetItem=256,//夏日西瓜引导棋子放入棋盘按钮
    
    MergeBoardFullGuideBag=401,//棋盘满引导提示背包按钮
    
    SeaRacingHomeEntrance = 410, //海上竞速开始按钮
    SeaRacingStartUIDes=411,//开始弹窗文字介绍
    SeaRacingStartUIBtn=412,//开始弹窗开始按钮
    SeaRacingMainUIDes=413,//主界面文字介绍
    SeaRacingMainUIProgress=414,//主界面节点介绍
    
    HappyGoButton = 600, //点击happy go按钮
    HappyGoPlay = 601, //点击happy go play按钮
    HappyGoInfo = 602, //仓鼠需要物品详情
    HappyGoEat = 603, //给仓鼠食物
    HappyGoCdProduct = 604, //选中cd物品
    HappyGoLevelUp = 605, //LevelUp
    HappyGoLevelUpReward = 606, //LevelUp reward
    
    CardLobbyGuideEntrance = 421, //卡牌大厅引导
    CardPackageEntrance=422,//卡包临时背包
    CardPackageClose=423,//点击关闭卡包界面
    CardMergeEntrance=424,//卡牌局内入口
    CardClickCardBook=425,//主界面点卡册
    CardCardBookReward=426,//卡册奖励
    CardReturnMain=427,//返回主界面
    CardThemeReward=428,//卡册主题奖励
    
    Easter2024GuideStart = 441,//复活节2024引导开始按钮
    Easter2024MainGuideDropBall = 442,//复活节2024主界面引导扔蛋
    Easter2024MainGetScore = 443,//复活节2024主界面获得积分
    Easter2024MainStore = 444,//复活节2024主界面商店入口
    Easter2024MainGuideClose = 445,//复活节2024主界面引导关闭按钮
    Easter2024MergeTask = 446,//复活节2024任务列表
    Easter2024MainGetCard = 447,//复活节2024主界面获得卡
    Easter2024MainGuideSelectCard = 448,//复活节2024主界面引导选择卡
    Easter2024MainSelectCardDescribeExtra = 449,//复活节2024主界面选择乘倍卡描述
    Easter2024MainSelectCardDescribeMulti = 450,//复活节2024主界面选择多球卡描述
    
    DogHopeLeaderBoardHomeEntrance = 451,//狗排行榜大厅入口
    DogHopeLeaderBoardMergeEntrance = 452,//狗排行榜棋盘入口
    DogHopeLeaderBoardMainEntrance = 453,//狗排行榜主界面入口
    ClimbTreeLeaderBoardHomeEntrance = 454,//猴子排行榜大厅入口
    ClimbTreeLeaderBoardMergeEntrance = 455,//猴子排行榜棋盘入口
    ClimbTreeLeaderBoardMainEntrance = 456,//猴子排行榜主界面入口
    DogHopeLeaderBoardDescribe=457,//狗排行榜描述
    ClimbTreeLeaderBoardDescribe=458,//猴子排行榜描述
    
    SnakeLadderGuideStart = 461,//蛇梯子引导开始按钮
    SnakeLadderGuideSpin = 462,//蛇梯子引导转盘按钮
    SnakeLadderAddScore = 463,//蛇梯子加分后文字提示
    SnakeLadderGuideShopEntrance = 464,//蛇梯子引导商店按钮
    SnakeLadderExit = 465,//蛇梯子退出文字提示
    SnakeLadderMerge = 466,//蛇梯子Merge界面提示
    SnakeLadderGetWildCard = 467,//蛇梯子获得wild卡
    SnakeLadderGuideWildCard = 468,//蛇梯子引导使用wild卡
    SnakeLadderUseWildCard = 469,//蛇梯子wild卡使用介绍
    
    ThemeDecorationStart = 471, // 主题装修开始按钮
    ThemeDecorationInfo1= 472, //主题装修介绍
    ThemeDecorationPreview = 473, //主题装修展示
    ThemeDecorationPlay = 474, //主题装修PLAY
    ThemeDecorationTask= 475, //主题装修任务介绍
    ThemeDecorationInfo2= 476, //主题装修介绍
    
    
    StimulateChoseBubble = 700, //选择刺激小游戏挂点
    StimulateChoseOperate = 701, //选择刺激小游戏操作按钮
    StimulateClose = 702, //关闭刺激小游戏
    ScrewTouch = 710, //点击SCREW
    
    SlotMachineAuxItem = 481, // 老虎机大厅入口
    SlotMachineSpin= 482, //老虎机Spin按钮
    SlotMachineInfo = 483, //老虎机结果说明
    SlotMachineReSpin = 484, //老虎机ReSpin按钮
    SlotMachineCollect= 485, //老虎机领取奖励
    
    MergeBoardGuideBag=771,//打开背包
    MergeBoardGuideBagTag=772,//点击页签
    
    MonopolyAuxItem = 491, // 大富翁大厅入口
    MonopolyThrowDice= 492, //大富翁扔骰子
    MonopolyGetScore= 493,//大富翁获得分数
    MonopolyBuyBlock=494,//大富翁购买地块
    MonopolyContinue=495,//大富翁继续扔骰子
    MonopolyTask=496,//大富翁任务介绍
    MonopolyLeaderBoardEntrance=497,//大富翁排行榜入口
    MonopolyLeaderBoardInfo=498,//大富翁排行榜主界面
    MonopolyBet1=499,//大富翁押注1
    MonopolyBet2=500,//大富翁押注2
    
    // KeepPetHomeEntrance=3001,//养狗大厅入口
    // KeepPetInfo1=3002,//养狗主界面玩法介绍
    // KeepPetSearchTask1=3003,//养狗主界面引导巡逻任务
    // KeepPetSearchTask2=3004,//养狗巡逻任务引导开心度巡逻
    // KeepPetSearchTask3=3005,//养狗巡逻状态引导快速结束
    // KeepPetSearchFinish1=3006,//养狗巡逻结束引导点击篮子
    // KeepPetSearchFinish2=3007,//养狗巡逻奖励选取介绍
    // KeepPetSearchFinish3=3008,//养狗巡逻奖励引导领取
    // KeepPetFeed1=3009,//养狗主界面引导喂鸡腿
    // KeepPetFeed2=3010,//养狗鸡腿不足界面引导进入merge
    // KeepPetFeed3=3011,//养狗merge界面引导鸡腿任务
    // KeepPetFeed4=3012,//养狗获得鸡腿后引导进入主界面
    // KeepPetFeed5=3013,//养狗主界面引导喂鸡腿
    // KeepPetFrisbee1=3014,//养狗主界面引导玩飞盘(赠送一个飞盘)
    // KeepPetDailyTask1=3015,//养狗主界面引导打开每日任务
    // KeepPetDailyTask2=3016,//养狗每日任务界面介绍
    // KeepPetPropSearch=3017,//养狗牛排引导
    
    KeepPetEntrance1=3021,//狗入口1,升级或登陆触发
    KeepPetWakeUp=3022,//唤醒狗
    KeepPetFrisbee1=3023,//养狗主界面引导玩飞盘(赠送一个飞盘)
    KeepPetClickExpBar=3024,//点经验条
    KeepPetLevelRewardInfo=3025,//升级奖励描述(文本)
    KeepPetCloseLevelReward=3026,//关闭升级奖励界面
    KeepPetDailyTask1=3027,//点击每日任务图标1
    KeepPetDailyTaskInfo1=3028,//每日任务介绍1(文本)
    KeepPetEntrance2=3029,//狗入口2,转盘够升级时触发
    KeepPetEntrance3=3030,//狗入口3，饥饿状态下登陆时触发
    KeepPetFeed1=3031,//喂鸡腿失败
    KeepPetFeed2=3032,//养狗鸡腿不足界面引导进入merge
    KeepPetFeed3=3033,//养狗merge界面引导鸡腿任务
    KeepPetFeed4=3034,//养狗获得鸡腿后引导进入主界面
    KeepPetFeed5=3035,//养狗主界面引导喂鸡腿
    KeepPetClickExpBar2 = 3044,//升级解锁探索后点经验条
    KeepPetCollectLevelReward=3036,//领奖界面解锁探索等级触发
    KeepPetClickSearchBtn=3037,//搜寻按钮
    KeepPetSearchTask2=3038,//养狗巡逻任务引导狗头巡逻
    KeepPetSearchTask3=3039,//养狗巡逻状态引导快速结束
    KeepPetSearchFinish1=3040,//养狗巡逻结束引导点击篮子
    KeepPetSearchFinish2=3041,//养狗巡逻奖励选取介绍
    KeepPetDailyTask2=3042,//点击每日任务图标2,选物品界面关闭时触发
    KeepPetDailyTaskInfo2=3043,//每日任务介绍2(文本),解锁巡逻任务等级触发
    
    TreasureHuntStart=601, //挖宝开始
    TreasureHuntBreak=602, //引导挖宝
    TreasureHuntDesc=603,  //描述
    
    LuckyGoldenEggStart=611, //金蛋开始
    LuckyGoldenEggBreak=612, //引导金蛋
    LuckyGoldenEggDesc=613,  //金蛋描述
    
    ButterFlyWorkShopStart = 4102, //蝴蝶开始 
    ButterFlyWorkShopNewItem = 4103, //蝴蝶临时背包 
    ButterFlyWorkShopMerge = 4104, //蝴蝶合成
    ButterFlyWorkShopEnterGame = 4105, //蝴蝶进入游戏
    ButterFlyUse = 4106, //蝴蝶使用
    
    GardenTreasureStart = 4201, //花园挖宝开始
    GardenTreasureOpenGrid= 4202, //花园挖宝点击格子 
    
    BlueBlock = 4301,//蓝图小游戏引导
    
    MixMasterEntrance = 4311,//调制大师入口
    MixMasterFormula = 4312,//调制大师配方
    
    StarrySkyCompassEntrance = 4321,//星空罗盘入口
    StarrySkyCompassSpin1 = 4322,//星空罗盘Spin1
    StarrySkyCompassArrow = 4323,//星空罗盘箭头提示
    StarrySkyCompassSpin2 = 4324,//星空罗盘Spin2
    StarrySkyCompassHappy1 = 4325,//星空罗盘开心时刻提示1
    StarrySkyCompassHappy2 = 4326,//星空罗盘开心时刻提示2
    
    TurtlePangEntrance = 4327,//王八入口
    TurtlePangPackage = 4328,//王八选包
    TurtlePangColor = 4329,//王八选色
    TurtlePangPut = 4330,//王八发包
    
    ZumaEntrance = 4331,//祖玛入口
    ZumaStart = 4332,//祖玛开始弹窗
    ZumaHit = 4333,//祖玛关卡1锁头
    ZumaTip = 4334,//祖玛进洞提示
    ZumaChangeBall = 4335,//祖玛换球
    ZumaAddScore = 4336,//祖玛加分
    
    KapibalaEntrance = 4414,//卡皮巴拉入口
    KapiScrewEntrance = 4415,//卡皮钉子入口
    KapiScrewMatchBtn = 4416,//卡皮钉子匹配
    
    ScrewToMerge = 4420, //screw to merge
    
    Farm_Enter = 4431,//农场入口点击
    Farm_TouchGround = 4434,//点击地块
    Farm_BuySeed = 4435,//购买种子
    Farm_Speed = 4437,//加速
    Farm_FinishTask = 4440,//完成任务
    
    DogPlayUnlock = 4443,//玩狗解锁
    DogPlayCollect = 4444,//玩狗领奖
    
    FishCultureEntrance = 4445,//养鱼入口
    FishCultureStart = 4446,//养鱼开始弹窗
    
    KapiTileEntrance = 4447,//卡皮tile入口
    
    PhotoAlbumEntrance = 4448,//相册入口
    PhotoAlbumStart = 4449,//相册开始弹窗
    
    JungleAdventureStart = 4450, //丛林挑战开始按钮
    JungleAdventurePreview= 4451, //丛林挑战展示
    JungleAdventureDes= 4452, //丛林挑战描述
    JungleAdventurePlay= 4453, //丛林挑战点击play
    JungleAdventureGame=4500,//丛林挑战
    
    PhotoAlbumGame=4501,//相册任务栏
    
    BiuBiuStart = 4461, //蝴蝶开始 
    BiuBiuNewItem = 4462, //蝴蝶临时背包 
    BiuBiuEnterGame = 4463, //蝴蝶进入游戏
    BiuBiuUse = 4464, //蝴蝶使用
    
    ParrotStart = 4560, //猴子开始按钮
    ParrotDes= 4561, //猴子描述
    ParrotPlay= 4562, //猴子点击play
    ParrotGameDes= 4563, //猴子游戏介绍
    ParrotLeaderBoardHomeEntrance = 4564,//狗排行榜大厅入口
    ParrotLeaderBoardMainEntrance = 4565,//狗排行榜主界面入口
    ParrotLeaderBoardDescribe=4566,//狗排行榜描述
    
    
    //BalloonRacingStart = 2201, //竞速开始界面
    BalloonRacingItemResource = 2202, //竞速竞速物品资源
    BalloonRacingPlay = 2203, //竞速play按钮
    BalloonRacingSelf = 2204, //竞速竞速自己
    BalloonRacingEnd = 2205, //竞速结束引导
    
    //RabbitRacingStart = 2201, //竞速开始界面
    RabbitRacingItemResource = 2302, //竞速竞速物品资源
    RabbitRacingPlay = 2303, //竞速play按钮
    RabbitRacingSelf = 2304, //竞速竞速自己
    RabbitRacingEnd = 2305, //竞速结束引导
    
    FlowerFieldStart = 4570, //猴子开始按钮
    FlowerFieldDes= 4571, //猴子描述
    FlowerFieldPlay= 4572, //猴子点击play
    FlowerFieldGameDes= 4573, //猴子游戏介绍
    FlowerFieldLeaderBoardHomeEntrance = 4574,//狗排行榜大厅入口
    FlowerFieldLeaderBoardMainEntrance = 4575,//狗排行榜主界面入口
    FlowerFieldLeaderBoardDescribe=4576,//狗排行榜描述
    
    TeamEntrance = 4580,//公会入口
    TeamDesc = 4581,//公会介绍
    TeamTask = 4582,//公会任务介绍
    TeamShopEntrance1 = 4583,//公会商城入口1
    TeamShopEntrance2 = 4584,//公会商城入口2
    TeamCardPackageClick = 4585,//公会卡包点击
    TeamCardPackageDesc = 4586,//公会卡包介绍
    TeamCardPackageOpen = 4587,//公会卡包开启
    TeamLifeDesc = 4588,//公会体力介绍
    
    VipStoreDesc = 4590,//Vip商店介绍
    VipStoreClose = 4591,//Vip商店关闭
    VipStoreHeadIcon = 4592,//点头像
    VipStoreDetailClick = 4593,//Vip详情点击
    VipStoreDetailDesc = 4594,//Vip详情介绍
    
    
    MiniGame_Button = 10001, //minigame 点击按钮
    MiniGame_Bubble = 10002, //minigame 点击气泡
    
    
    PillowWheelStart = 4600, //开始按钮
    PillowWheelDes= 4601, //描述
    PillowWheelPlay= 4602, //点击play
    PillowWheelToMerge= 4603, //点击跳转Merge
    PillowWheelGameDes= 4604, //游戏介绍
    PillowWheelLeaderBoardHomeEntrance = 4605,//排行榜大厅入口
    PillowWheelLeaderBoardMainEntrance = 4606,//排行榜主界面入口
    PillowWheelLeaderBoardDescribe=4607,//排行榜描述
}

// 1.挂点按钮
// 2.主页play
// 3.装修选择按钮
// 4.确认装修按钮
// 5.mergeItem
// targetTypes  触发教程类型
public enum GuideTargetType
{
    None = 0, //null
    PlayButton = 1, // 主页play
    MergeItem = 2, //merge
    ProductBuild = 3, //生产建筑
    Task = 4, //任务
    Asmr = 5, //asmr
    AsmrPlay = 6, //asmrPLAY
    MatchPlay = 7, //match
    
    ClickTaskNeedItem = 10, //点击任务需求物品
    CloseItemInfo = 11, //关闭info信息
    ClickInfoButton = 12, //点击info按钮
    InfoDes = 13, //info描述
    
    UnlockDeco = 20, //解锁装修
    GoDeco = 21, //去装修
    
    TouchBubble = 30, //点击挂点泡泡
    BuyNode = 31, //购买挂点
    ConfirmNode=32,//确认购买
    
    GetReward = 40, //临时物品序列
    ChoseItem = 41, //选中物品
    
    EnterASMR = 50,
    
    AddEnergy = 59, //点击+号增加体力
    FreeEnergy = 60, //免费体力
    
    BoxOpen = 70, //开启箱子
    BoxSpeedUp = 71, //点击箱子加速
    
    EatFood=80, //喂食食物
    ChoseEatCdItem=81,//选择cd需要喂食的建筑
    
    ChoseCdProduct = 100, //选中cd物品
    CdProductSpeedUp = 101, //钻石秒cd
    
    ClickStore = 120, //点击商店
    GetFreeReward = 121, //领取免费物品
    
    ClickTaskCenter = 130, //点击商店
    EnterTaskCenter = 131, //领取免费物品
    
    Bp_Reward = 140, //bp reward
    Bp_Task = 141, //bp task
    Bp_Pay = 142, //bp pay
    Bp_PayButton = 143, //bp pay
    
    EnergyTorrent1 = 151, 
    EnergyTorrent2 = 152,   
    EnergyTorrent3 = 153,   
    
    EnergyTorrent4 = 505, 
    EnergyTorrent5 = 506, 
    
    MermaidStart = 160, //美人鱼开始按钮
    MermaidPreview = 161, //美人鱼展示
    MermaidPlay = 162, //美人鱼 PLAY
    MermaidTask = 163, //美人鱼 Task任务介绍
    MermaidInfo= 164, //美人鱼 介绍
    
    
    ClimbTreeStart = 170, //猴子开始按钮
    ClimbTreePreview= 171, //猴子展示
    ClimbTreeDes= 172, //猴子描述
    ClimbTreePlay= 173, //猴子点击play
    ClimbTreeGameDes= 174, //猴子游戏介绍
    
    RecoverCoinStart=181,//回收金币开始按钮
    
    RecoverCoinTask = 182, //美人鱼 Task任务介绍
    RecoverCoinInfo= 183, //美人鱼 介绍
    RecoverCoinBuy=184,//回收金币购买按钮

    CoinLeaderBoardDes=185,//金币排行榜活动描述
    CoinLeaderBoardStart=186,//金币排行榜开始按钮
    CoinLeaderBoardTask= 187, //金币排行榜任务介绍
    CoinLeaderBoardInfo= 188, //金币排行榜局内入口
    
    CoinCompetitionStart = 190, 
    CoinCompetitionPreview = 191,
    CoinCompetitionPlay = 192,
    CoinCompetitionTask = 193, 
    
    JumpGridStart = 1910, 
    JumpGridPreview = 1911,
    JumpGridPlay = 1912,
    JumpGridTask = 1913, 
    
    SummerWatermelonStart = 200, //夏日西瓜开始按钮
    SummerWatermelonEnterGame=202,//夏日西瓜进入游戏按钮
    SummerWatermelonGameEntrance=203,//夏日西瓜游戏内入口按钮
    SummerWatermelonProgressInfo=204,//夏日西瓜进度条介绍
    SummerWatermelonUnSetItem=206,//夏日西瓜引导棋子放入棋盘按钮
    
    SummerWatermelonBreadStart = 250, //夏日西瓜开始按钮
    SummerWatermelonBreadEnterGame=252,//夏日西瓜进入游戏按钮
    SummerWatermelonBreadGameEntrance=253,//夏日西瓜游戏内入口按钮
    SummerWatermelonBreadProgressInfo=254,//夏日西瓜进度条介绍
    SummerWatermelonBreadUnSetItem=256,//夏日西瓜引导棋子放入棋盘按钮
    
    MergeBoardFullGuideBag=401,//棋盘满引导提示背包按钮
    
    SeaRacingHomeEntrance = 410, //海上竞速开始按钮
    SeaRacingStartUIDes=411,//开始弹窗文字介绍
    SeaRacingStartUIBtn=412,//开始弹窗开始按钮
    SeaRacingMainUIDes=413,//主界面文字介绍
    
    HappyGoButton = 600, //点击happy go按钮
    HappyGoPlay = 601, //点击happy go play按钮
    HappyGoInfo = 602, //仓鼠需要物品详情
    HappyGoEat = 603, //给仓鼠食物
    HappyGoCdProduct = 604, //选中cd物品
    HappyGoLevelUp = 605, //LevelUp
    HappyGoLevelUpReward = 606, //LevelUp reward
    
    CardLobbyGuideEntrance = 421, //卡牌大厅引导
    CardPackageEntrance=422,//卡包临时背包
    // CardPackageClose=423,//点击关闭卡包界面
    CardMergeEntrance=424,//卡牌局内入口
    CardClickCardBook=425,//主界面点卡册
    CardCardBookReward=426,//卡册奖励
    CardReturnMain=427,//返回主界面
    CardThemeReward=428,//卡册主题奖励
    
    Easter2024GuideStart = 441,//复活节2024引导开始按钮
    Easter2024MainGuideDropBall = 442,//复活节2024主界面引导扔蛋
    Easter2024MainGetScore = 443,//复活节2024主界面获得积分
    Easter2024MainStore = 444,//复活节2024主界面商店入口
    Easter2024MainGuideClose = 445,//复活节2024主界面引导关闭按钮
    Easter2024MergeTask = 446,//复活节2024任务列表
    Easter2024MainGetCard = 447,//复活节2024主界面获得卡
    Easter2024MainGuideSelectCard = 448,//复活节2024主界面引导选择卡
    Easter2024MainSelectCardDescribeExtra = 449,//复活节2024主界面选择乘倍卡描述
    Easter2024MainSelectCardDescribeMulti = 450,//复活节2024主界面选择多球卡描述
    
    DogHopeLeaderBoardHomeEntrance = 451,//狗排行榜大厅入口
    DogHopeLeaderBoardMergeEntrance = 452,//狗排行榜棋盘入口
    DogHopeLeaderBoardMainEntrance = 453,//狗排行榜主界面入口
    ClimbTreeLeaderBoardHomeEntrance = 454,//猴子排行榜大厅入口
    ClimbTreeLeaderBoardMergeEntrance = 455,//猴子排行榜棋盘入口
    ClimbTreeLeaderBoardMainEntrance = 456,//猴子排行榜主界面入口
    
    SnakeLadderGuideStart = 461,//蛇梯子引导开始按钮
    SnakeLadderGuideSpin = 462,//蛇梯子引导转盘按钮
    // SnakeLadderAddScore = 463,//蛇梯子加分后文字提示
    SnakeLadderGuideShopEntrance = 464,//蛇梯子引导商店按钮
    // SnakeLadderExit = 465,//蛇梯子退出文字提示
    SnakeLadderMerge = 466,//蛇梯子Merge界面提示
    // SnakeLadderGetWildCard = 467,//蛇梯子获得wild卡
    SnakeLadderGuideWildCard = 468,//蛇梯子引导使用wild卡
    SnakeLadderUseWildCard = 469,//蛇梯子wild卡使用介绍
    
    ThemeDecorationStart = 471, // 主题装修开始按钮
    // ThemeDecorationInfo1= 472, //主题装修介绍
    ThemeDecorationPreview = 473, //主题装修展示
    ThemeDecorationPlay = 474, //主题装修PLAY
    ThemeDecorationTask= 475, //主题装修任务介绍
    ThemeDecorationInfo2= 476, //主题装修介绍
    
    
    StimulateChoseBubble = 700, //选择刺激小游戏挂点
    StimulateChoseOperate = 701, //选择刺激小游戏操作按钮
    StimulateClose = 702, //关闭刺激小游戏
    ScrewTouch = 710, //点击SCREW
    
    SlotMachineAuxItem = 481, // 老虎机大厅入口
    SlotMachineSpin= 482, //老虎机Spin按钮
    // SlotMachineInfo = 483, //老虎机结果说明
    SlotMachineReSpin = 484, //老虎机ReSpin按钮
    SlotMachineCollect= 485, //老虎机领取奖励
    
    MergeBoardGuideBag=771,//打开背包
    MergeBoardGuideBagTag=772,//点击页签
    
    MonopolyAuxItem = 491, // 大富翁大厅入口
    MonopolyThrowDice= 492, //大富翁扔骰子
    // MonopolyGetScore= 493,//大富翁获得分数
    MonopolyBuyBlock=494,//大富翁购买地块
    // MonopolyContinue=495,//大富翁继续扔骰子
    MonopolyTask=496,//大富翁任务介绍
    MonopolyLeaderBoardEntrance=497,//大富翁排行榜入口
    // MonopolyLeaderBoardInfo=498,//大富翁排行榜主界面
    MonopolyBet1=499,//大富翁押注1
    MonopolyBet2=500,//大富翁押注2
    
    // KeepPetHomeEntrance=3001,//养狗大厅入口
    // KeepPetInfo1=3002,//养狗主界面玩法介绍
    // KeepPetSearchTask1=3003,//养狗主界面引导巡逻任务
    // KeepPetSearchTask2=3004,//养狗巡逻任务引导开心度巡逻
    // KeepPetSearchTask3=3005,//养狗巡逻状态引导快速结束
    // KeepPetSearchFinish1=3006,//养狗巡逻结束引导点击篮子
    // KeepPetSearchFinish2=3007,//养狗巡逻奖励选取介绍
    // KeepPetSearchFinish3=3008,//养狗巡逻奖励引导领取
    // KeepPetFeed1=3009,//养狗主界面引导喂鸡腿
    // KeepPetFeed2=3010,//养狗鸡腿不足界面引导进入merge
    // KeepPetFeed3=3011,//养狗merge界面引导鸡腿任务
    // KeepPetFeed4=3012,//养狗获得鸡腿后引导进入主界面
    // KeepPetFeed5=3013,//养狗主界面引导喂鸡腿
    // KeepPetFrisbee1=3014,//养狗主界面引导玩飞盘(赠送一个飞盘)
    // KeepPetDailyTask1=3015,//养狗主界面引导打开每日任务
    // KeepPetDailyTask2=3016,//养狗每日任务界面介绍
    // KeepPetPropSearch=3017,//养狗牛排引导
    
    
    KeepPetEntrance1=3021,//狗入口1,升级或登陆触发
    KeepPetWakeUp=3022,//唤醒狗
    KeepPetFrisbee1=3023,//养狗主界面引导玩飞盘(赠送一个飞盘)
    KeepPetClickExpBar=3024,//点经验条
    // KeepPetLevelRewardInfo=3025,//升级奖励描述(文本)
    KeepPetCloseLevelReward=3026,//关闭升级奖励界面
    KeepPetDailyTask1=3027,//点击每日任务图标1
    // KeepPetDailyTaskInfo1=3028,//每日任务介绍1(文本)
    KeepPetEntrance2=3029,//狗入口2,转盘够升级时触发
    KeepPetEntrance3=3030,//狗入口3，饥饿状态下登陆时触发
    KeepPetFeed1=3031,//喂鸡腿失败
    KeepPetFeed2=3032,//养狗鸡腿不足界面引导进入merge
    KeepPetFeed3=3033,//养狗merge界面引导鸡腿任务
    KeepPetFeed4=3034,//养狗获得鸡腿后引导进入主界面
    KeepPetFeed5=3035,//养狗主界面引导喂鸡腿
    KeepPetClickExpBar2 = 3044,//升级解锁探索后点经验条
    KeepPetCollectLevelReward=3036,//领奖界面解锁探索等级触发
    KeepPetClickSearchBtn=3037,//搜寻按钮
    KeepPetSearchTask2=3038,//养狗巡逻任务引导狗头巡逻
    KeepPetSearchTask3=3039,//养狗巡逻状态引导快速结束
    KeepPetSearchFinish1=3040,//养狗巡逻结束引导点击篮子
    // KeepPetSearchFinish2=3041,//养狗巡逻奖励选取介绍
    KeepPetDailyTask2=3042,//点击每日任务图标2,选物品界面关闭时触发
    // KeepPetDailyTaskInfo2=3043,//每日任务介绍2(文本),解锁巡逻任务等级触发
    
    TreasureHuntStart=601, //挖宝开始
    TreasureHuntBreak=602, //引导挖宝
    TreasureHuntDesc=603,  //描述
    
    
    LuckyGoldenEggStart=611, //金蛋开始
    LuckyGoldenEggBreak=612, //引导金蛋
    LuckyGoldenEggDesc=613,  //金蛋描述
    
    
    ButterFlyWorkShopStart = 4102, //蝴蝶开始 
    ButterFlyWorkShopNewItem = 4103, //蝴蝶临时背包 
    ButterFlyWorkShopMerge = 4104, //蝴蝶合成
    ButterFlyWorkShopEnterGame = 4105, //蝴蝶进入游戏
    ButterFlyUse = 4106, //蝴蝶使用
    
    
    GardenTreasureStart = 4201, //花园挖宝开始
    GardenTreasureOpenGrid= 4202, //花园挖宝点击格子 
    
    BlueBlock = 4301,//蓝图小游戏引导
    
    MixMasterEntrance = 4311,//调制大师入口
    MixMasterFormula = 4312,//调制大师配方
    
    StarrySkyCompassEntrance = 4321,//星空罗盘入口
    StarrySkyCompassSpin1 = 4322,//星空罗盘Spin1
    StarrySkyCompassArrow = 4323,//星空罗盘箭头提示
    StarrySkyCompassSpin2 = 4324,//星空罗盘Spin2
    StarrySkyCompassHappy1 = 4325,//星空罗盘开心时刻提示1
    StarrySkyCompassHappy2 = 4326,//星空罗盘开心时刻提示2
    
    TurtlePangEntrance = 4327,//王八入口
    TurtlePangPackage = 4328,//王八选包
    TurtlePangColor = 4329,//王八选色
    TurtlePangPut = 4330,//王八发包
    
    ZumaEntrance = 4331,//祖玛入口
    ZumaStart = 4332,//祖玛开始弹窗
    ZumaHit = 4333,//祖玛关卡1锁头
    ZumaTip = 4334,//祖玛进洞提示
    ZumaChangeBall = 4335,//祖玛换球
    ZumaAddScore = 4336,//祖玛加分
    
    KapibalaEntrance = 4414,//卡皮巴拉入口
    KapiScrewEntrance = 4415,//卡皮钉子入口
    KapiScrewMatchBtn = 4416,//卡皮钉子匹配
    
    ScrewToMerge = 4420, //screw to merge
    
    Farm_Enter = 4431,//农场入口点击
    Farm_TouchGround = 4434,//点击地块
    Farm_BuySeed = 4435,//购买种子
    Farm_Speed = 4437,//加速
    Farm_FinishTask = 4440,//完成任务
    
    DogPlayUnlock = 4443,//玩狗解锁
    DogPlayCollect = 4444,//玩狗领奖
    
    FishCultureEntrance = 4445,//养鱼入口
    FishCultureStart = 4446,//养鱼开始弹窗
    
    KapiTileEntrance = 4447,//卡皮tile入口
    
    PhotoAlbumEntrance = 4448,//相册入口
    PhotoAlbumStart = 4449,//相册开始弹窗
    
    JungleAdventureStart = 4450, //丛林挑战开始按钮
    JungleAdventurePreview= 4451, //丛林挑战展示
    JungleAdventureDes= 4452, //丛林挑战描述
    JungleAdventurePlay= 4453, //丛林挑战点击play
    JungleAdventureGame=4500,//丛林挑战
    
    PhotoAlbumGame=4501,//相册任务栏
    
    BiuBiuStart = 4461, //蝴蝶开始 
    BiuBiuNewItem = 4462, //蝴蝶临时背包 
    BiuBiuEnterGame = 4463, //蝴蝶进入游戏
    BiuBiuUse = 4464, //蝴蝶使用
    
    ParrotStart = 4560, //猴子开始按钮
    ParrotPlay= 4562, //猴子点击play
    ParrotGameDes= 4563, //猴子游戏介绍
    ParrotLeaderBoardHomeEntrance = 4564,//狗排行榜大厅入口
    ParrotLeaderBoardMainEntrance = 4565,//狗排行榜主界面入口
    
    //BalloonRacingStart = 2201, //竞速开始界面
    BalloonRacingItemResource = 2202, //竞速竞速物品资源
    BalloonRacingPlay = 2203, //竞速play按钮
    BalloonRacingSelf = 2204, //竞速竞速自己
    BalloonRacingEnd = 2205, //竞速结束引导
    
    
    //RabbitRacingStart = 2201, //竞速开始界面
    RabbitRacingItemResource = 2302, //竞速竞速物品资源
    RabbitRacingPlay = 2303, //竞速play按钮
    RabbitRacingSelf = 2304, //竞速竞速自己
    RabbitRacingEnd = 2305, //竞速结束引导
    
    FlowerFieldStart = 4570, //猴子开始按钮
    FlowerFieldPlay= 4572, //猴子点击play
    FlowerFieldGameDes= 4573, //猴子游戏介绍
    FlowerFieldLeaderBoardHomeEntrance = 4574,//狗排行榜大厅入口
    FlowerFieldLeaderBoardMainEntrance = 4575,//狗排行榜主界面入口
    
    TeamEntrance = 4580,//公会入口
    TeamDesc = 4581,//公会介绍
    TeamTask = 4582,//公会任务介绍
    TeamShopEntrance1 = 4583,//公会商城入口1
    TeamShopEntrance2 = 4584,//公会商城入口2
    TeamCardPackageClick = 4585,//公会卡包点击
    TeamCardPackageDesc = 4586,//公会卡包介绍
    TeamCardPackageOpen = 4587,//公会卡包开启
    TeamLifeDesc = 4588,//公会体力介绍
    
    VipStoreDesc = 4590,//Vip商店介绍
    VipStoreClose = 4591,//Vip商店关闭
    VipStoreHeadIcon = 4592,//点头像
    VipStoreDetailClick = 4593,//Vip详情点击
    VipStoreDetailDesc = 4594,//Vip详情介绍
    
    
    MiniGame_Button = 10001, //minigame 点击按钮
    MiniGame_Bubble = 10002, //minigame 点击气泡
    
    
    PillowWheelStart = 4600, //猴子开始按钮
    // PillowWheelDes= 4601, //猴子描述
    PillowWheelPlay= 4602, //猴子点击play
    PillowWheelToMerge= 4603, //猴子点击跳转Merge
    PillowWheelGameDes= 4604, //猴子游戏介绍
    PillowWheelLeaderBoardHomeEntrance = 4605,//狗排行榜大厅入口
    PillowWheelLeaderBoardMainEntrance = 4606,//狗排行榜主界面入口
    // PillowWheelLeaderBoardDescribe=4607,//狗排行榜描述
}