using System;
using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Asset;

namespace ConnectLineSpace
{
    public partial class Model: Manager<Model>
    {
        private List<DownloadInfo> _allDownLoadTask = new List<DownloadInfo>();
        
        public List<AssetGroup> GetNeedDownloadAssetGroups(int levelId)
        {
            return null;
        }
        
        public List<AssetGroup> GetBaseDownloadAssetGroups()
        {
            var assets = new List<AssetGroup>();
            
            var variantPostFix = ResourcesManager.Instance.IsSdVariant ? "sd" : "hd";
            assets.Add(new AssetGroup("ConnectLine", $"spriteatlas/connectlineatlas/{variantPostFix}.ab"));
            return assets;
        }

        public Dictionary<string, string> GetBaseDownLoadAssets()
        {
            List<AssetGroup> resGroupList = GetBaseDownloadAssetGroups();
            if (resGroupList == null) 
                return new Dictionary<string, string>();

            return AssetCheckManager.Instance.ResDownloadFitter(resGroupList);
        }
        
        public Dictionary<string, string> GetNeedDownloadAssets(int levelId)
        {
            List<AssetGroup> resGroupList = GetNeedDownloadAssetGroups(levelId);
            if (resGroupList == null) 
                return new Dictionary<string, string>();

            return AssetCheckManager.Instance.ResDownloadFitter(resGroupList);
        }
        
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
                    if (_allDownLoadTask.Find(t => t.fileMd5 == fileMd5) != null) continue;

                    downloadedInTask++;
                    var info = DownloadManager.Instance.DownloadInSeconds(fileName, fileMd5, (downloadinfo) =>
                    {
                        downloadedInTask--;

                        _allDownLoadTask.Remove(downloadinfo);

                        if (downloadinfo.result == DownloadResult.Success)
                        {
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
                    _allDownLoadTask.Add(info);
                }

                if (downloadedInTask == 0) onFinished?.Invoke(downloadSuccess);
            }
            else //不需要更新任何文件
            {
                onFinished?.Invoke(downloadSuccess);
            }

            var taskList = _allDownLoadTask.FindAll(info =>
            {
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
              _allDownLoadTask.Clear();
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
    }
}