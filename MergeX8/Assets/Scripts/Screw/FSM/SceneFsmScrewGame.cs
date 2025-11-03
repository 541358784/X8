using System;
using Decoration;
using Decoration.DynamicMap;
using Screw.GameLogic;
using Screw.Module;
using UnityEngine;

namespace Screw
{
    public class SceneFsmScrewGame : IFsmState
    {
        public StatusType Type => StatusType.ScrewGame;

        private float _worldCameraSize;
        private float _uiCameraSize;
        private Vector3 _worldCameraPosition;
        private Vector3 _uiCameraPosition;

        private int _levelId;
        private Action<object> _action;
        
        public void Enter(params object[] objs)
        {
            _levelId= (int)objs[0];
            _action = null;
            if (objs.Length >= 2)
                _action = (Action<object>)objs[1];
            var sendBi = true;
            if (objs.Length >= 3)
                sendBi = (bool)objs[2];
            DecoSceneRoot.Instance.SetPhysics2DRaycasterEnable(true);
            
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
            InitWorldCamera();
            
            ScrewGameLogic.Instance.EnterScrewGame(_levelId, _action,sendBi);
        }

        public void RePlay(bool sendBi = true)
        {
            ScrewGameLogic.Instance.ExitGame();
            ScrewGameLogic.Instance.EnterScrewGame(_levelId, _action,sendBi);
        }
        
        public void Update(float deltaTime)
        {
        }

        public void LateUpdate(float deltaTime)
        {
        }

        public void Exit()
        {
            RestWorldCamera();
            ScrewGameLogic.Instance.ExitGame();
            Physics2D.simulationMode = SimulationMode2D.Script;
            DecoSceneRoot.Instance.SetPhysics2DRaycasterEnable(false);
        }

        private void InitWorldCamera()
        {
            _worldCameraSize = UIModule.Instance.WorldCamera.orthographicSize;
            _worldCameraPosition = UIModule.Instance.WorldCamera.transform.position;
            _uiCameraSize = UIModule.Instance.UICamera.orthographicSize;
            _uiCameraPosition = UIModule.Instance.UICamera.transform.position;
            
            UIModule.Instance.WorldCamera.orthographicSize = ScrewGameLogic.Instance.WorldCameraOrthographicSize;
            UIModule.Instance.WorldCamera.transform.position = ScrewGameLogic.Instance.WorldCameraPosition;
            
            UIModule.Instance.UICamera.orthographicSize = ScrewGameLogic.Instance.WorldCameraOrthographicSize;
            UIModule.Instance.UICamera.transform.position = ScrewGameLogic.Instance.WorldCameraPosition;
        }

        private void RestWorldCamera()
        {
            UIModule.Instance.WorldCamera.orthographicSize = _worldCameraSize;
            UIModule.Instance.WorldCamera.transform.position = _worldCameraPosition;
            
            UIModule.Instance.UICamera.orthographicSize = _uiCameraSize;
            UIModule.Instance.UICamera.transform.position = _uiCameraPosition;
        }
    }
}