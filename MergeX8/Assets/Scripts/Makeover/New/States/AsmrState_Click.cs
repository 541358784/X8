using System.Collections.Generic;
using DragonU3DSDK.Audio;
using UnityEngine;

namespace MiniGame
{
    public class AsmrState_Click : AsmrState_Base
    {
        private AsmrInputHandler_Click _inputHandler;
        private string _animName = "loop";

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _inputHandler = new AsmrInputHandler_Click(_asmrParam.Level.camera, OnPointClick);

            if (_targetColliders != null)
                foreach (var targetCollider in _targetColliders)
                {
                    var sp = targetCollider.GetComponent<Renderer>();
                    if (sp) sp.enabled = false;
                }

            _asmrParam.Level.PlayHandAnim(_animName, _targetColliders[0].transform.position, true, AsmrHintType.Loop, false);
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
                if (objHashCode == collider2D.gameObject.GetHashCode())
                {
                    if (_targetStepIdMap.TryGetValue(objHashCode, out var step))
                    {
                        ShowTip(false);

                        if (!string.IsNullOrEmpty(step.Config.animatorName))
                        {
                            Fsm.ChangeState<AsmrState_PlayAnimator>(
                                new AsmrState_PlayAnimatorParam(
                                    _asmrParam.Level,
                                    _asmrParam.RuningStep,
                                    _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.animatorPath).GetComponent<Animator>(),
                                    _asmrParam.RuningStep.Config.animatorName,
                                    () => _asmrParam.Level.CurrentGroup.FinishStep(step.Config.id)
                                )
                            );
                        }
                        else if (step.Config.spineAnimName != null && step.Config.spineAnimName.Length > 0)
                        {
                            Fsm.ChangeState<AsmrState_PlaySpine>(new AsmrStateParamBase(_asmrParam.Level, step));
                        }
                        else
                        {
                            _asmrParam.Level.CurrentGroup.FinishStep(step.Config.id);
                        }
                        
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