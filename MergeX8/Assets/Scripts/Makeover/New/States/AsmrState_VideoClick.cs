using DragonU3DSDK;
using MiniGame;
using UnityEngine;

namespace asmr_new
{
    public class AsmrState_VideoClick : AsmrState_ProcessBase
    {
        private const string AnimName = "loop";
        private AsmrInputHandler_Click _inputHandler;
        private bool _isCanClick;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _asmrParam.Level.VideoHandler.Play();
            _inputHandler = new AsmrInputHandler_Click(_asmrParam.Level.camera, OnPointClick);

            if (_targetColliders != null)
                foreach (var targetCollider in _targetColliders)
                {
                    var sp = targetCollider.GetComponent<Renderer>();
                    if (sp) sp.enabled = false;
                }
            
            PlayAudio();
        }

        public override void OnExit()
        {
            base.OnExit();

            _inputHandler = null;
        }

        public override void Update()
        {
            base.Update();

            _inputHandler.Update();

            if (_asmrParam.Level.VideoHandler.IsEnded())
            {
                _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.id);
                return;
            }

            if (!_isCanClick && _asmrParam.Level.VideoHandler.Time >= _asmrParam.RuningStep.Config.requireValue)
            {
                _isCanClick = true;
                _asmrParam.Level.VideoHandler.Pause();
                var target = _targetColliders[0];
                target.GetComponent<Renderer>().enabled = true;
                _asmrParam.Level.PlayHandAnim(AnimName, _targetColliders[0].transform.position, true, AsmrHintType.Loop,
                    true);
                DebugUtil.Log(
                    $"Video time: {_asmrParam.Level.VideoHandler.Time}, Limit time: {_asmrParam.RuningStep.Config.requireValue}");
            }
        }

        private void OnTargetClicked(int objHashCode)
        {
            foreach (var collider2D in _targetColliders)
            {
                if (objHashCode == collider2D.gameObject.GetHashCode())
                {
                    if (_targetStepIdMap.TryGetValue(objHashCode, out var step))
                    {
                        ShowTip(false);
                        _asmrParam.Level.CurrentGroup.FinishStep(step.Config.id);
                        collider2D.gameObject.SetActive(false);
                        AsmrLevel.VibrationShort();
                        break;
                    }
                }
            }
        }

        public void ShowTip(bool show)
        {
            _asmrParam.Level.ShowGuideHand(show, _targetColliders[0].transform.position, AsmrHintType.Loop);
        }

        private void OnPointClick(Vector3 position)
        {
            if (!_isCanClick)
                return;

            CheckCollider(position);
        }

        private void CheckCollider(Vector2 position)
        {
            var boxCollider2D = _targetColliders[0];
            if (_targetColliders[0].OverlapPoint(position))
            {
                //Debug.Log("点击到碰撞体: " + boxCollider2D.name);
                OnTargetClicked(boxCollider2D.gameObject.GetHashCode());
            }
        }
    }
}