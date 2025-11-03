using Coffee.UIExtensions;
using UnityEngine;

namespace Framework
{
    public class UITransformScaleAdapter : MonoBehaviour
    {
        public float min;
        public float max;

        private void OnEnable()
        {
            var t = InverseLerpUnclamped((float)1365 / 768, (float)1720 / 768, (float)Screen.height / Screen.width);
            var value = Mathf.LerpUnclamped(min, max, t);

            if (value > 0.96f)
                value = 1f;
            
            transform.localScale = Vector3.one * value;
        }

        private static float InverseLerpUnclamped(float a, float b, float value)
        {
            if (b - a == 0) return 0;
            return (value - a) / (b - a);
        }
    }
}