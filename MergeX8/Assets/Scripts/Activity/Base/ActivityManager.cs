using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Util;
using Google.Protobuf;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

// 检查活动数据是否改变的数据结构
public class ActivityCheckData
{
    public string ActivityId;
    public ulong EndTime;
    public bool ManualEnd;
    public ulong RewardEndTime;
    public ulong StartTime;
}

public enum EActivityType
{
    NONE = 0,
    TYPE_ACTIVITY_EASTER, //活动-复活节
    TYPE_ACTIVITY_MONOPOLY, //大富翁
    TYPE_ACTIVITY_THEME, //主题装修
    TYPE_ACTIVITY_FRANCE, //法国主题
}

public partial class ActivityManager : ActivityResHotUpdate
{
    public const string ACTIVITY_SERVER_DATA_KEY = "ACTIVITY_SERVER_DATA_V2";
    public const string ACTIVITY_EXPIRE_TIME_KEY = "ACTIVITY_EXPIRE_TIME_V2";
    private static readonly string ACTIVITY_DATA_CACHE = "ACTIVITY_DATA_CACHE";

    private static ActivityManager _instance;

    // 缓存的  活动数据是否改变的数据结构
    public readonly Dictionary<string, ActivityCheckData> activityCheckDatas =
        new Dictionary<string, ActivityCheckData>();

    public Dictionary<string, ActivityEntityBase> _activityModules = new Dictionary<string, ActivityEntityBase>();

    private readonly Stopwatch stopWatch = new Stopwatch();

    //是否初始化过数据,如果初始化过不应该重新从本地再次初始化
    private bool initedData;

    //从服务器拉取礼包及活动配置的cd
    private int SeverActivityDataFetchCD = 180;
    private string m_ActivityData = "";
    public long m_ActivityDataFetchedTime = -1000;

    public static ActivityManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (ActivityManager) FindObjectOfType(typeof(ActivityManager));

                if (_instance == null)
                {
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<ActivityManager>();
                    singletonObject.name = " ActivityManager";
                    singletonObject.hideFlags = HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    private string ActivityCacheData
    {
        get
        {
            var encryptData = Convert.FromBase64String(PlayerPrefs.GetString(ACTIVITY_DATA_CACHE, ""));
            return RijndaelManager.Instance.DecryptStringFromBytes(encryptData);
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                PlayerPrefs.SetString(ACTIVITY_DATA_CACHE, "");
                return;
            }

            var encryptData = RijndaelManager.Instance.EncryptStringToBytes(value);
            PlayerPrefs.SetString(ACTIVITY_DATA_CACHE, Convert.ToBase64String(encryptData));
        }
    }

    private void LoadFromCache()
    {
        DebugUtil.Log("使用缓存数据初始化活动");
        SGetResActivities serverActivities = null;
        try
        {
            serverActivities = JsonConvert.DeserializeObject<SGetResActivities>(ActivityCacheData);
        }
        catch (Exception e)
        {
            DebugUtil.LogWarning("本地已缓存活动配置格式错误 : " + e.StackTrace);
            ActivityCacheData = "";
        }

        ParseActivityServerData(serverActivities);
    }

    private void Awake()
    {
        InvokeRepeating("UpdateActivity", 0, 5);
    }

    protected void UpdateActivity()
    {
        if(m_ActivityDataFetchedTime < 0)
            return;
        
        if (Time.realtimeSinceStartup - m_ActivityDataFetchedTime < SeverActivityDataFetchCD)
            return;

        RequestActivityInfosFromServer();
    }

