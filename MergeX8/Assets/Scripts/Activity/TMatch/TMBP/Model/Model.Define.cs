//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月19日 星期四
//describe    :   
//-----------------------------------

using System;
using UnityEngine;

namespace TMatch
{


    public static partial class EventEnum
    {
        public const string TM_BattlePassOnLevelChanged = "TM_BattlePassOnLevelChanged"; // 战令等级发生变化
        public const string TM_BattlePassOnExpChanged = "TM_BattlePassOnExpChanged"; // 战令经验发生变化
        public const string TM_BattlePassOnClaimReward = "TM_BattlePassOnClaimReward"; // 战令领取奖励
        public const string TM_BattlePassOnBuy = "TM_BattlePassOnBuy"; // 战令购买
        public const string TM_RewardUpdateStatus = "TM_RewardUpdateStatus"; // 战令奖励状态变化
        public const string TM_CreateRewardItemAnim = "TM_CreateRewardItemAnim"; // 战令创建奖励动画
        public const string TM_BPOnListAnimEnd = "TM_BPOnListAnimEnd"; // 动画结束后
    }

    public class GameItemInfo
    {
        public int ItemId;
        public int ItemNum;

        public GameItemInfo(int id, int num)
        {
            ItemId = id;
            ItemNum = num;
        }
    }

    public class TM_BPClaimRewardEvent : BaseEvent
        {
            /// <summary>
            /// 钻石数量
            /// </summary>
            public int Id;

            /// <summary>
            /// 类型
            /// </summary>
            public TM_BpType BpType;

            /// <summary>
            /// 是否是循环奖励
            /// </summary>
            public bool IsLoopReward;

            public TM_BPClaimRewardEvent(bool isLoop) : base(EventEnum.TM_BattlePassOnClaimReward)
            {
                IsLoopReward = isLoop;
            }

            public TM_BPClaimRewardEvent(int id, TM_BpType bpType) : base(EventEnum.TM_BattlePassOnClaimReward)
            {
                Id = id;
                BpType = bpType;
                IsLoopReward = false;
            }
        }

        /// <summary>
        /// 创建奖励
        /// </summary>
        public class TM_BPCreateRewardItemAnimEvent : BaseEvent
        {
            /// <summary>
            /// 目标位置
            /// </summary>
            public Transform Target;

            /// <summary>
            /// 道具id
            /// </summary>
            public int ItemId;

            /// <summary>
            /// 道具数量
            /// </summary>
            public int ItemNum;

            /// <summary>
            /// 类型
            /// </summary>
            public TM_BpType BpType;

            public TM_BPCreateRewardItemAnimEvent(Transform target, int itemId, int itemNum, TM_BpType bpType) : base(
                EventEnum.TM_CreateRewardItemAnim)
            {
                Target = target;
                ItemId = itemId;
                ItemNum = itemNum;
                BpType = bpType;
            }
        }


        /// <summary>
        /// 购买
        /// </summary>
        public class TM_BPBuyEvent : BaseEvent
        {
            /// <summary>
            /// 钻石数量
            /// </summary>
            public ShopLevel ShopLevel;

            public TM_BPBuyEvent(ShopLevel shopLevel) : base(EventEnum.TM_BattlePassOnBuy)
            {
                ShopLevel = shopLevel;
            }
        }

        /// <summary>
        /// 战令奖励状态
        /// </summary>
        public class TM_BPRewardStatusChangeEvent : BaseEvent
        {
            public TM_BPRewardStatusChangeEvent() : base(EventEnum.TM_RewardUpdateStatus)
            {

            }
        }

        /// <summary>
        /// 动画结束后事件
        /// </summary>
        public class TM_BPListAnimEndEvent : BaseEvent
        {
            public TM_BPListAnimEndEvent() : base(EventEnum.TM_BPOnListAnimEnd)
            {

            }
        }

        /// <summary>
        /// 战令商品等级
        /// </summary>
        public enum ShopLevel
        {
            /// <summary>
            /// 普通
            /// </summary>
            Normal,

            /// <summary>
            /// 增加等级
            /// </summary>
            AddLevel,
        }


        /// <summary>
        /// 道具类型
        /// </summary>
        public enum TM_BpAwardType
        {
            /// <summary>
            /// 道具
            /// </summary>
            Item = 1,

            /// <summary>
            /// 宝箱
            /// </summary>
            Chest = 2,
        }

        /// <summary>
        /// 奖励状态
        /// </summary>
        public enum TM_BPRewardStatus
        {
            Locked = 1, // 锁定
            Unlock = 2, // 解锁
            Available = 3, // 可领取
            AvailableAd = 4, // 可广告领取
            Claimed = 5, // 已领取
        }

        /// <summary>
        /// 战令状态
        /// </summary>
        public enum TM_BpType
        {
            /// <summary>
            /// 普通
            /// </summary>
            Normal,

            /// <summary>
            /// 购买
            /// </summary>
            Golden
        }
}