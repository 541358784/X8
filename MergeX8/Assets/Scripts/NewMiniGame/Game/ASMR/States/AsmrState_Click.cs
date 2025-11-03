using UnityEngine;

namespace fsm_new
{
    public class AsmrState_Click : AsmrState_Base
    {
        protected AsmrInputHandler_Click _inputHandler;

        private string _animName = "loop";

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _inputHandler = new AsmrInputHandler_Click(_asmrParam.Level.camera, OnPointClick, s => ASMR.ASMRModel.Instance.PlaySound(s));

            if (_targetColliders == null || _targetColliders.Count <= 0) return;

            if (_asmrParam.RuningStep.Config.TipType == 2)
            {
                ShowAllTargetCircles(false);
                _asmrParam.Level.PlayHandAnim(_animName, _targetColliders[0].transform.position, true, AsmrHintType.Loop, false, _asmrParam.RuningStep.Config.TipSize);
            }
            else if (_asmrParam.RuningStep.Config.TipType == 1)
            {
                ShowAllTargetCircles(true);
            }
        }

        private void ShowAllTargetCircles(bool show)
        {
            foreach (var targetCollider in _targetColliders)
            {
                var sp = targetCollider.GetComponent<Renderer>();
                if (sp) sp.enabled = show;
            }
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
        }

        private void OnTargetClicked(int objHashCode)
        {
            foreach (var collider2D in _targetColliders)
            {
                if (objHashCode != collider2D.gameObject.GetHashCode()) continue;
                if (!_targetStepIdMap.TryGetValue(objHashCode, out var step)) continue;

                ShowTip(false);

                HandleClick(step);

                collider2D.gameObject.SetActive(false);
                ShakeManager.Instance.ShakeLight();

                break;
            }
        }

        protected virtual void HandleClick(AsmrStep step)
        {
            switch (string.IsNullOrEmpty(step.Config.AnimatorName))
            {
                case false:
                    _asmrParam.fsm.ChangeState<AsmrState_PlayAnimator>(
                        new AsmrState_PlayAnimatorParam(
                            _asmrParam.Level,
                            _asmrParam.RuningStep,
                            _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.AnimatorPath).GetComponent<Animator>(),
                            _asmrParam.RuningStep.Config.AnimatorName,
                            () =>
                            {
                                if (step.Config.TransitionAnimationType == 1)
                                {
                                    _asmrParam.fsm.ChangeState<AsmrState_Transition>(new AsmrStateParamBase(_asmrParam.Level, step, _asmrParam.fsm));
                                }
                                else
                                {
                                    _asmrParam.Level.CurrentGroup.FinishStep(step.Config.Id);
                                }
                            },
                            _asmrParam.fsm
                        )
                    );
                    break;
                default:
                {
                    if (step.Config.SpineAnimName != null && step.Config.SpineAnimName.Count > 0)
                    {
                        _asmrParam.fsm.ChangeState<AsmrState_PlaySpine>(new AsmrStateParamBase(_asmrParam.Level, step, _asmrParam.fsm));
                    }
                    else
                    {
                        _asmrParam.Level.CurrentGroup.FinishStep(step.Config.Id);
                    }

                    break;
                }
            }
        }

        public void ShowTip(bool show)
        {
            _asmrParam.Level.ShowGuideHand(show, _targetColliders[0].transform.position, AsmrHintType.Loop, _asmrParam.RuningStep.Config.TipSize);
        }

        private void OnPointClick(Vector3 position)
        {
            CheckCollider(position);
        }

        private void CheckCollider(Vector2 position)
        {
            var boxCollider2D = _targetColliders[0];
            if (!_targetColliders[0].OverlapPoint(position)) return;

            //Debug.Log("点击到碰撞体: " + boxCollider2D.name);
            OnTargetClicked(boxCollider2D.gameObject.GetHashCode());
        }
    }
}