using System.Collections.Generic;
using Deco.World;
using Decoration;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StoryMovie
{
    public class Action_Move : ActionBase
    {
        private Tweener _tweener;
        
        protected override void Init()
        {
        }

        protected override void Start()
        {
            if(_movieObject == null)
                return;
            
            InitPosition();
            PlayAnimation();
            
            if(_movePosition == null || _movePosition.Count == 0)
                return;

            List<Vector3> movePosition = new List<Vector3>();
            movePosition.Add(_movieObject.transform.position);
            movePosition.AddRange(_movePosition);
            _tweener = _movieObject.transform.DOPath(movePosition.ToArray(), _config.movieTime).OnComplete(() =>
            {
                _tweener = null;
            }).SetEase((Ease)_config.easeType);
            
            if(_config.controlType == 3)
                DecoManager.Instance.CurrentWorld.PinchMap.UpdateTargetPosition(_movePosition[_movePosition.Count-1]);
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
            ParamToVector3DList();
        }
    }
}