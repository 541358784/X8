using System;
using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;

public partial class BlindBoxModel
{
    private List<DownloadInfo> _allTask = new List<DownloadInfo>();
    public List<DownloadInfo> DownloadFiles(Dictionary<string, string> needDownloadFiles, Action<bool> onFinished)
    {
        var downloadSuccess = true;
    
        if (needDownloadFiles.Count > 0) // 去下载
        {
            var downloadedInTask = 0;
            foreach (var kv in needDownloadFiles)
            {
                var fileName = kv.Key;
                var fileMd5 = kv.Value;
                if (_allTask.Find(t => t.fileMd5 == fileMd5) != null) continue;
    
                downloadedInTask++;
                var info = DownloadManager.Instance.DownloadInSeconds(fileName, fileMd5, (downloadinfo) =>
                {
                    downloadedInTask--;
    
                    _allTask.Remove(downloadinfo);
    
                    if (downloadinfo.result == DownloadResult.Success)
                    {
                        AssetCheckManager.Instance.PathMd5Dictionary.Remove(fileName);
                        VersionManager.Instance.RefreshRemoteToLocal(new List<string>(new[] { kv.Key }));
                    }
                    else // 超时，或者 重试3次后依然下载错误
                    {
                        if (downloadinfo.result != DownloadResult.ForceAbort) // 强制终止下载的不算
                        {
                            downloadSuccess = false;
                        }
                    }
    
                    if (downloadedInTask <= 0) //所有文件都处理完了
                    {
                        onFinished?.Invoke(downloadSuccess);
                        if (!downloadSuccess)
                        {
                            OnDownloadError(null);
                        }
                    }
                });
                _allTask.Add(info);
                //DragonPlus.GameBIManager.SendGameEvent(BiEventMergeCooking.Types.GameEventType.GameEventDownloadStart, fileName, info.downloadSize.ToString());
            }
    
            if (downloadedInTask == 0) onFinished?.Invoke(downloadSuccess);
        }
        else //不需要更新任何文件
        {
            onFinished?.Invoke(downloadSuccess);
        }
    
        var taskList = _allTask.FindAll(info =>
        {
            //needDownloadFiles.Values.Contains(info.fileMd5
            foreach (var v in needDownloadFiles.Values)
            {
                if (v.Contains(info.fileMd5)) return true;
            }
            return false;
        });
    
        return taskList;
    }

    public void OnDownloadError(BaseEvent e)
    {
        _allTask.Clear();
    }
    
    public void UpdateProgressFromDownloadInfoList(List<DownloadInfo> downloadTaskList, Action<float, string> onProgressUpdate)
    {
        var taskCount = downloadTaskList.Count;
        if(taskCount == 0)
            return;
              
        var downloadedBytes = 0f;
        var totalBytes = 0f;
        var allSizeGot = true;
        for (int i = 0; i < taskCount; i++)
        {
            if (downloadTaskList[i].downloadSize >= downloadTaskList[i].downloadedSize) //确保get httphead之后，才开始算进度
            {
                totalBytes += downloadTaskList[i].downloadSize;
                downloadedBytes += downloadTaskList[i].downloadedSize;
            }

            if (downloadTaskList[i].downloadSize <= 0) allSizeGot = false;
        }

        if (totalBytes > 0)
        {
            var downloadStr = string.Format("{0:N1}M", downloadedBytes / 1024 / 1024);
            var totalStr = string.Format("{0:N1}M", totalBytes / 1024 / 1024);
            var extralInfo = $"{downloadStr}/{totalStr}";
            if (!allSizeGot) extralInfo = string.Empty; //LocalizationManager.Instance.GetLocalizedString("UI_loading_count");

            onProgressUpdate?.Invoke(downloadedBytes / totalBytes, extralInfo);
        }
    }
    
    public bool IsResReady(StorageBlindBox storage)
    {
        var themeConfig = ThemeConfigDic[storage.ThemeId];
        var assets = themeConfig.GetNeedDownloadAssets();
        if (assets == null || assets.Count == 0)
        {
            return true;
        }
        return false;
    }
    public async void TryDownloadRes(StorageBlindBox storage,Action<float, string> onProgressUpdate = null,Action<bool> onFinish = null)
    {
        var themeConfig = ThemeConfigDic[storage.ThemeId];
        var assets = themeConfig.GetNeedDownloadAssets();
        if (assets == null || assets.Count == 0)
        {
            return;
        }
        var downLoadFinish = false;
        var downloadTaskList = DownloadFiles(assets, (success) => downLoadFinish = true);
        
        while (!downLoadFinish)
        {
            UpdateProgressFromDownloadInfoList(downloadTaskList, (progress,extralInfo)=>
            {
                Debug.LogError("盲盒主题"+themeConfig.Id+" 下载进度:"+progress*100+"% "+extralInfo);
                if (onProgressUpdate!=null)
                    onProgressUpdate(progress, extralInfo);
            });
            await XUtility.WaitSeconds(0.1f);
        }
        
        foreach (var info in downloadTaskList)
        {
            if (info.result != DownloadResult.Success)
            {
                if (onFinish != null)
                    onFinish(false);
                return;
            }
        }
        if (onFinish != null)
            onFinish(true);
    }
}