using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509.Qualified;
using Difference;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using Manager;
using Newtonsoft.Json;
using TotalRecharge_New;
using UnityEngine;

public class GlobalConfigManager : Singleton<GlobalConfigManager>
{
    public Dictionary<string, TableGlobalConfigNumber> tableGlobal_Config_Number =
        new Dictionary<string, TableGlobalConfigNumber>();

    private Dictionary<int, TableItem> tableItemConfigMapping = new Dictionary<int, TableItem>();

    private List<TableShop> tableShops = new List<TableShop>();

    private Dictionary<int, TableShop> tableShopsDic = new Dictionary<int, TableShop>();

    private List<TableGuide> tableGuides = new List<TableGuide>();
    private List<TableGuide> tableGuides_2 =null;
    private List<TableGuide> tableGuides_3 =null;
    private List<TableGuide> tableGuides_4 =null;
    private List<TableGuide> tableGuides_miniGame =null;
    private Dictionary<int, List<TableGuide>> tableGuidesMap = new Dictionary<int, List<TableGuide>>();
    
    private List<TablePushNotification> tablePushNotification = new List<TablePushNotification>();

    private Dictionary<int, TableSound> tableSoundsDic = new Dictionary<int, TableSound>();

    private List<TableBundle> tableBundles = null;
    private Dictionary<int, TableBundle> tableBundlesDic = new Dictionary<int, TableBundle>();

    private List<TableBoost> tableBoots = null;
    private Dictionary<int, TableBoost> tableBootsDic = new Dictionary<int, TableBoost>();

    private List<TableStory> tableStory = null;
    private Dictionary<int, TableStory> tableStoryDic = new Dictionary<int, TableStory>();
    private Dictionary<string, TableStory> tableStoryMaping = new Dictionary<string, TableStory>();

    private Dictionary<int, TableRoles> tableRolesDic = new Dictionary<int, TableRoles>();
    private Dictionary<int, TableRoleColor> tableRoleColorsDic = new Dictionary<int, TableRoleColor>();

    public List<TableAvatar> _tableAvatars;
    public Dictionary<int, TableAvatarFrame> TableAvatarFrames = new Dictionary<int, TableAvatarFrame>();

    private Dictionary<int, TableStoryMovie> tableStoryMovieDic= new Dictionary<int, TableStoryMovie>();
    private Dictionary<string, TableStoryMovie> tableStoryMovieMaping = new Dictionary<string, TableStoryMovie>();
    
    public List<TablePayRebateLocal> tablePayRebateLocals;
    
    public List<TableEnergyPackage> tableEnergyPackages;

    public Dictionary<int, TableNewDailyPackGroupConfig> tableNewDailyPackGroupConfig =
        new Dictionary<int, TableNewDailyPackGroupConfig>();
    public Dictionary<int, TableNewDailyPackLevelConfig> tableNewDailyPackLevelConfig =
        new Dictionary<int, TableNewDailyPackLevelConfig>();
    public Dictionary<int, TableNewDailyPackPackageConfig> tableNewDailyPackPackageConfig =
        new Dictionary<int, TableNewDailyPackPackageConfig>();

    public List<TableNewDailyPackLevelChangeConfig> tableNewDailyPackLevelChangeConfig;
    

    public List<TableMiniGameGroup> TableMiniGameGroupConfig;
    public List<TableMiniGameGroup> TableMiniGameGroupNewConfig;
    public Dictionary<int, List<TableMiniGameItem>> MiniGameGroupDictionary = new Dictionary<int, List<TableMiniGameItem>>();
    public Dictionary<int, List<TableMiniGameItem>> MiniGameNewGroupDictionary = new Dictionary<int, List<TableMiniGameItem>>();
    
    public Dictionary<int,TableMiniGameItem> TableMiniGameItemConfig = new Dictionary<int, TableMiniGameItem>();
    public Dictionary<int,TableExtraOrderRewardCouponConfig> TableExtraOrderRewardCouponConfig = new Dictionary<int, TableExtraOrderRewardCouponConfig>();

