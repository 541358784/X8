using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.Networking;

namespace DragonU3DSDK.Network.API
{
    public class APIEntry
    {
        public string uri = "/";
        public string method = "POST";
        public string scheme = "https";
        public int timeout = 10;
        public bool gzip = false;
        public bool ignoreAuth = false;
    }

    public class APIManager : Manager<APIManager>
    {
        static float HEART_BEAT_INTERVAL = 60f;

        public NetworkReachability GetNetworkStatus
        {
            get
            {
                return m_NetworkStatus;
            }
        }
        public bool HasNetwork
        {
            get
            {
                return m_hasNetwork;
            }
        }

        private bool m_hasNetwork = false;

        private NetworkReachability m_NetworkStatus;
  
        public bool Inited
        {
            get;
            private set;
        }

        string host = null;
        byte[] secret = null;
        float networkTimer = 0;
        float heartBeatTimer = 0;
        long serverTimeOffset = 0;
        ulong lastSyncServerTime = 0;
        ulong lastSyncLocalTime = 0;
        float websocketAutoReconnectTimer = 0;

        // socket.io
        Dictionary<string, Action<IMessage>> pushCallbacks = new Dictionary<string, Action<IMessage>>();
        Queue<Tuple<Action<IMessage>, IMessage, string>> respCallbakQueue = new Queue<Tuple<Action<IMessage>, IMessage, string>>();
        Queue<Tuple<Action<ErrorCode, string, IMessage>, ErrorCode, string, IMessage, string>> errorCallbackQueue = new Queue<Tuple<Action<ErrorCode, string, IMessage>, ErrorCode, string, IMessage, string>>();
        Queue<Tuple<Action<IMessage>, IMessage, string>> pushCallbackQueue = new Queue<Tuple<Action<IMessage>, IMessage, string>>();

        void init()
        {
            secret = System.Text.Encoding.UTF8.GetBytes(ConfigurationController.Instance.APIServerSecret);
            host = ConfigurationController.Instance.APIServerURL;
            DebugUtil.Log("api server host = {0}", host);
            Inited = true;
        }

        protected override void InitImmediately()
        {
            if (!Inited) init();
        }

        private void Awake()
        {
            m_hasNetwork = Application.internetReachability != NetworkReachability.NotReachable;
            m_NetworkStatus = Application.internetReachability;
        }

        void Update()
        {
            networkTimer += Time.deltaTime;
            if (networkTimer >= 1.0f)
            {
                m_hasNetwork = Application.internetReachability != NetworkReachability.NotReachable;
                m_NetworkStatus = Application.internetReachability;
            }

            heartBeatTimer += Time.deltaTime;
            if (heartBeatTimer > HEART_BEAT_INTERVAL)
            {
                heartBeatTimer = 0.0f;
                HeartBeat();
            }

            
            if (pushCallbackQueue.Count > 0) {
                var tuple = pushCallbackQueue.Dequeue();
                try
                {
                    tuple.Item1.Invoke(tuple.Item2);
                }
                catch (Exception e)
                {
                    BI.BIManager.Instance.SendException(e, 0, tuple.Item3);
                }
            }

            if (respCallbakQueue.Count > 0) {
                var tuple = respCallbakQueue.Dequeue();
                try
                {
                    tuple.Item1.Invoke(tuple.Item2);
                }
                catch (Exception e)
                {
                    BI.BIManager.Instance.SendException(e, 0, tuple.Item3);
                }
            }

            if (errorCallbackQueue.Count > 0) {
                var tuple = errorCallbackQueue.Dequeue();
                try
                {
                    tuple.Item1.Invoke(tuple.Item2, tuple.Item3, tuple.Item4);
                }
                catch (Exception e)
                {
                    BI.BIManager.Instance.SendException(e, 0, tuple.Item5);
                }
            }
        }

        public void HeartBeat()
        {
            var cHeartBeat = new CHeartBeat { };
            Send(cHeartBeat, (SHeartBeat sHeartBeat) =>
            {
                SaveServerTime(sHeartBeat.Timestamp);
            }, (errno, errmsg, resp) =>
            {
                DebugUtil.LogWarning("heart beat errno = {0} errmsg = {1}", errno, errmsg);
            });

            // 心跳到Adjust
            var adjust = Dlugin.SDK.GetInstance().adjustPlugin;
            if (adjust != null)
            {
                adjust.TrackEvent("heart_beat", 0, "{}");
            }
        }

