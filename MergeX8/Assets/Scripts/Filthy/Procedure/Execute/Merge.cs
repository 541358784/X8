using System;
using Filthy.Game;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Filthy.Procedure
{
    public class Merge : IExecute
    {
        public Transform _root { get; set; }
        public ProcedureBase _procedureBase { get; set; }
        public void Init(Transform root, ProcedureBase procedureBase)
        {
            _root = root;
            _procedureBase = procedureBase;
        }

        public bool Execute()
        {
            _root.transform.Find("Root/SkipButton").gameObject.SetActive(false);
            Action action = () =>
            {
                FilthyGameLogic.Instance._uiProcedureScript?.SetVideoPause(false);
                UIManager.Instance.CloseUI(UINameConst.UIFilthyMergeMain, true);
                FilthyGameLogic.Instance.TriggerProcedure(TriggerType.LevelFinish, _procedureBase._config.ExecuteParam);
            };
            
            FilthyGameLogic.Instance._uiProcedureScript?.SetVideoPause(true);
            UIManager.Instance.CloseUI(UINameConst.UIFilthyMergeMain, true);
            UIManager.Instance.OpenUI(UINameConst.UIFilthyMergeMain, null, int.Parse(_procedureBase._config.ExecuteParam), action);
            return true;
        }
    }
}