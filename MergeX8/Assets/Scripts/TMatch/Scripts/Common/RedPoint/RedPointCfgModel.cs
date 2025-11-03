/*
 * 红点配置 - RedPointCfg
 * @author lu
 */

using System.Collections.Generic;

namespace TMatch
{
    public class RedPointCfgModel : Manager<RedPointCfgModel>
    {
        public Dictionary<string, RedPointConfig> Cfg;

        public void Init()
        {
            Cfg = new Dictionary<string, RedPointConfig>();

            //Cfg["dailyTask"] = new RedPointConfig { };
            //Cfg["achievement"] = new RedPointConfig { };

            Cfg["menu"] = new RedPointConfig();                                                       
            Cfg[RedPointCenter.DailyBonusRedPointKey] = new RedPointConfig();
            Cfg["menu.fbfriends"] = new RedPointConfig();
            Cfg["menu.set"] = new RedPointConfig();
            Cfg[RedPointCenter.ContactUsRedPointKey] = new RedPointConfig();

            Cfg["store"] = new RedPointConfig();
            Cfg["storeNewChef"] = new RedPointConfig();
            Cfg["cookingFund"] = new RedPointConfig();
            Cfg["adstask"] = new RedPointConfig();
            Cfg["menu.mailbox.system"] = new RedPointConfig();
            Cfg[RedPointCenter.SelectMapKey] = new RedPointConfig();
            Cfg[RedPointCenter.RedPointType_RvShop] = new RedPointConfig();
            Cfg[RedPointCenter.RedPointTypeWinStreak2] = new RedPointConfig();
            Cfg[RedPointCenter.BPEvent] = new RedPointConfig();
            Cfg[RedPointCenter.BPEventOther] = new RedPointConfig();
            Cfg[RedPointCenter.BPReward] = new RedPointConfig();
            Cfg[RedPointCenter.BPRewardOther] = new RedPointConfig();
            Cfg[RedPointCenter.BPRankReward] = new RedPointConfig();
            Cfg[RedPointCenter.BPRankRewardOther] = new RedPointConfig();
            Cfg[RedPointCenter.PassLevelLB] = new RedPointConfig();
            
            //Cfg["endless"] = new RedPointConfig();
        }
    }

    public class RedPointConfig
    {
        private bool _leaf = true;
        private bool _isSumValue = true;

        // 是否是叶子节点,非叶子节点不能设置
        public bool Leaf
        {
            get
            {
                return _leaf;
            }
            set
            {
                _leaf = value;
            }
        }
        // 该节点的上级节点
        public RedPointConfig Parent { get; set; }
        // 该节点的下级节点
        public Dictionary<string, RedPointConfig> Children { get; set; }
        // 同Key,初始化的时候赋值
        public string Key { get; set; }
        // 当前红点累积值
        public int Value { get; set; }
        // 索引
        public int Index { get; set; }
        // 是否累积计数
        public bool IsSumValue
        {
            get
            {
                return _isSumValue;
            }
            set
            {
                _isSumValue = value;
            }
        }
    }
}
