using System.Collections.Generic;

namespace TMatch
{
    public class TMGiftBagLinkResourceConfig
    {
        //id
        public int Id { get; set; }

        //奖励id
        public List<int> RewardID { get; set; }

        //奖励数量（无限体力单位为秒）
        public List<int> Amount { get; set; }

        //消耗货币类型 1.广告 2.钻石 3.免费获得 4.充值 5.金币
        public int ConsumeType { get; set; }

        //消耗参数
        public int ConsumeAmount { get; set; }
    }
}