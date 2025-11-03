
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PhotoGraphBuilder
{
    public enum ImageFormat
    {
        JPG,
        TGA,
        PNG,
    }
    
    private Camera _camera;
    private Texture2D _image;
    private ImageFormat _format;
    private TextureFormat _tFormat;
    private RenderTexture _rt;
    
    public PhotoGraphBuilder(Camera camera, ImageFormat format)
    {
        _camera = camera;
        SetImageFormat(format);
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
        ResetImage();
    }

    public void SetImageFormat(ImageFormat format)
    {
        var oldTFormat = _tFormat;
        
        _format = format;

        switch (_format)
        {
            case ImageFormat.JPG:
                _tFormat = TextureFormat.RGB24;
                break;
            case ImageFormat.PNG:
            case ImageFormat.TGA:
                _tFormat = TextureFormat.RGBA32;
                break;
            default:
                _tFormat = TextureFormat.RGB24;
                break;
        }

        ResetImage();
    }

    public byte[] CamCapture()
    {
        try
        {
            CamCapture2Texture();
            switch (_format)
            {
                case ImageFormat.JPG:
                    return _image.EncodeToJPG();
                case ImageFormat.PNG:
                    return _image.EncodeToPNG();
                case ImageFormat.TGA:
                    return _image.EncodeToTGA();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return null;
    }
    
    
    public Texture2D CamCapture2Texture()
    {
        try
        {
            object lockThis = new object();
            lock (lockThis)
            {
                var old = RenderTexture.active;
                var oldActive = _camera.gameObject.activeSelf;
                RenderTexture.active = _camera.targetTexture;
                _camera.gameObject.SetActive(true);
                _camera.Render();

                GetTextureSize(out var width, out var height);
                Debug.Log($"Camera Capture: width = {width}, height = {height},  image size ({_image.width}, {_image.height})");
                _image.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                _image.Apply();
            
                RenderTexture.active = old;
                _camera.gameObject.SetActive(oldActive);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return _image;
    }
    
    private void ResetImage()
    {
        GetTextureSize(out var width, out var height);
        Debug.Log($"{GetType()}.ResetImage width = {width}, height = {height}");
        _image = new Texture2D(width, height, _tFormat, false);
    }


    private void GetTextureSize(out int width, out int height)
    {
        if (_camera.targetTexture != null)
        {
            width = _camera.targetTexture.width;
            height = _camera.targetTexture.height;
            Debug.Log($"{GetType()}.GetTextureSize 1111111111111");
        }
        else
        {
#if UNITY_EDITOR
            var gameViewSize = Handles.GetMainGameViewSize();
            width = (int)gameViewSize.x;
            height = (int) gameViewSize.y;
            Debug.Log($"{GetType()}.GetTextureSize 2222222222222");
#else
            width = Screen.width;
            height = Screen.height;
            Debug.Log($"{GetType()}.GetTextureSize 3333333333333333");
#endif
        }
    }
}