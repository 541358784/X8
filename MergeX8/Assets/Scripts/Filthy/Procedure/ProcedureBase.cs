using DragonPlus;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

namespace Filthy.Procedure
{
    public enum ExecuteType
    {
        None = 0,
        PlayVideo,
        ObjectActive,
        Screw,
        Filthy,
        Merge,
    }

    public enum TriggerType
    {
        None = 0,
        Time,
        Click,
        LevelFinish,
    }
    
    public class ProcedureBase
    {
        public TriggerType _triggerType;
        public ExecuteType _executeType;
        public FilthyProcedure _config;

        protected bool _isFinsh = false;

        protected IExecute _execute;
        protected ITrigger _trigger;
        protected Transform _root;

        private bool _isLast = false;
        
        public ProcedureBase(Transform root, FilthyProcedure config, bool isLast)
        {
            _root = root;
            _config = config;
            _triggerType = (TriggerType)_config.TriggerType;
            _executeType = (ExecuteType)_config.ExecuteType;
            _isLast = isLast;
            
            InitExecute();
            InitTrigger();
        }

        public virtual void Update()
        {
            if(_isFinsh)
                return;
            
            _trigger.Update();

            if (_trigger._isFinish && _trigger._isTrigger)
            {
                //Debug.LogError("----------------- execute " + _config.Id);
                _isFinsh = true;
                _execute.Execute();

                if (_config.Bi != null && _config.Bi.Count > 0)
                {
                    foreach (var id in _config.Bi)
                    {
                        GameBIManager.Instance.SendGameEvent((BiEventAdventureIslandMerge.Types.GameEventType)id);
                    }
                }
                
                if (_isLast)
                {
                    SceneFsm.mInstance.ChangeState(StatusType.BackHome);
                }
            }
        }

        public virtual bool IsFinish()
        {
            return _trigger._isFinish;
        }

        public virtual void Trigger(TriggerType type, string param)
        {
            if(_isFinsh)
                return;
            
            if(_triggerType != TriggerType.Time && type != _triggerType)
                return;
            
            if(_trigger._isTrigger || _trigger._isFinish)
                return;
            
            _trigger.Trigger(param);
        }
        
        public virtual bool IsTrigger()
        {
            return _trigger._isTrigger;
        }

        public void SkipTrigger()
        {
            _trigger._isFinish = true;
            _trigger._isTrigger = true;
        }
        
        private void InitExecute()
        {
            switch (_executeType)
            {
                case ExecuteType.PlayVideo:
                {
                    _execute = new PlayVideo();
                    break;
                }
                case ExecuteType.ObjectActive:
                {
                    _execute = new ObjectActive();
                    break;
                }
                case ExecuteType.Screw:
                {
                    _execute = new Screw();
                    break;
                }
                case ExecuteType.Filthy:
                {
                    _execute = new Filthy();
                    break;
                }
                case ExecuteType.None:
                {
                    _execute = new None();
                    break;
                }
                case ExecuteType.Merge:
                {
                    _execute = new Merge();
                    break;
                }
            }
            
            _execute.Init(_root, this);
        }

        private void InitTrigger()
        {
            switch (_triggerType)
            {
                case TriggerType.Time:
                {
                    _trigger = new Time();
                    break;
                }
                case TriggerType.Click:
                {
                    _trigger = new Click();
                    break;
                }
                case TriggerType.LevelFinish:
                {
                    _trigger = new LevelFinish();
                    break;
                }
            }
            
            _trigger.Init(_config.TriggerParam);
        }
    }
}