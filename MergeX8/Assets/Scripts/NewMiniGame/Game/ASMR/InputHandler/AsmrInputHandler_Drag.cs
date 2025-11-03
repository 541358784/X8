using UnityEngine;

namespace fsm_new
{
    public class AsmrInputHandler_Drag : AsmrInputHandler_Base
    {
        private bool isDragging = false;

        private Camera _camera;


        private System.Action<Vector3> _onClicked;


        public AsmrInputHandler_Drag(Camera camera)
        {
            _camera = camera;
        }

        public void SetClickCallback(System.Action<Vector3> onClicked)
        {
            _onClicked = onClicked;
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

                    _onClicked.Invoke(touchPos);
                    break;
                }
                default:
                {
                    if (Input.GetMouseButton(0))
                    {
                        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                        mousePos.z = 0f;

                        _onClicked.Invoke(mousePos);
                    }

                    break;
                }
            }
        }
    }
}