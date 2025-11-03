using System;
using DragonPlus;
using DragonPlus.Config.Team;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;

public partial class EventEnum
{
    public const string TeamLifeChange = "TeamLifeChange";
}
public class EventTeamLifeChange : BaseEvent
{
    public int OldValue;
    public int NewValue;

    public EventTeamLifeChange() : base(EventEnum.TeamLifeChange) { }

    public EventTeamLifeChange(int oldValue,int newValue) : base(EventEnum.TeamLifeChange)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
namespace Scripts.UI
{
    public partial class TeamManager
    {
        
        public int GetLife()
        {
            return Storage.Life;
        }
        public void AddLife(int count,string source)
        {
            var oldValue = Storage.Life;
            Storage.Life += count;
            var newValue = Storage.Life;
            if (oldValue >= TeamConfigManager.Instance.LocalTeamConfig.MaxLife && newValue < TeamConfigManager.Instance.LocalTeamConfig.MaxLife)
            {
                Storage.LifeUpdateTime = (long)APIManager.Instance.GetServerTime();
            }
            EventDispatcher.Instance.SendEventImmediately(new EventTeamLifeChange(oldValue,newValue));

            BiEventAdventureIslandMerge.Types.ItemChangeReason reason =
                BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug;
            if (source == "init")
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CreateProfile;
            }
            else if (source == "recover")
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TeamLifeTime;
            }
            else if (source == "BuyCardGuide")
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TeamLifeCard;
            }
            else if (source == "Debug")
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug;
            }
            else if (source == "BuyCard")
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TeamLifeCard;
            }
            else
            {
                if (Enum.TryParse(source, out BiEventAdventureIslandMerge.Types.ItemChangeReason restoredColor))
                {
                    Debug.LogError($"字符串 '{source}' 成功转为枚举: {restoredColor}");
                    reason = restoredColor;
                }
            }

            GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.TeamLife, count, (ulong) Storage.Life, new GameBIManager.ItemChangeReasonArgs(reason));
        }
        public void UpdateLife()
        {
            if (Storage.Life >= TeamConfigManager.Instance.LocalTeamConfig.MaxLife)
                return;
            var lifeAddUnitTime = TeamConfigManager.Instance.LocalTeamConfig.LifeRecoverTime * (long)XUtility.Min;
            var passTime = (long)APIManager.Instance.GetServerTime() - Storage.LifeUpdateTime;
            var addLife = passTime / lifeAddUnitTime;
            Storage.LifeUpdateTime += addLife * lifeAddUnitTime;
            if (Storage.Life + addLife >= TeamConfigManager.Instance.LocalTeamConfig.MaxLife)
                addLife = TeamConfigManager.Instance.LocalTeamConfig.MaxLife - Storage.Life;
            if (addLife !=0 )
                AddLife((int)addLife, Storage.LifeUpdateTime==0?"init":"recover");
        }
    }
}