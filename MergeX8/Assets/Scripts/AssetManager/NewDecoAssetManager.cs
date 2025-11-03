using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using System.IO;
using BestHTTP;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Newtonsoft.Json;
using File = System.IO.File;
using Object = UnityEngine.Object;


public class AssetData
{
    public string resKey;
    public UnityEngine.Object asset;
    public int refCount;
    public float lastUsingTime;
}


public class NewDecoAssetManager : GlobalSystem<NewDecoAssetManager>, IInitable
{
    private readonly Dictionary<string, AssetData> _allRes =
        new Dictionary<string, AssetData>(); //所有资源

    private Dictionary<string, DateTime> _dicDownTime = new Dictionary<string, DateTime>(); //BI 记录下载资源所用时间
    private string _httpsHeadPath;
    private SortedSet<AssetData> _toBeCleaned = new SortedSet<AssetData>(new ByLastUsingTime());
    private const float cleanInterval = 3f;
    private const uint savedAssetOnClean = 3;

    public void Init()
    {
        ClearDownloadDir();
        _httpsHeadPath = Path.Combine(FilePathTools.AssetBundleDownloadPath, PathManager.dynSubPath);
        CoroutineManager.Instance.StartCoroutine(_CoTimerCleanUnusedAsset());
        DownloadManager.Instance.SetMaxDownloadsCount(CustomDownloadSubsystem.MaxDownloadCount);
    }

    public void Release()
    {
    }

