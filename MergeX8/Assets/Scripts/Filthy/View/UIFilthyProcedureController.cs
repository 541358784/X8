using System;
using System.Collections.Generic;
using DragonPlus.Config.Filthy;
using DragonPlus.UI;
using Filthy.Game;
using Filthy.Model;
using Filthy.Procedure;
using Filthy.SubFsm;
using Screw.GameLogic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Filthy.View
{
    public class UIFilthyProcedureController: MonoBehaviour
    {
        private Canvas _canvas;

        private string[] buttonNames = new[]
        {
            "HelpButton",
            "Step_1",
            "Step_2",
            "Step_3",
        };

        private VideoPlayer _videoObject;
        private void Awake()
        {
            transform.Find("Root/screwMask").gameObject.SetActive(false);
            transform.Find("Root/screwScale/screwbg").gameObject.SetActive(false);
            transform.Find("Root/screwScale/screwNode").gameObject.SetActive(false);
            _canvas = gameObject.GetOrCreateComponent<Canvas>();
            _canvas.overrideSorting = true;
            gameObject.GetOrCreateComponent<GraphicRaycaster>();
            
            _canvas.sortingOrder = 2;
            
            var skipButton = transform.Find("Root/SkipButton");
            skipButton.gameObject.SetActive(false);
            CommonUtils.NotchAdapte(skipButton);
            
            foreach (var buttonName in buttonNames)
            {
                var button = transform.Find($"Root/{buttonName}").GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    OnEnterButton(buttonName);
                });
                button.gameObject.SetActive(false);
            }
            
            _videoObject = transform.Find("Root/Video").GetComponent<VideoPlayer>();
            ClearRenderTextureToBlack();
        }

        private void OnDisable()
        {
            ClearRenderTextureToBlack();
        }

        private void Start()
        {
        }

        private void OnDestroy()
        {
        }

        private void OnEnterButton(string name)
        {
            FilthyGameLogic.Instance.TriggerProcedure(TriggerType.Click, name);
        }
        
        private void ClearRenderTextureToBlack()
        {
            if(_videoObject == null || _videoObject.targetTexture == null)
                return;
            
            RenderTexture currentActiveRT = RenderTexture.active;

            RenderTexture.active = _videoObject.targetTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = currentActiveRT;
        }
        
        
        public void ShowScrewView()
        {
            if (ScreViewVisible)
                return;
            ScreViewVisible = true;
            _videoObject.Pause();
            transform.Find("Root/screwMask").gameObject.SetActive(true);
            var screwBg = transform.Find("Root/screwScale/screwbg");
            var screwNode = transform.Find("Root/screwScale/screwNode");
            var screwImage = transform.Find("Root/screwScale/screwNode/screwCameraImage").GetComponent<RawImage>();
            var scaleNode = transform.Find("Root/screwScale");
            screwBg.gameObject.SetActive(true);
            screwNode.gameObject.SetActive(true);
            var size = (_canvas.transform as RectTransform).GetHeight();
            var scale = 1f;
            if (size >= 1365)
                scale = 1.5f;
            scaleNode.transform.localScale = new Vector3(scale, scale, 1f);
            screwImage.rectTransform.SetSizeHeight(size);
            screwImage.rectTransform.SetSizeWidth(size);
            var cameraTexture = new RenderTexture((int)screwImage.rectTransform.rect.width,
                (int)screwImage.rectTransform.rect.height, GraphicsFormat.R8G8B8A8_UNorm,
                GraphicsFormat.D24_UNorm_S8_UInt);
            screwImage.texture = cameraTexture;
            var mainCamera = Camera.main;
            CopyCamera = Instantiate(mainCamera.transform, mainCamera.transform.parent).GetComponent<Camera>();
            CopyCamera.targetTexture = cameraTexture;
            var cameraScale = size / 1024f;
            mainCamera.orthographicSize *= cameraScale/scale;
            CopyCamera.orthographicSize *= cameraScale;
            var raycaster = CopyCamera.gameObject.GetComponent<Physics2DRaycaster>();
            if (raycaster)
            {
                DestroyImmediate(raycaster);
            }
        }

        public bool ScreViewVisible = false;
        public Camera CopyCamera;

        public void HideScrewView()
        {
            if (!ScreViewVisible)
                return;
            ScreViewVisible = false;
            _videoObject.Play();
            transform.Find("Root/screwMask").gameObject.SetActive(false);
            var screwBg = transform.Find("Root/screwScale/screwbg");
            var screwNode = transform.Find("Root/screwScale/screwNode");
            var screwImage = transform.Find("Root/screwScale/screwNode/screwCameraImage").GetComponent<RawImage>();
            screwBg.gameObject.SetActive(false);
            screwNode.gameObject.SetActive(false);
            DestroyImmediate(CopyCamera.gameObject);
            screwImage.texture = null;
        }

        public void SetVideoPause(bool isPause)
        {
            if(isPause)
                _videoObject.Pause();
            else
            {
                _videoObject.Play();
            }
        }
    }
}