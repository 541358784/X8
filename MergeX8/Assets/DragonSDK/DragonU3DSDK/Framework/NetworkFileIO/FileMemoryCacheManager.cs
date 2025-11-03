using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Collections.Specialized;

namespace DragonU3DSDK
{
	public class FileMemoryCacheManager : Manager<FileMemoryCacheManager>
	{
		// 内存中最大缓存 bytes
		private readonly int maxMemoryCacheSize = 50 * 1024 * 1024;
		// 内存中缓存的最大文件限制 bytes
		private readonly int maxSingleFileMemoryCacheSize = 5 * 1024 * 1024;


		// 当前内存中缓存大小
		private int CurrentMemoryCacheSize { get; set; }

		struct CacheEntry
		{
			public byte[] data;
		}

		// 数据缓存
		private OrderedDictionary dataMemoryCache = new OrderedDictionary();
		private ReaderWriterLockSlim rwLockOfDataMemoryCache = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		// 把数据缓存到内存里面
		public void AddDataToMemoryCache(string key, byte[] data)
		{
			if (!string.IsNullOrEmpty(key) && data != null && data.Length < maxSingleFileMemoryCacheSize)
			{
				rwLockOfDataMemoryCache.EnterWriteLock();
				try
				{
					// 如果存在，先删除，在添加，保证访问顺序
					if (dataMemoryCache.Contains(key))
					{
						CurrentMemoryCacheSize -= ((CacheEntry)dataMemoryCache[key]).data.Length;
						dataMemoryCache.Remove(key);
					}

					if (CurrentMemoryCacheSize + data.Length > maxMemoryCacheSize)
					{
						List<string> removeKeys = new List<string>();
						int removeSize = 0;
						foreach (DictionaryEntry entry in dataMemoryCache)
						{
							if (CurrentMemoryCacheSize - removeSize + data.Length <= maxMemoryCacheSize)
							{
								break;
							}

							removeKeys.Add((string)entry.Key);
							removeSize += ((CacheEntry)entry.Value).data.Length;
						}

						foreach (var removeKey in removeKeys)
						{
							dataMemoryCache.Remove(removeKey);
						}
                        CurrentMemoryCacheSize -= removeSize;
                    }

					dataMemoryCache.Add(key, new CacheEntry()
					{
						data = data,
					});

					CurrentMemoryCacheSize += data.Length;
					DragonU3DSDK.DebugUtil.Log("File data memory cache size : " + CurrentMemoryCacheSize);
				}
				finally
				{
					rwLockOfDataMemoryCache.ExitWriteLock();
				}
			}
		}

		// 从缓存中取数据
		public byte[] GetDataFromMemoryCache(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}

			rwLockOfDataMemoryCache.EnterReadLock();
			try
			{
				if (dataMemoryCache.Contains(key))
				{
					CacheEntry cache = (CacheEntry)dataMemoryCache[key];
					dataMemoryCache.Remove(key);
					dataMemoryCache.Add(key, cache);
					return cache.data;
				}
			}
			finally
			{
				rwLockOfDataMemoryCache.ExitReadLock();
			}

			return null;
		}

		// 清除内存缓存
		public void ClearMemoryCache()
		{
			rwLockOfDataMemoryCache.EnterReadLock();
			try
			{
				dataMemoryCache.Clear();
				CurrentMemoryCacheSize = 0;
			}
			finally
			{
				rwLockOfDataMemoryCache.ExitReadLock();
			}
		}
	}
}