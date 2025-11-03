using UnityEngine;
using System.Collections;

public static partial class EventEnum
{
    public const string GuideFinish = "GuideFinish";//引导结束
    public const string UPDATE_ASSETS_COMPLATE = "UPDATE_ASSETS_COMPLATE"; //资源更新完毕
    public const string LOGIN_COMPLATE = "LOGIN_COMPLATE"; //登录成功
    public const string GDPR_ACCEPTED = "PRIVACY_ACCEPTED";
    public const string GDPR_REFUSED = "PRIVACY_REFUSED";
    public const string LOADING_PROGRESS = "LOADING_PROGRESS"; //加载进度
    public const string DOWNLOAD_ERROR = "DOWNLOAD_ERROR"; // 下载报错
    public const string DOWNLOAD_FINISH = "DOWNLOAD_FINISH"; // 文件更新完毕,只要有文件下载完成就会发送这个
    public const string SPEED_UP_LOADING_ANIMATION = "SPEED_UP_LOADING_ANIMATION"; // 加速loading界面的spine动画
    public const string LOADING_FINISH = "LOADING_FINISH"; // loading finish 加载完成 带loading条的
    public const string LOADING_START = "LOADING_START"; // loading start 开始加载 带loading条的
    public const string BackLogin = "BackLogin"; // 回到开始界面

    public const string SCREEN_ORIENTATION_CHANGED = "SCREEN_ORIENTATION_CHANGED"; // 屏幕方向改变
    public const string OnConfigHubUpdated = "OnConfigHubUpdated"; // 远程配置更新完毕
    public const string EnterHome = "EnterHome";
    public const string UserDataUpdate = "UserDataUpdate"; //游戏资源更新
    public const string EnergyChanged = "EnergyChanged"; //体力变化 
    public const string HGEnergyChanged = "HGEnergyChanged"; //体力变化 
    public const string RoomAnimationEnd = "RoomAnimationEnd"; //房间动画播放完成
    public const string OnIAPItemPaid = "OnIAPItemPaid"; //充值事件
    public const string HOTEL_PURCHASE_SUCCESS = "HOTEL_PURCHASE_SUCCESS"; //充值事件
    public const string HOTEL_CLAIM_FREE_SUCCESS = "HOTEL_CLAIM_FREE_SUCCESS";
    public const string OnLevelUp = "OnLevelUp";//升级
    public const string OnKeepPetLevelUp = "OnKeepPetLevelUp";//狗升级

    public const string BackHomeStep = "BackHomeStep";

    public const string UpdateRedPoint = "UpdateRedPoint";
    public const string UpdateDailyBonus = "UpdateDailyBonus";
    public const string RV_SHOP_DATA_CHANGE = "RvShopDataChange";
    public const string M3_RV_SHOP_REFRESH = "RvShopRefresh";
    public const string RV_SHOP_PURCHASE = "rvshoppurchase";

    public const string BUYSOURCES_PURCHASE = "buysourcespurchase";

    public static readonly string TASK_REFRESH = "TASK_REFRESH";
    public static readonly string TASK_COMPLETE_REFRESH = "TASK_COMPLETE_REFRESH";
    public static readonly string TASKBOX_PLAYANIM = "TASKBOX_PLAYANIM";
    public static readonly string ORDER_REMOVE_REFRESH = "ORDER_REMOVE_REFRESH";

    public static readonly string IceBreak_Pack_Begin = "IceBreak_Pack_Begin";
    public static readonly string IceBreak_Pack_REFRESH = "IceBreak_Pack_REFRESH";
    public static readonly string IceBreak_Pack_Finish = "IceBreak_Pack_Finish";
    public static readonly string Daily_Pack_Begin = "Daily_Pack_Begin";
    public static readonly string Daily_Pack_REFRESH = "Daily_Pack_REFRESH";
    public static readonly string Daily_Pack_Time_REFRESH = "Daily_Pack_Time_REFRESH";
    public static readonly string Daily_Pack_Finish = "Daily_Pack_Finish";
    public static readonly string NewDaily_Pack_Begin = "NewDaily_Pack_Begin";
    public static readonly string NewDaily_Pack_REFRESH = "NewDaily_Pack_REFRESH";
    public static readonly string NewDaily_Pack_Time_REFRESH = "NewDaily_Pack_Time_REFRESH";
    public static readonly string NewDaily_Pack_Finish = "NewDaily_Pack_Finish";
    public static readonly string REWARD_POPUP = "REWARD_POPUP";
    public static readonly string NOTICE_POPUP = "NOTICE_POPUP";
    public static readonly string GLOBAL_MAIL_UPDATED = "GLOBAL_MAIL_UPDATED";
    public static readonly string GET_DECORATION_REWARD = "GET_DECORATION_REWARD";
    