    public Dictionary<int, TableLevelUpPackageLevelConfig> TableLevelUpPackageLevelConfig =
        new Dictionary<int, TableLevelUpPackageLevelConfig>();
    public Dictionary<int, TableLevelUpPackageContentConfig> TableLevelUpPackageContentConfig =
        new Dictionary<int, TableLevelUpPackageContentConfig>();

    public TableExchangeEnergy _exchangeEnergy;
    
    public List<TableWeeklyCard> tableWeeklyCards = new List<TableWeeklyCard>();
    // public Dictionary<int, TablePayLevelConfig> TablePayLevelConfig = new Dictionary<int, TablePayLevelConfig>();
    public Dictionary<int, TablePayLevelIgnoreConfig> TablePayLevelIgnoreConfig = new Dictionary<int, TablePayLevelIgnoreConfig>();
    public List<TablePayLevelGlobalConfig> TablePayLevelGlobalConfig;
    public List<TablePayLevelStartLevelConfig> TablePayLevelStartLevelConfig;
    public List<TableMiniGameSetting> MiniGameSettingList;
    public Dictionary<int, TableBuyDiamondTicket> TableBuyDiamondTickets = new Dictionary<int, TableBuyDiamondTicket>();
    
    public List<TableEnergyTorrent> TableEnergyTorrentList;
    private List<TableTotalRechargeNew> TableTotalRechargeNewList;
    public List<TableTotalRechargeNewReward> TableTotalRechargeNewRewardsList;
    public List<TableNewIceBreakGiftBag> TableNewIceBreakGiftBagList;

    public List<TableVipStoreSetting> _vipStoreSettings;
    public Dictionary<int, List<TableVipStore>> _vipStoreMap = new Dictionary<int, List<TableVipStore>>();
    public Dictionary<int, List<TableTotalRechargeNew>> _configTotalRechargeMap = new Dictionary<int, List<TableTotalRechargeNew>>();

    public List<TableNewNewIceBreakPackGlobal> NewNewIceBreakPackGlobalList;
    public List<TableNewNewIceBreakPackReward> NewNewIceBreakPackRewardList;

    public List<TableNoAdsGiftBag> NoAdsGiftBagList;
    public List<TableLocalPigBox> LocalPigBoxList;
    public List<TableEndlessEnergyGiftBagReward> EndlessEnergyGiftBagRewardList;
    public List<TableEndlessEnergyGiftBagGlobal> EndlessEnergyGiftBagGlobalList;
    
