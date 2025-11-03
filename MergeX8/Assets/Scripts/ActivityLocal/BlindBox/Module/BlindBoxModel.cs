using System;
using System.Collections.Generic;
using System.Linq;
using ActivityLocal.BlindBox.View;
using DragonPlus;
using DragonPlus.Config.BlindBox;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class BlindBoxModel : Manager<BlindBoxModel>
{
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BlindBox);
    public StorageDictionary<int, StorageBlindBox> StorageDic => StorageManager.Instance.GetStorage<StorageHome>().BlindBox;
    public StorageBlindBoxGlobal StorageGlobal => StorageManager.Instance.GetStorage<StorageHome>().BlindBoxGlobal;

    public StorageBlindBox GetStorage(int themeId)
    {
        if (!ThemeConfigDic.TryGetValue(themeId, out var themeConfig))
            return null;
        if (StorageDic.TryGetValue(themeConfig.Id, out var storage))
            return storage;
        storage = new StorageBlindBox();
        storage.ThemeId = themeConfig.Id;
        StorageDic.Add(storage.ThemeId,storage);
        return storage;
    }

    public Dictionary<int, BlindBoxBoxConfig> BoxConfigDic;
    public Dictionary<int, BlindBoxGroupConfig> GroupConfigDic;
    public Dictionary<int, BlindBoxItemConfig> ItemConfigDic;
    public Dictionary<int, BlindBoxThemeConfig> ThemeConfigDic;
    public List<BlindBoxRecycleShopConfig> RecycleShopConfig => BlindBoxConfigManager.Instance.GetConfig<BlindBoxRecycleShopConfig>();

    public void InitConfig()
    {
        BlindBoxConfigManager.Instance.InitConfig();
        BuildConfigDic(out BoxConfigDic);
        BuildConfigDic(out GroupConfigDic);
        BuildConfigDic(out ItemConfigDic);
        BuildConfigDic(out ThemeConfigDic);
        XUtility.WaitFrames(5, CreateAuxEntrance);
    }
    public void CreateAuxEntrance()
    {
        var storage = GetStorage(4);
        DynamicEntry_Home_BindBoxTheme dynamicEntry = new DynamicEntry_Home_BindBoxTheme(storage.ThemeId);
        DynamicEntryManager.Instance.RegisterDynamicEntry<DynamicEntry_Home_BindBoxTheme>(this, dynamicEntry);

        if (!storage.IsResReady())
        {
            storage.TryDownloadRes((p,s) =>
            {
            }, (success) =>
            {
            });   
        }
    }

    public void BuildConfigDic<T>(out Dictionary<int, T> configDic)where T:TableBase
    {
        configDic = new Dictionary<int, T>();
        var configList = BlindBoxConfigManager.Instance.GetConfig<T>();
        foreach (var config in configList)
        {
            configDic.Add(config.GetID(),config);
        }
    }
    public bool IsBlindBoxId(int resourceId)
    {
        return resourceId >= 751 && resourceId <= 799;
    }

    public int GetBlindBoxCount(int resourceId)
    {
        var boxConfig = BoxConfigDic[resourceId];
        var storage = GetStorage(boxConfig.ThemeId);
        if (storage == null)
            return 0;
        return storage.BlindBoxCount;
    }

    public void AddBlindBox(int resourceId,int addCount)
    {
        var boxConfig = BoxConfigDic[resourceId];
        var storage = GetStorage(boxConfig.ThemeId);
        if (storage == null)
            return;
        storage.BlindBoxCount += addCount;
    }

    public int GetBlindBoxCountAll()
    {
        var totalCount = 0;
        foreach (var pair in ThemeConfigDic)
        {
            totalCount += GetStorage(pair.Key).BlindBoxCount;
        }
        return totalCount;
    }

    public int GetCanCollectGroupCountAll()
    {
        var totalCount = 0;
        foreach (var pair in ThemeConfigDic)
        {
            var storage = GetStorage(pair.Key);
            totalCount += GetCanCollectGroupCount(storage);
        }
        return totalCount;
    }

    public int GetCanCollectGroupCount(StorageBlindBox storage)
    {
        var totalCount = 0;
        var config = ThemeConfigDic[storage.ThemeId];
        foreach (var group in config.GroupList)
        {
            if (storage.CanCollectGroup(GroupConfigDic[group]))
                totalCount++;
        }
        return totalCount;
    }
    

    #region  Entrance
    public Transform GetCommonFlyTarget()
    {
        if (UIStarrySkyCompassMainController.Instance && UIStarrySkyCompassMainController.Instance.BlindBoxEntrance)
        {
            return UIStarrySkyCompassMainController.Instance.BlindBoxEntrance.transform;
        }
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = GetTaskEntrance();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = GetAuxItem();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    public bool ShowAuxItem()
    {
        if (!IsUnlock)
            return false;
        return true;
    }

    public Aux_BlindBoxTheme GetAuxItem()
    {
        return Aux_BlindBoxTheme.Instance;
    }

    public bool ShowTaskEntrance()
    {
        if (!IsUnlock)
            return false;
        return true;
    }
    public MergeBlindBox GetTaskEntrance()
    {
        return MergeTaskTipsController.Instance.MergeBlindBox;
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/BlindBox/Common/Aux_BlindBox";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/BlindBox/Common/TaskList_BlindBox";
    }
    public string GetTaskItemAssetPath(int themeId)
    {
        return "Prefabs/BlindBox/Theme"+themeId+"/TaskList_BlindBox"+themeId;
    }
    public string GetAuxItemAssetPath(int themeId)
    {
        return "Prefabs/BlindBox/Theme"+themeId+"/Aux_BlindBox"+themeId;
    }
    #endregion
    
    
    
    public BlindBoxItemConfig OpenBlindBox(StorageBlindBox storage)
    {
        if (storage.BlindBoxCount == 0)
        {
            return null;
        }
        storage.BlindBoxCount--;
        storage.TotalCollectTimes++;
        BlindBoxItemConfig collectItem = null;
        var themeConfig = ThemeConfigDic[storage.ThemeId];
        var itemList = themeConfig.ItemList;
        itemList = new List<int>(itemList);
        var specialConfigs = themeConfig.GetSpecialItemConfigs();
        foreach (var specialConfig in specialConfigs)
        {
            itemList.Remove(specialConfig.Id);
        }

        for (var i=0;i<specialConfigs.Count;i++)
        {
            if (!storage.CollectItems.ContainsKey(specialConfigs[i].Id) &&
                storage.TotalCollectTimes >= themeConfig.SpecialTimes[i])
            {
                collectItem = specialConfigs[i];
                break;
            }
        }
        if (collectItem == null)
        {
            storage.CurCollectTimes++;
            var collectTypes = storage.CollectItems.Count;
            foreach (var specialConfig in specialConfigs)
            {
                if (storage.CollectItems.ContainsKey(specialConfig.Id))
                    collectTypes--;   
            }
            if (collectTypes >= themeConfig.RuleMinTimes.Count)
            {
                var item = itemList[Random.Range(0, itemList.Count)];
                collectItem = ItemConfigDic[item];
            }
            else
            {
                var tempItemList = new List<int>();
                var minTimes = themeConfig.RuleMinTimes[collectTypes];
                var maxTimes = themeConfig.RuleMaxTimes[collectTypes];
                var hasUnCollectItem = storage.CurCollectTimes >= minTimes;
                var hasCollectItem = storage.CurCollectTimes < maxTimes;
                foreach (var item in itemList)
                {
                    if (storage.CollectItems.ContainsKey(item))
                    {
                        if (hasCollectItem)
                            tempItemList.Add(item);
                    }
                    else
                    {
                        if (hasUnCollectItem)
                            tempItemList.Add(item);
                    }
                }
                var tempItem = tempItemList[Random.Range(0, tempItemList.Count)];
                collectItem = ItemConfigDic[tempItem];
            }
        }
        var isNew = storage.CollectItem(collectItem);
        if (isNew && !specialConfigs.Contains(collectItem))
        {
            storage.CurCollectTimes = 0;
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBlindBoxOpen,collectItem.Id.ToString());
        return collectItem;
    }

    public void CheckCollectAllItem(StorageBlindBox storage)
    {
        var themeConfig = ThemeConfigDic[storage.ThemeId];
        foreach (var item in themeConfig.ItemList)
        {
            if (!storage.CollectItems.ContainsKey(item))
            {
                return;
            }
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBlindBoxSet,storage.ThemeId.ToString());
    }

    public string BlindBoxCollectStateStr()
    {
        var totalCount=0;
        var collectCount = 0;
        foreach (var pair in ThemeConfigDic)
        {
            totalCount++;
            if (GetStorage(pair.Key).CollectItems.Count >= pair.Value.ItemList.Count)
            {
                collectCount++;
            }
        }
        return collectCount + "/" + totalCount;
    }
}