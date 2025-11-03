using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Filthy.Game;
using Screw;
using UnityEngine;

namespace Filthy.Procedure
{
    public class Screw : IExecute
    {
        public Transform _root { get; set; }
        public ProcedureBase _procedureBase { get; set; }

        private SceneFsmScrewGame _subScrewGame;
        
        public void Init(Transform root, ProcedureBase procedureBase)
        {         
            _root = root;
            _procedureBase = procedureBase;
        }

        public bool Execute()
        {
            _root.transform.Find("Root/SkipButton").gameObject.SetActive(false);
            // FilthyGameLogic.Instance.SetFithyUIActive(false);
            Action<object> action = (b) =>
            {
                if ((bool)b)
                {
                    FilthyGameLogic.Instance._uiProcedureScript?.HideScrewView();
                    FilthyGameLogic.Instance.SetFithyUIActive(true);
                    _subScrewGame.Exit();
                    FilthyGameLogic.Instance.TriggerProcedure(TriggerType.LevelFinish, _procedureBase._config.ExecuteParam);
                }
                else
                {
                    _subScrewGame.RePlay(false);   
                }
            };

            if (_procedureBase._config.ExecuteParam == "990001")
            {
                _subScrewGame = new SceneFsmScrewGame();
                _subScrewGame.Enter(int.Parse(_procedureBase._config.ExecuteParam), action,false);
                FilthyGameLogic.Instance._uiProcedureScript?.ShowScrewView();
            }
            else
            {
                // TMatch.UILoadingEnter.Open(() =>
                // {
                //     _subScrewGame = new SceneFsmScrewGame();
                //     _subScrewGame.Enter(int.Parse(_procedureBase._config.ExecuteParam), action);
                //     FilthyGameLogic.Instance._uiProcedureScript?.ShowScrewView();
                // });
                _subScrewGame = new SceneFsmScrewGame();
                _subScrewGame.Enter(int.Parse(_procedureBase._config.ExecuteParam), action,false);
                FilthyGameLogic.Instance._uiProcedureScript?.ShowScrewView();
            }
            
            return true;
        }
    }
}