    public void InitTableConfigs()
    {
        tableStoryMaping.Clear();
        tableStoryMovieMaping.Clear();
        tableGuidesMap.Clear();
        _vipStoreMap.Clear();
        
        
        TableManager.Instance.InitLocation("configs/global");
        InitGlobal_Config_Number(tableGlobal_Config_Number);

        InitTable(tableItemConfigMapping);

        tableShops = TableManager.Instance.GetTable<TableShop>();
        InitTable<TableShop>(tableShopsDic);

        tableGuides = TableManager.Instance.GetTable<TableGuide>();
        
        TextAsset json2 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/global","guide_2"));
        tableGuides_2 = TableManager.DeSerialize<TableGuide>(json2.text);
            
        TextAsset json3 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/global","guide_3"));
        tableGuides_3 = TableManager.DeSerialize<TableGuide>(json3.text);
        
        TextAsset json4 = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/global","guide_4"));
        tableGuides_4 = TableManager.DeSerialize<TableGuide>(json4.text);
        
        TextAsset jsonMiniGame = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/global","guide_minigame"));
        tableGuides_miniGame = TableManager.DeSerialize<TableGuide>(jsonMiniGame.text);
            
        tableGuides.ForEach(a =>
        {
            if (!tableGuidesMap.ContainsKey(a.triggerPosition))
                tableGuidesMap[a.triggerPosition] = new List<TableGuide>();
            
            var guides = tableGuidesMap[a.triggerPosition];
            guides.Add(a);
        });

        foreach (var kv in tableGuidesMap)
        {
            kv.Value.Sort((a, b) => a.id - b.id);
        }
        
        tablePushNotification = TableManager.Instance.GetTable<TablePushNotification>();

        InitTable<TableSound>(tableSoundsDic);

        tableBundles = TableManager.Instance.GetTable<TableBundle>();
        InitTable<TableBundle>(tableBundlesDic);

        tableBoots = TableManager.Instance.GetTable<TableBoost>();
        InitTable<TableBoost>(tableBootsDic);

        tableStory = TableManager.Instance.GetTable<TableStory>();
        InitTable<TableStory>(tableStoryDic);
        tableStory.ForEach(a =>
        {
            if (a.triggerPosition > 0)
            {
                string storyKey = GetStoryKey(a);

                try
                {
                    if(!storyKey.IsEmptyString())
                        tableStoryMaping.Add(storyKey, a);
                }
                catch (Exception e)
                {
                    Debug.LogError("重复key " + a.id + "\t" + storyKey);
                }
            }
        });

        var movieData = TableManager.Instance.GetTable<TableStoryMovie>();
        InitTable<TableStoryMovie>(tableStoryMovieDic);
        movieData.ForEach(a =>
        {
            if (a.triggerPosition > 0)
            {
                var keys = GetStoryKey(a);

                if (keys != null && keys.Count > 0)
                {
                    keys.ForEach(key =>
                    {
                        try
                        {
                            if (!key.IsEmptyString())
                                tableStoryMovieMaping.Add(key, a);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("重复key " + a.id + "\t" + key);
                        }
                    });
                }
            }
        });
        
        
        InitTable<TableRoles>(tableRolesDic);
        InitTable<TableRoleColor>(tableRoleColorsDic);
        
        _tableAvatars= TableManager.Instance.GetTable<TableAvatar>();
        InitTable<TableAvatarFrame>(TableAvatarFrames);
        tablePayRebateLocals = TableManager.Instance.GetTable<TablePayRebateLocal>();
        tableEnergyPackages = TableManager.Instance.GetTable<TableEnergyPackage>();
        
        InitTable<TableNewDailyPackGroupConfig>(tableNewDailyPackGroupConfig);
        InitTable<TableNewDailyPackLevelConfig>(tableNewDailyPackLevelConfig);
        InitTable<TableNewDailyPackPackageConfig>(tableNewDailyPackPackageConfig);
        tableNewDailyPackLevelChangeConfig = TableManager.Instance.GetTable<TableNewDailyPackLevelChangeConfig>();
        

        TableMiniGameGroupConfig = new List<TableMiniGameGroup>();
        TableMiniGameGroupNewConfig = new List<TableMiniGameGroup>();
        {
            var configList = TableManager.Instance.GetTable<TableMiniGameGroup>();
            for (var i = 0; i < configList.Count; i++)
            {
                var config = configList[i];
                if (config.newPlayer)
                {
                    TableMiniGameGroupNewConfig.Add(config);
                }
                else
                {
                    TableMiniGameGroupConfig.Add(config);
                }
            }
        }
        InitTable<TableMiniGameItem>(TableMiniGameItemConfig);
        InitTable<TableExtraOrderRewardCouponConfig>(TableExtraOrderRewardCouponConfig);
        
        InitTable<TableLevelUpPackageLevelConfig>(TableLevelUpPackageLevelConfig);
        InitTable<TableLevelUpPackageContentConfig>(TableLevelUpPackageContentConfig);
        
        tableWeeklyCards = TableManager.Instance.GetTable<TableWeeklyCard>();
        var energies= TableManager.Instance.GetTable<TableExchangeEnergy>();
        _exchangeEnergy = energies[0];
        
        // InitTable(TablePayLevelConfig);
        InitTable(TablePayLevelIgnoreConfig);
        TablePayLevelGlobalConfig = TableManager.Instance.GetTable<TablePayLevelGlobalConfig>();
        TablePayLevelStartLevelConfig = TableManager.Instance.GetTable<TablePayLevelStartLevelConfig>();
        MiniGameSettingList = TableManager.Instance.GetTable<TableMiniGameSetting>();
        TableEnergyTorrentList = TableManager.Instance.GetTable<TableEnergyTorrent>();
        TableTotalRechargeNewList = TableManager.Instance.GetTable<TableTotalRechargeNew>();
        
        MiniGameGroupDictionary.Clear();
        MiniGameNewGroupDictionary.Clear();
        foreach (var tableMiniGameGroup in TableMiniGameGroupConfig)
        {
            foreach (var itemId in tableMiniGameGroup.itemList)
            {
                var itemConfig = TableMiniGameItemConfig[itemId];
                if (!MiniGameGroupDictionary.ContainsKey(itemConfig.configType))
                    MiniGameGroupDictionary[itemConfig.configType] = new List<TableMiniGameItem>();
                
                MiniGameGroupDictionary[itemConfig.configType].Add(itemConfig);
            }
        }
        foreach (var tableMiniGameGroup in TableMiniGameGroupNewConfig)
        {
            foreach (var itemId in tableMiniGameGroup.itemList)
            {
                var itemConfig = TableMiniGameItemConfig[itemId];
                if (!MiniGameNewGroupDictionary.ContainsKey(itemConfig.configType))
                    MiniGameNewGroupDictionary[itemConfig.configType] = new List<TableMiniGameItem>();
                
                MiniGameNewGroupDictionary[itemConfig.configType].Add(itemConfig);
            }
        }
        InitTable(TableBuyDiamondTickets);
        
        TableTotalRechargeNewRewardsList= TableManager.Instance.GetTable<TableTotalRechargeNewReward>();
        TableNewIceBreakGiftBagList = TableManager.Instance.GetTable<TableNewIceBreakGiftBag>();
        
        _vipStoreSettings =  TableManager.Instance.GetTable<TableVipStoreSetting>();
        var vipStore = TableManager.Instance.GetTable<TableVipStore>();
        _vipStoreMap.Clear();
        foreach (var tableVipStore in vipStore)
        {
            if(!_vipStoreMap.ContainsKey(tableVipStore.storeid))
                _vipStoreMap.Add(tableVipStore.storeid, new List<TableVipStore>());
            
            _vipStoreMap[tableVipStore.storeid].Add(tableVipStore);
        }
        
        _configTotalRechargeMap.Clear();
        foreach (var config in TableTotalRechargeNewList)
        {
            if (!_configTotalRechargeMap.ContainsKey(config.payLevelGroup))
                _configTotalRechargeMap[config.payLevelGroup] = new List<TableTotalRechargeNew>();
            
            _configTotalRechargeMap[config.payLevelGroup].Add(config);
        }
        
        NewNewIceBreakPackGlobalList = TableManager.Instance.GetTable<TableNewNewIceBreakPackGlobal>();
        NewNewIceBreakPackRewardList = TableManager.Instance.GetTable<TableNewNewIceBreakPackReward>();
        
        NoAdsGiftBagList = TableManager.Instance.GetTable<TableNoAdsGiftBag>();
        LocalPigBoxList = TableManager.Instance.GetTable<TableLocalPigBox>();
        
        EndlessEnergyGiftBagRewardList = TableManager.Instance.GetTable<TableEndlessEnergyGiftBagReward>();
        EndlessEnergyGiftBagGlobalList = TableManager.Instance.GetTable<TableEndlessEnergyGiftBagGlobal>();
    }

