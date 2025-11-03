/************************************************
 * Config class : TableGuide3
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableGuide3 : TableBase
{   
    
    // ID
    public int id ;
    
    // 优先级; ; 优先级越小 越早执行
    public int priority ;
    
    // 引导组; 只要触发带组的 必须完成组内的 才可以 继续触发其他
    public int group ;
    
    // 引导类型; 0或NULL 主线; 2 HAPPY GO; 3 STIMULATE; 4 BUTTERFLY; 5 FILYTH
    public int guideType ;
    
    // 是否为弱引导
    public bool isWeak ;
    
    // 是否存盘; (连续步骤中，; 最后一步存盘)
    public bool saveFlag ;
    
    // 触发位置（与代码GUIDETRIGGER定义一致）;     GUIDEEND = 1, //引导结束;     STORYEND = 2, //剧情结束;     ENTERMERGE = 3, //进入游戏;     MERGEFINISH = 4, //MERGE完成;     PRODUCTFINISH = 5, //生产;     TASKFINISH = 6, //引导任务;     CLICKTASKNEEDITEM = 10, //点击任务需求物品;     CLOSEITEMINFO = 11, //关闭INFO信息;     CLICKINFOBUTTON = 12, //点击INFO按钮;     INFODES = 13, //INFO介绍; ASMRPLAY=15,//ASMR点击PLAY; ;    ;     UNLOCKDECO = 20, //解锁装修;     GODECO = 21, //去装修; ;    TOUCHBUBBLE = 30, //点击挂点泡泡;     BUYNODE = 31, //购买挂点; CONFIRMNODE=32//确认购买; ; ;     GETREWARD = 40, //临时物品序列;     CHOSEITEM = 41, //选中物品; ; ENTERHOME = 50, //进入HOME; ENTERASMR = 51, //进入ASMR; ; ADDENERGY=59//没有体力点击加号; FREEENERGY=60 //免费体力; ; ;     BOXOPEN = 70, //开启箱子;     BOXSPEEDUP = 71, //点击箱子加速; ; EATFOOD=80, //喂食食物; 　CHOSEEATCDITEM=81,//选择CD需要喂食的建筑; ;  CHOSECDPRODUCT = 100, //选中CD物品;     CDPRODUCTSPEEDUP = 101, //钻石秒CD; ; CLICKSTORE = 120, //点击商店;     GETFREEREWARD = 121, //领取免费物品; ; ;     CLICKTASKCENTER = 130, //点击商店;     ENTERTASKCENTER = 131, //领取免费物品; ;     BP_REWARD = 140, //BP REWARD;     BP_TASK = 141, //BP TASK;     BP_PAY = 142, //BP PAY;     BP_PAYBUTTON = 143, //BP PAY; ; ; MERMAIDSTART = 160, // 美人鱼开始按钮; MERMAIDPREVIEW = 161, //美人鱼展示; MERMAIDPLAY = 162, //美人鱼 PLAY; MERMAIDTASK= 163, //美人鱼 任务介绍; MERMAIDINFO= 164, //美人鱼 介绍; ; ; CLIMBTREESTART = 170, //猴子开始按钮; CLIMBTREEPREVIEW= 171, //猴子展示; CLIMBTREEDES= 172, //猴子描述; CLIMBTREEPLAY= 173, //猴子点击PLAY; CLIMBTREEGAMEDES= 174, //猴子游戏介绍; ; RECOVERCOINDES=180,//回收金币活动描述; RECOVERCOINSTART=181,//回收金币开始按钮; RECOVERCOINTASK= 182, //回收金币任务介绍; RECOVERCOININFO= 183, //回收金币局内入口; RECOVERCOINBUY= 184, //回收金币购买界面; ; COINLEADERBOARDDES=185,//金币排行榜活动描述; COINLEADERBOARDSTART=186,//金币排行榜开始按钮; COINLEADERBOARDTASK= 187, //金币排行榜任务介绍; COINLEADERBOARDINFO= 188, //金币排行榜局内入口; ; SUMMERWATERMELONSTART = 200, //夏日西瓜开始按钮; SUMMERWATERMELONDES=201,//夏日西瓜活动描述; SUMMERWATERMELONENTERGAME=202,//夏日西瓜进入游戏按钮; SUMMERWATERMELONGAMEENTRANCE=203,//夏日西瓜游戏内入口按钮; SUMMERWATERMELONPROGRESSINFO=204,//夏日西瓜进度条介绍; SUMMERWATERMELONPRODUCTINFO=205,//夏日西瓜棋子产出介绍; SUMMERWATERMELONUNSETITEM=206,//夏日西瓜引导棋子放入棋盘按钮; ; SUMMERWATERMELONBREADSTART = 250, //夏日西瓜开始按钮; SUMMERWATERMELONBREADDES=251,//夏日西瓜活动描述; SUMMERWATERMELONBREADENTERGAME=252,//夏日西瓜进入游戏按钮; SUMMERWATERMELONBREADGAMEENTRANCE=253,//夏日西瓜游戏内入口按钮; SUMMERWATERMELONBREADPROGRESSINFO=254,//夏日西瓜进度条介绍; SUMMERWATERMELONBREADPRODUCTINFO=255,//夏日西瓜棋子产出介绍; SUMMERWATERMELONBREADUNSETITEM=256,//夏日西瓜引导棋子放入棋盘按钮; ; ; DIGTRENCHSAVEFISH=301,//挖沟救鱼; DIGTRENCHSIDEQUEST=302,//挖沟走岔路; DIGTRENCHGETHAMMER=311,//挖沟获得锤子; DIGTRENCHGETRAKE=312,//挖沟获得锤子; DIGTRENCHDESTROYSTONE=321,//挖沟破坏石头; DIGTRENCHDESTROYSTACK=322,//挖沟破坏草垛; DIGTRENCHSTONEFAILED=331,//挖沟石头失败; DIGTRENCHSTACKFAILED=332,//挖沟草垛失败; ; MERGEBOARDFULLGUIDEBAG=401,//棋盘满引导提示背包按钮; ; SEARACINGHOMEENTRANCE = 410, //海上竞速开始按钮; SEARACINGSTARTUIDES=411,//开始弹窗文字介绍; SEARACINGSTARTUIBTN=412,//开始弹窗开始按钮; SEARACINGMAINUIDES=413,//主界面文字介绍; SEARACINGMAINUIPROGRESS=414,//主界面节点介绍; SEARACINGMAINUIREWARD=415,//主界面奖励介绍; ; ;      HAPPYGOBUTTON = 600, //点击HAPPY GO按钮;     HAPPYGOPLAY = 601, //点击HAPPY GO PLAY按钮;     HAPPYGOINFO = 602, //仓鼠需要物品详情;     HAPPYGOEAT = 603, //给仓鼠食物;     HAPPYGOCDPRODUCT = 604, //选中CD物品;     HAPPYGOLEVELUP = 605, //LEVELUP;     HAPPYGOLEVELUPREWARD = 606, //LEVELUP REWARD; ; CARDLOBBYGUIDEENTRANCE = 421, //卡牌大厅引导; CARDPACKAGEENTRANCE=422,//卡包临时背包; CARDPACKAGECLOSE=423,//点击关闭卡包界面; CARDMERGEENTRANCE=424,//卡牌局内入口; CARDCLICKCARDBOOK=425,//主界面点卡册; CARDCARDBOOKREWARD=426,//卡册奖励; CARDRETURNMAIN=427,//返回主界面; CARDTHEMEREWARD=428,//卡册主题奖励; ; EASTER2024GUIDESTART = 441,//复活节2024引导开始按钮; EASTER2024MAINGUIDEDROPBALL = 442,//复活节2024主界面引导扔蛋; EASTER2024MAINGETSCORE = 443,//复活节2024主界面获得积分; EASTER2024MAINSTORE = 444,//复活节2024主界面商店入口; EASTER2024MAINGUIDECLOSE = 445,//复活节2024主界面引导关闭按钮; EASTER2024MERGETASK = 446,//复活节2024任务列表; EASTER2024MAINGETCARD = 447,//复活节2024主界面获得卡; EASTER2024MAINGUIDESELECTCARD = 448,//复活节2024主界面引导选择卡; EASTER2024MAINSELECTCARDDESCRIBEEXTRA = 449,//复活节2024主界面选择多球卡描述; EASTER2024MAINSELECTCARDDESCRIBEMULTI = 450,//复活节2024主界面选择乘倍卡描述; ; DOGHOPELEADERBOARDHOMEENTRANCE = 451,//狗排行榜大厅入口; DOGHOPELEADERBOARDMERGEENTRANCE = 452,//狗排行榜棋盘入口; DOGHOPELEADERBOARDMAINENTRANCE = 453,//狗排行榜主界面入口; CLIMBTREELEADERBOARDHOMEENTRANCE = 454,//猴子排行榜大厅入口; CLIMBTREELEADERBOARDMERGEENTRANCE = 455,//猴子排行榜棋盘入口; CLIMBTREELEADERBOARDMAINENTRANCE = 456,//猴子排行榜主界面入口; DOGHOPELEADERBOARDDESCRIBE=457,//狗排行榜描述; CLIMBTREELEADERBOARDDESCRIBE=458,//猴子排行榜描述; ; SNAKELADDERGUIDESTART = 461,//蛇梯子引导开始按钮; SNAKELADDERGUIDESPIN = 462,//蛇梯子引导转盘按钮; SNAKELADDERADDSCORE = 463,//蛇梯子加分后文字提示; SNAKELADDERGUIDESHOPENTRANCE = 464,//蛇梯子引导商店按钮; SNAKELADDEREXIT = 465,//蛇梯子退出文字提示; SNAKELADDERMERGE = 466,//蛇梯子MERGE界面提示; SNAKELADDERGETWILDCARD = 467,//蛇梯子获得WILD卡; SNAKELADDERGUIDEWILDCARD = 468,//蛇梯子引导使用WILD卡; SNAKELADDERUSEWILDCARD = 469,//蛇梯子WILD卡使用介绍; ; THEMEDECORATIONSTART = 471, // 主题装修开始按钮; THEMEDECORATIONINFO1= 472, //主题装修介绍; THEMEDECORATIONPREVIEW = 473, //主题装修展示; THEMEDECORATIONPLAY = 474, //主题装修PLAY; THEMEDECORATIONTASK= 475, //主题装修任务介绍; THEMEDECORATIONINFO2= 476, //主题装修介绍; ; STIMULATECHOSEBUBBLE = 700 //选择刺激小游戏挂点; STIMULATECHOSEOPERATE = 701 //选择刺激小游戏操作按钮; STIMULATECLOSE = 702 //关闭刺激小游戏; SCREWTOUCH = 710 //点击SCREW; ; SLOTMACHINEAUXITEM = 481, // 老虎机大厅入口; SLOTMACHINESPIN= 482, //老虎机SPIN按钮; SLOTMACHINEINFO = 483, //老虎机结果说明; SLOTMACHINERESPIN = 484, //老虎机RESPIN按钮; SLOTMACHINECOLLECT= 485, //老虎机领取奖励; ; MONOPOLYAUXITEM = 491, // 大富翁大厅入口; MONOPOLYTHROWDICE= 492, //大富翁扔骰子; MONOPOLYGETSCORE= 493,//大富翁获得分数; MONOPOLYBUYBLOCK=494,//大富翁购买地块; MONOPOLYCONTINUE=495,//大富翁继续扔骰子; MONOPOLYTASK=496,//大富翁任务介绍; MONOPOLYLEADERBOARDENTRANCE=497,//大富翁排行榜入口; MONOPOLYLEADERBOARDINFO=498,//大富翁排行榜主界面; ; KEEPPETHOMEENTRANCE=3001,//养狗大厅入口; KEEPPETINFO1=3002,//养狗主界面玩法介绍; KEEPPETSEARCHTASK1=3003,//养狗主界面引导巡逻任务; KEEPPETSEARCHTASK2=3004,//养狗巡逻任务引导开心度巡逻; KEEPPETSEARCHTASK3=3005,//养狗巡逻状态引导快速结束; KEEPPETSEARCHFINISH1=3006,//养狗巡逻结束引导点击篮子; KEEPPETSEARCHFINISH2=3007,//养狗巡逻奖励选取介绍; KEEPPETSEARCHFINISH3=3008,//养狗巡逻奖励引导领取; KEEPPETFEED1=3009,//养狗主界面引导喂鸡腿; KEEPPETFEED2=3010,//养狗鸡腿不足界面引导进入MERGE; KEEPPETFEED3=3011,//养狗MERGE界面引导鸡腿任务; KEEPPETFEED4=3012,//养狗获得鸡腿后引导进入主界面; KEEPPETFEED5=3013,//养狗主界面引导喂鸡腿; KEEPPETFRISBEE1=3014,//养狗主界面引导玩飞盘(赠送一个飞盘); KEEPPETDAILYTASK1=3015,//养狗主界面引导打开每日任务; KEEPPETDAILYTASK2=3016,//养狗每日任务界面介绍; KEEPPETPROPSEARCH=3017,//养狗牛排引导
    public int triggerPosition ;
    
    // 点击目标类型（与代码GUIDETARGETTYPE一致）; 0 占位; 1 PLAY按钮; 2 MERGEITEM; 3 生产建筑; 4 任务; 5 ASMR 按钮; 6 ASMRPLAY 按钮; 7 TMATCH; ; 10 点击任务要的物品 出现INFO; 11 关闭INFO界面; ; 20 解锁装修任务; 21 点击去装修; ;    30 点击泡泡; 31 购买挂点; 32 确认装修; ; 50 进入ASMR; ; 40 临时物品序列; 41 选中物品; ; 59 没有体力点击加号; 60 免费体力; ; EATFOOD=80, //喂食食物; 　CHOSEEATCDITEM=81,//选择CD需要喂食的建筑; ; 100 选择CD物品; 101 点击钻石加速按钮; ; 120 点击商店; 121 领取免费物品; ; 130 点击任务中心; 131 任务中心描述; ; ;     BP_REWARD = 140, //BP REWARD;     BP_TASK = 141, //BP TASK;     BP_PAY = 142, //BP PAY;     BP_PAYBUTTON = 143, //BP PAY; ; MERMAIDSTART = 160, //美人女开始按钮; MERMAIDPREVIEW = 161, //美人女展示; MERMAIDPLAY = 162, //美人鱼 PLAY; MERMAIDTASK= 163, //美人鱼 任务介绍; MERMAIDINFO= 164, //美人鱼 介绍; ; CLIMBTREESTART = 170, //猴子开始按钮; CLIMBTREEPREVIEW= 171, //猴子展示; CLIMBTREEDES= 172, //猴子描述; CLIMBTREEPLAY= 173, //猴子点击PLAY; CLIMBTREEGAMEDES= 174, //猴子游戏介绍; ; RECOVERCOINDES=180,//回收金币活动描述; RECOVERCOINSTART=181,//回收金币开始按钮; RECOVERCOINTASK= 182, //回收金币任务介绍; RECOVERCOININFO= 183, //回收金币局内入口; RECOVERCOINBUY= 184, //回收金币购买界面; ; COINLEADERBOARDDES=185,//金币排行榜活动描述; COINLEADERBOARDSTART=186,//金币排行榜开始按钮; COINLEADERBOARDTASK= 187, //金币排行榜任务介绍; COINLEADERBOARDINFO= 188, //金币排行榜局内入口; ; SUMMERWATERMELONSTART = 200, //夏日西瓜开始按钮; SUMMERWATERMELONENTERGAME=202,//夏日西瓜进入游戏按钮; SUMMERWATERMELONGAMEENTRANCE=203,//夏日西瓜游戏内入口按钮; SUMMERWATERMELONPROGRESSINFO=204,//夏日西瓜进度条介绍; SUMMERWATERMELONUNSETITEM=206,//夏日西瓜引导棋子放入棋盘按钮; ; SUMMERWATERMELONBREADSTART = 250, //夏日西瓜开始按钮; SUMMERWATERMELONBREADENTERGAME=252,//夏日西瓜进入游戏按钮; SUMMERWATERMELONBREADGAMEENTRANCE=253,//夏日西瓜游戏内入口按钮; SUMMERWATERMELONBREADPROGRESSINFO=254,//夏日西瓜进度条介绍; SUMMERWATERMELONBREADUNSETITEM=256,//夏日西瓜引导棋子放入棋盘按钮; ; DIGTRENCHSAVEFISH=210,//挖沟救鱼; DIGTRENCHGETPROPS1=211,//挖沟获得锤子; DIGTRENCHGETPSOPS2=212,//挖沟获得爬犁; DIGTRENCHSIDEQUEST=213,//挖沟走岔路; ; MERGEBOARDFULLGUIDEBAG=401,//棋盘满引导提示背包按钮; ; SEARACINGHOMEENTRANCE = 410, //海上竞速开始按钮; SEARACINGSTARTUIDES=411,//开始弹窗文字介绍; SEARACINGSTARTUIBTN=412,//开始弹窗开始按钮; SEARACINGMAINUIDES=413,//主界面文字介绍; SEARACINGMAINUIPROGRESS=414,//主界面节点介绍; SEARACINGMAINUIREWARD=415,//主界面奖励介绍; ;   HAPPYGOBUTTON = 600, //点击HAPPY GO按钮;     HAPPYGOPLAY = 601, //点击HAPPY GO PLAY按钮;     HAPPYGOINFO = 602, //仓鼠需要物品详情;     HAPPYGOEAT = 603, //给仓鼠食物;     HAPPYGOCDPRODUCT = 604, //选中CD物品;     HAPPYGOLEVELUP = 605, //LEVELUP;     HAPPYGOLEVELUPREWARD = 606, //LEVELUP REWARD; ; CARDLOBBYGUIDEENTRANCE = 421, //卡牌大厅引导; CARDPACKAGEENTRANCE=422,//卡包临时背包; CARDPACKAGECLOSE=423,//点击关闭卡包界面; CARDMERGEENTRANCE=424,//卡牌局内入口; CARDCLICKCARDBOOK=425,//主界面点卡册; CARDCARDBOOKREWARD=426,//卡册奖励; CARDRETURNMAIN=427,//返回主界面; CARDTHEMEREWARD=428,//卡册主题奖励; ; EASTER2024GUIDESTART = 441,//复活节2024引导开始按钮; EASTER2024MAINGUIDEDROPBALL = 442,//复活节2024主界面引导扔蛋; EASTER2024MAINGETSCORE = 443,//复活节2024主界面获得积分; EASTER2024MAINSTORE = 444,//复活节2024主界面商店入口; EASTER2024MAINGUIDECLOSE = 445,//复活节2024主界面引导关闭按钮; EASTER2024MERGETASK = 446,//复活节2024任务列表; EASTER2024MAINGETCARD = 447,//复活节2024主界面获得卡; EASTER2024MAINGUIDESELECTCARD = 448,//复活节2024主界面引导选择卡; EASTER2024MAINSELECTCARDDESCRIBEEXTRA = 449,//复活节2024主界面选择乘倍卡描述; EASTER2024MAINSELECTCARDDESCRIBEMULTI = 450,//复活节2024主界面选择多球卡描述; ; DOGHOPELEADERBOARDHOMEENTRANCE = 451,//狗排行榜大厅入口; DOGHOPELEADERBOARDMERGEENTRANCE = 452,//狗排行榜棋盘入口; DOGHOPELEADERBOARDMAINENTRANCE = 453,//狗排行榜主界面入口; CLIMBTREELEADERBOARDHOMEENTRANCE = 454,//猴子排行榜大厅入口; CLIMBTREELEADERBOARDMERGEENTRANCE = 455,//猴子排行榜棋盘入口; CLIMBTREELEADERBOARDMAINENTRANCE = 456,//猴子排行榜主界面入口; DOGHOPELEADERBOARDDESCRIBE=457,//狗排行榜描述; CLIMBTREELEADERBOARDDESCRIBE=458,//猴子排行榜描述; ; SNAKELADDERGUIDESTART = 461,//蛇梯子引导开始按钮; SNAKELADDERGUIDESPIN = 462,//蛇梯子引导转盘按钮; SNAKELADDERADDSCORE = 463,//蛇梯子加分后文字提示; SNAKELADDERGUIDESHOPENTRANCE = 464,//蛇梯子引导商店按钮; SNAKELADDEREXIT = 465,//蛇梯子退出文字提示; SNAKELADDERMERGE = 466,//蛇梯子MERGE界面提示; SNAKELADDERGETWILDCARD = 467,//蛇梯子获得WILD卡; SNAKELADDERGUIDEWILDCARD = 468,//蛇梯子引导使用WILD卡; SNAKELADDERUSEWILDCARD = 469,//蛇梯子WILD卡使用介绍; ; THEMEDECORATIONSTART = 471, // 主题装修开始按钮; THEMEDECORATIONINFO1= 472, //主题装修介绍; THEMEDECORATIONPREVIEW = 473, //主题装修展示; THEMEDECORATIONPLAY = 474, //主题装修PLAY; THEMEDECORATIONTASK= 475, //主题装修任务介绍; THEMEDECORATIONINFO2= 476, //主题装修介绍; ; ; STIMULATECHOSEBUBBLE = 700 //选择刺激小游戏挂点; STIMULATECHOSEOPERATE = 701 //选择刺激小游戏操作按钮; STIMULATECLOSE = 702 //关闭刺激小游戏; ; SLOTMACHINEAUXITEM = 481, // 老虎机大厅入口; SLOTMACHINESPIN= 482, //老虎机SPIN按钮; SLOTMACHINEINFO = 483, //老虎机结果说明; SLOTMACHINERESPIN = 484, //老虎机RESPIN按钮; SLOTMACHINECOLLECT= 485, //老虎机领取奖励; ; MONOPOLYAUXITEM = 491, // 大富翁大厅入口; MONOPOLYTHROWDICE= 492, //大富翁扔骰子; MONOPOLYGETSCORE= 493,//大富翁获得分数; MONOPOLYBUYBLOCK=494,//大富翁购买地块; MONOPOLYCONTINUE=495,//大富翁继续扔骰子; MONOPOLYTASK=496,//大富翁任务介绍; MONOPOLYLEADERBOARDENTRANCE=497,//大富翁排行榜入口; MONOPOLYLEADERBOARDINFO=498,//大富翁排行榜主界面; ; KEEPPETHOMEENTRANCE=3001,//养狗大厅入口; KEEPPETINFO1=3002,//养狗主界面玩法介绍; KEEPPETSEARCHTASK1=3003,//养狗主界面引导巡逻任务; KEEPPETSEARCHTASK2=3004,//养狗巡逻任务引导开心度巡逻; KEEPPETSEARCHTASK3=3005,//养狗巡逻状态引导快速结束; KEEPPETSEARCHFINISH1=3006,//养狗巡逻结束引导点击篮子; KEEPPETSEARCHFINISH2=3007,//养狗巡逻奖励选取介绍; KEEPPETSEARCHFINISH3=3008,//养狗巡逻奖励引导领取; KEEPPETFEED1=3009,//养狗主界面引导喂鸡腿; KEEPPETFEED2=3010,//养狗鸡腿不足界面引导进入MERGE; KEEPPETFEED3=3011,//养狗MERGE界面引导鸡腿任务; KEEPPETFEED4=3012,//养狗获得鸡腿后引导进入主界面; KEEPPETFEED5=3013,//养狗主界面引导喂鸡腿; KEEPPETFRISBEE1=3014,//养狗主界面引导玩飞盘(赠送一个飞盘); KEEPPETDAILYTASK1=3015,//养狗主界面引导打开每日任务; KEEPPETDAILYTASK2=3016,//养狗每日任务界面介绍; KEEPPETPROPSEARCH=3017,//养狗牛排引导
    public int targetType ;
    
    // 自动设置INDEX
    public bool autoSetIndex ;
    
    // 开始合成INDEX
    public int mergeStartIndex ;
    
    // 结束合成引导
    public int mergeEndIndex ;
    
    // 是否有空格子
    public bool checkEmptyGrid ;
    
    // 触发参数
    public string triggerParam ;
    
    // 完成任务的参数
    public string finishParam ;
    
    // 保存教程ID
    public int[] saveGuideIds ;
    
    // 自动执行任务ID
    public int nextGuideid ;
    
    // 触发此任务前置是否完成
    public bool preFinish ;
    
    // 触发任务必须完成的ID
    public int preGuideId ;
    
    // 是否自动选中物品
    public bool autoChose ;
    
    // 延迟出现SKIP 3秒
    public bool delaySkip ;
    
    // 可否SKIP
    public int skip ;
    
    // 跳过教程的ID ; 不写是当前
    public int[] skipGuides ;
    
    // 到达次步之后 如果重启游戏是否需要返回GAME
    public bool backGame ;
    
    // 屏蔽装修关闭
    public bool shieldDecoClose ;
    
    // 是否指向世界坐标
    public bool isWorldPos ;
    
    // 是否点击关闭引导
    public bool blockAll ;
    
    // 是否开启MASK射线
    public bool rayCast ;
    
    // 纯透明MASK
    public bool clearMask ;
    
    // 遮罩半径倍数
    public float radius ;
    
    // 是否是圆形遮罩
    public bool isRandiusMask ;
    
    // 显示MERGEMASK
    public bool showMergeMask ;
    
    // 是否可以点击BACK返回房间
    public bool canBackHome ;
    
    // 是否可以产出MERGE物品
    public bool canProduct ;
    
    // 是否屏蔽进入GAME
    public bool maskEnterGame ;
    
    // NPC位置相对于半径扩大倍数
    public float expandMultiple ;
    
    // 小手是否显示 1
    public int handsGuide ;
    
    // 小手偏移
    public int[] arrowOffset ;
    
    // 箭头角度
    public int arrowAngle ;
    
    // 引导文字效果; -1 关闭文字引导; 0 带NPC头像的; 1 不带
    public int tipType ;
    
    // NPC头像; ROLES表ID
    public int npcHeadId ;
    
    // 多少秒后可以关闭
    public float lockTime ;
    
    // 等待时间 默认0.3
    public float waitTime ;
    
    // 是否立即显示引导界面; 默认 0.3秒延迟
    public bool isImmedShow ;
    
    // 文字指引(KEY)
    public string textGuide ;
    
    // SPINE动画名字
    public string spineAnimName ;
    
    // BI
    public string tiggerBi ;
    
    // BI
    public string finishBi ;
    


    public override int GetID()
    {
        return id;
    }
}