        public void HeartBeatWithCallback(Action<bool> onFinish)
        {
            var cHeartBeat = new CHeartBeat { };
            Send(cHeartBeat, (SHeartBeat sHeartBeat) =>
            {
                SaveServerTime(sHeartBeat.Timestamp);
                onFinish?.Invoke(true);
            }, (errno, errmsg, resp) =>
            {
                onFinish?.Invoke(false);
                DebugUtil.LogWarning("heart beat errno = {0} errmsg = {1}", errno, errmsg);
            });

            // 心跳到Adjust
            var adjust = Dlugin.SDK.GetInstance().adjustPlugin;
            if (adjust != null)
            {
                adjust.TrackEvent("heart_beat", 0, "{}");
            }
        }

        private void SaveServerTime(ulong serverTime)
        {
            lastSyncLocalTime = DeviceHelper.CurrentTimeMillis();
            lastSyncServerTime = serverTime;
            serverTimeOffset = (long)lastSyncServerTime - (long)lastSyncLocalTime;
        }
        
        byte[] gzip(byte[] fi)
        {
            using (MemoryStream outFile = new MemoryStream())
            {
                using (MemoryStream inFile = new MemoryStream(fi))
                using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                {
                    inFile.CopyTo(Compress);
                }
                return outFile.ToArray();
            }
        }

        byte[] gunzip(byte[] fi)
        {
            using (MemoryStream outFile = new MemoryStream())
            {
                using (MemoryStream inFile = new MemoryStream(fi))
                using (GZipStream Compress = new GZipStream(inFile, CompressionMode.Decompress))
                {
                    Compress.CopyTo(outFile);
                }
                return outFile.ToArray();
            }
        }

        IEnumerator sendUnityWebRequest<T1, T2>(T1 req, System.Action<T2> onResponse,
            System.Action<ErrorCode, string, T2> onError) where T1 : IMessage where T2 : IMessage
        {
            var ReqName = req.GetType().Name;
            //var ReqName = typeof(T1).Name;
            bool callbakInvoked = false;
            Action<ErrorCode, string, T2> onErrorWrapper = (ErrorCode errno, string errmsg, T2 resp) =>
            {
                if (!callbakInvoked)
                {
                    callbakInvoked = true;
                    errorCallbackQueue.Enqueue(new Tuple<Action<ErrorCode, string, IMessage>, ErrorCode, string, IMessage, string>((ErrorCode _errno, string _errmsg, IMessage _resp) => onError?.Invoke(_errno, _errmsg, (T2)_resp), errno, errmsg, resp, ReqName));
                }
            };

            Action<T2> onResponseWrapper = (T2 resp) =>
            {
                if (!callbakInvoked)
                {
                    callbakInvoked = true;
                    respCallbakQueue.Enqueue(new Tuple<Action<IMessage>, IMessage, string>((IMessage _resp) => onResponse?.Invoke((T2)_resp), resp, ReqName));
                }
            };

            if (!HasNetwork)
            {
                onErrorWrapper(ErrorCode.NetworkError, "no network", default(T2));
                yield break;
            }

            if (!APIConfig.APIEntries.ContainsKey(ReqName))
            {
                onErrorWrapper(ErrorCode.ApiNotExistsError, "api mapping dosen't contain the " + ReqName, default(T2));
                yield break;
            }
            var apiEntry = APIConfig.APIEntries[ReqName];
            string uri = apiEntry.uri;
            string method = apiEntry.method;
            string scheme = apiEntry.scheme;
            bool useGzip = apiEntry.gzip;

            int timeout = ConfigurationController.Instance.APIServerTimeout;
            if (apiEntry.timeout > timeout)
            {
                timeout = apiEntry.timeout;
            }

            string url = host + uri;
            
            if (!apiEntry.ignoreAuth)
            {
                switch (Account.AccountManager.Instance.loginStatus)
                {
                    case Account.LoginStatus.LOGOUT:
                        onErrorWrapper(ErrorCode.TokenExpireError, "not login", default(T2));
                        yield break;
                    case Account.LoginStatus.LOGIN_LOCKING:
                    case Account.LoginStatus.TOKEN_EXPIRED:
                        var frames = 0;
                        var stopFrames = Application.targetFrameRate * timeout;
                        yield return new WaitUntil(() => (frames++ > stopFrames) || Account.AccountManager.Instance.loginStatus == Account.LoginStatus.LOGIN);
                        if (frames >= stopFrames)
                        {
                            onErrorWrapper(ErrorCode.HttpTimeoutError, "timeout wating for login lock", default(T2));
                            yield break;
                        }
                        break;
                }
            }

            
            var webRequest = new UnityWebRequest(new Uri(url));
            
            byte[] data1 = req.ToByteArray();
            byte[] zipped1 = data1;
            if (useGzip && data1.Length > 1024)
            {
                webRequest.SetRequestHeader("x-gzip", "1");
                zipped1 = gzip(data1);
            }
           
            byte[] encrypted1 = RC4.Encrypt(secret, zipped1);
            webRequest.timeout = timeout;
            
            if (encrypted1.Length > 0)
            {
                webRequest.uploadHandler = new UploadHandlerRaw(encrypted1);
            }
            
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            
            webRequest.SetRequestHeader("x-type", "protobuf");
            webRequest.SetRequestHeader("content-type", "application/octet-stream");
            webRequest.SetRequestHeader("x-method", method);
            webRequest.SetRequestHeader("x-accept-gzip", "1");
            webRequest.SetRequestHeader("x-platform", DeviceHelper.GetPlatform().ToString());
            webRequest.SetRequestHeader("x-client-version-name", DeviceHelper.GetAppVersion());
            webRequest.SetRequestHeader("x-client-version-code", DragonNativeBridge.GetVersionCode().ToString());
            webRequest.SetRequestHeader("User-Agent", DeviceHelper.GetUserAgent());
            webRequest.SetRequestHeader("x-device-id", DeviceHelper.GetDeviceId());
    

            var storageCommon1 = Storage.StorageManager.Instance.GetStorage<Storage.StorageCommon>();
            if (storageCommon1 != null)
            {
                webRequest.SetRequestHeader("x-user-group", storageCommon1.AdsPredictUserGroup.ToString());
            }

            if (!string.IsNullOrEmpty(Account.AccountManager.Instance.Token))
            {
                webRequest.SetRequestHeader("x-token", Account.AccountManager.Instance.Token);
            }
            yield return webRequest.SendWebRequest();
 
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                onErrorWrapper(ErrorCode.UnknownError, webRequest.error, default(T2));
                yield break;
            }
 
