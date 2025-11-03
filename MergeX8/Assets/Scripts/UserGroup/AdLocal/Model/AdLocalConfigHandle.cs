using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABTest;
using DragonPlus;
using DragonPlus.Config.AdLocal;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Util;
using UnityEngine;


/// <summary>
/// 本地分组判断数据类型
/// </summary>
public enum AdLocalDataType
{
    CompletedOrder,
    CloseShop,
    SkipRv, //连续跳过RV
    CostEnergy,//消耗体力
}


/// <summary>
/// 操作场景
/// </summary>
public enum AdLocalOperateScene
{
    Shop,
    Skip,
}


/// <summary>
/// 跳过操作场景
/// </summary>
public enum AdLocalSkipScene
{
    None = 0,
    ShopSource = 1,
    LuckBubble = 2,
    BuyEnergy = 3,
    LuckBalloon = 4,
    RvShop = 5,
    /// <summary>
    /// 神秘礼物
    /// </summary>
    MysticalGift = 6,
}

/// <summary>
/// 操作类型
/// </summary>
public enum AdLocalOperate
{
    Open,
    Operate,
    Close,
}


public class AdLocalOperateData
{
    public bool IsOpen { get; set; }
    public bool IsOperate { get; set; }

    public void Reset()
    {
        IsOpen = false;
        IsOperate = false;
    }
}

public class AdLocalConfigHandle : Manager<AdLocalConfigHandle>
{
    /// <summary>
    /// debug模式，直接使用新
    /// </summary>
    public bool IsDebug { get; set; }
    
    
    /// <summary>
    /// debug模式，直接使用新
    /// </summary>
    public bool IsDebugPayLevel { get; set; }
    
    /// <summary>
    /// debug模式，直接使用新
    /// </summary>
    public bool IsDebugBubble { get; set; }
    
    private bool _useLocalConfig;


    private int _defaultUserGroup = 100;

    /// <summary>
    /// 记录支付数据的天数
    /// </summary>
    private const int RecordPayDay = 30;
    
    /// <summary>
    /// 本地分组存档数据
    /// </summary>
    public StorageAdConfigLocal Storage => StorageManager.Instance.GetStorage<StorageHome>().AdConfigLocal;

    private Dictionary<AdLocalOperateScene, AdLocalOperateData> _dicOperateData =
        new Dictionary<AdLocalOperateScene, AdLocalOperateData>();

    private bool IsInitConfig = false;

    private string _localConfigCacheKey = "AdLocalConfigHandleConfigKey" + AssetConfigController.Instance.RootVersion;
    
    private void Start()
    {
        InvokeRepeating(nameof(TryRefreshData), 0, 1);
        
        //监听分层消息
        EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent>();
        Action<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent> callback = null;
        EventFunctor<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent> handler = null;
        callback = (a) =>
        {
            if (IsInitConfig)
                InitGroup();
        };
        handler = new EventFunctor<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent>(callback);
        EventManager.Instance.Subscribe(handler);
    }


    public void InitConfig()
    {
        IsInitConfig = false;
        if (!PlayerPrefs.HasKey(_localConfigCacheKey))
        {
            Debug.Log("AdLocalConfigHandle  没有缓存 走本地配置");
            AdLocalConfigManager.Instance.InitConfig();
        }
        else
        {
            Debug.Log("AdLocalConfigHandle  走服务器缓存配置");

            var encryptData = System.Convert.FromBase64String(PlayerPrefs.GetString(_localConfigCacheKey));
            var cacheString = RijndaelManager.Instance.DecryptStringFromBytes(encryptData);
            AdLocalConfigManager.Instance.InitConfig(cacheString);
        }
        IsInitConfig = true;
        InitServerConfig();
    }
    
    public void InitServerConfig()
    {
        CGetConfig cGetConfig = new CGetConfig
        {
            Route = "UserGroup_AdLocal_" + AssetConfigController.Instance.RootVersion
        };

        APIManager.Instance.Send(cGetConfig, (SGetConfig sGetConfig) =>
        {
            if (string.IsNullOrEmpty(sGetConfig.Config.Json))
            {
                Debug.Log("AdLocalConfigHandle 服务器配置为空！ 走本地配置");
                return;
            }

            AdLocalConfigManager.Instance.InitConfig(sGetConfig.Config.Json);
            if (AdLocalConfigManager.Instance.ConfigFromRemote)
            {
                var encryptData = RijndaelManager.Instance.EncryptStringToBytes(sGetConfig.Config.Json);
                PlayerPrefs.SetString(_localConfigCacheKey, System.Convert.ToBase64String(encryptData));
            }
        }, (errno, msg, resp) => { });
    }

    class FbInstallReferrerStruct
    {
        public string fb_install_referrer;
    }
    class FbInstallReferrerInnerStruct
    {
        public string campaign_group_name;
    }
    static string GetCampaignGroupNameFB(string jsonStr)
    {
        try
        {
            var struct1 = JsonUtility.FromJson<FbInstallReferrerStruct>(jsonStr);
            var referrer = struct1.fb_install_referrer;
            referrer = referrer.Replace("\\\"", "\"");
            var struct2 = JsonUtility.FromJson<FbInstallReferrerInnerStruct>(referrer);
            return struct2.campaign_group_name;
        }
        catch (Exception e)
        {
            Debug.LogError("fb找Campaign报错" + e.Message);
            return "";
        }
    }
    
