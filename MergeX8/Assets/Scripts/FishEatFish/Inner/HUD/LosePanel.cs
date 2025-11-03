using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FishEatFishSpace
{
    public class LosePanel : MonoBehaviour
    {
        [SerializeField] Transform scale;
        [SerializeField] RawImage rawImage;
        [SerializeField] Camera fishCamera;
        public Action OnEnableMove;

        void Awake()
        {
            RenderTexture targetTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.Default);
            fishCamera.targetTexture = targetTexture;
            rawImage.texture = targetTexture;
        }

        public void Lose()
        {
            scale.localScale = Vector3.one * 0f;
            gameObject.SetActive(true);
            scale.DOScale(Vector3.one, 0.25f).SetEase(Ease.InCirc);
        }

        public void OnClosePanel()
        {
            // scale.DOKill();
            gameObject.SetActive(false);
            OnEnableMove?.Invoke();
            OnEnableMove = null;
        }
    }
}