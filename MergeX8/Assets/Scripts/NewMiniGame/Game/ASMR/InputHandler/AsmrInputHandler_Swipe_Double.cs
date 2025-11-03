using UnityEngine;

namespace fsm_new
{
    public class AsmrInputHandler_Swipe_Double : AsmrInputHandler_Base
    {
        private Vector2 initialTouch1Pos;
        private Vector2 initialTouch2Pos;
        private float initialDistance;
        private float _totalDistance;

#if UNITY_EDITOR
        private AsmrInputHandler_Swipe_Single _singleInputWithEditor;
#endif

        public AsmrInputHandler_Swipe_Double()
        {
#if UNITY_EDITOR
            _singleInputWithEditor = new AsmrInputHandler_Swipe_Single();
#endif
        }

        public float totalDistance
        {
            get
            {
#if UNITY_EDITOR
                return _singleInputWithEditor.totalDistance;
#endif

                return _totalDistance;
            }

            private set { _totalDistance = value; }
        }

        public override void Update()
        {
#if UNITY_EDITOR
            _singleInputWithEditor.Update();
            return;
#endif

            switch (Input.touchCount)
            {
                case 2:
                {
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);

                    switch (touch1.phase)
                    {
                        case TouchPhase.Began when touch2.phase == TouchPhase.Began:
                            initialTouch1Pos = touch1.position;
                            initialTouch2Pos = touch2.position;
                            initialDistance = Vector2.Distance(initialTouch1Pos, initialTouch2Pos);
                            break;
                        case TouchPhase.Moved when touch2.phase == TouchPhase.Moved:
                        {
                            Vector2 currentTouch1Pos = touch1.position;
                            Vector2 currentTouch2Pos = touch2.position;
                            float currentDistance = Vector2.Distance(currentTouch1Pos, currentTouch2Pos);

                            float deltaDistance = currentDistance - initialDistance;
                            _totalDistance += Mathf.Abs(deltaDistance);

                            initialTouch1Pos = currentTouch1Pos;
                            initialTouch2Pos = currentTouch2Pos;
                            initialDistance = currentDistance;
                            break;
                        }
                    }

                    break;
                }
            }

// Debug.Log("总距离: " + totalDistance);
        }


        private bool _startVibration;

        public void FixedUpdate(float deltaTime)
        {
            // if (Input.touchCount == 2)
            // {
            //     AsmrLevel.VibrationShortFix();
            // }
        }
    }
}