    public static readonly string GIFTBAGLINK_GET_REWARD = "GIFTBAGLINK_GET_REWARD";
    public static readonly string GIFTBAGLINK_PURCHASE_REFRESH = "GIFTBAGLINK_PURCHASE_REFRESH";
    public static readonly string TASKASSIST_PURCHASE_REFRESH = "TASKASSIST_PURCHASE_REFRESH";
    public static readonly string HAPPYGOBUNDLE_PURCHASE_REFRESH = "HAPPYGOBUNDLE_PURCHASE_REFRESH";
    public static readonly string HAPPYGO_CLAIM_REWARD = "HAPPYGO_CLAIM_REWARD";
    public static readonly string GARAGE_CLEANUP_PURCHASE_REFRESH = "GARAGE_CLEANUP_PURCHASE_REFRESH";
    public static readonly string GIFTBAGLINK_OPEN_REFRESH = "GIFTBAGLINK_OPEN_REFRESH";
    
    public static readonly string GIFT_BAG_BUY_BETTER_PURCHASE_REFRESH = "GIFT_BAG_BUY_BETTER_PURCHASE_REFRESH";
    public static readonly string GIFT_BAG_SEND_ONE_PURCHASE_REFRESH = "GIFT_BAG_SEND_ONE_PURCHASE_REFRESH";

    public static readonly string FAQ_SELECT_QUESTION = "FAQ_SELECT_QUESTION";
    public static readonly string FAQ_QUESTION_SERVER_BACK = "FAQ_QUESTION_SERVER_BACK";

    public const string MASTERCARD_PURCHASE = "MASTERCARD_PURCHASE";
    public const string MASTERCARD_GETREWARD = "MASTERCARD_GETREWARD";

    public static readonly string PIGBANK_PURCHASE_REFRESH = "PIGBANK_PURCHASE_REFRESH";
    public static readonly string PIGBANK_UI_REFRESH = "PIGBANK_UI_REFRESH";
    public static readonly string PIGBANK_VALUE_REFRESH = "PIGBANK_VALUE_REFRESH"; 
    public static readonly string PIGBANK_SHOW_BUTTON = "PIGBANK_SHOW_BUTTON"; 
    public static readonly string PIGBANK_INITIMAGE = "PIGBANK_INITIMAGE"; 
    
    public static readonly string VIEW_NEW_HEAD = "GET_NEW_HEAD"; // 浏览新头像
    public static readonly string GET_NEW_HEAD = "GET_NEW_HEAD"; // 获得新头像
    
    public static readonly string UPDATE_HEAD = "UPDATE_HEAD"; // 更新头像
    public static readonly string UPDATE_NAME = "UPDATE_NAME"; // 更新名字
    
    public static readonly string DAILY_RANK_UPDATE = "DAILY_RANK_UPDATE";
    
    public static readonly string GARAGE_CLEANUP_TURNIN = "GARAGE_CLEANUP_TURNIN";
    public static readonly string GARAGE_CLEANUP_LevelFinish = "GARAGE_CLEANUP_LevelFinish";
    public static readonly string GARAGE_CLEANUP_Finish = "GARAGE_CLEANUP_Finish";
    
    public static readonly string EASTER_CLAIM = "EASTER_CLAIM";
    
    public static readonly string EASTER_PACK_BEGIN = "EASTER_PACK_BEGIN";
    public static readonly string EASTER_PACK_REFRESH = "EASTER_PACK_REFRESH";
    public static readonly string EASTER_PACK_Finish = "EASTER_PACK_Finish";
    
