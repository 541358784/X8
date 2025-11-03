using System;
using System.Collections.Generic;
using Decoration;
using Framework;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace StoryMovie
{
    public abstract class ActionBase
    {
        protected GameObject _movieObject;
        protected TableStoryMovie _config;
        protected Animator _animator;
        protected SkeletonAnimation _skeleton;
        protected bool _isStop;
        protected bool _isStart;
        protected bool _isUpdate = true;
        protected float _time;
        protected List<Vector3> _movePosition;
        protected Transform _orgParent;
        protected float _crossFade = 0;
        
        public void OnInit(TableStoryMovie config)
        {
            _config = config;
            _isUpdate = true;
            _isStart = false;
            _isStop = false;
            _time = 0;
            
            ParsingParam();
            InitMovieObject();
            BuildLink();
            UILogic(false);
            Init();
            
            
            OnUpdate();
        }
        
        protected virtual void InitMovieObject()
        {
            if(_config == null)
                return;

            switch (_config.controlType)
            {
                //主角
                case 1:
                {
                    _movieObject = PlayerManager.Instance.GetPlayer(PlayerManager.PlayerType.Chief);
                    _animator = PlayerManager.Instance.GetAnimator(PlayerManager.PlayerType.Chief);
                    
                    if(_config.stopStatus)
                        PlayerManager.Instance.StopPlayerStatus(PlayerManager.PlayerType.Chief);
                    break;
                }
                //小狗
                case 2:
                {
                    _movieObject = PlayerManager.Instance.GetPlayer(PlayerManager.PlayerType.Dog);
                    _animator = PlayerManager.Instance.GetAnimator(PlayerManager.PlayerType.Dog);
                    
                    if(_config.stopStatus)
                        PlayerManager.Instance.StopPlayerStatus(PlayerManager.PlayerType.Dog);
                    break;
                }
                //男主
                case 4:
                {
                    _movieObject = PlayerManager.Instance.GetPlayer(PlayerManager.PlayerType.Hero);
                    _animator = PlayerManager.Instance.GetAnimator(PlayerManager.PlayerType.Hero);
                    
                    if(_config.stopStatus)
                        PlayerManager.Instance.StopPlayerStatus(PlayerManager.PlayerType.Hero);
                    break;
                }
                //摄像机
                case 3:
                {
                    _movieObject = DecoSceneRoot.Instance.mSceneCamera.gameObject;
                    break;
                }
                //场景对象
                case 10:
                {
                    try
                    {
                        _movieObject = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find(_config.controlNames).gameObject;
                        _animator = _movieObject.GetComponent<Animator>();
                        _skeleton = _movieObject.GetComponentInChildren<SkeletonAnimation>();
                    }
                    catch (Exception e)
                    {
                       Debug.LogError("Obj is Null " + _config.id + "\t" + _config.controlNames);
                    }
                    break;
                }
            }
        }
        public void OnUpdate()
        {
            if(_config == null)
                return;
            
            if(_isStop)
                return;

            _time += Time.deltaTime;
            
            if (_isUpdate)
            {
                if(_time < _config.timeLine)
                    return;
                
                Start();
                _time = 0;
                _isUpdate = false;
                _isStart = true;
            }

            if (_isStart)
            {
                if(_time < _config.movieTime)
                    return;

                OnStop();
                _time = 0;
                _isStart = false;
            }
        }

        protected void PlayAnimation()
        {
            if(_config == null || _config.animationName.IsEmptyString())
                return;
            
            if(_animator != null)
                _animator.CrossFade(_config.animationName, _crossFade, 0);

            if (_skeleton != null)
            {
                _skeleton.AnimationState?.SetAnimation(0, _config.animationName, _config.isLoop);
                _skeleton.Update(0);
            }
            
        }

        protected void BuildLink()
        {
            if (_config.linkPath.IsEmptyString() || _movieObject == null)
                return;
            
            var parent = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find(_config.linkPath);
            if(parent == null)
                return;

            _orgParent = _movieObject.transform.parent;
           _movieObject.transform.SetParent(parent);
        }

        protected void InitPosition()
        {
            if(_movieObject == null)
                return;
            
            _movieObject.gameObject.SetActive(true);
            
            if(_config.position != null && _config.position.Length == 3)
                _movieObject.transform.localPosition = new Vector3(_config.position[0], _config.position[1], _config.position[2]);
              
            if(_config.rotation != null && _config.rotation.Length == 3)
                _movieObject.transform.rotation = Quaternion.Euler(new Vector3(_config.rotation[0], _config.rotation[1], _config.rotation[2]));
        }
        
        protected void RescindLink()
        {
            if (_config.linkPath.IsEmptyString() || _movieObject == null)
                return;
            
            _movieObject.transform.SetParent(_orgParent);
        }
        
        public void OnStop()
        {
            _isStop = true;
            Stop();
        }

        public virtual void OnExit()
        {
            UILogic(true);

            RescindLink();
            
            if(_config.controlType == 1 && _config.restoreStatus)
                PlayerManager.Instance.UpdatePlayersState();
        }
        
        public bool IsStop()
        {
            return _isStop;
        }
        
        protected void ParamToVector3DList()
        {
            if(_config == null)
                return;

            if(_config.actionParam.IsEmptyString())
                return;
            
            _movePosition = new List<Vector3>();
            var srPos = _config.actionParam.Split(',');

            Vector3 position = Vector3.zero;
            if(srPos.Length >= 3)
                position = new Vector3(float.Parse(srPos[0]), float.Parse(srPos[1]), float.Parse(srPos[2]));
            else if(srPos.Length >= 2)
                position = new Vector3(float.Parse(srPos[0]), float.Parse(srPos[1]), 0);
            else if(srPos.Length >= 1)
                position = new Vector3(float.Parse(srPos[0]), 0, 0);
            
            _movePosition.Add(position);
        }

        protected void ParamToFloat()
        {
            if(_config == null)
                return;

            if(_config.actionParam.IsEmptyString())
                return;

            float.TryParse(_config.actionParam, out _crossFade);
        }

        protected void UILogic(bool isRestore)
        {
            if(_config.uiLogic == null || _config.uiLogic.Length == 0)
                return;

            if(_config.uiLogic[0] != 1 && _config.uiLogic[0] != 2)
                return;

            if(isRestore && !_config.restoreUI)
                return;
            
            bool isShow = _config.uiLogic[0] == 1 ? false : true;
            isShow = isRestore ? !isShow : isShow;

            bool isAnim = true;
            if (!isRestore && _config.uiLogic.Length >= 2)
                isAnim = _config.uiLogic[1] == 0;
            
            UIManager.Instance.SetCanvasGroupAlpha(isShow, isAnim);
        }
        
        protected abstract void Init();
        protected abstract void Start();
        protected abstract void Stop();
        protected abstract void ParsingParam();
    }
}