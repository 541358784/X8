using System.Collections.Generic;
using System;
using System.Collections;
using BestHTTP;
using BestHTTP.Caching;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

namespace DragonU3DSDK
{
	public class NetworkFileIOManager : Manager<NetworkFileIOManager>
	{
		// 数据流块的大小 bytes
		private readonly int streamFragmentSize = 1024 * 1024;
		// 超时时间 秒
		private readonly int timeout = 60;
		// 最大本地缓存 bytes
		private readonly ulong maxCacheSize = 100 * 1024 * 1024;
		// 最长缓存时间 天
		private readonly int maxCacheDays = 7;

		// downloadingActions arg1:是否成功 arg2:字节流 arg3:状态码
		private Dictionary<string, Action<bool, byte[], int>> downloadingActions = new Dictionary<string, Action<bool, byte[], int>>();
		private ReaderWriterLockSlim rwLockOfDownloadingActions = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		protected override void InitImmediately()
		{
			base.InitImmediately();

			HTTPCacheService.BeginMaintainence(new HTTPCacheMaintananceParams(TimeSpan.FromDays(maxCacheDays), maxCacheSize));
			DragonU3DSDK.DebugUtil.Log("http cache is support : " + HTTPCacheService.IsSupported);
			DragonU3DSDK.DebugUtil.Log("http cache path : " + HTTPCacheService.CacheFolder);
		}

        public bool IsImageDownloaded(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            string key = Url2Key(url);

            var memoryCacheData = FileMemoryCacheManager.Instance.GetDataFromMemoryCache(key);
            if (null != memoryCacheData)
            {
                return true;
            }
            return false;
        }

        // 返回值不为空代表有缓存， 否早会在下载好之后调用onResponse
        public byte[] GetFileBytesWithUrl(string url, Action<bool, byte[], int> onResponse = null)
		{
			if (string.IsNullOrEmpty(url))
			{
				DragonU3DSDK.DebugUtil.LogWarning("GetFileBytesWithUrl url is null or empty");
				return null;
			}

            string key = Url2Key(url);

            // 先尝试从内存中获取
            var memoryCacheData = FileMemoryCacheManager.Instance.GetDataFromMemoryCache(key);
			if (null != memoryCacheData)
			{
				onResponse?.Invoke(true, memoryCacheData, 0);
				return memoryCacheData;
			}

			rwLockOfDownloadingActions.EnterWriteLock();
			try
			{
                if (!downloadingActions.ContainsKey(key))
				{
					downloadingActions[key] = onResponse;
					StartCoroutine(DownloadFile(url));
				}
				else
				{
					downloadingActions[key] += onResponse;
				}
			}
			finally
			{
				rwLockOfDownloadingActions.ExitWriteLock();
			}

			return null;
		}

		// 移除掉url对应的onResponse, 注意如果onResponse中调用的UI已经销毁，需要移除掉onResponse
		public void RemoveGetFileBytesResponse(string url)
		{
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            string key = Url2Key(url);
			
            rwLockOfDownloadingActions.EnterWriteLock();
            try
            {
                if (downloadingActions.ContainsKey(key))
                {
                    downloadingActions.Remove(key);
                }
            }
            finally
            {
                rwLockOfDownloadingActions.ExitWriteLock();
            }
        }

		// 下载文件
		private IEnumerator DownloadFile(string url)
		{
			List<byte> totalFragments = new List<byte>();
			HTTPRequest request = new HTTPRequest(new Uri(url), (req, resp) =>
			{
				if (null != resp)
				{
					List<byte[]> fragments = resp.GetStreamedFragments();
					foreach (var data in fragments)
					{
						totalFragments.AddRange(data);
					}
				}

				if (null != resp && resp.IsStreamingFinished)
				{
					DragonU3DSDK.DebugUtil.Log("download finished : " + url + " , isSuccess : " + resp.IsSuccess + " , StatusCode : " + resp.StatusCode);

                    string key = Url2Key(url);
					FileMemoryCacheManager.Instance.AddDataToMemoryCache(key, totalFragments.ToArray());

					rwLockOfDownloadingActions.EnterWriteLock();
					try
					{
						if (downloadingActions.ContainsKey(key))
						{
							downloadingActions[key]?.Invoke(resp.IsSuccess, totalFragments.ToArray(), resp.StatusCode);
							downloadingActions.Remove(key);
						}
					}
					finally
					{
						rwLockOfDownloadingActions.ExitWriteLock();
					}
				}
				else
				{
                    if (resp == null)
                    {
                        DragonU3DSDK.DebugUtil.LogError("download unfinished : " + url);
                        string key = Url2Key(url);

                        rwLockOfDownloadingActions.EnterWriteLock();
                        try
                        {
                            if (downloadingActions.ContainsKey(key))
                            {
                                downloadingActions[key]?.Invoke(false, totalFragments.ToArray(), 0);
                                downloadingActions.Remove(key);
                            }
                        }
                        finally
                        {
                            rwLockOfDownloadingActions.ExitWriteLock();
                        }
                    }
				}
			})
			{
				UseStreaming = true,
				StreamFragmentSize = streamFragmentSize,
				DisableCache = false,
				Timeout = TimeSpan.FromSeconds(timeout)
			};

			request.Send();

			yield return null;
		}

        // URL 转换成key 防止url过长 浪费内存
        private string Url2Key(string url)
        {
            string key = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(url));

                    StringBuilder sBuilder = new StringBuilder();

                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    key = sBuilder.ToString();
                }
            }

            return key;
        }
	}
}
