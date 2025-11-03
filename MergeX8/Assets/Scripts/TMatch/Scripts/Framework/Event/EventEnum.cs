namespace TMatch
{


    public static partial class EventEnum
    {
        public const string GDPR_ACCEPTED = "PRIVACY_ACCEPTED";
        public const string GDPR_REFUSED = "PRIVACY_REFUSED";
        public const string REDPOINT = "REDPOINT"; // 红点监听

        public const string OnConfigHubUpdated = "OnConfigHubUpdated"; // 远程配置更新完毕

        public const string WORLD_MAINUI_SWITCH = "WORLD_MAINUI_SWITCH"; //主界面切换

        public const string DIAMOND_CHANGED = "DIAMOND_CHANGED"; //钻石变化
        public const string ENERGY_CHANGED = "ENERGY_CHANGED"; // 体力变化

        public const string CLAIMED_FRIEND_GIFT = "CLAIMED_FRIEND_GIFT"; // 领取好友送的体力

        public const string REFRESH_FACEBOOK_FRIENDS = "REFRESH_FACEBOOK_FRIENDS"; // 刷新Facebook好友列表
        public const string SENTTO_FRIEND_GIFT = "SENTTO_FRIEND_GIFT"; // 送好友体力
        public const string ASKEDFOR_FRIEND_GIFT = "ASKEDFOR_FRIEND_GIFT"; // 向好友请求体力

        public const string DOWNLOAD_ERROR = "DOWNLOAD_ERROR"; // 下载报错
        public const string DOWNLOAD_FINISH = "DOWNLOAD_FINISH"; // 文件更新完毕,只要有文件下载完成就会发送这个
        public const string DYNAMIC_DOWNLOAD_REFRESH_EVENT = "DYNAMIC_DOWNLOAD_REFRESH_EVENT"; // 动态下载刷新通知

        public const string SCREEN_ORIENTATION_CHANGED = "SCREEN_ORIENTATION_CHANGED"; // 屏幕方向改变
        public const string ACTIVITY_UPDATE = "ACTIVITY_UPDATE"; // 成功获取到活动信息

        public const string CURRENCY_CHANGED = "CURRENCY_CHANGED"; // 货币发生改变
        public const string SHOW_BUILD_BUBBLE = "SHOW_BUILD_POINT_ON_WORLD"; // 显示地图上所有可建造的点
        public const string HIDE_NODE_BUY = "HIDE_NODE_BUY"; // 隐藏挂点购买弹窗
        public const string SELECT_NODE = "SELECT_NODE"; // 选中一个建筑点
        public const string SELECT_THEME_AREA = "SELECT_THEME_AREA"; // 选中主题区域
        public const string UNSELECT_NODE = "UNSELECT_NODE"; // 取消选中一个建筑点
        public const string SELECT_ONE_ITEM = "CHOOSE_ONE_BUILDING"; //在N选1界面上选中一个建筑
        public const string REMOVE_BUILDING_INFO = "REMOVE_BUILDING_INFO"; //在N选1界面上选中一个建筑
        public const string DECORATION_UNLOCK_ITEM = "DECORATION_UNLOCK_BUILDING"; // 解锁建筑
        public const string SHOW_OR_HIDE_LONG_PRESS_ARROW = "SHOW_OR_HIDE_LONG_PRESS_ARROW"; //显示或者隐藏长按的提示箭头
        public const string UNLOCK_MAPAREA = "UNLOCK_MAPAREA"; //地图区域解锁
        public const string SWITCH_WORLD = "SWITCH_WORLD"; // 切换世界

        public const string LACK_BUILDING_RESOURCES = "LACK_BUILDING_RESOURCES"; // 缺少建造资源

        // 邮件事件
        public const string RefreshMail = "RefreshMail";

        public const string CloseMail = "CloseMail";
        public const string FlyRewards = "FlyRewards";

        // 每日签到显隐事件
        public const string ShowDailyBonus = "ShowDailyBonus";

        //聚焦建议挂点
        public const string FocusOnSuggestNode = "FocusOnSuggestNode";

        // 点击餐厅故事icon
        public const string SelectRestaurantStory = "SelectRestaurantStory";

        /// <summary>
        /// 多语言变化
        /// </summary>
        public const string LANGUAGE_CHNAGE = "LANGUAGE_CHNAGE";


        public const string SELECT_AREA_THEME_CELL = "SELECT_AREA_THEME_CELL";

        //刷新主页更新游戏icon
        public const string REFRESH_UPDATE_APP_ICON = "REFRESH_UPDATE_APP_ICON";

        //刷新扭蛋机UI
        public const string REFRESH_GACHA_UI = "REFRESH_GACHA_UI";

        /// <summary>
        /// 点击FBLike
        /// </summary>
        public const string ClickFBLikeUs = "ClickFBLikeUs";

        /// <summary>
        /// 点击切换toggle
        /// </summary>
        public const string ClickWorldMapToggle = "ClickWorldMapToggle";

        /// <summary>
        /// 角色姓名更新
        /// </summary>
        public const string Role_Name_Update = "Role_Name_Update";

        /// <summary>
        /// 角色头像更新
        /// </summary>
        public const string Role_Head_Icon_Update = "Role_Head_Icon_Update";

        /// <summary>
        /// 头像红点
        /// </summary>
        public const string Role_Head_Icon_Redpoint = "Role_Head_Icon_Redpoint";


        #region 物品系统

        /// <summary>
        /// 物品发生变化
        /// </summary>
        public const string ItemChange = "ItemChange";

        /// <summary>
        /// 物品更新
        /// </summary>
        public const string ItemUpdate = "ItemUpdate";

        /// <summary>
        /// 物品持续时间更新
        /// </summary>
        public const string ItemDurationUpdate = "ItemDurationUpdate";

        /// <summary>
        /// 物品最大值更新
        /// </summary>
        public const string ItemMaxUpdate = "ItemMaxUpdate";

        #endregion

        #region 笑脸收集

        /// <summary>
        /// 收集笑脸
        /// </summary>
        public const string SmileCollect = "SmileCollect";

        /// <summary>
        /// 显示收集笑脸
        /// </summary>
        public const string ShowSmileCollect = "ShowSmileCollect";

        #endregion

        public const string OnPurchaseSuccess = "OnPurchaseSuccess"; //购买或者其他方式成功后获得物品事件
        public const string OnPurchaseFailOrCancel = "OnPurchaseFailOrCancel"; // 购买失败或取消
        public const string PURCHASE_SUCCESS = "PURCHASE_SUCCESS"; //支付成功
        public const string STORE_SHOP_FREEITEM_CLAIM = "STORE_SHOP_FREEITEM_CLAIM"; //商城免费道具领取
        public const string STORE_CLAIM_FREE_SUCCESS = "STORE_CLAIM_FREE_SUCCESS"; //买N送N领取免费
        public const string STORE_SALE_UPDATE = "STORE_SALE_UPDATE"; // 成功获取到商城打折信息
        public const string UnfulfilledPaymentHandleSuccess = "UnfulfilledPaymentHandleSuccess"; //补单处理成功事件

        public const string PACKCLAIM_SUCCESS = "PACKCLAIM_SUCCESS"; //礼包领取完成

        /// <summary>
        /// 打开UI
        /// </summary>
        public const string OpenUIWindow = "OpenUIWindow";

        /// 关闭UI
        /// </summary>
        public const string CloseUIWindow = "CloseUIWindow";

        public const string UIMapSelectItem = "UIMapSelectItem";

        #region 引导相关

        public const string GuideClaimPatReward = "GuideClaimPatReward";

        #endregion

        #region 小猪存钱罐

        public const string PiggyBankUpdate = "PiggyBankUpdate";

        #endregion

        #region 局内道具使用

        public const string OnUseGameBoost = "OnUseGameBoost";
        public const string OnUsePreGameBoost = "OnUsePreGameBoost";

        #endregion

        #region 实验室活动

        public const string LabActivityUpdate = "LabActivityUpdate";

        #endregion

        #region 连赢

        public const string WinStreak2Update = "WinStreak2Update";

        #endregion

        #region 玩家信息

        public const string PlayerInfoName = "PlayerInfoName";
        public const string PlayerInfoPortrait = "PlayerInfoPortrait";
        public const string PlayerInfoPortraitFrame = "PlayerInfoPortraitFrame";
        public const string PlayerInfoAvatar = "PlayerInfoAvatar";
        public const string PlayerInfoAvatarNew = "PlayerInfoAvatarNew";
        public const string PlayerInfoAvatarClickTab = "PlayerInfoAvatarClickTab";
        public const string PlayerInfoAvatarClickSubTab = "PlayerInfoAvatarClickSubTab";

        #endregion

        #region 买二赠一礼包

        public const string StageGiftUpdate = "StageGiftUpdate";

        #endregion

        #region Buff系统

        public const string OnBuffStatusChanged = "OnBuffStatusChanged";

        #endregion

        #region 循环关Map选择

        public const string OnMapSelectChanged = "OnMapSelectChanged";

        #endregion
    }
}