    public TableNewDailyPackGroupConfig GetNewDailyPackGroupConfig(int id)
    {
        if (tableNewDailyPackGroupConfig.TryGetValue(id, out var config))
            return config;
        return null;
    }
    public TableNewDailyPackLevelConfig GetNewDailyPackLevelConfig(int id)
    {
        if (tableNewDailyPackLevelConfig.TryGetValue(id, out var config))
            return config;
        return null;
    }
    public TableNewDailyPackPackageConfig GetNewDailyPackPackageConfig(int id)
    {
        if (tableNewDailyPackPackageConfig.TryGetValue(id, out var config))
            return config;
        return null;
    }
    public List<TableNewDailyPackLevelChangeConfig> GetNewDailyPackLevelChangeConfig()
    {
        return tableNewDailyPackLevelChangeConfig;
    }

    private void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        Debug.LogWarning($"Load Config[ {typeof(T)} ] ");
        List<T> tableData = TableManager.Instance.GetTable<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }

    private void InitGlobal_Config_Number(Dictionary<string, TableGlobalConfigNumber> config)
    {
        if (config == null)
            return;

        List<TableGlobalConfigNumber> tableData = TableManager.Instance.GetTable<TableGlobalConfigNumber>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (TableGlobalConfigNumber kv in tableData)
        {
            config.Add(kv.key, kv);
        }
    }

    public string GetGlobal_Config_Number_Value(string key)
    {
        if (tableGlobal_Config_Number.ContainsKey(key))
            return tableGlobal_Config_Number[key].value;

        return "";
    }

    public int GetNumValue(string key)
    {
        int v = 0;
        int.TryParse(GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(key), out v);
        return v;
    }

    public List<TableShop> GetTableShop()
    {
        return tableShops;
    }

    public List<TableShop> GetTableShopByType(int type)
    {
        return tableShops.FindAll(a =>  a.productType == type);
    }

    public TableShop GetTableShopByID(int id)
    {
        if (!tableShopsDic.ContainsKey(id))
            return null;

        return tableShopsDic[id];
    }

    public TableShop GetTableShopByPruchaseId(string purchaseId)
    {
        return GetTableShop().Find((sc) =>
        {
#if UNITY_IOS
            return purchaseId == sc.product_id_ios;
#else
            return purchaseId == sc.product_id;
#endif
        });
    }

    public TableItem GetTableItem(int id)
    {
        if (tableItemConfigMapping.ContainsKey(id))
            return tableItemConfigMapping[id];

        return null;
    }

    public List<TablePushNotification> GetNotificationConfigs()
    {
        return tablePushNotification;
    }

    public string GetSoundName(int soundId)
    {
        if (!tableSoundsDic.ContainsKey(soundId))
            return "";

        string soundPath = GetSoundPath(soundId);
        if (soundPath.IsEmptyString())
            return tableSoundsDic[soundId].sound_name;
        else
            return soundPath + "/" + tableSoundsDic[soundId].sound_name;
    }

    public string GetSoundPath(int soundId)
    {
        if (!tableSoundsDic.ContainsKey(soundId))
            return "";

        return tableSoundsDic[soundId].sound_path;
    }

    public List<TableBundle> GetTableBundles()
    {
        return tableBundles;
    }

    public TableBundle GetTableBundleByShopID(int shopID)
    {
        foreach (var item in tableBundles)
        {
            if (item.shopItemId == shopID)
                return item;
        }

        return null;
    }

    public List<TableBoost> GetTableBoosts()
    {
        return tableBoots;
    }

    public TableBoost GetTableBoost(int id)
    {
        if (!tableBootsDic.ContainsKey(id))
            return null;

        return tableBootsDic[id];
    }


    public TableStory GetTableStory(int id)
    {
        if (!tableStoryDic.ContainsKey(id))
            return null;
        
        return tableStoryDic[id];
    }

    public TableStory GetTableStory(int triggerPos, string triggerParam)
    {
        string key = GetStoryKey(triggerPos, triggerParam);
        if (key.IsEmptyString())
            return null;

        if (!tableStoryMaping.ContainsKey(key))
            return null;

        return tableStoryMaping[key];
    }
    
    public TableStoryMovie GetTableMovie(int id)
    {
        if (!tableStoryMovieDic.ContainsKey(id))
            return null;
        
        return tableStoryMovieDic[id];
    }
    
    public TableStoryMovie GetTableStoryMovie(int triggerPos, string triggerParam)
    {
        string key = GetStoryKey(triggerPos, triggerParam);
        if (key.IsEmptyString())
            return null;

        if (!tableStoryMovieMaping.ContainsKey(key))
            return null;

        return tableStoryMovieMaping[key];
    }
    
    
    public TableRoles GetTableRole(int id)
    {       
        if (!tableRolesDic.ContainsKey(id))
            return null;
        
        return tableRolesDic[id];
    }

    public TableAvatar GetTableAvatar(int id)
    {
        return _tableAvatars.Find(a => a.id == id);
    }
    
    public List<TableEnergyPackage> GetEnergyPackages()
    {
        var lists = new List<TableEnergyPackage>();
        var payLevelConfig = EnergyPackageModel.Instance.StorageEnergyPackage.GroupId;
        for (var i = 0; i < tableEnergyPackages.Count; i++)
        {
            if (payLevelConfig.Contains(tableEnergyPackages[i].id))
            {
                lists.Add(tableEnergyPackages[i]);
            }
        }
        return lists;
    }
    
    public TableEnergyPackage GetEnergyPackage()
    {
        if (tableEnergyPackages != null && tableEnergyPackages.Count <= 0)
            return null;
        return tableEnergyPackages[0];
    } 
    public TableEnergyPackage GetEnergyPackageByID(int id)
    {
        var packages = GetEnergyPackages();
        if (packages != null && packages.Count <= 0)
            return null;
        return packages.Find(a=>a.shopId==id);
    }
    public List<TableGuide> GetGuidesByPosition(int position)
    {
        if (!tableGuidesMap.ContainsKey(position))
            return null;

        return tableGuidesMap[position];
    }

    public List<TableGuide> GetGuides()
    {
        return tableGuides;
    }
    
    public TableRoleColor GetRoleColor(int id)
    {
        if (!tableRoleColorsDic.ContainsKey(id))
            return null;

        return tableRoleColorsDic[id];
    }   
    public TableEnergyTorrent GetTableEnergyTorrent()
    {
        return TableEnergyTorrentList[0];
    }
    private string GetStoryKey(TableStory config)
    {
        if (config == null)
            return "";
        
        return GetStoryKey(config.triggerPosition, config.triggerParam);
    }
    
    private List<string> GetStoryKey(TableStoryMovie config)
    {
        if (config == null)
            return null;

        List<string> keys = new List<string>();
        if (config.triggerParam == null)
        {
            keys.Add(GetStoryKey(config.triggerPosition, ""));
        }
        else
        {
            foreach (var s in config.triggerParam)
            {
                keys.Add(GetStoryKey(config.triggerPosition, s));
            } 
        }

        return keys;
    }
    
    private string GetStoryKey(int triggerPosition, string triggerParam)
    {
        return string.Format("Pos{0}_Param{1}", triggerPosition, triggerParam);
    }

    
    
    private int[] plan_A = new[]
    {
        110,
        120,
        121,
        123,
        130,
        131,
        140,
        141,
        150,
        160,
        161,
        170,
        171,
        180,
        181,
        182,
        183,
        184,
        185,
        186,
        290,
        291,
        292,
        293,
        294,
    };

    private int[] plan_B = new[]
    {
        200,
        201,
        210,
        211,
        212,
        215,
        220,
        295,
    };
    
    private int[] plan_C = new[]
    {
        // 260,
        // 261,
        // 262,
        234,
    };
    
    public void GuideDataRecombine()
    {
        tableGuidesMap.Clear();
        
        foreach (var id in plan_A)
        {
            var removeData = tableGuides.Find(a => a.id == id);
            if(removeData == null)
                continue;

            tableGuides.Remove(removeData);
        }

        int insertStartIndex = 1;
        int insertCount = 0;
        
        for (var i = 0; i < tableGuides_2.Count; i++)
        {
            insertCount++;
            if (tableGuides_2[i].id == 290)
            {
                insertStartIndex = tableGuides.FindIndex(a=>a.id == 281);
                insertCount = 1;
            }
            
            tableGuides.Insert(insertStartIndex+insertCount, tableGuides_2[i]);
        }

        if (DifferenceManager.Instance.IsDiffPlan_B())
        {
            foreach (var id in plan_B)
            {
                var removeData = tableGuides.Find(a => a.id == id);
                if(removeData == null)
                    continue;

                tableGuides.Remove(removeData);
            }

            foreach (var tableGuide in tableGuides_3)
            {
                for (var i = 0; i < tableGuides.Count; i++)
                {
                    if(tableGuides[i].id != tableGuide.id)
                        continue;

                    tableGuides[i] = tableGuide;
                }
            }
        }

        if (DifferenceManager.Instance.IsDiffPlan_C())
        {
            foreach (var id in plan_C)
            {
                var removeData = tableGuides.Find(a => a.id == id);
                if(removeData == null)
                    continue;

                tableGuides.Remove(removeData);
            }

            int installIndex = tableGuides.FindIndex(a=>a.id == 235);
            if (installIndex > 0)
            {
                for(int i = 0; i < tableGuides_4.Count; i++)
                {
                    tableGuides.Insert(installIndex+i, tableGuides_4[i]);
                }
            }
        }
        
        
        
        tableGuides.ForEach(a =>
        {
            if (!tableGuidesMap.ContainsKey(a.triggerPosition))
                tableGuidesMap[a.triggerPosition] = new List<TableGuide>();
            
            var guides = tableGuidesMap[a.triggerPosition];
            guides.Add(a);
        });
        
        foreach (var kv in tableGuidesMap)
        {
            kv.Value.Sort((a, b) => a.id - b.id);
        }
        
       RecombineMiniGameGuide();
    }

    private void RecombineMiniGameGuide()
    {
        if(GameModeManager.Instance.GetGameMode() != GameModeManager.GameMode.MiniAndMerge)
            return;
        
        tableGuidesMap.Clear();
        
        for (var i = 0; i < tableGuides.Count; i++)
        {
            if (tableGuides[i].id <= 262 || tableGuides[i].id == 339 || tableGuides[i].id == 340)
            {
                tableGuides.RemoveAt(i);
                i--;
            }
        }
        
            
        for (var i = tableGuides_miniGame.Count-1; i >= 0; i--)
        {
            tableGuides.Insert(0, tableGuides_miniGame[i]);
        }
        
        tableGuides.ForEach(a =>
        {
            if (!tableGuidesMap.ContainsKey(a.triggerPosition))
                tableGuidesMap[a.triggerPosition] = new List<TableGuide>();
            
            var guides = tableGuidesMap[a.triggerPosition];
            guides.Add(a);
        });
        
        foreach (var kv in tableGuidesMap)
        {
            kv.Value.Sort((a, b) => a.id - b.id);
        }
    }

    public List<TableVipStore> GetVipStoreInfo(int vipLevel)
    {
        if (!_vipStoreMap.ContainsKey(vipLevel))
            return null;

        return _vipStoreMap[vipLevel];
    }

    public List<TableTotalRechargeNew> GatTotalRechargeConfig()
    {
        int group = TotalRechargeModel_New.Instance.StorageTotalRechargeNew.PayLevelGroup;

        if (_configTotalRechargeMap.ContainsKey(group))
            return _configTotalRechargeMap[group];

        return _configTotalRechargeMap[_configTotalRechargeMap.Keys.First()];
    }
}