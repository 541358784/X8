using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string EXTRA_ORDER_REWARD_COUPON_END = "EXTRA_ORDER_REWARD_COUPON_END";
    public const string EXTRA_ORDER_REWARD_COUPON_START = "EXTRA_ORDER_REWARD_COUPON_START";
    public const string EXTRA_ORDER_REWARD_COUPON_GET_NEW_COUPON = "EXTRA_ORDER_REWARD_COUPON_GET_NEW_COUPON";
    public const string EXTRA_ORDER_REWARD_COUPON_USE_PAY_COUPON = "EXTRA_ORDER_REWARD_COUPON_USE_PAY_COUPON";
}
public class EventExtraOrderRewardCouponEnd : BaseEvent
{
    public TableExtraOrderRewardCouponConfig Config;
    public EventExtraOrderRewardCouponEnd() : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_END) { }

    public EventExtraOrderRewardCouponEnd(TableExtraOrderRewardCouponConfig config) : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_END)
    {
        Config = config;
    }
}

public class EventExtraOrderRewardCouponStart : BaseEvent
{
    public StorageExtraOrderRewardCouponItem StorageItem;
    public EventExtraOrderRewardCouponStart() : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_START) { }

    public EventExtraOrderRewardCouponStart(StorageExtraOrderRewardCouponItem storageItem) : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_START)
    {
        StorageItem = storageItem;
    }
}

public class EventExtraOrderRewardCouponGetNewCoupon : BaseEvent
{
    public TableExtraOrderRewardCouponConfig Config;
    public EventExtraOrderRewardCouponGetNewCoupon() : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_GET_NEW_COUPON) { }

    public EventExtraOrderRewardCouponGetNewCoupon(TableExtraOrderRewardCouponConfig config) : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_GET_NEW_COUPON)
    {
        Config = config;
    }
}

public class EventExtraOrderRewardCouponUsePayCoupon : BaseEvent
{
    public TableExtraOrderRewardCouponConfig Config;
    public EventExtraOrderRewardCouponUsePayCoupon() : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_USE_PAY_COUPON) { }

    public EventExtraOrderRewardCouponUsePayCoupon(TableExtraOrderRewardCouponConfig config) : base(EventEnum.EXTRA_ORDER_REWARD_COUPON_USE_PAY_COUPON)
    {
        Config = config;
    }
}
