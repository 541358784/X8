using UnityEngine;

namespace fsm_new
{
    public class AsmrState_Drag : AsmrState_Base
    {
        private AsmrInputHandler_Drag _inputHandler_Drag;

        private Transform _target;
        private string _animName = "hold";


        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _inputHandler_Drag = new AsmrInputHandler_Drag(_asmrParam.Level.camera);
            _inputHandler_Drag.SetClickCallback(OnPointClick);


            var handTip = _toolTransform;
            var startPos = handTip.transform.position;

            var step = _asmrParam.Level.CurrentGroup.Steps.Find(s => !s.Finish);
            _target = _asmrParam.Level.transform.Find(step.Config.Target);

            _asmrParam.Level.PlayDragTip(startPos, _target.position, _animName, _asmrParam.RuningStep.Config.TipSize);
        }

        public override void OnExit()
        {
            base.OnExit();

            _inputHandler_Drag = null;

            _asmrParam.Level.StopDragTip();
        }

        public override void Update()
        {
            base.Update();

            _inputHandler_Drag.Update();
            // _inputHandler_Click.Update();

            if (AsmrInputHandler_Base.TouchEnd)
            {
                if (CheckReachTarget2()) return;
                if (_asmrParam.Level.TipShowing()) return;

                var handTip = _toolTransform;
                var startPos = handTip.transform.position;
                _asmrParam.Level.PlayDragTip(startPos, _target.position, _animName, _asmrParam.RuningStep.Config.TipSize);
            }
            else
            {
                _input_once_started = true;
                _asmrParam.Level.StopDragTip();
            }
        }

        private bool _input_once_started;

        private bool CheckReachTarget2()
        {
            if (!_input_once_started) return false;


            var filter = new ContactFilter2D();
            filter.NoFilter();

            Collider2D[] results = new Collider2D[9];
            var count = _toolCollider.OverlapCollider(filter, results);
            if (count <= 0) return false;
            //Debug.Log("拖动到位");
            foreach (var collider2D in results)
            {
                if (!collider2D) continue;
                if (!collider2D.gameObject) continue;

                if (!_targetStepIdMap.TryGetValue(collider2D.gameObject.GetHashCode(), out var step)) continue;

                ShakeManager.Instance.ShakeLight();
                _asmrParam.fsm.ChangeState<AsmrState_PlaySpine>(new AsmrStateParamBase(_asmrParam.Level, step, _asmrParam.fsm));

                collider2D.gameObject.SetActive(false);
                return true;
            }

            return false;
        }

        private void OnPointClick(Vector3 position)
        {
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
    }
}