    public int GetServerUserGroup()
    {
        if (StorageManager.Instance.GetStorage<StorageCommon>() == null ||
            StorageManager.Instance.GetStorage<StorageCommon>().Marketing == null)
            return UserGroupManager.Instance.UserGroup;
        var storageMarketing = StorageManager.Instance.GetStorage<StorageCommon>().Marketing;
        Debug.Log("storageMarketing");
        if (storageMarketing.Network == null)
            storageMarketing.Network = "";
        if (storageMarketing.Campaign == null)
            storageMarketing.Campaign = "";
        var netWork = storageMarketing.Network.ToLower();
        var campaign = storageMarketing.Campaign.ToLower();
        var campaignFaceBook = "";
        if (!storageMarketing.FbInstallReferrer.IsEmptyString())
        {
            campaignFaceBook = GetCampaignGroupNameFB(storageMarketing.FbInstallReferrer);
            Debug.LogError("解析得campaignFaceBook=" + campaignFaceBook);
        }
        Debug.Log("netWork = "+netWork);
        Debug.Log("campaign = "+campaign);
        Debug.Log("campaignFaceBook = "+campaignFaceBook);
        var configs = AdLocalConfigManager.Instance.CampaignIntoList;
        foreach (var config in configs)
        {
            
            var netWorkKeys = (config.NetworkKey == null ? "" : config.NetworkKey).ToLower().Split(',');
            var campaignKeys = (config.CampaignKey == null ? "" : config.CampaignKey).ToLower().Split(',');
            Debug.Log("netWorkKeys = "+netWorkKeys.ArrayToString());
            Debug.Log("campaignKeys = "+campaignKeys.ArrayToString());
            var containNetWorkKey = false;
            var containCampaignKey = false;
            foreach (var netWorkKey in netWorkKeys)
            {
                if (netWorkKey == "-1" || netWork.Contains(netWorkKey))
                {
                    containNetWorkKey = true;
                    break;
                }
            }
            Debug.Log("containNetWorkKey = "+containNetWorkKey);
            if (!containNetWorkKey)
                continue;
            foreach (var campaignKey in campaignKeys)
            {
                if (campaignKey == "-1" || campaign.Contains(campaignKey))
                {
                    containCampaignKey = true;
                    break;
                }
            }

            if (!containCampaignKey && !campaignFaceBook.IsEmptyString())
            {
                foreach (var campaignKey in campaignKeys)
                {
                    if (campaignKey == "-1" || campaignFaceBook.Contains(campaignKey))
                    {
                        containCampaignKey = true;
                        break;
                    }
                }
            }
            Debug.Log("containCampaignKey = "+containCampaignKey);
            if (!containCampaignKey)
                continue;
            Debug.Log("定位到客户端服务器分组为 = "+config.GroupId);
            return config.GroupId;
        }
        return UserGroupManager.Instance.UserGroup;
    }

    public bool IsOpenMiniGame()
    {
        return true;
        // if (PlayerPrefs.HasKey("OpenMiniGame"))
        // {
        //     if (PlayerPrefs.GetString("OpenMiniGame") == "true")
        //         return true;
        //     return false;
        // }
        //
        // int group = GetServerUserGroup();
        //
        // var storageMarketing = StorageManager.Instance.GetStorage<StorageCommon>().Marketing;
        // string campaign = storageMarketing.Campaign;
        // campaign = campaign == null ? "" : campaign.ToLower();
        //
        // string fbCampaign = "";
        // if (!storageMarketing.FbInstallReferrer.IsEmptyString())
        //     fbCampaign = GetCampaignGroupNameFB(storageMarketing.FbInstallReferrer);
        // fbCampaign = fbCampaign == null ? "" : fbCampaign.ToLower();
        //
        // if (campaign.IsEmptyString() && fbCampaign.IsEmptyString())
        //     return false;
        //
        // var config = AdLocalConfigManager.Instance.CampaignIntoList.Find(a=>a.GroupId == group);
        // if (config == null)
        //     return false;
        //
        // if (config.MiniGameKey == null)
        //     return false;
        //
        // foreach (var key in config.MiniGameKey)
        // {
        //     if (campaign.Contains(key) || fbCampaign.Contains(key))
        //         return true;
        // }
        //
        // return false;
    }
    
