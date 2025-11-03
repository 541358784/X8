using DG.Tweening;
using IFix.Core;
using TMPro;
using UnityEngine;
using Decoration;

namespace StoryMovie
{
    public class Action_CameraScale : ActionBase
    {
        private Tweener _tweener;
        private float _scale = 14;
        protected override void Init()
        {
        }

        protected override void Start()
        {
            if(_movieObject == null)
                return;

            float oldValue = DecoSceneRoot.Instance.mSceneCamera.orthographicSize;

            _tweener = DOTween.To(() => oldValue, x => oldValue = x, _scale, _config.movieTime).OnUpdate(() =>
            {
                DecoSceneRoot.Instance.mSceneCamera.orthographicSize = oldValue;
                _tweener = null;
            });
        }

        protected override void Stop()
        {
            if(_tweener == null)
                return;
            
            _tweener.Kill(false);
            _tweener = null;
        }

        protected override void ParsingParam()
        {
            float.TryParse(_config.actionParam, out _scale);
        }
    }
}