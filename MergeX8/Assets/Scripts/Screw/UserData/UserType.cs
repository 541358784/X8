namespace Screw.UserData
{
    public enum ResType
    {
        None,
        Coin = 1, //金币
        Energy = 2, //体力
        // Star = 3, //星星

        ExtraSlot = 4, //额外的卡位
        BreakBody = 5, //敲碎玻璃
        TwoTask = 6, //同时两个任务

        // TimeFreeze = 9, //时间冻结
        EnergyInfinity = 10,

        // Screw = 11, // 螺丝
    }

    public enum ItemInfinityIconType
    {
        None,
        NoTag, //无横幅，如无限体力
        TagAndInfinity, //横幅+无限标记 如闪电时钟
        TagAndX2, //横幅+x2 如周挑战buff 道具
    }
}