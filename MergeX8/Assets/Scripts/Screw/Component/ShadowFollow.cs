using UnityEngine;

namespace Screw
{
    public class ShadowFollow : MonoBehaviour
    {
        private Vector3 originalPos;
        private Vector3 originalOffset;

        public Transform panel;

        public void SetOriginalPos(Vector3 inOriginalPos)
        {
            originalPos = panel.position;
            transform.position = inOriginalPos;
            originalOffset = panel.position - inOriginalPos;
        }

        private void LateUpdate()
        {
            if (originalOffset != Vector3.zero && originalPos != panel.position)
            {
                transform.position = panel.position - originalOffset;
                originalPos = panel.position;
            }
        }
    }
}