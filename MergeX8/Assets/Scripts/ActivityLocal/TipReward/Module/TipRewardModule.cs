using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TipReward;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;

namespace ActivityLocal.TipReward.Module
{
    public class TipRewardModule : Singleton<TipRewardModule>
    {
        private const string _coolTimeKey = "CoolTime_TipReward";

        public bool IsDebugOpen = false;
        
        private StorageCoolTime coolTime
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().CoolTimeData;
            }
        }

        public void ResetCoolTime()
        {
            coolTime.IntervalTime[_coolTimeKey] = (long)APIManager.Instance.GetServerTime();;
        }

        public long GetCoolTime()
        {
            if (!coolTime.IntervalTime.ContainsKey(_coolTimeKey))
                ResetCoolTime();

            return coolTime.IntervalTime[_coolTimeKey];
        }
        
        public bool CanShow(StorageTaskItem order, List<ResData> resDatas)
        {
            if (order == null)
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TipReward))
            {
                Debug.LogWarning($"--------------------小费奖励失败 等级不到--------------------");
                return false;
            }
            
            if (MainOrderManager.Instance.IsSpecialTask(order.Id))
            {
                Debug.LogWarning($"--------------------小费奖励失败 特殊任务--------------------");
                return false;
            }
            if (order.Slot > (int)SlotDefinition.SecondRecycle)
            {
                Debug.LogWarning($"--------------------小费奖励失败 任务点位不允许：{order.Slot} --------------------");
                return false;
            }

            if (IsDebugOpen)
            {            
                UIManager.Instance.OpenWindow(UINameConst.UIPopupTipRewardMain, order, resDatas);
                return true;
            }
            var common = AdConfigHandle.Instance.GetCommon();
            if (common == null)
            {
                Debug.LogWarning("--------------------小费奖励失败 获取分层失败--------------------");
                return false;
            }

            if (common.TipReward == 0)
            {
                Debug.LogWarning($"--------------------小费奖励失败 当前分层不可以展示: {common.Id} --------------------");
                return false;
            }
            
            if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.R_TIPS_REWARD))
            {
                Debug.LogWarning("--------------------小费奖励失败 不可以播放RV--------------------");
                return false;
            }

            if (common.TipReward == 1)//类型b不处理插屏
            {
                if (!AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.I_TIPS_REWARD))
                {
                    Debug.LogWarning("--------------------小费奖励失败 不可以播放插屏--------------------");
                    return false;
                }   
            }

            if (common.TipReward == 2)
            {
                //类型b不处理cd
                // if ((long)APIManager.Instance.GetServerTime()/1000 - GetCoolTime()/1000 < TipRewardConfigManager.Instance.TipRewardSetting.PopTimeLimit)
                // {
                //     Debug.LogWarning($"--------------------小费奖励失败 CD时间不到：{(long)APIManager.Instance.GetServerTime() - GetCoolTime()} --------------------");
                //     return false;
                // }

                bool isReachLevel = false;
                var coinCount = 0;
                for (var i = 0; i < order.RewardTypes.Count; i++)
                {
                    if (order.RewardTypes[i] == (int)UserData.ResourceId.Coin)
                    {
                        coinCount += order.RewardNums[i];
                    }
                }
                for (var i = 0; i < order.ExtraRewardTypes.Count; i++)
                {
                    if (order.ExtraRewardTypes[i] == (int)UserData.ResourceId.Coin)
                    {
                        coinCount += order.ExtraRewardNums[i];
                    }
                }

                isReachLevel = coinCount >= TipRewardConfigManager.Instance.TipRewardSetting.OrderMaxLevel;
                //类型b最高道具等级判断换为金币数量判断
                // foreach (var orderItemId in order.ItemIds)
                // {
                //     TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(orderItemId);
                //     if(itemConfig == null)
                //         continue;
                //
                //     if (itemConfig.level >= TipRewardConfigManager.Instance.TipRewardSetting.OrderMaxLevel)
                //     {
                //         isReachLevel = true;
                //         break;
                //     }
                // }

                if (!isReachLevel)
                {
                    Debug.LogWarning($"--------------------小费奖励失败 不满足需求物品最大等级：{TipRewardConfigManager.Instance.TipRewardSetting.OrderMaxLevel} --------------------");
                    return false;
                }
            }
            if (!UIManager.Instance.GetOpenUI(UINameConst.UIPopupTipRewardMain))
                UIManager.Instance.OpenWindow(UINameConst.UIPopupTipRewardMain, order, resDatas);
            return true;
        }
    }
}