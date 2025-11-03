using System;
using UnityEngine;

namespace Framework
{
    public class iPadAdapter : MonoBehaviour
    {
        public float scale;

        private void Awake()
        {
            if (IsLE_16_10())
            {
                transform.localScale = new Vector3(scale, scale, 1);
            }
        }

        public static bool IsLE_16_10()
        {
            float maxR = Mathf.Max(Screen.width, Screen.height);
            float minR = Mathf.Min(Screen.width, Screen.height);
            var ratio = (maxR / minR) <= 1.605f;
            //DebugUtil.LogError($"IsPad {Screen.width} {Screen.height} {isiPad}");
            return ratio;
        }
    }
}