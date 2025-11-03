namespace TMatch
{


    public static partial class EventEnum
    {
        public const string TMATCH_GAME_ENTER = "TMATCH_GAME_ENTER"; //进入
        public const string TMATCH_GAME_CREATE = "TMATCH_GAME_CREATE"; //创建
        public const string TMATCH_GAME_START = "TMATCH_GAME_START"; //开始
        public const string TMATCH_GAME_WIN = "TMATCH_GAME_WIN"; //赢
        public const string TMATCH_GAME_WIN_BEFORE_ADD_MAIN_LEVEL = "TMATCH_GAME_WIN_BEFORE_ADD_MAIN_LEVEL";//增加mainLevel之前 赢
        public const string TMATCH_GAME_FAIL = "TMATCH_GAME_FAIL"; //输
        public const string TMATCH_GAME_FINISH = "TMATCH_GAME_FINISH"; //结束
        public const string TMATCH_GAME_EXIT = "TMATCH_GAME_EXIT"; //离开
        public const string TMATCH_GAME_TRY_AGAIN = "TMATCH_GAME_TRY_AGAIN"; //重来

        public const string TMATCH_GAME_TARGET_FINISH = "TMATCH_GAME_TARGET_FINISH"; //目标完成
        public const string TMATCH_GAME_TIMEOUT = "TMATCH_GAME_TIMEOUT"; //超时
        public const string TMATCH_GAME_SPACEOUT = "TMATCH_GAME_SPACEOUT"; //收集的格子满了

        public const string TMATCH_GAME_TIME_PAUSE = "TMATCH_GAME_TIME_PAUSE"; //时间暂停

        public const string TMATCH_GAME_TRIPLE = "TMATCH_GAME_TRIPLE"; //点消
        public const string TMATCH_GAME_TRIPLE_BOOST = "TMATCH_GAME_TRIPLE_BOOST"; //点消-道具
        public const string TMATCH_GAME_TRIPLE_BOOST_HANDLE = "TMATCH_GAME_TRIPLE_BOOST_HANDLE"; //点消-道具-手动添加

        public const string TMATCH_COLLECTOR_ITEMS_CHANGE = "TMATCH_COLLECTOR_ITEMS_CHANGE"; //收集栏-道具变化

        public const string TMATCH_LOW_SPACE_TIP = "TMATCH_LOW_SPACE_TIP"; //收集栏-空间不足提示

        public const string TMATCH_GAME_CHANGE_STATE = "TMATCH_GAME_CHANGE_STATE"; //切换游戏状态

        public const string GAME_ITEM_USE = "GAME_ITEM_USE";
        public const string GAME_ITEM_CHANGE = "GAME_ITEM_CHANGE";

        public const string TMATCH_NEED_COLLECT_ITEM = "TMATCH_NEED_COLLECT_ITEM"; //本场需要收集的Item
        public const string TMATCH_EVENT_UPDATE = "TMATCH_EVENT_UPDATE";
        
        public const string BuyReviveGiftPackSuccess = "BuyReviveGiftPackSuccess";

    }
}