    public static readonly string BATTLE_PASS_PURCHASE = "BATTLE_PASS_PURCHASE"; // battle pass 充值
    public static readonly string BATTLE_PASS_PURCHASE_UNLOCK = "BATTLE_PASS_PURCHASE_UNLOCK"; // battle pass 充值
    public static readonly string BATTLE_PASS_TASK_REFRESH = "BATTLE_PASS_TASK_REFRESH"; // battle task
    public static readonly string BATTLE_PASS_REFRESH = "BATTLE_PASS_REFRESH"; // battle task
    public static readonly string BATTLE_PASS_STORE_REFRESH = "BATTLE_PASS_STORE_REFRESH";
    public static readonly string BATTLE_PASS_TASK_COMPLETE = "BATTLE_PASS_TASK_COMPLETE"; // battle task
    public static readonly string BATTLE_PASS_COLLECT_LOOP = "BATTLE_PASS_COLLECT_LOOP"; // battle pass 循环奖励领取
    
    
    public static readonly string BATTLE_PASS_2_PURCHASE = "BATTLE_PASS_2_PURCHASE"; // battle pass 充值
    public static readonly string BATTLE_PASS_2_PURCHASE_UNLOCK = "BATTLE_PASS_2_PURCHASE_UNLOCK"; // battle pass 充值
    public static readonly string BATTLE_PASS_2_TASK_REFRESH = "BATTLE_PASS_2_TASK_REFRESH"; // battle task
    public static readonly string BATTLE_PASS_2_REFRESH = "BATTLE_PASS_2_REFRESH"; // battle task
    public static readonly string BATTLE_PASS_2_STORE_REFRESH = "BATTLE_PASS_2_STORE_REFRESH";
    public static readonly string BATTLE_PASS_2_TASK_COMPLETE = "BATTLE_PASS_2_TASK_COMPLETE"; // battle task
    public static readonly string BATTLE_PASS_2_COLLECT_LOOP = "BATTLE_PASS_2_COLLECT_LOOP"; // battle pass 循环奖励领取
    
    public static readonly string ASMR_BEGIN_DOWNLOAD = "ASMR_BEGIN_DOWNLOAD"; 
    public static readonly string ASMR_End_DOWNLOAD = "ASMR_End_DOWNLOAD"; 
    public static readonly string ASMR_DOWNLOAD_PROGRESS = "ASMR_DOWNLOAD_PROGRESS";
    public static readonly string ASMR_REFRESH_REDPOINT = "ASMR_REFRESH_REDPOINT";
    
    public static readonly string DIG_TRENCH_BEGIN_DOWNLOAD = "DIG_TRENCH_BEGIN_DOWNLOAD"; 
    public static readonly string DIG_TRENCH_End_DOWNLOAD = "DIG_TRENCH_End_DOWNLOAD"; 
    public static readonly string DIG_TRENCH_DOWNLOAD_PROGRESS = "DIG_TRENCH_DOWNLOAD_PROGRESS";
    public static readonly string DIG_TRENCH_REFRESH_REDPOINT = "DIG_TRENCH_REFRESH_REDPOINT";
    
    public static readonly string FISH_EAT_FISH_BEGIN_DOWNLOAD = "FISH_EAT_FISH_BEGIN_DOWNLOAD"; 
    public static readonly string FISH_EAT_FISH_End_DOWNLOAD = "FISH_EAT_FISH_End_DOWNLOAD"; 
    public static readonly string FISH_EAT_FISH_DOWNLOAD_PROGRESS = "FISH_EAT_FISH_DOWNLOAD_PROGRESS";
    public static readonly string FISH_EAT_FISH_REFRESH_REDPOINT = "FISH_EAT_FISH_REFRESH_REDPOINT";
    
    public static readonly string ONE_PATH_BEGIN_DOWNLOAD = "ONE_PATH_BEGIN_DOWNLOAD"; 
    public static readonly string ONE_PATH_End_DOWNLOAD = "ONE_PATH_End_DOWNLOAD"; 
    public static readonly string ONE_PATH_DOWNLOAD_PROGRESS = "ONE_PATH_DOWNLOAD_PROGRESS";
    public static readonly string ONE_PATH_REFRESH_REDPOINT = "ONE_PATH_REFRESH_REDPOINT";
    
