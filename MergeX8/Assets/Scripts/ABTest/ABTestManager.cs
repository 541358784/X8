using System.Collections;
using DragonPlus.Config.AdLocal;
using DragonU3DSDK.Storage;
using Facebook.MiniJSON;
using Merge.Order;
using Newtonsoft.Json;
using UnityEngine;

namespace ABTest
{
    public class ABTestManager : Singleton<ABTestManager>
    {
        public bool IsLockMap()
        {
            return false;
        }

        public bool IsOpenOldUserMergeMainTest()
        {
            return false;
            string key = "ABTEST_OLDUSER_MERGEMAIN";
            string localValue = GetLocalValue(key);
            if (localValue != "")
                return true;

            string isOldUser = GetLocalValue("MergeMainUIAB");
            if (isOldUser != "")
                return false;
                    
            var config = DragonU3DSDK.Network.ABTest.ABTestManager.Instance.GetConfig(key);
            if (config == null)
                return false;

            var table = JsonConvert.DeserializeObject<Hashtable>(config);
            if (table == null)
                return false;
            
            if (table == null || !table.ContainsKey("type"))
                return false;
                
            int type = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(table["type"]));

            return type == 0 ? false : true;
        }
        
        
        public bool IsOpenNewUserMergeMainTest()
        {
            return true;
            string key = "ABTEST_NEWUSER_MERGEMAIN";
            string localValue = GetLocalValue(key);
            if (localValue != "")
                return true;

            string isOldUser = GetLocalValue("MergeMainUIAB");
            if (isOldUser == "")
                return false;
                    
            var config = DragonU3DSDK.Network.ABTest.ABTestManager.Instance.GetConfig(key);
            if (config == null)
                return false;

            var table = JsonConvert.DeserializeObject<Hashtable>(config);
            if (table == null)
                return false;
            
            if (table == null || !table.ContainsKey("type"))
                return false;
                
            int type = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(table["type"]));