    // 从服务器拉取活动信息
    public void RequestActivityInfosFromServer()
    {
        stopWatch.Stop();
        DebugUtil.Log("qushuang =====> RequestActivityInfosFromServer 1 =====> " + stopWatch.ElapsedMilliseconds);
        stopWatch.Restart();
        var localServerData = m_ActivityData;
        var timeout = m_ActivityDataFetchedTime;
        //LoadFromCache();
        SGetResActivities serverActivities = null;
        if (Time.realtimeSinceStartup - m_ActivityDataFetchedTime < SeverActivityDataFetchCD)
        {
            DebugUtil.Log("使用已缓存活动配置：" + localServerData);
            try
            {
                serverActivities = JsonConvert.DeserializeObject<SGetResActivities>(localServerData);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("本地已缓存活动配置格式错误 : " + e.StackTrace);
            }
        }

        DebugUtil.Log("本地缓存活动配置是否可用 ? " + (serverActivities != null));
        stopWatch.Stop();
        DebugUtil.Log("qushuang =====> RequestActivityInfosFromServer 2 =====> " + stopWatch.ElapsedMilliseconds);
        stopWatch.Restart();
        if (serverActivities == null)
        {
            DebugUtil.Log("开始请求服务器活动配置");
            m_ActivityDataFetchedTime = (long)Time.realtimeSinceStartup;
            APIManager.Instance.Send(new CGetResActivities
            {
#if UNITY_IOS
                Platform = Platform.Ios,
                Version = ulong.Parse(AssetConfigController.Instance.IOSVersionCode)
#else
                Platform = Platform.Google,
                Version = ulong.Parse(AssetConfigController.Instance.VersionCode)
#endif
            }, (IMessage obj) =>
            {
                try
                {
                    var activities = obj as SGetResActivities;

                    ParseActivityServerData(activities);

                    var json = JsonConvert.SerializeObject(activities);
                    // DebugUtil.LogWarning("获取到服务器活动配置： " + json);
                    DebugUtil.LogWarning("获取到服务器活动配置： " + json);

                    m_ActivityData = json;
                    ActivityCacheData = json;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.ToString());
                }
            }, (arg1, arg2, arg3) =>
            {
                DebugUtil.LogWarning(string.Format("活动: 获取服务器活动配置出错 " +
                                                   "error code : {0}, string : {1}, message : {2}", arg1.ToString(),
                    arg2, arg3));
            });
            return;
        }

        stopWatch.Stop();
        DebugUtil.Log("qushuang =====> RequestActivityInfosFromServer 3 =====> " + stopWatch.ElapsedMilliseconds);
        stopWatch.Restart();

        if (!initedData)
        {
            DebugUtil.Log("活动: 使用本地缓存活动配置");
            ParseActivityServerData(serverActivities);
        }
        else
        {
            DebugUtil.Log("活动: 未拉取服务器数据,本地也初始化过了,放弃初始化");
            UpdateActivityState();
        }

