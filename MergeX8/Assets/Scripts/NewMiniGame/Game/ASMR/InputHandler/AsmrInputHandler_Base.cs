using UnityEngine;

namespace fsm_new
{
    public abstract class AsmrInputHandler_Base
    {
        public abstract void Update();

        public static bool TouchEnd => Input.touchCount == 0 && !Input.GetMouseButton(0);
    }
}