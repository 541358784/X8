using System;
using System.Collections.Generic;
using ABTest;
using DragonPlus.Config.AdConfigExtend;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

namespace DragonPlus.ConfigHub.Ad
{
    public enum UserTaskType
    {
        None=0,
        Free,
        Pay,
        Count
    }

    public class AdConfigHandle : Singleton<AdConfigHandle>
    {
        private StorageHome storageHome = null;

        public StorageHome StorageHome
        {
            get
            {
                if (storageHome == null)
                    storageHome = StorageManager.Instance.GetStorage<StorageHome>();

                return storageHome;
            }
        }

        private StorageCommon storageCommon = null;

        public StorageCommon StorageCommon
        {
            get
            {
                if (storageCommon == null)
                    storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

                return storageCommon;
            }
        }

        private List<Common> parsingCommons = null;
        private List<RVAd> parsingRvAds = null;
        private List<InterstitialAd> parsingInterstitialAd = null;

        private float PayMaxAmount
        {
            get { return StorageHome.PayMaxAmount; }
        }

        private int _cacheUserTypeId = 0;
        private Common _cacheUserTypeIdCommon = null;
        
        private int _cacheRvAdsUserTypeId = 0;
        private RVAd _cacheRvAdsUserTypeIdRvAd = null;
        
        private int _cacheInterstitialAdUserTypeId = 0;
        private InterstitialAd _cacheInterstitialAdUserTypeIdInterstitialAd = null;
        
        public int GetServerSubUserType()
        {
            if (!AdConfigManager.Instance.IsRemote)
                return -1;

            return -1;
            // List<ServerUserGroup> config = AdConfigManager.Instance.GetConfig<ServerUserGroup>();
            // if (config == null || config.Count == 0)
            //     return -1;
            //
            // return config[0].SubUserGroup;
        }

        // 11-ios美国
        // 12-ios非美国
        // 21-安卓美国
        // 22-安卓非美国
        public int GetPlatformGroup()
        {
            bool isUs = StorageCommon.Country.ToUpper() == "US";
            bool isIos = storageCommon.Platform == (int) Platform.Ios;

            if (isIos)
            {
                if (isUs)
                    return 11;

                return 12;
            }
            else
            {
                if (isUs)
                    return 21;

                return 22;
            }
        }

        public Common GetCommonByUserTypeId()
        {
            return GetCommonByUserTypeId(AdLocalConfigHandle.Instance.Storage.CurGroup);
        }
        
        
        /// <summary>
        /// 根据userTypeId拿Common数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        private Common GetCommonByUserTypeId(int typeId)
        {
            if (_cacheUserTypeId != typeId || _cacheUserTypeIdCommon == null)
            {
                _cacheUserTypeId = typeId;
                List<Common> commons = AdConfigManager.Instance.GetConfig<Common>();
                if (commons == null || commons.Count == 0)
                    return null;
                _cacheUserTypeIdCommon = commons.Find(x => x.UserTypeId == typeId);
                
                return _cacheUserTypeIdCommon;
            }
            else
            {
                return _cacheUserTypeIdCommon;
            }
        }
        
        public List<Common> GetAdCommons()
        {
            if (!AdConfigManager.Instance.IsRemote)
            {
                if (parsingCommons != null && parsingCommons.Count > 0)
                    return parsingCommons;

                parsingCommons = new List<Common>();
                List<Common> localData = AdConfigManager.Instance.GetConfig<Common>();
                if (localData == null)
                    return parsingCommons;

                foreach (var data in localData)
                {
                    foreach (var group in data.UserGroup)
                    {
                        if (group != UserGroupManager.Instance.UserGroup)
                            continue;

                        parsingCommons.Add(data);
                        break;
                    }
                }

                return parsingCommons;
            }

            return AdConfigManager.Instance.GetConfig<Common>();
        }

        public Common GetCommon()
        {
            return GetCommon(UserGroupManager.Instance.SubUserGroup);
        }

