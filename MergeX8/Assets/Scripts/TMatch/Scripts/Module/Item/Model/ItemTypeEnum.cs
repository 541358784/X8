
namespace TMatch
{

    /// <summary>
    /// 物品类型
    /// </summary>
    public enum ItemType
    {
        None,

        

        // TM玩法道具
        TMCoin = 1001, //金币
        TMEnergy = 1002, //体力
        TMStar = 1003, //星星
        TMMagnet = 1004, //磁铁
        TMBroom = 1005, //扫把
        TMWindmill = 1006, //风车
        TMFrozen = 1007, //冰封
        TMLighting = 1008, //闪电
        TMClock = 1009, //时钟
        TMEnergyInfinity = 1010, //无限体力
        TMLightingInfinity = 1011, //无限闪电
        TMClockInfinity = 1012, //无限时钟
        TMInfinityActivityCollect = 1013, //显示活动收集
        TMWeeklyChallengeCollect = 1014, //周挑战收集
        TMWeeklyChallengeBuff = 1015, //周挑战buff
    }

    // public enum eResourceId
    // {
    //     None,
    //     Diamond = 101,
    //     Gold = 201,
    //     Energy = 301,
    // }

    public enum ItemInfinityIconType
    {
        None,
        NoTag, //无横幅，如无限体力
        TagAndInfinity, //横幅+无限标记 如闪电时钟
        TagAndX2, //横幅+x2 如周挑战buff 道具
    }
}