        stopWatch.Stop();
        DebugUtil.Log("qushuang =====> RequestActivityInfosFromServer 4 =====> " + stopWatch.ElapsedMilliseconds);
        stopWatch.Restart();
    }

    /// <summary>
    /// 解析活动数据
    /// </summary>
    /// <param name="data"></param>
    private void ParseActivityServerData(SGetResActivities data)
    {
        if (data == null)
            return;

        initedData = true;
        stopWatch.Stop();
        DebugUtil.LogError("qushuang =====> ParseActivityServerData 1 =====> " + stopWatch.ElapsedMilliseconds);
        stopWatch.Restart();
        //m_ActivityResDict.Clear();
        foreach (var activity in data.Activities)
        {
            if (_activityModules.ContainsKey(activity.ActivityType) == false)
                continue;
            
            ActivityEntityBase activityEntity = _activityModules[activity.ActivityType];
            // if (!activityEntity.CanDownLoadRes())
            // {
            //     DebugUtil.Log("活动: 暂时不下载活动资源  不满足下载活动资源条件  " + activity.ActivityId);
            //     continue;
            // }
            
            // 检查是否活动数据是否没有改变
            if (activityCheckDatas.ContainsKey(activity.ActivityId))
            {
                var activityCheckData = activityCheckDatas[activity.ActivityId];
                if (activity.ActivityId == activityCheckData.ActivityId
                    && activity.StartTime == activityCheckData.StartTime
                    && activity.EndTime == activityCheckData.EndTime
                    && activity.RewardEndTime == activityCheckData.RewardEndTime
                    && activity.ManualEnd == activityCheckData.ManualEnd
                )
                {
                    // 完全一致,不执行活动初始化
                    DebugUtil.Log("活动: 完全一致,不执行活动初始化  " + activity.ActivityId);
                    continue;
                }
            }
            else
            {
                activityCheckDatas[activity.ActivityId] = new ActivityCheckData();
            }
            
            // 不一致,执行活动初始化
            var checkData = activityCheckDatas[activity.ActivityId];
            checkData.ActivityId = activity.ActivityId;
            checkData.StartTime = activity.StartTime;
            checkData.EndTime = activity.EndTime;
            checkData.RewardEndTime = activity.RewardEndTime;
            checkData.ManualEnd = activity.ManualEnd;
            
            DebugUtil.Log("活动: 不一致,执行活动初始化  " + activity.ActivityId);
            SetActivityAllResMd5Dict(activity.ActivityId, activity.ResourcePath.ToList());
            activityEntity.InitFromServerData(activity.ActivityId, activity.ActivityType,
                activity.StartTime, activity.EndTime, activity.RewardEndTime, activity.ManualEnd,
                activity.Config, activity.ActivitySubType);
            if (!activity.ManualEnd)
                activityEntity.UpdateActivityUsingResList(activity.ActivityId,false);
            //CheckActivityResState();

            AddActivityCache(activityEntity.IsCache, activity.ActivityType, activity.ActivityId, activity.Config, (long)activity.StartTime, (long)activity.EndTime);
        }

        CheckActivityResState();
        UpdateActivityState();
        foreach (var kv in m_ActivityResDict)
        {
            var activity = data.Activities.ToList().Find(a => a.ActivityId == kv.Key);
            if(activity == null)
                continue;
            
            var activityEntity = _activityModules[activity.ActivityType];
            if (!activityEntity.CanDownLoadRes())
            {
                DebugUtil.Log("活动: 暂时不下载活动资源  不满足下载活动资源条件  " + activity.ActivityId);
                continue;
            }
            TryPullSingleActivityRes(kv.Key);
        }
        TryPullOtherRes();
        //Game.EventManager.Instance.UIEvtDispatcher.DispatchEvent(EventEnum.ACTIVITY_CHANGE);
    }

    public void SetActivityAllResMd5Dict(string activityId,List<string> allResList)
    {
        if (m_ActivityAllResDict.ContainsKey(activityId))
        {
            DebugUtil.Log("### 重设活动所有资源列表 ###" + activityId);
            m_ActivityAllResDict[activityId] = allResList;
        }
        else
        {
            DebugUtil.Log("### 新增活动所有资源列表 ###" + activityId);
            m_ActivityAllResDict.Add(activityId, allResList);
        }
        // 初始化每个活动的资源更新情况
        if (!m_ActivityResDownloadedDict.ContainsKey(activityId))
            m_ActivityResDownloadedDict.Add(activityId, false);
    }
    public List<string> GetActivityMd5List(string activityId)
    {
        if (!m_ActivityResDict.ContainsKey(activityId))
            return null;
        return m_ActivityResDict[activityId];
    }

    public List<string> GetActivityAllMd5List(string activityId)
    {
        if (!m_ActivityAllResDict.ContainsKey(activityId))
            return null;
        return m_ActivityAllResDict[activityId];
    }
    public bool CheckResMd5Exist(List<string> md5List)
    {
        foreach (var fileNameWithMd5 in md5List)
            if (IsFileNeedDownload(fileNameWithMd5)) //只要该活动有任意资源需要更新，就返回false
                return false;
        return true;
    }
    public bool CheckResExist(List<string> resPathList)
    {
        foreach (var fileName in resPathList)
        {
            var totalFilePath = string.Format("{0}/{1}", FilePathTools.persistentDataPath_Platform, fileName);
            if (!System.IO.File.Exists(totalFilePath))
                return false;
        }
        return true;
    }

    public void UpdateActivityUsingResList(string activityId,List<string> resList)
    {
        if (m_ActivityResDict.ContainsKey(activityId))
        {
            DebugUtil.Log("### 更新活动使用中资源列表 ###" + activityId);
            m_ActivityResDict[activityId] = resList;
        }
        else
        {
            DebugUtil.Log("### 新增活动使用中资源列表 ###" + activityId);
            m_ActivityResDict.Add(activityId, resList);
        }
    }
    

    public void TryPullSingleActivityRes(string activityId)
    {
        if (m_ActivityResDict.TryGetValue(activityId, out var resList))
        {
            if (!ActivityDownloadStateDictionary.TryGetValue(activityId,out var downloadStatus))
            {
                downloadStatus = new ActivityDownloadStatus();
                ActivityDownloadStateDictionary.Add(activityId, downloadStatus);
            }
            else
            {
                return;
            }
            foreach (var filename in resList)
            {
                downloadStatus.ResList.Add(filename);
                if (IsFileNeedDownload(filename) == false)
                    continue;

                DebugUtil.Log("### 开始下载活动资源: ####" + filename);
                var downloadInfo = DownloadManager.Instance.DownloadInSeconds(activityId, filename, ActivityResType.Activity, downloadinfo =>
                {
                    if (downloadinfo.result == DownloadResult.Success)
                    {
                        DownLoadPost(filename);
                        Instance.CheckActivityResState();
                        //Game.EventManager.Instance.UIEvtDispatcher.DispatchEvent(EventEnum.ACTIVITY_CHANGE);
                        return;
                    }

                    if (downloadinfo.result != DownloadResult.ForceAbort)
                        DebugUtil.LogError(string.Format("can not download : {0}", downloadinfo.url));
                    ActivityDownloadStateDictionary.Remove(activityId);

                });
                downloadStatus.DownloadInfoList.Add(downloadInfo);
            }
        }
    }
    public float GetActivityResourcesDownloadProgress(string activityId)
    {
#if UNITY_EDITOR
        if (EditorPrefs.GetBool("cache_editor_ignore_activity_res_check")) return 1f;
#endif

        if (string.IsNullOrEmpty(activityId))
            return 0f;

        if (!ActivityDownloadStateDictionary.ContainsKey(activityId))
            return 0f;

        return ActivityDownloadStateDictionary[activityId].GetDownloadProgress();
    }
    private Dictionary<string, ActivityDownloadStatus> ActivityDownloadStateDictionary = new Dictionary<string, ActivityDownloadStatus>();
    public class ActivityDownloadStatus
    {
        public List<string> ResList = new List<string>();
        public List<DownloadInfo> DownloadInfoList = new List<DownloadInfo>();

        public float GetDownloadProgress()
        {
            var progress = (ResList.Count - DownloadInfoList.Count) * 1f;
            foreach (var downloadInfo in DownloadInfoList)
            {
                progress += downloadInfo.currProgress;
            }
            progress /= ResList.Count;
            return progress;
        }
    }
    public void UpdateActivityState()
    {
        if (initedData == false)
            return;

        if (_activityModules == null || _activityModules.Count <= 0)
            return;

        foreach (var itemData in _activityModules)
        {
            itemData.Value.UpdateActivityState();
        }
    }

    public void TryPullOtherRes()
    {
        if (initedData == false)
            return;

        if (_activityModules == null || _activityModules.Count <= 0)
            return;

        foreach (var itemData in _activityModules)
        {
            itemData.Value.CheckOrDownloadOtherRes();
        }
    }

    public void OnPurchased(int shopId)
    {
        if (_activityModules == null)
            return;
        foreach (var itemData in _activityModules)
        {
            if (itemData.Value == null)
                continue;
            itemData.Value.OnPurchased(shopId);
        }
    }
}