        public bool CanCloseFunction(string funcType)
        {
            Common common = GetCommon();
            if (common == null)
                return false;

            if (common.FunctionOpenType == null || common.FunctionOpenType.Count == 0)
                return false;

            return common.FunctionOpenType.Contains(funcType);
        }
        
        public int GetCommonID()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return 0;

            return commonData.Id;
        }

        public Common GetCommon(int subUserGroup)
        {
            if (ABTestManager.Instance.IsAdLocalConfigPayLevelTest())
            {
                return GetCommonByUserTypeId();
            }
            List<Common> commons = GetAdCommons();
            if (commons == null || commons.Count == 0)
                return null;

            foreach (var comData in commons)
            {
                if (!comData.SubUserGroup.Contains(subUserGroup))
                    continue;

                if (!comData.PlatformGroup.Contains(GetPlatformGroup()))
                    continue;

                if (comData.MaxpayGroup == null || comData.MaxpayGroup.Count == 0)
                    continue;

                if (comData.MaxpayGroup.Count == 1)
                {
                    if (comData.MaxpayGroup[0] < 0)
                        return comData;

                    if (comData.MaxpayGroup[0] < PayMaxAmount)
                        continue;

                    return comData;
                }

                if (comData.MaxpayGroup.Count >= 2)
                {
                    int min = comData.MaxpayGroup[0];
                    int max = comData.MaxpayGroup[1];
                    max = max < 0 ? int.MaxValue : max;

                    if (PayMaxAmount >= min && PayMaxAmount < max)
                        return comData;
                }
            }

            return null;
        }
        
        
        public List<RVAd> GetRvAds()
        {
            if (ABTestManager.Instance.IsAdLocalConfigPayLevelTest())
            {
                if (parsingRvAds != null && parsingRvAds.Count > 0)
                    return parsingRvAds;
                parsingRvAds = new List<RVAd>();
                List<RVAd> localData = AdConfigManager.Instance.GetConfig<RVAd>();
                foreach (var data in localData)
                {
                    if (data.UserTypeId != null && data.UserTypeId.Contains(AdLocalConfigHandle.Instance.Storage.CurGroup))
                    {
                        parsingRvAds.Add(data);
                    }   
                }
                return parsingRvAds;
            }
            if (!AdConfigManager.Instance.IsRemote)
            {
                if (parsingRvAds != null && parsingRvAds.Count > 0)
                    return parsingRvAds;

                parsingRvAds = new List<RVAd>();
                List<RVAd> localData = AdConfigManager.Instance.GetConfig<RVAd>();
                foreach (var data in localData)
                {
                    if(data.UserGroup == null)
                        continue;
                    
                    foreach (var group in data.UserGroup)
                    {
                        if (group != UserGroupManager.Instance.UserGroup)
                            continue;
                    
                        parsingRvAds.Add(data);
                        break;
                    }
                }

                return parsingRvAds;
            }
            return AdConfigManager.Instance.GetConfig<RVAd>();
        }

        public RVAd GetRvAd(int subUserGroup, string placeId)
        {
            List<RVAd> datas = GetRvAds();
            if (datas == null || datas.Count == 0)
                return null;

            foreach (var data in datas)
            {
                if (!data.PlaceId.Equals(placeId))
                    continue;

                if (!data.PlatformGroup.Contains(GetPlatformGroup()))
                    continue;

                if (!data.SubUserGroup.Contains(subUserGroup))
                    continue;

                return data;
            }

            DebugUtil.LogError("GetRvAd  Failed  subUserGroup=" + subUserGroup + " placeId= " + placeId);
            return null;
        }

