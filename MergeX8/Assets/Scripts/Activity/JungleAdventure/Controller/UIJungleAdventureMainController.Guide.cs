using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Activity.JungleAdventure.Controller
{
    public partial class UIJungleAdventureMainController
    {
        private bool _isGuideing = false;
        
        private void Awake_Guide()
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_playButton.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JungleAdventurePreview, transform as RectTransform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JungleAdventureDes, transform as RectTransform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JungleAdventurePlay, _playButton.transform as RectTransform, topLayer:topLayer);
        }

        private void TriggerGuide()
        {
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JungleAdventurePreview, null))
            {
                GuidePreView();
            }
            else
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JungleAdventureDes, null);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JungleAdventurePlay, null);
            }
        }

        private async UniTask GuidePreView()
        {
            _playButton.gameObject.SetActive(false);
            _isGuideing = true;
            Vector2 startPosition = _pathMap._points.Last().points.Last();
            startPosition = LimitPosition(startPosition);
            LimitCameraPosition(startPosition);

            await UniTask.WaitForSeconds(1f);
            
            Vector2 endPosition = _pathMap._points.First().points.First();
            endPosition = LimitPosition(endPosition);

            await DOTween.To(() => startPosition, x => startPosition = x, endPosition, 6f).SetEase(Ease.Linear).OnUpdate(() =>
            {
                LimitCameraPosition(startPosition);
            });

            _isGuideing = false;
            _playButton.gameObject.SetActive(true);
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.JungleAdventurePreview);
        }
    }
}