            return type == 0 ? false : true;
        }
        
        public bool IsOpenGuideTest()
        {
            string key = "ABTEST_GUIDE";
            string localValue = GetLocalValue(key);
            if (localValue != "")
                return true;

            string isOldUser = GetLocalValue("GuideAB");
            if (isOldUser == "")
                return false;
                    
            return true;
            var config = DragonU3DSDK.Network.ABTest.ABTestManager.Instance.GetConfig(key);
            if (config == null)
                return false;

            var table = JsonConvert.DeserializeObject<Hashtable>(config);
            if (table == null)
                return false;
            
            if (table == null || !table.ContainsKey("type"))
                return false;
                
            int type = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(table["type"]));

            return type == 0 ? false : true;
        }

        public bool IsOpenTMatch()
        {
            return true;
            string key = "ABTEST_TMATCH";
            string localValue = GetLocalValue(key);
            if (localValue != "")
                return true;

            string isOldUser = GetLocalValue("Tmatch");
            if (isOldUser.IsEmptyString())
                return true;
                    
            var config = DragonU3DSDK.Network.ABTest.ABTestManager.Instance.GetConfig(key);
            if (config == null)
                return true;

            var table = JsonConvert.DeserializeObject<Hashtable>(config);
            if (table == null)
                return true;
            
            if (table == null || !table.ContainsKey("type"))
                return true;
                
            int type = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(table["type"]));

            return type == 1 ? false : true;
        }

        public bool IsOpenADTest()
        {
            if (UserGroupManager.Instance.UserGroup != 991)
                return false;
            
            string key = "ABTEST_AD";
            return GetBoolValue(key, 1);
        }
        
        public bool IsOpenSecondRecycleOrder()
        {
            return false;
            string key = "ABTEST_SECOND_RECYCLE_ORDER";
            return GetBoolValue(key, 1);
        }
        
        public bool IsOpenScrewFilthyGame()
        {
            return false;
            string key = "ABTEST_SCREWFILTHYGAME";
            return GetBoolValue(key, 1);
        }

        public bool IsOpenFarmGame()
        {
            return true;
            string key = "ABTEST_FRAM";
            return GetBoolValue(key, 1);
        }
        
        public bool IsOpenFilthyAndMerge()
        {
            return true;
            string key = "ABTEST_MINIGAME_MERGE_FILTHY";
            string localValue = GetLocalValue(key);
            if (localValue != "")
                return true;
            
            var config = DragonU3DSDK.Network.ABTest.ABTestManager.Instance.GetConfig(key);
            if (config.IsEmptyString())
                return false;

            return true;
        }

        public int GetFilthyAndMergeType()
        {
            return 0;
            string key = "ABTEST_MINIGAME_MERGE_FILTHY";
            string localValue = GetLocalValue(key);
            if (localValue != "")
                return int.Parse(localValue);
            
            return GetTypeValue(key, "type");
        }
        
        public bool IsOpenNewIceBreak()
        {
            string key = "ABTest_NewIceBreak";
            return !GetBoolValue(key, 0);
        }

        public bool IsOpenSecondGuide()
        {
            string isNewUser = GetLocalValue("SecondGuideAB");
            if (isNewUser == "")
                return false;

            return true;
            string key = "ABTEST_GUIDE_SECOND";
            return GetBoolValue(key, 1);
        }
        
        public bool IsOpenGuidePlan_C()
        {
            return true;
            string isNewUser = GetLocalValue("GuidePlanC");
            if (isNewUser == "")
                return false;
            
            string key = "ABTEST_GUIDE_PLAN_C";
            return GetBoolValue(key, 1);
        }
        
        public bool IsOpenIosDitchPlan_D()
        {
            string isNewUser = GetLocalValue("DitchPlanD");
            if (isNewUser == "")
                return false;
            
            string key = "ABTEST_IOS_DITCH_PLAN_D";
            return GetBoolValue(key, 1);
        }
        
        public bool IsOpenDogPlay()
        {
            string key = "ABTest_DogPlay";
            return GetBoolValue(key, 1);
        }
        public bool IsOpenNewPayLevel()
        {
            string key = "ABTest_NewPayLevel";
            return GetBoolValue(key, 1);
        }
            
        public bool IsOpenBeginnerOrder()
        {
            return false;
            
            string key = "ABTEST_RANDOM_ORDER_PLAN_B";
            return GetBoolValue(key, 1);
        }
        
        public bool GetBoolValue(string key, int value)
        {
            string localValue = GetLocalValue(key);
            if (localValue != "")
                return true;
                    
            var config = DragonU3DSDK.Network.ABTest.ABTestManager.Instance.GetConfig(key);
            if (config == null)
                return false;

            var table = JsonConvert.DeserializeObject<Hashtable>(config);
            if (table == null)
                return false;
            
            if (table == null || !table.ContainsKey("type"))
                return false;
                
            int type = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(table["type"]));

            return type == value ? true : false;
        }

        public int GetTypeValue(string key, string value)
        {
            var config = DragonU3DSDK.Network.ABTest.ABTestManager.Instance.GetConfig(key);
            if (config == null)
                return 0;

            var table = JsonConvert.DeserializeObject<Hashtable>(config);
            if (table == null)
                return 0;
            
            if (table == null || !table.ContainsKey(value))
                return 0;
                
            int type = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(table[value]));

            return type;
        }
        
        public CreateOrderType GetCreateOrderType()
        {
            return CreateOrderType.Difficulty;
        }

        private bool _isOpenOrderABTest = false;
        public bool IsOpenOrderABTest()
        {
            return true;
        }
        public bool IsOpenKeepPet()
        {
            return true;
        }
        
        
        public readonly string AdLocalConfigPayLevelTestKey = "AD_LOCAL_CONFIG_PAY";
        
        /// <summary>
        /// 是否使用本地分组payLevel关联测试
        /// </summary>
        /// <returns></returns>
        public bool IsAdLocalConfigPayLevelTest()
        {
            if (AdLocalConfigHandle.Instance.IsDebugPayLevel)
                return true;
            
            
            string isNewUser = GetLocalValue("AB_TEST_AD_LOCAL_CONFIG_PAY_NEW_KEY");
            if (isNewUser == "")
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests["AB_TEST_AD_LOCAL_CONFIG_PAY_NEW_KEY"] = "Old";
                return IsAdLocalConfigPayLevelTestOld();
            }
            else
            {
                StorageManager.Instance.GetStorage<StorageCommon>().Abtests["AB_TEST_AD_LOCAL_CONFIG_PAY_NEW_KEY"] = "New";
                return IsAdLocalConfigPayLevelTestNew();   
            }
        }

        /// <summary>
        /// 是否使用本地分组payLevel关联测试新用户
        /// </summary>
        /// <returns></returns>
        public bool IsAdLocalConfigPayLevelTestNew()
        {
            if (AdLocalConfigManager.Instance.UserTypeConfigList == null)
                return false;
            
            UserTypeConfig curGroupConfig =
                AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == AdLocalConfigHandle.Instance.Storage.CurGroup);
            if (curGroupConfig != null && curGroupConfig.GroupId == 2)
            {
                return true;
            }
            string key = "AB_TEST_AD_LOCAL_CONFIG_PAY_NEW";  
            return GetBoolValue(key, 1);
        }
        
        /// <summary>
        /// asmr小游戏ab
        /// </summary>
        /// <returns></returns>
        public bool IsMiniGameOpened()
        {
            return true;
            // string key = "AB_TEST_MINIGAME";  
            // return GetBoolValue(key, 1);
        }
        /// <summary>
        /// 是否使用本地分组payLevel关联测试老用户
        /// </summary>
        /// <returns></returns>
        public bool IsAdLocalConfigPayLevelTestOld()
        {
            string key = "AB_TEST_AD_LOCAL_CONFIG_PAY_OLD2";  
            return GetBoolValue(key, 1);
        }
        
        
        private string GetLocalValue(string key)
        {
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(key))
                return "";

            return StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[key];
        }

        private void SaveLocalValue(string key, string value)
        {
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[key] = value;
        }
    }
}