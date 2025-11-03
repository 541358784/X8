using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniGame
{
    public partial class AsmrLevel
    {
        private void InitTips(GameObject root)
        {
            _guideHandRoot = root.transform.Find("GuideHand");
            if (_guideHandRoot != null)
            {
                _guideHandSpine = _guideHandRoot.Find("GuideHand").GetComponent<SkeletonAnimation>();
                _guideHandSpine.autoUpdate = true;

                _guideHandRoot.gameObject.SetActive(false);
            }

            _doubleHandRoot = root.transform.Find("DoubleHand");
            if (_doubleHandRoot != null) _doubleHandRoot.gameObject.SetActive(false);
        }

        public async void PlayDragTip(Vector3 startPos, Vector3 targetPos, string animName)
        {
            DOTween.KillAll();
            _guideHandRoot.position = new Vector3(startPos.x, startPos.y, 0);

            PlayHandAnim(animName, startPos, false, AsmrHintType.None, true);

            var track = _guideHandSpine.AnimationState.GetCurrent(0);
            var animTime = 0f;
            if (track != null) animTime = track.Animation.Duration;

            await Task.Delay((int)(animTime * 1000));

            if (_guideHandRoot == null) return;
            var sequence = DOTween.Sequence();
            sequence.Append(_guideHandRoot.transform.DOMove(targetPos, 1f));
            sequence.AppendInterval(0.5f);
            sequence.SetLoops(-1);
        }

        public void StopDragTip()
        {
            DOTween.KillAll();
            ShowGuideHand(false, Vector2.zero, AsmrHintType.None);
        }

        public bool TipShowing()
        {
            return _guideHandRoot.gameObject.activeSelf;
        }

        public void PlayHandAnim(string animName, Vector3 pos, bool loop, AsmrHintType hintType, bool useHand)
        {
            if (string.IsNullOrEmpty(animName)) return;

            if (useHand)
            {
                _guideHandSpine.AnimationState.SetAnimation(0, animName, loop);
                _guideHandSpine.gameObject.SetActive(true);
            }
            else
            {
                _guideHandSpine.gameObject.SetActive(false);
                DOTween.KillAll();
            }

            ShowGuideHand(true, pos, hintType);
        }


        public void ShowGuideHand(bool show, Vector2 pos, AsmrHintType hintType)
        {
            DOTween.KillAll();
            _guideHandRoot.position = new Vector3(pos.x, pos.y, 0);

            if (_guideHandRoot.gameObject.activeSelf == show) return;

            _guideHandRoot.gameObject.SetActive(show);

            _guideHandRoot.Find("vfx_hint").gameObject.SetActive(hintType == AsmrHintType.Once);
            _guideHandRoot.Find("vfx_batter").gameObject.SetActive(hintType == AsmrHintType.Loop);

            if (show)
            {
                var trackEntry = _guideHandSpine.AnimationState.GetCurrent(0);
                if (trackEntry != null)
                {
                    trackEntry.TrackTime = _guideHandSpine.AnimationState.GetCurrent(0).AnimationStart;
                }
            }
        }

        public void ShowDoubleHand(bool show)
        {
            _doubleHandRoot.gameObject.SetActive(show);
        }
    }
}