            try
            {
                byte[] decrypted;
                
                var responseData = webRequest.downloadHandler.data;
                
                if (responseData != null && responseData.Length > 0)
                {
                    decrypted = RC4.Decrypt(secret, responseData);
                }
                else
                {
                    decrypted = new byte[0];
                }

                byte[] unzipped = decrypted;
                if (decrypted.Length > 0 && !string.IsNullOrEmpty(webRequest.GetResponseHeader("x-gzip")))
                {
                    unzipped = gunzip(decrypted);
                }

                string RespName = 'S' + ReqName.Substring(1);
                Type respType = GetType("DragonU3DSDK.Network.API.Protocol." + RespName);
                PropertyInfo prop = respType.GetProperty("Parser");
                object obj = prop.GetValue(null, null);
                MethodInfo m = prop.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
                T2 resp = (T2)m.Invoke(obj, new object[] { unzipped });
                //T2 resp = new T2();
                //resp.MergeFrom(unzipped);

                try
                {
                    var serverTimeList = webRequest.GetResponseHeader("x-server-time");
                    if (!string.IsNullOrEmpty(serverTimeList))
                    {
                        if (ulong.TryParse(serverTimeList, out var msTime))
                        {
                            SaveServerTime(msTime);
                        }
                        
                        // foreach (var serverTimeStr in serverTimeList)
                        // {
                        //     if (ulong.TryParse(serverTimeStr, out var msTime))
                        //     {
                        //         SaveServerTime(msTime);
                        //     }
                        // }
                    }
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }

                var x_Errno = webRequest.GetResponseHeader("x-errno");
                var errID = 1;
                if (!string.IsNullOrEmpty(x_Errno))
                {
                    int.TryParse(x_Errno, out errID);
                }

                ErrorCode Errno =(ErrorCode) errID;
                var x_errmsg = webRequest.GetResponseHeader("x-errmsg");
                
                 if (Errno > 0)
                 {
                   string Errmsg = string.IsNullOrEmpty(x_errmsg) ? "Unknow" : x_errmsg;
#if DEBUG || DEVELOPMENT_BUILD
                   DebugUtil.LogError("http {0} errno: {1} errmsg: {2}", ReqName, Errno.ToString(), Errmsg);
#endif
                    if (Errno == ErrorCode.TokenExpireError)
                    {
                        Account.AccountManager.Instance.OnTokenExpire();
                    }

                    if (Errno == ErrorCode.RefreshTokenExpireError)
                    {
                        Account.AccountManager.Instance.OnRefreshTokenExpire();
                    }

                    onErrorWrapper(Errno, Errmsg, resp);
                    yield break;
                }

                onResponseWrapper(resp);
            }
            catch (Exception e)
            {
                var ee = e;
                while (ee.InnerException != null) ee = ee.InnerException;   // 仅记录真正产生异常的信息
                onErrorWrapper(ErrorCode.UnknownError, ee.ToString(), default);

                if (ReqName != "CSendEvents") BI.BIManager.Instance.SendException(ee, 0, ReqName);
            }
            
