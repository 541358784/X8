using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public partial class KeepPetModel
{
    public bool GetBuilding(int buildingItemId)
    {
        if (!BuildingItemConfig.TryGetValue(buildingItemId,out var buildingItemConfig))
            return false;
        var hangPoint = BuildingHangPointConfig[buildingItemConfig.HangPoint];
        if (Storage.BuildingCollectState.TryAdd(buildingItemId, true))
        {
            EventDispatcher.Instance.SendEventImmediately(new EventKeepPetGetNewBuildingItem(buildingItemConfig));
            ChangeActiveBuildingItem(buildingItemId);
        }
        return true;
    }

    public void ChangeActiveBuildingItem(int buildingItemId)
    {
        if (!BuildingItemConfig.TryGetValue(buildingItemId,out var buildingItemConfig))
            return;
        if (!Storage.BuildingCollectState.TryGetValue(buildingItemId, out var state) || !state)
            return;
        var hangPoint = BuildingHangPointConfig[buildingItemConfig.HangPoint];
        if (Storage.BuildingActiveState.ContainsKey(hangPoint.Id))
        {
            var oldItem = BuildingItemConfig[Storage.BuildingActiveState[hangPoint.Id]];
            Storage.BuildingActiveState[hangPoint.Id] = buildingItemConfig.Id;
            var newItem = buildingItemConfig;
            EventDispatcher.Instance.SendEventImmediately(new EventKeepPetBuildingActiveChange(hangPoint,oldItem,newItem));
        }
        else
        {
            Storage.BuildingActiveState.Add(hangPoint.Id,buildingItemConfig.Id);
            var newItem = buildingItemConfig;
            EventDispatcher.Instance.SendEventImmediately(new EventKeepPetBuildingActiveChange(hangPoint,null,newItem));
        }
    }
}