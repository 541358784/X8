/************************************************
 * AdConfigExtend Config Manager class : AdConfigExtendConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.AdConfigExtend
{
    public partial class AdConfigExtendConfigManager : Manager<AdConfigExtendConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<ConsumeExtend> ConsumeExtendList;
        public List<DailyShop> DailyShopList;
        public List<MysteryGift> MysteryGiftList;
        public List<MysteryPack> MysteryPackList;
        public List<PigSale> PigSaleList;
        public List<IceBreakPack> IceBreakPackList;
        public List<BalloonPack> BalloonPackList;
        public List<FlashSale> FlashSaleList;
        public List<PigBank> PigBankList;
        public List<DailyPack> DailyPackList;
        public List<DailyPackInfo> DailyPackInfoList;
        public List<DailyPackPrice> DailyPackPriceList;
        public List<DailyPackPriceRules> DailyPackPriceRulesList;
        public List<RvShopPriceRules> RvShopPriceRulesList;
        public List<RvShopPrice> RvShopPriceList;
        public List<RVshopList> RVshopListList;
        public List<RVshopResource> RVshopResourceList;
        public List<DailyBonus> DailyBonusList;
        public List<DailyBonusChest> DailyBonusChestList;
        public List<TMIceBreakPack> TMIceBreakPackList;
        public List<TMIceBreakPackChain> TMIceBreakPackChainList;
        public List<TMReviveGiftPack> TMReviveGiftPackList;
        public List<TMReviveGiftPackLevel> TMReviveGiftPackLevelList;
        public List<TmRemoveAd> TmRemoveAdList;
        public List<MasterCardList> MasterCardListList;
        public List<MasterCardResource> MasterCardResourceList;
        public List<BuyResource> BuyResourceList;
        public List<FlashSaleBox> FlashSaleBoxList;
        public List<TaskAssist> TaskAssistList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(ConsumeExtend)] = "ConsumeExtend",
            [typeof(DailyShop)] = "DailyShop",
            [typeof(MysteryGift)] = "MysteryGift",
            [typeof(MysteryPack)] = "MysteryPack",
            [typeof(PigSale)] = "PigSale",
            [typeof(IceBreakPack)] = "IceBreakPack",
            [typeof(BalloonPack)] = "BalloonPack",
            [typeof(FlashSale)] = "FlashSale",
            [typeof(PigBank)] = "PigBank",
            [typeof(DailyPack)] = "DailyPack",
            [typeof(DailyPackInfo)] = "DailyPackInfo",
            [typeof(DailyPackPrice)] = "DailyPackPrice",
            [typeof(DailyPackPriceRules)] = "DailyPackPriceRules",
            [typeof(RvShopPriceRules)] = "RvShopPriceRules",
            [typeof(RvShopPrice)] = "RvShopPrice",
            [typeof(RVshopList)] = "RVshopList",
            [typeof(RVshopResource)] = "RVshopResource",
            [typeof(DailyBonus)] = "DailyBonus",
            [typeof(DailyBonusChest)] = "DailyBonusChest",
            [typeof(TMIceBreakPack)] = "TMIceBreakPack",
            [typeof(TMIceBreakPackChain)] = "TMIceBreakPackChain",
            [typeof(TMReviveGiftPack)] = "TMReviveGiftPack",
            [typeof(TMReviveGiftPackLevel)] = "TMReviveGiftPackLevel",
            [typeof(TmRemoveAd)] = "TmRemoveAd",
            [typeof(MasterCardList)] = "MasterCardList",
            [typeof(MasterCardResource)] = "MasterCardResource",
            [typeof(BuyResource)] = "BuyResource",
            [typeof(FlashSaleBox)] = "FlashSaleBox",
            [typeof(TaskAssist)] = "TaskAssist",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("consumeextend")) return false;
            if (!table.ContainsKey("dailyshop")) return false;
            if (!table.ContainsKey("mysterygift")) return false;
            if (!table.ContainsKey("mysterypack")) return false;
            if (!table.ContainsKey("pigsale")) return false;
            if (!table.ContainsKey("icebreakpack")) return false;
            if (!table.ContainsKey("balloonpack")) return false;
            if (!table.ContainsKey("flashsale")) return false;
            if (!table.ContainsKey("pigbank")) return false;
            if (!table.ContainsKey("dailypack")) return false;
            if (!table.ContainsKey("dailypackinfo")) return false;
            if (!table.ContainsKey("dailypackprice")) return false;
            if (!table.ContainsKey("dailypackpricerules")) return false;
            if (!table.ContainsKey("rvshoppricerules")) return false;
            if (!table.ContainsKey("rvshopprice")) return false;
            if (!table.ContainsKey("rvshoplist")) return false;
            if (!table.ContainsKey("rvshopresource")) return false;
            if (!table.ContainsKey("dailybonus")) return false;
            if (!table.ContainsKey("dailybonuschest")) return false;
            if (!table.ContainsKey("tmicebreakpack")) return false;
            if (!table.ContainsKey("tmicebreakpackchain")) return false;
            if (!table.ContainsKey("tmrevivegiftpack")) return false;
            if (!table.ContainsKey("tmrevivegiftpacklevel")) return false;
            if (!table.ContainsKey("tmremovead")) return false;
            if (!table.ContainsKey("mastercardlist")) return false;
            if (!table.ContainsKey("mastercardresource")) return false;
            if (!table.ContainsKey("buyresource")) return false;
            if (!table.ContainsKey("flashsalebox")) return false;
            if (!table.ContainsKey("taskassist")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "ConsumeExtend": cfg = ConsumeExtendList as List<T>; break;
                case "DailyShop": cfg = DailyShopList as List<T>; break;
                case "MysteryGift": cfg = MysteryGiftList as List<T>; break;
                case "MysteryPack": cfg = MysteryPackList as List<T>; break;
                case "PigSale": cfg = PigSaleList as List<T>; break;
                case "IceBreakPack": cfg = IceBreakPackList as List<T>; break;
                case "BalloonPack": cfg = BalloonPackList as List<T>; break;
                case "FlashSale": cfg = FlashSaleList as List<T>; break;
                case "PigBank": cfg = PigBankList as List<T>; break;
                case "DailyPack": cfg = DailyPackList as List<T>; break;
                case "DailyPackInfo": cfg = DailyPackInfoList as List<T>; break;
                case "DailyPackPrice": cfg = DailyPackPriceList as List<T>; break;
                case "DailyPackPriceRules": cfg = DailyPackPriceRulesList as List<T>; break;
                case "RvShopPriceRules": cfg = RvShopPriceRulesList as List<T>; break;
                case "RvShopPrice": cfg = RvShopPriceList as List<T>; break;
                case "RVshopList": cfg = RVshopListList as List<T>; break;
                case "RVshopResource": cfg = RVshopResourceList as List<T>; break;
                case "DailyBonus": cfg = DailyBonusList as List<T>; break;
                case "DailyBonusChest": cfg = DailyBonusChestList as List<T>; break;
                case "TMIceBreakPack": cfg = TMIceBreakPackList as List<T>; break;
                case "TMIceBreakPackChain": cfg = TMIceBreakPackChainList as List<T>; break;
                case "TMReviveGiftPack": cfg = TMReviveGiftPackList as List<T>; break;
                case "TMReviveGiftPackLevel": cfg = TMReviveGiftPackLevelList as List<T>; break;
                case "TmRemoveAd": cfg = TmRemoveAdList as List<T>; break;
                case "MasterCardList": cfg = MasterCardListList as List<T>; break;
                case "MasterCardResource": cfg = MasterCardResourceList as List<T>; break;
                case "BuyResource": cfg = BuyResourceList as List<T>; break;
                case "FlashSaleBox": cfg = FlashSaleBoxList as List<T>; break;
                case "TaskAssist": cfg = TaskAssistList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            return cfg;
        }
        public void InitConfig(String configJson = null)
        {
            ConfigFromRemote = true;
            Hashtable table = null;
            if (!string.IsNullOrEmpty(configJson))
                table = JsonConvert.DeserializeObject<Hashtable>(configJson);

            if (table == null || !CheckTable(table))
            {
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/AdExtend/adextend");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/AdExtend/adextend error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            ConsumeExtendList = JsonConvert.DeserializeObject<List<ConsumeExtend>>(JsonConvert.SerializeObject(table["consumeextend"]));
            DailyShopList = JsonConvert.DeserializeObject<List<DailyShop>>(JsonConvert.SerializeObject(table["dailyshop"]));
            MysteryGiftList = JsonConvert.DeserializeObject<List<MysteryGift>>(JsonConvert.SerializeObject(table["mysterygift"]));
            MysteryPackList = JsonConvert.DeserializeObject<List<MysteryPack>>(JsonConvert.SerializeObject(table["mysterypack"]));
            PigSaleList = JsonConvert.DeserializeObject<List<PigSale>>(JsonConvert.SerializeObject(table["pigsale"]));
            IceBreakPackList = JsonConvert.DeserializeObject<List<IceBreakPack>>(JsonConvert.SerializeObject(table["icebreakpack"]));
            BalloonPackList = JsonConvert.DeserializeObject<List<BalloonPack>>(JsonConvert.SerializeObject(table["balloonpack"]));
            FlashSaleList = JsonConvert.DeserializeObject<List<FlashSale>>(JsonConvert.SerializeObject(table["flashsale"]));
            PigBankList = JsonConvert.DeserializeObject<List<PigBank>>(JsonConvert.SerializeObject(table["pigbank"]));
            DailyPackList = JsonConvert.DeserializeObject<List<DailyPack>>(JsonConvert.SerializeObject(table["dailypack"]));
            DailyPackInfoList = JsonConvert.DeserializeObject<List<DailyPackInfo>>(JsonConvert.SerializeObject(table["dailypackinfo"]));
            DailyPackPriceList = JsonConvert.DeserializeObject<List<DailyPackPrice>>(JsonConvert.SerializeObject(table["dailypackprice"]));
            DailyPackPriceRulesList = JsonConvert.DeserializeObject<List<DailyPackPriceRules>>(JsonConvert.SerializeObject(table["dailypackpricerules"]));
            RvShopPriceRulesList = JsonConvert.DeserializeObject<List<RvShopPriceRules>>(JsonConvert.SerializeObject(table["rvshoppricerules"]));
            RvShopPriceList = JsonConvert.DeserializeObject<List<RvShopPrice>>(JsonConvert.SerializeObject(table["rvshopprice"]));
            RVshopListList = JsonConvert.DeserializeObject<List<RVshopList>>(JsonConvert.SerializeObject(table["rvshoplist"]));
            RVshopResourceList = JsonConvert.DeserializeObject<List<RVshopResource>>(JsonConvert.SerializeObject(table["rvshopresource"]));
            DailyBonusList = JsonConvert.DeserializeObject<List<DailyBonus>>(JsonConvert.SerializeObject(table["dailybonus"]));
            DailyBonusChestList = JsonConvert.DeserializeObject<List<DailyBonusChest>>(JsonConvert.SerializeObject(table["dailybonuschest"]));
            TMIceBreakPackList = JsonConvert.DeserializeObject<List<TMIceBreakPack>>(JsonConvert.SerializeObject(table["tmicebreakpack"]));
            TMIceBreakPackChainList = JsonConvert.DeserializeObject<List<TMIceBreakPackChain>>(JsonConvert.SerializeObject(table["tmicebreakpackchain"]));
            TMReviveGiftPackList = JsonConvert.DeserializeObject<List<TMReviveGiftPack>>(JsonConvert.SerializeObject(table["tmrevivegiftpack"]));
            TMReviveGiftPackLevelList = JsonConvert.DeserializeObject<List<TMReviveGiftPackLevel>>(JsonConvert.SerializeObject(table["tmrevivegiftpacklevel"]));
            TmRemoveAdList = JsonConvert.DeserializeObject<List<TmRemoveAd>>(JsonConvert.SerializeObject(table["tmremovead"]));
            MasterCardListList = JsonConvert.DeserializeObject<List<MasterCardList>>(JsonConvert.SerializeObject(table["mastercardlist"]));
            MasterCardResourceList = JsonConvert.DeserializeObject<List<MasterCardResource>>(JsonConvert.SerializeObject(table["mastercardresource"]));
            BuyResourceList = JsonConvert.DeserializeObject<List<BuyResource>>(JsonConvert.SerializeObject(table["buyresource"]));
            FlashSaleBoxList = JsonConvert.DeserializeObject<List<FlashSaleBox>>(JsonConvert.SerializeObject(table["flashsalebox"]));
            TaskAssistList = JsonConvert.DeserializeObject<List<TaskAssist>>(JsonConvert.SerializeObject(table["taskassist"]));
            
        }
    }
}