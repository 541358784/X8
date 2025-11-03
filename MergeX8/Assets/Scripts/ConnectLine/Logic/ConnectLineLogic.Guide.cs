using ConnectLine.Model;
using DG.Tweening;
using UnityEngine;

namespace ConnectLine.Logic
{
    public partial class ConnectLineLogic
    {
        private GameObject _guideRoot = null;
        private Line _guideLine = null;
        private Tween _guideTween;
        private bool _isGuide = false;
        private int _levelId = 0;
        public void InitGuide(GameObject guideRoot, int levelId)
        {
            _levelId = levelId;
            _guideRoot = guideRoot;
            _guideRoot.gameObject.SetActive(false);
        }

        private void CheckGuide()
        {
            if (_levelId == 1 && !ConnectLineModel.Instance.IsFinish(_levelId))
            {
                BeginGuide();
            } 
        }
        private void BeginGuide()
        {
            StopGuide();
            
            _isGuide = true;
            
            SetPipeActive(true);
            
            if (_guideTween != null)
            {
                _guideTween.Kill();
                _guideTween = null;
            }
            
            var startPipe = GetPipeByType(PipeType.WaterInlet);
            var endPipe = GetPipeByType(PipeType.Connect);

            _guideLine = PathLineManager.Instance.Spawn();
            _guideLine.SetLineModel(Line.LineModel.Black);

            Vector3 position = startPipe.GetFirstPortPosition();
            _guideRoot.transform.position = position;
            _guideLine.SetLineRenderPosition(position);
            
            _guideTween = DOTween.To(() => position, x => position = x, endPipe.GetFirstPortPosition(), 2f).OnUpdate(() =>
            {
                _guideLine.SetLineRenderPosition(position);
                _guideRoot.transform.position = position;
            }).OnComplete(() =>
            {
                _guideTween = null;
                _guideLine.SetFree(true);
                BeginGuide();
            });
            
            _guideRoot.gameObject.SetActive(true);
        }

        private void StopGuide()
        {
            if(!_isGuide)
                return;

            _isGuide = false;
            
            if (_guideTween != null)
            {
                _guideTween.Kill();
                _guideTween = null;
            }

            if (_guideLine != null)
            {
                _guideLine.SetFree(true);
                _guideLine = null;
            }
            
            _guideRoot.gameObject.SetActive(false);
            SetPipeActive(false);
        }

        public Pipe GetPipeByType(PipeType type)
        {
            return _pipes.Find(a => a.GetPipeType() == type);
        }
    }
}