        public List<InterstitialAd> GetInterstitialAds()
        {
            if (ABTestManager.Instance.IsAdLocalConfigPayLevelTest())
            {
                if (parsingInterstitialAd != null && parsingInterstitialAd.Count > 0)
                    return parsingInterstitialAd;
                parsingInterstitialAd = new List<InterstitialAd>();
                List<InterstitialAd> localData = AdConfigManager.Instance.GetConfig<InterstitialAd>();
                foreach (var data in localData)
                {
                    if (data.UserTypeId != null && data.UserTypeId.Contains(AdLocalConfigHandle.Instance.Storage.CurGroup))
                    {
                        parsingInterstitialAd.Add(data);
                    }   
                }
                return parsingInterstitialAd;
            }
            if (!AdConfigManager.Instance.IsRemote)
            {
                if (parsingInterstitialAd != null && parsingInterstitialAd.Count > 0)
                    return parsingInterstitialAd;

                parsingInterstitialAd = new List<InterstitialAd>();
                List<InterstitialAd> localData = AdConfigManager.Instance.GetConfig<InterstitialAd>();
                foreach (var data in localData)
                {
                    foreach (var group in data.UserGroup)
                    {
                        if (group != UserGroupManager.Instance.UserGroup)
                            continue;
                    
                        parsingInterstitialAd.Add(data);
                        break;
                    }
                }

                return parsingInterstitialAd;
            }

            return AdConfigManager.Instance.GetConfig<InterstitialAd>();
        }

        public InterstitialAd GetInterstitialAd(int subUserGroup, string placeId)
        {
            List<InterstitialAd> datas = GetInterstitialAds();
            if (datas == null || datas.Count == 0)
                return null;

            foreach (var data in datas)
            {
                if (!data.PlaceId.Equals(placeId))
                    continue;

                if (!data.PlatformGroup.Contains(GetPlatformGroup()))
                    continue;

                if (!data.SubUserGroup.Contains(subUserGroup))
                    continue;

                return data;
            }

            return null;
        }

        public List<ResData> GetBonus(string placeId, bool isUpdateIndex = false)
        {
            RVAd rvAd = GetRvAd(UserGroupManager.Instance.SubUserGroup, placeId);
            if (rvAd == null)
                return null;

            return GetBonus(rvAd.Bonus, isUpdateIndex);
        }

        public List<ResData> GetBonus(int id, bool isUpdateIndex = false)
        {
            List<Bonus> datas = AdConfigManager.Instance.GetConfig<Bonus>();
            if (datas == null || datas.Count == 0)
                return null;

            Bonus bonus = datas.Find(a => a.Id == id);
            if (bonus == null)
                return null;

            // 1: 按顺序取 循环
            if (bonus.RewardFormat == 1)
            {
                return GetRewardByIndex(bonus, isUpdateIndex);
            }
            // 2: 随机奖励
            else if (bonus.RewardFormat == 2)
            {
                return GetRewardRandom(bonus);
            }
            // 3:先按顺序在随机
            else if (bonus.RewardFormat == 3)
            {
                return GetRewardByIndexOrRandom(bonus, isUpdateIndex);
            }
            else
            {
                if (bonus.Rewardid != null && bonus.Rewardid.Count > 0)
                {
                    var resDatas = new List<ResData>();
                    for (int i = 0; i < bonus.Rewardid.Count; i++)
                    {
                        ResData resData = new ResData(bonus.Rewardid[i], bonus.Num[i]);
                        resDatas.Add(resData);
                    }

                    return resDatas;
                }
            }

            return null;
        }

        private List<ResData> GetRewardRandom(Bonus bonus)
        {
            if (bonus.Weight == null)
                return null;
            var index = CommonUtils.RandomIndexByWeight(bonus.Weight);
            var resDatas = new List<ResData>();
            if (index >= 0)
            {
                ResData resData = new ResData(bonus.Rewardid[index], bonus.Num[index]);
                resDatas.Add(resData);
            }

            return resDatas;
        }

