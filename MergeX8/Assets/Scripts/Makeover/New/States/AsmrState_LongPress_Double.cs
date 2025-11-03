using DragonU3DSDK;
using UnityEngine;

namespace MiniGame
{
    public class AsmrState_LongPress_Double : AsmrState_ProcessBase
    {
        private AsmrInputHandler_LongPress _inputHandler;

        private float _totalTouchTime;

        private Vector3 _hintHandPos;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _totalTouchTime = _asmrParam.RuningStep.Config.requireValue;
            _inputHandler = new AsmrInputHandler_LongPress();

            if(_totalTouchTime == 0) DebugUtil.LogError("_totalTouchTime 不可为0");

            
            _hintHandPos = new Vector3(0, -5f, 0);
            if (_asmrParam.RuningStep.Config.tipPos != null && _asmrParam.RuningStep.Config.tipPos.Length == 2)
            {
                _hintHandPos.x = _asmrParam.RuningStep.Config.tipPos[0];
                _hintHandPos.y = _asmrParam.RuningStep.Config.tipPos[1];
            }


            // asmrParam.Level.PlayHandAnim(asmrParam.RuningStep.Config.HandTipAnim, _hintHandPos, true, AsmrHintType.Loop, true);
            
            _asmrParam.Level.ShowDoubleHand(true);

            _asmrParam.Level.ShowProgressBar(true);
        }

        public override void OnExit()
        {
            base.OnExit();

            _inputHandler = null;
            _asmrParam.Level.ShowProgressBar(false);
        }

        private float _progress;

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
                _progress = _inputHandler.totalTimer / _totalTouchTime;
                ShowTip(AsmrInputHandler_Base.TouchEnd);
            }

            _progress = Mathf.Min(1, _progress);

            CheckAudio();
            _asmrParam.Level.UpdateProgress(_progress);

            foreach (var levelSpinePlayer in _spinePlayers) levelSpinePlayer.Play(_progress);

            if (_progress >= 1f)
            {
                //Debug.LogError("双指按动时间满足");
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
                    //Debug.LogError($"SpinePlayer Finish");

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
            // asmrParam.Level.ShowGuideHand(show, _hintHandPos, AsmrHintType.Loop);
            _asmrParam.Level.ShowDoubleHand(show);
        }
    }
}