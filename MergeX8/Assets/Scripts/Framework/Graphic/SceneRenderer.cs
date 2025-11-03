using System;
using System.Collections;
using DragonU3DSDK;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class SceneRenderer : IDisposable
    {
        private Camera _camera;
        private Coroutine _coRender;
        private RenderTexture _rt;
        private RawImage _image;

        public SceneRenderer(string name, Camera camera, RawImage image)
        {
            try
            {
                _camera = camera;
                _image = image;
                var rect = image.rectTransform.rect;
                //DebugUtil.LogError("SceneRenderer create image rect = {0}", rect.ToString());
                _rt = RenderTextureFactory.Instance.Create(name, (int) rect.width, (int) rect.height, 24);
                image.texture = _rt;
                _camera.targetTexture = _rt;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }


        public void Render(float time)
        {
            try
            {
                _camera.enabled = true;
                _camera.Render();

                if (_coRender != null)
                {
                    CoroutineManager.Instance.StopCoroutine(_coRender);
                }

                _coRender = CoroutineManager.Instance.StartCoroutine(CoDisableCamera(time));
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        private IEnumerator CoDisableCamera(float time)
        {
            yield return new WaitForSeconds(time);
            _camera.enabled = false;
            _coRender = null;
        }

        public void Dispose()
        {
            try
            {
                _camera.targetTexture = null;
                _image.texture = null;
                if (_rt != null)
                {
                    RenderTextureFactory.Instance.Destroy(_rt);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }
    }


    public class SceneRenderer2 : IDisposable
    {
        private Camera _camera;
        private Coroutine _coRender;
        private CameraTextureVisitor _ctv;
        private RawImage _image;

        public SceneRenderer2(string name, Camera camera, RawImage image)
        {
            try
            {
                _camera = camera;
                _image = image;
                _ctv = _camera.GetComponent<CameraTextureVisitor>();
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }


        public void Render(float time)
        {
            try
            {
                _camera.enabled = true;
                _image.texture = null;
                _image.color = Color.clear;
                _image.material.SetFloat("_ColorMask", 15);

                if (_coRender != null)
                {
                    CoroutineManager.Instance.StopCoroutine(_coRender);
                }

                _coRender = CoroutineManager.Instance.StartCoroutine(CoDisableCamera(time));
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        private IEnumerator CoDisableCamera(float time)
        {
            yield return new WaitForSeconds(time);
            yield return new WaitForEndOfFrame();

            try
            {
                // var size = RoomView.Instance.GetViewportRectSize();
                // if (_ctv.RT == null)
                // {
                //     _ctv.RT = RenderTextureFactory.Instance.Create("SceneRenderer2", (int) size.x, (int) size.y, 0);
                //     _ctv.RT.antiAliasing = 1;
                // }

                _ctv.CopyRTOnce = true;
            }
            catch (Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e);
            }

            yield return new WaitForEndOfFrame();
            try
            {
                _image.texture = _ctv.RT;
                _image.color = Color.white;
                _image.material.SetFloat("_ColorMask", 14);
                _camera.enabled = false;
                _coRender = null;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        public void Dispose()
        {
            _camera.enabled = false;
            if (_ctv.RT)
            {
                RenderTexture.Destroy(_ctv.RT);
                _ctv.RT = null;
            }

            _ctv = null;
            if (_coRender != null)
            {
                CoroutineManager.Instance.StopCoroutine(_coRender);
                _coRender = null;
            }
        }
    }
}