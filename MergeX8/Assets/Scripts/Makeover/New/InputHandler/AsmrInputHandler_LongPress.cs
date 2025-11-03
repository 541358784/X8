using UnityEngine;

namespace MiniGame
{
    public class AsmrInputHandler_LongPress : AsmrInputHandler_Base
    {
        public float totalTimer;

        private bool _startVibration;

        public override void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                totalTimer += Time.deltaTime;
            }

            return;
#endif

            if (Input.touchCount == 2)
            {
                totalTimer += Time.deltaTime;
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            // if (Input.touchCount == 2)
            // {
            //     AsmrLevel.VibrationShortFix();
            // }
        }
    }
}