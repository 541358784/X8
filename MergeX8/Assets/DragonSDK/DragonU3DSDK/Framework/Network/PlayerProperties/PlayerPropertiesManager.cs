using System;
using UnityEngine;
using Google.Protobuf;
using DragonU3DSDK.Asset;
using Dlugin;
using DragonU3DSDK.Storage;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace DragonU3DSDK.Network.PlayerProperties
{
    public class PlayerPropertiesManager : Manager<PlayerPropertiesManager>
    {
        float syncTimer = 0.0f;
        float uploadTimer = 0.0f;
        const float SYNC_INTERVAL = 120.0f;
        const float UPLOAD_INTERVAL = 300.0f;
        const string PLAYER_PROPERTIES_KEY = "PlayerPropertiesPb";
        bool syncLock = false;
        bool isInitialized = false;

        private PlayerPurchaseType m_PlayerType = PlayerPurchaseType.UNKNOWN;
        public PlayerPurchaseType PlayerPurchaseType => m_PlayerType;
        public bool PlayerPurchaseTypeMarketingOverride
        {
            get
            {
                var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                if (storageCommon != null)
                {
                    return storageCommon.AdsUserGroupMarketingOverride;
                }
                return false;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }

        public API.Protocol.Personas PlayerProperties
        {
            get
            {
                return personas;
            }
        }
        API.Protocol.Personas personas = new API.Protocol.Personas();

        public void Init(Action<bool> cb = null)
        {
            Load();
            Sync(cb);
            Upload();
            FetchPlayerPurchaseType();
        }

        void Start()
        {
        }

        void Update()
        {
            syncTimer += Time.deltaTime;
            uploadTimer += Time.deltaTime;
            if (syncTimer >= SYNC_INTERVAL)
            {
                syncTimer = 0;
                Sync();
                FetchPlayerPurchaseType();
                if (this.m_PlayerType == PlayerPurchaseType.PAID) return;
                FetchMarketingOverrides(t =>
                {
                    if (this.m_PlayerType == PlayerPurchaseType.PAID) return;
                    if ((int)t > 0)
                    {
                        this.m_PlayerType = t;
                        var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                        if (storageCommon != null)
                        {
                            storageCommon.AdsPredictUserGroup = (int)t;
                            storageCommon.AdsUserGroupMarketingOverride = true;
                        }
                    }
                });
            }

            if (uploadTimer >= UPLOAD_INTERVAL)
            {
                uploadTimer = 0;
                Upload();
            }
        }

        public void FetchPlayerPurchaseType()
        {

            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

            if (storageCommon != null)
            {
                if (this.m_PlayerType == PlayerPurchaseType.UNKNOWN)
                {
                    this.m_PlayerType = (PlayerPurchaseType)storageCommon.AdsPredictUserGroup;
                }

                if (storageCommon.RevenueUSDCents > 0)
                {
                    this.m_PlayerType = PlayerPurchaseType.PAID;
                    return;
                }

                if (storageCommon.AdsUserGroupMarketingOverride && storageCommon.AdsPredictUserGroup > 0)
                {
                    this.m_PlayerType = (PlayerPurchaseType)storageCommon.AdsPredictUserGroup;
                    return;
                }
            }

            if (!FirebaseState.Instance.Initialized)
            {
                return;
            }

            ChangeableConfig.Instance.FetchDataAsync();

            bool most = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("purchase_probability_most_likely").BooleanValue;
            if (most)
            {
                this.m_PlayerType = PlayerPurchaseType.HIGH_POSSIBILITY_TO_PAY;
                return;
            }
            bool middle = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("purchase_probability_middle").BooleanValue;
            if (middle)
            {
                this.m_PlayerType = PlayerPurchaseType.LOW_POSSIBILITY_TO_PAY;
                return;
            }
            bool least = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("purchase_probability_least_likely").BooleanValue;
            if (least)
            {
                this.m_PlayerType = PlayerPurchaseType.NO_POSSIBILITY_TO_PAY;
                return;
            }

        }

        public void Sync(Action<bool> cb = null)
        {
            if (syncLock)
            {
                cb?.Invoke(false);
                return;
            }

            syncLock = true;


            //尝试获取Firebase远端信息
            ChangeableConfig.Instance.FetchDataAsync();

            DragonU3DSDK.Network.API.APIManager.Instance.Send(new DragonU3DSDK.Network.API.Protocol.CGetPersonas { },
            (DragonU3DSDK.Network.API.Protocol.SGetPersonas resp) =>
            {
                personas = resp.Personas.Clone();
                Save();
                syncLock = false;
                isInitialized = true;
                cb?.Invoke(true);
            },
            (errno, errmsg, resp) =>
            {
                DebugUtil.LogError(errmsg);
                syncLock = false;
                cb?.Invoke(false);
            });
        }

        public void ManualUpload()
        {
            uploadTimer = 0;
            Upload();
        }
        
        void Upload()
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            if (storageCommon == null)
            {
                return;
            }
            
            try
            {
                var update = new DragonU3DSDK.Network.API.Protocol.Personas
                {
                    PlayerId = storageCommon.PlayerId,
                    PlayerType = (uint)(Account.AccountManager.Instance.HasBindFacebook() ? DragonU3DSDK.Network.API.Protocol.PlayerType.Facebook : DragonU3DSDK.Network.API.Protocol.PlayerType.Guest),
    #if UNITY_ANDROID
                    Platform = "android",
    #endif
    #if UNITY_IOS
                    Platform = "ios",
    #endif
                    InstalledAt = storageCommon.InstalledAt,
                    DeviceMemory = DeviceHelper.GetTotalMemory().ToString(),
                    DeviceModel = DeviceHelper.GetDeviceModel(),
                    DeviceType = DeviceHelper.GetDeviceType(),
                    DeviceOsVersion = DeviceHelper.GetOSVersion(),
                    DeviceLanguage = DeviceHelper.GetLanguage(),
                    NetworkType = DeviceHelper.GetNetworkStatus().ToString(),
                    // IapTotal = storageCommon.RevenueUSDCents,
                    // IapCount = storageCommon.RevenueCount,
                    // LastLoginDate = API.APIManager.Instance.GetServerTime(),
                    // LastPayDate = storageCommon.LastRevenueTime,
                };

                if (!string.IsNullOrEmpty(storageCommon.NaviveVersion))
                {
                    update.ClientVersion = storageCommon.NaviveVersion;
                }
                if (!string.IsNullOrEmpty(storageCommon.Country))
                {
                    update.Country = storageCommon.Country;
                }
                if (!string.IsNullOrEmpty(storageCommon.ResVersion))
                {
                    update.ResVersion = storageCommon.ResVersion;
                }
                if (!string.IsNullOrEmpty(storageCommon.DeviceId))
                {
                    update.DeviceId = storageCommon.DeviceId;
                }
                if (!string.IsNullOrEmpty(storageCommon.Email))
                {
                    update.Email = storageCommon.Email;
                }
                if (!string.IsNullOrEmpty(storageCommon.FacebookEmail))
                {
                    update.FacebookEmail = storageCommon.FacebookEmail;
                }
                if (!string.IsNullOrEmpty(storageCommon.FacebookName))
                {
                    update.FacebookName = storageCommon.FacebookName;
                }
                if (!string.IsNullOrEmpty(storageCommon.FacebookId))
                {
                    update.FacebookId = storageCommon.FacebookId;
                }
                // if (storageCommon.Marketing != null)
                // {
                //     if (!string.IsNullOrEmpty(storageCommon.Marketing.Network))
                //     {
                //         update.SourceNetwork = storageCommon.Marketing.Network;
                //     }
                //     if (!string.IsNullOrEmpty(storageCommon.Marketing.Campaign))
                //     {
                //         update.SourceCampaign = storageCommon.Marketing.Campaign;
                //     }
                //     if (!string.IsNullOrEmpty(storageCommon.Marketing.AdGroup))
                //     {
                //         update.SourceAdgroup = storageCommon.Marketing.AdGroup;
                //     }
                //     if (!string.IsNullOrEmpty(storageCommon.Marketing.Creative))
                //     {
                //         update.SourceCreative = storageCommon.Marketing.Creative;
                //     }
                // }

                DragonU3DSDK.Network.API.APIManager.Instance.Send(new DragonU3DSDK.Network.API.Protocol.CUpdatePersonas
                {
                    Personas = update,
                },
                (DragonU3DSDK.Network.API.Protocol.SUpdatePersonas resp) =>
                {

                },
                (errno, errmsg, resp) =>
                {
                    DebugUtil.LogError(errmsg);
                });
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        void FetchMarketingOverrides(Action<PlayerPurchaseType> cb)
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            if (string.IsNullOrEmpty(storageCommon?.Marketing.Campaign))
            {
                cb?.Invoke(PlayerPurchaseType.UNKNOWN);
                return;
            }

            DragonU3DSDK.Network.API.APIManager.Instance.Send(new DragonU3DSDK.Network.API.Protocol.CGetConfig
            {
                Route = "campaigns_group"
            },
            (DragonU3DSDK.Network.API.Protocol.SGetConfig resp) =>
            {
                var data = resp.Config.Json;

                if (string.IsNullOrEmpty(data))
                {
                    cb?.Invoke(PlayerPurchaseType.UNKNOWN);
                    return;
                }

                var jObj = JObject.Parse(data);
                foreach (var pair in jObj)
                {
                    if (fuzzyContains(storageCommon?.Marketing.Campaign, pair.Key))
                    {
                        int val = int.Parse(pair.Value.ToString());
                        if (val > 0)
                        {
                            cb?.Invoke((PlayerPurchaseType)val);
                            return;
                        }
                    }
                }
                cb?.Invoke(PlayerPurchaseType.UNKNOWN);
            },
            (errno, errmsg, resp) =>
            {
                DebugUtil.LogError(errmsg);
                cb?.Invoke(PlayerPurchaseType.UNKNOWN);
            });
        }

        bool fuzzyContains(string a, string b)
        {
            Regex rx = new Regex(@"\s");
            a = rx.Replace(a.ToLower(), "");
            b = rx.Replace(b.ToLower(), "");
            return a.Contains(b);
        }

        void Save()
        {
            byte[] arr = personas.ToByteArray();
            string str = Convert.ToBase64String(arr);
            PlayerPrefs.SetString(PLAYER_PROPERTIES_KEY, str);
        }

        void Load()
        {
            if (PlayerPrefs.HasKey(PLAYER_PROPERTIES_KEY))
            {
                string str = PlayerPrefs.GetString(PLAYER_PROPERTIES_KEY);
                byte[] arr = Convert.FromBase64String(str);
                personas = API.Protocol.Personas.Parser.ParseFrom(arr);
            }
        }
    }
}
