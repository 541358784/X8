using System;
using System.Collections.Generic;
using System.IO;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using UnityEngine;
using File = System.IO.File;

public class OnlineImgManager
{
    private readonly Dictionary<string, string> _onlineImgs = new Dictionary<string, string>(32);
    private Dictionary<string, DateTime> _dicDownTime = new Dictionary<string, DateTime>(); //BI 记录下载资源所用时间


    public void LoadImage(string url, Action<bool, Texture2D> callback, bool forceUpdateCache)
    {
        try
        {
            if (!forceUpdateCache)
            {
                _LocalLoad(url, delegate(bool b, Texture2D d)
                {
                    if (b)
                    {
                        callback(true, d);
                    }
                    else
                    {
                        DownloadImage(url, delegate(bool downSuccess)
                        {
                            if (downSuccess)
                            {
                                _LocalLoad(url, callback);
                            }
                            else
                            {
                                callback?.Invoke(false, null);
                            }
                        });
                    }
                });
            }
            else
            {
                DownloadImage(url, delegate(bool b)
                {
                    if (b)
                    {
                        _LocalLoad(url, callback);
                    }
                    else
                    {
                        callback?.Invoke(false, null);
                    }
                });
            }
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    private void _LocalLoad(string url, Action<bool, Texture2D> callback)
    {
        string localPath;
        if (_onlineImgs.TryGetValue(url, out localPath) && File.Exists(localPath))
        {
            var data = File.ReadAllBytes(localPath);
            var tex = Texture2DFactory.Instance.CreateTexture(url);
            tex.LoadImage(data);
            callback?.Invoke(true, tex);
        }
        else
        {
            callback?.Invoke(false, null);
        }
    }

    public void LoadImage(string url, Action<bool, Sprite> callback, bool forceUpdateCache)
    {
        try
        {
            LoadImage(url, delegate(bool succ, Texture2D texture2D)
            {
                if (succ)
                {
                    Sprite sprite = _CreateSprite(texture2D);
                    callback?.Invoke(true, sprite);
                }
                else
                {
                    callback?.Invoke(false, null);
                }
            }, forceUpdateCache);
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    public void DownloadImage(string url, Action<bool> callback)
    {
        try
        {
            if (!_dicDownTime.ContainsKey(url))
            {
                _dicDownTime.Add(url, DateTime.Now);
            }
            else
            {
                _dicDownTime[url] = DateTime.Now;
            }

            CustomDownloadSubsystem.Instance.Download(url, (success, hr) =>
            {
                if (success)
                {
                    DateTime startTime = new DateTime();
                    if (_dicDownTime.TryGetValue(url, out startTime))
                    {
                        ulong userTime = (ulong) (DateTime.Now - startTime).TotalSeconds;
                    }

                    var cachePath = Path.Combine(FilePathTools.persistentDataPath_Platform, "room_img");
                    if (!Directory.Exists(cachePath))
                    {
                        Directory.CreateDirectory(cachePath);
                    }

                    var cacheName = Path.Combine(cachePath, Path.GetFileName(url));
                    if (_onlineImgs.ContainsKey(url))
                    {
                        _onlineImgs.Remove(url);
                    }

                    _onlineImgs.Add(url, cacheName);
                    File.WriteAllBytes(cacheName, hr.Data);
                    DebugUtil.Log($"file saved, url = {url}, file = {cacheName}");
                    callback?.Invoke(true);
                }
                else
                {
                    callback?.Invoke(false);
                }
            });
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }


    private Sprite _CreateSprite(Texture2D tex2D)
    {
        Sprite sprite = null;
        try
        {
            if (tex2D)
            {
                sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0f, 0f));
                sprite.name = tex2D.name;
                return sprite;
            }
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return sprite;
    }

    private static OnlineImgManager _instance;

    public static OnlineImgManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new OnlineImgManager();
            }

            return _instance;
        }
    }
}