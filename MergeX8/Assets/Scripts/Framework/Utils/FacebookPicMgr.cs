using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BestHTTP;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.Networking;

public class FacebookPicMgr : Manager<FacebookPicMgr>
{
    private Dictionary<string, Texture2D> mFacebookPicCacheDict = new Dictionary<string, Texture2D>();
    private string fbAvatarUrl = "https://graph.facebook.com/{0}/picture?width=84&height=84&type=normal";

    public void GetFacebookPic(string fbId, Action<string, Texture2D> callback)
    {
        Init();
        if (string.IsNullOrEmpty(fbId) || fbId == "0")
        {
            callback?.Invoke(fbId, null);
            return;
        }

        if (mFacebookPicCacheDict.ContainsKey(fbId))
        {
            if (callback != null)
                callback(fbId, mFacebookPicCacheDict[fbId]);
        }
        else
        {
            StartCoroutine(LoadLocalImage(fbId, callback));
            StartCoroutine(GetPicture(fbId, callback));
        }
    }

    IEnumerator GetPicture(string fbId, Action<string, Texture2D> callback)
    {
        string uri = string.Format(fbAvatarUrl, fbId);
        using (HTTPRequest request = new HTTPRequest(new Uri(uri), HTTPMethods.Get, (req, rep) =>
        {
            if (rep == null)
            {
                DragonU3DSDK.DebugUtil.LogError("GetPicture() Response null {0}", uri);
            }
            else if (rep.StatusCode >= 200 && rep.StatusCode < 300)
            {
                //DragonU3DSDK.DebugUtil.LogError("GetPicture() Success. StatusCode={0}, DataAsTexture2D={1}, DataAsText={2}, Data={3} ", rep.StatusCode, rep.DataAsTexture2D, rep.DataAsText, rep.Data);

                if (rep.DataAsTexture2D != null)
                {
                    Texture2D picTexture = rep.DataAsTexture2D;
                    SaveLocalImage(picTexture, fbId);
                    if (mFacebookPicCacheDict.ContainsKey(fbId))
                        mFacebookPicCacheDict[fbId] = picTexture;
                    else
                        mFacebookPicCacheDict.Add(fbId, picTexture);

                    try
                    {
                        callback?.Invoke(fbId, mFacebookPicCacheDict[fbId]);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            else
            {
                DragonU3DSDK.DebugUtil.LogError("GetPicture() Unexpected response from server, status = {0}",
                    rep.StatusCode);
            }
        })
        {
            DisableCache = true,
            IsCookiesEnabled = false,
            ConnectTimeout = TimeSpan.FromSeconds(5),
            Timeout = TimeSpan.FromSeconds(20)
        })
        {
            request.Send();
            yield return StartCoroutine(request);
        }
    }

    private void Init()
    {
        if (Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
            return;

        Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
    }

    private string path
    {
        get
        {
            //pc,ios //android :jar:file//
            return Application.persistentDataPath + "/ImageCache/";
        }
    }

    private void SaveLocalImage(Texture2D tex2d, string fbId)
    {
        string filePath = path + fbId.GetHashCode();
        if (File.Exists(filePath))
            File.Delete(filePath);

        byte[] pngData = tex2d.EncodeToPNG();
        File.WriteAllBytes(filePath, pngData);

        //Debug.LogError("SaveImage " + filePath);
    }

    IEnumerator LoadLocalImage(string fbId, Action<string, Texture2D> callback)
    {
        //Debug.LogError("LoadLocalImage1 " + fbId);
        if (callback == null)
            yield break;

        string filePath = "file:///" + path + fbId.GetHashCode();

        //Debug.LogError("LoadLocalImage " + filePath);
        WWW www = new WWW(filePath);
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("downloading new image:" + fbId + "\t" + www.error);
            yield break;
        }

        //Debug.LogError("LoadLocalImage Success " + filePath);
        callback.Invoke(fbId, www.texture);
    }
}