using System;
using DG.Tweening;
using DragonU3DSDK;
using ScratchCardAsset;
using ScratchCardAsset.Core;
using UnityEngine;

namespace fsm_new
{
    public class AsmrState_ScratchCard_Base : AsmrState_Base
    {
        protected ScratchCardManager _cardManager;

        private AsmrInputHandler_Drag _inputHandler_Drag;

        private string _animName = "loop3";
        private Vector3 _hintHandPos;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _inputHandler_Drag = new AsmrInputHandler_Drag(_asmrParam.Level.camera);
            _inputHandler_Drag.SetClickCallback(OnPointClick);

            _cardManager = _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.Target).GetComponent<ScratchCardManager>();
            _cardManager.InputEnabled = false;

            _cardManager.Progress.OnProgress += OnScratchProgress;


            _hintHandPos = new Vector3(0, -5f, 0);
            if (_asmrParam.RuningStep.Config.TipPos != null && _asmrParam.RuningStep.Config.TipPos.Count == 2)
            {
                _hintHandPos.x = _asmrParam.RuningStep.Config.TipPos[0];
                _hintHandPos.y = _asmrParam.RuningStep.Config.TipPos[1];
            }

            _asmrParam.Level.PlayHandAnim(_animName, _hintHandPos, true, AsmrHintType.None, true, _asmrParam.RuningStep.Config.TipSize);
        }

        private bool _scratchFinish;

        private void OnScratchProgress(float progress)
        {
            switch (_cardManager.Card.Mode)
            {
                case ScratchMode.Erase:
                {
                    if (progress >= 0.8f)
                    {
                        _scratchFinish = true;
                        ShowTip(false);

                        _cardManager.Progress.OnProgress -= OnScratchProgress;

                        _cardManager.SpriteRendererCard.DOFade(0, 0.5f).onComplete =
                            () => _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.Id);

                        DebugUtil.Log($"User scratched {Math.Round(progress * 100f, 2)}% of surface");
                    }

                    break;
                }
                default:
                {
                    if (progress <= 0.2f)
                    {
                        _scratchFinish = true;
                        ShowTip(false);

                        _cardManager.Progress.OnProgress -= OnScratchProgress;

                        _cardManager.ClearScratchCard();
                        _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.Id);

                        DebugUtil.Log($"User scratched {Math.Round(progress * 100f, 2)}% of surface");
                    }

                    break;
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (_enterAnimFinish)
            {
                _inputHandler_Drag?.Update();

                if (!_scratchFinish) ShowTip(AsmrInputHandler_Base.TouchEnd);
            }
            else
            {
                ShowTip(false);
            }
        }

        private void OnPointClick(Vector3 position)
        {
            _cardManager.InputEnabled = true;

            if (_asmrParam.RuningStep.Config.Hide_when_start_erase != null)
            {
                foreach (var s in _asmrParam.RuningStep.Config.Hide_when_start_erase)
                {
                    var t = _asmrParam.Level.transform.Find(s);
                    if (t) t.gameObject.SetActive(false);
                }
            }

            var pos = position;
            var anchor = _toolTransform.Find("collision");
            if (anchor)
            {
                var childWorldPos = anchor.position;
                pos = pos - childWorldPos + anchor.parent.position;
            }

            pos.z = _toolTransform.position.z;

            _toolTransform.position = pos;
        }

        private void ShowTip(bool show)
        {
            ShowTargets(show);
            _asmrParam.Level.ShowGuideHand(show, _hintHandPos, AsmrHintType.None, _asmrParam.RuningStep.Config.TipSize);

            if (_targetColliders == null || _targetColliders.Count <= 0) return;

            var sp = _targetColliders[0].GetComponent<Renderer>();
            if (sp) sp.enabled = false;
        }
    }
}