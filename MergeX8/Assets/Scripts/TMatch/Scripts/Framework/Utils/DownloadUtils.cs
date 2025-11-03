using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using Framework;
using UnityEngine;

namespace TMatch
{
    public enum DownloadContentState
    {
        None = -1,
        NeedDownload = 0, //资源未下载
        Downloading, //资源下载中
        Downloaded, //已全部下载到本地
    }

    public interface DownloadingListener
    {
        void Prepare2Download(bool isDownloadingStarter);
        void OnDownloadVersionFileFailed();
        void OnDownloadResFailed();
        void OnDownloadFinished();
        void OnDownloading(float rate);
    }

    public static class DownloadUtils
    {
        /// <summary>
        /// 流程：
        /// 1、下载version文件。 成功：比对hash获取要下载的文件   失败：退出下载，刷新界面
        /// 2、下载版本低于服务器上的资源文件。 全部成功：更新本地version  任意文件失败或超时：退出下载，刷新界面
        /// </summary>
        /// <param name="listener">下载监听</param>
        /// <param name="needDownloadFiles">需要下载的资源</param>
        /// <param name="taskKey"></param>
        /// <returns></returns>
        public static IEnumerator DownloadRes(DownloadingListener listener, List<LevelResVisiter.FileInfo> fileInfos)
        {
            var versionFileLoaded = false;
            var resourceDownloaded = false;
            var downloadVersionFileFailed = false;
            var downloadResFailed = false;
            var needDownloadFiles = new Dictionary<string, string>();
    
            // 先更新version文件
            VersionManager.Instance.LoadVersionFile((bool success) =>
            {
                if (success)
                {
                    if (fileInfos != null)
                    {
                        foreach (var inf in fileInfos)
                        {
                            needDownloadFiles.Add(inf.key, inf.md5);
                        }
                    }
    
                    versionFileLoaded = true;
                }
                else
                {
                    versionFileLoaded = true;
                    downloadVersionFileFailed = true;
                }
            });
    
            while (!versionFileLoaded)
            {
                yield return new WaitForEndOfFrame();
            }
    
            if (downloadVersionFileFailed)
            {
                listener.OnDownloadVersionFileFailed();
                yield break;
            }
    
            var downloadTask = new List<DownloadInfo>();
            if (needDownloadFiles.Count > 0) //去下载
            {
                var resCount = needDownloadFiles.Count;
                foreach (var kv in needDownloadFiles)
                {
                    var info = DownloadManager.Instance.DownloadInSeconds(kv.Key, kv.Value, (downloadinfo) =>
                    {
                        if (downloadinfo.result == DownloadResult.Success)
                        {
                            VersionManager.Instance.RefreshRemoteToLocal(new List<string>(new[] {kv.Key}));
                            resCount--;
                            if (resCount <= 0) // 所有文件都成功下载到本地了
                            {
                                resourceDownloaded = true;
                            }
                        }
                        else
                        {
                            // 超时，或者 重试3次后依然下载错误
                            resourceDownloaded = true;
                            downloadResFailed = true;
                        }
                    });
                    downloadTask.Add(info);
                }
            }
            else
            {
                //不需要下载任何文件
                resourceDownloaded = true;
            }
    
            while (!resourceDownloaded)
            {
                float totalProgress = 0f;
                // listener.OnDownloading(totalProgress);//这里一直为0，注释
                foreach (DownloadInfo info in downloadTask)
                {
                    totalProgress += info.currProgress;
                }
    
                float rate = totalProgress / downloadTask.Count;
                listener.OnDownloading(rate);
                yield return new WaitForEndOfFrame();
            }
    
            if (downloadResFailed)
            {
                listener.OnDownloadResFailed();
                yield break;
            }
    
            listener.OnDownloadFinished();
        }
    }
}