using System;
using UnityEngine;

namespace fsm_new
{
    public class AsmrInputHandler_Click : AsmrInputHandler_Base
    {
        private Camera _camera;
        private Action<Vector3> _onTargetClicked;

        private Action<string> _onPlaySound;

        public AsmrInputHandler_Click(Camera camera, Action<Vector3> onTargetClicked, Action<string> onPlaySound)
        {
            _camera = camera;
            _onTargetClicked = onTargetClicked;
            _onPlaySound = onPlaySound;
        }

        public override void Update()
        {
            switch (Input.touchCount)
            {
                case > 0:
                {
                    Touch touch = Input.GetTouch(0);
                    Vector3 touchPos = _camera.ScreenToWorldPoint(touch.position);
                    touchPos.z = 0f;

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            _onTargetClicked(touchPos);
                            _onPlaySound?.Invoke("yx_common_tap");
                            break;
                    }

                    break;
                }
                default:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                        mousePos.z = 0f;
                        _onTargetClicked(mousePos);
                        _onPlaySound?.Invoke("yx_common_tap");
                    }

                    break;
                }
            }
        }
    }
}