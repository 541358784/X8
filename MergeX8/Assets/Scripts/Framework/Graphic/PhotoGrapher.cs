using System;
using DragonU3DSDK;
using UnityEngine;

public class PhotoGrapher
{
    private Camera _camera;
    private Texture2D _image;

    public PhotoGrapher(Camera camera)
    {
        _camera = camera;
        _image = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height, TextureFormat.RGB24, false);
    }


    public byte[] CamCapture()
    {
        try
        {
            var old = RenderTexture.active;
            var oldActive = _camera.gameObject.activeSelf;
            RenderTexture.active = _camera.targetTexture;
            _camera.gameObject.SetActive(true);
            _camera.Render();
            _image.ReadPixels(new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height), 0, 0);
            _image.Apply();
            RenderTexture.active = old;
            _camera.gameObject.SetActive(oldActive);

            return _image.EncodeToJPG();
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return null;
    }
}