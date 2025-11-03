using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;
using UnityEngine;

public enum CommercialUserType
{
    JustEntered = 101,
    NotPaidWithin96hours = 201,
    FirstPayBetween24To48Hours = 301,
    FirstPayBetween48To72Hours = 302,
    FirstPayBetween72To96Hours = 303,
    FirstPayOver96Hours = 304,
    FirstPayWithin24hours = 401,
}


public enum UserPlatformBasedType
{
    IOS_Base = 100,
    Android_Base = 100,
}

public class UserGroupManager : Manager<UserGroupManager>
{
    private StorageAdData storageAd;
    private bool isAddEvent = false;

    public StorageAdData storageAdData
    {
        get
        {
            if (storageAd == null)
                storageAd = StorageManager.Instance.GetStorage<StorageHome>().AdData;

            return storageAd;
        }
    }

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

    public int SubUserGroup
    {
        get
        {
            if (storageAd == null)
                return (int) CommercialUserType.JustEntered;

            if (storageAd.SubUserGroup == 0)
                return (int) CommercialUserType.JustEntered;

            return storageAd.SubUserGroup;
        }
    }

    public int UserGroup
    {
        get
        {
            int groupId = AdConfigManager.Instance.MetaData == null ? 0 : AdConfigManager.Instance.MetaData.GroupId;
#if UNITY_IOS
           return groupId == 0 ? (int)UserPlatformBasedType.IOS_Base : groupId;
#else
            return groupId == 0 ? (int) UserPlatformBasedType.Android_Base : groupId;
#endif
        }
    }

    public System.TimeSpan TimeAfterInstall
    {
        get
        {
            System.DateTime install;
            if (StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt > 0)
                install = CommonUtils.ConvertFromUnixTimestamp((ulong) StorageManager.Instance
                    .GetStorage<StorageCommon>().InstalledAt);
            else
                install = CommonUtils.ConvertFromUnixTimestamp((ulong) StorageManager.Instance.GetStorage<StorageHome>()
                    .LocalFirstRunTimeStamp);
            System.DateTime cur = System.DateTime.UtcNow;
            return cur - install;
        }
    }

    public void Init()
    {
        InitUserGroup();

        if (isAddEvent)
            return;
        isAddEvent = true;
        EventDispatcher.Instance.AddEventListener(EventEnum.OnIAPItemPaid, OnIAPItemPaid);
    }

    public void InitUserGroup()
    {
        if (storageAdData.UserGroup == 0)
        {
            storageAdData.UserGroup = UserGroup;
        }

        if (storageAdData.SubUserGroup > 0)
            return;

        CommercialUserType initType = AdConfigHandle.Instance.GetServerSubUserType() == 201
            ? CommercialUserType.NotPaidWithin96hours
            : CommercialUserType.JustEntered;

        SetSubUserGroup(initType);
    }

    public void UpdateSubUserGroup()
    {
        CheckUserGroupByTime();
    }

    public void OnConfigHubUpdated()
    {
        if (storageAdData.UserGroup == 0)
        {
            storageAdData.UserGroup = UserGroup;
        }

        if (AdConfigManager.Instance.IsRemote)
        {
            if (storageAdData.UserGroup != UserGroup)
            {
                storageAdData.UserGroup = UserGroup;
            }
        }

        if (AdConfigHandle.Instance.GetServerSubUserType() != (int) CommercialUserType.NotPaidWithin96hours)
            return;

        if (storageAdData.SubUserGroup == (int) CommercialUserType.JustEntered || storageAdData.SubUserGroup == 0)
            storageAdData.SubUserGroup = (int) CommercialUserType.NotPaidWithin96hours;
    }

    private void CheckUserGroupByTime()
    {
        if (this.TimeAfterInstall.TotalHours >= 96 &&
            StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents == 0)
        {
            SetSubUserGroup(CommercialUserType.NotPaidWithin96hours);
        }
    }

    public string GetPlatformUserGroupString()
    {
        return string.Format("{0}_{1}", UserGroup, (int) SubUserGroup);
    }

    void SetSubUserGroup(CommercialUserType ut, bool useServer = false)
    {
        if ((int) ut <= storageAdData.SubUserGroup)
            return;

        if (AdConfigHandle.Instance.GetCommon((int) ut) == null)
            return;

        DebugUtil.Log(
            $"Set New SubUserGroup {ut} for Old type {storageAdData.SubUserGroup} userServerGroup{useServer}");
        storageAdData.SubUserGroup = (int) ut;
    }

    void OnIAPItemPaid(BaseEvent be)
    {
        var st = StorageManager.Instance.GetStorage<StorageCommon>();
        if (st.RevenueCount == 1)
        {
            int serverUserType = AdConfigHandle.Instance.GetServerSubUserType();
            if (serverUserType > 0 && serverUserType != (int) CommercialUserType.JustEntered)
            {
                SetSubUserGroup((CommercialUserType) serverUserType, true);
                return;
            }

            if (TimeAfterInstall.TotalHours <= 24)
            {
                SetSubUserGroup(CommercialUserType.FirstPayWithin24hours);
            }
            else if (TimeAfterInstall.TotalHours <= 48)
            {
                SetSubUserGroup(CommercialUserType.FirstPayBetween24To48Hours);
            }
            else if (TimeAfterInstall.TotalHours <= 72)
            {
                SetSubUserGroup(CommercialUserType.FirstPayBetween48To72Hours);
            }
            else if (TimeAfterInstall.TotalHours <= 96)
            {
                SetSubUserGroup(CommercialUserType.FirstPayBetween72To96Hours);
            }
            else
            {
                SetSubUserGroup(CommercialUserType.FirstPayOver96Hours);
            }
        }
    }
}