    public static readonly string CONNECT_LINE_BEGIN_DOWNLOAD = "CONNECT_LINE_BEGIN_DOWNLOAD"; 
    public static readonly string CONNECT_LINE_End_DOWNLOAD = "CONNECT_LINE_End_DOWNLOAD"; 
    public static readonly string CONNECT_LINE_DOWNLOAD_PROGRESS = "CONNECT_LINE_DOWNLOAD_PROGRESS";
    public static readonly string CONNECT_LINE_REFRESH_REDPOINT = "CONNECT_LINE_REFRESH_REDPOINT";
    
    public static readonly string StimulateBeginDownload = "StimulateBeginDownload"; 
    public static readonly string StimulateEndDownload = "StimulateEndDownload"; 
    public static readonly string StimulateDownloadProgress = "StimulateDownloadProgress";
    public static readonly string StimulateRefreshRedPoint = "StimulateRefreshRedPoint";
    
    public static readonly string MERMAID_PURCHASE_SUCCESS = "MERMAID_PURCHASE_SUCCESS";
    public const string AddCoin = "AddCoin"; //金币增加
    public const string AddRecoverCoinStar = "AddRecoverCoinStar"; //回收金币星星增加
    public const string AddFishpondToken = "AddFishpondToken"; 
    public const string OwnDecoNode = "OwnDecoNode"; //获得挂点
    
    // 连赢活动
    public const string RefreshWinStreak = "RefreshWinStreak";
    public const string RefreshWinStreakProgress = "RefreshWinStreakProgress";
    
    public static readonly string HAPPYGO_EXTEND_PURCHASE_SUCCESS = "HAPPYGO_EXTEND_PURCHASE_SUCCESS";
    public static readonly string FLASH_SALE_REFRESH = "FLASH_SALE_REFRESH";
    public static readonly string THREE_GIFT_PURCHASE_SUCCESS = "THREE_GIFT_PURCHASE_SUCCESS";


    public static readonly string CONNECTLINE_SUCCESS = "ONE_PATH_BEGIN_DOWNLOAD"; 
    public static readonly string CONNECTLINE_FAILED = "CONNECTLINE_FAILED"; 
    
    
    public const string WEEKLYCARD_PURCHASE = "WEEKLYCARD_PURCHASE";
    public const string WEEKLYCARD_GETREWARD = "WEEKLYCARD_GETREWARD";
    
    public const string KEEPPET_GIFT_PURCHASE = "KEEPPET_GIFT_PURCHASE";
    
    public const string TREASURE_HUNT_ITEM_BREAK = "TREASURE_HUNT_ITEM_BREAK";
    public const string TREASURE_HUNT_PURCHASE = "TREASURE_HUNT_PURCHASE"; 
    
    public const string LUCKY_GOLDEN_EGG_ITEM_BREAK = "LUCKY_GOLDEN_EGG_ITEM_BREAK";
    public const string LUCKY_GOLDEN_EGG_PURCHASE = "LUCKY_GOLDEN_EGG_PURCHASE"; 
    
    public const string OPTIONAL_GIFT_SELECT = "OPTIONAL_GIFT_SELECT";
    public const string OPTIONAL_GIFT_PURCHASE_SUCCESS = "OPTIONAL_GIFT_PURCHASE_SUCCESS";

    public const string DEATH_PRODUCT_ITEM = "DEATH_PRODUCT_ITEM";
    
    public const string BUTTERFLY_WORKSHOP_PURCHASE = "BUTTERFLY_WORKSHOP_PURCHASE"; 
    
    public const string GARDEN_TREASURE_PURCHASE = "GARDEN_TREASURE_PURCHASE"; 
    
    public const string LOGIN_SUCCESS = "LOGIN_SUCCESS"; 
    
    public const string PURCHASE_SUCCESS_REWARD = "PURCHASE_SUCCESS_REWARD";


    public const string STORY_MOVIE_FINISH = "STORY_MOVIE_FINISH";
    
    //搬家订单订单刷新
    public static readonly string TRAIN_ORDER_ORDER_REFRESH = "TRAIN_ORDER_ORDER_REFRESH";
}