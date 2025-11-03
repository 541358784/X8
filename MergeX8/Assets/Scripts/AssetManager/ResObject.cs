using System;
using DragonU3DSDK;
using Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResObject : IDisposable
{
    private AssetData _assetData;
    private Action<Object> _onReady;

    private bool _disposed = false;

    // private bool _downloaded = false;
    private Object _instance;

    public ResObject(Action<Object> onReady)
    {
        _onReady = onReady;
    }

    public bool Disposed => _disposed;

    public void Load(AssetData assetData)
    {
        if (assetData != null && !_disposed)
        {
            if (_assetData != null)
            {
                DebugUtil.LogError($"A {GetType()} load asset twice.");
                return;
            }

            if (!(assetData.asset is Sprite))
            {
                _instance = Object.Instantiate(assetData.asset);
            }

            _assetData = assetData;
            _assetData.refCount++;
            //DebugUtil.Log($"{_assetData.resKey} ref count : {_assetData.refCount}");
            _onReady?.Invoke(GetInstance());
        }
    }

    public Object GetInstance()
    {
        return _instance ?? _assetData?.asset;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_instance)
            {
                Object.Destroy(_instance);
                _instance = null;
            }

            if (_assetData != null)
            {
                var sprite = _assetData.asset as Sprite;
                if (sprite != null)
                {
                    Texture2DFactory.Instance.DestroyTexture(sprite.texture);
                }

                _assetData.refCount--;
                //DebugUtil.Log($"{_assetData.resKey} ref count : {_assetData.refCount}");
                if (_assetData.refCount <= 0)
                {
                    _assetData.lastUsingTime = Time.time;
                    _assetData.refCount = 0;
                    //DebugUtil.Log($"{_assetData.resKey} ref count : {_assetData.refCount}, last use time : {_assetData.lastUsingTime}");
                }
            }
        }

        _instance = null;
        _onReady = null;
        _assetData = null;
        _disposed = true;
    }
}