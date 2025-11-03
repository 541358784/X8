using UnityEngine;

namespace MiniGame
{
    public class AsmrInputHandler_Drag : AsmrInputHandler_Base
    {
        private bool isDragging = false;
        private Vector3 offset = Vector3.zero;

        private Camera _camera;

        private Transform _target;

        private RaycastHit2D[] results = new RaycastHit2D[9];

        private System.Action<Vector3> _onClicked;

        
        public AsmrInputHandler_Drag(Transform target, Camera camera)
        {
            _target = target;
            _camera = camera;
        }

        public void SetClickCallback(System.Action<Vector3> onClicked)
        {
            _onClicked = onClicked;
        }

        public override void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchPos = _camera.ScreenToWorldPoint(touch.position);
                touchPos.z = 0f;

                _onClicked.Invoke(touchPos);

            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                    
                _onClicked.Invoke(mousePos);

            }
        }
    }
}