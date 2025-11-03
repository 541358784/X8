using System;
using System.Collections.Generic;
using ASMR;
using Framework.Utils;

namespace fsm_new
{
    public partial class AsmrLevel
    {
        private bool _isFinish;

        private void InitGroups()
        {
            _asmrGroups = new List<AsmrGroup>();

            for (var i = 0; i < Config.GroupIds.Count; i++)
            {
                var groupId = Config.GroupIds[i];
                var config = ASMRModel.Instance.AsmrGroupConfigs.Find(c => c.Id == groupId);

                _asmrGroups.Add(new AsmrGroup(config, this));
            }

            var finished = CurrentGroup.Steps.FindAll(c => c.Finish);
            EventBus.Send(new EventAsmrStepChange(_currentGroupIndex, finished.Count, CurrentGroup.Steps.Count, _asmrGroups.Count));
        }

        public void ToNextGroup()
        {
            if (_currentGroupIndex < _asmrGroups.Count - 1)
            {
                _currentGroupIndex++;

                ToNextStep();
                
                EventBus.Send(new EventAsmrGroupChange(_currentGroupIndex, _asmrGroups.Count));
            }
            else
            {
                Finish();
                EventBus.Send(new EventAsmrGroupChange(_currentGroupIndex + 1, _asmrGroups.Count));
            }
        }

        private void Finish()
        {
            _isFinish = true;
            if (!string.IsNullOrEmpty(Config.VideoName))
            {
                _fsm.ChangeState<AsmrState_Win>(new AsmrStateParamBase(this, _fsm));
            }
            else
            {
                _fsm.ChangeState<AsmrState_Finish>(new AsmrStateParamBase(this, _fsm));
            }

            ASMR.ASMRModel.Instance.FinishedLevel();
        }

        public void ToNextStep()
        {
            var asmrState = _fsm.CurrentState as AsmrState_Base;
            if (asmrState != null)
            {
                HideNode_Exit(asmrState._asmrParam.RuningStep);
                //IsShowChangeAnimation(asmrState._asmrParam.RuningStep);
                List<string> ignoreTools = null;
                if (asmrState._asmrParam.RuningStep != null)
                {
                    ignoreTools = asmrState._asmrParam.RuningStep.Config.ToolsKeepPos;
                }

                ResetToolsPos(ignoreTools);
            }

            var finished = CurrentGroup.Steps.FindAll(c => c.Finish);
            EventBus.Send(new EventAsmrStepChange(_currentGroupIndex, finished.Count, CurrentGroup.Steps.Count, _asmrGroups.Count));

            var nextStep = CurrentGroup.Steps.Find(c => !c.Finish);

            if (nextStep.Config.TransitionAnimationType == 2)
            {
                _fsm.ChangeState<AsmrState_TransitionOver>(new AsmrStateParamBase(this, nextStep, _fsm));
                return;
            }
            switch ((AsmrTypes)nextStep.Config.ActionType)
            {
                case AsmrTypes.Click:
                    if (!string.IsNullOrEmpty(Config.VideoName)) _fsm.ChangeState<AsmrState_VideoPlayer>(new AsmrStateParamBase(this, nextStep, _fsm));
                    else _fsm.ChangeState<AsmrState_Click>(new AsmrStateParamBase(this, nextStep, _fsm));
                    break;
                case AsmrTypes.Drag:
                    _fsm.ChangeState<AsmrState_Drag>(new AsmrStateParamBase(this, nextStep, _fsm));
                    break;
                case AsmrTypes.Swipe_Single:
                    _fsm.ChangeState<AsmrState_Swipe_Single>(new AsmrStateParamBase(this, nextStep, _fsm));
                    break;
                case AsmrTypes.Swipe_Double:
                    _fsm.ChangeState<AsmrState_Swipe_Double>(new AsmrStateParamBase(this, nextStep, _fsm));
                    break;
                case AsmrTypes.LongPress_Double:
                    _fsm.ChangeState<AsmrState_LongPress_Double>(new AsmrStateParamBase(this, nextStep, _fsm));
                    break;
                case AsmrTypes.Erase:
                    _fsm.ChangeState<AsmrState_Erase>(new AsmrStateParamBase(this, nextStep, _fsm));
                    break;
                case AsmrTypes.Paint:
                    _fsm.ChangeState<AsmrState_Fill>(new AsmrStateParamBase(this, nextStep, _fsm));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdateProgress(float value)
        {
             var ui = Framework.UI.UIManager.Instance.GetView<UIAsmr>();
             if (ui != null)
             {
                 ui.OnProgressChanged(value);
             }
        }

        public void ShowProgressBar(bool show)
        {
             var ui = Framework.UI.UIManager.Instance.GetView<UIAsmr>();
             if (ui != null)
             {
                 ui.ShowProgressBar(show);
                 ui.OnProgressChanged(0f);
             }
        }

        public int GetCurrentCompltete()
        {
            // 补丁 ：_asmrGroups.Count 为1的时候，进游戏直接退出，获取到的进度为100
            if (_asmrGroups.Count == 1 && !_isFinish)
            {
                return 0;
            }

            var percent = (_currentGroupIndex + 1) / (float)_asmrGroups.Count;
            var completeText = ((int)(percent * 100));
            return completeText;
        }

        public void FastFinishCurrentLevel()
        {
            _fsm.ChangeState<AsmrState_Win>(new AsmrStateParamBase(this, _fsm));
        }
    }
}