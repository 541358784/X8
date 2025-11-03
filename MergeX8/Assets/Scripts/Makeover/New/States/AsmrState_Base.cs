using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Spine.Unity;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MiniGame
{
    public class AsmrStateParamBase : FsmStateParamBase
    {
        public AsmrLevel Level;
        public AsmrStep RuningStep;

        public AsmrStateParamBase(AsmrLevel level)
        {
            Level = level;
        }

        public AsmrStateParamBase(AsmrLevel level, AsmrStep step)
        {
            Level = level;
            RuningStep = step;
        }
    }

    public class AsmrState_Base : FsmStateBase
    {
        public AsmrStateParamBase _asmrParam;

        protected Collider2D _toolCollider;
        protected Transform _toolTransform;
        protected List<Collider2D> _targetColliders;
        protected Dictionary<int, AsmrStep> _targetStepIdMap;
        protected List<SpinePlayer> _spinePlayers;

        protected bool _toolAttached; //工具是否已挂载到对应的spine骨骼上

        private Vector3 _cameraInitPos;
        private Vector3 _cameraTargePos;
        private float _cameraInitSize;

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
                    if (_asmrParam.RuningStep.Config.autoAttachTool) AttackTool();

                    if (_asmrParam.RuningStep.Config.hidePaths_Enter != null) HideWhenEnter();
                    if (_asmrParam.RuningStep.Config.showPaths_Enter != null) ShowWhenEnter();

                    if (_asmrParam.RuningStep.Config.cameraPos != null && _asmrParam.RuningStep.Config.cameraPos.Length == 2)
                    {
                        _cameraTargePos = new Vector3(_asmrParam.RuningStep.Config.cameraPos[0], _asmrParam.RuningStep.Config.cameraPos[1], _cameraInitPos.z);
                    }

                    UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
                    if (!_asmrParam.RuningStep.Config.textGuide.IsEmptyString())
                    {
                        controller?.ShowGuideText(_asmrParam.RuningStep.Config.textGuide);
                    }
                    else
                    {
                        controller.HideGuideText();
                    }
                }
                
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_spinePlayers != null)
                foreach (var spinePlayer in _spinePlayers)
                    spinePlayer.ClearEvent();

            if (_asmrParam != null && _asmrParam.RuningStep != null && !_asmrParam.RuningStep.Config.tiggerBi.IsEmptyString())
            {
                BiEventAdventureIslandMerge.Types.GameEventType biEvent;
                if(GameBIManager.TryParseGameEventType(_asmrParam.RuningStep.Config.tiggerBi, out biEvent))
                    GameBIManager.Instance.SendGameEvent(biEvent); 
            }
            
            UIGameMainController controller = UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain);
            controller?.HideGuideText();
            
            ResetTool();
        }


        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            if (_spinePlayers != null)
                foreach (var spinePlayer in _spinePlayers)
                    spinePlayer.FixedUpdate(Time.deltaTime);

            //相机移动
            if (_asmrParam != null && _asmrParam.RuningStep != null)
            {
                var t = _asmrParam.ElapsedTime / 0.5f;
                if (_asmrParam.RuningStep.Config.cameraPos != null && _asmrParam.RuningStep.Config.cameraPos.Length > 0)
                {
                    _asmrParam.Level.camera.transform.position = Vector3.Lerp(_cameraInitPos, _cameraTargePos, t);
                }

                if (_asmrParam.RuningStep.Config.cameraSize != 0 && Math.Abs(_asmrParam.RuningStep.Config.cameraSize - _asmrParam.Level.camera.orthographicSize) > 0.001)
                {
                    _asmrParam.Level.camera.orthographicSize = Mathf.Lerp(_cameraInitSize, _asmrParam.RuningStep.Config.cameraSize, t);
                }
            }
        }

        public override string ToString()
        {
            if (_asmrParam != null)
            {
                if (_asmrParam.RuningStep != null)
                {
                    return $"{GetType()} StepID:{_asmrParam.RuningStep.Config.id}";
                }
            }

            return base.ToString();
        }

        private void ShowTool_Enter()
        {
            if (_asmrParam.RuningStep == null) return;
            if (string.IsNullOrEmpty(_asmrParam.RuningStep.Config.toolPath)) return;

            var node = _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.toolPath);
            node.gameObject.SetActive(true);

            UpAllToolNodeLayer(node);
        }

        protected void UpAllToolsLayer()
        {
            if (_asmrParam.RuningStep == null) return;
            if (string.IsNullOrEmpty(_asmrParam.RuningStep.Config.toolPath)) return;

            var node = _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.toolPath);
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
            if (string.IsNullOrEmpty(_asmrParam.RuningStep.Config.toolPath)) return;

            var node = _asmrParam.Level.transform.Find(_asmrParam.RuningStep.Config.toolPath);
            if (!node) return;

            var renders = node.GetComponentsInChildren<Renderer>();
            if (renders != null)
            {
                foreach (var renderer in renders)
                {
                    if (_asmrParam.Level.ToolsLayerDic.TryGetValue(renderer, out var layer))
                    {
                        renderer.sortingOrder = layer;
                    }
                }
            }
        }

        private string InitTarget(AsmrStep step)
        {
            var toolPath = string.Empty;

            if (step == null) return toolPath;

            if (!string.IsNullOrEmpty(step.Config.toolPath)) toolPath = step.Config.toolPath;

            if (step.Config.target == null) return toolPath;

            var target = _asmrParam.Level.transform.Find(step.Config.target);
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
                    Debug.Log($"Target Path 未挂载碰撞体:{step.Config.target}");
                }
            }
            else
            {
                Debug.LogError($"Target Path 未找到:{step.Config.target}");
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
            // if (_asmrParam.Level.CurrentGroup.Config.fixedOrder)
            // {
            //     toolPath = InitTarget(_asmrParam.RuningStep);
            // }
            // else
            // {
            //     foreach (var step in _asmrParam.Level.CurrentGroup.Steps)
            //     {
            //         if (step.Finish) continue;
            //         var tempToolPath = InitTarget(step);
            //         if (!string.IsNullOrEmpty(tempToolPath)) toolPath = tempToolPath;
            //     }
            // }

            if (_asmrParam.RuningStep != null) toolPath = _asmrParam.RuningStep.Config.toolPath;
            if (!string.IsNullOrEmpty(toolPath))
            {
                _toolTransform = _asmrParam.Level.transform.Find(toolPath);
                if (_toolTransform != null)
                {
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
                        if (_asmrParam.RuningStep != null && _asmrParam.RuningStep.Config.actionType == (int)AsmrActionType.Drag)
                        {
                            Debug.LogError($"dragToolPath:{toolPath}  has no collider2d");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"dragToolPath:{toolPath}  not found");
                }
            }
        }

        private void InitSpinePlayers()
        {
            if (_asmrParam.RuningStep == null) return;
            if (_asmrParam.RuningStep.Config.spinePath == null) return;

            _spinePlayers = new List<SpinePlayer>();
            for (var index = 0; index < _asmrParam.RuningStep.Config.spinePath.Length; index++)
            {
                var path = _asmrParam.RuningStep.Config.spinePath[index];
                var spineTransform = _asmrParam.Level.transform.Find(path);
                if (spineTransform != null)
                {
                    var spine = spineTransform.GetComponent<SkeletonAnimation>();
                    spine.UpdateCullMode = false;
                    var player = new SpinePlayer(spine, _asmrParam.RuningStep.Config.spineAnimName[index]);
                    _spinePlayers.Add(player);
                }
                else
                {
                    Debug.LogError($"Spine Path没有找到{path}");
                }
            }
        }

        protected void AttackTool()
        {
            if (_asmrParam.RuningStep == null) return;

            ResetAllToolsLayer();

            if (!string.IsNullOrEmpty(_asmrParam.RuningStep.Config.toolAttachPath))
            {
                var toolParent = _asmrParam.Level.transform.Find($"{_asmrParam.RuningStep.Config.toolAttachPath}");
                _toolTransform.SetParent(toolParent);
                _toolTransform.localPosition = Vector3.zero;
                if (_asmrParam.RuningStep.Config.toolResetRotation) _toolTransform.localRotation = Quaternion.identity;
                if (_asmrParam.RuningStep.Config.toolResetScale) _toolTransform.localScale = Vector3.one;

                _toolAttached = true;
            }
        }

        protected void ResetTool()
        {
            if (_asmrParam.RuningStep == null) return;

            if (!string.IsNullOrEmpty(_asmrParam.RuningStep.Config.toolAttachPath))
            {
                _toolTransform.SetParent(_asmrParam.Level.transform);
                _toolAttached = false;
                _toolTransform.gameObject.SetActive(false);
            }
        }

        protected void ShowTargets(bool show)
        {
            if (_targetColliders != null)
                foreach (var targetCollider in _targetColliders)
                    targetCollider.gameObject.SetActive(show);
        }

        private void HideWhenEnter()
        {
            if (_asmrParam != null && _asmrParam.RuningStep != null)
            {
                if (_asmrParam.RuningStep.Config.hidePaths_Enter != null)
                {
                    for (var i = 0; i < _asmrParam.RuningStep.Config.hidePaths_Enter.Length; i++)
                    {
                        var path = _asmrParam.RuningStep.Config.hidePaths_Enter[i];
                        var obj = _asmrParam.Level.transform.Find(path);
                        if (obj) obj.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void ShowWhenEnter()
        {
            if (_asmrParam != null && _asmrParam.RuningStep != null)
            {
                if (_asmrParam.RuningStep.Config.showPaths_Enter != null)
                {
                    for (var i = 0; i < _asmrParam.RuningStep.Config.showPaths_Enter.Length; i++)
                    {
                        var path = _asmrParam.RuningStep.Config.showPaths_Enter[i];
                        var obj = _asmrParam.Level.transform.Find(path);
                        if (obj) obj.gameObject.SetActive(true);
                    }
                }
            }
        }
        


    }
}