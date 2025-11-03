using System;
using System.Collections.Generic;
using BestHTTP;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using Framework;

public class CustomDownloadSubsystem : GlobalSystem<CustomDownloadSubsystem>, IUpdatable
{
    public const int MaxDownloadCount = 4;
    private int currentDownloadCount = 0;

    class DownloadInfo
    {
        public string fileUrl;
        public Action<bool, HTTPResponse> callback;
    }

    private Queue<DownloadInfo> pendingDownloads = new Queue<DownloadInfo>();

    private int CurrentDownloadCount
    {
        get { return currentDownloadCount; }
        set
        {
            currentDownloadCount = value;
            if (currentDownloadCount > MaxDownloadCount)
            {
                DebugUtil.LogError($"{GetType()}: currentDownloadCount exceed, value = {currentDownloadCount}");
            }
        }
    }


    /// <summary>
    /// 首先获取md5文件
    /// </summary>
    /// <returns>The and get md5 text.</returns>
    /// <param name="resFullName">Res full name.</param>
    ///
    public void Download(string fileUrl, Action<bool, HTTPResponse> callback)
    {
        pendingDownloads.Enqueue(new DownloadInfo() {fileUrl = fileUrl, callback = callback});
    }


    public void _Download(string fileUrl, Action<bool, HTTPResponse> callback)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                DebugUtil.Log("url为空：" + fileUrl);
                callback?.Invoke(false, null);
                return;
            }

            CurrentDownloadCount = currentDownloadCount + 1;
            new HTTPRequest(new Uri(fileUrl), (req, response) =>
            {
                CurrentDownloadCount = currentDownloadCount - 1;
                if (response == null)
                {
                    DebugUtil.LogError("Response null. Server unreachable? Try again later. {0}", fileUrl);

                    callback?.Invoke(false, null);
                }
                else if (response.StatusCode >= 200 && response.StatusCode < 300)
                {
                    callback?.Invoke(true, response);
                }
                else
                {
                    DebugUtil.LogError("Unexpected response from server: {0}, StatusCode = {1}", fileUrl,
                        response.StatusCode);
                    callback?.Invoke(false, null);
                }
            })
            {
                DisableCache = true,
                IsCookiesEnabled = false,
                ConnectTimeout = TimeSpan.FromSeconds(5),
                Timeout = TimeSpan.FromSeconds(10)
            }.Send();
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    public void CancelAllDownloading()
    {
        pendingDownloads.Clear();
        CurrentDownloadCount = 0;
    }

    public void Update(float deltaTime)
    {
        var toDownloadCount = MaxDownloadCount - currentDownloadCount;
        if (toDownloadCount > 0 && pendingDownloads.Count > 0)
        {
            var downloadInfo = pendingDownloads.Dequeue();
            _Download(downloadInfo.fileUrl, downloadInfo.callback);
        }
    }
}