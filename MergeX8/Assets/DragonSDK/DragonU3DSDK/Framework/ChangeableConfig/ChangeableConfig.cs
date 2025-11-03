
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using Dlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Firebase.Extensions;
using System.Threading;

namespace DragonU3DSDK.Asset
{

    public class ChangeableConfig
    {

        public const string KEY_GROUP = "group";
        public const string KEY_GROUPAll = "groupAll";
        public const string DEFAULT_GROUP = "";
        public const string REMOTE_CONFIG_PREDICT_KEY = "purchase_probability_most_likely";
        public const string REMOTE_CONFIG_PREDICT_KEY_FOR_MARKETING = "marketing_purchase_probability_most_likely";
        public const string REMOTE_CONFIG_TAICHI_CONFIGS = "taichi_configs";

        private string m_groupId = "";
        private bool m_isTarget = false;

        private System.Collections.Generic.Dictionary<string, string> defaults = null;



        public class TaichiAC25Config
        {
            public string app_name;
            public string platform;
            public string country;

            public List<float> thresholds;
        };

        private static TaichiAC25Config DEFAULT_TAICHI_AC_25_CONFIG = new TaichiAC25Config
        {
            platform = "android",
            country = "OTHERS",
            thresholds = new List<float> {
                0.97f,
                0.64f,
                0.46f,
                0.35f,
                0.26f,
            },
        };

        private Dictionary<string, Dictionary<string, TaichiAC25Config>> taichiAC25Configs = null;
        private ReaderWriterLockSlim taichiConfigsLock = new ReaderWriterLockSlim();

        private bool m_hasNewRemoteData = false;

        public bool HasNewRemoteData
        {
            get
            {
                return m_hasNewRemoteData;
            }
            set
            {
                m_hasNewRemoteData = value;
            }
        }


        private static ChangeableConfig instance = null;
        private static readonly object syslock = new object();

