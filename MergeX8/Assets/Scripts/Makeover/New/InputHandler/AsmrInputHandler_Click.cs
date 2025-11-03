using System;
using System.Collections.Generic;
using ASMR;
using UnityEngine;

namespace MiniGame
{
    public class AsmrInputHandler_Click : AsmrInputHandler_Base
    {
        private Camera _camera;
        private Action<Vector3> _onTargetClicked;

        public AsmrInputHandler_Click(Camera camera, Action<Vector3> onTargetClicked)
        {
            _camera = camera;
            _onTargetClicked = onTargetClicked;
        }

        public override void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchPos = _camera.ScreenToWorldPoint(touch.position);
                touchPos.z = 0f;

                if (touch.phase == TouchPhase.Began)
                {
                    _onTargetClicked(touchPos);
                    Model.Instance.PlaySound("yx_common_tap");

                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                _onTargetClicked(mousePos);
                Model.Instance.PlaySound("yx_common_tap");
            }
        }
    }
}