    /// <summary>
    /// 初始化本地分组
    /// </summary>
    public void InitGroup()
    {
        // if (ABTestManager.Instance.IsAdLocalConfigPayLevelTest())
        // {
        //     Debug.Log("进入本地分组ab测");
        // }
        //老用户相信一次，新用户相信10次
        if ((Storage.IsNewUser && Storage.InitCount >= 10) || (!Storage.IsNewUser && Storage.InitCount >= 3)) //初始化次数+2
        {
            //初始化时新增246特判逻辑
            // if (SpecialTryInitWith246())
            // {
            //     return;
            // }
            
            if (!Storage.IsNewUser)
            {
                {
                    var key = "2025_4_24_SetPayValueToNewUserGroup";
                    if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(key))
                    {
                        StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(key, "true");
                        var userGroup = GetServerUserGroup();
                        if (userGroup == 100)
                        {
                            if (StorageManager.Instance.GetStorage<StorageCommon>().RemoteGroupIdDatas
                                .TryGetValue("config_ad", out var storageUserGroup))
                            {
                                userGroup = (int)storageUserGroup;
                            }
                        }
                        UserTypeInto intoConfig = AdLocalConfigManager.Instance.UserTypeIntoList.Find(p => p.GroupId == userGroup);
                        if (intoConfig.IntoType == 1)//付费用户才做强制转化
                        {
                            Debug.Log("The World");
                            Storage.LastPayData.Clear();
                            var curDay = 20193;
                            for (var i = curDay - 6; i <= curDay; i++)
                            {
                                if (PayLevelModel.Instance.Storage.AllDayPayDic.TryGetValue(i, out var value))
                                {
                                    Storage.LastPayData.Add(value);  
                                    // Debug.Log("老付费分层付费记录导入新付费分层 日期:"+i+" 付费金额"+value);
                                }
                                else
                                {
                                    Storage.LastPayData.Add(-1);
                                    // Debug.Log("老付费分层付费记录导入新付费分层 日期:"+i+" 付费金额0");
                                }
                            }
                            PayTrySwitchGroup();
                            PayTrySwitchGroup();   
                        }
                    }   
                }
                {
                    var key = "2025_5_6_SetOldFreeUserGroup";
                    if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(key))
                    {
                        StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(key, "true");
                        var userGroup = GetServerUserGroup();
                        if (userGroup == 100)
                        {
                            if (StorageManager.Instance.GetStorage<StorageCommon>().RemoteGroupIdDatas
                                .TryGetValue("config_ad", out var storageUserGroup))
                            {
                                userGroup = (int)storageUserGroup;
                            }
                        }
                        UserTypeInto intoConfig = AdLocalConfigManager.Instance.UserTypeIntoList.Find(p => p.GroupId == userGroup);
                        if (intoConfig.IntoType == 0) //免费用户做补丁
                        {
                            Storage.InitCount = 0;
                            InitGroup();
                            Debug.Log("2025.5.7 免费老用户补丁生效，重新初始化分层");
                        }
                    }
                }
            }
            return;
        }

        int commonUserGroup = GetServerUserGroup();
        Debug.Log("服务器分组userId="+commonUserGroup);
        if (commonUserGroup == 100)
        {
            if (StorageManager.Instance.GetStorage<StorageCommon>().RemoteGroupIdDatas
                .TryGetValue("config_ad", out var userGroup))
            {
                commonUserGroup = (int)userGroup;
                Debug.Log("再初始化成功,本地记录userId="+commonUserGroup);
            }
            else
            {
                Debug.Log("初始化错误,本地没有记录userId");
            }
        }
        UserTypeInto into = AdLocalConfigManager.Instance.UserTypeIntoList.Find(p => p.GroupId == commonUserGroup);

        int initGroup = -1;
        String country = AdConfigHandle.Instance.StorageCommon.Country.ToUpper();
        bool isIos = AdConfigHandle.Instance.StorageCommon.Platform == (int)Platform.Ios;

        if (into == null)
        {
            initGroup = StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents>0?902:901;
        }
        else
        {
            int groupIndex = 0;
            int notIosBaseIndex = 4;

            if (into.CountryT0.Contains(country))
                groupIndex = isIos ? 0 : notIosBaseIndex;
            else if (into.CountryT1.Contains(country))
                groupIndex = isIos ? 1 : notIosBaseIndex + 1;
            else if (into.CountryT2.Contains(country))
                groupIndex = isIos ? 2 : notIosBaseIndex + 2;
            else
                groupIndex = isIos ? 3 : notIosBaseIndex + 3;

       
            //免费用户逻辑
            if (into.IntoType == 0)
            {
                groupIndex = Mathf.Min(groupIndex, into.FreeUserInto.Count);
                initGroup = into.FreeUserInto[groupIndex];
            }
            else
            {

                groupIndex = PayLevelModel.Instance.Storage.PayLevel-1;
                groupIndex = Mathf.Min(groupIndex, into.PayUserInto.Count);
                initGroup = into.PayUserInto[groupIndex];
            }
        
            //初始化group容错
            UserTypeConfig initGroupConfig =
                AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == initGroup);
            if (initGroupConfig == null)
                initGroup = into.IntoType == 0 ? 901 : 902;
        }


        
        if (into !=null)
        {
            //新用户
            if (Storage.IsNewUser)
            {
                if (Storage.InitCount > 0)
                {
                    //免费组并且不是第一次初始化
                    if (into.IntoType == 0)
                    {
                        
                        // int commonId = AdConfigHandle.Instance.GetCommonID();
                        
                        //不包含在需要重新初始化的分组里
                        if (!AdLocalConfigManager.Instance.NewUserIntoPollingList.First().GroupIDPollingTarget.Contains(Storage.CurGroup))
                        {
                            Debug.Log("跳过重置");
                            Storage.InitCount++;
                            return;
                        } 
                    }
                    else
                    {
                        Storage.InitCount++;
                        return;
                    } 
                }
            }
        }
        
        Storage.InitCount++;
        Storage.CurGroup = initGroup;
        InitClear();
        PayLevelModel.Instance.LoadPayDataToAdLocal();
        
        Debug.Log(
            $"====>AdLocal Init common.UserGroup {commonUserGroup} common.Country {country} isIos {isIos.ToString()} >>>>>>>>>>>>>>>Storage.CurGroup {Storage.CurGroup} Storage.InitCount:{Storage.InitCount}");
    }

    /// <summary>
    /// 初始化清空数据
    /// </summary>
    private void InitClear()
    {
        //ResetJudgingData();
        Storage.EnterCurGroupTime = (long)APIManager.Instance.GetServerTime();
        Storage.CurLowTimes = 0;
        Storage.LastPayTime = 0;
        Storage.CurDayRvNum = 0;
        Storage.CurDayInNum = 0;
        Storage.CurDayPay = 0;
        Storage.LastPayData.Clear();
        Storage.LastPlayInNum.Clear();
        Storage.LastPlayRvNum.Clear();
    }

    /// <summary>
    /// 刷新判断数据
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="isSkipRv"></param>
    /// <param name="add"></param>
    public void RefreshJudgingData(AdLocalDataType dataType, bool isSkipRv = true, int add = 1)
    {
        switch (dataType)
        {
            case AdLocalDataType.CompletedOrder:
                Storage.JudgingData.CompleteOrderNum += add;
                break;
            case AdLocalDataType.CloseShop:
                Storage.JudgingData.CloseShopNum += add;
                break;
            case AdLocalDataType.SkipRv:
                if (isSkipRv)
                    Storage.JudgingData.SkipRvNum += add;
                else
                    Storage.JudgingData.SkipRvNum = 0;
                break;
            case AdLocalDataType.CostEnergy:
                Storage.JudgingData.EnergyNum += add;
                break;
        }

        TrySwitchGroup();
    }


    /// <summary>
    /// 刷新操作状态
    /// </summary>
    /// <param name="operateScene"></param>
    /// <param name="operate"></param>
    /// <param name="skipScene"></param>
    public void RefreshSceneOperate(AdLocalOperateScene operateScene, AdLocalOperate operate,
        AdLocalSkipScene skipScene = AdLocalSkipScene.None)
    {
        Debug.Log(
            $"RefreshSceneOperate operateScene: {operateScene.ToString()} operate: {operate.ToString()} skipScene: {skipScene.ToString()}");
        if (!_dicOperateData.ContainsKey(operateScene))
        {
            _dicOperateData.Add(operateScene, new AdLocalOperateData());
        }
        
        switch (operate)
        {
            case AdLocalOperate.Open:
                _dicOperateData[operateScene].IsOpen = true;
                break;
            case AdLocalOperate.Operate:
                _dicOperateData[operateScene].IsOperate = true;
                break;
        }

        if (operate == AdLocalOperate.Close)
        {
            if (_dicOperateData[operateScene].IsOpen)
            {
                //打开了没操作
                if (!_dicOperateData[operateScene].IsOperate)
                {
                    switch (operateScene)
                    {
                        case AdLocalOperateScene.Shop:
                            RefreshJudgingData(AdLocalDataType.CloseShop);
                            TryShowShopRv();
                            break;
                        case AdLocalOperateScene.Skip:
                            RefreshJudgingData(AdLocalDataType.SkipRv);
                            TryShowPassiveIn(skipScene);
                            break;
                    }
                }
                else
                {
                    if (operateScene == AdLocalOperateScene.Skip)
                    {
                        RefreshJudgingData(AdLocalDataType.SkipRv, false);
                    }
                }
            }

            //计算完当次打开数据后重置
            _dicOperateData[operateScene].Reset();
        }
    }


    /// <summary>
    /// 刷新广告播放次数
    /// </summary>
    /// <param name="isRv"></param>
    public void RefreshAdPlayNum(bool isRv)
    {
        if (isRv)
        {
            Storage.CurDayRvNum++;
        }
        else
        {
            Storage.CurDayInNum++;
        }
    }


    /// <summary>
    /// 尝试切换分组
    /// </summary>
    private void TrySwitchGroup()
    {
        // if (SpecialTryInitWith246())
        //     return;
        UserTypeConfig curGroupConfig =
            AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == Storage.CurGroup);

        UserTypeConfig switchGroup = null;

        //免费组转出
        //数组1：watchRVNum  符合3的情况，转入数组1
        //数组2：watchRVNum  符合2的情况，转入数组2
        //数组3：watchRVNum  符合1的情况，且watch5sInterNum符合 3的情况，转入数组3
        //数组4：watchRVNum  符合1的情况，且watch5sInterNum符合 1或2的情况，转入数组4
        if (curGroupConfig.PayPowerInterval[0] == -1)
        {
            if (!CanUserExport(curGroupConfig.UserExportCondition))
                return;
            int rvCount = curGroupConfig.WatchRvMaxCycle == -1
                ? curGroupConfig.UserExportCondition[0]+1
                : curGroupConfig.WatchRvMaxCycle;
            int maxRv = GetListMax(Storage.LastPlayRvNum, rvCount, Storage.CurDayRvNum);
            int rvState = 1;
            if (maxRv <= curGroupConfig.WatchRvNum[0])
                rvState = 1;
            else if (maxRv > curGroupConfig.WatchRvNum[0] && maxRv <= curGroupConfig.WatchRvNum[1])
                rvState = 2;
            else
                rvState = 3;


            int inCount = curGroupConfig.Watch5sInterMaxCycle == -1
                ? curGroupConfig.UserExportCondition[0]+1
                : curGroupConfig.Watch5sInterMaxCycle;
            int maxIn = GetListMax(Storage.LastPlayInNum, inCount, Storage.CurDayInNum);

            int inState = 1;
            if (maxIn <= curGroupConfig.Watch5sInterNum[0])
                inState = 1;
            else if (maxIn > curGroupConfig.Watch5sInterNum[0] && maxIn <= curGroupConfig.Watch5sInterNum[1])
                inState = 2;
            else
                inState = 3;

            int trySwitchGroupId = 0;

            if (rvState == 3)
                trySwitchGroupId = curGroupConfig.FreeExportTarger[0];
            else if (rvState == 2)
                trySwitchGroupId = curGroupConfig.FreeExportTarger[1];
            else
            {
                if (inState == 3)
                    trySwitchGroupId = curGroupConfig.FreeExportTarger[2];
                else
                    trySwitchGroupId = curGroupConfig.FreeExportTarger[3];
            }

            Debug.Log(
                $"====>AdLocal TrySwitchGroup Free maxRv{maxRv} rvState{rvState} maxIn{maxIn} inState{inState} trySwitchGroupId{trySwitchGroupId}");

            switchGroup = AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == trySwitchGroupId);
        }
        else
        {
            //是否走新用户逻辑
            bool useNewUserLogic = Storage.IsNewUser ||
                                   (!Storage.IsNewUser &&
                                    (curGroupConfig.OldUserInto == -1 || Storage.LastPayTime != 0));
            Debug.Log(
                $"====>AdLocal TrySwitchGroup Pay 付费用户走新用户逻辑{useNewUserLogic.ToString()}  Storage.IsNewUser {Storage.IsNewUser}  curGroupConfig.OldUserInto {curGroupConfig.OldUserInto}  Storage.LastPayTime {Storage.LastPayTime} ");

            if (useNewUserLogic)
            {
                //已经降档过一次
                if (Storage.CurLowTimes > 0)
                {
                    Debug.Log($"====>AdLocal TrySwitchGroup Pay 付费组已降档过一次 ");

                    if (!CanUserExport(curGroupConfig.PayUserFreeExport))
                        return;
                }
                else
                {
                    if (!CanUserExport(curGroupConfig.UserExportCondition))
                        return;
                }

                //已经降档过一次
                if (Storage.CurLowTimes > 0)
                {
                    switchGroup =  AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == curGroupConfig.PayUserSecondExport);
                    Debug.Log(
                            $"====>AdLocal TrySwitchGroup  付费已经降过档用户降到指定分组: {curGroupConfig.PayUserSecondExport}");
                }
                else
                {
                    //首次需要跳到指定分组的
                    if (curGroupConfig.PayUserWaitPayExport!=-1)
                    {
                        switchGroup =  AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == curGroupConfig.PayUserWaitPayExport);
                        Debug.Log(
                            $"====>AdLocal TrySwitchGroup  付费首次降档用户需要跳到指定分组: {curGroupConfig.PayUserWaitPayExport}"); 
                    }
                    else
                    {
                        if (curGroupConfig.PayPowerInterval[0] == -2 && curGroupConfig.SilenceUserExportInto > 0)
                        {
                            Debug.Log("====>AdLocal TrySwitchGroup 沉默用户 非付费固定转化 "+curGroupConfig.UserTypeId+" -> "+ curGroupConfig.SilenceUserExportInto);
                            switchGroup =  AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == curGroupConfig.SilenceUserExportInto);
                        }
                        else
                        {
                            //转换30天
                            float payMax = GetDayAveragePayment(curGroupConfig.PayPowerCycle[2]);
                            Debug.Log($"====>AdLocal TrySwitchGroup Pay payMax {payMax} ");
                            switchGroup = GetUserTypeConfigByPay(payMax,curGroupConfig.GroupId);

                            //新增高频付费特殊转化
                            if (switchGroup != null && curGroupConfig.FrequentlyPayCondition.Count > 1)
                            {
                                if (GetPayDayCount(curGroupConfig.FrequentlyPayCondition[0]) >= curGroupConfig.FrequentlyPayCondition[1])
                                {
                                    if (switchGroup.UserTypeSequence > curGroupConfig.UserTypeSequence && switchGroup.UpTargetUsertype > 0)//升档
                                    {
                                        Debug.Log("高频付费转化额外升档 原目标层:"+switchGroup.UserTypeId+" 新目标层:"+switchGroup.UpTargetUsertype);
                                        switchGroup = AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == switchGroup.UpTargetUsertype);
                                    }
                                    else if(switchGroup.UserTypeSequence < curGroupConfig.UserTypeSequence && curGroupConfig.HoldNowUsertype > 0)//降档
                                    {
                                        Debug.Log("高频付费转化保持不降档 原目标层:"+switchGroup.UserTypeId+" 新目标层:"+curGroupConfig.HoldNowUsertype);
                                        switchGroup = AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == curGroupConfig.HoldNowUsertype);
                                    }
                                }   
                            }   
                        }
                    }
                    

                }
            }
            else
            {
                if (!CanUserExport(curGroupConfig.OldUserExport))
                    return;
                Debug.Log($"====>AdLocal TrySwitchGroup Pay 老用户即将降到的分组{curGroupConfig.OldUserInto} ");
                switchGroup =
                    AdLocalConfigManager.Instance.UserTypeConfigList.Find(p =>
                        p.UserTypeId == curGroupConfig.OldUserInto);
            }
        }

        Debug.Log($"====>AdLocal TrySwitchGroup End  是否拿到分组 {(switchGroup != null).ToString()}");

        //没拿到对应付费组？
        if (switchGroup == null)
            return;

        Debug.Log(
            $"====>AdLocal TrySwitchGroup End  是否付费组 {(curGroupConfig.PayPowerInterval[0] >= 0).ToString()}  是否已经降过档 {(Storage.CurLowTimes > 0).ToString()} 付费组首次降档是否跳到指定分组 {(curGroupConfig.PayUserWaitPayExport!=-1).ToString()}");

        if (curGroupConfig.PayPowerInterval[0] >= 0 && Storage.CurLowTimes <= 0 && curGroupConfig.PayUserWaitPayExport==-1 && curGroupConfig.SilenceUserExportInto <= 0)
        {
            Storage.CurLowTimes++;
        }
        else
        {
            Storage.CurLowTimes = 0;
        }


        ResetJudgingData();
        Storage.CurGroup = switchGroup.UserTypeId;
        Storage.EnterCurGroupTime = (long)APIManager.Instance.GetServerTime();
        Debug.Log($"====>AdLocal PayTrySwitchGroup End  当前分组 {Storage.CurGroup}");
    }

    public int GetPayDayCount(int dayCount)
    {
        var count = 0;
        if (Storage.CurDayPay > 0)
            count++;
        var startIndex = Math.Max(0, Storage.LastPayData.Count - (dayCount - 1));
        for (var i = startIndex; i < Storage.LastPayData.Count; i++)
        {
            if (Storage.LastPayData[i] > 0)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 获取最大值
    /// </summary>
    /// <param name="num"></param>
    /// <param name="lastCount"></param>
    /// <param name="add"></param>
    /// <returns></returns>
    private int GetListMax(List<int> num, int lastCount, int add)
    {
        //需要从支付数据里拿的后几次的数据
        int count = lastCount - 1;
        count = Mathf.Min(num.Count, count);

        //添加当天的支付数据
        List<int> temp = num.Skip(Math.Max(0, num.Count - count)).Take(count).ToList();

        temp.Add(add);

        return temp.Max();
    }


    /// <summary>
    /// 是否能转出
    /// 0、用户距离转入该分层日期＞N个活跃天(当天不算)
    /// 1、用户累计完成N个任务
    /// 2、用户累计关闭N次SHOP界面
    /// 3、用户连续skip N个激励视频场景
    /// 4、用户累计消耗N点体力
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    private bool CanUserExport(List<int> condition)
    {
        for (var i = 0; i < condition.Count; i++)
        {
            if (condition[i] == -1)
                continue;
            switch (i)
            {
                case 0:
                    Debug.Log(
                        $"====>AdLocal CanUserExport 条件1 当前{Storage.JudgingData.EnterGroupActiveDay} 判断{condition[i]}");
                    if (Storage.JudgingData.EnterGroupActiveDay >= condition[i])
                        return true;
                    break;
                case 1:
                    Debug.Log(
                        $"====>AdLocal CanUserExport 条件2 当前{Storage.JudgingData.CompleteOrderNum} 判断{condition[i]}");
                    if (Storage.JudgingData.CompleteOrderNum >= condition[i])
                        return true;
                    break;
                case 2:
                    Debug.Log(
                        $"====>AdLocal CanUserExport 条件3 当前{Storage.JudgingData.CloseShopNum} 判断{condition[i]}");
                    if (Storage.JudgingData.CloseShopNum >= condition[i])
                        return true;
                    break;
                case 3:
                    Debug.Log(
                        $"====>AdLocal CanUserExport 条件4 当前{Storage.JudgingData.SkipRvNum} 判断{condition[i]}");
                    if (Storage.JudgingData.SkipRvNum >= condition[i])
                        return true;
                    break;
                case 4:
                    Debug.Log(
                        $"====>AdLocal CanUserExport 条件5 当前{Storage.JudgingData.EnergyNum} 判断{condition[i]}");
                    if (Storage.JudgingData.EnergyNum >= condition[i])
                        return true;
                    break;
            }
        }

        return false;
    }


    /// <summary>
    /// 重置判断数据
    /// </summary>
    private void ResetJudgingData()
    {
        Storage.JudgingData.Clear();
    }


    /// <summary>
    /// 初始化登录时间
    /// </summary>
    public void TryInitLastLoginTime()
    {
        if (Storage.LastLoginTime == 0)
        {
            Storage.LastLoginTime = (long)APIManager.Instance.GetServerTime();
        }
    }


    /// <summary>
    /// 定数刷新数据
    /// </summary>
    private void TryRefreshData()
    {
        CheckRefreshLastData();
        CheckGamePlayTime();
    }

    #region 支付相关

    /// <summary>
    /// 刷新相关数据（支付数据，活跃天,广告播放次数）
    /// </summary>
    private void CheckRefreshLastData()
    {
        if (!IsInitConfig)
            return;
        
        if (CommonUtils.IsSameDay((ulong)Storage.LastLoginTime,
                APIManager.Instance.GetServerTime()))
            return;

        //没初始化过
        if (Storage.LastLoginTime == 0)
            return;

        if (Storage.LastPayData.Count >= RecordPayDay)
            Storage.LastPayData.RemoveAt(0);

        //记录上次登录相关支付数据
        Storage.LastPayData.Add(Storage.CurDayPay);
        Storage.CurDayPay = 0;

        if (Storage.LastPlayRvNum.Count >= RecordPayDay)
            Storage.LastPlayRvNum.RemoveAt(0);

        //记录上次登录Rv次数
        Storage.LastPlayRvNum.Add(Storage.CurDayRvNum);
        Storage.CurDayRvNum = 0;

        if (Storage.LastPlayInNum.Count >= RecordPayDay)
            Storage.LastPlayInNum.RemoveAt(0);

        //记录上次登录In次数
        Storage.LastPlayInNum.Add(Storage.CurDayInNum);
        Storage.CurDayInNum = 0;

        bool activeDayRefresh = false;

        
        Debug.Log(
            $"====>AdLocal 尝试刷新活跃天  EnterCurGroupTime: {Storage.EnterCurGroupTime} GetServerTime: {APIManager.Instance.GetServerTime()}  LastLoginTime: {Storage.LastLoginTime}  跟当天是否同一天: {CommonUtils.IsSameDay((ulong)Storage.LastLoginTime, APIManager.Instance.GetServerTime())}");
        
        //大于进入分组时间并且跟上次登录时间不是同一天，增加一天活跃天
        if (Storage.EnterCurGroupTime < (long)APIManager.Instance.GetServerTime() && !CommonUtils.IsSameDay(
                (ulong)Storage.LastLoginTime,
                APIManager.Instance.GetServerTime()))
        {
            Storage.JudgingData.EnterGroupActiveDay++;
            activeDayRefresh = true;
        }

        //登录时间间隔
        int dayInterval = Utils.GetDayInterval(Storage.LastLoginTime / 1000,
            (long)APIManager.Instance.GetServerTime() / 1000);
        dayInterval = Mathf.Min(dayInterval, RecordPayDay);

        Storage.LastLoginTime = (long)APIManager.Instance.GetServerTime();

        //间隔大于1天
        if (dayInterval > 1)
        {
            //补足自然天数支付数据
            for (int i = 1; i < dayInterval; i++)
            {
                if (Storage.LastPayData.Count >= RecordPayDay)
                    Storage.LastPayData.RemoveAt(0);

                Storage.LastPayData.Add(-1);
            }
        }

        //活跃天刷新了之后尝试刷新分组
        if (activeDayRefresh)
            TrySwitchGroup();
    }

    public bool SpecialTryInitWith246()
    {
        var key = "2025_5_21_SpecialTryInitWith246";
        if (StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(key))
        {
            return false;
        }

        int commonUserGroup = GetServerUserGroup();
        Debug.Log("服务器分组userId="+commonUserGroup);
        if (commonUserGroup == 100)
        {
            if (StorageManager.Instance.GetStorage<StorageCommon>().RemoteGroupIdDatas
                .TryGetValue("config_ad", out var userGroup))
            {
                commonUserGroup = (int)userGroup;
            }
        }

        if (commonUserGroup == 253)
            commonUserGroup = 246;
        if (commonUserGroup != 246)
        {
            return false;
        }
        var curGroupConfig =
            AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == Storage.CurGroup);
        if (curGroupConfig.GroupId == 2)
            return false;
        
        
        StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(key, "true");
        
        //初始化逻辑
        {
            UserTypeInto into = AdLocalConfigManager.Instance.UserTypeIntoList.Find(p => p.GroupId == commonUserGroup);
            int initGroup = -1;
            String country = AdConfigHandle.Instance.StorageCommon.Country.ToUpper();
            bool isIos = AdConfigHandle.Instance.StorageCommon.Platform == (int)Platform.Ios;
            if (into == null)
            {
                initGroup = StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents > 0 ? 902 : 901;
            }
            else
            {
                int groupIndex = 0;
                int notIosBaseIndex = 4;

                if (into.CountryT0.Contains(country))
                    groupIndex = isIos ? 0 : notIosBaseIndex;
                else if (into.CountryT1.Contains(country))
                    groupIndex = isIos ? 1 : notIosBaseIndex + 1;
                else if (into.CountryT2.Contains(country))
                    groupIndex = isIos ? 2 : notIosBaseIndex + 2;
                else
                    groupIndex = isIos ? 3 : notIosBaseIndex + 3;


                //免费用户逻辑
                if (into.IntoType == 0)
                {
                    groupIndex = Mathf.Min(groupIndex, into.FreeUserInto.Count);
                    initGroup = into.FreeUserInto[groupIndex];
                }
                else
                {

                    groupIndex = PayLevelModel.Instance.Storage.PayLevel - 1;
                    groupIndex = Mathf.Min(groupIndex, into.PayUserInto.Count);
                    initGroup = into.PayUserInto[groupIndex];
                }

                //初始化group容错
                UserTypeConfig initGroupConfig =
                    AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == initGroup);
                if (initGroupConfig == null)
                    initGroup = into.IntoType == 0 ? 901 : 902;
            }

            Storage.CurGroup = initGroup;
        }


        return true;
    }
    
    /// <summary>
    /// 支付之后尝试切换分组
    /// </summary>
    private void PayTrySwitchGroup()
    {
        // if (SpecialTryInitWith246())
        //     return;
        UserTypeConfig curGroupConfig =
            AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == Storage.CurGroup);

        Debug.Log(
            $"====>AdLocal PayTrySwitchGroup Start 当前是否免费分组{(curGroupConfig.PayPowerCycle[0] == -1).ToString()}");


        UserTypeConfig switchGroup = null;

        //免费组首次付费,直接用当前的支付金额做切组判断
        if (curGroupConfig.PayPowerCycle[0] == -1)
        {
            switchGroup = GetUserTypeConfigByPay(Storage.CurDayPay,curGroupConfig.GroupId);
            Debug.Log($"====>AdLocal PayTrySwitchGroup Start  Storage.CurDayPay {Storage.CurDayPay}");
        }
        else
        {
            if (curGroupConfig.SilenceUserUpInto > 0)
            {
                Debug.Log("====>AdLocal PayTrySwitchGroup 沉默用户 付费固定转化 "+curGroupConfig.UserTypeId+" -> "+ curGroupConfig.SilenceUserUpInto);
                switchGroup =  AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == curGroupConfig.SilenceUserUpInto);
            }
            else
            {
                float payMax1 = GetDayAveragePayment(curGroupConfig.PayPowerCycle[0]);
                float payMax2 = GetDayAveragePayment(curGroupConfig.PayPowerCycle[1]);
                float payMax = Mathf.Max(payMax1, payMax2);
                Debug.Log(
                    $"====>AdLocal PayTrySwitchGroup Start  payMax1 {payMax1} payMax2 {payMax2} payMax {payMax}");

                switchGroup = GetUserTypeConfigByPay(payMax,curGroupConfig.GroupId);
                //新增高频付费特殊转化
                if (switchGroup != null && curGroupConfig.FrequentlyPayCondition.Count > 1)
                {
                    if (GetPayDayCount(curGroupConfig.FrequentlyPayCondition[0]) >= curGroupConfig.FrequentlyPayCondition[1])
                    {
                        if (switchGroup.UserTypeSequence > curGroupConfig.UserTypeSequence && switchGroup.UpTargetUsertype > 0)//升档
                        {
                            Debug.Log("高频付费转化额外升档 原目标层:"+switchGroup.UserTypeId+" 新目标层:"+switchGroup.UpTargetUsertype);
                            switchGroup = AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == switchGroup.UpTargetUsertype);
                        }
                        else if(switchGroup.UserTypeSequence < curGroupConfig.UserTypeSequence && curGroupConfig.HoldNowUsertype > 0)//降档
                        {
                            Debug.Log("高频付费转化保持不降档 原目标层:"+switchGroup.UserTypeId+" 新目标层:"+curGroupConfig.HoldNowUsertype);
                            switchGroup = AdLocalConfigManager.Instance.UserTypeConfigList.Find(p => p.UserTypeId == curGroupConfig.HoldNowUsertype);
                        }
                    }   
                }   
            }
        }

        Debug.Log($"====>AdLocal PayTrySwitchGroup End  是否拿到分组 {(switchGroup != null).ToString()}");

        //没拿到对应付费组？
        if (switchGroup == null)
            return;
        ResetJudgingData();
        Storage.CurGroup = switchGroup.UserTypeId;
        Storage.EnterCurGroupTime = (long)APIManager.Instance.GetServerTime();
        Debug.Log($"====>AdLocal PayTrySwitchGroup End  当前分组 {Storage.CurGroup}");
    }

    /// <summary>
    /// 根据付费金额获取付费组
    /// </summary>
    /// <param name="payNum"></param>
    /// <returns></returns>
    private UserTypeConfig GetUserTypeConfigByPay(float payNum,int groupId)
    {
        foreach (UserTypeConfig config in AdLocalConfigManager.Instance.UserTypeConfigList)
        {
            if (config.GroupId != groupId)
                continue;
            if (config.PayPowerInterval.Count <= 1)
                continue;

            int min = config.PayPowerInterval[0];
            int max = config.PayPowerInterval[1];
            max = max == -1 ? int.MaxValue : max;

            if (payNum * 100 >= min && payNum * 100 <= max)
            {
                return config;
            }
        }

        return null;
    }

    // /// <summary>
    // /// 获取自然天内活跃天平均付费金额
    // /// </summary>
    // /// <param name="day"></param>
    // private float GetDayAveragePayment(int day)
    // {
    //     //需要从支付数据里拿的后几次的数据
    //     int takeDay = day - 1;
    //     takeDay = Mathf.Min(Storage.LastPayData.Count, takeDay);
    //
    //     //添加当天的支付数据
    //     List<float> pay = Storage.LastPayData.Skip(Math.Max(0, Storage.LastPayData.Count - takeDay)).Take(takeDay)
    //         .Where(num => num != -1).ToList();
    //
    //     pay.Add(Storage.CurDayPay);
    //
    //     return (float)Math.Round(pay.Average(), 2);
    // }
    
    
    
    /// <summary>
    /// 获取自然天内付费天平均付费金额
    /// </summary>
    /// <param name="day"></param>
    private float GetDayAveragePayment(int day)
    {
        //需要从支付数据里拿的后几次的数据
        int takeDay = day - 1;
        takeDay = Mathf.Min(Storage.LastPayData.Count, takeDay);

        //获取付费大于0的天数数据
        List<float> pay = Storage.LastPayData.Skip(Math.Max(0, Storage.LastPayData.Count - takeDay)).Take(takeDay)
            .Where(num => num > 0 ).ToList();

        //当天充值数据大于0的才加入集合里
        if (Storage.CurDayPay>0)
            pay.Add(Storage.CurDayPay);
        
        
        if (pay.Count <= 0)
            return 0f;
        
        return (float)Math.Round(pay.Average(), 2);
    }

    /// <summary>
    /// 支付成功后累计当天支付金额
    /// </summary>
    /// <param name="shopCfg"></param>
    public void OnPaySuccess(TableShop shopCfg)
    {
        var value = shopCfg.price;
        if (value > 0)
        {
            Storage.LastPayTime = (long)APIManager.Instance.GetServerTime();
            Storage.CurDayPay += value;
            Storage.CurLowTimes = 0;
            PayTrySwitchGroup();
        }
    }

    #endregion

    #region Debug

    /// <summary>
    /// 设置单天付费金额
    /// </summary>
    /// <param name="num"></param>
    public void SetOneDayPayNumDebug(float num)
    {
        if (Storage.LastPayData.Count >= RecordPayDay)
            Storage.LastPayData.RemoveAt(0);

        Storage.LastPayData.Add(num);
    }

    public void AddActiveDayDebug(int add)
    {
        Storage.JudgingData.EnterGroupActiveDay += add;
        TrySwitchGroup();
    }

    public void SetGroupDebug(int group)
    {
        ResetJudgingData();
        Storage.CurGroup = group;
        Storage.EnterCurGroupTime = (long)APIManager.Instance.GetServerTime();
    }

    public void ClearCurDayDataDebug()
    {
        Storage.CurDayRvNum = 0;
        Storage.CurDayInNum = 0;
        Storage.CurDayPay = 0;
    }
    
    public void ClearDataDebug()
    {
        Storage.CurDayRvNum = 0;
        Storage.CurDayInNum = 0;
        Storage.CurDayPay = 0;
        Storage.LastPayData.Clear();
        Storage.LastPlayInNum.Clear();
        Storage.LastPlayRvNum.Clear();
    }

    #endregion

    #region shopRv LuckBubbleRv

    /// <summary>
    /// 尝试显示气泡Rv弹窗
    /// </summary>
    public void TryShowShopRv()
    {
        if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_SHOP_SOURCE))
            return;

        //是否使用主动插屏替换
        bool canUseActiveIn = CanUseActiveIn(ADConstDefine.RV_SHOP_SOURCE, out string placeId);

        UIManager.Instance.OpenWindow(UINameConst.UIPopupShopRv, canUseActiveIn, placeId);
    }

    /// <summary>
    /// 尝试显示气泡Rv弹窗
    /// </summary>
    public void TryShowLuckBubbleRv(int bubbleIndex)
    {
        Common common = AdConfigHandle.Instance.GetCommon();
        if (common == null)
            return;

        //分组不开启主动气泡弹窗
        if (common.LuckyBubblePopup == 0)
            return;

        if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BUBBLE_OPEN))
            return;


        RVAd ad = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, ADConstDefine.RV_BUBBLE_OPEN);

        //是否使用主动插屏替换
        bool canUseActiveIn = CanUseActiveIn(ADConstDefine.RV_BUBBLE_OPEN, out string placeId);

        UIManager.Instance.OpenWindow(UINameConst.UIPopupLuckBubbleRv, bubbleIndex, canUseActiveIn, placeId);
    }


    /// <summary>
    /// 是否能用主动插屏替换
    /// </summary>
    /// <param name="rvPlaceId"></param>
    /// <param name="placeId"></param>
    /// <returns></returns>
    public bool CanUseActiveIn(string rvPlaceId, out string placeId)
    {
        placeId = string.Empty;
        RVAd ad = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, rvPlaceId);

        if (ad == null)
        {
            Debug.Log($"是否能用主动插屏替换  {rvPlaceId} config is null ");
            return false;
        }

        placeId = ad.ActiveInterAdsPlaceId;
        Debug.Log(
            $"是否能用主动插屏替换  {rvPlaceId} {ad.ActiveInterAdsChange == 1 && ad.ActiveInterAdsPlaceId != "-1" && AdSubSystem.Instance.CanPlayInterstitial(placeId)} ");
        return ad.ActiveInterAdsChange == 1 && ad.ActiveInterAdsPlaceId != "-1" &&
               AdSubSystem.Instance.CanPlayInterstitial(placeId);
    }

    /// <summary>
    /// 尝试显示被动插屏
    /// </summary>
    /// <param name="skipScene"></param>
    public void TryShowPassiveIn(AdLocalSkipScene skipScene)
    {
        string placeId = ADConstDefine.IN_SHOP_SOURCE_CLOSE;

        switch (skipScene)
        {
            case AdLocalSkipScene.BuyEnergy:
                placeId = ADConstDefine.IN_GET_ENERGY_CLOSE;
                break;
            case AdLocalSkipScene.LuckBalloon:
                placeId = ADConstDefine.IN_BALLOON_CLOSE;
                break;
            case AdLocalSkipScene.LuckBubble:
                placeId = ADConstDefine.IN_BUBBLE_OPEN_CLOSE;
                break;
            case AdLocalSkipScene.ShopSource:
                placeId = ADConstDefine.IN_SHOP_SOURCE_CLOSE;
                break;
            case AdLocalSkipScene.RvShop:
                placeId = ADConstDefine.IN_TV_REWARD_CLOSE;
                break;
            case AdLocalSkipScene.MysticalGift:
                placeId = ADConstDefine.IN_GET_MYSTICAL_GIFT;
                break;
        }

        if (!AdSubSystem.Instance.CanPlayInterstitial(placeId))
            return;

        AdSubSystem.Instance.PlayInterstital(placeId, (b => { }));
    }
    

    /// <summary>
    /// 检测Rv播放后被动插屏豁免时间是否结束
    /// </summary>
    public void CheckGamePlayTime()
    {
        //没初始化
        if (Storage.InitCount<=0)
            return;

        Common common = AdConfigHandle.Instance.GetCommon(UserGroupManager.Instance.SubUserGroup);
        if (common == null)
            return;

        StorageAdData dataStorage = StorageManager.Instance.GetStorage<StorageHome>().AdData;

        if (dataStorage.PlayRvTodayTime == 0)
            return;


        bool isFailedTimeLimit = Math.Abs(CommonUtils.GetCurTime() - dataStorage.PlayRvTodayTime) <
                                 (common.RvShowPause * 1000);

        if (isFailedTimeLimit)
            return;
        Debug.Log($"被动插屏广告播放间隔限制 豁免时间结束  ");
        
        PausePassInGamePlayTime(false);
        
        dataStorage.PlayRvTodayTime = 0;
    }


    private string _passInGamePlayTimeKey = "_passInGamePlayTimeKey";


    public void StartPassInGamePlayTime(bool isReset = true)
    {
        GamePlayTimeTracker.Instance.StartTracking(_passInGamePlayTimeKey,isReset);
    }
    public void StopPassInGamePlayTime()
    {
        GamePlayTimeTracker.Instance.StopTracking(_passInGamePlayTimeKey);
    }
    
    public void PausePassInGamePlayTime(bool pause)
    {
        GamePlayTimeTracker.Instance.PauseTracking(_passInGamePlayTimeKey,pause);
    }

    public float GetPassInGamePlayTime()
    {
       return GamePlayTimeTracker.Instance.GetTotalPlayTime(_passInGamePlayTimeKey);
    }
    #endregion
}