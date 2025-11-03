using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASMR;
using asmr_new;
using Newtonsoft.Json;
using DG.Tweening;
using DragonPlus.Config.Makeover;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Serialization;

namespace MiniGame
{
    public partial class AsmrLevel
    {
        private void InitGroups()
        {
            _asmrGroups = new List<AsmrGroup>();

            for (var i = 0; i < Config.groupIds.Length; i++)
            {
                var groupId = Config.groupIds[i];
                var config = MakeoverConfigManager.Instance.groupList.Find(c => c.id == groupId);

                _asmrGroups.Add(new AsmrGroup(config, this));
            }
            
            var finished = CurrentGroup.Steps.FindAll(c => c.Finish);
            UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            controller?.SetToNextStep(_currentGroupIndex);
            // var uiLevel = UIViewSystem.Instance.Get<UILevel>();
            // if (uiLevel != null) uiLevel.SetCellsProgress(_currentGroupIndex, finished.Count, CurrentGroup.Steps.Count);
        }

        public void ToNextGroup()
        {
            var percent = (_currentGroupIndex + 1) / (float)_asmrGroups.Count;
            var completeText = ((int)(percent * 100));

            UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            controller?.SetToNextStep(_currentGroupIndex);
            //UIViewSystem.Instance.Get<UILevel>().SetGroupProgressUI(_currentGroupIndex, completeText);


            if (_currentGroupIndex < _asmrGroups.Count - 1)
            {
                _currentGroupIndex++;

                ToNextStep();
            }
            else
            {
                Finish();
            }
        }

        private void Finish()
        {
            //_fsm.ChangeState<AsmrState_Finish>(new AsmrStateParamBase(this));
            _fsm.ChangeState<AsmrState_Win>(new AsmrStateParamBase(this));
            
            Model.Instance.FinishedLevel();
            
            UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            controller?.SetAllStepFinish();
        }

        public void ToNextStep()
        {
            var asmrState = _fsm.CurrentState as AsmrState_Base;
            if (asmrState != null)
            {
                HideNode_Exit(asmrState._asmrParam.RuningStep);
            }

            var finished = CurrentGroup.Steps.FindAll(c => c.Finish);

            UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            controller?.SetToNextStep(_currentGroupIndex);
            
            //var nextStep = CurrentGroup.Steps.Find(c => !c.Finish);
            var nextStep = CurrentGroup.GetNextStep();
            
            switch ((AsmrActionType)nextStep.Config.actionType)
            {
                case AsmrActionType.Click:
                    _fsm.ChangeState<AsmrState_Click>(new AsmrStateParamBase(this, nextStep));
                    break;
                case AsmrActionType.Drag:
                    _fsm.ChangeState<AsmrState_Drag>(new AsmrStateParamBase(this, nextStep));
                    break;
                case AsmrActionType.Swipe_Single:
                    _fsm.ChangeState<AsmrState_Swipe_Single>(new AsmrStateParamBase(this, nextStep));
                    break;
                case AsmrActionType.Swipe_Double:
                    _fsm.ChangeState<AsmrState_Swipe_Double>(new AsmrStateParamBase(this, nextStep));
                    break;
                case AsmrActionType.LongPress_Double:
                    _fsm.ChangeState<AsmrState_LongPress_Double>(new AsmrStateParamBase(this, nextStep));
                    break;
                case AsmrActionType.VideoClick:
                    _fsm.ChangeState<AsmrState_VideoClick>(new AsmrStateParamBase(this, nextStep));
                    break;
                case AsmrActionType.VideoFinish:
                    _fsm.ChangeState<AsmrState_VideoFinish>(new AsmrStateParamBase(this, nextStep));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (asmrState != null)
            {
                List<string> ignoreTools = null;
                if (asmrState._asmrParam.RuningStep != null)
                {
                    if(asmrState._asmrParam.RuningStep.Config.toolsKeepPos != null && asmrState._asmrParam.RuningStep.Config.toolsKeepPos.Length > 0)
                        ignoreTools = new List<string>(asmrState._asmrParam.RuningStep.Config.toolsKeepPos);
                }

                ResetToolsPos(ignoreTools);
            }
        }

        public void UpdateProgress(float value)
        {
            UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            controller?.OnProgressChanged(value);
        }

        public void ShowProgressBar(bool show)
        {
            UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            controller?.ShowProgressBar(show);
            controller?.OnProgressChanged(0f);
        }
    }
}