    public ResObject Load<T>(string resKey, Action<Object> callback) where T : Object
    {
        try
        {
            var resObj = new ResObject(callback);
            AssetData assetData;
            if (_allRes.TryGetValue(resKey, out assetData))
            {
                resObj.Load(assetData);
                return resObj;
            }

            string filePath = _GetFilePath(resKey);
            if (string.IsNullOrEmpty(filePath))
            {
                DebugUtil.LogWarning("缺少资源: " + resKey);
                callback?.Invoke(null);
                resObj.Dispose();
                return null;
            }

            _Download(filePath, delegate(bool success)
            {
                if (success && !resObj.Disposed)
                {
                    var asset = _LoadAsset<T>(filePath);
                    assetData = _BuildAssetData(resKey, asset);
                    resObj.Load(assetData);
                }
            });
            return resObj;
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return null;
    }

    public ResObject LoadJpeg(string resKey, Action<Object> callback)
    {
        try
        {
            var resObj = new ResObject(callback);
            AssetData assetData;
            if (_allRes.TryGetValue(resKey, out assetData))
            {
                resObj.Load(assetData);
                return resObj;
            }

            string filePath = _GetFilePath(resKey);
            if (string.IsNullOrEmpty(filePath))
            {
                DebugUtil.LogWarning("缺少资源: " + resKey);
                callback?.Invoke(null);
                resObj.Dispose();
                return null;
            }

            // 兼容老版本
            if (Path.GetExtension(filePath) == ".ab")
            {
                resObj.Dispose();
                return Load<Sprite>(resKey, callback);
            }

            _Download(filePath, delegate(bool success)
            {
                if (success)
                {
                    var asset = _LoadJpgAsset(filePath);
                    assetData = _BuildAssetData(resKey, asset);
                    resObj.Load(assetData);
                }
            });
            return resObj;
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return null;
    }

    public void Download(string resKey, Action<bool> callback)
    {
        try
        {
            string filePath = _GetFilePath(resKey);
            if (string.IsNullOrEmpty(filePath))
            {
                DebugUtil.LogWarning("缺少资源: " + resKey);
                callback?.Invoke(false);
                return;
            }

            _Download(filePath, callback);
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }


    private void _Download(string filePath, Action<bool> callback)
    {
        if (File.Exists(Path.Combine(FilePathTools.persistentDataPath_Platform, filePath)))
        {
            callback?.Invoke(true);
            return;
        }

        if (!_dicDownTime.ContainsKey(filePath))
        {
            _dicDownTime.Add(filePath, DateTime.Now);
        }
        else
        {
            _dicDownTime[filePath] = DateTime.Now;
        }

        string fileUrl = Path.Combine(_httpsHeadPath, Path.ChangeExtension(filePath, ".txt"));
        CustomDownloadSubsystem.Instance.Download(fileUrl, (success, hr) =>
        {
            if (success)
            {
                var md5 = hr.DataAsText.Trim();
                DownResources(filePath, md5, callback);
            }
            else
            {
                callback?.Invoke(false);
            }
        });
    }


    static List<string> _allDynResPath = new List<string>()
    {
        FilePathTools.persistentDataPath_Platform + "/ui/remotedyn",
        FilePathTools.persistentDataPath_Platform + "/scene/remotedyn"
    };

    public void ClearDownloadDir()
    {
        try
        {
            for (int i = 0; i < _allDynResPath.Count; i++)
            {
                if (Directory.Exists(_allDynResPath[i]))
                {
                    Directory.Delete(_allDynResPath[i], true);
                }
            }

            _allRes.Clear();
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    private void DownResources(string filePath, string md5, Action<bool> callback)
    {
        try
        {
            DownloadInfo info2 = DownloadManager.Instance.DownloadInSeconds(PathManager.dynSubPath,
                filePath, md5, info =>
                {
                    DownloadResult result = info.result;
                    if (result == DownloadResult.Success)
                    {
                        DateTime startTime = new DateTime();
                        if (_dicDownTime.TryGetValue(filePath, out startTime))
                        {
                            ulong userTime = (ulong) (DateTime.Now - startTime).TotalSeconds;
                            _dicDownTime.Remove(filePath);
                        }

                        callback?.Invoke(true);
                    }
                    else
                    {
                        Debug.Log("下载失败");
                        callback?.Invoke(false);
                    }
                });
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    private T _LoadAsset<T>(string bundleFilePath) where T : Object
    {
        try
        {
            AssetBundle targetAb = _LoadAssetBundle(bundleFilePath);
            var asset = targetAb.LoadAsset<T>(Path.GetFileNameWithoutExtension(bundleFilePath));
            CoroutineManager.Instance.StartCoroutine(_CoUnloadAB(targetAb));
            return asset;
        }
        catch (Exception e)
        {
            DebugUtil.LogError($"{GetType()}._LoadAsset error : bundle file name = {bundleFilePath}");
            DebugUtil.LogError(e);
        }

        return null;
    }

    private IEnumerator _CoUnloadAB(AssetBundle ab)
    {
        if (ab)
        {
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            ab.Unload(false);
        }
    }

    private Sprite _LoadJpgAsset(string bundleFilePath)
    {
        try
        {
            byte[]
                decryptBytes =
                    null; //AssetUtils.RijndaelDecryptFile(FilePathTools.persistentDataPath_Platform + "/" + bundleFilePath);
            return _LoadSpriteFromByte(bundleFilePath, decryptBytes);
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return null;
    }

    private AssetData _BuildAssetData(string resKey, Object asset)
    {
        AssetData assetData;

        if (_allRes.TryGetValue(resKey, out assetData))
        {
            assetData.resKey = resKey;
            assetData.asset = asset;
        }
        else
        {
            assetData = new AssetData()
            {
                resKey = resKey,
                asset = asset,
            };
            _allRes.Add(resKey, assetData);
        }

        return assetData;
    }

    private AssetBundle _LoadAssetBundle(string bundleFilePath)
    {
        AssetBundle targetAb = null;
        try
        {
            var fullPath = Path.Combine(FilePathTools.persistentDataPath_Platform, bundleFilePath);
            ResourcesManager.Instance.LoadAssetBundleAsync(fullPath, loader => targetAb = loader.AssetBundle);
            byte[] decryptBytes = null; // AssetUtils.RijndaelDecryptFile(fullPath);
            targetAb = AssetBundle.LoadFromMemory(decryptBytes);
        }
        catch (Exception e)
        {
            DebugUtil.LogError($"LoadAssetBundle failed : ${bundleFilePath}");
            DebugUtil.LogError(e);
        }

        return targetAb;
    }

    private Sprite _LoadSpriteFromByte(string name, byte[] data)
    {
        Sprite sprite = null;
        try
        {
            Texture2D texture = Texture2DFactory.Instance.CreateTexture(name);
            //byte[] dfdfdf = GetBytesFromLocal(UnityEngine.Application.dataPath + "/Export/UI/RemoteDyn/Pic/Textures/3.jpg");
            texture.LoadImage(data);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f));

            return sprite;
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return sprite;
    }


    private string _GetFilePath(string fileName)
    {
        var key = fileName?.ToLower();
        ResProp value = null;
        if (!string.IsNullOrEmpty(fileName) && GlobalData.g_pathFile.DicPathFile.TryGetValue(key, out value))
        {
            return value.path;
        }

        return string.Empty;
    }

    public void CleanUnusedAsset(uint saveCount = 0)
    {
        try
        {
            if (_allRes != null)
            {
                _toBeCleaned.Clear();
                var e = _allRes.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Value.refCount <= 0)
                    {
                        _toBeCleaned.Add(e.Current.Value);
                    }
                }

                var removeCount = _toBeCleaned.Count - saveCount;
                if (removeCount > 0)
                {
                    var eClean = _toBeCleaned.GetEnumerator();
                    while (eClean.MoveNext() && removeCount-- > 0)
                    {
                        AssetData assetData;
                        if (_allRes.TryGetValue(eClean.Current.resKey, out assetData))
                        {
                            _ReleaseAssetData(assetData);
                        }

                        _allRes.Remove(eClean.Current.resKey);
                        //DebugUtil.Log($"asset cleared, key : {eClean.Current.resKey}");
                    }

                    _toBeCleaned.Clear();
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                }
            }
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e.ToString());
        }
    }

    public void LoadDummyMaterialAssetBundle()
    {
        string filePath = _GetFilePath(PathManager.DummyMaterialBundleName);
        if (string.IsNullOrEmpty(filePath))
        {
            DebugUtil.LogWarning("缺少资源: " + PathManager.DummyMaterialBundleName);
        }

        var dummyMatAB = _LoadAssetBundle(filePath);
    }

    private IEnumerator _CoTimerCleanUnusedAsset()
    {
        while (true)
        {
            yield return new WaitForSeconds(cleanInterval);
            CleanUnusedAsset(savedAssetOnClean);
        }
    }

    private void _ReleaseAssetData(AssetData assetData)
    {
        if (assetData != null)
        {
            assetData.asset = null;
        }
    }


    /// <summary>
    /// 下载路径文件
    /// </summary>
    /// <returns>The path file.</returns>
    public void DownPathFile(float start, float end, Action<bool> callback)
    {
        // bool isPathFileDowned = false;
        string filePath = PathManager.dynPathFile;
        string fullFilePath = Path.Combine(FilePathTools.persistentDataPath_Platform, filePath);
        string fullFileMd5Path = Path.ChangeExtension(fullFilePath, ".txt");
        string md5Url = Path.Combine(_httpsHeadPath, Path.ChangeExtension(filePath, ".txt"));

        string localMd5 = "";
        if (File.Exists(fullFilePath))
        {
            localMd5 = AssetUtils.BuildFileMd5(fullFilePath);
        }

        CustomDownloadSubsystem.Instance.Download(md5Url, (success, hr) =>
        {
            if (success)
            {
                var md5 = hr.DataAsText.Trim();
                if (localMd5 != md5)
                {
                    DownResources(filePath, md5, delegate(bool b) { callback?.Invoke(b && _LocalLoadPathFile()); });
                }
                else
                {
                    callback?.Invoke(_LocalLoadPathFile());
                }
            }
            else
            {
                callback?.Invoke(false);
            }
        });
    }

    private bool _LocalLoadPathFile()
    {
        try
        {
            string filePath = PathManager.dynPathFile;
            string fullFilePath = Path.Combine(FilePathTools.persistentDataPath_Platform, filePath);
            if (File.Exists(fullFilePath))
            {
                var sr = File.OpenText(fullFilePath);
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.NullValueHandling = NullValueHandling.Ignore;
                JsonConvert.PopulateObject(sr.ReadToEnd(), GlobalData.g_pathFile.DicPathFile, setting);
                sr.Close();
                sr.Dispose();
                DebugUtil.Log("end load pathfile");
                return true;
            }
        }
        catch (Exception e)
        {
            DebugUtil.Log(e);
        }

        return false;
    }
}


public class ByLastUsingTime : IComparer<AssetData>
{
    public int Compare(AssetData x, AssetData y)
    {
        return x.lastUsingTime > y.lastUsingTime ? 1 : -1;
    }
}