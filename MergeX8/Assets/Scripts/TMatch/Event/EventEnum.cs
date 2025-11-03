namespace TMatch
{


    public static partial class EventEnum
    {
        // public const string GDPR_ACCEPTED = "PRIVACY_ACCEPTED";
        // public const string GDPR_REFUSED = "PRIVACY_REFUSED";
        public const string LOADING_PROGRESS = "LOADING_PROGRESS"; //加载进度
        public const string LOGIN_NEWDAY = "LOGIN_NEWDAY"; //新的一天首次登陆
        public const string RED_POINT = "RED_POINT"; // 红点监听

        public const string GET_STAR = "GET_STAR"; //获得星星
        public const string USE_ITEM = "USE_ITEM"; //使用道具
        public const string GET_COMBO = "GET_COMBO"; //获得COMBE
        public const string ADD_COMBOCOIN = "ADD_COMBOCOIN"; //通过Combo获得的金币
        public const string SERVE_CUSTOMER = "SERVE_CUSTOMER"; //服务客人
        public const string LIKE_RECEIVED = "LIKE_RECEIVED"; //获得赞
        public const string ADD_TIPCOIN = "ADD_TIPCOIN"; //获得小费
        public const string DISH_SENT = "DISH_SENT"; //制作食物
        public const string PASS_CHECKPOINT = "PASS_CHECKPOINT"; //通过关卡
        public const string LEVEL_RESULT = "LEVEL_RESULT"; //关卡结束

        // public const string WORLD_MAINUI_SWITCH = "WORLD_MAINUI_SWITCH"; //主界面切换


        public const string UPGRADECOOKER = "UPGRADECOOKER"; //升级厨具花费金币
        public const string UPGRADEFOOD = "UPGRADEFOOD"; //升级食物花费金币
        public const string REFRESH_UPGRADE_SHOP = "REFRESH_UPGRADE_SHOP"; //升级商店界面刷新

        public const string POPUP_OPEN = "POPUP_OPEN";
        public const string POPUP_CLOSE = "POPUP_CLOSE";

        /// <summary>
        /// 定位到 level ui
        /// </summary>
        public const string OPEN_LEVEL_UI = "OPEN_LEVEL_UI";

        /// <summary>
        /// 解锁新的 round
        /// </summary>
        public const string UNLOCK_NEW_ROUND = "UNLOCK_NEW_ROUND";

        /// <summary>
        /// 解锁新的餐厅
        /// </summary>
        public const string UNLOCK_NEW_RESTAURANT_AFTER_WINDOW_CLOSED = "UNLOCK_NEW_RESTAURANT_AFTER_WINDOW_CLOSED";

        public const string FSCREPE_CHANGED = "FSCREPE_CHANGED"; //fs可丽饼变化
        public const string FSSTAR_CHANGED = "FSSTAR_CHANGED"; //fs星星变化

        public const string STAR_CHANGED = "STAR_CHANGED"; //星星变化

        // public const string DIAMOND_CHANGED = "DIAMOND_CHANGED"; //钻石变化
        public const string COIN_CHANGED = "COIN_CHANGED"; // 金币变化

        public const string FSENERGY_CHANGED = "FSENERGY_CHANGED"; // fs体力变化

        // public const string ENERGY_CHANGED = "ENERGY_CHANGED"; // 体力变化
        public const string ENDLESS_ACTIVITY_ENERGY_CHANGED = "ENDLESS_ACTIVITY_ENERGY_CHANGED"; // 无尽挑战活动体力变化
        public const string NO_DIE_CHALLENGE_ENERGY_CHANGED = "NO_DIE_CHALLENGE_ENERGY_CHANGED"; // 不死挑战体力变化

        /// <summary>
        /// 获得新ACTIVITY卡后刷新UI
        /// </summary>
        public const string GALLERY_REFRESH_ACTIVITY_MAIN_UI = "GALLERY_REFRESH_ACTIVITY_MAIN_UI";

        public const string CLAIM_DAILY_BONUS = "CLAIM_DAILY_BONUS"; // 领取每日奖励

        public const string DAILYTASK_PROGRESS = "DAILYTASK_PROGRESS"; // 每日任务进度改变

        public const string POP_ANNOUNCEMENT = "POP_ANNOUNCEMENT"; //弹出系统公告
        // public const string REFRESH_SYSTEM_MAIL = "REFRESH_SYSTEM_MAIL"; // 刷新系统邮件
        // public const string CLAIMED_SYSTEM_MAIL_REWARD = "CLAIMED_SYSTEM_MAIL_REWARD"; //领取系统邮件奖励事件（服务器验证成功)
        // public const string CLAIMED_FRIEND_GIFT = "CLAIMED_FRIEND_GIFT"; // 领取好友送的体力
        // public const string REPLIED_FRIEND_GIFT = "REPLIED_FRIEND_GIFT"; // 回应好友请求的体力
        // public const string REFRESH_MAILBOX_VISIBLE = "REFRESH_MAILBOX_VISIBLE"; // 刷新邮箱的可见性

        // public const string REFRESH_FACEBOOK_FRIENDS = "REFRESH_FACEBOOK_FRIENDS"; // 刷新Facebook好友列表
        // public const string SENTTO_FRIEND_GIFT = "SENTTO_FRIEND_GIFT"; // 送好友体力
        // public const string ASKEDFOR_FRIEND_GIFT = "ASKEDFOR_FRIEND_GIFT"; // 向好友请

        // public const string DOWNLOAD_ERROR = "DOWNLOAD_ERROR"; // 下载报错
        // public const string DOWNLOAD_FINISH = "DOWNLOAD_FINISH"; // 文件更新完毕,只要有文件下载完成就会发送这个
        // public const string DYNAMIC_DOWNLOAD_REFRESH_EVENT = "DYNAMIC_DOWNLOAD_REFRESH_EVENT"; // 动态下载刷新通知

        public const string SPEED_UP_LOADING_ANIMATION = "SPEED_UP_LOADING_ANIMATION"; // 加速loading界面的spine动画

        // public const string SCREEN_ORIENTATION_CHANGED = "SCREEN_ORIENTATION_CHANGED"; // 屏幕方向改变
        public const string SALE_UPDATE = "SALE_UPDATE"; // 成功获取到活动信息
        // public const string ACTIVITY_UPDATE = "ACTIVITY_UPDATE"; // 成功获取到活动信息
        // public const string STORE_SALE_UPDATE = "STORE_SALE_UPDATE"; // 成功获取到商城打折信息

        public const string COLLECTION_RANK_ACTIVITY_STATE_CHANGED = "COLLECTION_RANK_ACTIVITY_STATE_CHANGED";

        /// <summary>
        /// 完成活动某个阶段
        /// </summary>
        public const string ACTIVITY_STAGE_COMPLETED = "ACTIVITY_STAGE_COMPLETED";

        //-------------------------------------------------- CK3 --------------------------------------------------//
        public const string HOME_EXP_CHANGED = "HOME_EXP_CHANGED"; //家园经验发生改变
        // public const string CURRENCY_CHANGED = "CURRENCY_CHANGED"; // 货币发生改变
        // public const string SHOW_BUILD_BUBBLE = "SHOW_BUILD_POINT_ON_WORLD"; // 显示地图上所有可建造的点
        // public const string HIDE_NODE_BUY = "HIDE_NODE_BUY"; // 隐藏挂点购买弹窗
        // public const string SELECT_NODE = "SELECT_NODE"; // 选中一个建筑点
        // public const string SELECT_THEME_AREA = "SELECT_THEME_AREA"; // 选中主题区域
        // public const string UNSELECT_NODE = "UNSELECT_NODE"; // 取消选中一个建筑点
        // public const string SELECT_ONE_ITEM = "CHOOSE_ONE_BUILDING"; //在N选1界面上选中一个建筑
        // public const string REMOVE_BUILDING_INFO = "REMOVE_BUILDING_INFO"; //在N选1界面上选中一个建筑
        // public const string DECORATION_UNLOCK_ITEM = "DECORATION_UNLOCK_BUILDING"; // 解锁建筑
        // public const string SHOW_OR_HIDE_LONG_PRESS_ARROW = "SHOW_OR_HIDE_LONG_PRESS_ARROW"; //显示或者隐藏长按的提示箭头
        // public const string UNLOCK_MAPAREA = "UNLOCK_MAPAREA"; //地图区域解锁

        // public const string LACK_BUILDING_RESOURCES = "LACK_BUILDING_RESOURCES"; // 缺少建造资源

        public const string JOB_COMPLETE = "JOB_COMPLETE";

        public const string SHOW_MAIN_MENU = "SHOW_MAIN_MENU";

        public const string UPDATE_CANDY_NOTICE = "UPDATE_CANDY_NOTICE";
        public const string UPDATE_BUILDING_TIP_TIME = "UPDATE_BUILDING_TIP_TIME";
        public const string UPDATE_TOP_PLAYER = "UPDATE_TOP_PLAYER";

        public const string UPDATE_DEFENSE = "UPDATE_DEFENSE";
        public const string MOVE_MAP = "MOVE_MAP";
        public const string MOVE_SCALE_MAP = "MOVE_SCALE_MAP";

        public const string ROLE_HEAD_CHANGE = "ROLE_HEAD_CHANGE"; // 钞票变化

        // 点击leveproperty ?
        public const string ShowLevelPropertyBoostsNotice = "ShowLevelPropertyBoostsNotice";

        // 点击切换了map tab
        public const string ShowMapTabStateChanged = "ShowMapTabStateChanged";

        // 点击map下载
        public const string ClickMapDownload = "ClickMapDownload";

        // 点击商城，展示更多
        public const string ShowStoreMoreItem = "ShowStoreMoreItem";

        // FC升级
        public const string FCUpgrade = "FCUpgrade";

        // 邮件事件
        // public const string RefreshMail = "RefreshMail";

        // public const string CloseMail = "CloseMail";

        // 每日签到显隐事件
        // public const string ShowDailyBonus = "ShowDailyBonus";

        //聚焦建议挂点
        // public const string FocusOnSuggestNode = "FocusOnSuggestNode";

        // 支付成功后发送
        public const string IAPSuccess = "IAPSuccess";
        public const string RestorePurchasesSuccess = "RestorePurchasesSuccess";

        // 春日活动领取门票
        public const string ClaimAcivityTicket = "ClaimAcivityTicket";

        /// <summary>
        /// 多语言变化
        /// </summary>
        // public const string LANGUAGE_CHNAGE = "LANGUAGE_CHNAGE";

        // public const string SELECT_AREA_THEME_CELL = "SELECT_AREA_THEME_CELL";

        /// <summary>
        /// 点击FBLike
        /// </summary>
        // public const string ClickFBLikeUs = "ClickFBLikeUs";

        /// <summary>
        /// 点击切换toggle
        /// </summary>
        // public const string ClickWorldMapToggle = "ClickWorldMapToggle";

        /// <summary>
        /// 主UI活动显示完毕
        /// </summary>
        public const string MainUI_Activity_List_Displayed = "MainUIActivityListDisplayed";

        // FAQ
        public const string FAQ_SELECT_QUESTION = "FAQ_SELECT_QUESTION"; //选择问题结束
        public const string FAQ_QUESTION_SERVER_BACK = "FAQ_QUESTION_SERVER_BACK"; //问题结束 服务器给出响应

        // public const string OnConfigHubUpdated = "OnConfigHubUpdated";                      // 远程配置更新完毕
        public const string OnIAPItemPaid = "OnIAPItemPaid"; //充值事件

        public const string BuyEnergyInOutLives = "BuyEnergyInOutLives";

        public const string LobbyMainShowState = "LobbyMainShowState";
        public const string LobbyNavigationActiveType = "LobbyNavigationActiveType";
        public const string SignedSuccess = "SignedSuccess";
        public const string CurrencyFlyAniEnd = "CurrencyFlyAniEnd";

        public const string TMatchResultExecute = "TMatchResultExecute";
        public const string LivesBankDataRefresh = "LivesBankDataRefresh";
        public const string JumpToLobbyNavigationType = "JumpToLobbyNavigationType";
        public const string WeeklyChallengeStateReset = "WeeklyChallengeStateReset";
        public const string WeeklyChellengeAddCollectCnt = "WeeklyChellengeAddCollectCnt";

        public const string OnApplicationPause = "OnApplicationPause";
        public const string LobbyRefreshShow = "LobbyRefreshShow";

        public const string StarChallengeRefresh = "StarChallengeRefresh"; // 明星挑战赛刷新
        public const string CollectGuildRefresh = "CollectGuildRefresh"; // 公会收集活动刷新
        public const string PiggyBankRefresh = "PiggyBankRefresh"; // 公会收集活动刷新

        public const string MakeoverResultExecute = "MakeoverResultExecute";
        public const string ASMRResultExecute = "ASMRResultExecute";
        
        public static readonly string TM_GIFTBAGLINK_GET_REWARD = "TM_GIFTBAGLINK_GET_REWARD";
        public static readonly string TM_GIFTBAGLINK_PURCHASE_REFRESH = "TM_GIFTBAGLINK_PURCHASE_REFRESH";
        public static readonly string TM_TASKASSIST_PURCHASE_REFRESH = "TM_TASKASSIST_PURCHASE_REFRESH";
        public static readonly string TM_GARAGE_CLEANUP_PURCHASE_REFRESH = "TM_GARAGE_CLEANUP_PURCHASE_REFRESH";
        public static readonly string TM_GIFTBAGLINK_OPEN_REFRESH = "TM_GIFTBAGLINK_OPEN_REFRESH";
    }
}