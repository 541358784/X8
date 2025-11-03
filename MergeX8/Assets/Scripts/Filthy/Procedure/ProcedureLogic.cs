using System.Collections.Generic;
using DragonPlus.Config.Filthy;
using UnityEngine;

namespace Filthy.Procedure
{
    public class ProcedureLogic
    {
        private Transform _root;
        private List<ProcedureBase> _procedureBases = new List<ProcedureBase>();

        private ProcedureBase _currentProcedure;
        
        public ProcedureLogic(int levelId, Transform root)
        {
            _procedureBases.Clear();

            var procedures = FilthyConfigManager.Instance.GetFilthyProcedures(levelId);
            if(procedures == null)
                return;
            
            foreach (var filthyProcedure in procedures)
            {
                _procedureBases.Add(new ProcedureBase(root, filthyProcedure, procedures[procedures.Count-1] == filthyProcedure));
            }
            
            foreach (var procedureBase in _procedureBases)
            {
                _currentProcedure = procedureBase;
                if(_currentProcedure._triggerType != TriggerType.Time)
                    break;
                
                _currentProcedure.Trigger(TriggerType.Time, _currentProcedure._config.TriggerParam);
            }
        }

        public void TriggerProcedure(TriggerType type, string param)
        {
            int index = _procedureBases.FindIndex(a => a == _currentProcedure);
            for (var i = 0; i < _procedureBases.Count; i++)
            {
                if(index > i)
                    continue;

                _currentProcedure = _procedureBases[i];
                _currentProcedure.Trigger(type, param);
                if(!_currentProcedure.IsTrigger())
                    break;
                
                _currentProcedure.Update();
            }
        }

        public bool IsProcedureLevel()
        {
            return _procedureBases.Count > 0;
        }

        public void Update()
        {
            _procedureBases.ForEach(a=>a.Update());
        }

        public void SkipProcedure(ProcedureBase procedureBase)
        {
            procedureBase.SkipTrigger();
           var index =  _procedureBases.FindIndex(a => a == procedureBase);
           if(index >= _procedureBases.Count-1)
               return;
           
           if( _procedureBases[index+1]._triggerType != TriggerType.Time)
               return;
                
           _procedureBases[index+1].SkipTrigger();
        }
    }
}