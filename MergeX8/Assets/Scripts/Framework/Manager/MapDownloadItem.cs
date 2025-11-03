using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Asset;
using DragonPlus;
using DragonU3DSDK;

public class MapDownloadItem : MonoBehaviour
{
    private Action<float> _progressCallback;
    private string _taskTag;

    public List<DownloadInfo> taskList;

    private void Awake()
    {
    }

    public void AttachTask(string tag, List<DownloadInfo> tasks, Action<float> callback)
    {
        _taskTag = tag;
        _progressCallback = callback;

        taskList = tasks;
    }

    private void Update()
    {
        if (taskList == null || taskList.Count <= 0) return;

        var totalProgress = 0f;
        var downloadedAll = true;
        foreach (var info in taskList)
        {
            totalProgress += info.currProgress;

            if (!info.isFinished)
                downloadedAll = false;
        }

        var rate = totalProgress / taskList.Count;
        RefreshProgress(rate);

        if (downloadedAll)
        {
            MapDownloadMgr.Instance.RemoveTask(this._taskTag);
            taskList.Clear();
        }
    }

    public void RefreshProgress(float progress)
    {
        if (_progressCallback != null)
        {
            try
            {
                _progressCallback(progress);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }
    }

    private void OnDestroy()
    {
        if (taskList == null || taskList.Count == 0)
            return;
        foreach (var info in taskList)
        {
            DownloadManager.Instance.AbortDownloadTask(info.fileName);
        }
    }
}