        public static ChangeableConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new ChangeableConfig();

                        }
                    }
                }
                return instance;
            }
        }


        private ChangeableConfig()
        {
            this.defaults = new System.Collections.Generic.Dictionary<string, string>();
            this.taichiAC25Configs = new Dictionary<string, Dictionary<string, TaichiAC25Config>>();
        }

        public string AddChangeableConfig(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            this.defaults[key] = value;

            if (!FirebaseState.Instance.Initialized)
            {
                return value;
            }

            if (string.IsNullOrEmpty(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue))
            {
                return value;
            }


            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        }


        /// <summary>
        /// 获取当前配置分组ID，默认分组ID为空字符串
        /// </summary>
        /// <returns></returns>
        public string GetGroups()
        {
            if (!string.IsNullOrEmpty(m_groupId))
            {
                return m_groupId;
            }

            if (!FirebaseState.Instance.Initialized)
            {
                return DEFAULT_GROUP;
            }

            string remotegroup = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_GROUP).StringValue;

            if (!string.IsNullOrEmpty(remotegroup))
            {
                return remotegroup;
            }

            return DEFAULT_GROUP;
        }

        public TaichiAC25Config GetTaichiAC25Config()
        {
#if UNITY_IOS
            string platform = "ios";
#else
            string platform = "android";
#endif
            string country = "OTHERS";
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            try
            {
                if (taichiConfigsLock.TryEnterReadLock(100))
                {
                    if (storageCommon != null && !string.IsNullOrEmpty(storageCommon.Country) && taichiAC25Configs.ContainsKey(storageCommon.Country.ToUpper()))
                    {
                        country = storageCommon.Country.ToUpper();
                    }

                    if (taichiAC25Configs.ContainsKey(country) && taichiAC25Configs[country].ContainsKey(platform))
                    {
                        return taichiAC25Configs[country][platform];
                    }
                }
            }
            finally
            {
                try
                {
                    taichiConfigsLock.ExitReadLock();
                }
                catch (SynchronizationLockException e)
                {
                    DebugUtil.LogError($"taichi config lock exception: {e.Message}");
                }
            }

            return DEFAULT_TAICHI_AC_25_CONFIG;
        }

        public string GetServerAdjustedGroup()
        {
            return m_groupId;
        }

        /// <summary>
        /// Enables the default config.
        /// </summary>
        /// <returns><c>true</c>, if default config was enabled, <c>false</c> otherwise.</returns>
        public bool EnableDefaultConfig()
        {
            //if (!FirebaseState.Instance.Initialized)
            //{
            //    return false;
            //}
            //DebugUtil.Log("Firebase EnableDefaultConfig setting defaults");
            //try
            //{
            //    Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(this.defaults);
            //}
            //catch (Exception e)
            //{
            //    DebugUtil.Log("Firebase EnableDefaultConfig setting defaults {0}", e.ToString());
            //}
            return true;
        }


        public Task FetchDataAsync(bool fetchImmediately = false)
        {
            if (FirebaseState.Instance.Initialized)
            {
                DebugUtil.Log("Firebase RemoteConfig Fetching data...");
                // FetchAsync only fetches new data if the current data is older than the provided
                // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
                // By default the timespan is 12 hours, and for production apps, this is a good
                // number.  For this example though, it's set to a timespan of zero, so that
                // changes in the console will always show up immediately.

                TimeSpan span = TimeSpan.FromHours(12);

                if (Debug.isDebugBuild || fetchImmediately)
                {
                    span = TimeSpan.Zero;
                }

                Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(span);
                return fetchTask.ContinueWith(FetchComplete);
            }

            DebugUtil.Log("Firebase RemoteConfig Fetching data... Firebase Not Inited");
            return null;
        }


        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                DebugUtil.Log("Firebase RemoteConfig Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                DebugUtil.Log("Firebase RemoteConfig Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                DebugUtil.Log("Firebase RemoteConfig Fetch completed successfully!");
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

                    this.HasNewRemoteData = true;
                    onFetchSuccess();

                    DebugUtil.Log(string.Format("Remote data loaded and ready (last fetch time {0}).",
                                           info.FetchTime));
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            DebugUtil.Log("Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            DebugUtil.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                        default:
                            DebugUtil.Log("Fetch failed for other reason : " + info.LastFetchFailureReason);
                            break;
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    DebugUtil.Log("Latest Fetch call still pending.");
                    break;
            }
        }


        public string GetRemoteStringValue(string key)
        {

            if (string.IsNullOrEmpty(key))
            {
                DebugUtil.LogWarning("ChangeableConfig: key must not be null or empty");
                return null;
            }

            if (!FirebaseState.Instance.Initialized)
            {
                if (!this.defaults.ContainsKey(key))
                {
                    return null;
                }
                return this.defaults[key];
            }

            if (!string.IsNullOrEmpty(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue))
            {
                return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
            }

            if (!this.defaults.ContainsKey(key))
            {
                return null;
            }
            return this.defaults[key];

        }

        public bool IsTargetPlayer()
        {
            return m_isTarget ? m_isTarget : PlayerPrefs.HasKey(KEY_GROUP) && !string.IsNullOrEmpty(PlayerPrefs.GetString(KEY_GROUP));
        }

        public void SetGroupId(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                this.m_isTarget = false;
                PlayerPrefs.DeleteKey(KEY_GROUP);
                PlayerPrefs.Save();
                m_groupId = DEFAULT_GROUP;
                return;
            }

            this.m_isTarget = true;

            this.m_groupId = groupId;

            PlayerPrefs.SetString(KEY_GROUP, groupId);
            PlayerPrefs.Save();

            SetDataTrakerGroupId(groupId);

            UpdateGroupsAll(groupId);
        }

        private void UpdateGroupsAll(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
                return;
                
            try
            {
                var groupAll = GetGroupsAll();
                if (groupAll.Contains(groupId))
                    return;
                
                groupAll += string.IsNullOrEmpty(groupAll) ? groupId : $",{groupId}";
                DebugUtil.Log($"ABTest: Add new group:{groupId}, all group:{groupAll}");
                PlayerPrefs.SetString(KEY_GROUPAll, groupAll);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        public string GetGroupsAll()
        {
            return PlayerPrefs.GetString(KEY_GROUPAll, "");
        }
        
        /// <summary>
        /// 设置第三方追踪平台groupId
        /// </summary>
        /// <param name="groupId"></param>
        void SetDataTrakerGroupId(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return;
            }

            var adjust = Dlugin.SDK.GetInstance().adjustPlugin;
            if (adjust != null && !string.IsNullOrEmpty(groupId))
            {
                adjust.SetPartnerParameter("abtests", groupId);
                DebugUtil.Log("Sync ABTest to adjust " + groupId);
            }
        }

        void onFetchSuccess()
        {
            var groups = GetGroups();
            m_groupId = groups;

            // 预测事件firebase打点 for marketing
            if (FirebaseState.Instance.Initialized && Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(REMOTE_CONFIG_PREDICT_KEY_FOR_MARKETING).BooleanValue)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("prediction_payer");
            }

            if (!string.IsNullOrEmpty(groups))
            {
                DelayActionManager.Instance.DebounceInMainThread(Guid.NewGuid().ToString(), 1, () =>
                {
                    DragonU3DSDK.Network.API.APIManager.Instance.Send(new DragonU3DSDK.Network.API.Protocol.CGetTestGroups
                    {
                        GroupId = groups,
                    }, (DragonU3DSDK.Network.API.Protocol.SGetTestGroups sGetTestGroups) =>
                    {
                        foreach (var groupId in sGetTestGroups.GroupIds)
                        {
                            SetGroupId(groupId);
                            break;
                        }

                    }, (errno, errmsg, resp) =>
                    {
                        DebugUtil.LogError(errmsg);
                    });
                });
            }

            // taichi configs
            if (FirebaseState.Instance.Initialized && !string.IsNullOrEmpty(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(REMOTE_CONFIG_TAICHI_CONFIGS).StringValue))
            {
                try
                {
                    if (taichiConfigsLock.TryEnterWriteLock(1000))
                    {
                        string str = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(REMOTE_CONFIG_TAICHI_CONFIGS).StringValue;
                        DebugUtil.Log("taichi config fetched " + str);
                        JsonConvert.PopulateObject(str, taichiAC25Configs);
                    }
                    else
                    {
                        DebugUtil.LogError("taichi config lock failed ");
                    }
                }
                finally
                {
                    try
                    {
                        taichiConfigsLock.ExitWriteLock();
                    }
                    catch (SynchronizationLockException e)
                    {
                        DebugUtil.LogError($"taichi config lock exception: {e.Message}");
                    }
                }
            }
            //SetDataTrakerGroupId(groups);
        }

    }
}




