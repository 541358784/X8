using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;

public partial class EventEnum
{
    public const string TeamCoinChange = "TeamCoinChange";
}
public class EventTeamCoinChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventTeamCoinChange() : base(EventEnum.TeamCoinChange) { }

    public EventTeamCoinChange(int oldValue,int newValue) : base(EventEnum.TeamCoinChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
namespace Scripts.UI
{
    public partial class TeamManager
    {
        
        public int GetCoin()
        {
            return Storage.Coin;
        }
        public void AddCoin(int count,string source,int biItemId = -1)
        {
            var oldValue = Storage.Coin;
            Storage.Coin += count;
            var newValue = Storage.Coin;
            EventDispatcher.Instance.SendEventImmediately(new EventTeamCoinChange(oldValue,newValue));
            BiEventAdventureIslandMerge.Types.ItemChangeReason reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug;
            if (source == "ShopBuy")
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TeamStore;
            }
            else
            {
                if (Enum.TryParse(source, out BiEventAdventureIslandMerge.Types.ItemChangeReason restoredColor))
                {
                    Debug.LogError($"字符串 '{source}' 成功转为枚举: {restoredColor}");
                    reason = restoredColor;
                }
            }
            GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.TeamCoin, count, (ulong) Storage.Coin, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
                data1 = biItemId.ToString(),
            });
        }

        private ulong OffsetDay = 3;
        public long NextShopRefreshTime => ((Storage.WeekId+1)* 7 - (long)OffsetDay) * (long)XUtility.DayTime;
        public void UpdateShop()
        {
            var curTime = APIManager.Instance.GetServerTime();
            
            var weekId = (long)((curTime / XUtility.DayTime + OffsetDay)/ 7);
            if (weekId != Storage.WeekId)
            {
                Storage.WeekId = weekId;
                Storage.BuyState.Clear();
            }
        }

        public bool CanBuyItem(int shopItemId)
        {
            if (!HasTeam())
                return false;
            var item = TeamConfigManager.Instance.ShopConfigList.Find(a => a.Id == shopItemId);
            if (item == null)
                return false;
            if (item.RequireLevel > MyTeamInfo.ExtraData.TeamLevel)
                return false;
            if (Storage.BuyState.ContainsKey(item.Id) && Storage.BuyState[item.Id] >= item.BuyTimes)
                return false;
            if (GetCoin() < item.Price)
                return false;
            return true;
        }

        public bool BuyItem(int shopItemId)
        {
            if (!CanBuyItem(shopItemId))
                return false;
            var item = TeamConfigManager.Instance.ShopConfigList.Find(a => a.Id == shopItemId);
            if (!Storage.BuyState.ContainsKey(item.Id))
            {
                Storage.BuyState.Add(item.Id,0);
            }
            Storage.BuyState[item.Id]++;
            AddCoin(-item.Price,"ShopBuy",item.RewardId);
            var rewards = new ResData(item.RewardId, item.RewardCount);
            var itemChangeReason =
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TeamStore);
            UserData.Instance.AddRes(rewards,itemChangeReason);
            if (!UserData.Instance.IsResource(item.RewardId))
            {
                var itemConfig=GameConfigManager.Instance.GetItemConfig(item.RewardId);
                if (itemConfig != null)
                {
                    for (var i = 0; i < item.RewardCount; i++)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonTeamStore,
                            itemAId =itemConfig.id,
                            isChange = true,
                        });   
                    }
                }
            }
            CommonRewardManager.Instance.PopCommonReward(new List<ResData>() { rewards },
                CurrencyGroupManager.Instance.currencyController, false, itemChangeReason);
            return true;
        }
    }
}