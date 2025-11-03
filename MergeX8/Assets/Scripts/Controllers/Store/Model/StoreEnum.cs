/// <summary>
/// 商店钻石活动类型.N后面的数字代表在参数列表的位置
/// </summary>
public enum StoreSaleGemType
{
    Normal, // 普通商品
    HandselMultiples, // 额外赠送N1倍(双倍奖励),不含原本的倍数
    SecondDiscount, // 第N1件价格系数N2(第二件半价),N3为打折支付时的商品ID
    BuyGetFree, // 买N1送N2
    BuyGetSpin, // 买N1次可抽奖N2次
}

public enum StoreProductType
{
    ///0:装修币,1:金币, 2:打折商品,3:Bundle,4:新手特卖,5:限时礼包,6:PiggyBank,7:分层礼包,8:去广告,9:黄金券
    DecoCoin = 0,
    Coin = 1,
    Sale = 2,
    Bundle = 3,
    NewbieBundle = 4,
    LimitBundle = 5,
    PiggyBank = 6,
    GroupBundle = 7,
    NoAds = 8,
    Energy = 21,
}