        //先顺序拿再随机
        private List<ResData> GetRewardByIndexOrRandom(Bonus bonus, bool isUpdateIndex = false)
        {
            List<ResData> resDatas = new List<ResData>();
            if (bonus == null)
                return resDatas;

            string coolTimeKey = "ADRewardKey_" + bonus.Id;
            int index = 0;
            if (UserGroupManager.Instance.storageAdData != null)
            {
                if (UserGroupManager.Instance.storageAdData.AdRewardIndex.ContainsKey(coolTimeKey))
                    index = UserGroupManager.Instance.storageAdData.AdRewardIndex[coolTimeKey];
            }

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                index = 0;
            }
            else
            {
                if (isUpdateIndex)
                {
                    index++;
                    if (index >= bonus.Rewardid.Count)
                    {
                        return GetRewardRandom(bonus);
                    }
                }
            }

            if (index < 0 || index >= bonus.Rewardid.Count)
                index = 0;

            UpdateRewardIndex(coolTimeKey, index);

            ResData resData = new ResData(bonus.Rewardid[index], bonus.Num[index]);
            resDatas.Add(resData);

            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
                CommonUtils.GetTimeStamp());

            return resDatas;
        }

        //顺序循环拿
        private List<ResData> GetRewardByIndex(Bonus bonus, bool isUpdateIndex = false)
        {
            List<ResData> resDatas = new List<ResData>();
            if (bonus == null)
                return resDatas;

            string coolTimeKey = "ADRewardKey_" + bonus.Id;
            int index = 0;
            if (UserGroupManager.Instance.storageAdData != null)
            {
                if (UserGroupManager.Instance.storageAdData.AdRewardIndex.ContainsKey(coolTimeKey))
                    index = UserGroupManager.Instance.storageAdData.AdRewardIndex[coolTimeKey];
            }

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                index = 0;
            }
            else
            {
                if (isUpdateIndex)
                {
                    index++;
                    if (index >= bonus.Rewardid.Count)
                        index = 0;
                }
            }

            if (index < 0 || index >= bonus.Rewardid.Count)
                index = 0;

            UpdateRewardIndex(coolTimeKey, index);

            ResData resData = new ResData(bonus.Rewardid[index], bonus.Num[index]);
            resDatas.Add(resData);

            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
                CommonUtils.GetTimeStamp());

            return resDatas;
        }

        private void UpdateRewardIndex(string coolTimeKey, int index)
        {
            if (UserGroupManager.Instance.storageAdData.AdRewardIndex.ContainsKey(coolTimeKey))
                UserGroupManager.Instance.storageAdData.AdRewardIndex[coolTimeKey] = index;
            else
                UserGroupManager.Instance.storageAdData.AdRewardIndex.Add(coolTimeKey, index);
        }

        public List<RVshopList> GetRvShopListByGroup(int group)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<RVshopList> rVshopLists = AdConfigExtendConfigManager.Instance.GetConfig<RVshopList>();
            if (rVshopLists == null || rVshopLists.Count == 0)
                return null;
            
            return rVshopLists.FindAll(a => a.Group == group);
        }
        
        public RVshopList GetRvShopListByID(int shopId)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<RVshopList> rVshopLists = AdConfigExtendConfigManager.Instance.GetConfig<RVshopList>();
            if (rVshopLists == null || rVshopLists.Count == 0)
                return null;

            return rVshopLists.Find(a => a.Id == shopId);
        }

        public int GetRvSpeedUpFactor()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return 1;

            return commonData.RvSpeedUpFactor;
        }

        public RVshopResource GetRVshopResource(int id)
        {
            List<RVshopResource> rVshopResources = AdConfigExtendConfigManager.Instance.GetConfig<RVshopResource>();
            if (rVshopResources == null || rVshopResources.Count == 0)
                return null;

            return rVshopResources.Find(a => a.Id == id);
        }

        public Global GetGlobal()
        {
            List<Global> adGlobal = AdConfigManager.Instance.GetConfig<Global>();
            if (adGlobal == null || adGlobal.Count == 0)
                return null;
            return adGlobal[0];
        }

        public BuyResource GetBuyResource(int id)
        {
            List<BuyResource> buyResources = AdConfigExtendConfigManager.Instance.GetConfig<BuyResource>();
            if (buyResources == null || buyResources.Count == 0)
                return null;

            return buyResources.Find(a => a.Id == id);
        }

        public List<MysteryGift> GetMysteryGifts()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<MysteryGift> configData = AdConfigExtendConfigManager.Instance.GetConfig<MysteryGift>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.FindAll(a => a.RvGroup == commonData.MysteryGift);
        }

        public List<DailyShop> GetDailyShops()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<DailyShop> configData = AdConfigExtendConfigManager.Instance.GetConfig<DailyShop>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.FindAll(a => a.RvGroup == commonData.DilayShop);
        }

        public List<FlashSale> GetFlashSales(int flashSaleGroup)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<FlashSale> configData = AdConfigExtendConfigManager.Instance.GetConfig<FlashSale>();
            if (configData == null || configData.Count == 0)
                return null;
            if (flashSaleGroup == 0 || configData.Find(a => a.GroupId == flashSaleGroup) == null)
            {
                flashSaleGroup = commonData.FlashSale;
            }

            return configData.FindAll(a => a.GroupId == flashSaleGroup);
        }

        public List<PigSale> GetPigSale()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<PigSale> configData = AdConfigExtendConfigManager.Instance.GetConfig<PigSale>();
            if (configData == null || configData.Count == 0)
                return null;
            var starNum = UserData.Instance.GetTotalDecoCoin();
            var datas= configData.FindAll(a => a.GroupId == commonData.FlashSaleBox);
            int findStarNum = 0;
            for (int i = 0; i <datas.Count; i++)
            {
                if (datas[i].StarNum > starNum)
                {
                    findStarNum = datas[i].StarNum;
                    break;
                }
            }
            return datas.FindAll(a => a.StarNum == findStarNum);
        }

        public FlashSale GetFlashSaleByID(int id)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<FlashSale> configData = AdConfigExtendConfigManager.Instance.GetConfig<FlashSale>();
            if (configData == null || configData.Count == 0)
                return null;
            var flashSale = StorageManager.Instance.GetStorage<StorageHome>().FlashSale;
            if (flashSale.CurConfigGroup == 0 || configData.Find(a => a.GroupId == flashSale.CurConfigGroup) == null)
            {
                flashSale.CurConfigGroup = commonData.FlashSale;
            }

            return configData.Find(a => a.Id == id && a.GroupId == flashSale.CurConfigGroup);
        }

        public List<FlashSaleBox> GetFlashSaleBoxs()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<FlashSaleBox> configData = AdConfigExtendConfigManager.Instance.GetConfig<FlashSaleBox>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.FindAll(a => a.RvGroup == commonData.FlashSaleBox);
        }

        public int GetDailyRewardType()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return 0;

            return commonData.DailyRewardType;
        }

        public int GetLuckBalloonData()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return -1;

            return commonData.LuckyBalloon;
        }

        public int GetLuckBalloonResRatio()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return 0;
        
            return commonData.LuckBalloonResRatio;
        }
        
        public List<int> GetLuckBalloonResIds()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;
        
            return commonData.LuckBalloonResIds;
        }
        
        public int GetHappyGoLuckBalloonData()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return -1;

            return commonData.HgLuckyBalloon;
        }

        public int GetHappyGoLuckBalloonCD()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return -1;

            return commonData.HgLuckyBalloonCD;
        }

        public int GetHappyGoLuckBalloonLevel()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return -1;

            return commonData.HgLuckyBalloonLevel;
        }

        public int GetHappyGoLuckBalloonLevelMax()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return -1;

            return commonData.HgLuckyBalloonLevelMAX;
        }

        public int GetMysteryGiftConsumeData()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return -1;

            return commonData.MysteryGiftConsume;
        }

        public ConsumeExtend GetConsumeExtendID(int id)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<ConsumeExtend> configData = AdConfigExtendConfigManager.Instance.GetConfig<ConsumeExtend>();
            if (configData == null || configData.Count == 0)
                return null;
            return configData.Find(a => a.Id == id);
        }

      
        public List<IceBreakPack> GetIceBreakPacks()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<IceBreakPack> configData = AdConfigExtendConfigManager.Instance.GetConfig<IceBreakPack>();
            if (configData == null || configData.Count == 0)
                return null;
            var iceBreakPackData = StorageManager.Instance.GetStorage<StorageHome>().IceBreakPackData;
            if (iceBreakPackData.CurConfigGroup == 0 ||
                configData.Find(a => a.RvGroup == iceBreakPackData.CurConfigGroup) == null)
            {
                iceBreakPackData.CurConfigGroup = commonData.PackData;
            }

            return configData.FindAll(a => a.RvGroup == iceBreakPackData.CurConfigGroup);
        }

        public List<IceBreakPack> GetIceBreakPacksSecond()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<IceBreakPack> configData = AdConfigExtendConfigManager.Instance.GetConfig<IceBreakPack>();
            if (configData == null || configData.Count == 0)
                return null;
            var iceBreakPackData = StorageManager.Instance.GetStorage<StorageHome>().SecondIceBreakPackData;
            if (iceBreakPackData.StartTime == 0 || iceBreakPackData.CurConfigGroup == 0 ||
                configData.Find(a => a.RvGroup == iceBreakPackData.CurConfigGroup) == null)
            {
                iceBreakPackData.CurConfigGroup = commonData.PackData;
            }

            return configData.FindAll(a => a.RvGroup == iceBreakPackData.CurConfigGroup);
        }

        public IceBreakPack GetIceBreakPackByShopID(int shopID)
        {
            var packs = GetIceBreakPacks();
            if (packs == null)
                return null;
            foreach (var item in packs)
            {
                if (item.ShopItem == shopID)
                    return item;
            }

            return null;
        }

        public IceBreakPack GetIceBreakSecondPackByShopID(int shopID)
        {
            var packs = GetIceBreakPacksSecond();
            if (packs == null)
                return null;
            foreach (var item in packs)
            {
                if (item.ShopItem == shopID)
                    return item;
            }

            return null;
        }
        

        public PigBank GetPigBank(int id)
        {
            List<PigBank> configData = AdConfigExtendConfigManager.Instance.GetConfig<PigBank>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == id);
        }
        

        public int GetMasterCardId()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return -1;

            return commonData.MasterCardId;
        }

        public MasterCardList GetMasterCardList(int id)
        {
            List<MasterCardList> configData = AdConfigExtendConfigManager.Instance.GetConfig<MasterCardList>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == id);
        }

        public List<MasterCardResource> GetMasterCardDatas(int id)
        {
            MasterCardList masterCardList = GetMasterCardList(id);
            if (masterCardList == null)
                return null;

            List<MasterCardResource> configData = AdConfigExtendConfigManager.Instance.GetConfig<MasterCardResource>();
            if (configData == null || configData.Count == null)
                return null;

            List<MasterCardResource> linkDatas = new List<MasterCardResource>();

            for (int i = 0; i < masterCardList.ListData.Count; i++)
            {
                int dataId = masterCardList.ListData[i];

                MasterCardResource data = configData.Find(a => a.Id == dataId);
                if (data == null)
                    continue;

                linkDatas.Add(data);
            }

            return linkDatas;
        }

        public List<TaskAssist> GetTaskAssistPacks()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<TaskAssist> configData = AdConfigExtendConfigManager.Instance.GetConfig<TaskAssist>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.FindAll(a => a.RvGroup == commonData.TaskAssist);
        }

        public List<TaskAssist> GetTaskAssistPacksByLine(int mergeLine)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<TaskAssist> configData = AdConfigExtendConfigManager.Instance.GetConfig<TaskAssist>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.FindAll(a => a.RvGroup == commonData.TaskAssist && a.MergeLine == mergeLine);
        }

        public TaskAssist GetTaskAssistPackById(int id)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<TaskAssist> configData = AdConfigExtendConfigManager.Instance.GetConfig<TaskAssist>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == id);
        }

        public DailyPackPrice GetDailyPackPrice()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<DailyPackPrice> configData = AdConfigExtendConfigManager.Instance.GetConfig<DailyPackPrice>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == commonData.DailyPackContain);
        }

        public DailyPack GetDailyPack(int group)
        {
            List<DailyPack> configData = AdConfigExtendConfigManager.Instance.GetConfig<DailyPack>();
            if (configData == null || configData.Count == 0)
                return null;
            foreach (var config in configData)
            {
                if (config.Groupid == group)
                {
                    return config;
                }
            }

            DebugUtil.LogError("GetDailyPack 策划表有误 ---------group " + group );
            return configData[0];
        }

        public UserTaskType GetUserTaskType()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return UserTaskType.Free;

            return (UserTaskType) commonData.TaskType;
        }

        public DailyBonus GetDailyBonus(int day)
        {
            List<DailyBonus> configData = AdConfigExtendConfigManager.Instance.GetConfig<DailyBonus>();
            if (configData == null || configData.Count == 0)
                return null;
            var storageHome=StorageManager.Instance.GetStorage<StorageHome>();
            if(storageHome.DailyBonus.TotalClaimDay<7)
                return configData.Find(a => a.Id == day);
            return configData.Find(a => a.Id == day+7);
        }

        public  List<DailyBonusChest> GetDailyBonusChest()
        {
            List<DailyBonusChest> configData = AdConfigExtendConfigManager.Instance.GetConfig<DailyBonusChest>();
            return configData;
        }
        
        public List<int> GetDailyRankDefaultValue()
        {
            var common = GetCommon();
            if (common == null)
            {
                var defaultRank = new List<int>();
                defaultRank.Add(1000);
                defaultRank.Add(1000);
                defaultRank.Add(1000);
                defaultRank.Add(1000);

                return defaultRank;
            }
            
            return common.DailyRank;
        }
        
        public DailyPackInfo GetDailyPackInfoById(int id)
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<DailyPackInfo> configData = AdConfigExtendConfigManager.Instance.GetConfig<DailyPackInfo>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == id);
        }
        
        public List<DailyPackPriceRules> GetDailyPackPriceRules()
        {
            List<DailyPackPriceRules> configData = AdConfigExtendConfigManager.Instance.GetConfig<DailyPackPriceRules>();

            return configData;
        }        
        public List<RvShopPriceRules> GetRvShopPriceRules()
        {
            List<RvShopPriceRules> configData = AdConfigExtendConfigManager.Instance.GetConfig<RvShopPriceRules>();

            return configData;
        }
        public RvShopPrice GetRvShopPrice()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<RvShopPrice> configData = AdConfigExtendConfigManager.Instance.GetConfig<RvShopPrice>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == commonData.RvshopList);
        }        
        
        public List<MysteryPack> GetMysteryGiftPack()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<MysteryPack> configData = AdConfigExtendConfigManager.Instance.GetConfig<MysteryPack>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.FindAll(a => a.Groupid == commonData.MysteryPack);
        } 
        public MysteryPack GetMysteryGiftPack(int id)
        {
          
            List<MysteryPack> configData = AdConfigExtendConfigManager.Instance.GetConfig<MysteryPack>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == id);
        }   
        public List<BalloonPack> GetBallPack()
        {
            Common commonData = GetCommon();
            if (commonData == null)
                return null;

            List<BalloonPack> configData = AdConfigExtendConfigManager.Instance.GetConfig<BalloonPack>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.FindAll(a => a.Groupid == commonData.BalloonPack);
        } 
        public BalloonPack GetBallPack(int id)
        {
          
            List<BalloonPack> configData = AdConfigExtendConfigManager.Instance.GetConfig<BalloonPack>();
            if (configData == null || configData.Count == 0)
                return null;

            return configData.Find(a => a.Id == id);
        }
    }
}