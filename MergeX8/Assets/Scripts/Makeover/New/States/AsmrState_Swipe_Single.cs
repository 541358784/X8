using DragonU3DSDK;
using Spine.Unity;
using UnityEngine;

namespace MiniGame
{
    public class AsmrState_Swipe_Single : AsmrState_ProcessBase
    {
        private AsmrInputHandler_Swipe_Single _inputHandler;
        private float _totalDistance;
        private Vector3 _hintHandPos;
        private string _animName = "right_left";

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _totalDistance = Screen.width * _asmrParam.RuningStep.Config.requireValue;
            _inputHandler = new AsmrInputHandler_Swipe_Single();
            
            if(_totalDistance == 0) DebugUtil.LogError("_totalDistance 不可为0");

            _hintHandPos = new Vector3(0, -5f, 0);
            if (_asmrParam.RuningStep.Config.tipPos != null && _asmrParam.RuningStep.Config.tipPos.Length == 2)
            {
                _hintHandPos.x = _asmrParam.RuningStep.Config.tipPos[0];
                _hintHandPos.y = _asmrParam.RuningStep.Config.tipPos[1];
            }


            _asmrParam.Level.PlayHandAnim(_animName, _hintHandPos, true, AsmrHintType.None, true);

            _asmrParam.Level.ShowProgressBar(true);
        }

        public override void OnExit()
        {
            base.OnExit();

            _inputHandler = null;

            _asmrParam.Level.ShowProgressBar(false);
        }

        public override void Update()
        {
            base.Update();

            if (_progress >= 0.9f)
            {
                _progress += 0.001f;
                ShowTip(false);
            }
            else
            {
                _inputHandler.Update();
                _progress = _inputHandler.totalDistance / _totalDistance;
                ShowTip(AsmrInputHandler_Base.TouchEnd);
            }

            _progress = Mathf.Min(1, _progress);
            
            CheckAudio();

            _asmrParam.Level.UpdateProgress(_progress);

            foreach (var levelSpinePlayer in _spinePlayers) levelSpinePlayer.Play(_progress);

            if (_progress >= 1f)
            {
                // Debug.LogError("滑动距离满足");

                var allFinish = true;
                foreach (var levelSpinePlayer in _spinePlayers)
                {
                    if (!levelSpinePlayer.Finish)
                    {
                        allFinish = false;
                        break;
                    }
                }

                if (allFinish)
                {
                    // Debug.LogError($"SpinePlayer Finish");

                    if (!AsmrInputHandler_Base.TouchEnd) return;

                    ShowTip(false);
                    _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.id);
                }
            }
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            _inputHandler.FixedUpdate(deltaTime);
        }

        private void ShowTip(bool show)
        {
            ShowTargets(show);
            _asmrParam.Level.ShowGuideHand(show, _hintHandPos, AsmrHintType.None);

            if (_progress >= 0.001f)
            {
                var sp = _targetColliders[0].GetComponent<Renderer>();
                if (sp) sp.enabled = false;
            }
        }
    }
}