            yield break;

        }
 
        public Type GetType(string typeName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
 
        public void Send<T1, T2>(T1 imessage, Action<T2> onResponse, Action<ErrorCode, string, T2> onError) where T1 : IMessage where T2 : IMessage
        {
            if (!Inited) init();
            string Name1 = typeof(T1).Name;
            string Name2 = typeof(T2).Name;
            if (Name2 == "IMessage")
            {
                DebugUtil.LogWarning("DragonU3DSDK.Network.API.APIManager.Send is deprecated, please use DragonU3DSDK.Network.API.APIManager.Send<T1,T2> instead.");
            }
            else if (!(Name1[0] == 'C' && Name2[0] == 'S' && Name1.Substring(1) == Name2.Substring(1)))
            {
                DebugUtil.LogError("request type {0} response type {1} not match", Name1, Name2);
                onError?.Invoke(ErrorCode.ParameterError, "request and response type not match", default(T2));
                return;
            }

            string name = imessage.GetType().Name;
            if (!APIConfig.APIEntries.ContainsKey(name))
            {
                onError(ErrorCode.ApiNotExistsError, "api mapping dosen't contain the " + name, default(T2));
                return;
            }
           // var apiEntry = APIConfig.APIEntries[name];
        
           
            {
                StartCoroutine(sendUnityWebRequest(imessage, onResponse, onError));
            }
        }

        public ulong GetLastSyncServerTime()
        {
            return lastSyncServerTime;
        }

        public long GetServerTimeOffset()
        {
            return serverTimeOffset;
        }

        public ulong GetLastSyncLocalTime()
        {
            return lastSyncLocalTime;
        }

        public ulong GetServerTime()
        {
            var localTime = DeviceHelper.CurrentTimeMillis();
#if (DEBUG || DEVELOPMENT_BUILD) && IGNORE_SERVER_TIME
            if(SROptions.Current.UseLoacalTime)
            {
                return localTime;
            }
            else
            {
               
                if (localTime < lastSyncLocalTime)
                {
                    DebugUtil.LogError("时间前调");
                    return lastSyncLocalTime + (ulong)serverTimeOffset;
                }
            }
#else        
            if (localTime < lastSyncLocalTime)
            {
                DebugUtil.LogError("时间前调");
                return lastSyncLocalTime + (ulong)serverTimeOffset;
            }
#endif
#if (DEBUG || DEVELOPMENT_BUILD) && IGNORE_SERVER_TIME
            if(SROptions.Current.ValidateLoacalTime){
                if (lastSyncLocalTime != 0 && localTime - lastSyncLocalTime > 7200000)
                {
                    DebugUtil.LogError("时间前调");
                    return lastSyncLocalTime + (ulong)serverTimeOffset;
                }
            }
#else
            if (lastSyncLocalTime != 0 && localTime - lastSyncLocalTime > 7200000)
            {
                DebugUtil.LogError("时间前调");
                return lastSyncLocalTime + (ulong)serverTimeOffset;
            }
#endif

            return localTime + (ulong)serverTimeOffset;
        }
 
        public void OnPush<T>(System.Action<T> callback) where T : IMessage
        {
            string protoName = typeof(T).Name;
            if (pushCallbacks.ContainsKey(protoName))
            {
                DebugUtil.Log("push callback duplicated, replacing...");
            }
            else
            {
                pushCallbacks.Add(protoName, (IMessage msg) =>  
                {
                    callback.Invoke((T)msg);
                });
            }
        }

         }
}
