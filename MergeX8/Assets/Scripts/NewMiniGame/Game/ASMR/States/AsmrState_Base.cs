using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace fsm_new
{
    public class AsmrStateParamBase : FsmStateParamBase
    {
        public AsmrLevel Level;
        public AsmrStep RuningStep;
        public AsmrFsm<AsmrState_Base> fsm;

        public AsmrStateParamBase(AsmrLevel level, AsmrFsm<AsmrState_Base> fsm)
        {
            Level = level;
            this.fsm = fsm;
        }

        public AsmrStateParamBase(AsmrLevel level, AsmrStep step, AsmrFsm<AsmrState_Base> fsm)
        {
            Level = level;
            RuningStep = step;
            this.fsm = fsm;
        }
    }

    public class AsmrState_Base : FsmStateBase
    {
        public AsmrStateParamBase _asmrParam;

        protected Collider2D _toolCollider;
        protected Transform _toolTransform;
        protected List<Collider2D> _targetColliders;
        protected Dictionary<int, AsmrStep> _targetStepIdMap;
        protected List<asmr_new.SpinePlayer> _spinePlayers;
        protected List<asmr_new.SpinePlayer> _enterPlayers;

        protected bool _toolAttached; //工具是否已挂载到对应的spine骨骼上

        private Vector3 _cameraInitPos;
        private Vector3 _cameraTargePos;
        private float _cameraInitSize;

        protected bool _enterAnimFinish = true;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            _asmrParam = stateParam as AsmrStateParamBase;

            base.OnEnter(stateParam);

            InitTargetAndDragable();

            InitSpinePlayers();

            ShowTool_Enter();

            if (_asmrParam != null)
            {
                _cameraInitPos = _asmrParam.Level.camera.transform.position;
                _cameraInitSize = _asmrParam.Level.camera.orthographicSize;

                if (_asmrParam.RuningStep != null)
                {
                    // if (_asmrParam.RuningStep.Config.AutoAttachTool) AttackTool();

                    if (_asmrParam.RuningStep.Config.HidePaths_Enter != null) HideWhenEnter();
                    if (_asmrParam.RuningStep.Config.ShowPaths_Enter != null) ShowWhenEnter();

                    if (_asmrParam.RuningStep.Config.CameraPos != null && _asmrParam.RuningStep.Config.CameraPos.Count == 2)
                    {
                        _cameraTargePos = new Vector3(_asmrParam.RuningStep.Config.CameraPos[0], _asmrParam.RuningStep.Config.CameraPos[1], _cameraInitPos.z);
                    }
                }
            }

            InitEnterSpinePlayers();
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_spinePlayers != null)
                foreach (var spinePlayer in _spinePlayers)
                    spinePlayer.ClearEvent();

            if (_enterPlayers != null)
                foreach (var enterPlayer in _enterPlayers)
                    enterPlayer.ClearEvent();

            ResetTool();
        }


        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            if (_spinePlayers != null)
                foreach (var spinePlayer in _spinePlayers)
                    spinePlayer.FixedUpdate(Time.deltaTime);

            //入场动画
            if (!_enterAnimFinish && _enterPlayers != null)
            {
                _enterAnimFinish = true;
                foreach (var enterPlayer in _enterPlayers)
                {
                    enterPlayer.FixedUpdate(Time.deltaTime);

                    if (!enterPlayer.Finish) _enterAnimFinish = false;
                }

                if (_enterAnimFinish)
                {
                    if (_asmrParam.RuningStep.Config.ShowPaths_EnterPlaySpineFinish != null)
                    {
                        foreach (var s in _asmrParam.RuningStep.Config.ShowPaths_EnterPlaySpineFinish)
                        {
                            _asmrParam.Level.transform.Find(s)?.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (_asmrParam.RuningStep.Config.HidePaths_EnterPlaySpineBegin != null)
                    {
                        foreach (var s in _asmrParam.RuningStep.Config.HidePaths_EnterPlaySpineBegin)
                        {
                            _asmrParam.Level.transform.Find(s)?.gameObject.SetActive(false);
                        }
                    }
                }
            }

            //相机移动
            if (_asmrParam == null || _asmrParam.RuningStep == null) return;
            var t = _asmrParam.ElapsedTime / 0.5f;
            if (_asmrParam.RuningStep.Config.CameraPos != null && _asmrParam.RuningStep.Config.CameraPos.Count > 0)
            {
                _asmrParam.Level.camera.transform.position = Vector3.Lerp(_cameraInitPos, _cameraTargePos, t);
            }

            if (_asmrParam.RuningStep.Config.CameraSize != 0 && Math.Abs(_asmrParam.RuningStep.Config.CameraSize - _asmrParam.Level.camera.orthographicSize) > 0.001)
            {
                _asmrParam.Level.camera.orthographicSize = Mathf.Lerp(_cameraInitSize, _asmrParam.RuningStep.Config.CameraSize, t);
            }
        }

        public override string ToString()
        {
            if (_asmrParam != null)
            {
                if (_asmrParam.RuningStep != null)
                {
                    return $"{GetType()} StepID:{_asmrParam.RuningStep.Config.Id}";
                }
            }

            return base.ToString();
        }

        private void ShowTool_Enter()
        {
            if (_asmrParam.RuningStep == null) return;
            if (string.IsNullOrEmpty(_asmrParam.RuningStep.Config.ToolPath)) return;

            var node = _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.ToolPath);
            node.gameObject.SetActive(true);

            UpAllToolNodeLayer(node);
        }

        protected void UpAllToolsLayer()
        {
            if (_asmrParam.RuningStep == null) return;
            if (string.IsNullOrEmpty(_asmrParam.RuningStep.Config.ToolPath)) return;

            var node = _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.ToolPath);
            UpAllToolNodeLayer(node);
        }

        private void UpAllToolNodeLayer(Transform node)
        {
            if (!node) return;

            var renders = node.GetComponentsInChildren<Renderer>();
            if (renders != null)
            {
                foreach (var renderer in renders)
                {
                    renderer.sortingOrder = renderer.sortingOrder + 100;
                }
            }
        }

        protected void ResetAllToolsLayer()
        {
            if (_asmrParam.RuningStep == null) return;
            if (string.IsNullOrEmpty(_asmrParam.RuningStep.Config.ToolPath)) return;

            var node = _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.ToolPath);
            if (!node) return;

            var renders = node.GetComponentsInChildren<Renderer>();
            if (renders == null) return;
            foreach (var renderer in renders)
            {
                if (!_asmrParam.Level.ToolsLayerDic.TryGetValue(renderer, out var layer)) continue;

                renderer.sortingOrder = layer;
            }
        }

        private string InitTarget(AsmrStep step)
        {
            var toolPath = string.Empty;

            if (step == null) return toolPath;

            if (!string.IsNullOrEmpty(step.Config.ToolPath)) toolPath = step.Config.ToolPath;

            if (step.Config.Target == null) return toolPath;

            var target = _asmrParam.Level.transform.Find(step.Config.Target);
            if (target)
            {
                var collider2D = target.GetComponent<Collider2D>();
                if (collider2D != null)
                {
                    collider2D.enabled = true;
                    collider2D.gameObject.SetActive(true);
                    if (!_targetColliders.Contains(collider2D)) _targetColliders.Add(collider2D);
                    if (!_targetStepIdMap.ContainsKey(collider2D.gameObject.GetHashCode())) _targetStepIdMap.Add(collider2D.gameObject.GetHashCode(), step);
                }
                else
                {
                    Debug.Log($"Target Path 未挂载碰撞体:{step.Config.Target}");
                }
            }
            else
            {
                Debug.LogError($"Target Path 未找到:{step.Config.Target}");
            }


            return toolPath;
        }

        private void InitTargetAndDragable()
        {
            _targetStepIdMap = new Dictionary<int, AsmrStep>();
            _targetColliders = new List<Collider2D>();

            var toolPath = string.Empty;

            var curSteps = _asmrParam.Level.CurrentGroup.GetCurSteps(_asmrParam.RuningStep);
            foreach (var step in curSteps)
            {
                toolPath = InitTarget(step);
            }

            if (_asmrParam.RuningStep != null) toolPath = _asmrParam.RuningStep.Config.ToolPath;
            if (string.IsNullOrEmpty(toolPath)) return;
            _toolTransform = _asmrParam.Level.transform.Find(toolPath);

            if (_toolTransform == null)
            {
                Debug.LogError($"dragToolPath:{toolPath}  not found");
                return;
            }

            var interCollider = _toolTransform.Find("collision");
            if (interCollider)
                _toolCollider = interCollider.GetComponent<Collider2D>();
            else
                _toolCollider = _toolTransform.GetComponent<Collider2D>();

            if (_toolCollider != null)
            {
                _toolCollider.enabled = true;
            }
            else
            {
                if (_asmrParam.RuningStep != null && _asmrParam.RuningStep.Config.ActionType == (int)AsmrTypes.Drag)
                {
                    Debug.LogError($"dragToolPath:{toolPath}  has no collider2d");
                }
            }
        }

        private void InitSpinePlayers()
        {
            if (_asmrParam.RuningStep == null) return;
            if (_asmrParam.RuningStep.Config.SpinePath == null) return;

            _spinePlayers = new List<asmr_new.SpinePlayer>();
            for (var index = 0; index < _asmrParam.RuningStep.Config.SpinePath.Count; index++)
            {
                var path = _asmrParam.RuningStep.Config.SpinePath[index];
                var spineTransform = _asmrParam.Level.transform.Find(path);
                if (spineTransform == null)
                {
                    Debug.LogError($"Spine Path没有找到{path}");
                    continue;
                }

                var spine = spineTransform.GetComponent<SkeletonAnimation>();
                var player = new asmr_new.SpinePlayer(spine, _asmrParam.RuningStep.Config.SpineAnimName[index], !_asmrParam.RuningStep.Config.DontUpdateFirstSpineFrame);
                _spinePlayers.Add(player);

                _enterAnimFinish = false;
            }
        }

        private void InitEnterSpinePlayers()
        {
            if (_asmrParam.RuningStep == null) return;
            if (_asmrParam.RuningStep.Config.PlaySpineAnim_Enter == null) return;

            _enterPlayers = new List<asmr_new.SpinePlayer>();

            for (var index = 0; index < _asmrParam.RuningStep.Config.PlaySpineAnim_Enter.Count; index++)
            {
                var path = _asmrParam.RuningStep.Config.PlaySpineAnim_Enter[index];
                var animName = _asmrParam.RuningStep.Config.PlaySpineAnimName_Enter[index];
                var spineTransform = _asmrParam.Level.transform.Find(path);
                if (spineTransform == null)
                {
                    Debug.LogError($"Spine Path没有找到{path}");
                    continue;
                }

                var spine = spineTransform.GetComponent<SkeletonAnimation>();
                var player = new asmr_new.SpinePlayer(spine, animName, !_asmrParam.RuningStep.Config.DontUpdateFirstSpineFrame);
                player.Play(animName);
                _enterPlayers.Add(player);
            }
        }

        protected void AttackTool()
        {
            if (_asmrParam.RuningStep == null) return;

            ResetAllToolsLayer();

            // if (!string.IsNullOrEmpty(_asmrParam.RuningStep.Config.ToolAttachPath))
            // {
            //     var toolParent = _asmrParam.Level.transform.Find($"{_asmrParam.RuningStep.Config.ToolAttachPath}");
            //     _toolTransform.SetParent(toolParent);
            //     _toolTransform.localPosition = Vector3.zero;
            //     if (_asmrParam.RuningStep.Config.ToolResetRotation) _toolTransform.localRotation = Quaternion.identity;
            //     if (_asmrParam.RuningStep.Config.ToolResetScale) _toolTransform.localScale = Vector3.one;
            //
            //     _toolAttached = true;
            // }
        }

        protected void ResetTool()
        {
            if (_asmrParam.RuningStep == null) return;

            // if (!string.IsNullOrEmpty(_asmrParam.RuningStep.Config.ToolAttachPath))
            // {
            //     _toolTransform.SetParent(_asmrParam.Level.transform);
            //     _toolAttached = false;
            //     _toolTransform.gameObject.SetActive(false);
            // }
        }

        protected void ShowTargets(bool show)
        {
            if (_targetColliders == null) return;

            foreach (var targetCollider in _targetColliders) targetCollider.gameObject.SetActive(show);
        }

        private void HideWhenEnter()
        {
            if (_asmrParam == null || _asmrParam.RuningStep == null) return;
            if (_asmrParam.RuningStep.Config.HidePaths_Enter == null) return;

            for (var i = 0; i < _asmrParam.RuningStep.Config.HidePaths_Enter.Count; i++)
            {
                var path = _asmrParam.RuningStep.Config.HidePaths_Enter[i];
                var obj = _asmrParam.Level.transform.Find(path);
                if (obj) obj.gameObject.SetActive(false);
            }
        }

        private void ShowWhenEnter()
        {
            if (_asmrParam == null || _asmrParam.RuningStep == null) return;
            if (_asmrParam.RuningStep.Config.ShowPaths_Enter == null) return;

            for (var i = 0; i < _asmrParam.RuningStep.Config.ShowPaths_Enter.Count; i++)
            {
                var path = _asmrParam.RuningStep.Config.ShowPaths_Enter[i];
                var obj = _asmrParam.Level.transform.Find(path);
                if (obj) obj.gameObject.SetActive(true);
            }
        }
    }
}