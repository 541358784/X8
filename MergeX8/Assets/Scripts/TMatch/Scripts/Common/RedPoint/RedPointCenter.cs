/*
 * 红点中心 - RedPointCenter
 * @author lu
 */

using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using UnityEngine;

namespace TMatch
{
    public class RedPointCenter : Manager<RedPointCenter>
    {
        public string EventKey = "RedPoint:";
        private Dictionary<string, RedPointConfig> _redPointCfgs;
        public const string ContactUsRedPointKey = "menu.set.contact_us";
        public const string DailyBonusRedPointKey = "menu.dailybonus";
        public const string SelectMapKey = "SelectMapKey";
        public const string RedPointType_RvShop = "RedPointType_RvShop";
        public const string RedPointTypeWinStreak2 = "RedPointTypeWinStreak2";
        public const string BPEvent = "bpevent";
        public const string BPEventOther = "bpeventother"; //与BPEvent交替应用
        public const string BPRankReward = "bpevent.bprankreward"; //bp排行榜奖励
        public const string BPRankRewardOther = "bpeventother.bprankreward"; //bp排行榜奖励
        public const string BPReward = "bpevent.bpreward"; //bp奖励
        public const string BPRewardOther = "bpeventother.bpreward"; //bp奖励
        public const string PassLevelLB = "passlevellb"; //通关排行榜

        public void Init()
        {
            if (_redPointCfgs == null)
            {
                RedPointCfgModel.Instance.Init();
                _redPointCfgs = RedPointCfgModel.Instance.Cfg;
                InitCfg();
            }
        }

        // 设置红点的值，该节点必须是叶子节点
        public void Set(string key, int value = 1)
        {
            if (!_redPointCfgs.ContainsKey(key))
            {
                DebugUtil.Log("红点中心: 不存在红点Key: " + key);
                return;
            }

            RedPointConfig curData = FindKey(key, true);
            if (!curData.Leaf)
            {
                DebugUtil.Log("红点中心: 该key不是叶子节点(leaf=true为叶子节点)不能调用set方法：" + key);
                return;
            }

            if (curData.Value == value)
            {
                // 值未发生变化

                return;
            }

            curData.Value = value;

            // 发送事件
            StartCoroutine(DispatchRedEvent(curData, value));
            DebugUtil.Log("红点中心: 修改红点 " + curData.Key + ",值为: " + value);

            // 一层一层向上找
            Upup(curData);
        }

        IEnumerator DispatchRedEvent(RedPointConfig curData, int value)
        {
            yield return new WaitForEndOfFrame();

            EventDispatcher.Instance.DispatchEvent(new RedPointEvent(curData.Index, value));
        }

        // 得到红点儿的值
        public int Get(string key)
        {
            var curData = FindKey(key, false);
            if (curData != null)
            {
                return curData.Value;
            }

            return 0;
        }


        /* 查找Key,为带*的配置预留
            @param key 就是key
            @param created 如果中途没有，是否要创建
        */
        public RedPointConfig FindKey(string key, bool created = false)
        {
            RedPointConfig config = null;
            if (_redPointCfgs.TryGetValue(key, out config))
            {
                return config;
            }

            return null;
        }

        void Upup(RedPointConfig curData)
        {
            // 往上层设置
            while (curData.Parent != null && curData.Parent.Key != null)
            {
                curData = curData.Parent;
                int sum = 0;
                foreach (string k in curData.Children.Keys)
                {
                    var v = curData.Children[k];
                    sum = sum + v.Value;
                }

                // 只要有一层是不需要设置的，那么往上全是不需要设置的
                if (curData.IsSumValue && curData.Value == sum)
                {
                    break;
                }

                // 只要有一层是不需要设置的，那么往上全是不需要设置的
                if ((!curData.IsSumValue) &&
                    (
                        (curData.Value == 0 && sum == 0) ||
                        (curData.Value == 1 && sum > 0))
                   )
                {
                    break;
                }

                // 设置值
                if (curData.IsSumValue)
                {
                    curData.Value = sum;
                }
                else if (sum > 0)
                {
                    curData.Value = 1;
                }
                else
                {
                    curData.Value = 0;
                }

                if (curData.Key != null)
                {
                    // 发送事件
                    EventDispatcher.Instance.DispatchEvent(new RedPointEvent(curData.Index, curData.Value));
                }
            }
        }

        // 处理配置表
        void InitCfg()
        {
            int sumIndex = 0;
            foreach (var cfg in _redPointCfgs)
            {
                var keys = cfg.Key.Split('.');
                // 在value里保存key
                cfg.Value.Key = cfg.Key;
                // 初始红点累积值
                cfg.Value.Value = 0;
                // 初始化索引
                cfg.Value.Index = sumIndex;
                sumIndex++;
                // 初始化子节点字典
                cfg.Value.Children = new Dictionary<string, RedPointConfig>();

                if (keys.Length > 1)
                {
                    var keyCon = keys[0];
                    var tempCfg = _redPointCfgs[keys[0]];

                    for (int index = 1; index < keys.Length; index++)
                    {
                        keyCon += "." + keys[index];
                        if (_redPointCfgs.ContainsKey(keyCon))
                        {
                            if (index == keys.Length - 1 && _redPointCfgs[keyCon].Parent == null)
                            {
                                // 在value里保存上级的key
                                _redPointCfgs[keyCon].Parent = tempCfg;
                                _redPointCfgs[keyCon].Parent.Leaf = false;
                                tempCfg.Children[keyCon] = _redPointCfgs[keyCon];
                            }
                            else
                            {
                                tempCfg = _redPointCfgs[keyCon];
                            }
                        }
                    }
                }
            }
        }
    };
}