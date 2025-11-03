using System.Collections.Generic;
using System.IO;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using UnityEditor;
using UnityEngine;

public class ActivityResHotUpdate : MonoBehaviour
{
    public const int ACTIVITY_RES_CHECK_INTERVAL = 60; //每个活动每隔xx秒检查一次资源是否是最新

    // <活动id, 资源列表>
    protected Dictionary<string, List<string>> m_ActivityResDict = new Dictionary<string, List<string>>();
    protected Dictionary<string, List<string>> m_ActivityAllResDict = new Dictionary<string, List<string>>();

    // <活动id, 资源是否是最新>
    protected Dictionary<string, bool> m_ActivityResDownloadedDict = new Dictionary<string, bool>();
    protected float m_ResCheckTicker;

    private void Update()
    {
        m_ResCheckTicker += Time.deltaTime;
        if (m_ResCheckTicker >= ACTIVITY_RES_CHECK_INTERVAL)
        {
            //CheckActivityResState();
        }
    }

    // 检查各个活动的资源更新状态
    public void CheckActivityResState()
    {
        var keys = new List<string>();

        foreach (var key in m_ActivityResDownloadedDict.Keys) keys.Add(key);

        foreach (var key in keys) CheckSingleActivityResState(key);
        m_ResCheckTicker = 0f;
    }

    public void CheckSingleActivityResState(string activityId)
    {
        if (m_ActivityResDownloadedDict.ContainsKey(activityId)) 
            m_ActivityResDownloadedDict[activityId] = IsActivityDownloadAllRes(activityId);
    }

    // 判断一个活动的资源是不是全更新到最新了
    public bool IsActivityResourcesDownloaded(string activityId)
    {
#if UNITY_EDITOR
        if (EditorPrefs.GetBool("cache_editor_ignore_activity_res_check")) return true;
#endif

        if (string.IsNullOrEmpty(activityId))
            return false;

        if (!m_ActivityResDownloadedDict.ContainsKey(activityId))
            return false;

        return m_ActivityResDownloadedDict[activityId];
    }

    private bool IsActivityDownloadAllRes(string activityId)
    {
        if (m_ActivityResDict.ContainsKey(activityId))
        {
            var resList = m_ActivityResDict[activityId];
            foreach (var fileNameWithMd5 in resList)
                if (IsFileNeedDownload(fileNameWithMd5)) //只要该活动有任意资源需要更新，就返回false
                    return false;
        }
        else
        {
            //DebugUtil.LogError("Activity id error:" + activityId);
            //活动已关闭或结束
            return false;
        }

        return true;
    }

    protected bool IsFileNeedDownload(string fileNameWithMd5)
    {
        var fileNameWithNoSuffix = fileNameWithMd5.Substring(0, fileNameWithMd5.Length - 3); //移除".ab"
        var subPaths = fileNameWithNoSuffix.Split('_');
        var fileName = "";
        string fileHash;
        //------------------ 拼接fileName
        for (var i = 0; i < subPaths.Length - 2; i++)
        {
            if (i > 0) fileName += "/";

            fileName += subPaths[i];
        }

        fileName += ".ab";

        //------------------ 提取hash
        fileHash = subPaths[subPaths.Length - 2];

        var localFilePath = string.Format("{0}/{1}", FilePathTools.persistentDataPath_Platform, fileName);
        var localFilePath_Hash = string.Format("{0}.{1}", localFilePath, fileHash);
        if (!File.Exists(localFilePath_Hash)) //本地不存在，说明要下载
        {
            DebugUtil.Log(string.Format("活动资源:{0}需要下载", fileNameWithMd5));
            return true;
        }

        return false;
    }
    public static string GetFilePath(string fileNameWithMd5)
    {
        var fileNameWithNoSuffix = fileNameWithMd5.Substring(0, fileNameWithMd5.Length - 3); //移除".ab"
        var subPaths = fileNameWithNoSuffix.Split('_');
        var fileName = "";
        //------------------ 拼接fileName
        for (var i = 0; i < subPaths.Length - 2; i++)
        {
            if (i > 0) fileName += "/";
            fileName += subPaths[i];
        }
        fileName += ".ab";
        return fileName;
    }

    //下载活动资源的一些后续操作
    //1.解压
    //2.生成一个名字末尾带hash值的空文件，记录资源的hash值，用于资源迭代。
    protected void DownLoadPost(string fileNameWithMd5)
    {
        var fileNameWithNoSuffix = fileNameWithMd5.Substring(0, fileNameWithMd5.Length - 3); //移除".ab"
        var subPaths = fileNameWithNoSuffix.Split('_');
        var fileName = "";
        //------------------ 拼接fileName
        for (var i = 0; i < subPaths.Length - 2; i++)
        {
            if (i > 0) fileName += "/";
            fileName += subPaths[i];
        }

        fileName += ".ab";

        //------------------ 提取hash
        var fileHash = subPaths[subPaths.Length - 2];

        //DragonU3DSDK.Asset.Zip.Tool.DecompressAB(fileName, () =>
        //{
        var path = string.Format("{0}/{1}", FilePathTools.persistentDataPath_Platform, fileName);
        var path_hash = string.Format("{0}.{1}", path, fileHash);
        if (!File.Exists(path_hash))
        {
            var stream = File.Create(path_hash);
            stream.Flush();
            stream.Close();

            DebugUtil.Log(string.Format("ActivityResHotUpdate DownLoadPost : {0}", path_hash));
        }

        //});
    }
}